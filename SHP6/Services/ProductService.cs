using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SHP6.DAL;
using SHP6.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Services
{
    public class ProductService : IProductService
    {
        private readonly NewDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(NewDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddProducts(Product product) //Add and saves products to DB
        {
            product.Pic1 = SaveImage(product.NotMappedPic1);
            product.Pic2 = SaveImage(product.NotMappedPic2);
            product.Pic3 = SaveImage(product.NotMappedPic3);

            product.Date = DateTime.Now;
            product.State = 1;
            product.Owner = RetrieveUserFromCookies();
            _context.Products.Add(product);
            _context.SaveChanges();
        }
        public void AddProductToCart(Product product) //Adds item to cart, if user not signed in guest cookies are created and product ID is stored
        {
            Product fullProductDetails = GetProductById(product.Id);
            product.Date = DateTime.Now;
            fullProductDetails.User = RetrieveUserFromCookies();

            if (fullProductDetails.User != null)
            {
                fullProductDetails.State = (int)StateOfItem.InCart;
                _context.SaveChanges();
            }
            else
            {
                if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Cookies["AspProjectGuestInfo"]))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("AspProjectGuestInfo", $"{product.Id}", new CookieOptions() { Expires = (DateTime.Now).AddMinutes(15) });
                }
                else
                {
                    string guestCookiesIds = _httpContextAccessor.HttpContext.Request.Cookies["AspProjectGuestInfo"];
                    if (SplitGuestCookie()[0] != null)
                    {
                        foreach (var item in SplitGuestCookie())
                        {
                            if (item.Id != product.Id)
                            {
                                guestCookiesIds += $",{product.Id}";
                            }
                        }
                    }
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("AspProjectGuestInfo", guestCookiesIds, new CookieOptions() { Expires = (DateTime.Now).AddMinutes(15) });
                }
                fullProductDetails.State = (int)StateOfItem.InCart;
                _context.SaveChanges();
            }
        }
        public void RemoveFromCart(int id) //Removes item from cart, sets "User" equal to null
        {
            Product product = GetProductById(id);
            product.User = null;
            product.State = (int)StateOfItem.Available;
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public void Checkout() //Checks to see what kind of cookies exist, changes "ItemState" to null
        {
            List<Product> userCart = _context.Products.Include(p => p.Owner).Include(p => p.User).Where(p => p.User == RetrieveUserFromCookies() && p.State == (int)StateOfItem.InCart).ToList();
            if (userCart.Count > 0)
            {
                foreach (var item in userCart)
                {
                    item.State = (int)StateOfItem.Sold;
                }
                userCart.Clear();//
            }
            else
            {
                SplitGuestCookie();
                if (SplitGuestCookie()[0] != null)
                {
                    foreach (var item in SplitGuestCookie())
                    {
                        item.State = (int)StateOfItem.Sold;
                    }
                    SplitGuestCookie().Clear();//
                }
            }
            _context.SaveChanges();
        }
        public void ScanStore() //Scans the store to see if a product is "InCart" but "User" is null
        {
            if (!DoesGuestExist())
            {
                List<Product> guestProductsList = _context.Products.Include(p => p.Owner).Include(p => p.User).Where(p => p.User == null && p.State == (int)StateOfItem.InCart).ToList();
                foreach (var item in guestProductsList)
                {
                    item.State = (int)StateOfItem.Available;
                }
            }
        }

        public List<Product> SortBy(string sortBy)
        {
            if (sortBy == "Title")
            {
                return _context.Products.OrderBy(ob => ob.Title).ToList();
            }
            else
            {
                return _context.Products.OrderBy(ob => ob.Date).ToList();
            }
        }
        public IEnumerable<Product> GetCartUser()
        {
            return _context.Products.Include(p => p.Owner).Include(p => p.User).Where(p => p.User == RetrieveUserFromCookies() && p.State == (int)StateOfItem.InCart).ToList();
        }
        public IEnumerable<Product> GetCartGuest()
        {
            string guestId = RetrieveGuestId();
            if (guestId != null)
            {
                return SplitGuestCookie();
            }
            return null;
        }
        public Product GetProductById(int id)
        {
            Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            return product;
        }

        public string RetrieveGuestId() //Retireves guest ID from cookies
        {
            string cookieGuestValue = _httpContextAccessor.HttpContext.Request.Cookies["AspProjectGuestInfo"];
            if (string.IsNullOrEmpty(cookieGuestValue))
            {
                return null;
            }
            return cookieGuestValue;
        }

        private ApplicationUser RetrieveUserFromCookies() //Retrieves "User" from cookies
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["AspProjectCookie"];
            if (string.IsNullOrEmpty(cookieValue))
            {
                return null;
            }
            string[] cookiesArray = cookieValue.Split(',');
            ApplicationUser aUser = _context.Users.FirstOrDefault(u => u.UserName == cookiesArray[0] && u.Password == cookiesArray[1]);
            return aUser;
        }
        private List<Product> SplitGuestCookie() //Splits cookie ID
        {
            string[] guestIdArray = RetrieveGuestId().Split(',');
            List<Product> guestProductsList = new List<Product>();
            foreach (var item in guestIdArray)
            {
                int id = int.Parse(item);
                guestProductsList.Add(_context.Products.Include(p => p.Owner).Include(p => p.User).FirstOrDefault(p => p.Id == id && p.State == (int)StateOfItem.InCart));
            }
            return guestProductsList;
        }
        private string SaveImage(IFormFile file) //Save and store image through conversion to "Base64String"
        {
            if (file == null)
            {
                return null;
            }
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        private bool DoesGuestExist() //Checks to see if guest cookies were created
        {
            string cookieGuestValue = _httpContextAccessor.HttpContext.Request.Cookies["AspProjectGuestInfo"];
            if (string.IsNullOrEmpty(cookieGuestValue))
            {
                return false;
            }
            return true;
        }
        enum StateOfItem
        {
            Available = 1,
            InCart = 2,
            Sold = 4
        }
    }
}

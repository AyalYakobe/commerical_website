using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SHP6.Models;
using SHP6.Services;

namespace SHP6.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productLogic;
        private readonly IUserService _userLogic;

        public ProductController(IProductService productLogic, IUserService userLogic)
        {
            _productLogic = productLogic;
            _userLogic = userLogic;
        }

        public IActionResult RegisterProduct()
        {
            ViewData["CurrentPage"] = "EnterItem";
            return View();
        }
        public IActionResult Cart() //Retireves different items depending on cookies
        {
            ViewData["CurrentPage"] = "Cart";
            if (_userLogic.RetrieveUserFromCookies() != null)
            {
                ViewData["CartUser"] = _productLogic.GetCartUser();
            }
            else
            {
                ViewData["CartGuest"] = _productLogic.GetCartGuest();
            }
            return View();
        }
        public IActionResult Thanks()
        {
            return View();
        }
        public IActionResult MoreDetails(int id)
        {
            return View(_productLogic.GetProductById(id));
        }

        public IActionResult AddToCart(Product product)
        {
            _productLogic.AddProductToCart(product);
            return RedirectToAction("HomePage", "Home");
        }
        public IActionResult RemoveFromCart(int id)
        {
            _productLogic.RemoveFromCart(id);
            return RedirectToAction("Cart", "Product");
        }
        public IActionResult Checkout() //Deletes user cookies during "Checkout"
        {
            HttpContext.Response.Cookies.Delete("AspProjectGuestInfo");
            _productLogic.Checkout();
            return RedirectToAction("Thanks", "Product");
        }  
        public IActionResult AddProducts(Product product)
        {
            _productLogic.AddProducts(product);
            return RedirectToAction("HomePage","Home");
        }
    }
}

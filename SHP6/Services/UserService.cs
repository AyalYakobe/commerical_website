using Microsoft.AspNetCore.Http;
using SHP6.DAL;
using SHP6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Services
{
    public class UserService : IUserService
    {
        private readonly NewDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(NewDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationUser GetUserDetails(string userName)
        {
            ApplicationUser user = _context.Users.FirstOrDefault(user => user.UserName == userName);
            return user;
        }
        public ApplicationUser RetrieveUserFromCookies() //Retrieves user info from cookies
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

        public void UpdateUserDetails(ApplicationUser newUserDetails)
        {
            var exist = _context.Users.Find(newUserDetails.Id);
            _context.Entry(exist).CurrentValues.SetValues(newUserDetails);
            _context.SaveChanges();
        }

        public string RetrieveGuestId() //Retrieves guest cookies
        {
            string cookieGuestValue = _httpContextAccessor.HttpContext.Request.Cookies["AspProjectGuestInfo"];
            if (string.IsNullOrEmpty(cookieGuestValue))
            {
                return null;
            }
            return cookieGuestValue;
        }

        public bool DoesUserExist(string userName, string password) //Checks the existance of a user
        {
            ApplicationUser user = _context.Users.FirstOrDefault(user => user.UserName == userName && user.Password == password);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool AddUser(ApplicationUser user) //Adds user and checks for duplicate Usernames and Passwords
        {
            ApplicationUser duplicate = _context.Users.Where(u => u.UserName == user.UserName || u.Password == user.Password).FirstOrDefault();
            if (duplicate == null)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SubmitUser(ApplicationUser user)
        {
            if (!AddUser(user))
            {
                return false;
            }
            AddUser(user);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AspProjectCookie", $"{user.UserName},{user.Password}", new CookieOptions() { Expires = (DateTime.Now).AddDays(3) });
            return true;
        }
    }
}

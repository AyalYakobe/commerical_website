using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SHP6.Models;
using SHP6.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace SHP6.Controllers
{
    //Newest Edition: 1.0
    public class HomeController : Controller
    {
        private readonly IProductService _productLogic;
        private readonly IUserService _userLogic;

        public HomeController(IProductService productLogic, IUserService userLogic)
        {
            _productLogic = productLogic;
            _userLogic = userLogic;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult HomePage(string sort) //Scans store for null user, sorts products, sets current page
        {
            _productLogic.ScanStore();
            ViewData["Product"] = _productLogic.SortBy(sort);
            ViewData["CurrentPage"] = "Home";
            return View();
        }
        public IActionResult AboutUs()
        {
            ViewData["CurrentPage"] = "AboutUs";
            return View();
        }
        public IActionResult RegisterUser() //Sends user to register page and determines if he's previously registered
        {
            var user = _userLogic.RetrieveUserFromCookies();
            if (user != null)
            {
                return View(user);
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(SignIn user) //Checks if user exists, if not creates cookies
        {
            if (user == null)
            {
                TempData["LogInError"] = "Incorrect";
                return RedirectToAction("HomePage", user);
            }
            else if (_userLogic.DoesUserExist(user.Username, user.SignInPassword))
            {
                TempData["LogInError"] = "Correct";
                HttpContext.Response.Cookies.Append("AspProjectCookie", $"{user.Username},{user.SignInPassword}", new CookieOptions() { Expires = (DateTime.Now).AddDays(3) });
                return RedirectToAction("HomePage");
            }
            else
            {
                TempData["LogInError"] = "Incorrect";
                return RedirectToAction("HomePage", user);
            }
        }
        [HttpPost]
        public IActionResult Submit(ApplicationUser user) //Submits user or updates details depending on cookies
        {
            if (_userLogic.RetrieveUserFromCookies() == null)
            {
                if (ModelState.IsValid)
                {
                    if (_userLogic.SubmitUser(user))
                    {
                        TempData["DuplicateInformation"] = "Incorrect";
                        return RedirectToAction("HomePage", "Home");
                    }
                    else
                    {
                        TempData["DuplicateInformation"] = "Correct";
                        return RedirectToAction("RegisterUser", "Home");
                    }
                }
                else
                {
                    TempData["DuplicateInformation"] = "Incorrect";
                    return RedirectToAction("RegisterUser", "Home", ModelState);
                }
            }
            else
            {
                TempData["DuplicateInformation"] = "Incorrect(Updated)";
                _userLogic.UpdateUserDetails(user);
                return RedirectToAction("RegisterUser", "Home", user);
            }
        }
        public IActionResult SignOut() //Deletes cookies when user signs out
        {
            HttpContext.Response.Cookies.Delete("AspProjectCookie");
            return RedirectToAction("HomePage", "Home");
        }
    }
}

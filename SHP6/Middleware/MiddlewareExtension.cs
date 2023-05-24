using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SHP6.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MiddlewareExtension
    {
        private readonly RequestDelegate _next;

        public MiddlewareExtension(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IUserService user) //Identifes user's "FirstName" based on cookies
        {           
            if (httpContext.Request.Cookies.ContainsKey("AspProjectCookie"))
            {
                string cookies = httpContext.Request.Cookies["AspProjectCookie"];
                string[] cookiesArray = cookies.Split(',');
                httpContext.Items.Add("CookieKey", user.GetUserDetails(cookiesArray[0]));
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MiddlewareExtension>();
        }
    }
}

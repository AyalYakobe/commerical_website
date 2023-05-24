using SHP6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Services
{
    public interface IUserService
    {
        ApplicationUser RetrieveUserFromCookies();
        ApplicationUser GetUserDetails(string userName);

        void UpdateUserDetails(ApplicationUser newUserDetails);

        string RetrieveGuestId();

        bool DoesUserExist(string userName, string password);
        bool AddUser(ApplicationUser user);
        bool SubmitUser(ApplicationUser user);

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using XQLiteMgm.Models;
using XQLiteMgm.Models.Repository.Tables;

namespace XQLiteMgm.Helper
{
    public class UserHelper
    {
        public static string GetUserAccount()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
                FormsAuthenticationTicket ticket = id.Ticket;
                OperatorViewModel user = JsonConvert.DeserializeObject<OperatorViewModel>(ticket.UserData);

                return user.name.ToString();
            }
            return string.Empty;
        }

        public static bool IsLogin()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }
    }
}
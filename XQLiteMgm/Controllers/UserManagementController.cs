using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XQLiteMgm.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserInfoMaintenance()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserOrderMaintenance()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserActionQuery()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserLoginQuery()
        {
            return View();
        }
    }
}
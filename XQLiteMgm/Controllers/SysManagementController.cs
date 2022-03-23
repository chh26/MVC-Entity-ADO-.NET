using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XQLiteMgm.Controllers
{
    public class SysManagementController : Controller
    {
        // GET: SysAccountMaintenance
        [Authorize]
        public ActionResult SysAccountMaintenance()
        {
            return View();
        }

        // GET: SysManagement
        public ActionResult Index()
        {
            return View();
        }
    }
}
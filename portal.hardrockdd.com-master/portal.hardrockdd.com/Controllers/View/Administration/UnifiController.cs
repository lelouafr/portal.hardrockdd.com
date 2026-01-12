using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class UnifiController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View("../Administration/Unifi/Index");
        }
    }
}
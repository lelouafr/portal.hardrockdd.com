using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class WebFormController : Controller
    {
        // GET: Reports
        [HttpGet]
        public ActionResult Index()
        {
            return View("../Administration/Azure/Index");
        }

        [HttpGet]
        public ActionResult Form(int id)
        {
            using var db = new VPContext();
            var result = db.WebControllers.FirstOrDefault(f => f.Id == id);

            return View("../Administration/Web/Form/Index", result);
        }

        [HttpGet]
        public ActionResult Table(int id)
        {
            using var db = new VPContext();
            var result = db.WebControllers.FirstOrDefault(f => f.Id == id);

            return View("../Administration/Web/Form/List/Table", result);
        }


    }
}
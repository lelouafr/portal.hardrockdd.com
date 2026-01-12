using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class WebFormUserLogsController : Controller
    {

        [HttpGet]
        public ActionResult Index(int controllerActionId, string userId)
        {
            using var db = new VPContext();
            var result = db.WebUserAccesses.FirstOrDefault(f => f.UserId == userId && f.ControllerActionId == controllerActionId);
            var model = new UserAccessLogListViewModel(result);
            return View("../Administration/Web/Access/Users/Index", model);
        }

        [HttpGet]
        public ActionResult Panel(int controllerActionId, string userId)
        {
            using var db = new VPContext();
            var result = db.WebUserAccesses.FirstOrDefault(f => f.UserId == userId && f.ControllerActionId == controllerActionId);
            var model = new UserAccessLogListViewModel(result);
            return PartialView("../Administration/Web/Access/UserLog/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(int controllerActionId, string userId)
        {
            using var db = new VPContext();
            var result = db.WebUserAccesses.FirstOrDefault(f => f.UserId == userId && f.ControllerActionId == controllerActionId);
            var model = new UserAccessLogListViewModel(result);
            return PartialView("../Administration/Web/Access/UserLog/List/Table", model);
        }


    }
}
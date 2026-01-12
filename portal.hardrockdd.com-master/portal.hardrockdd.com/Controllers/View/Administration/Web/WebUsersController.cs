using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace portal.Controllers
{
    public class WebUsersController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var model = new WebUserListViewModel(db);
            return View("../Administration/Web/Access/Users/Index", model);
        }

        [HttpGet]
        public JsonResult Combo()
        {
            var memKey = "WebUsersCombo";

            if (!(MemoryCache.Default[memKey] is WebUserListViewModel cachemodel) )
            {
                using var db = new VPContext();
                cachemodel = new WebUserListViewModel(db);

                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
                };
                systemCache.Set(memKey, cachemodel, policy);
            }
                        

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select User"
                }
            };
            result.AddRange(cachemodel.List.Select(s => new SelectListItem() {
                Text = s.FullName,
                Value = s.Id
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public ActionResult Index(int controllerId)
        //{
        //    using var db = new VPContext();
        //    var result = db.WebControllers.FirstOrDefault(f => f.Id == controllerId);
        //    var model = new UserAccessListViewModel(result);
        //    return View("../Administration/Web/Access/Users/Index", model);
        //}

        ////[HttpGet]
        ////public ActionResult Form(int controllerActionId, string userId)
        ////{
        ////    using var db = new VPContext();
        ////    var result = db.WebUserAccesses.FirstOrDefault(f => f.ControllerActionId == controllerActionId && f.UserId == userId);

        ////    return View("../Administration/Web/Access/Users/Form/Index", result);
        ////}

        //[HttpGet]
        //public ActionResult Table(int id)
        //{
        //    using var db = new VPContext();
        //    var result = db.WebControllers.FirstOrDefault(f => f.Id == id);
        //    var model = new UserAccessListViewModel(result);

        //    return View("../Administration/Web/Access/Form/List/Table", model);
        //}


        //[HttpGet]
        //public PartialViewResult Add(int controllerActionId, string userId)
        //{
        //    ViewBag.tableRow = "True";
        //    ViewBag.ViewOnly = false;

        //    using var db = new VPContext();
        //    var entry = new WebUserAccess();
        //    entry.AccessLevel = (byte)DB.AccessLevelEnum.Read;
        //    entry.ControllerActionId = controllerActionId;
        //    entry.UserId = userId;
        //    db.WebUserAccesses.Add(entry);
        //    db.SaveChanges(ModelState);


        //    return PartialView("", entry);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(UserAccessViewModel model)
        //{
        //    using var db = new VPContext();
        //    var updObj = db.WebUserAccesses.FirstOrDefault(f => f.UserId == model.UserId && f.ControllerActionId == model.ControllerActionId);
        //    if (updObj != null)
        //    {
        //        updObj.AccessLevel = (byte)model.AccessLevel;
        //        db.SaveChanges(ModelState);
        //    }

        //    //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        //    return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(string userId, int controllerActionId)
        //{
        //    using var db = new VPContext();
        //    var updObj = db.WebUserAccesses.FirstOrDefault(f => f.UserId == userId && f.ControllerActionId == controllerActionId);
        //    if (updObj != null)
        //    {
        //        db.WebUserAccesses.Remove(updObj);
        //        db.SaveChanges(ModelState);
        //    }
        //    return Json(new { success = "true"});
        //}
    }
}
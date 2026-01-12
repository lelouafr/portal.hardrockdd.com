using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Notification;
using portal.Repository.VP.WP;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.WP
{
    [Authorize]
    public class NotificationController : BaseController
    {
        //private readonly NotificationRepository repo = new NotificationRepository();

        [HttpGet]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var obj = db.Notifications
                        .Where(f => f.AssignedTo == userId && f.IsDeleted == (int)DB.YesNoEnum.No)
                        .OrderByDescending(o => o.CreatedOn)
                        .ToList();
            var model = obj.AsEnumerable().Select(s => new NotificationViewModel(s)).ToList();
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var obj = db.Notifications
                        .Where(f => f.AssignedTo == userId && f.IsDeleted == (int)DB.YesNoEnum.No)
                        .OrderByDescending(o => o.CreatedOn)
                        .ToList();
            var model = obj.AsEnumerable().Select(s => new NotificationViewModel(s)).ToList();
            return PartialView("~/Views/Notification/Table.cshtml", model);
        }

        [HttpGet]
        public PartialViewResult IndexMenu()
        {
            //using var repo = new NotificationRepository();
            //var model = repo.GetNotifications(StaticFunctions.GetUserId());
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var obj = db.Notifications
                        .Where(f => f.AssignedTo == userId && f.IsDeleted == (int)DB.YesNoEnum.No)
                        .OrderByDescending(o => o.CreatedOn)
                        .ToList();
            var model = obj.AsEnumerable().Select(s => new NotificationViewModel(s)).ToList();
            return PartialView("IndexMenu", model);
        }

        [HttpGet]
        public ActionResult GotoNotificationObjectURL(int id)
        {
            using var repo = new NotificationRepository();
            var model = repo.GetNotification(id);
            model.IsRead = (int)DB.YesNoEnum.Yes;
            model = repo.ProcessUpdate(model, ModelState);

            return Json(new { url = model.Url }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UserNotifications()
        {
            //using var repo = new NotificationRepository();
            //var model = repo.GetUnReadNotifications(StaticFunctions.GetUserId());

            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var obj = db.Notifications
                        .Where(f => f.AssignedTo == userId && f.IsRead == (int)DB.YesNoEnum.No)
                        .OrderByDescending(o => o.CreatedOn)
                        .ToList();
            var model = obj.AsEnumerable().Select(s => new NotificationViewModel(s)).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetNotificationRead(int id)
        {
            using var repo = new NotificationRepository();
            var model = repo.GetNotification(id);

            model.IsRead = (int)DB.YesNoEnum.Yes;
            model = repo.ProcessUpdate(model, ModelState);
            //notification.IsRead = Models.VP.WP.YesNo.Yes;
            //repo.UpdateSQL(notification);

            return Json(model.IsRead);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using var repo = new NotificationRepository();
            var model = repo.GetNotification(id);

            model.IsDeleted = (int)DB.YesNoEnum.Yes;
            model.IsRead = (int)DB.YesNoEnum.Yes;
            model = repo.ProcessUpdate(model, ModelState);

            //repo.UpdateSQL(notification);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
           
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
    }
}
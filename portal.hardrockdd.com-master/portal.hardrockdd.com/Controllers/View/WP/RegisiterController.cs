using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web.User.Device;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.WP
{
    public class APIRegisterController : BaseController
    {
        [HttpGet]
        [Route("APIRegister")]
        public ActionResult Index(string deviceId, string deviceName)
        {
            using var db = new VPContext();
            var entity = db.WPUserDeviceRegistries.FirstOrDefault(f => f.DeviceId == deviceId);
            var user = db.GetCurrentUser();
            if (entity == null)
            {
                entity = new WPUserDeviceRegistry()
                {
                    UserId = user.Id,
                    DeviceId = deviceId,
                    DeviceName = deviceName,
                    IsDisabled = !user.Active,
                };

                db.WPUserDeviceRegistries.Add(entity);
                db.SaveChanges();
            }
            var results = new UserDeviceRegistrationViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutExcelPane.cshtml";
            return View("../Sys/APIRegister/Index", results);
        }

        public ActionResult Register(UserDeviceRegistrationViewModel model)
        {
            return Json("");
        }

    }
}
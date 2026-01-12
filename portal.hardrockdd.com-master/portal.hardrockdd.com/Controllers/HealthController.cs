using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class HealthController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("Health")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var result = db.ErrorLogs.FirstOrDefault();
            var model = new ErrorLogViewModel(result);
            return View("../Health/Health");
        }
    }
}
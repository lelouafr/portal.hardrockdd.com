using portal.Models.Views.Dashboard;
using portal.Repository.VP.EM;
using portal.Repository.VP.WP;
using System.Web.Mvc;

namespace portal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            WebUserAccessRepository.CachedAccessList();
            EquipmentAuditRepository.AutoGenerateUserEquipmentAudit();
            WebUserAccessRepository.AddDefaultSecurity();
            var model = new HomeViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Blank()
        {
            return PartialView();
        }

    }

    //https://www.c-sharpcorner.com/article/connect-to-sharepoint-online-in-office-365-using-console-application/
}
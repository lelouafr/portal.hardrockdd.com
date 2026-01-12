using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.PR.Report;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.HR
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class PRPayrateController : BaseController
    {
        [HttpGet]
        [Route("PR/Report/PayRate")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            
            var results = new PayrateListViewModel();
            results.List.Add(new PayrateViewModel());

            results.PRCo = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).PRCo ?? 1;
            results.Year = System.DateTime.Now.Year;
            ViewBag.DataController = "PRPayrate";
            ViewBag.DataAction = "Data";
            return View("../PR/Report/Payrate/Index", results);
        }

        [HttpGet]
        public ActionResult Table(byte prco, int year)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();

            var results = new PayrateListViewModel();
            results.List.Add(new PayrateViewModel());

            results.PRCo = prco;
            results.Year = year;
            ViewBag.DataController = "PRPayrate";
            ViewBag.DataAction = "Data";
            return View("../PR/Report/Payrate/_Table", results);
        }

        [HttpGet]
        public ActionResult Data(byte prco, int year)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);
            PayrateListViewModel results;
            results = new PayrateListViewModel(company, year);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}
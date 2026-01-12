using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    [ControllerAuthorize]
    public class AllPORequestController : BaseController
    {
        [HttpGet]
        [Route("PORequest/All")]
        public ActionResult Index()
        {
            //using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var emp = user.Employee.FirstOrDefault();
            //var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "AllPORequest";
            ViewBag.Data = "Data";

            return View("../PO/Request/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {

            //using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var emp = user.Employee.FirstOrDefault();
            //var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "AllPORequest";
            ViewBag.Data = "Data";

            return PartialView("../PO/Request/Summary/List/Table", results);
        }


        [HttpGet]
        public ActionResult Data()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var emp = user.Employee.FirstOrDefault();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms
                .Include("POCompanyParm")
                .Include("POCompanyParm.PORequests")
                .Include("POCompanyParm.PORequests.Lines")
                .FirstOrDefault(f => f.HQCo == comp.HQCo);


            var poReqList = company.POCompanyParm.PORequests
                //.Include("Lines")
                //.Include("Vendor")
                .ToList();
            var results = new PORequestSummaryListViewModel(poReqList);


            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}
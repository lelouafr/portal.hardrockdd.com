using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Audit;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardAuditController : BaseController
    {
        [HttpGet]
        [Route("AP/Credit/Audit")]
        public ActionResult Index()
        {
            using var db = new VPContext();           
            var currentEmp = StaticFunctions.GetCurrentEmployee();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == currentEmp.PortalCompanyCode);
            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            var mth = cld.Date;

            var model = new FormViewModel(comp, mth);

            return View("../AP/CreditCard/Audit/Index", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte ccco, DateTime mth)
        {
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == ccco);
            var model = new AuditListViewModel(company, mth);

            return PartialView("../AP/CreditCard/Audit/List/Table", model);
        }

    }
}
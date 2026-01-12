using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Administration;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardAdministrationTransactionController : BaseController
    {
        [HttpGet]
        [Route("AP/Credit/Administration/Transactions")]///{co}-{employeeId}-{mth}
        public ActionResult Index()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var division = company.Divisions.FirstOrDefault();

            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            var mth = cld.Date;

            var model = new FormViewModel(division, mth, db);

            return View("../AP/CreditCard/Administration/Index", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte ccco, DateTime mth)
        {
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == ccco);
            var division = company.Divisions.FirstOrDefault();

            var model = new TransactionListViewModel(division, mth, db);

            return PartialView("../AP/CreditCard/Administration/List/Table", model);
        }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Invoice;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PurchasOrders
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AR,FIN-ARMGR,FIN-AR,FIN-ARMGR,FIN-CTRL,HR-MGR,OF-GA,OP-DM,OP-ENGD,OP-EQADM,OP-EQMGR,OP-GM,OP-PM,OP-SF,OP-SFMGR,OP-SLS,OP-SLSMGR,OP-SUP,SHP-MGR,SHP-SUP")]
    public class ARInvoiceController : BaseController
    {
        //[HttpGet]
        //[Route("AR/Invoices/{status}")]
        //public ActionResult Index()
        //{
        //    var timeSelection = DB.TimeSelectionEnum.LastThreeMonths;
        //    var results = new InvoiceSummaryListViewModel((ARStatusEnum)status, timeSelection);
        //    results.List.Add(new InvoiceSummaryViewModel());

        //    ViewBag.Controller = "ARInvoice";

        //    return View("../AR/Invoice/Index", results);
        //}

        //[HttpGet]
        //public ActionResult Table(DB.TimeSelectionEnum timeSelection)
        //{
        //    var results = new InvoiceSummaryListViewModel(status, timeSelection);
        //    results.List.Add(new InvoiceSummaryViewModel());

        //    ViewBag.Controller = "ARInvoice";

        //    return PartialView("../AR/Invoice/Table", results);
        //}

        #region Header
        [HttpGet]
        public ActionResult HeaderForm(byte arco, string mth, int arTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.ARTrans.FirstOrDefault(f => f.ARCo == arco && f.Mth == mthDate && f.ARTransId == arTransId);
            var model = new InvoiceViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AR/Invoice/Form/Header/Form", model);
        }
        #endregion

        #region Main Invoice Form
        [HttpGet]
        public ActionResult PopupForm(byte arco, string mth, int arTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.ARTrans.FirstOrDefault(f => f.ARCo == arco && f.Mth == mthDate && f.ARTransId == arTransId);
            var model = new InvoiceFormViewModel(result);
            ViewBag.ViewOnly = true;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../AR/Invoice/Form/Index", model);
        }

        [HttpGet]
        public ActionResult Panel(byte arco, string mth, int arTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.ARTrans.FirstOrDefault(f => f.ARCo == arco && f.Mth == mthDate && f.ARTransId == arTransId);
            var model = new InvoiceFormViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AR/Invoice/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult Form(byte arco, string mth, int arTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.ARTrans.FirstOrDefault(f => f.ARCo == arco && f.Mth == mthDate && f.ARTransId == arTransId);
            var model = new InvoiceFormViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AR/Invoice/Form/Form", model);
        }
        #endregion

        #region Line
        [HttpGet]
        public ActionResult LineForm(byte arco, DateTime mth, int arTransId, int lineId)
        {
            using var db = new VPContext();
            var result = db.ARTransLines.FirstOrDefault(f => f.ARCo == arco && f.Mth == mth && f.ARTransId == arTransId && f.ARLineId == lineId);
            var model = new InvoiceLineViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AR/Invoice/Form/Lines/Form/Panel", model);
        }
        #endregion

        [HttpGet]
        public ActionResult Data(DB.TimeSelectionEnum timeSelection)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new InvoiceListViewModel(company, timeSelection);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}
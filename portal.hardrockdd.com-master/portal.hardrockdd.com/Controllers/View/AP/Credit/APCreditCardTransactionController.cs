using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard;
using portal.Models.Views.AP.CreditCard.Form;
using portal.Models.Views.AP.CreditCard.Employee;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using portal.Repository.VP.AP.CreditCard;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardTransactionController : BaseController
    {

        [HttpGet]
        public ActionResult Index(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions
                .Include("Merchant")
                .Include("Coding")
                .Include("Coding.Transaction")
                .Include("Coding.Transaction.Merchant")
                .Include("LinkedImages")
                .Include("Lines")
                .Include("Employee")
                .Include("Employee.CreditCardImages")
                .Include("Employee.CreditCardImages.Transactions")
                .FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new TransactionFormViewModel(results);
            ViewBag.Partial = true;
            ViewBag.ViewOnly = model.Actions.Access == DB.SessionAccess.Edit ? "False" : "True";
            return View("../AP/CreditCard/Form/Index", model);
        }
        
        [HttpGet]
        public ActionResult Panel(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions
                .Include("Merchant")
                .Include("Coding")
                .Include("Coding.Transaction")
                .Include("Coding.Transaction.Merchant")
                .Include("LinkedImages")
                .Include("Lines")
                .Include("Employee")
                .Include("Employee.CreditCardImages")
                .Include("Employee.CreditCardImages.Transactions")
                .FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new TransactionFormViewModel(results);
            ViewBag.Partial = true;
            ViewBag.ViewOnly = model.Actions.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/CreditCard/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult Form(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions
                .Include("Merchant")
                .Include("Coding")
                .Include("Coding.Transaction")
                .Include("Coding.Transaction.Merchant")
                .Include("LinkedImages")
                .Include("Lines")
                .Include("Employee")
                .Include("Employee.CreditCardImages")
                .Include("Employee.CreditCardImages.Transactions")
                .FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new TransactionFormViewModel(results);
            ViewBag.Partial = true;
            ViewBag.ViewOnly = model.Actions.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/CreditCard/Form/Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval(TransactionApprovalViewModel model)
        {
            using var db = new VPContext();
            model.ApprovalStatusId = model.Approved ? DB.CMApprovalStatusEnum.SupervisorApproved : DB.CMApprovalStatusEnum.New;
            CreditTransactionRepository.ProcessUpdate(model, db);
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        #region Request Info
        [HttpGet]
        public ActionResult RequestInfoStatus(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions
                .FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new RequestInfoViewModel(results);
            return PartialView("../AP/CreditCard/RequestInfo/Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestInfoStatus(RequestInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using var db = new VPContext();
            model.ProcessUpdate(db, ModelState, ControllerContext);
            
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        #endregion
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard;
using portal.Models.Views.AP.CreditCard.Form;
using portal.Models.Views.AP.CreditCard.Employee;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardEmployeeTransactionController : BaseController
    {
        [HttpGet]
        [Route("AP/Credit/Employee/Transactions")]///{co}-{employeeId}-{mth}
        public ActionResult Index()
        {
            using var db = new VPContext();
            var currentEmp = StaticFunctions.GetCurrentEmployee();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == currentEmp.PRCo && f.EmployeeId == currentEmp.EmployeeId);
            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            if (DateTime.Now.Date.Day <= 5)
            {
                cld.Date = cld.Date.AddMonths(-1);
            }
            var mth = cld.Date;

            var model = new FormViewModel(emp, mth);

            return View("../AP/CreditCard/Employee/Index", model);
        }

        [HttpGet]
        [Route("AP/Credit/Employee/Transactions/{mth}")]///{co}-{employeeId}-{mth}
        public ActionResult MonthIndex(DateTime mth)
        {
            using var db = new VPContext();
            var currentEmp = StaticFunctions.GetCurrentEmployee();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == currentEmp.PRCo && f.EmployeeId == currentEmp.EmployeeId);
            //var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            //var mth = cld.Date;

            var model = new FormViewModel(emp, mth);

            return View("../AP/CreditCard/Employee/Index", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte prco, int employeeId, DateTime mth)
        {
            using var db = new VPContext();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == employeeId);
           
            var model = new TransactionListViewModel(emp, mth);

            return PartialView("../AP/CreditCard/Employee/List/Table", model);
        }


        [HttpGet]
        public ActionResult Form(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new TransactionFormViewModel(results);
            ViewBag.Partial = true;
            //ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/CreditCard/Form/Form", model);
        }

        [HttpPost]
        public ActionResult ImageBankAdd(byte co, int employeeId, DateTime mth)
        {
            using var db = new VPContext();
            var import = new CreditImport();

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase fileUpload = Request.Files[fileName];
                if (fileUpload.ContentType.Contains("image") || fileUpload.ContentType.Contains("pdf"))
                {
                    //using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
                    //var attach = db.CreditTransactionAttachments.FirstOrDefault(f => f.ReceiptImageRefId == fileUpload.FileName);
                    //if (attach != null)
                    //{
                    //    var trans = attach.Transaction;

                    //    CreditTransactionRepository.ProcessCreditCardReceipt(fileUpload, trans, db, dbAttch);

                    //    db.SaveChanges(ModelState);
                    //    dbAttch.SaveChanges(ModelState);

                    //}
                }
            }

            var model = new CreditImportViewModel(import); 
            return Json(new { Message = "Uploaded", model });
            //return PartialView("../AP/CreditCard/Upload/List/TableRow", model);
        }
    }
}
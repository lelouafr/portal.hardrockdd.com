using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard;
using portal.Models.Views.AP.CreditCard.Employee;
using portal.Models.Views.AP.CreditCard.Form;
using portal.Repository.VP.AP.CreditCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardEmployeeImageBankController : BaseController
    {
        [HttpPost]
        public ActionResult Add(byte prco, DateTime mth, int employeeId, int transId)
        {
            using var db = new VPContext();
            //using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == employeeId);
            var fileList = new List<DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile>();
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase fileUpload = Request.Files[fileName];
                if (fileUpload.ContentType.Contains("image") || fileUpload.ContentType.Contains("pdf"))
                {
                    if (emp != null)
                    {
                        var image = emp.AddCreditCardImage(fileUpload, mth);
                        if (image != null)
                        {
                            if (transId != 0)
                            {
                                var trans = db.CreditTransactions.FirstOrDefault(f => f.TransId == transId);
                                trans.LinkImageToTrans(image);
                            }
                            //fileList.Add(CreditCardImageRepository.ProcessCreditCardReceipt(fileUpload, emp, mth, transId, db, dbAttch));

                            db.SaveChanges(ModelState);
                            //dbAttch.SaveChanges(ModelState);
                        }

                    }
                }
            }
            foreach (var file in fileList)
            {
                file.Company = null;                
            }
            var jsonModelState = JsonConvert.SerializeObject(fileList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { Message = "Uploaded", dbStatus = ModelState.ModelErrors(), FileList = jsonModelState });
        }

        [HttpGet]
        public ActionResult ImageSelectType(byte ccco, long transId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new InfoViewModel(result);

            return PartialView("../AP/CreditCard/Employee/Attach/Modal/Modal", model);
        }

        [HttpGet]
        public ActionResult ImageBankTransModal(byte ccco, long transId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new ImageBankListViewModel(result, false, true);

            return PartialView("../AP/CreditCard/Form/ImageBank/TransModal", model);
        }

        [HttpGet]
        public ActionResult ImageBankModal(byte prco, int employeeId, DateTime mth)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == employeeId);
            var model = new ImageBankListViewModel(result, mth);

            return PartialView("../AP/CreditCard/Form/ImageBank/Modal", model);
        }

        [HttpGet]
        public ActionResult FilePreview(byte ccco, int employeeId, DateTime mth, int imageId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.CreditCardImages.FirstOrDefault(f => f.CCCo == ccco && f.EmployeeId == employeeId && f.Mth == mth && f.ImageId == imageId);
            var model = new ImageBankViewModel(result);

            return PartialView("../AP/CreditCard/Form/ImageBank/List/FilePreview", model);
        }


        [HttpPost]
        public ActionResult TagImage(byte ccco, int employeeId, DateTime mth, int imageId, long transId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var image = db.CreditCardImages.FirstOrDefault(f => f.CCCo == ccco && f.EmployeeId == employeeId && f.Mth == mth && f.ImageId == imageId);
            var trans = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            CreditCardImageRepository.TagCreditCardReceipt(image, trans);
            db.SaveChanges(ModelState);

            return Json(new { Message = "tagged" });
        }
        [HttpPost]
        public ActionResult UnTagImage(byte ccco, int employeeId, DateTime mth, int imageId, long transId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var image = db.CreditCardImages.FirstOrDefault(f => f.CCCo == ccco && f.EmployeeId == employeeId && f.Mth == mth && f.ImageId == imageId);
            var trans = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            CreditCardImageRepository.UnTagCreditCardReceipt(image, trans);
            db.SaveChanges(ModelState);

            return Json(new { Message = "tagged" });
        }

        [HttpPost]
        public ActionResult DeleteImage(byte ccco, int employeeId, DateTime mth, int imageId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
            var result = db.CreditCardImages.FirstOrDefault(f => f.CCCo == ccco && f.EmployeeId == employeeId && f.Mth == mth && f.ImageId == imageId);
            foreach (var item in result.Transactions.ToList())
            {
                result.Transactions.Remove(item);
            }
            var attachment = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == result.AttachmentId);
            var attachmentThumb = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == result.ThumbAttachmentId);
            if (attachment != null)
            {
                db.HQAttachmentFiles.Remove(attachment);
                var attachmentFile = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == attachment.AttachmentId);
                if (attachmentFile != null)
                    dbAttch.Attachments.Remove(attachmentFile);
            }
            if (attachmentThumb != null)
            {
                db.HQAttachmentFiles.Remove(attachmentThumb);
                var attachmentFile = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == attachmentThumb.AttachmentId);
                if (attachmentFile != null)
                    dbAttch.Attachments.Remove(attachmentFile);
            }
            db.CreditCardImages.Remove(result);
            db.SaveChanges(ModelState);
            dbAttch.SaveChanges(ModelState);
            return Json(new { Message = "Deleted" });
        }


        [HttpGet]
        public ActionResult Panel(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);

            var model = new TransactionFormViewModel(results);
            ViewBag.Partial = true;
            //ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/CreditCard/Form/Panel", model);
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
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Forms;
using portal.Repository.VP.AP;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class VendorMerchantController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(VendorMerchantViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = VendorRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(VendorMerchantViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RunMatch(byte vendGroupId, int vendorId)
        {
            using var db = new VPContext();
            var vendor = db.APVendors.FirstOrDefault(f => f.VendorGroupId == vendGroupId && f.VendorId == vendorId);
            //var APParms = db.APParameters.FirstOrDefault(f => f.APCo == emp.HRCo);
            if (string.IsNullOrEmpty(vendor.MerchantMatchString ))
                return Json("");
            var matchList = vendor.MerchantMatchString.Split(';');
            foreach (var str in matchList)
            {
                var merchantList = db.CreditMerchants
                    .Where(f => f.VendGroupId == vendGroupId && 
                                f.Name.Contains(str) &&
                                f.Transactions.Any(trans => (trans.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open))
                    .ToList();
                foreach (var item in merchantList)
                {
                    var isVendorIdChanged = item.VendorId != vendor.VendorId;
                    item.VendorId = vendor.VendorId;
                    vendor.AddAddress(item);
                    
                    if (db.CreditTransactions.Where(f => (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open && f.Merchant.Vendor.VendorId == item.VendorId && f.MerchantId == item.MerchantId).Any())
                    {
                        var transList = db.CreditTransactions.Where(f => (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open && f.Merchant.Vendor.VendorId == item.VendorId && f.MerchantId == item.MerchantId).ToList();

                        //if (db.CreditTransactions.Where(f => f.TransStatusId == (int)DB.CMTranStatusEnum.Open && f.Merchant.Vendor.VendorId == item.VendorId && f.PictureStatusId == (int)DB.CMPictureStatusEnum.Empty).Any() ||
                        //    db.CreditTransactions.Where(f => f.TransStatusId == (int)DB.CMTranStatusEnum.Open && f.Merchant.Vendor.VendorId == item.VendorId && f.CodedStatusId == (int)DB.CMTransCodeStatusEnum.Empty).Any())
                        //{
                            var picList = transList.Where(f => (f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.Empty).ToList();

                            foreach (var trans in picList)
                            {
                                trans.AutoPictureStatus();
                            }
                            var codeList = transList.Where(f => (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.Empty).ToList();

                            foreach (var trans in codeList)
                            {
                                trans.AutoCode();
                            }
                        //}
                    }
                }
            }
            db.SaveChanges(ModelState);
            return Json("");
        }
    }
}
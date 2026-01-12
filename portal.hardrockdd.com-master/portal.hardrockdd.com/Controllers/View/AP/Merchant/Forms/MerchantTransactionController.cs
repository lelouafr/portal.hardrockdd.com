using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Repository.VP.AP.Merchant;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantTransactionController : BaseController
    {
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(MerchantInfoViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using var db = new VPContext();
        //        model = CreditMerchantRepository.ProcessUpdate(model, db);
        //        db.SaveChanges(ModelState);
        //    }
        //    return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public JsonResult Validate(MerchantInfoViewModel model)
        //{
        //    return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult XData(byte ccco, string merchantId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var data = db.CreditTransactions
                    .Include("Employee")
                    .Where(f => f.CCCo == ccco && f.MerchantId == merchantId)
                    .ToList();

            var emplList = data.GroupBy(g => new { g.PRCo, g.Employee }).Select(s => new
            {

                id = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.Key.Employee.EmployeeId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.Key.Employee.FirstName, s.Key.Employee.LastName),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.Sum(sum => sum.TransAmt)),
                Count = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.Count()),

            });

            var transList = data.Select(s => new {
                id = string.Format(AppCultureInfo.CInfo(), "1.{0}.{1}", s.EmployeeId, s.TransId),
                parent = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.EmployeeId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.OrigDescription ?? s.NewDescription),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.TransAmt),
                TransDate = string.Format(AppCultureInfo.CInfo(),"{0}", s.TransDate.ToShortDateString())
            });

            var dataSet = new List<dynamic>();
            dataSet.AddRange(emplList);
            dataSet.AddRange(transList);

            JsonResult result = Json(dataSet, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;

            return result;
        }

    }
}
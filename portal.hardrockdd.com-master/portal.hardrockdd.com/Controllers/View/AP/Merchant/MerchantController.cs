using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant;
using portal.Repository.VP.AP.Merchant;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantController : BaseController
    {
        [HttpGet]
        [Route("Merchant")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();

            var results = new MerchantListViewModel();
            results.List.Add(new MerchantViewModel());
            results.VendGroupId = (byte)comp.VendorGroupId;
            ViewBag.DataController = "Merchant";
            ViewBag.DataAction = "Data";
            return View("../AP/Vendor/Merchant/Summary/List/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();

            var results = new MerchantListViewModel();
            results.List.Add(new MerchantViewModel());
            results.VendGroupId = (byte)comp.VendorGroupId;
            ViewBag.DataController = "Merchant";
            ViewBag.DataAction = "Data";
            return PartialView("../AP/Vendor/Merchant/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult XTable()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new MerchantListViewModel(company);
            return PartialView("../AP/Vendor/Merchant/Summary/List/xTable", results);
        }

        [HttpGet]
        public ActionResult XForm(byte vendGroupId, string categoryGroup, int? categoryCodeId, int? merchantId, int? vendorId, string FormType)
        {
            return FormType switch
            {
                "Group" => RedirectToAction("Panel", "MerchantGroupForm", new { Area = "",vendGroupId, categoryGroup }),
                "Category" => RedirectToAction("Panel", "MerchantCategoryForm", new { Area = "",vendGroupId, categoryGroup, categoryCodeId }),
                "Merchant" => RedirectToAction("Panel", "MerchantForm", new { Area = "",vendGroupId, merchantId }),
                "Vendor" => RedirectToAction("Panel", "VendorForm", new { Area = "",vendGroupId, vendorId }),
                _ => View(),
            };
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new MerchantListViewModel(company);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        public JsonResult XData()
        {
            using var db = new VPContext();

            var comp = StaticFunctions.GetCurrentCompany();
            var data = db.HQCompanyParms
                    .Include("MerchantGroups")
                    .Include("MerchantGroups.Categories")
                    .Include("MerchantGroups.Categories.Merchants.Vendor")
                    .Include("MerchantGroups.Categories.Merchants.Transactions")
                    .FirstOrDefault(f => f.HQCo == comp.HQCo);

            var groupListv2 = data.MerchantGroups.OrderBy( o=> o.Description).Select(s => new
            {

                id = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.CategoryGroup),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.Categories.SelectMany(s => s.Merchants.SelectMany(t => t.Transactions)).Sum(sum => sum.TransAmt)),
                Count = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.Categories.SelectMany(s => s.Merchants.SelectMany(t => t.Transactions)).Count()),

                VendGroupId = s.VendGroupId,
                CategoryGroup = s.CategoryGroup,
                FormType = "Group",
                GLOverride = s.DefaultGLAcct?.Trim(),
                JobOverride = s.DefaultJCPhaseId != null || s.DefaultJCCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultJCPhaseId, s.DefaultJCCType) : "",
                EqpOverride = s.DefaultEMCostCodeId != null || s.DefaultEMCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultEMCostCodeId, s.DefaultEMCType) : "",
            });

            var catListV2 = data.MerchantGroups.SelectMany(s => s.Categories).OrderBy(o => o.Description).Select(s => new {
                id = string.Format(AppCultureInfo.CInfo(), "1.{0}.{1}", s.CategoryGroup, s.CategoryCodeId),
                parent = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.CategoryGroup),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.Merchants.SelectMany(t => t.Transactions).Sum(sum => sum.TransAmt)),
                Count = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.Merchants.SelectMany(t => t.Transactions).Count()),
                VendGroupId = s.VendGroupId,
                CategoryGroup = s.CategoryGroup,
                CategoryCodeId = s.CategoryCodeId,
                FormType = "Category",
                GLOverride = s.DefaultGLAcct?.Trim(),
                JobOverride = s.DefaultJCPhaseId != null || s.DefaultJCCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultJCPhaseId, s.DefaultJCCType) : "",
                EqpOverride = s.DefaultEMCostCodeId != null || s.DefaultEMCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultEMCostCodeId, s.DefaultEMCType) : "",

            });


            var vendorListV2 = data.MerchantGroups.SelectMany(s => s.Categories.SelectMany(c => c.Merchants))
                                                  .GroupBy(g => new { g.VendGroupId, g.CategoryGroup, g.CategoryCodeId, g.VendorId, g.Vendor })
                                                  .Select(s => new
            {
                id = string.Format(AppCultureInfo.CInfo(), "2.{0}.{1}.{2}", s.Key.CategoryGroup, s.Key.CategoryCodeId, s.Key.VendorId),
                parent = string.Format(AppCultureInfo.CInfo(), "1.{0}.{1}", s.Key.CategoryGroup, s.Key.CategoryCodeId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Key.Vendor.DisplayName),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.SelectMany(t => t.Transactions).Sum(sum => sum.TransAmt)),
                Count = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.SelectMany(t => t.Transactions).Count()),
                VendGroupId = s.Key.VendGroupId,
                CategoryGroup = s.Key.CategoryGroup,
                CategoryCodeId = s.Key.CategoryCodeId,
                VendorId = s.Key.VendorId,
                FormType = "Vendor",
                GLOverride = s.Key.Vendor.DefaultGLAcct?.Trim(),
                JobOverride = s.Key.Vendor.DefaultJCPhaseId != null || s.Key.Vendor.DefaultJCCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.Key.Vendor.DefaultJCPhaseId, s.Key.Vendor.DefaultJCCType) : "",
                EqpOverride = s.Key.Vendor.DefaultEMCostCodeId != null || s.Key.Vendor.DefaultEMCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.Key.Vendor.DefaultEMCostCodeId, s.Key.Vendor.DefaultEMCType) : "",
            });

            var merchantListV2 = data.MerchantGroups.SelectMany(s => s.Categories.SelectMany(c => c.Merchants)).OrderBy(o => o.Name).Select(s => new
            {
                id = string.Format(AppCultureInfo.CInfo(), "3.{0}.{1}.{2}.{3}", s.CategoryGroup, s.CategoryCodeId, s.VendorId, s.MerchantId),
                parent = string.Format(AppCultureInfo.CInfo(), "2.{0}.{1}.{2}", s.CategoryGroup, s.CategoryCodeId, s.VendorId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Name),
                Amount = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.Transactions.Sum(sum => sum.TransAmt)),
                Count = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.Transactions.Count),
                s.VendGroupId,
                s.CategoryGroup,
                s.CategoryCodeId,
                s.MerchantId,
                FormType = "Merchant",
                GLOverride = s.DefaultGLAcct?.Trim(),
                JobOverride = s.DefaultJCPhaseId != null || s.DefaultJCCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultJCPhaseId, s.DefaultJCCType) : "",
                EqpOverride = s.DefaultEMCostCodeId != null || s.DefaultEMCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultEMCostCodeId, s.DefaultEMCType) : "",
            });
            var dataSet = new List<dynamic>();
            dataSet.AddRange(groupListv2);
            dataSet.AddRange(catListV2);
            dataSet.AddRange(vendorListV2);
            dataSet.AddRange(merchantListV2);

            //var jsonData = JsonConvert.SerializeObject(dataSet);

            JsonResult result = Json(dataSet, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;

            return result;
        }

        [HttpGet]
        public ActionResult Form(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new Models.Views.AP.Vendor.Merchant.Forms.MerchantFormViewModel(obj);
            return PartialView("../AP/Vendor/Merchant/Forms/Form", results);
        }


    }
}
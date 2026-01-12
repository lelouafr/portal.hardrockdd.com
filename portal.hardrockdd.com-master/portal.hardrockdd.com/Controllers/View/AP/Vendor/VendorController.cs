using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Vendor
{
    [ControllerAuthorize]
    public class VendorController : BaseController
    {

        [HttpGet]
        [Route("Vendor")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new VendorListViewModel();
            results.List.Add(new VendorViewModel());
            results.Co = company.HQCo;
            ViewBag.DataController = "Vendor";
            ViewBag.DataAction = "Data";
            return View("../AP/Vendor/Summary/List/Index", results);
        }

        [HttpGet]
        public ActionResult XForm(byte co, int vendorId, string FormType)
        {
            return FormType switch
            {
                "Vendor" => RedirectToAction("Panel", "VendorForm", new { co, vendorId }),
                _ => View(),
            };
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();

            var results = new VendorListViewModel();
            results.List.Add(new VendorViewModel());
            results.Co = comp.HQCo;
            ViewBag.DataController = "Vendor";
            ViewBag.DataAction = "Data";
            return PartialView("../AP/Vendor/Summary/List/Table", results);
        }



        [HttpGet]
        public ActionResult Data()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new VendorListViewModel(company);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }


        public JsonResult XData()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var data = db.HQCompanyParms
                    .Include("VendorGroup.APVendors")
                    .Include("VendorGroup.APVendors.PurchaseOrders")
                    .FirstOrDefault(f => f.HQCo == comp.HQCo);
            var vendorList = data.VendorGroup.APVendors.OrderBy(o => o.VendorId).Select(s => new
            {

                id = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.VendorId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Name),
                Contact = string.Format(AppCultureInfo.CInfo(), "{0}", s.Contact),
                Active = string.Format(AppCultureInfo.CInfo(), "{0}", s.ActiveYN == "Y" ? "Active" : "Disabled"),
                OpenPOCount = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.PurchaseOrders.Where(w => w.Status == (int)DB.POStatusEnum.Open).Count()),
                OpenPOBalance = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.PurchaseOrders.Where(w => w.Status == (int)DB.POStatusEnum.Open).Count()),

                VendorGroupId = s.VendorGroupId,
                VendorId = s.VendorId,
                //CategoryGroup = s.CategoryGroup,
                FormType = "Vendor",
                GLOverride = s.DefaultGLAcct?.Trim(),
                JobOverride = s.DefaultJCPhaseId != null || s.DefaultJCCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultJCPhaseId, s.DefaultJCCType) : "",
                EqpOverride = s.DefaultEMCostCodeId != null || s.DefaultEMCType != null ? string.Format(AppCultureInfo.CInfo(), "{0}/{1}", s.DefaultEMCostCodeId, s.DefaultEMCType) : "",
            });

            var dataSet = new List<dynamic>();
            dataSet.AddRange(vendorList);

            JsonResult result = Json(dataSet, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;

            return result;
        }

    }
}
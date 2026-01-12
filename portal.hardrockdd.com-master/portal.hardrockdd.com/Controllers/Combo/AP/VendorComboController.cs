//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Vendor;
//using portal.Repository.VP.AP;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.AR
//{
//    [ValidateAntiForgeryTokenOnAllPosts]
//    public class VendorComboController : BaseController
//    {
//        [HttpGet]
//        public PartialViewResult Search(byte co)
//        {
//            using var db = new VPContext();
//            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);
//            var result = new VendorListViewModel(comp);

//            return PartialView("../AP/Vendor/Search/Panel", result);
//        }

//        [HttpGet]
//        public PartialViewResult SearchTable(byte co)
//        {
//            using var db = new VPContext();
//            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);
//            var result = new VendorListViewModel(comp);

//            return PartialView("../AP/Vendor/Search/Table", result);
//        }

//        [HttpPost]
//        public JsonResult SearchReturn(VendorViewModel model)
//        {
//            if (model == null)
//            {
//                model = new VendorViewModel();
//            }
//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = "true", value = model.VendorId, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult Combo(byte co, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.APVendors
//                        .Where(f => f.VendorGroup == co && f.VendorId != 999999)
//                        .OrderByDescending(o => o.ActiveYN)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Vendor"
//                }
//            };
//            result.Add(new SelectListItem
//            {
//                Text = "New Vendor",
//                Value = "999999",
//                Group = new SelectListGroup
//                {
//                    Name = "Active",
//                    Disabled = false
//                },
//            });
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
//                Selected = s.VendorId.ToString(AppCultureInfo.CInfo()) == selected,
//                Group = new SelectListGroup
//                {
//                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.ActiveYN != "Y"
//                },

//            }).ToList());


//            return Json(result, JsonRequestBehavior.AllowGet);
//        }


//        [HttpGet]
//        public JsonResult ActiveCombo(byte co, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.APVendors
//                        .Where(f => f.VendorGroup == co && f.VendorId != 999999)
//                        .OrderByDescending(o => o.ActiveYN)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Vendor"
//                }
//            };
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
//                Selected = s.VendorId.ToString(AppCultureInfo.CInfo()) == selected,
//                Group = new SelectListGroup
//                {
//                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.ActiveYN != "Y"
//                },

//            }).ToList());


//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//        [HttpGet]
//        public JsonResult MerchantCombo(byte co, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.CreditMerchants
//                        .Where(f => f.Co == co)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Merchant"
//                }
//            };
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.MerchantId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.MerchantId, s.Name),
//                Selected = s.MerchantId.ToString(AppCultureInfo.CInfo()) == selected,
//                //Group = new SelectListGroup
//                //{
//                //    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                //    Disabled = s.ActiveYN != "Y"
//                //},

//            }).ToList());


//            return Json(result, JsonRequestBehavior.AllowGet);
//        }




//        [HttpGet]
//        public JsonResult JobVendorCombo(byte co, string jobId, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.APVendors
//                        .Where(f => f.VendorGroup == co && f.VendorId != 999999 && f.PurchaseOrders.Any(a => a.Items.Any(i => i.JobId == jobId)))
//                        .OrderByDescending(o => o.ActiveYN)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Vendor"
//                }
//            };
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
//                Selected = s.VendorId.ToString(AppCultureInfo.CInfo()) == selected,
//                Group = new SelectListGroup
//                {
//                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.ActiveYN != "Y"
//                },

//            }).ToList());


//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//    }
//}
//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job;
//using portal.Models.Views.Purchase.Order;
//using portal.Repository.VP.JC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.JV
//{
//    [Authorize]
//    public class POController : BaseController
//    {
//        [HttpGet]
//        public PartialViewResult Search(byte co, int? vendorId)
//        {
//            using var db = new VPContext();
//            if (vendorId != null)
//            {
//                var vend = db.APVendors.FirstOrDefault(f => f.VendorGroupId == co && f.VendorId == vendorId);
//                var result = new PurchaseOrderSummaryListViewModel(vend, true);

//                return PartialView("../PO/PurchaseOrder/Search/Panel", result);
//            }
//            else
//            {
//                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);
//                var result = new PurchaseOrderSummaryListViewModel(comp, true);

//                return PartialView("../PO/PurchaseOrder/Search/Panel", result);
//            }
//        }

//        [HttpGet]
//        public PartialViewResult SearchTable(byte co, int? vendorId)
//        {
//            using var db = new VPContext();
//            if (vendorId != null)
//            {
//                var vend = db.APVendors.FirstOrDefault(f => f.VendorGroupId == co && f.VendorId == vendorId);
//                var result = new PurchaseOrderSummaryListViewModel(vend, true);

//                return PartialView("../PurchaseOrder/Search/Table", result);
//            }
//            else
//            {
//                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);
//                var result = new PurchaseOrderSummaryListViewModel(comp, true);

//                return PartialView("../PurchaseOrder/Search/Table", result);
//            }
//        }

//        [HttpGet]
//        public ActionResult SearchData(byte co, int? vendorId)
//        {
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            if (vendorId != null)
//            {
//                var vend = db.APVendors.FirstOrDefault(f => f.VendorGroupId == co && f.VendorId == vendorId);
//                var results = new PurchaseOrderSummaryListViewModel(vend, true);

//                JsonResult result = Json(new
//                {
//                    data = results.List.ToArray()
//                }, JsonRequestBehavior.AllowGet);
//                result.MaxJsonLength = int.MaxValue;
//                return result;
//            }
//            else
//            {
//                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);
//                var results = new PurchaseOrderSummaryListViewModel(comp, true);

//                JsonResult result = Json(new
//                {
//                    data = results.List.ToArray()
//                }, JsonRequestBehavior.AllowGet);
//                result.MaxJsonLength = int.MaxValue;
//                return result;
//            }
//        }

//        [HttpPost]
//        public JsonResult SearchReturn(PurchaseOrderSummaryViewModel model)
//        {
//            if (model == null)
//            {
//                model = new PurchaseOrderSummaryViewModel();
//            }
//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = "true", value = model.PO, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult VendorCombo(byte co, int vendorId = -1, string selected = null)
//        {
//            using var db = new VPContext();
//            var currentDate = DateTime.Now.Date.AddYears(-1);
//            var list = db.PurchaseOrders
//                .OrderBy(o => o.Status)
//                .ThenByDescending(o => o.PO)
//                .Where(f => f.POCo == co && 
//                f.VendorId == (vendorId == -1 ? f.VendorId : vendorId)
//                && ((f.OrderDate >= currentDate && f.Status ==2 && vendorId != -1) || (f.Status != 2))
//                )
//                .ToList();// && f.Status != 2

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select PO"
//                }
//            };
//            //result.AddRange(list.OrderBy(o => o.PO)
//            //              .Select(s => new SelectListItem
//            //              {
//            //                  Value = s.PO.ToString(AppCultureInfo.CInfo()),
//            //                  Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.PO),
//            //                  Selected = s.PO.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//            //              }).ToList());

//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.PO.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.PO),
//                Selected = s.PO.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
//                Group = new SelectListGroup
//                {
//                    Name = s.Status != 2 ? "Open" : "Closed",
//                    Disabled = s.Status == 2
//                },

//            }).ToList());
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult VendorAllCombo(byte co, int vendorId = -1, string selected = null)
//        {
//            using var db = new VPContext();

//            //var list = db.PurchaseOrders.Where(f => f.POCo == co && f.VendorId == (vendorId == -1 ? f.VendorId : vendorId)).ToList();

//            var currentDate = DateTime.Now.Date.AddYears(-1);
//            var list = db.PurchaseOrders
//                .OrderBy(o => o.Status)
//                .ThenByDescending(o => o.PO)
//                .Where(f => f.POCo == co &&
//                f.VendorId == (vendorId == -1 ? f.VendorId : vendorId)
//                && ((f.OrderDate >= currentDate && f.Status == 2 && vendorId != -1) || (f.Status != 2))
//                )
//                .ToList();// && f.Status != 2
//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select PO"
//                }
//            };
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.PO.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.PO),
//                Selected = s.PO.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
//                Group = new SelectListGroup
//                {
//                    Name = s.Status != 2 ? "Open" : "Closed",
//                    Disabled = s.Status == 2
//                },

//            }).ToList());

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult AllCombo(byte co, string selected = null)
//        {
//            using var db = new VPContext();

//            var list = db.PurchaseOrders.Where(f => f.POCo == co).ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select PO"
//                }
//            };
            
//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.PO.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.PO),
//                Selected = s.PO.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
//                Group = new SelectListGroup
//                {
//                    Name = s.Status != 2 ? "Open" : "Closed",
//                    Disabled = s.Status == 2
//                },

//            }).ToList());
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }



//        [HttpGet]
//        public JsonResult POJobCombo(byte co, string jobId, string selected = null)
//        {
//            using var db = new VPContext();
//            var list = db.PurchaseOrders
//                .OrderBy(o => o.Status)
//                .ThenByDescending(o => o.PO)
//                .Where(f => f.POCo == co &&
//                            f.Items.Any(i => i.JobId == jobId)
//                )
//                .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select PO"
//                }
//            };

//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.PO.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.PO),
//                Selected = s.PO.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
//                Group = new SelectListGroup
//                {
//                    Name = s.Status != 2 ? "Open" : "Closed",
//                    Disabled = s.Status == 2
//                },

//            }).ToList());
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//    }
//}
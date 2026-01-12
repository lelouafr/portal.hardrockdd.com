using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Vendor;
using portal.Repository.VP.AP;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AR
{
    [ValidateAntiForgeryTokenOnAllPosts]
    public class APComboController : BaseController
    {
        [HttpGet]
        public PartialViewResult VendorSearch(byte vendGroupId)
        {
            using var db = new VPContext();
            var hqGroup = db.HQGroups.FirstOrDefault(f => f.GroupId == vendGroupId);
            var result = new VendorListViewModel(hqGroup);

            return PartialView("../AP/Vendor/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult VendorSearchTable(byte vendGroupId)
        {
            using var db = new VPContext();
            var hqGroup = db.HQGroups.FirstOrDefault(f => f.GroupId == vendGroupId);
            var result = new VendorListViewModel(hqGroup);

            return PartialView("../AP/Vendor/Search/Table", result);
        }

        [HttpPost]
        public JsonResult VendorSearchReturn(VendorViewModel model)
        {
            if (model == null)
            {
                model = new VendorViewModel();
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.VendorId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult VendorCombo(byte? vendGroupId)
        {
            if (vendGroupId == null)
            {
                vendGroupId = 1;
            }
            using var db = new VPContext();
            var list = db.APVendors
                        .Where(f => f.VendorGroupId == vendGroupId && f.VendorId != 999999)
                        .OrderByDescending(o => o.ActiveYN)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Vendor"
                }
            };
            result.Add(new SelectListItem
            {
                Text = "New Vendor",
                Value = "999999",
                Group = new SelectListGroup
                {
                    Name = "Active",
                    Disabled = false
                },
            });
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
                Group = new SelectListGroup
                {
                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                    Disabled = s.ActiveYN != "Y"
                },

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VendorActiveCombo(byte? vendGroupId)
        {
            if (vendGroupId == null)
            {
                vendGroupId = 1;
            }
            using var db = new VPContext();
            var list = db.APVendors
                        .Where(f => f.VendorGroupId == vendGroupId && f.VendorId != 999999)
                        .OrderByDescending(o => o.ActiveYN)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Vendor"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
                Group = new SelectListGroup
                {
                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                    Disabled = s.ActiveYN != "Y"
                },

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult MerchantCombo(byte vendGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.CreditMerchants
                        .Where(f => f.VendGroupId == vendGroupId)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Merchant"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.MerchantId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.MerchantId, s.Name),
                Selected = s.MerchantId.ToString(AppCultureInfo.CInfo()) == selected,
                //Group = new SelectListGroup
                //{
                //    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                //    Disabled = s.ActiveYN != "Y"
                //},

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JobVendorCombo(byte vendGroupId, string jobId, string selected)
        {
            using var db = new VPContext();
            var list = db.APVendors
                        .Where(f => f.VendorGroupId == vendGroupId && f.VendorId != 999999 && f.PurchaseOrders.Any(a => a.Items.Any(i => i.JobId == jobId)))
                        .OrderByDescending(o => o.ActiveYN)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Vendor"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.VendorId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.VendorId, s.DisplayName),
                Selected = s.VendorId.ToString(AppCultureInfo.CInfo()) == selected,
                Group = new SelectListGroup
                {
                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                    Disabled = s.ActiveYN != "Y"
                },

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Customer;
using portal.Repository.VP.AP;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AR
{
    [ValidateAntiForgeryTokenOnAllPosts]
    public class ARComboController : BaseController
    {
        [HttpGet]
        public PartialViewResult Search(byte custGroupId)
        {
            using var db = new VPContext();
            var hqGroup = db.HQGroups.FirstOrDefault(f => f.GroupId == custGroupId);
            var result = new CustomerListViewModel(hqGroup);

            return PartialView("../AR/Customer/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte custGroupId)
        {
            using var db = new VPContext();
            var hqGroup = db.HQGroups.FirstOrDefault(f => f.GroupId == custGroupId);
            var result = new CustomerListViewModel(hqGroup);

            return PartialView("../AR/Customer/Search/Table", result);
        }

        [HttpPost]
        public JsonResult SearchReturn(CustomerViewModel model)
        {
            if (model == null)
            {
                model = new CustomerViewModel();
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.CustomerId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Combo(byte custGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.Customers
                        .Where(f => f.CustGroupId == custGroupId && (f.Status == "A" || f.TempYN == "Y") && f.CustomerId < 90000)
                        .OrderBy(o => o.Name)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Customer"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CustomerId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}{1}", s.CustomerId > 90000 ? "(New) " : "", s.Name),
                Selected = s.CustomerId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.Status == "A" || s.CustomerId > 90000 ? "Active" : "Inactive",
                    Disabled = !(s.Status == "A" || s.CustomerId > 90000)
                },

            }).ToList());
            //result.AddRange(CustomerRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerCombo(byte custGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.Customers
                        .Where(f => f.CustGroupId == custGroupId && (f.Status == "A" || f.TempYN == "Y") && f.CustomerId < 90000)
                        .OrderBy(o => o.Name)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Customer"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CustomerId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}{1}", s.CustomerId > 90000 ? "(New) " : "", s.Name),
                Selected = s.CustomerId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.Status == "A" || s.CustomerId > 90000 ? "Active" : "Inactive",
                    Disabled = !(s.Status == "A" || s.CustomerId > 90000)
                },

            }).ToList());
            //result.AddRange(CustomerRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult PayTermsCombo()
        {
            using var db = new VPContext();
            var list = db.PayTerms
                        .OrderBy(o => o.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Terms"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.PayTerms.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AllCombo(byte custGroupId, string selected)
        {
            //using var repo = new CustomerRepository();
            using var db = new VPContext();
            var list = db.Customers
                        .Where(f => f.CustGroupId == custGroupId && (f.Status == "A" || f.TempYN == "Y"))
                        .OrderBy(o => o.Name)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Customer"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CustomerId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}{1}", s.CustomerId > 90000 ? "(New) " : "", s.Name),
                Selected = s.CustomerId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.Status == "A" || s.CustomerId > 90000 ? "Active" : "Inactive",
                    Disabled = !(s.Status == "A" || s.CustomerId > 90000)
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BidCombo(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(s => s.BDCo == bdco && s.BidId == bidId);
            var list = bid.Customers.Select(s => s.Customer).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Customer"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CustomerId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}{1}", s.CustomerId > 90000 ? "(New) " : "", s.Name),
                Group = new SelectListGroup
                {
                    Name = s.Status == "A" || s.CustomerId > 90000 ? "Active" : "Inactive",
                    Disabled = !(s.Status == "A" || s.CustomerId > 90000)
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult ContactCombo(byte custGroupId, int? customerId = null, string selected = null)
        {

            if (customerId != null)
            {
                using var db = new VPContext();
                var list = db.CustomerContacts.Where(f => f.CustGroupId == custGroupId && f.CustomerId == customerId).Select(s => s.Contact).ToList()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ContactId.ToString(AppCultureInfo.CInfo()),
                        Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.FirstName, s.LastName),
                        Selected = s.ContactId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                    }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Contact"
                    }
                };
                result.AddRange(list);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "No customer selected"
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
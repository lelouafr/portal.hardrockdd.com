using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class HQComboController : BaseController
    {

        [HttpGet]
        public JsonResult HQCompanyCombo()
        {
            var memKey = "HQCombo_HQCompanyCombo";

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
            };

            if (!(MemoryCache.Default[memKey] is List<DB.Infrastructure.ViewPointDB.Data.HQCompanyParm> list))
            {
                using var db = new VPContext();
                list = db.HQCompanyParms
                        .Where(f => f.HQCo != 2 &&
                                    f.HQCo != 3)
                        .ToList();

                systemCache.Set(memKey, list, policy);
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Company",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.HQCo.ToString(),
                Text = string.IsNullOrEmpty(s.CompanyLabel) ? s.Name : s.CompanyLabel,

            }).OrderBy(o => o.Text).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UMCombo(string selected)
        {
            using var db = new VPContext();

            var list = db.UnitofMeasures.Select(s => new SelectListItem
            {
                Value = s.UM,
                Text = s.UM,
                Selected = s.UM == selected ? true : false
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select UM"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OfficeCombo()
        {
            using var db = new VPContext();

            var list = db.HQOffices.Select(s => new SelectListItem
            {
                Value = s.OfficeId.ToString(),
                Text = s.Description,
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Office"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult StateCombo(string country)
        {
            using var db = new VPContext();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select State"
                }
            };
            country ??= "US";

            var list = db.HQStates
                        .Where(f => f.Country == country)
                        .OrderBy(o => o.State)
                        .Select(s => new SelectListItem {
                            Value = s.State,
                            Text = s.Name,
                        }).ToList();
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TaxCombo(byte hqco)
        {
            using var db = new VPContext();
            var results = db.TaxCodes.Where(f => f.TaxGroupId == hqco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select TaxCode"
                }
            };
            var list = results.OrderBy(o => o.TaxCodeId)
                           .Select(s => new SelectListItem
                           {
                               Value = s.TaxCodeId,
                               Text = s.Description,
                           }).ToList();
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HQBatchTransType()
        {

            var list = StaticFunctions.GetComboValues("BatchTransType");
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.DatabaseValue,
                Text = s.DisplayValue
            }).ToList()
            );

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
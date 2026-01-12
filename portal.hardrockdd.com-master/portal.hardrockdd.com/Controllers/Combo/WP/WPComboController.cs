using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class WPComboController : BaseController
    {

        [HttpGet]
        public JsonResult WPDivisionCombo()
        {
            var memKey = "WPCombo_WPDivisionCombo";

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
            };

            if (!(MemoryCache.Default[memKey] is List<CompanyDivision> list))
            {
                using var db = new VPContext();
                list = db.CompanyDivisions
                        .Where(f => f.HQCo != 2 &&
                                    f.HQCo != 3 &&
                                    f.IsActive)
                        .ToList();

                systemCache.Set(memKey, list, policy);
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Division",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.DivisionId.ToString(),
                Text = s.Description,

            }).OrderBy(o => o.Text).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WPUserCombo()
        {

            var memKey = "Combo_WPUserCombo";
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
            };

            if (!(MemoryCache.Default[memKey] is List<SelectListItem> result))
            {

                using var db = new VPContext();
                var list = db.WebUsers
                    .Include("Employee")
                    .ToList();

                result = list.Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.FullName(),
                    Group = new SelectListGroup()
                    {
                        Disabled = !s.Active,
                        Name = s.Active ? "Active": "Disabled"
                    }

                }).OrderBy(o => o.Text).ToList();
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WPUserCompanyCombo()
        {
            //var memKey = "WPCombo_WPCompanyCombo";

            //ObjectCache systemCache = MemoryCache.Default;
            //CacheItemPolicy policy = new CacheItemPolicy
            //{
            //    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
            //};

            //if (!(MemoryCache.Default[memKey] is List<HQCompanyParm> list))
            //{
            //    using var db = new VPContext();
            //    list = db.HQCompanyParms
            //            .Where(f => f.HQCo != 2 &&
            //                        f.HQCo != 3 &&
            //                        f.HQCo != 101)
            //            .ToList();

            //    systemCache.Set(memKey, list, policy);
            //}
            using var db = new VPContext();
            var usr = StaticFunctions.GetCurrentUser();
            var user = db.WebUsers
                .Include("DivisionLinks")
                .Include("DivisionLinks.Division")
                .Include("DivisionLinks.Division.HQCompany")
                .FirstOrDefault(f => f.Id == usr.Id);

            var list = user.DivisionLinks.Select(s => s.Division.HQCompany).Distinct().ToList();
            
            var result = list.Select(s => new SelectListItem
            {
                Value = s.HQCo.ToString(),
                Text = s.CompanyLabel ?? s.Name,

            }).OrderBy(o => o.Text).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WPUserDivisionCombo()
        {
            using var db = new VPContext();
            var usr = StaticFunctions.GetCurrentUser();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == usr.Id);

            var list = user.DivisionLinks.Select(s => s.Division).ToList();

            var result = list.Select(s => new SelectListItem
            {
                Value = s.DivisionId.ToString(),
                Text = s.Description,

            }).OrderBy(o => o.Text).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
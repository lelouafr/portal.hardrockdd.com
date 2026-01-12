using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class WAComboController : BaseController
    {
        //210-315-3528
        [HttpGet]
        public JsonResult WAApplicantCombo(string positionCodeId)
        {
            //var memKey = string.Format("WACombo_WAAppicantCombo_{0}", positionCodeId);

            //if (MemoryCache.Default[memKey] is List<SelectListItem> result)
            //    return Json(result, JsonRequestBehavior.AllowGet);

            using var db = new VPContext();
            var list = db.WebApplicants.Where(f => f.Applications.Any(a => a.AppliedPositions.Any(p => p.tPositionCodeId == positionCodeId))).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Applicant",
                }
            };

            result.AddRange(list.Select(s => s.SelectListItem()).OrderBy(o => o.Text).ToList());

            //ObjectCache systemCache = MemoryCache.Default;
            //CacheItemPolicy policy = new CacheItemPolicy
            //{
            //    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1)
            //};

            //systemCache.Set(memKey, result, policy);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WAApprovedAppicantCombo(string positionCodeId)
        {
            //var memKey = string.Format("WACombo_WAApprovedAppicantCombo_{0}", positionCodeId);

            //if (MemoryCache.Default[memKey] is List<SelectListItem> result)
            //    return Json(result, JsonRequestBehavior.AllowGet);

            using var db = new VPContext();
            var list = db.WebApplicants.Where(f => f.Applications.Any(a => (a.tStatusId == (int)DB.WAApplicationStatusEnum.Approved || 
                                                                            a.tStatusId == (int)DB.WAApplicationStatusEnum.Shelved) &&
                                                                            a.AppliedPositions.Any(p => p.tPositionCodeId == positionCodeId)
                        )).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Applicant",
                }
            };

            result.AddRange(list.Select(s => s.SelectListItem()).OrderBy(o => o.Text).ToList());

            //ObjectCache systemCache = MemoryCache.Default;
            //CacheItemPolicy policy = new CacheItemPolicy
            //{
            //    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1)
            //};

            //systemCache.Set(memKey, result, policy);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.JV
{
    [Authorize]
    public class GLAccountController : BaseController
    {

        [HttpGet]
        public JsonResult Combo(byte glco, string selected)
        {
            using var db = new VPContext();
            var list = db.GLAccounts.Where(f => f.GLCo == glco && f.Active == "Y").ToList();

            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.GLAcct,
                Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.GLAcct, s.Description),
                Selected = s.GLAcct == selected ? true : false
            }).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Account"
                }
            };
            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CCCombo(byte glco, int transId, string selected)
        {
            using var db = new VPContext();
            var list = new List<GLAccount>();

            list = db.GLAccounts.Where(f => f.GLCo == glco && f.Active == "Y" && (f.AcctType == "E" || f.AcctType == "A")).ToList();
            var enabledList = list.Where(f => f.AcctType == "E" && string.IsNullOrEmpty(f.SubType) && f.POCCActiveYN == "Y").ToList();
            if (User.HasAccess("APCreditCardAdministrationTransaction", "Index"))
            {
                enabledList = list;
            }
            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.GLAcct,
                Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.GLAcct, s.Description),
                Selected = s.GLAcct == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = !enabledList.Any(a => a.GLAcct == s.GLAcct) ? "Disabled" : "Enabled",
                    Disabled = !enabledList.Any(a => a.GLAcct == s.GLAcct)
                }
            }).OrderByDescending(o => o.Group.Name)
            .ThenBy(o => o.Text)
            .ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Account"
                }
            };
            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult POCombo(byte glco, int transId, string selected)
        {
            using var db = new VPContext();
            var list = new List<GLAccount>();

            list = db.GLAccounts.Where(f => f.GLCo == glco && f.Active == "Y" && (f.AcctType == "E" || f.AcctType == "A")).ToList();
            var enabledList = list.Where(f => f.AcctType == "E" && string.IsNullOrEmpty(f.SubType) && f.POCCActiveYN == "Y").ToList();
            if (User.HasAccess("Index", "APCreditCardAdministrationTransaction"))
            {
                enabledList = list;
            }
            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.GLAcct,
                Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.GLAcct, s.Description),
                Selected = s.GLAcct == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = !enabledList.Any(a => a.GLAcct == s.GLAcct) ? "Disabled" : "Enabled",
                    Disabled = !enabledList.Any(a => a.GLAcct == s.GLAcct)
                }
            }).OrderByDescending(o => o.Group.Name)
            .ThenBy(o => o.Text)
            .ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Account"
                }
            };
            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult APMthCombo(byte? glco, string? selected)
        {
            glco ??= 1;
            using var db = new VPContext();
            var parms = db.GLParameters.FirstOrDefault(f => f.GLCo == glco);
            var endDate = System.DateTime.Now.AddDays(7);

            var list = db.GLFinPeriods
                .Where(f => f.GLCo == glco && f.Mth <= endDate)
                .OrderByDescending(o => o.Mth)
                .ToList();//&& f.Mth > parms.LastMthAPClsd

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Period"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.Mth.ToShortDateString(),
                Text = string.Format(AppCultureInfo.CInfo(), "{0:MMMM yyyy}", s.Mth),
                Selected = s.Mth.ToShortDateString() == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.Mth > parms.LastMthAPClsd ? "" : string.Format(AppCultureInfo.CInfo(), "Closed {0:yyyy}", s.Mth),
                    Disabled = !(s.Mth > parms.LastMthAPClsd)
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult APAllMthCombo(byte glco, string selected)
        {
            //using var db = new VPContext();
            //var endDate = System.DateTime.Now.AddDays(7);

            //var list = db.GLFinPeriods
            //    .Where(f => f.GLCo == co && f.Mth <= endDate)
            //    .OrderByDescending(o => o.Mth)
            //    .ToList();

            //var result = new List<SelectListItem>
            //{
            //    new SelectListItem
            //    {
            //        Text = "Select Period"
            //    }
            //};

            //result.AddRange(list.Select(s => new SelectListItem
            //{
            //    Value = s.Mth.ToShortDateString(),
            //    Text = string.Format(AppCultureInfo.CInfo(), "{0:MMMM yyyy}", s.Mth),
            //    Selected = s.Mth.ToShortDateString() == selected ? true : false,
            //    Group = new SelectListGroup
            //    {
            //        Name = string.Format(AppCultureInfo.CInfo(), "{0:yyyy}", s.Mth),
            //        Disabled = false
            //    },
            //}).ToList());
            using var db = new VPContext();
            var parms = db.GLParameters.FirstOrDefault(f => f.GLCo == glco);
            var endDate = System.DateTime.Now.AddDays(7);

            var list = db.GLFinPeriods
                .Where(f => f.GLCo == glco && f.Mth <= endDate)
                .OrderByDescending(o => o.Mth)
                .ToList();//&& f.Mth > parms.LastMthAPClsd

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Period"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.Mth.ToShortDateString(),
                Text = string.Format(AppCultureInfo.CInfo(), "{0:MMMM yyyy}", s.Mth),
                Selected = s.Mth.ToShortDateString() == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.Mth > parms.LastMthAPClsd ? "" : string.Format(AppCultureInfo.CInfo(), "Closed {0:yyyy}", s.Mth),
                    Disabled = false//!(s.Mth > parms.LastMthAPClsd)
                },

            }).ToList()) ;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
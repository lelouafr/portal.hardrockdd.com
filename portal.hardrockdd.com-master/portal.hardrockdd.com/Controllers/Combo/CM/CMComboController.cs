using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Vendor;
using portal.Repository.VP.AP;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.CM
{
    [ValidateAntiForgeryTokenOnAllPosts]
    public class CMComboController : BaseController
    {

        [HttpGet]
        public JsonResult CMAccountCombo(byte? cmco)
        {
            if (cmco == null)
            {
                cmco = 1;
            }
            using var db = new VPContext();
            var list = db.CMAccounts
                        .Where(f => f.CMCo == cmco)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select CMAccount"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CMAcct.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CMAcct, s.Description),

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
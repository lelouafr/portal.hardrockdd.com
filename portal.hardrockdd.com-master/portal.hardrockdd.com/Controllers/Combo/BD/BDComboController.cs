using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class BDComboController : BaseController
    {
        [HttpGet]
        public JsonResult BoreTypeCombo(byte bdco)
        {
            using var db = new VPContext();
            var codes = db.BidBoreTypes.Where(f => f.BDCo == bdco && f.BoreTypeId != 3).ToList();

            var list = codes.Select(s => new SelectListItem
            {
                Value = s.BoreTypeId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
            }).ToList();

            var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Bore Type"
                    }
                };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult BoreTypeLineCombo(byte bdco)
        {
            using var db = new VPContext();
            var codes = db.BidBoreTypes.Where(f => f.BDCo == bdco).ToList();

            var list = codes.Select(s => new SelectListItem
            {
                Value = s.BoreTypeId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
            }).ToList();

            var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Bore Type"
                    }
                };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult BidCombo()
        {
            using var db = new VPContext();
            var codes = db.vBids
                .Where(f => f.Status != (int)DB.BidStatusEnum.Deleted && f.Status != (int)DB.BidStatusEnum.Canceled)
                .OrderByDescending(f => f.BidId)
                .ToList();

            var list = codes.Select(s => new SelectListItem
            {
                Value = s.BidId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format("{0}: {1}", s.BidId, s.Description),
            }).ToList();

            var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Bid"
                    }
                };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
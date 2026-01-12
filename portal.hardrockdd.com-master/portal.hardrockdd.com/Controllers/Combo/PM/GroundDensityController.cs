using portal.Repository.VP.PM;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class GroundDensityController : BaseController
    {
        [HttpGet]
        public JsonResult Combo(byte bdco, string selected)
        {
            using var repo = new GroundDensityRepository();
            var codes = repo.GetGroundDensities(bdco);
            var list = GroundDensityRepository.GetSelectList(codes, selected);

            var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Ground Density"
                    }
                };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ComboNoDirt(byte bdco, string selected)
        {
            using var repo = new GroundDensityRepository();
            var codes = repo.GetGroundDensities(bdco).Where(w => w.GroundDensityId != 0).ToList();
            var list = GroundDensityRepository.GetSelectList(codes, selected);

            var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Ground Density"
                    }
                };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
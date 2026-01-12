using portal.Repository.VP.PM;
using System.Collections.Generic;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class DivisionController : BaseController
    {
        [HttpGet]
        public JsonResult Combo(byte pmco, string selected)
        {
            using var repo = new ProjectDivisionRepository();
            var list = repo.GetProjectDivisions(pmco);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Division"
                }
            };
            result.AddRange(ProjectDivisionRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
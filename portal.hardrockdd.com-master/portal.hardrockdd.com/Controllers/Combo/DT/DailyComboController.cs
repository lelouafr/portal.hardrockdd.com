using System.Collections.Generic;
using System.Web.Mvc;

namespace portal.Controllers.VP.JV
{
    [Authorize]
    public class DailyComboController : BaseController
    {


        [HttpGet]
        public JsonResult GroundConditionCombo(string selected)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Dirt", Text = "Dirt", Selected = selected == "Dirt"},
                new SelectListItem(){ Value = "Rock", Text = "Rock", Selected = selected == "Rock"}
            };

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Ground Condition"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WeatherConditionCombo(string selected)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Clear",Text = "Clear", Selected = selected == "Clear" },
                new SelectListItem(){ Value = "Partly Sunny",Text = "Partly Sunny", Selected = selected == "Partly Sunny" },
                new SelectListItem(){ Value = "Cloudy/Overcast",Text = "Cloudy/Overcast", Selected = selected == "Cloudy/Overcast" },
                new SelectListItem(){ Value = "Raining",Text = "Raining", Selected = selected == "Raining" },
                new SelectListItem(){ Value = "Scattered Shower",Text = "Scattered Shower", Selected = selected == "Scattered Shower" },
                new SelectListItem(){ Value = "Hot/Extreme Heat",Text = "Hot/Extreme Heat", Selected = selected == "Hot/Extreme Heat" },
                new SelectListItem(){ Value = "Ice/Snow",Text = "Ice/Snow", Selected = selected == "Ice/Snow" }
            };

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Weather Condition"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
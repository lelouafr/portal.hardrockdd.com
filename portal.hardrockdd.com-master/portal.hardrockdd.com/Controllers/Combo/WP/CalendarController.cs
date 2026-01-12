using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.WP
{
    [Authorize]
    public class CalendarController : BaseController
    {
        [HttpGet]
        public JsonResult WeekCombo(string selected)
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();

            var list = db.Calendars.Where(f => f.Year >= DateTime.Now.Year-1 && f.Date <= DateTime.Now)
                                    .GroupBy(g => new {  g.Week, g.WeekDescription })   
                                    .OrderByDescending(o => o.Key.Week)
                                    .ToList()
                                    .Select(s => new SelectListItem {
                                                Value = s.Key.Week.ToString(),
                                                Text = s.Key.WeekDescription,
                                                Selected = s.Key.Week.ToString() == selected ? true : false
                                            })
                                    .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Week"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
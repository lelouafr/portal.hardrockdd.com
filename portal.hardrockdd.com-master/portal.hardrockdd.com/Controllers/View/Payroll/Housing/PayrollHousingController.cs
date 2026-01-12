using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Employee.Housing;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.EmployeeHouse
{
    [ControllerAuthorize]
    public class PayrollHousingController : BaseController
    {
        [HttpGet]
        [Route("Employee/House")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var date = DateTime.Now.Date.AddDays(-7);
            var dateFilter = db.Calendars.Where(f => f.Date == date).Max(max => max.Date);
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var results = new EmployeeHousingListViewModel(StaticFunctions.GetCurrentCompany().HQCo, weekId, db);
            return View("../PR/House/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table(byte prco, int weekId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var results = new EmployeeHousingListViewModel(prco, weekId, db);
            return PartialView("../PR/House/Summary/List/Table", results);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(EmployeeHousingViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            var weekStart = db.Calendars.Where(f => f.Week == model.WeekId).FirstOrDefault();
                
            int? val;
            for (int i = 1; i <= 7; i++)
            {
                val = i switch
                {
                    1 => model.SaturdayHouse,
                    2 => model.SundayHouse,
                    3 => model.MondayHouse,
                    4 => model.TuesdayHouse,
                    5 => model.WednesdayHouse,
                    6 => model.ThursdayHouse,
                    7 => model.FridayHouse,
                    _ => null,
                };
                var workDate = weekStart.Date.AddDays(i - 1);
                var updObj = db.EmployeeHouses.FirstOrDefault(f => f.Co == model.PRCo && f.EmployeeId == model.EmployeeId && f.WorkDate == workDate);
                if (updObj != null)
                {
                    updObj.HouseTypeId = val;
                }
                else
                {
                    updObj = new DB.Infrastructure.ViewPointDB.Data.EmployeeHouse
                    {
                        Co = model.PRCo,
                        EmployeeId = (int)model.EmployeeId,
                        WorkDate = workDate,
                        WeekId = (int)model.WeekId,
                        HouseTypeId = val
                    };
                    db.EmployeeHouses.Add(updObj);
                }
            }
            db.SaveChanges(ModelState);

            var result = model;

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }
    }
}
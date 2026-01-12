using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Equipment;
using portal.Models.Views.Payroll;
using portal.Repository.VP.DT;
using portal.Repository.VP.EM;
using portal.Repository.VP.HQ;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.Payroll
{
    //[AuthorizePosition(PositionCodes = "FIN-CTRL,CFO,COO,PRES,IT-DIR")]
    public class DailyEquipmentUsageController : BaseController
    {
        [HttpGet]
        [Route("Equipment/Usage/Review")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var dateFilter = db.PayrollEntries.Max(max => max.PREndDate).AddDays(1);
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var comp = StaticFunctions.GetCurrentCompany();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new DailyEquipmentUsageListViewModel(result, weekId, db);
            return View("../EM/EMUsage/Index", model);
        }

        [HttpGet]
        public ActionResult Table(int weekId)
        {
            using var db = new VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();

            var comp = StaticFunctions.GetCurrentCompany();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new DailyEquipmentUsageListViewModel(result, weekId, db);
            return PartialView("../EM/EMUsage/Table",model);
        }

        [HttpGet]
        public ActionResult Panel(int weekId)
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new DailyEquipmentUsageListViewModel(result, weekId, db);
            return PartialView("../EM/EMUsage/Panel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte dtco, int equipmentId, int weekId, int status)
        {
            //using var db = new VPContext();
            //foreach (var entry in db.DTPayrollHours.Where(f => f.Co == co && f.EmployeeId == employeeId && 
            //                                                               f.Calendar.Week == weekId &&
            //                                                               f.Status != (int)DB.PayrollEntryStatusEnum.Posted &&
            //                                                               f.Status != (int)DB.PayrollEntryStatusEnum.Reversal)
            //                                        .ToList())
            //{
            //    entry.Status = payrollStatus;
            //}
            //foreach (var entry in db.PayrollPerdiems.Where(f => f.Co == co && f.EmployeeId == employeeId &&
            //                                                               f.Calendar.Week == weekId &&
            //                                                               f.Status != (int)DB.PayrollEntryStatusEnum.Posted &&
            //                                                               f.Status != (int)DB.PayrollEntryStatusEnum.Reversal)
            //                                        .ToList())
            //{
            //    entry.Status = payrollStatus;
            //}
            //db.SaveChanges(ModelState);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        public ActionResult AddtoBatchList(List<DailyEquipmentUsageListViewModel> List)
        {
            using var db = new VPContext();
            var weekId = List.Max(max => max.WeekId);
            var dtco = List.Max(max => max.DTCo);
            var batchList = new List<EMBatchUsage>();

            var postList = db.DailyEquipmentUsages.Where(f => f.DTCo == dtco && f.Calendar.Week == weekId && f.Status == 0).ToList();
            foreach (var item in postList)
            {
                batchList.Add(item.AddToBatch());
            }
            db.BulkSaveChanges();

            var batches = batchList.GroupBy(g => g.Batch).Select( s=> s.Key) .ToList();
            foreach (var batch  in batches)
            {
                batch.InUseBy = null;
            }
            db.BulkSaveChanges();

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult EquipmentWeekIndex(byte dtco, int weekId, string equipmentId)
        {
            using var db = new VPContext();
            var model = new DailyEquipmentUsageListViewModel(dtco, weekId, equipmentId, db);
            return PartialView("../EM/EMUsage/Lines/Index", model);
        }

        [HttpGet]
        public ActionResult LineTablePanel(byte dtco, int weekId, string equipmentId)
        {
            using var db = new VPContext();
            var model = new DailyEquipmentUsageListViewModel(dtco, weekId, equipmentId, db);
            return PartialView("../EM/EMUsage/Lines/Panel", model);
        }

        [HttpGet]
        public ActionResult LineTable(byte dtco, int weekId, string equipmentId)
        {
            using var db = new VPContext();
            var model = new DailyEquipmentUsageListViewModel(dtco, weekId, equipmentId, db);

            return PartialView("../EM/EMUsage/Lines/Table", model);
        }

        [HttpGet]
        public ActionResult LinePanel(byte dtco, int ticketId, int seqId)
        {
            using var db = new VPContext();

            var results = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.SeqId == seqId);
            var model = new DailyEquipmentUsageViewModel(results);

            return PartialView("../EM/EMUsage/Lines/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult LineForm(byte dtco, int ticketId, int seqId)
        {
            using var db = new VPContext();
            var results = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.SeqId == seqId);
            var model = new DailyEquipmentUsageViewModel(results);

            return PartialView("../EM/EMUsage/Lines/Form/Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineFormUpdate(DailyEquipmentUsageViewModel model)
        {
            using var db = new VPContext();
            var updObj = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.SeqId == model.SeqId);
            if (updObj != null)
            {
                DailyEquipmentUsageRepository.ProcessUpdate(updObj, model);
                db.SaveChanges(ModelState);
            }
            var result = new DailyEquipmentUsageViewModel(updObj);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }
    }
}
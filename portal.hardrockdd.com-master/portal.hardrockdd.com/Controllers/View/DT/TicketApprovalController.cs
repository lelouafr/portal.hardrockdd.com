using Microsoft.AspNet.Identity;
using portal.Models.Views.DailyTicket;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyTicketApprovalController : BaseController
    {
        [HttpGet]
        [Route("Ticket/Approval")]
        public ActionResult Index()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var userEmplId = StaticFunctions.GetCurrentEmployee().EmployeeId;
            var isAdmin = HttpContext.User.IsInPosition("IT-DIR,HR-MGR");
            var tickets = db.DailyTickets.Where(f => f.tStatusId == (short)DB.DailyTicketStatusEnum.Submitted).ToList().Where(f => 
                                                     (
                                                        (f.FormType != DB.DTFormEnum.JobFieldTicket && f.DailyJobTicket == null &&
                                                            (
                                                                f.CreatedUser.Employee.FirstOrDefault().Supervisor?.WebId == userId ||
                                                                f.CreatedUser.Employee.FirstOrDefault().Supervisor?.Supervisor.WebId == userId
                                                            )
                                                        ) ||
                                                        (f.FormType == DB.DTFormEnum.JobFieldTicket && f.DailyJobTicket != null &&
                                                            (
                                                                f.DailyJobTicket.Job.Division.WPDivision?.ManagerId == userEmplId ||
                                                                f.DailyJobTicket.Job.Division.WPDivision?.DivisionManger.Supervisor.EmployeeId == userEmplId
                                                            )
                                                        ) ||
                                                        isAdmin
                                                     )
                                                     );

            var results = tickets.AsEnumerable().Select(s => new DailyTicketApprovalViewModel(s)).OrderBy(o => o.WorkDate).ToList();
            //if (results == null || results.Count == 0)
            //{
            //    results.Add(new DailyTicketApprovalViewModel());
            //}
            return View("../DT/Approval/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var isAdmin = HttpContext.User.IsInPosition("IT-DIR,HR-MGR");
            var tickets = db.DailyTickets.Where(f => f.tStatusId == (short)DB.DailyTicketStatusEnum.Submitted &&
                                                     (
                                                     f.CreatedUser.Employee.FirstOrDefault().Supervisor.WebId == userId ||
                                                     f.CreatedUser.Employee.FirstOrDefault().Supervisor.Supervisor.WebId == userId ||
                                                     isAdmin
                                                     )
                                                     );

            var results = tickets.AsEnumerable().Select(s => new DailyTicketApprovalViewModel(s)).OrderBy(o => o.WorkDate).ToList();

            return PartialView("../DT/Approval/List/Table", results);
        }
    }
}
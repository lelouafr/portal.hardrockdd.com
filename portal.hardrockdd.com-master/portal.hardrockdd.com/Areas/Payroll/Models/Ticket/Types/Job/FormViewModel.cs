using DB.Infrastructure.ViewPointDB.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Job
{

    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DailyTicket ticket)
        {
            Ticket = new TicketViewModel(ticket);
            Employees = new EmployeeListViewModel(ticket);
            Phases = new JobPhaseListViewModel(ticket.DailyJobTicket);
            StatusLogs = new StatusLogListViewModel(ticket);

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;
			UniqueAttchID = ticket.Attachment.UniqueAttchID;
		}

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == Ticket.DTCo && f.TicketId == Ticket.TicketId);
            var ok = true;

            var jobTicket = ticket.DailyJobTicket;
            var productionHours = jobTicket.PayrollHourTasks();
            var taskTotHours = productionHours.Sum(s => s.PayrollValue);

            var userMaxHours = Employees.List.Max(m => m.Hours);
            if (taskTotHours != userMaxHours)
            {
                modelState.AddModelError("", "Task hours do not add up to max employee Hours");
                ok &= false;
            }

            var perDiemCnt = Employees.List.Sum(sum => (int)sum.PerDiem);
            if (jobTicket.DTJobPhases.Count == 0 && (perDiemCnt > 0 || taskTotHours > 0))
            {
                modelState.AddModelError("", "You must have a task if hours or per diem is applied to an employee");
                ok &= false;
            }

            if (!jobTicket.Rig.RevenueCodes.Any(r => r.RevCode == (jobTicket.Rig.RevenueCodeId ?? "2")))
            {
                modelState.AddModelError("", "The rig selected does not have rates, cannot be use in ticket");
                ok &= false;
            }

            foreach (var task in jobTicket.DTJobPhases)
            {
                if (task.JobPhase.HasBudgetRadius() && task.ProgressBudgetCode == null)
                {
                    task.UpdateProgressBudgetCode();
                    task.db.SaveChanges();
                }
                if (task.JobPhase.HasBudgetRadius() && task.ProgressBudgetCode == null)
                {
                    modelState.AddModelError("", "You are missing a pilot size or ream size under tasks");
                    ok &= false;
                    break;
                }
            }

            var audit = ticket.CreatedUser.ActiveEquipmentAudits().FirstOrDefault();

            if (audit != null && audit.IsAuditLate(ticket))
            {
                modelState.AddModelError("", "You have a Late Equipment Audit, must complete Audit before submitting Ticket!");
            }

            return ok;
        }

		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public TicketViewModel Ticket { get; set; }

        public EmployeeListViewModel Employees { get; set; }

        public JobPhaseListViewModel Phases { get; set; }

        public StatusLogListViewModel StatusLogs { get; set; }

        public System.Guid? UniqueAttchID { get; set; }

	}
}
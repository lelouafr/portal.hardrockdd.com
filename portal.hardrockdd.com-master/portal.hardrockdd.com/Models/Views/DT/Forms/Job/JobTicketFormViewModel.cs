using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class JobTicketFormViewModel : TicketForm
    {
        public JobTicketFormViewModel()
        {

        }

        public JobTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyJobTicketViewModel(ticket);
            Employees = new DailyJobEmployeeListViewModel(ticket);
            //Tasks = new DailyJobTaskListViewModel(ticket);

            Phases = new Job.DailyJobPhaseListViewModel(ticket.DailyJobTicket);
            //Equipments = new DailyEquipmentListViewModel();
            StatusLogs = new DailyStatusLogListViewModel(ticket);
            //JobProgress = new JobProductionBgtVsActListViewModel(ticket);
            //POUsages = new DailyPOUsageListViewModel(ticket);
            //Attachments = new Attachment.AttachmentListViewModel(ticket.DTCo, "udDTTM", ticket.KeyID, ticket.UniqueAttchID);

            UniqueAttchID = ticket.Attachment.UniqueAttchID;
            Audit = new Views.Equipment.Audit.EquipmentAuditMeterFormViewModel(ticket);
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
            //var productionHours = jobTicket.DTJobPhases.SelectMany(s => s.DTCostLines).Where(f => f.JobPhaseCost.UnitofMeasure.Description == "Hours" && f.JobPhaseCost.CostType.JBCostTypeCategory == "L").ToList();
            var productionHours = jobTicket.PayrollHourTasks();
            var taskTotHours = productionHours.Sum(s => s.PayrollValue);

            //var totalHRs = Tasks.Tasks.Where(f => (f.UnitofMeasure == "Hours" || f.UnitofMeasure == "HRS") && f.CostTypeCategory == "L").Sum(s => s.Value);
            var userMaxHours = Employees.Employees.Max(m => m.Hours);
            if (taskTotHours != userMaxHours)
            {
                modelState.AddModelError("", "Task hours do not add up to max employee Hours");
                ok &= false;
            }

            foreach (var emp in ticket.DailyJobEmployees.ToList())
            {
                if (emp.PREmployee.HREmployee.Position == null)
					modelState.AddModelError("", string.Format("Employee {0} needs to be updated they have no position code", emp.PREmployee.FullName()));
			}

            var perdiemCnt = Employees.Employees.Sum(sum => (int)(sum.Perdiem ?? 0));
            if (jobTicket.DTJobPhases.Count == 0 && (perdiemCnt > 0 || taskTotHours > 0))
            {
                modelState.AddModelError("", "You must have a task if hours or perdiem is applied to an employee");
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

        public DailyJobTicketViewModel Ticket { get; set; }

        public DailyJobEmployeeListViewModel Employees { get; set; }


        //public DailyJobTaskListViewModel Tasks { get; set; }


        public Job.DailyJobPhaseListViewModel Phases { get; set; }

        public DailyEquipmentListViewModel Equipments { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }

        //public JobProductionBgtVsActListViewModel JobProgress { get; set; }

        public DailyPOUsageListViewModel POUsages { get; set; }

        //public Attachment.AttachmentListViewModel Attachments { get; set; }

        public System.Guid? UniqueAttchID { get; set; }

        public Views.Equipment.Audit.EquipmentAuditMeterFormViewModel Audit { get; set; }
    }
}
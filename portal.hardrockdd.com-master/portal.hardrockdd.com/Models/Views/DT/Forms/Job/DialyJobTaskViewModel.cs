using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Job
{
    public class DailyJobPhaseListViewModel
    {

        public DailyJobPhaseListViewModel()
        {

        }

        public DailyJobPhaseListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyJobTicket jobTicket)
        {
            if (jobTicket == null)
                return;

            DTCo = jobTicket.DTCo;
            TicketId = jobTicket.TicketId;
            AddTask = new AddDailyJobPhaseCostViewModel(jobTicket.DailyTicket);
            List = jobTicket.DTJobPhases.Select(s => new DailyJobPhaseViewModel(s)).ToList();
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        public AddDailyJobPhaseCostViewModel AddTask { get; set; }

        public List<DailyJobPhaseViewModel> List { get; }
    }

    public class DailyJobPhaseViewModel
    {

        public DailyJobPhaseViewModel()
        {

        }

        public DailyJobPhaseViewModel(DB.Infrastructure.ViewPointDB.Data.DTJobPhase dailyPhase)
        {
            if (dailyPhase == null)
                return;

            DTCo = dailyPhase.DTCo;
            TicketId = dailyPhase.TicketId;
            TaskId = dailyPhase.TaskId;
            WorkDate = dailyPhase.WorkDate;
            PhaseId = dailyPhase.PhaseId;
            PhaseDescription = dailyPhase.JobPhase.Description;

            Radius = dailyPhase.PassSize;
            Comments = dailyPhase.Comments;

            Costs = dailyPhase.DTCostLines.Select(s => new DailyJobPhaseCostViewModel(s)).ToList();
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int TaskId { get; set; }

        [HiddenInput]
        [Display(Name = "Work Date")]
        public DateTime? WorkDate { get; set; }

        [HiddenInput]
        [Display(Name = "Phase Group")]
        public byte? PhaseGroupId { get; set; }

        [Required]
        [Display(Name = "Phase")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", Placeholder = "Select Task", ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }

        [Display(Name = "Radius")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "PhaseGroupId,PhaseId")]
        public decimal? Radius { get; set; }

        [Display(Name = "Phase")]
        [UIHint("TextBox")]
        public string PhaseDescription { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }

        public List<DailyJobPhaseCostViewModel> Costs { get; }

    }

    public class DailyJobPhaseCostViewModel
    {
        public DailyJobPhaseCostViewModel()
        {

        }

        public DailyJobPhaseCostViewModel(DB.Infrastructure.ViewPointDB.Data.DTJobPhaseCost dailyPhaseCost)
        {
            if (dailyPhaseCost == null)
                return;

            var costType = dailyPhaseCost.JobPhaseCost.CostType;
            DTCo = dailyPhaseCost.DTCo;
            TicketId = dailyPhaseCost.TicketId;
            TaskId = dailyPhaseCost.TaskId;
            LineId = dailyPhaseCost.LineId;

            CostTypeId = dailyPhaseCost.JCCType;
            CostTypeDescription = costType.Description;
            UnitofMeasure = dailyPhaseCost.JobPhaseCost.UM;
            Value = dailyPhaseCost.PayrollValue;

            PhaseGroupId = dailyPhaseCost.PhaseGroupId;
            PhaseId = dailyPhaseCost.DailyJobTask.PhaseId;
            PhaseDescription = dailyPhaseCost.DailyJobTask.JobPhase.Description;

            Radius = dailyPhaseCost.DailyJobTask.PassSize;
            Comments = dailyPhaseCost.DailyJobTask.Comments;
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "TaskId")]
        public int TaskId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int LineId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string CostTypeDescription { get; set; }

        [Display(Name = "UM")]
        [UIHint("TextBox")]
        public string UnitofMeasure { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Value")]
        [Field(LabelSize = 2, TextSize = 10)]
        public decimal? Value { get; set; }

        [HiddenInput]
        [Display(Name = "Phase Group")]
        public byte? PhaseGroupId { get; set; }

        [Required]
        [Display(Name = "Phase")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", Placeholder = "Select Task", ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }

        [Display(Name = "Radius")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "PhaseGroupId,PhaseId")]
        public decimal? Radius { get; set; }

        [Display(Name = "Phase")]
        [UIHint("TextBox")]
        public string PhaseDescription { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }

        internal DailyJobPhaseCostViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DTJobPhaseCosts.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.TaskId == TaskId && f.LineId == LineId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                if (Comments != null)
                {
                    updObj.DailyJobTask.Comments = Comments;
                }
                else
                {
                    updObj.PayrollValue = Value;
                }
                if (Radius != null)
                {
                    updObj.DailyJobTask.PassSize = Radius;
                }

                try
                {
                    db.SaveChanges(modelState);
                    return new DailyJobPhaseCostViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }
    }


    public class AddDailyJobPhaseCostViewModel
    {
        public AddDailyJobPhaseCostViewModel()
        {

        }

        public AddDailyJobPhaseCostViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            JobId = ticket.DailyJobTicket.JobId;

            PhaseGroupId = ticket.HQCompanyParm.PhaseGroupId;
            //PhaseId = ticket.DailyJobTicket?.Job?.Phases?.FirstOrDefault()?.PhaseId;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [HiddenInput]
        [Required]
        [Display(Name = "Job Id")]
        public string JobId { get; set; }

        public byte? PhaseGroupId { get; set; }

        //[Required]
        [Display(Name = "Phase")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }

    }
}
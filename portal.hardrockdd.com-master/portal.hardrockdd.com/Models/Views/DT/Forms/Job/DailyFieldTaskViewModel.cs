//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.DailyTicket
//{
//    public class DailyJobTaskListViewModel
//    {
//        public DailyJobTaskListViewModel()
//        {
//            Tasks = new List<DailyJobTaskViewModel>();
//        }

//        public DailyJobTaskListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            DTCo = ticket.DTCo;
//            TicketId = ticket.TicketId;
//            Tasks = ticket.DailyJobTasks.Select(s => new DailyJobTaskViewModel(s)).OrderBy(o => o.PhaseId).ToList();

//            AddTask = new AddJobTaskViewModel(ticket);
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte DTCo { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Ticket Id")]
//        public int TicketId { get; set; }

//        public List<DailyJobTaskViewModel> Tasks { get; }

//        public AddJobTaskViewModel AddTask { get; set; }
//    }

//    public class DailyJobTaskViewModel : AuditBaseViewModel
//    {
//        public DailyJobTaskViewModel()
//        {

//        }

//        public DailyJobTaskViewModel(DB.Infrastructure.ViewPointDB.Data.DailyJobTask dailyJobTask): base(dailyJobTask)
//        {
//            if (dailyJobTask == null)
//            {
//                throw new System.ArgumentNullException(nameof(dailyJobTask));
//            }
//            DTCo = dailyJobTask.DTCo;
//            TicketId = dailyJobTask.TicketId;
//            LineNum = dailyJobTask.LineNum;
//            SeqId = dailyJobTask.SeqId;
//            WorkDate = dailyJobTask.WorkDate;
//            PhaseId = dailyJobTask.PhaseId;
//            PhaseDescription = dailyJobTask.JobPhase?.Description;
//            CostTypeId = dailyJobTask.CostTypeId;
//            CostTypeDescription = dailyJobTask.CostType?.Description ?? "Undefined";
//            UnitofMeasure = dailyJobTask.JobPhaseCost?.UM;
//            if (dailyJobTask.JobPhaseCost != null)
//            {
//                if (dailyJobTask.JobPhaseCost.UnitofMeasure != null)
//                {
//                    UnitofMeasure = dailyJobTask.JobPhaseCost.UnitofMeasure.Description;
//                }
//            }
//            CostTypeCategory = dailyJobTask.CostType.JBCostTypeCategory;
//            //t.CostType.
//            Value = dailyJobTask.Value;
//            Comments = dailyJobTask.Comments;
//            PhaseGroupId = dailyJobTask.PhaseGroupId;
//            BudgetCodeId = dailyJobTask.BudgetCodeId;
//            Radius = dailyJobTask.BudgetCode?.Radius;
            
//            //Hardness = t.BudgetCode?.Hardness ?? HardnessEnum.Medium;
//            //HardnessSelectList = new ProjectBudgetCodeRepository().GetHardnessList((byte)t.PhaseGroupId, t.PhaseId, (Hardness).ToString(AppCultureInfo.CInfo()));
//            //using var repo = new ProjectBudgetCodeRepository();
//            //RadiusSelectList = repo.GetRadiusList((byte)t.PhaseGroupId, t.PhaseId, t.BudgetCode?.Radius?.ToString(AppCultureInfo.CInfo()));

//            Status = (DB.DailyTicketStatusEnum)dailyJobTask.DailyTicket.Status;
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte DTCo { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Ticket Id")]
//        public int TicketId { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Line Num")]
//        public int LineNum { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Seq Id")]
//        public int SeqId { get; set; }

//        [HiddenInput]
//        [Display(Name = "Sort Order")]
//        public int? SortOrder { get; set; }

//        [HiddenInput]
//        [Display(Name = "Work Date")]
//        public DateTime? WorkDate { get; set; }

//        [HiddenInput]
//        [Display(Name = "Phase Group")]
//        public byte? PhaseGroupId { get; set; }

//        [Required]
//        [Display(Name = "Phase")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", Placeholder = "Select Task", ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
//        public string PhaseId { get; set; }

//        [Display(Name = "Phase")]
//        [UIHint("TextBox")]
//        [ReadOnly(true)]
//        public string PhaseDescription { get; set; }

//        [Required]
//        [UIHint("TextBox")]
//        [Display(Name = "Cost Type")]
//        public byte? CostTypeId { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Description")]
//        [UIHint("TextBox")]
//        public string CostTypeDescription { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Cost Type Category")]
//        [UIHint("TextBox")]
//        public string CostTypeCategory { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "UM")]
//        [UIHint("TextBox")]
//        public string UnitofMeasure { get; set; }


//        [Required]
//        [UIHint("IntegerBox")]
//        [Display(Name = "Value")]
//        [Field(LabelSize = 2, TextSize = 10)]
//        public decimal? Value { get; set; }

//        [UIHint("TextAreaBox")]
//        [Display(Name = "Comments")]
//        [Field(LabelSize = 2, TextSize = 10)]
//        [TableField(InternalTableRow = 2)]
//        public string Comments { get; set; }

//        [Display(Name = "Radius")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "PhaseGroupId,PhaseId")]
//        public decimal? Radius { get; set; }

//        [Display(Name = "Density")]
//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", Placeholder = "Density?")]
//        public HardnessEnum Hardness { get; set; }

//        [HiddenInput]
//        public string BudgetCodeId { get; set; }

//        //[HiddenInput]
//        //public List<SelectListItem> RadiusSelectList { get; }

//        //[HiddenInput]
//        //public List<SelectListItem> HardnessSelectList { get; }

//        public DB.DailyTicketStatusEnum Status { get; set; }
//    }

//    public class AddJobTaskViewModel
//    {
//        public AddJobTaskViewModel()
//        {
            
//        }

//        public AddJobTaskViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            DTCo = ticket.DTCo;
//            TicketId = ticket.TicketId;
//            JobId = ticket.DailyJobTicket.JobId;

//            PhaseGroupId = ticket.HQCompanyParm.PhaseGroupId;
//            //PhaseId = ticket.DailyJobTicket?.Job?.Phases?.FirstOrDefault()?.PhaseId;
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte DTCo { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Ticket Id")]
//        public int TicketId { get; set; }

//        [HiddenInput]
//        [Required]
//        [Display(Name = "Job Id")]
//        public string JobId { get; set; }

//        public byte? PhaseGroupId { get; set; }

//        //[Required]
//        [Display(Name = "Phase")]
//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
//        public string PhaseId { get; set; }

//    }
//}
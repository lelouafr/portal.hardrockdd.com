//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.DT.Forms.Job
//{
//    public class DailyJobPhaseListViewModel
//    {
//        public DailyJobPhaseListViewModel()
//        {
//            Tasks = new List<DTJobPhaseViewModel>();
//        }

//        public DailyJobPhaseListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            DTCo = ticket.DTCo;
//            TicketId = ticket.TicketId;
//            Tasks = ticket.DailyJobTicket.DTJobPhases.Select(s => new DTJobPhaseViewModel(s)).ToList();

//            AddTask = new DTJobPhaseAddViewModel(ticket);
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

//        public List<DailyJobPhaseViewModel> Tasks { get; }

//        public DTJobPhaseAddViewModel AddTask { get; set; }
//    }
//    public class DailyJobPhaseViewModel
//    {
//        public DailyJobPhaseViewModel()
//        {

//        }

//        public DailyJobPhaseViewModel(DB.Infrastructure.ViewPointDB.Data.DTJobPhase dtJobPhase)
//        {
//            DTCo = dtJobPhase.DTCo;
//            TicketId = dtJobPhase.TicketId;
//            TaskId = dtJobPhase.TaskId;
//            WorkDate = dtJobPhase.WorkDate;
//            PhaseId = dtJobPhase.PhaseId;
//            PhaseDescription = dtJobPhase.JobPhase?.Description;
//            Comments = dtJobPhase.Comments;
//            PhaseGroupId = dtJobPhase.PhaseGroupId;
//            PassSize = dtJobPhase.PassSize;
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
//        public int TaskId { get; set; }

//        [HiddenInput]
//        [Display(Name = "Work Date")]
//        public DateTime? WorkDate { get; set; }

//        [HiddenInput]
//        [Display(Name = "Phase Group")]
//        public byte? PhaseGroupId { get; set; }

//        [Required]
//        [Display(Name = "Phase")]
//        [Field(ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
//        public string PhaseId { get; set; }

//        [Display(Name = "Phase")]
//        [UIHint("TextBox")]
//        public string PhaseDescription { get; set; }

//        [UIHint("TextAreaBox")]
//        [Display(Name = "Comments")]
//        [Field(LabelSize = 2, TextSize = 10)]
//        [TableField(InternalTableRow = 2)]
//        public string Comments { get; set; }

//        [Display(Name = "Radius")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "PhaseGroupId,PhaseId")]
//        public decimal? PassSize { get; set; }
//    }

//    public class DTJobPhaseAddViewModel
//    {        
//        public DTJobPhaseAddViewModel()
//        {

        
//        }
//        public DTJobPhaseAddViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            DTCo = ticket.DTCo;
//            TicketId = ticket.TicketId;
//            JobId = ticket.DailyJobTicket.JobId;
//            PhaseGroupId = ticket.HQCompanyParm.PhaseGroupId;
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
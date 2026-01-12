using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Job
{

    public class TicketViewModel
    {
        public TicketViewModel()
        {

        }

        public TicketViewModel(DailyJobTicket ticket)
        {
            if (ticket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.DailyTicket.FormId;
            WorkDate = (DateTime)ticket.WorkDate;
            JCCo = ticket.JCCo ?? ticket.DailyTicket.HQCompanyParm.JCCo;
            JobId = ticket.JobId;
            PRCo = ticket.PRCo ?? ticket.DailyTicket.HQCompanyParm.PRCo;
            CrewId = ticket.CrewId;
            EMCo = ticket.EMCo ?? ticket.DailyTicket.HQCompanyParm.EMCo;
            RigId = ticket.RigId;
            GroundCondition = ticket.GroundCondition;
            WeatherCondition = ticket.WeatherCondition;
            RainOut = ticket.RainOut ?? false;
            MudMotor = ticket.MudMotor ?? false;
            Comments = ticket.Comments;
        }

        public TicketViewModel(DailyTicket ticket)
		{
			var jobTicket = ticket?.DailyJobTicket;
			if (ticket == null || jobTicket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            WorkDate = (DateTime)ticket.WorkDate;

            JCCo = jobTicket.JCCo ?? ticket.HQCompanyParm.JCCo;
            JobId = jobTicket.JobId;

            PRCo = jobTicket.PRCo ?? ticket.HQCompanyParm.PRCo;
            CrewId = jobTicket.CrewId;

            EMCo = jobTicket.EMCo ?? ticket.HQCompanyParm.EMCo;
            RigId = jobTicket.RigId;

            GroundCondition = jobTicket.GroundCondition;
            WeatherCondition = jobTicket.WeatherCondition;

            RainOut = jobTicket.RainOut ?? false;
            MudMotor = jobTicket.MudMotor ?? false;

            Comments = jobTicket.Comments;

            Status = (DB.DailyTicketStatusEnum)ticket.Status;

        }

        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [HiddenInput]
        [Display(Name = "Form Id")]
        public int FormId { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        [Range(typeof(DateTime), "1/1/2000", "1/1/2099", ErrorMessage = "Date is out of Range")]
        public DateTime WorkDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [Display(Name = "Work Date")]
        public string WorkDateDescription { get { return WorkDate.ToLongDateString(); } }


        [Display(Name = "PR Co")]
        public byte? PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 2, Placeholder = "Select Crew", ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        //[ReadOnly(true)]
        //[UIHint("TextBox")]
        //[Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 2, Placeholder = "Select Crew", SelectListViewBag = "CrewList")]
        //[Display(Name = "Crew")]
        //public string CrewDescription { get; set; }


        [Display(Name = "JC Co")]
        public byte? JCCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 3, SearchUrl = "/JCCombo/Search", SearchForeignKeys = "JCCo", ComboUrl = "/JCCombo/JobTicketCombo", ComboForeignKeys = "DTCo,TicketId", InfoUrl = "/Project/Job/PopupForm", InfoForeignKeys = "JCCo,JobId")]//, InfoUrl = "/JobForm/PopUpForm", InfoForeignKeys = "Co,JobId"
        [Display(Name = "Job")]
        public string JobId { get; set; }

        //[ReadOnly(true)]
        //[UIHint("TextBox")]
        //[Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 3, Placeholder = "Select Job", SelectListViewBag = "JobList")]
        //[Display(Name = "Job")]
        //public string JobDescription { get; set; }


        [Display(Name = "EM Co")]
        public byte? EMCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 4, ComboUrl = "/EMCombo/CrewRigCombo", ComboForeignKeys = "PRCo,CrewId")]
        [Display(Name = "Rig")]
        public string RigId { get; set; } 

        //[ReadOnly(true)]
        //[UIHint("TextBox")]
        //[Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 4, Placeholder = "Select Crew", SelectListViewBag = "RigList")]
        //[Display(Name = "Rig")]
        //public string RigDescription { get; set; }

        [Required]
        [UIHint("DropdownBox")]//DailyCombo
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 5, ComboUrl = "/DailyCombo/GroundConditionCombo", ComboForeignKeys = "")]
        [Display(Name = "Ground Conditions")]
        public string GroundCondition { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 6, ComboUrl = "/DailyCombo/WeatherConditionCombo", ComboForeignKeys = "")]
        [Display(Name = "Weather Conditions")]
        public string WeatherCondition { get; set; }


        [Required]
        [UIHint("SwitchBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 7)]
        [Display(Name = "Rain Out Day?")]
        public bool RainOut { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 8)]
        [Display(Name = "Mud Motor Added?")]
        public bool MudMotor { get; set; }


        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 9)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        public DB.DailyTicketStatusEnum Status { get; set; }

        internal TicketViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyJobTickets.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                if (WorkDate.Year > 2000)
                    updObj.WorkDate = WorkDate;
                updObj.JobId = JobId;
                updObj.RigId = RigId;
                updObj.CrewId = CrewId;

                updObj.GroundCondition = GroundCondition;
                updObj.WeatherCondition = WeatherCondition;
                updObj.RainOut = RainOut;
                updObj.MudMotor = MudMotor;
                updObj.Comments = Comments;

                try
                {
                    db.SaveChanges(modelState);
                    return new TicketViewModel(updObj);
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
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Shop
{

    public class TicketViewModel
    {
        public TicketViewModel()
        {

        }

        public TicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyShopTicket ticket)
        {
            if (ticket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.DailyTicket.FormId;
            WorkDate = (DateTime)ticket.DailyTicket.WorkDate;

            JCCo = ticket.JCCo ?? ticket.DailyTicket.HQCompanyParm.JCCo;
            JobId = ticket.JobId;
            
            PRCo = ticket.PRCo ?? ticket.DailyTicket.HQCompanyParm.PRCo;
            CrewId = ticket.CrewId;

            Comments = ticket.Comments;
        }

        public TicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
		{
			var shopTicket = ticket?.DailyShopTicket;
			if (ticket == null || shopTicket == null)
				return;

			DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            WorkDate = (DateTime)ticket.WorkDate;

            JCCo = shopTicket.JCCo ?? ticket.HQCompanyParm.JCCo;
            JobId = shopTicket.JobId;

            PRCo = shopTicket.PRCo ?? ticket.HQCompanyParm.PRCo;
            CrewId = shopTicket.CrewId;

            Comments = shopTicket.Comments;
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
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [Display(Name = "Work Date")]
        public string WorkDateDescription { get { return WorkDate.ToLongDateString(); } }

        [Display(Name = "Co")]
        public byte? PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2, ComboUrl = "/PRCombo/ShopTypeCrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Display(Name = "Co")]
        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 3, ComboUrl = "/JCCombo/ShopCrewJobCombo", ComboForeignKeys = "JCCo,CrewId")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 4)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }
        
        internal TicketViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyShopTickets.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId);

            if (updObj != null)
            {
                WorkDate = WorkDate.Year < 1900 ? (DateTime)updObj.WorkDate : WorkDate;
                /****Write the changes to object****/
                updObj.JobId = JobId;
                updObj.CrewId = CrewId;
                updObj.Comments = Comments;
                updObj.WorkDate = WorkDate;

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
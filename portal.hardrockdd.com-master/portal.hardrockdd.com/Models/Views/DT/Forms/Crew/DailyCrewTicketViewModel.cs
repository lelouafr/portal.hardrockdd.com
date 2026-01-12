using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{

    public class DailyCrewTicketViewModel : AuditBaseViewModel
    {
        public DailyCrewTicketViewModel()
        {

        }

        public DailyCrewTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyShopTicket ticket):base(ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
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

        public DailyCrewTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket == null ? throw new System.ArgumentNullException(nameof(ticket)) : ticket.DailyShopTicket)
        {
            if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            WorkDate = (DateTime)ticket.WorkDate;

            JCCo = ticket.DailyShopTicket.JCCo ?? ticket.HQCompanyParm.JCCo;
            JobId = ticket.DailyShopTicket.JobId;

            PRCo = ticket.DailyShopTicket.PRCo ?? ticket.HQCompanyParm.PRCo;
            CrewId = ticket.DailyShopTicket.CrewId;

            Comments = ticket.DailyShopTicket.Comments;
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
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2, Placeholder = "Select Crew", ComboUrl = "/PRCombo/CrewTypeCrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Display(Name = "Co")]
        public byte? JCCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 3, Placeholder = "Select Job", ComboUrl = "/JCCombo/CostJobCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }


        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 4)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal DailyCrewTicketViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
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
                    return new DailyCrewTicketViewModel(updObj);
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
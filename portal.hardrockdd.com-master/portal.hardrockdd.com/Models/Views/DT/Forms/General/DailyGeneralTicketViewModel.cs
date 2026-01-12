using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{

    public class DailyGeneralTicketViewModel : AuditBaseViewModel
    {
        public DailyGeneralTicketViewModel()
        {

        }

        public DailyGeneralTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyGeneralTicket ticket):base(ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.DailyTicket.FormId;
            WorkDate = (DateTime)ticket.DailyTicket.WorkDate;
            Comments = ticket.Comments;
        }

        public DailyGeneralTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket == null ? throw new ArgumentNullException(nameof(ticket)) : ticket.GeneralTicket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            WorkDate = (DateTime)ticket.WorkDate;
            Comments = ticket.GeneralTicket.Comments;
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
        
        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 4)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal DailyGeneralTicketViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeeTickets.FirstOrDefault(f => f.DTCo == this.DTCo && f.TicketId == this.TicketId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Comments = Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new DailyGeneralTicketViewModel(updObj.DailyTicket);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
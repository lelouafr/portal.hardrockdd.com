using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{

    public class DailyHolidayTicketViewModel : AuditBaseViewModel
    {

        public DailyHolidayTicketViewModel()
        {

        }

        public DailyHolidayTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket):base(ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            Comments = ticket.GeneralTicket?.Comments;
            WorkDate = (DateTime)ticket.WorkDate;


        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [HiddenInput]
        [Display(Name = "Form Id")]
        public int FormId { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 2, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Week")]
        public DateTime WorkDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [Display(Name = "Work Date")]
        public string DateDescription { get { return string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.ToLongDateString()); } }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal DailyHolidayTicketViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyGeneralTickets.FirstOrDefault(f => f.DTCo == this.DTCo && f.TicketId == this.TicketId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Comments = Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new DailyHolidayTicketViewModel(updObj.DailyTicket);
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
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Employee
{

    public class TicketViewModel
    {

        public TicketViewModel()
        {

        }

        public TicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            Comments = ticket.DailyEmployeeTicket.Comments;
            EmployeeName = ticket.DailyEmployeeTicket.PREmployee.FullName;

            var cal = ticket.db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId);
            StartDate = cal.Min(m => m.Date);
            EndDate = cal.Max(m => m.Date);

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

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Week")]
		[Field(LabelSize = 2, TextSize = 2)]
		public DateTime StartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "End Date")]
		[Field(LabelSize = 0, TextSize = 2)]
		public DateTime EndDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Work Date")]
        public string DateDescription { get { return string.Format(AppCultureInfo.CInfo(), "{0}, - {1}", StartDate.ToLongDateString(), EndDate.ToLongDateString()); } }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal TicketViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeeTickets.FirstOrDefault(f => f.DTCo == this.DTCo && f.TicketId == this.TicketId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Comments = Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new TicketViewModel(updObj.DailyTicket);
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
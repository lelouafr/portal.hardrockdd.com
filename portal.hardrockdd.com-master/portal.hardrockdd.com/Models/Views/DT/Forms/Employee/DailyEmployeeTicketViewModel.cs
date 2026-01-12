using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{

    public class DailyEmployeeTicketViewModel : AuditBaseViewModel
    {

        public DailyEmployeeTicketViewModel()
        {

        }

        public DailyEmployeeTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket):base(ticket == null ? throw new ArgumentNullException(nameof(ticket)) : ticket.DailyEmployeeTicket)
        {
            if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            Comments = ticket.DailyEmployeeTicket.Comments;
            EmployeeName = ticket.DailyEmployeeTicket.PREmployee.FullName;
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var cal = db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId).ToList();
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

        [HiddenInput]
        [Display(Name = "Form Id")]
        public int FormId { get; set; }

        [HiddenInput]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 2, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Week")]
        public DateTime StartDate { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 2, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime EndDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [Display(Name = "Work Date")]
        public string DateDescription { get { return string.Format(AppCultureInfo.CInfo(), "{0}, - {1}", StartDate.ToLongDateString(), EndDate.ToLongDateString()); } }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal DailyEmployeeTicketViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeeTickets.FirstOrDefault(f => f.DTCo == this.DTCo && f.TicketId == this.TicketId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Comments = Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new DailyEmployeeTicketViewModel(updObj.DailyTicket);
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
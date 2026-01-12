using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketEmailViewModel
    {
        public DailyTicketEmailViewModel()
        {

        }

        public DailyTicketEmailViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            Comments = ticket.WorkFlow.CurrentSequence().Comments;
        }

        public byte DTCo { get; set; }

        public int TicketId { get; set; }

        public string Comments { get; set; }
    }

    public class DailyTicketRejectViewModel
    {

        public DailyTicketRejectViewModel()
        {

        }
        public DailyTicketRejectViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            WorkDate = ticket.WorkDate;
            FormName = ticket.Form.Description;

            switch (ticket.FormId)
            {
                case 1:
                    Description = ticket.DailyJobTicket.JobId + "(" + ticket.DailyJobTicket?.Job?.Description + ")";
                    break;
                case 5:
                    var cal = db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId);
                    Description = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", cal.Min(m => m.Date).ToShortDateString(), cal.Max(m => m.Date).ToShortDateString());
                    break;
                case 6:
                    Description = ticket.DailyShopTicket.JobId + "(" + ticket.DailyShopTicket?.Job?.Description + ")";
                    break;
                case 8:
                    Description = ticket.DailyShopTicket.JobId + "(" + ticket.DailyShopTicket.Job.Description + ")";
                    break;
                default:
                    break;
            }
            CreatedUser = new Web.WebUserViewModel(ticket.CreatedUser); //Repository.VP.WP.WebUserRepository.EntityToModel(ticket.CreatedUser, "");

        }
        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "Ticket #")]
        public int TicketId { get; set; }

        [HiddenInput]
        public string Url { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1)]
        public DateTime? WorkDate { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "")]
        [Field(LabelSize = 0, TextSize = 6, FormGroup = "Ticket", FormGroupRow = 1)]
        public string WorkDateString { get { return WorkDate?.ToLongDateString(); } }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Form Name")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 2)]
        public string FormName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 3)]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "CreatedUser")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 4)]
        public portal.Models.Views.Web.WebUserViewModel CreatedUser { get; set; }

        [UIHint("TextAreaBox")]
        [Required]
        [Display(Name = "Reject Comment")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 5)]
        public string Comments { get; set; }


    }
}
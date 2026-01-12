using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.TimeOff
{

    public class TicketViewModel 
    {

        public TicketViewModel()
        {

        }

        public TicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            Comments = ticket.GeneralTicket.Comments;
            WorkDate = ticket.WorkDate;
            EmployeeName = Repository.VP.PR.EmployeeRepository.FullName(ticket.GeneralTicket.Employee);
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
        [Display(Name = "Work Date")]
        public DateTime? WorkDate { get; set; }

        [HiddenInput]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2)]
        [Display(Name = "Comment")]
        public string Comments { get; set; }

        internal TicketViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyGeneralTickets.FirstOrDefault(f => f.DTCo == this.DTCo && f.TicketId == this.TicketId);

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
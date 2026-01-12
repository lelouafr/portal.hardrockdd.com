using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketApprovalListViewModel
    {
        public DailyTicketApprovalListViewModel()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var isAdmin = HttpContext.Current.User.IsInRole("Admin");
            var tickets = db.DailyTickets.Where(f => f.tStatusId == (short)DB.DailyTicketStatusEnum.Submitted &&
                                                     (
                                                     f.CreatedUser.Employee.FirstOrDefault().Supervisor.WebId == userId ||
                                                     f.CreatedUser.Employee.FirstOrDefault().Supervisor.Supervisor.WebId == userId ||
                                                     isAdmin
                                                     ));

            Tickets = tickets.AsEnumerable().Select(s => new DailyTicketApprovalViewModel(s)).ToList();

        }

        public List<DailyTicketApprovalViewModel> Tickets { get; }
    }
    public class DailyTicketApprovalViewModel
    {

        public DailyTicketApprovalViewModel()
        {

        }

        public DailyTicketApprovalViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            WorkDate = ticket.WorkDate;
            FormName = ticket.Form.Description;
            Status = (DB.DailyTicketStatusEnum)ticket.Status;

            var createEmp = ticket.CreatedUser.Employee.FirstOrDefault();
            if (createEmp != null)
            {
                var super = createEmp.Supervisor?.PREmployee;
                if (super != null)
                {
                    SupervisorUser = new Web.WebUserViewModel(super);
                }
            }
            switch (ticket.FormId)
            {
                case 1:
                    Description = ticket.DailyJobTicket.JobId + "(" + ticket.DailyJobTicket.Job.Description + ")";
                    Comments = ticket.DailyJobTicket.Comments;
                    break;
                case 5:
                    var cal = db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId);
                    Description = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", cal.Min(m => m.Date).ToShortDateString(), cal.Max(m => m.Date).ToShortDateString());
                    Comments = ticket.DailyEmployeeTicket.Comments;
                    break;
                case 6:
                    Description = ticket.DailyShopTicket?.JobId + "(" + ticket.DailyShopTicket?.Job?.Description + ")";
                    Comments = ticket.DailyShopTicket?.Comments;
                    break;
                case 8:
                    Description = ticket.DailyShopTicket.JobId + "(" + ticket.DailyShopTicket.Job?.Description + ")";
                    Comments = ticket.DailyShopTicket.Comments;
                    break;
                default:
                    break;
            }
            CreatedUser = new Web.WebUserViewModel(ticket.CreatedUser);// Repository.VP.WP.WebUserRepository.EntityToModel(ticket.CreatedUser, "");
        }

        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "Ticket #")]
        public int TicketId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "")]
        public string WorkDateString { get { return WorkDate?.ToLongDateString(); } }

        [ReadOnly(true)]
        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WorkDate { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "FormName")]
        public string FormName { get; set; }

        [ReadOnly(true)]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.DailyTicketStatusEnum Status { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Supervisor")]
        public Web.WebUserViewModel SupervisorUser { get; set; }
    }
}
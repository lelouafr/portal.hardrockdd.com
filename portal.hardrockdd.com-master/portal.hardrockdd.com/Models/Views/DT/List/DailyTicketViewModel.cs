using portal.Models.Views.DailyTicket.Form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketListViewModel
    {
        public DailyTicketListViewModel()
        {

        }

        public DailyTicketListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DB.Infrastructure.ViewPointDB.Data.Calendar calendar)
        {

            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            if (calendar == null)
            {
                throw new System.ArgumentNullException(nameof(calendar));
            }
            var ticketList = employee.DailyEmployeeEntries.Where(f => f.DailyTicket.Calendar.Week == calendar.Week).GroupBy(g => g.DailyTicket).Select(s => s.Key).ToList();
            ticketList.AddRange(employee.DailyJobEmployees.Where(f => f.DailyTicket.Calendar.Week == calendar.Week).GroupBy(g => g.DailyTicket).Select(s => s.Key).ToList());

            List = ticketList.Select(s => new DailyTicket.DailyTicketViewModel(s)).ToList();
        }


        public List<DailyTicketViewModel> List { get; }
    }

    public class DailyTicketViewModel
    {
        public DailyTicketViewModel()
        {

        }

        public DailyTicketViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            FormId = (int)ticket.FormId;
            CreatedUser = new Web.WebUserViewModel(ticket.CreatedUser);
            CreatedOn = (DateTime)ticket.CreatedOn;
            WorkDate = (DateTime)ticket.WorkDate;
            Status = (DB.DailyTicketStatusEnum)ticket.Status;
            switch (ticket.FormId)
            {
                case 1:
                    //DynaimicTicket = new JobTicketFormViewModel(ticket);
                    break;
                case 5:
                    //DynaimicTicket = new EmployeeTicketFormViewModel(ticket);
                    break;
                case 6:
                    //DynaimicTicket = new ShopTicketFormViewModel(ticket);
                    break;
                case 8:
                    //DynaimicTicket = new CrewTicketFormViewModel(ticket);
                    break;
                default:
                    break;
            }


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

        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }

        [ReadOnly(true)]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.DailyTicketStatusEnum Status { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

        //public byte? PRCo { get; set; }

        //[Required]
        //[UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2, ComboUrl = "/PRCombo/ShopTypeCrewCombo", ComboForeignKeys = "PRCo")]
        //[Display(Name = "Crew")]
        //public string CrewId { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 7)]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime CreatedOn { get; set; }

        public dynamic DynaimicTicket { get; set; }

    }
}
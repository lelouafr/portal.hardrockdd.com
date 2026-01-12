using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace portal.Models.Views.DailyTicket
{
    public class DailyCrewSubmissionListViewModel
    {
        public DailyCrewSubmissionListViewModel()
        {

        }

        public DailyCrewSubmissionListViewModel(int weekId)
        {
            using var db = new VPContext();

            var dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            var tickets = db.vDailyTickets.Where(f => f.WorkDate >= dates.Min() && f.WorkDate <= dates.Max() && f.Status <= 4).ToList();
			var ticketCrewList = tickets.GroupBy(g => g.CrewId).Select(s => s.Key).ToList();

			var crews = db.Crews.OrderBy(o => o.Description).Where(f => 
                            (f.CrewStatus == "ACTIVE" &&  f.udCrewType != "Admin" ) ||
                            ticketCrewList.Contains(f.CrewId)
			            ).ToList();

            var crewList = crews.Select(s => new DailyCrewSubmissionViewModel(s, weekId, tickets.Where(f => f.CrewId == s.CrewId).ToList(), dates)).ToList();

            Co = 1;
            WeekId = weekId;
            List = crewList;
            Dates = dates;
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int Co { get; set; }


        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        public List<DailyCrewSubmissionViewModel> List { get; }

        public List<DateTime> Dates { get; }
    }


    public class DailyCrewSubmissionViewModel
    {
        public DailyCrewSubmissionViewModel()
        {

        }

        public DailyCrewSubmissionViewModel(Crew crew, int weekId, List<vDailyTicket> tickets, List<DateTime> dates)
        {
            if (tickets == null)
                throw new ArgumentNullException(nameof(tickets));

            if (crew == null)
                throw new ArgumentNullException(nameof(crew));
            Crew = crew.Description;

            PRCo = crew.PRCo;
            CrewId = crew.CrewId;
            WeekId = weekId;
            if (tickets.Count != 0)
            {
                CreatedBy = tickets.FirstOrDefault()?.CreatedBy;
                using var db = new VPContext();
                var CreatedUser = db.WebUsers.FirstOrDefault(f => f.Id == CreatedBy);
                CreatedBy = CreatedUser.FirstName + " " + CreatedUser.LastName;
            }
            DayList = dates.Select(s => new DailyCrewSubmissionDayViewModel(tickets.Where(f => f.WorkDate == s.Date).ToList(), s)).ToList();
            Dates = dates;
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int PRCo { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        [Key]
        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string Crew { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        public List<DateTime> Dates { get; }

        public List<DailyCrewSubmissionDayViewModel> DayList { get; }

    }

    public class DailyCrewSubmissionDayViewModel
    {
        public DailyCrewSubmissionDayViewModel()
        {

        }

        public DailyCrewSubmissionDayViewModel(List<vDailyTicket> tickets, DateTime date)
        {
            WorkDate = date;
            List = tickets.Select(s => new DailyCrewSubmissionDetailViewModel(s)).ToList();
        }

        [ReadOnly(true)]
        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "", FormGroupRow = 0, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }

        public List<DailyCrewSubmissionDetailViewModel> List { get; }
    }

    public class DailyCrewSubmissionDetailViewModel
    {
        public DailyCrewSubmissionDetailViewModel(vDailyTicket ticket)
        {
            if (ticket == null)
            {

            }
            else
            {
                DTCo = ticket.DTCo;
                TicketId = ticket.TicketId;
                WorkDate = (DateTime)ticket.WorkDate;
                Status = (DB.DailyTicketStatusEnum)ticket.Status;
                Description = ticket.Description;
                JCCo = ticket.JCCo ?? 0;
                JobId = ticket.JobId;
                
                Comments = ticket.Comments;
                FormName = ticket.FormDescription;
                CreatedBy = ticket.CreatedBy;

                HasAttachment = ticket.HasAttachments == 0 ? false : true;
            }
        }

        [Key]
        [Required]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Company #")]
        public int DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Ticket #")]
        public int TicketId { get; set; }

        public string WorkDateString { get { return WorkDate.ToShortDateString(); } }

        [ReadOnly(true)]
        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "", FormGroupRow = 0, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }

        [ReadOnly(true)]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.DailyTicketStatusEnum Status { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public string StatusString { get { return Status.ToString(); } }


        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Company #")]
        public int JCCo { get; set; }

        [ReadOnly(true)]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/CrewCombo", ComboForeignKeys = "JCCo,CrewId")]
        [Display(Name = "Job")]
        public string JobId { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string Job { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Form Name")]
        public string FormName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [Display(Name = "")]
        public bool HasAttachment { get; set; }
    }
}
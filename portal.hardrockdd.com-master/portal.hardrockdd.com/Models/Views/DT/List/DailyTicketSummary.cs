using Org.BouncyCastle.Asn1.Cms;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketListSummaryViewModel
    {
        

        public DailyTicketListSummaryViewModel(DB.TimeSelectionEnum timeSelection)
        {
            NewTicket = new DailyTicketCreateViewModel();
            Tickets = new List<DailyTicketSummaryViewModel>();
            TimeSelection = timeSelection;
        }

        public DailyTicketListSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.HQCompanyParm> companies, DB.TimeSelectionEnum timeSelection)
        {
            if (companies == null) throw new System.ArgumentNullException(nameof(companies));

            using var db = new VPContext();
            Tickets = new List<DailyTicketSummaryViewModel>();
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };

            var vtickets = db.vDailyTickets.Where(f => f.WorkDate >= asofDate).OrderByDescending(o => o.WorkDate).ToList();
            var list = vtickets.Select(s => new DailyTicketSummaryViewModel(s)).ToList();
            Tickets.AddRange(list);
            NewTicket = new DailyTicketCreateViewModel();
            TimeSelection = timeSelection;
        }


        public DailyTicketListSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, VPContext db, DB.TimeSelectionEnum timeSelection)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Tickets = new List<DailyTicketSummaryViewModel>();
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };

            var vtickets = (from fldTickets in db.DailyJobTickets.Where(f => f.JobId == job.JobId)                            
                            join vticket in db.vDailyTickets on
                                new { p1 = fldTickets.DTCo, p2 = fldTickets.TicketId }
                                equals
                                new { p1 = vticket.DTCo, p2 = vticket.TicketId }
                            where vticket.WorkDate >= asofDate
                            select vticket
                            ).ToList();

            var list = vtickets.Select(s => new DailyTicketSummaryViewModel(s)).ToList();
            Tickets.AddRange(list);

            NewTicket = new DailyTicketCreateViewModel();
            TimeSelection = timeSelection;
            JobId = job.JobId;
            JCCo = job.JCCo;
        }

        public DailyTicketListSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.HQCompanyParm> companies, List<DB.DailyTicketStatusEnum> Status, DB.TimeSelectionEnum timeSelection)
        {
            if (companies == null) throw new System.ArgumentNullException(nameof(companies));
            if (Status == null) throw new System.ArgumentNullException(nameof(Status));

            using var db = new VPContext();
            Tickets = new List<DailyTicketSummaryViewModel>();
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };

            var DTCompanies = companies.GroupBy(g => g.DTCo).Select(s => s.Key).ToList();


            foreach (var dtCo in DTCompanies)
            {
                //var tickets = db.DailyTickets.Where(f => f.Co == company.HQCo).ToList();
                var statusList = Status.Select(s => (int)s).ToList();

                var vtickets = db.vDailyTickets.Where(f => f.DTCo == dtCo && f.WorkDate >= asofDate && statusList.Contains((int)f.Status) ).OrderByDescending(o => o.WorkDate).ToList();
                var list = vtickets.Select(s => new DailyTicketSummaryViewModel(s)).ToList();
                Tickets.AddRange(list);
            }
            NewTicket = new DailyTicketCreateViewModel();
            TimeSelection = timeSelection;
            
        }

        public DailyTicketListSummaryViewModel(WebUser user, DB.TimeSelectionEnum timeSelection)
        {
            using var db = new VPContext();
            Tickets = new List<DailyTicketSummaryViewModel>();

            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };
            var vtickets = db.vDailyTickets.Where(f => f.CreatedBy == user.Id && f.WorkDate >= asofDate).OrderByDescending(o => o.WorkDate).ToList();
            var list = vtickets.Select(s => new DailyTicketSummaryViewModel(s)).ToList();
            Tickets.AddRange(list);

            NewTicket = new DailyTicketCreateViewModel();
            TimeSelection = timeSelection;
        }

        public List<DailyTicketSummaryViewModel> Tickets { get; }

        public DailyTicketCreateViewModel NewTicket { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Time Range")]
        public DB.TimeSelectionEnum TimeSelection { get; set; }

        public string JobId { get; set; }

        public byte? JCCo { get; set; }
    }

    public class DailyTicketSummaryViewModel
    {

        public DailyTicketSummaryViewModel()
        {

        }

        public DailyTicketSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.vDailyTicket vticket)// : base(ticket)
        {
            if (vticket == null) throw new System.ArgumentNullException(nameof(vticket));

            //var user = users.FirstOrDefault(f => f.Id == vticket.CreatedBy);
            DTCo = vticket.DTCo;
            TicketId = vticket.TicketId;
            WorkDate = (DateTime)vticket.WorkDate;
            Status = (DB.DailyTicketStatusEnum)vticket.Status;
            Description = vticket.Description;
            Comments = vticket.Comments;
            FormName = vticket.FormDescription;
            //CreatedUser = new WebUserViewModel(user);
            CreatedBy = vticket.CreatedBy;
            CreatedName = string.Format("{0} {1}", vticket.FirstName, vticket.LastName);
            HasAttachment = vticket.HasAttachments == 0 ? false : true;

            SearchString = vticket.SearchString;
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

        public string WorkDateString => WorkDate.ToString("MM/dd/yyyy");

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
        public bool AllowEdit
        {
            get
            {
                return Status switch
                {
                    DB.DailyTicketStatusEnum.Draft => true,
                    DB.DailyTicketStatusEnum.Rejected => true,
                    DB.DailyTicketStatusEnum.Submitted => false,
                    DB.DailyTicketStatusEnum.Approved => false,
                    DB.DailyTicketStatusEnum.Processed => false,
                    DB.DailyTicketStatusEnum.Canceled => true,
                    DB.DailyTicketStatusEnum.Deleted => true,
                    _ => false,
                };
            }
        }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Form Name")]
        public string FormName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User")]
        public string CreatedName { get; set; }


        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [Display(Name = "")]
        public bool HasAttachment { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Search String")]
        public string SearchString { get; set; }
    }
}

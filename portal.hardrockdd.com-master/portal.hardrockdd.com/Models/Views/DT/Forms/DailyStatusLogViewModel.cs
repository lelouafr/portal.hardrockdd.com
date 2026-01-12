using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyStatusLogListViewModel
    {
        public DailyStatusLogListViewModel()
        {

        }

        public DailyStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            StatusLogs = ticket.DailyTicketStatusLogs
                               .Where(f => !(f.DailyTicket.CreatedBy == f.CreatedBy && f.Status == (int)DB.DailyTicketStatusEnum.Reviewed))
                               .GroupBy(g => new { g.DTCo, g.TicketId, g.CreatedBy, g.CreatedUser, g.Status, g.CreatedOn.Date })
                               .ToList()
                               .Select(s => new DailyStatusLogViewModel{ 
                                               DTCo = s.Key.DTCo,
                                               TicketId = s.Key.TicketId,
                                               CreatedUser = new Web.WebUserViewModel(s.Key.CreatedUser),
                                               Status = (DB.DailyTicketStatusEnum)s.Key.Status,
                                               CreatedOn = s.Min(min => min.CreatedOn),
                                               Comments = s.Max(max => max.Comments)
                                                }
                               )
                               .OrderBy(o => o.CreatedOn)
                               .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        public List<DailyStatusLogViewModel> StatusLogs { get; }
    }

    public class DailyStatusLogViewModel
    {
        public DailyStatusLogViewModel()
        {

        }

        public DailyStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.DailyStatusLog t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            DTCo = t.DTCo;
            TicketId = t.TicketId;
            LineNum = t.LineNum;
            CreatedOn = t.CreatedOn;
            CreatedUser = new Web.WebUserViewModel(t.CreatedUser);
            Status = (DB.DailyTicketStatusEnum)t.Status;
            Comments = t.Comments;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Reviewed", FormGroupRow = 93, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Log Date")]
        public DateTime? CreatedOn { get; set; }

        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 7)]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.DailyTicketStatusEnum Status { get; set; }

    }
}
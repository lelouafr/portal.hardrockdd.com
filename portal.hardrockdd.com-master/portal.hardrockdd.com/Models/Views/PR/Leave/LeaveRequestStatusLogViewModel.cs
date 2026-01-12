using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class LeaveRequestStatusLogListViewModel
    {
        public LeaveRequestStatusLogListViewModel()
        {

        }

        public LeaveRequestStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            PRCo = request.PRCo;
            RequestId = request.RequestId;
            List = request.StatusLogs
                               .Where(f => !(f.Request.CreatedBy == f.CreatedBy && f.Status == (int)DB.LeaveRequestStatusEnum.Reviewed))
                               //.GroupBy(g => new { g.PRCo, g.RequestId, g.CreatedBy, g.CreatedUser, g.Status, g.CreatedOn.Value.Date })
                               //.ToList()
                               //.Select(s => new LeaveRequestStatusLogViewModel{ 
                               //                PRCo = s.Key.PRCo,
                               //                RequestId = s.Key.RequestId,
                               //                CreatedUser = new Web.WebUserViewModel(s.Key.CreatedUser),
                               //                Status = (DB.LeaveRequestStatusEnum)s.Key.Status,
                               //                CreatedOn = s.Min(min => min.CreatedOn),
                               //                Comments = s.Max(max => max.Comments)
                               //                 }
                               //)
                               .Select(s => new LeaveRequestStatusLogViewModel(s))
                               .OrderBy(o => o.CreatedOn)
                               .ThenBy(o => o.LineNum)
                               .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        public List<LeaveRequestStatusLogViewModel> List { get; }
    }

    public class LeaveRequestStatusLogViewModel
    {
        public LeaveRequestStatusLogViewModel()
        {

        }

        public LeaveRequestStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequestStatusLog t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            PRCo = t.PRCo;
            RequestId = t.RequestId;
            LineNum = t.LineNum;
            CreatedOn = t.CreatedOn;
            CreatedUser = new Web.WebUserViewModel(t.CreatedUser);
            Status = (DB.LeaveRequestStatusEnum)t.Status;
            Comments = t.Comments;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

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
        public DB.LeaveRequestStatusEnum Status { get; set; }

    }
}
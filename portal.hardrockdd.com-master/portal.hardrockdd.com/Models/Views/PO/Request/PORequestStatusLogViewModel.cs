using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{
    public class PORequestStatusLogListViewModel
    {
        public PORequestStatusLogListViewModel()
        {

        }

        public PORequestStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            POCo = request.POCo;
            RequestId = request.RequestId;
            List = request.StatusLogs
                               .Where(f => !(f.PORequest.CreatedBy == f.CreatedBy && f.Status == (int)DB.PORequestStatusEnum.Reviewed))
                               .Select(s => new PORequestStatusLogViewModel(s))
                               .OrderBy(o => o.CreatedOn)
                               .ThenBy(o => o.LineNum)
                               .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        public List<PORequestStatusLogViewModel> List { get; }
    }

    public class PORequestStatusLogViewModel
    {
        public PORequestStatusLogViewModel()
        {

        }

        public PORequestStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.PORequestStatusLog t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            POCo = t.POCo;
            RequestId = t.RequestId;
            LineNum = t.LineNum;
            CreatedOn = t.CreatedOn;
            CreatedUser = new Web.WebUserViewModel(t.CreatedUser);
            Status = (DB.PORequestStatusEnum)t.Status;
            Comments = t.Comments;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

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
        public DB.PORequestStatusEnum Status { get; set; }

    }
}
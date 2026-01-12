using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{

    public class StatusLogListViewModel
    {
        public StatusLogListViewModel()
        {

        }

        public StatusLogListViewModel(WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            List = workFlow.Sequences.Select(s => new StatusLogViewModel(s)).ToList();
        }

        public List<StatusLogViewModel> List { get; }
    }

    public class StatusLogViewModel : portal.Models.Views.WF.WorkFlowStatusViewModel
    {

        public StatusLogViewModel()
        {

        }

        public StatusLogViewModel(WorkFlowSequence sequence) : base(sequence)
        {
            if (sequence == null)
                return;

            Status = (DB.APDocumentStatusEnum)sequence.Status;
        }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.APDocumentStatusEnum Status { get; set; }
    }

    public class DocumentStatusLogListViewModel
    {
        public DocumentStatusLogListViewModel()
        {

        }

        public DocumentStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document)
        {
            if (document == null)
            {
                throw new System.ArgumentNullException(nameof(document));
            }
            APCo = document.APCo;
            DocId = document.DocId;
            List = document.StatusLogs
                               .Where(f => !(f.Document.CreatedBy == f.CreatedBy && f.Status == (int)DB.APDocumentStatusEnum.Reviewed))
                               .Select(s => new DocumentStatusLogViewModel(s))
                               .OrderBy(o => o.CreatedOn)
                               .ThenBy(o => o.LineNum)
                               .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int DocId { get; set; }

        public List<DocumentStatusLogViewModel> List { get; }
    }

    public class DocumentStatusLogViewModel
    {
        public DocumentStatusLogViewModel()
        {

        }

        public DocumentStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.APDocumentStatusLog t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            APCo = t.APCo;
            DocId = t.DocId;
            LineNum = t.LineNum;
            CreatedOn = t.CreatedOn;
            CreatedUser = t.CreatedUser.FullName();
            Status = (DB.APDocumentStatusEnum)t.Status;
            Comments = t.Comments;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int DocId { get; set; }

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

        [UIHint("TextBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 7)]
        public string CreatedUser { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.APDocumentStatusEnum Status { get; set; }

    }
}
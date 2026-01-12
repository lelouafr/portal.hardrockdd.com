using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.WF
{
    public class WorkFlowStatusListViewModel
    {
        public WorkFlowStatusListViewModel()
        {

        }

        public WorkFlowStatusListViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlow workFlow)
        {
            if (workFlow == null)
                return;
            
            WFCo = workFlow.WFCo;
            WorkFlowId = workFlow.WorkFlowId;
            List = workFlow.Sequences.Select(s => new WorkFlowStatusViewModel(s)).ToList();

        }

        [Key]
        public byte WFCo { get; set; }
        [Key]
        public int WorkFlowId { get; set; }
        public List<WorkFlowStatusViewModel> List { get; set; }
    }

    public class WorkFlowStatusViewModel
    {
        public WorkFlowStatusViewModel()
        {

        }

        public WorkFlowStatusViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlowSequence sequence)
        {
            if (sequence == null)
                return;

            WFCo = sequence.WFCo;
            WorkFlowId = sequence.WorkFlowId;
            SeqId = sequence.SeqId;
            StatusId = sequence.Status;
            StatusDate = sequence.StatusDate;
            Active = sequence.Active;
            CompletedOn = sequence.CompletedOn;
            CompletedBy = sequence.CompletedBy;
            CompletedUser = sequence.CompletedUser?.FullName();

            CreatedBy = sequence.CreatedBy;
            CreatedUser = sequence.CreatedUser?.FullName();

            Comments = sequence.Comments;
        }

        [Key]
        [UIHint("LongBox")]
        public byte WFCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int WorkFlowId { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int SeqId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Status Id")]
        public int StatusId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public System.DateTime StatusDate { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Active")]
        public bool Active { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Completed Date")]
        public DateTime? CompletedOn { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "User")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo")]
        public string CompletedBy { get; set; }
        
        [UIHint("TextBox")]
        [Display(Name = "User")]
        public string CompletedUser { get; set; }


        [UIHint("DropdownBox")]
        [Display(Name = "User")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo")]
        public string CreatedBy { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "User")]
        public string CreatedUser { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }
}
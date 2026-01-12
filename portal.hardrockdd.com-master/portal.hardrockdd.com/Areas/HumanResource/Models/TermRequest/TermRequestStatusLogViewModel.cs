using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.HumanResource.Models.TermRequest
{
    public class TermRequestStatusLogListViewModel
    {
        public TermRequestStatusLogListViewModel()
        {

        }

        public TermRequestStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            List = workFlow.Sequences.Select(s => new TermRequestStatusLogViewModel(s)).ToList();
        }

        public List<TermRequestStatusLogViewModel> List { get; }
    }

    public class TermRequestStatusLogViewModel: portal.Models.Views.WF.WorkFlowStatusViewModel
    {

        public TermRequestStatusLogViewModel()
        {

        }

        public TermRequestStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlowSequence sequence) : base(sequence)
        {
            if (sequence == null)
                return;

            Status = (DB.HRTermRequestStatusEnum)sequence.Status;
        }

       
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRTermRequestStatusEnum Status { get; set; }
    }
}
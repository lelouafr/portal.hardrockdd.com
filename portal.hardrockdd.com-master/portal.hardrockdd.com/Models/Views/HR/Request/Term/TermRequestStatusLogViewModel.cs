using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Term
{
    public class TermRequestStatusLogListViewModel
    {
        public TermRequestStatusLogListViewModel()
        {

        }

        public TermRequestStatusLogListViewModel(Code.Data.VP.WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            List = workFlow.Sequances.Select(s => new TermRequestStatusLogViewModel(s)).ToList();
        }

        public List<TermRequestStatusLogViewModel> List { get; set; }
    }

    public class TermRequestStatusLogViewModel: WF.WorkFlowStatusViewModel
    {

        public TermRequestStatusLogViewModel()
        {

        }

        public TermRequestStatusLogViewModel(Code.Data.VP.WorkFlowSequance sequance): base(sequance)
        {
            if (sequance == null)
                return;

            Status = (HRTermRequestStatusEnum)sequance.Status;
        }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public HRTermRequestStatusEnum Status { get; set; }
    }
}
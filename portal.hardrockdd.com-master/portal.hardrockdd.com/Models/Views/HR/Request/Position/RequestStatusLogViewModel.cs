using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Position
{
    public class RequestStatusLogListViewModel
    {
        public RequestStatusLogListViewModel()
        {

        }

        public RequestStatusLogListViewModel(Code.Data.VP.WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            List = workFlow.Sequances.Select(s => new RequestStatusLogViewModel(s)).ToList();
        }

        public List<RequestStatusLogViewModel> List { get; set; }
    }

    public class RequestStatusLogViewModel: WF.WorkFlowStatusViewModel
    {

        public RequestStatusLogViewModel()
        {

        }

        public RequestStatusLogViewModel(Code.Data.VP.WorkFlowSequance sequance): base(sequance)
        {
            if (sequance == null)
                return;

            Status = (HRPositionRequestStatusEnum)sequance.Status;
        }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public HRPositionRequestStatusEnum Status { get; set; }
    }
}
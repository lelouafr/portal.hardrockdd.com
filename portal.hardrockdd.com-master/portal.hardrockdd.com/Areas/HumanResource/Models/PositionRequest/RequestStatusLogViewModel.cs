using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.HumanResource.Models.PositionRequest
{
    public class RequestStatusLogListViewModel
    {
        public RequestStatusLogListViewModel()
        {

        }

        public RequestStatusLogListViewModel(WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            List = workFlow.Sequences.Select(s => new RequestStatusLogViewModel(s)).ToList();
        }

        public List<RequestStatusLogViewModel> List { get; }
    }

    public class RequestStatusLogViewModel: portal.Models.Views.WF.WorkFlowStatusViewModel
    {

        public RequestStatusLogViewModel()
        {

        }

        public RequestStatusLogViewModel(WorkFlowSequence sequence): base(sequence)
        {
            if (sequence == null)
                return;

            Status = (DB.HRPositionRequestStatusEnum)sequence.Status;
        }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRPositionRequestStatusEnum Status { get; set; }
    }
}
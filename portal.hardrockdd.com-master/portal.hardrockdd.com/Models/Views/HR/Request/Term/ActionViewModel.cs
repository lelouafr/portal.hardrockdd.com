using portal.Code.Data.VP;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Request.Term
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {

        }

        public ActionViewModel(HRTermRequest request)
        {

            HRCo = request.HRCo;
            RequestId = request.RequestId;

            WorkFlowActions = BuildActions(request);
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }


        public List<WF.WorkFlowAction> BuildActions(HRTermRequest request)
        {
            var results = new List<WF.WorkFlowAction>();

            switch (request.Status)
            {
                case HRTermRequestStatusEnum.New:
                    results.Add(new WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)HRTermRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.Submitted:
                    results.Add(new WF.WorkFlowAction() { Title = "Approve", GotoStatusId = (int)HRTermRequestStatusEnum.PRReview, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.Approved:
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.PRReview:
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.HRReview:
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.Completed:
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRTermRequestStatusEnum.Canceled:
                    results.Add(new WF.WorkFlowAction() { Title = "Re-Open", GotoStatusId = (int)HRTermRequestStatusEnum.New, ButtonClass = "btn-primary", ActionRedirect = "Reload", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
using portal.Code.Data.VP;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Request.Position
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {

        }

        public ActionViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;

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


        private List<WF.WorkFlowAction> BuildActions(HRPositionRequest request)
        {
            var results = new List<WF.WorkFlowAction>();

            switch (request.Status)
            {
                case HRPositionRequestStatusEnum.New:
                    results.Add(new WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)HRPositionRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRPositionRequestStatusEnum.Submitted:
                    results.Add(new WF.WorkFlowAction() { Title = "Approve", GotoStatusId = (int)HRPositionRequestStatusEnum.HRApproved, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRPositionRequestStatusEnum.HRApproved:
                    results.Add(new WF.WorkFlowAction() { Title = "Send to HR", GotoStatusId = (int)HRPositionRequestStatusEnum.ManagementReviewed, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    if (HttpContext.Current.User.IsInPosition("IT-DIR"))
                        results.Add(new WF.WorkFlowAction() { Title = "Hire", GotoStatusId = (int)HRPositionRequestStatusEnum.Hire, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRPositionRequestStatusEnum.ManagementReviewed:
                    results.Add(new WF.WorkFlowAction() { Title = "Hire", GotoStatusId = (int)HRPositionRequestStatusEnum.Hire, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case HRPositionRequestStatusEnum.Hire:
                    break;
                case HRPositionRequestStatusEnum.Canceled:
                    results.Add(new WF.WorkFlowAction() { Title = "Re-Open", GotoStatusId = (int)HRPositionRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Reload", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
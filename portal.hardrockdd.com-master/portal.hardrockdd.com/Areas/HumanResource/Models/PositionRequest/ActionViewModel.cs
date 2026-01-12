using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.PositionRequest
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

        public List<portal.Models.Views.WF.WorkFlowAction> WorkFlowActions { get; }


        private List<portal.Models.Views.WF.WorkFlowAction> BuildActions(HRPositionRequest request)
        {
            var results = new List<portal.Models.Views.WF.WorkFlowAction>();

            switch (request.Status)
            {
                case DB.HRPositionRequestStatusEnum.New:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Submitted, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right" });
                    break;
                case DB.HRPositionRequestStatusEnum.Submitted:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Approve", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.HRApproved, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right" });
                    break;
                case DB.HRPositionRequestStatusEnum.HRApproved:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Send to HR", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.ManagementReviewed, ButtonClass = "btn-primary" });
                    //if (HttpContext.Current.User.IsInPosition("IT-DIR"))
                    //    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Hire", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Hire, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right" });
                    break;
                case DB.HRPositionRequestStatusEnum.ManagementReviewed:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Hire", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Hire, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right" });
                    break;
                case DB.HRPositionRequestStatusEnum.Hire:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Complete", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Complete, ButtonClass = "btn-primary" });
                    break;
                case DB.HRPositionRequestStatusEnum.Canceled:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Re-Open", GotoStatusId = (int)DB.HRPositionRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Reload", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
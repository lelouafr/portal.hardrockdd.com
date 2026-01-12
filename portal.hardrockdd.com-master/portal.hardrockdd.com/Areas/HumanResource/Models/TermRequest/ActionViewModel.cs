using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.TermRequest
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

        public List<portal.Models.Views.WF.WorkFlowAction> WorkFlowActions { get; }


        public List<portal.Models.Views.WF.WorkFlowAction> BuildActions(HRTermRequest request)
        {
            var results = new List<portal.Models.Views.WF.WorkFlowAction>();

            switch (request.Status)
            {
                case DB.HRTermRequestStatusEnum.New:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.Submitted:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Approve", GotoStatusId = (int)DB.HRTermRequestStatusEnum.PRReview, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.Approved:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.PRReview:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.HRReview:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.Completed:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.HRTermRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.HRTermRequestStatusEnum.Canceled:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Re-Open", GotoStatusId = (int)DB.HRTermRequestStatusEnum.New, ButtonClass = "btn-primary", ActionRedirect = "Reload", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
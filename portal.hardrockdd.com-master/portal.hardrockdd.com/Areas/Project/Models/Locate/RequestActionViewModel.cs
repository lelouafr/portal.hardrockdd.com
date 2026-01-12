using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class RequestActionViewModel
    {
        public RequestActionViewModel()
        {

        }

        public RequestActionViewModel(PMLocateRequest request)
        {
            if (request == null)
                return;

            RequestId = request.RequestId;

            WorkFlowActions = BuildActions(request);
        }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        public List<portal.Models.Views.WF.WorkFlowAction> WorkFlowActions { get; }

        private List<portal.Models.Views.WF.WorkFlowAction> BuildActions(PMLocateRequest request)
        {
            var results = new List<portal.Models.Views.WF.WorkFlowAction>();

            
            switch (request.Status)
            {
                case DB.PMRequestStatusEnum.Submitted:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Completed", GotoStatusId = (int)DB.PMRequestStatusEnum.Completed, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.PMRequestStatusEnum.Canceled, ButtonClass = "btn-danger pull-right" });
                    break;
                case DB.PMRequestStatusEnum.Completed:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.PMRequestStatusEnum.Submitted, ButtonClass = "btn-primary" });
                    break;
                case DB.PMRequestStatusEnum.Canceled:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.PMRequestStatusEnum.Submitted, ButtonClass = "btn-primary" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
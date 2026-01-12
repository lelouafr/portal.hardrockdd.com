using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.WA.Application
{

    public class ActionViewModel
    {
        public ActionViewModel()
        {

        }

        public ActionViewModel(Code.Data.VP.WebApplication application)
        {

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            WorkFlowActions = BuildActions(application);
        }
        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }


        public List<WF.WorkFlowAction> BuildActions(Code.Data.VP.WebApplication application)
        {
            var results = new List<WF.WorkFlowAction>();
            switch (application.Status)
            {
                case WAApplicationStatusEnum.Draft:
                case WAApplicationStatusEnum.Started:
                    results.Add(new WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)WAApplicationStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case WAApplicationStatusEnum.Submitted:
                    results.Add(new WF.WorkFlowAction() { Title = "Approved", GotoStatusId = (int)WAApplicationStatusEnum.Approved, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new WF.WorkFlowAction() { Title = "Denied", GotoStatusId = (int)WAApplicationStatusEnum.Denied, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case WAApplicationStatusEnum.Approved:
                    results.Add(new WF.WorkFlowAction() { Title = "Shelve", GotoStatusId = (int)WAApplicationStatusEnum.Shelved, ButtonClass = "btn-warning", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case WAApplicationStatusEnum.Hired:
                    break;
                case WAApplicationStatusEnum.Shelved:
                    results.Add(new WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case WAApplicationStatusEnum.Denied:
                    results.Add(new WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case WAApplicationStatusEnum.Canceled:
                    results.Add(new WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
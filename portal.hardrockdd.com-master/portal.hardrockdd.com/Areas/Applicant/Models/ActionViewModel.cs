using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portal.Areas.Applicant.Models
{

    public class ActionViewModel
    {
        public ActionViewModel()
        {

        }

        public ActionViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            WorkFlowActions = BuildActions(application);
        }
        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        public List<portal.Models.Views.WF.WorkFlowAction> WorkFlowActions { get; }


        public static List<portal.Models.Views.WF.WorkFlowAction> BuildActions(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            var results = new List<portal.Models.Views.WF.WorkFlowAction>();
            if (application == null)
                return results;

            switch (application.Status)
            {
                case DB.WAApplicationStatusEnum.Draft:
                case DB.WAApplicationStatusEnum.Started:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)DB.WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.WAApplicationStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.WAApplicationStatusEnum.Submitted:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Approved", GotoStatusId = (int)DB.WAApplicationStatusEnum.Approved, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Denied", GotoStatusId = (int)DB.WAApplicationStatusEnum.Denied, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.WAApplicationStatusEnum.Approved:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Shelve", GotoStatusId = (int)DB.WAApplicationStatusEnum.Shelved, ButtonClass = "btn-warning", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.WAApplicationStatusEnum.Hired:
                    break;
                case DB.WAApplicationStatusEnum.Shelved:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.WAApplicationStatusEnum.Denied:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                case DB.WAApplicationStatusEnum.Canceled:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.WAApplicationStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}
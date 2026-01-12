using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace portal.Areas.AccountsPayable.Models.Document
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(APDocument document)
        {
            if (document == null)
            {
                throw new System.ArgumentNullException(nameof(document));
            }
            APCo = document.APCo;
            DocId = document.DocId;

            var inWorkFlow = document.WorkFlow.CurrentSequence().AssignedUsers.Any(f => f.AssignedTo == document.db.CurrentUserId);

            WorkFlowActions = BuildActions(document);
            RequestInfo = new DocumentRequestInfoViewModel(document);
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int DocId { get; set; }

        public List<portal.Models.Views.WF.WorkFlowAction> WorkFlowActions { get; }
        public DocumentRequestInfoViewModel RequestInfo { get; set; }

        private List<portal.Models.Views.WF.WorkFlowAction> BuildActions(APDocument document)
        {
            var results = new List<portal.Models.Views.WF.WorkFlowAction>();

            switch (document.Status)
            {
                case DB.APDocumentStatusEnum.New:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Filed", GotoStatusId = (int)DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-primary" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Mark as duplicate", GotoStatusId = (int)DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger" });
                    break;
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.LinesAdded:
                case DB.APDocumentStatusEnum.Error:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Request Info", GotoStatusId = (int)DB.APDocumentStatusEnum.RequestedInfo, ButtonClass = "btn-warning" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Mark as duplicate", GotoStatusId = (int)DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Remove", GotoStatusId = (int)DB.APDocumentStatusEnum.Canceled, ButtonClass = "btn-danger" });
                    break;
                case DB.APDocumentStatusEnum.RequestedInfo:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Send Back to File", GotoStatusId = (int)DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-success" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Mark as duplicate", GotoStatusId = (int)DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Remove", GotoStatusId = (int)DB.APDocumentStatusEnum.Canceled, ButtonClass = "btn-danger" });
                    break;
                case DB.APDocumentStatusEnum.Duplicate:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Send Back to File", GotoStatusId = (int)DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-success" });
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Remove", GotoStatusId = (int)DB.APDocumentStatusEnum.Canceled, ButtonClass = "btn-danger" });
                    break;
                case DB.APDocumentStatusEnum.Canceled:
                    results.Add(new portal.Models.Views.WF.WorkFlowAction() { Title = "Reopen", GotoStatusId = (int)DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-success" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }


}
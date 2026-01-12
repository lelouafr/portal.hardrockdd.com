using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{
    public class WizardFormViewModel
    {
        public WizardFormViewModel()
        {

        }

        public WizardFormViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document, Controller controller)
        {
            if (document == null)
            {
                throw new System.ArgumentNullException(nameof(document));
            }

            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            StatusInt = document.StatusId;
            #endregion

            WizardSteps = BuildSteps(controller);
            WizardActions = BuildActions(controller);
            UniqueAttchID = document.Attachment.UniqueAttchID;
            Forum = new portal.Models.Views.Forums.ForumLineListViewModel(document.Forum);
            WorkFlowUsers = new portal.Models.Views.WF.WorkFlowUserListViewModel(document.WorkFlow.CurrentSequence());

        }

        
        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Document Id")]
        public int DocId { get; set; }

        public int StatusInt { get; set; }

        public List<WizardSteps> WizardSteps { get; }

        public List<WizardAction> WizardActions { get; }

        public portal.Models.Views.WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public Guid UniqueAttchID { get; set; }

        public portal.Models.Views.Forums.ForumLineListViewModel Forum { get; set; }


        public List<WizardSteps> BuildSteps(Controller controller)
        {
            var baseSteps = new List<WizardSteps>();

            if (controller == null)
                return baseSteps;

            baseSteps.Add(new WizardSteps { Title = "Vendor/Invoice Info", AjaxUrl = controller.Url.Action("InfoForm", "Document", new {  apco = APCo, docId = DocId }) });

            switch ((DB.APDocumentStatusEnum)StatusInt)
            {
                case DB.APDocumentStatusEnum.New:
                    break;
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.Error:
                case DB.APDocumentStatusEnum.LinesAdded:
                case DB.APDocumentStatusEnum.Duplicate:
                case DB.APDocumentStatusEnum.RequestedInfo:
                    baseSteps.Add(new WizardSteps { Title = "Invoice Line(s)", AjaxUrl = controller.Url.Action("LineListPanel", "Document", new { apco = APCo, docId = DocId }) });
                    baseSteps.Add(new WizardSteps { Title = "Review", AjaxUrl = controller.Url.Action("SummaryPanel", "Document", new { apco = APCo, docId = DocId }) });
                    baseSteps.Add(new WizardSteps { Title = "Submit", AjaxUrl = controller.Url.Action("ActionPanel", "Document", new { apco = APCo, docId = DocId }) });
                    break;
                case DB.APDocumentStatusEnum.Processed:
                    baseSteps.Add(new WizardSteps { Title = "Invoice Line(s)", AjaxUrl = controller.Url.Action("LineListPanel", "Document", new { apco = APCo, docId = DocId }) });
                    break;
                default:
                    break;
            }

            var stepId = 1;
            baseSteps.ForEach(e => e.StepId = stepId++);

            return baseSteps;
        }


        public List<WizardAction> BuildActions(Controller controller)
        {
            var results = new List<WizardAction>();

            switch ((DB.APDocumentStatusEnum)StatusInt)
            {
                case DB.APDocumentStatusEnum.New:
                    results.Add(new WizardAction() { Title = "Filed", GotoStatusId = DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-success", ActionRedirect = "Reload" });
                    //results.Add(new WizardAction() { Title = "Mark as duplicate", GotoStatusId = DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger", ActionRedirect = "Reload" });
                    break;
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.Error:
                case DB.APDocumentStatusEnum.Reviewed:
                    results.Add(new WizardAction() { Title = "Send To Batch", GotoStatusId = DB.APDocumentStatusEnum.Processed, ButtonClass = "btn-success", ActionRedirect = "Reload" });
                    //results.Add(new WizardAction() { Title = "Mark as duplicate", GotoStatusId = DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger", ActionRedirect = "Reload" });
                    break;
                case DB.APDocumentStatusEnum.Processed:
                    break;
                case DB.APDocumentStatusEnum.RequestedInfo:
                    results.Add(new WizardAction() { Title = "Send back to Filed", GotoStatusId = DB.APDocumentStatusEnum.Filed, ButtonClass = "btn-success", ActionRedirect = "Reload" });
                    //results.Add(new WizardAction() { Title = "Mark as duplicate", GotoStatusId = DB.APDocumentStatusEnum.Duplicate, ButtonClass = "btn-danger", ActionRedirect = "Reload" });
                    break;
                default:
                    break;
            }
            return results;
        }
    }


    public class WizardSteps
    {

        public int StepId { get; set; }

        public string AjaxUrl { get; set; }

        public string Title { get; set; }
    }

    public class WizardAction
    {
        public int ActionId { get; set; }

        public string Title { get; set; }

        public string ButtonClass { get; set; }

        public string ActionUrl { get; set; }

        public DB.APDocumentStatusEnum GotoStatusId { get; set; }

        public string ActionRedirect { get; set; }
    }
}
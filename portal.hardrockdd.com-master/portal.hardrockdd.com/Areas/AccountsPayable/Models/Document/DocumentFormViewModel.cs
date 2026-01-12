using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{

    public class DocumentFormViewModel
    {
        public DocumentFormViewModel()
        {

        }

        public DocumentFormViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document, Controller controller)
        {
            if (document == null)
                return;

            APCo = document.APCo;
            DocId = document.DocId;

            UniqueAttchId = document.Attachment.UniqueAttchID;

            Info = new DocumentInfoViewModel(document);
            Action = new ActionViewModel(document);
            DocStatusLogs = new DocumentStatusLogListViewModel(document);
            StatusLogs = new StatusLogListViewModel(document.WorkFlow);
            Forum = new portal.Models.Views.Forums.ForumLineListViewModel(document.Forum);
            Wizard = new WizardFormViewModel(document, controller);
            RequestInfo = new DocumentRequestInfoViewModel(document);

            Lines = new DocumentLineListViewModel(document);
        }

        public DocumentFormViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document)
        {
            if (document == null)
                return;

            APCo = document.APCo;
            DocId = document.DocId;

            UniqueAttchId = document.Attachment.UniqueAttchID;

            Info = new DocumentInfoViewModel(document);
            Action = new ActionViewModel(document);
            DocStatusLogs = new DocumentStatusLogListViewModel(document);
            StatusLogs = new StatusLogListViewModel(document.WorkFlow);
            Forum = new portal.Models.Views.Forums.ForumLineListViewModel(document.Forum);
            Wizard = new WizardFormViewModel();

            RequestInfo = new DocumentRequestInfoViewModel(document);
            Lines = new DocumentLineListViewModel(document);
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                return false;

            var ok = modelState.IsValid;
            ok &= Info.Validate(modelState);
            //ok &= Lines.Validate(modelState);

            return ok;
        }

        [Key]
        [Required]
        public byte APCo { get; set; }

        [Key]
        [Required]
        public int DocId { get; set; }

        public System.Guid? UniqueAttchId { get; set; }

        public DocumentInfoViewModel Info { get; set; }

        public ActionViewModel Action { get; set; }

        public DocumentStatusLogListViewModel DocStatusLogs { get; set; }

        public StatusLogListViewModel StatusLogs { get; set; }

        public portal.Models.Views.Forums.ForumLineListViewModel Forum { get; set; }

        public DocumentLineListViewModel Lines { get; set; }

        public WizardFormViewModel Wizard { get; set; }

        public DocumentRequestInfoViewModel RequestInfo { get; set; }

    }
}
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Document
{

    public class DocumentFormViewModel : AuditBaseViewModel
    {
        public DocumentFormViewModel()
        {

        }

        public DocumentFormViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document) : base(document)
        {
            Document = new DocumentViewModel(document);
            Action = new ActionViewModel(document);
            Sequences = new DocumentSeqListViewModel(document);
            StatusLogs = new DocumentStatusLogListViewModel(document);
            Forum = new Forums.ForumLineListViewModel(document.Forum);
            //Attachments = new Attachment.AttachmentListViewModel(purchaseOrder.POCo, "POHD", purchaseOrder.KeyID, purchaseOrder.UniqueAttchID);
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            return ok;
        }

        public DocumentViewModel Document { get; set; }

        public DocumentSeqListViewModel Sequences { get; set; }

         public ActionViewModel Action { get; set; }

        public DocumentStatusLogListViewModel StatusLogs { get; set; }

        public Forums.ForumLineListViewModel Forum { get; set; }

        //public Attachment.AttachmentListViewModel Attachments { get; set; }
    }
}
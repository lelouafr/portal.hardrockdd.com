using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Invoice
{

    public class InvoiceFormViewModel 
    {
        public InvoiceFormViewModel()
        {

        }

        public InvoiceFormViewModel(DB.Infrastructure.ViewPointDB.Data.APTran invoice)
        {
            Invoice = new InvoiceViewModel(invoice);
            Action = new ActionViewModel(invoice);
            Lines = new InvoiceLineListViewModel(invoice);
            //StatusLogs = new PORequestStatusLogListViewModel(purchaseOrder);
            UniqueAttchId = invoice.Attachment.UniqueAttchID;
            //Attachments = new Attachment.AttachmentListViewModel(invoice.APCo, "APTH", invoice.KeyID, invoice.UniqueAttchID);
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
        public System.Guid? UniqueAttchId { get; set; }

        public InvoiceViewModel Invoice { get; set; }

        public InvoiceLineListViewModel Lines { get; set; }

        public ActionViewModel Action { get; set; }

        //public PORequestStatusLogListViewModel StatusLogs { get; set; }

        //public Attachment.AttachmentListViewModel Attachments { get; set; }
    }
}
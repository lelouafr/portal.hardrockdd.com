using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Invoice
{

    public class InvoiceFormViewModel 
    {
        public InvoiceFormViewModel()
        {

        }

        public InvoiceFormViewModel(DB.Infrastructure.ViewPointDB.Data.ARTran invoice)
        {
            Invoice = new InvoiceViewModel(invoice);
            Action = new ActionViewModel(invoice);
            Lines = new InvoiceLineListViewModel(invoice);
            UniqueAttchId = invoice.Attachment.UniqueAttchID;
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

    }
}
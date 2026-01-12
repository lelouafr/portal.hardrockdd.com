using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Order
{

    public class PurchaseOrderFormViewModel 
    {
        public PurchaseOrderFormViewModel()
        {

        }

        public PurchaseOrderFormViewModel(DB.Infrastructure.ViewPointDB.Data.PurchaseOrder purchaseOrder)
        {
            Order = new PurchaseOrderViewModel(purchaseOrder);
            Action = new ActionViewModel(purchaseOrder);
            Items = new PurchaseOrderItemListViewModel(purchaseOrder);
            //StatusLogs = new PORequestStatusLogListViewModel(purchaseOrder);
            Attachments = new Attachment.AttachmentListViewModel(purchaseOrder.POCo, "POHD", purchaseOrder.KeyID, purchaseOrder.UniqueAttchID);
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

        public PurchaseOrderViewModel Order { get; set; }

        public PurchaseOrderItemListViewModel Items { get; set; }

        public ActionViewModel Action { get; set; }

        //public PORequestStatusLogListViewModel StatusLogs { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }
    }
}
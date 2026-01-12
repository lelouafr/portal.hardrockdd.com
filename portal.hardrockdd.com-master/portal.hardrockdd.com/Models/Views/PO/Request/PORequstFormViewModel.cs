using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{

    public class PORequstFormViewModel 
    {
        public PORequstFormViewModel()
        {

        }

        public PORequstFormViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest request)
        {
            Request = new PORequestViewModel(request);
            Action = new ActionViewModel(request);
            Lines = new PORequestLineListViewModel(request);
            StatusLogs = new PORequestStatusLogListViewModel(request);
            WorkFlowUsers = new WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequence());
            Attachments = new Attachment.AttachmentListViewModel(request.POCo, "udPORH", request.KeyID, request.UniqueAttchID);
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

        public PORequestViewModel Request { get; set; }

        public PORequestLineListViewModel Lines { get; set; }

        public ActionViewModel Action { get; set; }

        public PORequestStatusLogListViewModel StatusLogs { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }
    }
}
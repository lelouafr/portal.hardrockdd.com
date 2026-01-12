using Org.BouncyCastle.Ocsp;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class LeaveRequstFormViewModel 
    {
        public LeaveRequstFormViewModel()
        {

        }

        public LeaveRequstFormViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest request)
        {
            Request = new LeaveRequestViewModel(request);
            Action = new ActionViewModel(request);
            Lines = new LeaveRequestLineListViewModel(request);
            StatusLogs = new LeaveRequestStatusLogListViewModel(request);
            //Attachments = new Attachment.AttachmentListViewModel(request.Co, "udPRLR", request.KeyID, request.UniqueAttchID);
            Summary = new LeaveEmployeeSummaryListViewModel(request);
            //WorkFlowUsers = new WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequence());
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            ok &= Request.Validate(modelState);
            return ok;
        }

        public LeaveRequestViewModel Request { get; set; }

        public LeaveRequestLineListViewModel Lines { get; set; }

        public ActionViewModel Action { get; set; }

        public LeaveRequestStatusLogListViewModel StatusLogs { get; set; }

        public LeaveEmployeeSummaryListViewModel Summary { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }
        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;

namespace portal.Areas.HumanResource.Models.TermRequest
{
    public class TermRequestFormViewModel
    {
        public TermRequestFormViewModel(HRTermRequest request)
        {
            if (request == null)
                return;
            HRCo = request.HRCo;
            RequestId = request.RequestId;
            Info = new TermRequestViewModel(request);
            
            //Attachments = new portal.Models.Views.Attachment.AttachmentListViewModel(request.Attachment);
            WorkFlowUsers = new portal.Models.Views.WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequence());
            StatusLogs = new TermRequestStatusLogListViewModel(request.WorkFlow);
            AssignedAssets = new Models.Resource.Form.AssignedAssetListViewModel(request.HRResource);
            Actions = new ActionViewModel(request);
        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        public TermRequestViewModel Info { get; set; }

        public portal.Models.Views.WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public TermRequestStatusLogListViewModel StatusLogs { get; set; }

        public Models.Resource.Form.AssignedAssetListViewModel AssignedAssets { get; set; }

        public ActionViewModel Actions { get; set; }
    }
}
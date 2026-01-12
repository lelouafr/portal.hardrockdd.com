using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Term
{
    public class TermRequestFormViewModel
    {
        public TermRequestFormViewModel(Code.Data.VP.HRTermRequest request)
        {
            if (request == null)
                return;
            HRCo = request.HRCo;
            RequestId = request.RequestId;
            Info = new TermRequestViewModel(request);
            
            Attachments = new Attachment.AttachmentListViewModel(request.Attachment);
            WorkFlowUsers = new WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequance());
            StatusLogs = new TermRequestStatusLogListViewModel(request.WorkFlow);
            AssignedAssets = new Resource.Form.ResourceAssignedAssetListViewModel(request.HRResource);
            Actions = new ActionViewModel(request);
        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        public TermRequestViewModel Info { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public TermRequestStatusLogListViewModel StatusLogs { get; set; }

        public Resource.Form.ResourceAssignedAssetListViewModel AssignedAssets { get; set; }

        public ActionViewModel Actions { get; set; }
    }
}
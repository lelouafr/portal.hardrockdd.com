using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Position
{
    public class RequestFormViewModel
    {
        public RequestFormViewModel(Code.Data.VP.HRPositionRequest request)
        {
            if (request == null)
                return;
            HRCo = request.HRCo;
            RequestId = request.RequestId;
            Info = new RequestViewModel(request);
            
            WorkFlowUsers = new WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequance());
            StatusLogs = new RequestStatusLogListViewModel(request.WorkFlow);
            //DefaultAssets = new Resource.Form.ResourceAssignedAssetListViewModel(request.HRResource);
            Actions = new ActionViewModel(request);
            Applications = new ApplicationListViewModel(request);
            PayrollInfo = new PayroleViewModel(request);
        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        public RequestViewModel Info { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public RequestStatusLogListViewModel StatusLogs { get; set; }

       // public Resource.Form.ResourceAssignedAssetListViewModel DefaultAssets { get; set; }

        public ActionViewModel Actions { get; set; }

        public ApplicationListViewModel Applications { get; set; }

        public PayroleViewModel PayrollInfo { get; set; }
    }
}
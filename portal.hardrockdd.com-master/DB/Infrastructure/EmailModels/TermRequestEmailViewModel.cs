using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.EmailModels
{
  
    public class TermRequestEmailViewModel
    {
        public TermRequestEmailViewModel()
        {

        }

        public TermRequestEmailViewModel(HRTermRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            HRRef = request.HRRef;
            Comments = request.Comments;
            RequestedOn = request.RequestedDate;
            RequestedBy = request.RequestedBy;
            Status = request.Status;
            PRComments = request.PRComments;
            HRComments = request.HRComments;
            TermCodeId = request.TermCodeId;
            TermDate = request.TermDate;
            EffectiveDate = request.EffectiveDate;
            PayrollEndDate = request.PayrollEndDate;
            IsOpenRequest = request.IsOpenRequest;

            if (request.RequestedUser != null)
                RequestedUser = request.RequestedUser.FullName();
            if (request.HRResource != null)
                EmployeeName = request.HRResource.FullName(true);

        }

        public byte HRCo { get; set; }

        public int RequestId { get; set; }

        public int? HRRef { get; set; }

        public string Comments { get; set; }

        public System.DateTime RequestedOn { get; set; }

        public string RequestedBy { get; set; }

        public DB.HRTermRequestStatusEnum Status { get; set; }

        public string PRComments { get; set; }

        public string HRComments { get; set; }

        public string TermCodeId { get; set; }

        public DateTime? TermDate { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? PayrollEndDate { get; set; }

        public bool IsOpenRequest { get; set; }

        public string EmployeeName { get; set; }

        public string RequestedUser { get; set; }
    }
}

using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;

namespace portal.Areas.HumanResource.Models.PositionRequest
{
    public class RequestFormViewModel
    {
        public RequestFormViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;
            HRCo = request.HRCo;
            RequestId = request.RequestId;
            Info = new RequestViewModel(request);
            
            WorkFlowUsers = new portal.Models.Views.WF.WorkFlowUserListViewModel(request.WorkFlow.CurrentSequence());
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

        public portal.Models.Views.WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public RequestStatusLogListViewModel StatusLogs { get; set; }

       // public Resource.Form.ResourceAssignedAssetListViewModel DefaultAssets { get; set; }

        public ActionViewModel Actions { get; set; }

        public ApplicationListViewModel Applications { get; set; }

        public PayroleViewModel PayrollInfo { get; set; }

        public bool ValidateNewHire(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (modelState == null)
                return false;

            if (PayrollInfo.HRDrugTestDate == null)
                modelState.AddModelError("HRDrugTestDate", "Drug Test Date Required");

            if (PayrollInfo.WPDivisionId == null)
                modelState.AddModelError("WPDivisionId", "Division Required");

            if (PayrollInfo.PRInsCodeId == null)
                modelState.AddModelError("PRInsCodeId", "Ins. Code Required");

            if (PayrollInfo.PRDeptId == null)
                modelState.AddModelError("PRDeptId", "Department Required");

            if (PayrollInfo.PRHrlyRate == null && PayrollInfo.PREarnCodeId == 1)
                modelState.AddModelError("PRHrlyRate", "Hourly Rate Required");

            if (PayrollInfo.PRSalaryAmt == null && PayrollInfo.PREarnCodeId == 4)
                modelState.AddModelError("PRSalaryAmt", "Salary Rate Required");

            if (PayrollInfo.HRDrugTestType == null)
                modelState.AddModelError("HRDrugTestType", "Drug Test Required");

            if (PayrollInfo.HRDrugTestStatus != "N")
                modelState.AddModelError("HRDrugTestStatus", "Drug Test Results Required to be Negative");

            return modelState.IsValid;
        }
    }
}
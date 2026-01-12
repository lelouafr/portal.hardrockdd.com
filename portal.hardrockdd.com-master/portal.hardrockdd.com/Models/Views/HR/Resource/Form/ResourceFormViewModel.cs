using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource.Form
{
    public class ResourceFormViewModel : AuditBaseViewModel
    {
        public ResourceFormViewModel()
        {

        }

        public ResourceFormViewModel(HRResource resource) : base(resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;
            Info = new ResourceInfoViewModel(resource);

            PayrollAssignment = new PR.Employee.Form.PayrollAssignmentViewModel(resource);
            PayrollInfo = new PR.Employee.Form.PayrollInfoViewModel(resource);
            PayrollPayInfo = new PR.Employee.Form.PayrollPayInfoViewModel(resource);
            PayrollHistory = new PR.Employee.Form.PayrollHistoryViewModel(resource);
            PersonalInfo = new PR.Employee.Form.PersonalInfoViewModel(resource);

            DivingInfo = new ResourceDrivingInfoViewModel(resource);
            AssignedAssets = new ResourceAssignedAssetListViewModel(resource);
            ADInfo = new ResourceADInfoViewModel(resource);

            Attachments = new Attachment.AttachmentListViewModel(resource.HRCo, "HRRM", resource.KeyID, resource.UniqueAttchID);
            Actions = new ResourceActionViewModel(resource);
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public ResourceInfoViewModel Info { get; set; }

        public ResourceDrivingInfoViewModel DivingInfo { get; set; }

        public PR.Employee.Form.PayrollInfoViewModel PayrollInfo { get; set; }

        public PR.Employee.Form.PayrollPayInfoViewModel PayrollPayInfo { get; set; }

        public PR.Employee.Form.PersonalInfoViewModel PersonalInfo { get; set; }

        public PR.Employee.Form.PayrollHistoryViewModel PayrollHistory { get; set; }

        public PR.Employee.Form.PayrollAssignmentViewModel PayrollAssignment { get; set; }


        public ResourceAssignedAssetListViewModel AssignedAssets { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public ResourceADInfoViewModel ADInfo { get; set; }

        public ResourceActionViewModel Actions { get; set; }
    }
}
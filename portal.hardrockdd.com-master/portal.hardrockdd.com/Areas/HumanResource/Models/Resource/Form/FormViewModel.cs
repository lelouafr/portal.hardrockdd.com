using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class FormViewModel : portal.Models.Views.AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(HRResource resource) : base(resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;
            Info = new InfoViewModel(resource);

            PayrollAssignment = new AssignmentViewModel(resource);
            PayrollInfo = new PayrollInfoViewModel(resource);
            PayrollPayInfo = new PayInfoViewModel(resource);
            PayrollHistory = new PayrollHistoryViewModel(resource);
            PersonalInfo = new PersonalInfoViewModel(resource);

            DivingInfo = new DrivingInfoViewModel(resource);
            AssignedAssets = new AssignedAssetListViewModel(resource);
            ADInfo = new ADInfoViewModel(resource);

            UniqueAttchId = resource.Attachment.UniqueAttchID;
            Actions = new ActionViewModel(resource);
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public Guid? UniqueAttchId { get; set; }

        public InfoViewModel Info { get; set; }

        public DrivingInfoViewModel DivingInfo { get; set; }

        public PayrollInfoViewModel PayrollInfo { get; set; }

        public PayInfoViewModel PayrollPayInfo { get; set; }

        public PersonalInfoViewModel PersonalInfo { get; set; }

        public PayrollHistoryViewModel PayrollHistory { get; set; }

        public AssignmentViewModel PayrollAssignment { get; set; }

        public AssignedAssetListViewModel AssignedAssets { get; set; }

        public ADInfoViewModel ADInfo { get; set; }

        public ActionViewModel Actions { get; set; }
    }
}
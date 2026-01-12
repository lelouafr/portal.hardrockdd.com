using portal.Models.Views.Attachment;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Forms
{

    public class EquipmentFormViewModel : AuditBaseViewModel
    {
        public EquipmentFormViewModel()
        {

        }

        public EquipmentFormViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment) : base(equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }
            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            Info = new EquipmentInfoViewModel(equipment);
            Ownership = new EquipmentOwnershipViewModel(equipment);
            Assignment = new EquipmentAssignmentViewModel(equipment);
            LicInfo = new EquipmentLicInfoViewModel(equipment);
            Meter = new EquipmentMeterViewModel(equipment);
            Specifications = new EquipmentSpecViewModel(equipment);
            ServiceItems = new EquipmentServiceItemListViewModel(equipment);

            ServiceHistory = new EM.Equipment.Forms.ServiceHistoryListViewModel(equipment);
            Attachments = new AttachmentListViewModel(equipment.EMCo, "EMEM", equipment.KeyID, equipment.UniqueAttchID);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public string EquipmentId { get; set; }

        public EquipmentInfoViewModel Info { get; set; }

        public EquipmentOwnershipViewModel Ownership { get; set; }

        public EquipmentAssignmentViewModel Assignment { get; set; }

        public EquipmentLicInfoViewModel LicInfo { get; set; }

        public EquipmentMeterViewModel Meter { get; set; }

        public EquipmentSpecViewModel Specifications { get; set; }
        
        public EquipmentServiceItemListViewModel ServiceItems { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public EM.Equipment.Forms.ServiceHistoryListViewModel ServiceHistory { get; set; }

    }
}
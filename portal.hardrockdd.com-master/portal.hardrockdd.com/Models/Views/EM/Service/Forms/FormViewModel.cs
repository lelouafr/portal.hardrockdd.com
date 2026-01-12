using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Service.Forms
{

    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(EMServiceItem service) : base(service)
        {
            if (service == null)
            {
                throw new System.ArgumentNullException(nameof(service));
            }
            Co = service.EMCo;
            ServiceItemId = service.ServiceItemId;
            Info = new InfoViewModel(service);
            Trigger = new TriggerViewModel(service);
            AssignedEquipment = new EquipmentLinkListViewModel(service);
            AssignedCategories = new CategoryLinkListViewModel(service);
            //Attachments = new AttachmentListViewModel(equipment.EMCo, "budEMSM", equipment.KeyID, equipment.UniqueAttchID);
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public int ServiceItemId { get; set; }

        public InfoViewModel Info { get; set; }

        public TriggerViewModel Trigger { get; set; }

        public EquipmentLinkListViewModel AssignedEquipment { get; set; }

        public CategoryLinkListViewModel AssignedCategories { get; set; }

    }
}
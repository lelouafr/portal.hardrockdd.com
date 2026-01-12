using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.EM.WorkOrder.Item.Forms
{

    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(EMWorkOrderItem item, Controller controller) : base(item)
        {
            if (item == null)
                return;

            EMCo = item.EMCo;
            WorkOrderId = item.WorkOrderId;
            ItemId = item.WOItem;
            Info = new InfoViewModel(item);
            //Trigger = new TriggerViewModel(workOrder);
            WorkFlowActions = BuildActions(controller);
            //EquipmentLines = new EquipmentLineListViewModel(workOrder);
            //AssignedCategories = new CategoryLinkListViewModel(workOrder);
            //Attachments = new AttachmentListViewModel(equipment.EMCo, "budEMSM", equipment.KeyID, equipment.UniqueAttchID);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public string WorkOrderId { get; set; }

        [Key]
        [HiddenInput]
        public int ItemId { get; set; }

        public InfoViewModel Info { get; set; }

        //public TriggerViewModel Trigger { get; set; }

        //public EquipmentLineListViewModel EquipmentLines { get; set; }

        //public CategoryLinkListViewModel AssignedCategories { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }


        public List<WF.WorkFlowAction> BuildActions(Controller controller)
        {
            var results = new List<WF.WorkFlowAction>();

            //if (Info.RequestType == SMRequestType.Equipment)
            //{
            //    switch(Info.Status)
            //    {
            //        case DB.SMRequestStatusEnum.Draft:
            //            results.Add(new WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)DB.SMRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home" });
            //            results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.SMRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home" });
            //            break;
            //        case DB.SMRequestStatusEnum.Submitted:
            //            results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.SMRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home" });
            //            break;
            //        default:
            //            break;
            //    }

            //}
            //else
            //{
            //}
            return results;
        }
    }
}
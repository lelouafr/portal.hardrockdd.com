using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.EM.WorkOrder.Forms
{

    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(EMWorkOrder workOrder, Controller controller) : base(workOrder)
        {
            if (workOrder == null)
                return;

            EMCo = workOrder.EMCo;
            WorkOrderId = workOrder.WorkOrderId;
            Info = new InfoViewModel(workOrder);
            Items = new Item.WorkOrderItemListViewModel(workOrder);
            WorkFlowActions = BuildActions(controller);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public string WorkOrderId { get; set; }

        public InfoViewModel Info { get; set; }

        public Item.WorkOrderItemListViewModel Items { get; set; }

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
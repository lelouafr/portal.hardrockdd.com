using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Forms
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {

        }

        public ActionViewModel(SMRequest request)
        {

            SMCo = request.SMCo;
            RequestId = request.RequestId;

            WorkFlowActions = BuildActions(request);
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }


        public List<WF.WorkFlowAction> BuildActions(SMRequest request)
        {
            var results = new List<WF.WorkFlowAction>();

            if (request.RequestType == DB.SMRequestTypeEnum.Equipment)
            {
                switch (request.Status)
                {
                    case DB.SMRequestStatusEnum.Draft:
                        results.Add(new WF.WorkFlowAction() { Title = "Submit", GotoStatusId = (int)DB.SMRequestStatusEnum.Submitted, ButtonClass = "btn-primary", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                        results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.SMRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                        break;
                    case DB.SMRequestStatusEnum.Submitted:
                        results.Add(new WF.WorkFlowAction() { Title = "Create Work Order", GotoStatusId = (int)DB.SMRequestStatusEnum.Submitted, ButtonClass = "btn-success", ActionRedirect = "WorkOrder", ActionUrl = "/ServiceRequest/CreateWorkOrderFromRequest" });
                        results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.SMRequestStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Home", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                        break;
                    case DB.SMRequestStatusEnum.Canceled:
                        results.Add(new WF.WorkFlowAction() { Title = "Re Open", GotoStatusId = (int)DB.SMRequestStatusEnum.Draft, ButtonClass = "btn-primary", ActionRedirect = "Reload", ActionUrl = "/ServiceRequest/ServiceRequestPanel" });
                        break;
                    default:
                        break;
                }

            }
            else
            {
            }
            return results;
        }
    }
}
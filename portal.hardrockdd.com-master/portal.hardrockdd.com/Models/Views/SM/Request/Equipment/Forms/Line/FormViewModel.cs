using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment.Forms.Line
{

    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(SMRequestLine requestLine, Controller controller) : base(requestLine)
        {
            if (requestLine == null)
                return;

            SMCo = requestLine.SMCo;
            RequestId = requestLine.RequestId;
            LineId = requestLine.LineId;
            Info = new InfoViewModel(requestLine);
            AssingedUsers = new AssignmentListViewModel(requestLine);
            WorkFlowActions = BuildActions(controller);
            Forum = new Forums.ForumLineListViewModel(requestLine.GetForum());

            Files = new HQ.Attachment.FileExplorerListViewModel(requestLine.Attachment, 0);
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        [Key]
        [HiddenInput]
        public int LineId { get; set; }

        public InfoViewModel Info { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }

        public AssignmentListViewModel AssingedUsers { get; set; }

        public portal.Models.Views.HQ.Attachment.FileExplorerListViewModel Files { get; set; }

        public List<WF.WorkFlowAction> BuildActions(Controller controller)
        {
            var results = new List<WF.WorkFlowAction>();

            if (Info.RequestType == DB.SMRequestTypeEnum.Equipment)
            {
                switch(Info.Status)
                {
                    case DB.SMRequestLineStatusEnum.Pending:
                        //results.Add(new WF.WorkFlowAction() { Title = "Create/Add to WO", GotoStatusId = (int)DB.SMRequestLineStatusEnum.WorkOrderCreated, ButtonClass = "btn-primary pull-left", ActionRedirect = "Home" });
                        
                        break;
                    case DB.SMRequestLineStatusEnum.Canceled:
                        break;
                    default:
                        break;
                }
                results.Add(new WF.WorkFlowAction() { Title = "Cancel", GotoStatusId = (int)DB.SMRequestLineStatusEnum.Canceled, ButtonClass = "btn-danger pull-right", ActionRedirect = "Home" });

            }
            else
            {
            }
            return results;
        }
        
        public Forums.ForumLineListViewModel Forum { get; set; }
    }
}
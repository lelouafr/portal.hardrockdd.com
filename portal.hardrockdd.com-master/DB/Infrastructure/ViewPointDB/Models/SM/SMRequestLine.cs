using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class SMRequestLine: IWorkFlow, IForum, IEquipment
    {
        public static string BaseTableName { get { return "budSMRL"; } }

        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db ??= this.Request.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                    return HQAttachment;

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = this.SMCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.RequestId,
                    TableName = BaseTableName,
                    HQCompanyParm = this.Request.HQCompanyParm,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return HQAttachment;
            }
        }

        public SMServiceStatusIdEnum ServiceStatus
        {
            get
            {
                return (SMServiceStatusIdEnum)(ServiceStatusId ?? (int)SMServiceStatusIdEnum.Pending);
            }
            set
            {
                if ((int)value != ServiceStatusId)
                {
                    ServiceStatusId = (int)(value);
                }
            }
        }
        
        public SMRequestLineStatusEnum Status
        {
            get
            {
                return (SMRequestLineStatusEnum)StatusId;
            }
            set
            {
                if ((int)value != StatusId)
                {
                    StatusId = (int)(value);
                    UpdateStatus();
                    AddWorkFlowAssignments();
                }
            }
        }

        public EMMeterTypeEnum EMMeterType { get => (EMMeterTypeEnum)(EMMeterTypeId ?? 0); set => EMMeterTypeId = (byte)value; }

        public string EquipmentId { get => tEquipmentId; set => UpdateEquipment(value); }

        private void UpdateStatus()
        {
            WorkFlow.CreateSequence(StatusId);
            var db = VPContext.GetDbContextFromEntity(this);
            switch (Status)
            {
                case SMRequestLineStatusEnum.Draft:
                    break;
                case SMRequestLineStatusEnum.Pending:
                    break;
                case SMRequestLineStatusEnum.Assigned:
                    break;
                case SMRequestLineStatusEnum.InProcess:
                    break;
                case SMRequestLineStatusEnum.Completed:
                    break;
                case SMRequestLineStatusEnum.Canceled:
                    if (WorkorderItem != null)
                    {
                        WorkorderItem.StatusCodeId = WorkorderItem.WorkOrder.EMCompany.WOStatusCodes.FirstOrDefault(f => f.Description == "Canceled").StatusCodeId;
                    }
                    break;
                case SMRequestLineStatusEnum.WorkOrderCreated:
                    if (WorkorderItem == null)
                    {
                        WorkorderItem = EMWorkOrderItem.Create(db, this);
                    }
                    break;
                case SMRequestLineStatusEnum.WorkOrderInProcess:
                    break;
                case SMRequestLineStatusEnum.WorkOrderCompleted:
                    break;
                case SMRequestLineStatusEnum.WorkOrderCanceled:
                    break;
                default:
                    break;
            }
        }

        public WorkFlow GetWorkFlow()
        {
            var workFlow = WorkFlow;
            if (workFlow == null)
            {
                workFlow = AddWorkFlow();

            }
            if (workFlow.CurrentSequence() == null)
            {
                workFlow.AddSequence(this.StatusId);
                this.AddWorkFlowAssignments();
                db.BulkSaveChanges();
            }
            return workFlow;
        }

        public WorkFlow AddWorkFlow()
        {
            var workflow = WorkFlow;
            if (workflow == null)
            {
                workflow = new WorkFlow
                {
                    WFCo = SMCo,
                    WorkFlowId = db.GetNextId(WorkFlow.BaseTableName, 1),
                    TableName = BaseTableName,
                    Id = RequestId,
                    LineId = LineId,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,

                    db = this.db,
                    Company = Request.HQCompanyParm,
                };

                db.WorkFlows.Add(workflow);
                WorkFlow = workflow;
                WorkFlowId = workflow.WorkFlowId;
                workflow.AddSequence(this.StatusId);
                this.AddWorkFlowAssignments();

                WorkFlow = workflow;
            }

            return workflow;
        }

        private void GenerateWorkFlowForEquipment()
        {
            if (Request.RequestType == SMRequestTypeEnum.Equipment)
            {

                switch (Status)
                {
                    case SMRequestLineStatusEnum.Draft:
                        WorkFlow.AddUser(Request.RequestUser);
                        break;
                    case SMRequestLineStatusEnum.Pending:
                        WorkFlow.AddUsersByPosition("SHP-MGR");
                        WorkFlow.AddUsersByPosition("OP-DM");
                        break;
                    case SMRequestLineStatusEnum.Assigned:
                        //WorkFlow.GenerateUsersByPosition("SHP-MGR");
                        WorkFlow.AddEmployee(AssignedEmployee.Supervisor);
                        WorkFlow.AddEmployee(AssignedEmployee);
                        break;
                    case SMRequestLineStatusEnum.InProcess:
                        //WorkFlow.GenerateUsersByPosition("SHP-MGR");
                        WorkFlow.AddEmployee(AssignedEmployee.Supervisor);
                        WorkFlow.AddEmployee(AssignedEmployee);
                        break;
                    case SMRequestLineStatusEnum.Completed:
                        WorkFlow.CompleteWorkFlow();
                        break;
                    case SMRequestLineStatusEnum.Canceled:
                        WorkFlow?.CompleteWorkFlow();
                        break;
                    case SMRequestLineStatusEnum.WorkOrderCreated:
                        WorkFlow.AddEmployee(AssignedEmployee, true);
                        WorkFlow.AddUsersByPosition("SHP-MGR");
                        WorkFlow.AddUsersByPosition("OP-DM");
                        break;
                    case SMRequestLineStatusEnum.WorkOrderInProcess:
                        break;
                    case SMRequestLineStatusEnum.WorkOrderCompleted:
                        WorkFlow.CompleteWorkFlow(WorkorderItem.Mechanic);
                        break;
                    case SMRequestLineStatusEnum.WorkOrderCanceled:
                        WorkFlow?.CompleteWorkFlow();
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddWorkFlowAssignments(bool reset = false)
        {
            if (WorkFlow == null)
                AddWorkFlow();
            if (reset)
            {
                var delList = WorkFlow.CurrentSequence().AssignedUsers.ToList();
                delList.ForEach(del => WorkFlow.CurrentSequence().AssignedUsers.Remove(del));
            }

            switch (Request.RequestType)
            {
                case SMRequestTypeEnum.Equipment:
                    GenerateWorkFlowForEquipment();
                    break;
                default:
                    break;
            }
        }

        public void EmailStatusUpdate(Controller controller)
        {
            //var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(Request);
            //var viewPath = "";
            //var subject = "";
            //using var db = new VPContext();

            //viewPath = "../SM/Request/Email/EmailSubmit";
            //switch (Request.Status)
            //{
            //    case SMRequestStatusEnum.Draft:
            //        subject = string.Format("Equipment Request hase been created by: {0} ", Request.RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Submitted:
            //        subject = string.Format("Equipment Request hase been Submitted from: {0} ", Request.RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Completed:
            //        subject = string.Format("Equipment Request hase been Completed by: {0} ", Request.RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Canceled:
            //        subject = string.Format("Equipment Request hase been Canceled by: {0} ", Request.RequestUser.FullName());
            //        break;
            //    default:
            //        break;
            //}

            //if (!string.IsNullOrEmpty(viewPath))
            //{
            //    try
            //    {
            //        using MailMessage msg = new MailMessage()
            //        {
            //            Body = Services.EmailHelper.RenderViewToString(controller.ControllerContext, viewPath, result, false),
            //            IsBodyHtml = true,
            //            Subject = subject,
            //        };

            //        foreach (var workFlow in Request.WorkFlow.CurrentSequence().AssignedUsers.ToList())
            //        {
            //            var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
            //            msg.To.Add(new MailAddress(user.Email));
            //        }

            //        Services.EmailHelper.Send(msg);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(ex.Message);
            //    }
            //}

        }

        public Forum GetForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = AddForum();
            }
            return forum;
        }

        public Forum AddForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = new Forum()
                {
                    Co = this.SMCo,
                    ForumId = db.GetNextId(Forum.BaseTableName),
                    RelKeyID = this.RequestId,
                    TableName = BaseTableName,

                    db = this.db
                };

                this.Forum = forum;
                db.BulkSaveChanges();
            }

            return forum;
        }

        public void UpdateEquipment(string value)
        {
            if (Equipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                if (equipment != null)
                {
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    Equipment = equipment;
                    EMMeterType = equipment.MeterType;
                }
            }

            if (tEquipmentId != value)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == value);
                if (equipment != null)
                {
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    Equipment = equipment;
                    EMMeterType = equipment.MeterType;
                }
                else
                {
                    tEquipmentId = null;
                    EMCo = null;
                    Equipment = null;
                }
            }
        }
    }
}
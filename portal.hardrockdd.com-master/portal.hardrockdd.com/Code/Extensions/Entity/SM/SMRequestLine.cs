using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace portal.Code.Data.VP
{
    public partial class SMRequestLine: IWorkFlow, IForum, IEquipment
    {
        public string BaseTableName { get { return "budSMRL"; } }

        private VPEntities _db;

        public VPEntities db
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
                    _db ??= VPEntities.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        private HQAttachment _Attachment;
        public HQAttachment Attachment
        {
            get
            {
                if (_Attachment == null)
                {
                    if (HQAttachment != null)
                    {
                        _Attachment = HQAttachment;
                        return _Attachment;
                    }

                    var db = VPEntities.GetDbContextFromEntity(this);
                    if (db == null)
                    {
                        db = new VPEntities();
                    }
                    _Attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == this.UniqueAttchID);

                    if (_Attachment == null)
                    {
                        _Attachment = HQAttachment.Init(this.SMCo, this.BaseTableName, 0);
                        db.HQAttachments.Add(_Attachment);
                        this.UniqueAttchID = _Attachment.UniqueAttchID;
                        db.BulkSaveChanges();
                    }

                    if (!_Attachment.Folders.Any())
                    {
                        _Attachment.BuildDefaultFolders();
                        db.BulkSaveChanges();
                    }
                }

                return _Attachment;
            }
        }

        public DB.SMRequestLineStatusEnum Status
        {
            get
            {
                return (DB.SMRequestLineStatusEnum)StatusId;
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

        public string EquipmentId { get => tEquipmentId; set => UpdateEquipment(value); }

        private void UpdateStatus()
        {
            WorkFlow.CreateSequance(StatusId);
            var db = VPEntities.GetDbContextFromEntity(this);
            switch (Status)
            {
                case DB.SMRequestLineStatusEnum.Draft:
                    break;
                case DB.SMRequestLineStatusEnum.Pending:
                    break;
                case DB.SMRequestLineStatusEnum.Assigned:
                    break;
                case DB.SMRequestLineStatusEnum.InProcess:
                    break;
                case DB.SMRequestLineStatusEnum.Completed:
                    break;
                case DB.SMRequestLineStatusEnum.Canceled:
                    if (WorkorderItem != null)
                    {
                        WorkorderItem.StatusCodeId = WorkorderItem.WorkOrder.EMCompany.WOStatusCodes.FirstOrDefault(f => f.Description == "Canceled").StatusCodeId;
                    }
                    break;
                case DB.SMRequestLineStatusEnum.WorkOrderCreated:
                    if (WorkorderItem == null)
                    {
                        WorkorderItem = EMWorkOrderItem.Create(db, this);
                    }
                    break;
                case DB.SMRequestLineStatusEnum.WorkOrderInProcess:
                    break;
                case DB.SMRequestLineStatusEnum.WorkOrderCompleted:
                    break;
                case DB.SMRequestLineStatusEnum.WorkOrderCanceled:
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
            if (workFlow.CurrentSequance() == null)
            {
                workFlow.AddSequance(this.StatusId);
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
                workflow.AddSequance(this.StatusId);
                this.AddWorkFlowAssignments();

                WorkFlow = workflow;
            }

            return workflow;
        }

        private void GenerateWorkFlowForEquipment()
        {
            if (Request.RequestType == DB.SMRequestTypeEnum.Equipment)
            {

                switch (Status)
                {
                    case DB.SMRequestLineStatusEnum.Draft:
                        WorkFlow.AddUser(Request.RequestUser);
                        break;
                    case DB.SMRequestLineStatusEnum.Pending:
                        WorkFlow.AddUsersByPosition("SHP-MGR");
                        WorkFlow.AddUsersByPosition("OP-DM");
                        break;
                    case DB.SMRequestLineStatusEnum.Assigned:
                        //WorkFlow.GenerateUsersByPosition("SHP-MGR");
                        WorkFlow.AddEmployee(AssignedEmployee.Supervisor);
                        WorkFlow.AddEmployee(AssignedEmployee);
                        break;
                    case DB.SMRequestLineStatusEnum.InProcess:
                        //WorkFlow.GenerateUsersByPosition("SHP-MGR");
                        WorkFlow.AddEmployee(AssignedEmployee.Supervisor);
                        WorkFlow.AddEmployee(AssignedEmployee);
                        break;
                    case DB.SMRequestLineStatusEnum.Completed:
                        WorkFlow.CompleteWorkFlow();
                        break;
                    case DB.SMRequestLineStatusEnum.Canceled:
                        WorkFlow?.CompleteWorkFlow();
                        break;
                    case DB.SMRequestLineStatusEnum.WorkOrderCreated:
                        WorkFlow.AddEmployee(AssignedEmployee, true);
                        WorkFlow.AddUsersByPosition("SHP-MGR");
                        WorkFlow.AddUsersByPosition("OP-DM");
                        break;
                    case DB.SMRequestLineStatusEnum.WorkOrderInProcess:
                        break;
                    case DB.SMRequestLineStatusEnum.WorkOrderCompleted:
                        WorkFlow.CompleteWorkFlow(WorkorderItem.Mechanic);
                        break;
                    case DB.SMRequestLineStatusEnum.WorkOrderCanceled:
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
                var delList = WorkFlow.CurrentSequance().AssignedUsers.ToList();
                delList.ForEach(del => WorkFlow.CurrentSequance().AssignedUsers.Remove(del));
            }

            switch (Request.RequestType)
            {
                case DB.SMRequestTypeEnum.Equipment:
                    GenerateWorkFlowForEquipment();
                    break;
                default:
                    break;
            }
        }

        public void EmailStatusUpdate(Controller controller)
        {
            var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(Request);
            var viewPath = "";
            var subject = "";
            using var db = new VPEntities();

            viewPath = "../SM/Request/Email/EmailSubmit";
            switch (Request.Status)
            {
                case DB.SMRequestStatusEnum.Draft:
                    subject = string.Format("Equipment Request hase been created by: {0} ", Request.RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Submitted:
                    subject = string.Format("Equipment Request hase been Submitted from: {0} ", Request.RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Completed:
                    subject = string.Format("Equipment Request hase been Completed by: {0} ", Request.RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Canceled:
                    subject = string.Format("Equipment Request hase been Canceled by: {0} ", Request.RequestUser.FullName());
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using MailMessage msg = new MailMessage()
                    {
                        Body = Code.EmailHelper.RenderViewToString(controller.ControllerContext, viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in Request.WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    {
                        var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                        msg.To.Add(new MailAddress(user.Email));
                    }

                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

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
                    TableName = this.BaseTableName,

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
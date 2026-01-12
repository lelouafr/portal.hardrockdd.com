using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HRTermRequest
    {
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
                    _db ??= this.HRCompanyParm?.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budHRTR"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment == null && UniqueAttchID != null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.HRCo,
                        UniqueAttchID = (Guid)UniqueAttchID,
                        TableKeyId = this.RequestId,
                        TableName = HRTermRequest.BaseTableName,
                        HQCompanyParm = this.HRCompanyParm.HQCompanyParm,
                    };
                    db.BulkSaveChanges();
                }
                else if (HQAttachment == null && UniqueAttchID == null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.HRCo,
                        UniqueAttchID = Guid.NewGuid(),
                        TableKeyId = this.RequestId,
                        TableName = HRTermRequest.BaseTableName,
                        HQCompanyParm = this.HRCompanyParm.HQCompanyParm,
                    };
                    this.UniqueAttchID = HQAttachment.UniqueAttchID;
                    db.BulkSaveChanges();
                }

                return HQAttachment;
            }
        }

        public string StatusComments { get; set; }

        public int StatusId
        {
            get
            {
                return tStatusId;
            }
            set
            {
                if (tStatusId != value)
                {
                    UpdateStatusChange(value);
                }
            }
        }

        public int? HRRef
        {
            get
            {
                return tHRRef;
            }
            set
            {
                if (value != tHRRef)
                    UpdateEmployee(value);
            }
        }

        public bool IsOpenRequest
        {
            get
            {
                return tIsOpenRequest ?? false;
            }
            set
            {
                if (value != tIsOpenRequest)
                {
                    UpdateOpenPosition(value);
                }
            }
        }

        public int? WPDivisionId
        {
            get
            {
                return this.tWPDivisionId;
            }
            set
            {
                if (tWPDivisionId != value)
                {
                    if (PositionRequest != null)
                        PositionRequest.tWPDivisionId = value;

                    tWPDivisionId = value;
                }
            }
        }

        private void UpdateEmployee(int? value)
        {
            if (tHRRef != null & HRResource == null)
            {
                var resource = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == tHRRef);
                if (resource != null)
                    this.HRResource = resource;
                else
                    tHRRef = null;
            }

            if (tHRRef != value)
            {
                var resource = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == value);
                if (resource != null)
                    this.HRResource = resource;
                else
                {
                    tHRRef = null;
                    this.HRResource = null;
                    value = null;
                }

                tHRRef = value;
            }

            if (PositionRequest != null)
                PositionRequest.PriorEmployeeId = this.HRRef;
        }

        private void UpdateOpenPosition(bool value)
        {
            if (value)
            {
                if (this.PositionRequestId == null)
                {
                    var posRequest = this.HRCompanyParm.AddPositionRequest();
                    posRequest.WPDivisionId = this.WPDivisionId;
                    posRequest.PositionCodeId = this.HRResource.PositionCode;
                    posRequest.ForCrewId = this.HRResource.PREmployee.CrewId;
                    posRequest.PriorEmployeeId = this.HRRef;
                    posRequest.RequestedBy = this.RequestedBy;
                    posRequest.RequestedUser = this.RequestedUser;
                    posRequest.Comments = this.Comments;
                    this.PositionRequestId = posRequest.RequestId;
                    this.PositionHRCo = posRequest.HRCo;
                    this.PositionRequest = posRequest;
                }
                if (this.PositionRequestId != null)
                    this.PositionRequest.UpdateStatusFromTermRequest(this);
            }
            else
            {
                if (this.PositionRequestId != null)
                {
                    this.PositionRequest.Status = HRPositionRequestStatusEnum.Canceled;
                }
            }
            tIsOpenRequest = value;
        }

        public HRTermRequestStatusEnum Status
        {
            get
            {
                return (HRTermRequestStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == HRTermRequestStatusEnum.New)
                    UpdateStatusChange((int)value);
                
            }
        }
        
        public void UpdateStatusChange(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var status = (HRTermRequestStatusEnum)newValue;
            if (newValue != StatusId || status == HRTermRequestStatusEnum.New)
            {
                switch (status)
                {
                    case HRTermRequestStatusEnum.New:
                        break;
                    case HRTermRequestStatusEnum.Submitted:

                        break;
                    case HRTermRequestStatusEnum.Approved:
                        break;
                    case HRTermRequestStatusEnum.PRReview:
                        break;
                    case HRTermRequestStatusEnum.HRReview:
                        break;
                    case HRTermRequestStatusEnum.Completed:
                        break;
                    case HRTermRequestStatusEnum.Canceled:
                        break;
                    default:
                        break;
                }
                tStatusId = newValue;
                WorkFlow.CreateSequence((int)newValue);
                WorkFlow.CurrentSequence().Comments = StatusComments;
                GenerateWorkFlowAssignments();
                if (this.PositionRequestId != null)
                    this.PositionRequest.UpdateStatusFromTermRequest(this);
            }
        }

        #region Workflow
        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = HRCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(HRCo),
                    TableName = HRTermRequest.BaseTableName,
                    Id = RequestId,
                    CreatedBy = RequestedBy,
                    CreatedOn = RequestedDate,
                    Active = true,

                    Company = HRCompanyParm.HQCompanyParm,
                };
                //HRCompanyParm.HQCompanyParm.WorkFlows.Add(workflow);
                db.WorkFlows.Add(workflow);
                WorkFlow = workflow;
                WorkFlowId = workflow.WorkFlowId;
            }
        }

        public void GenerateWorkFlowAssignments(bool reset = false)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();
            if (reset)
            {
                var assignedusers = WorkFlow.CurrentSequence().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequence().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case HRTermRequestStatusEnum.New:
                    WorkFlow.AddUser(RequestedBy);
                    break;
                case HRTermRequestStatusEnum.Submitted:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    EmailStatusUpdate();
                    break;
                case HRTermRequestStatusEnum.Approved:

                    break;
                case HRTermRequestStatusEnum.PRReview:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case HRTermRequestStatusEnum.HRReview:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case HRTermRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case HRTermRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }
        #endregion

        public void EmailStatusUpdate()
        {
            if (HttpContext.Current == null)
                return;

            var result = new DB.Infrastructure.EmailModels.TermRequestEmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPContext();
            switch (Status)
            {
                case HRTermRequestStatusEnum.New:
                    break;
                case HRTermRequestStatusEnum.Submitted:
                    viewPath = "Email/Submit";
                    subject = string.Format("Employee Termination Requested By: {0} For: {1} ", result.RequestedUser, result.EmployeeName);
                    break;
                case HRTermRequestStatusEnum.Approved:
                    break;
                case HRTermRequestStatusEnum.PRReview:
                    break;
                case HRTermRequestStatusEnum.HRReview:
                    break;
                case HRTermRequestStatusEnum.Completed:
                    break;
                case HRTermRequestStatusEnum.Canceled:
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                    {
                        Body = Services.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequence().AssignedUsers.ToList())
                    {
                        var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                        msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
                    }

                    if (RequestedUser != null && RequestedUser?.Email != db.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(db.GetCurrentUser().Email));
                    }
                    Services.EmailHelper.Send(msg);
                }
                catch (Exception)
                {
                    //throw new Exception(ex.Message);
                }
            }
        }
    }
}
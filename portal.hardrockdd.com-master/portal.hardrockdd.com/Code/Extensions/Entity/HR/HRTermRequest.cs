using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class HRTermRequest
    {
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
                    _db ??= this.HRCompanyParm?.db;
                    _db ??= VPEntities.GetDbContextFromEntity(this);

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
                    this.PositionRequest.Status = DB.HRPositionRequestStatusEnum.Canceled;
                }
            }
            tIsOpenRequest = value;
        }

        public DB.HRTermRequestStatusEnum Status
        {
            get
            {
                return (DB.HRTermRequestStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == DB.HRTermRequestStatusEnum.New)
                    UpdateStatusChange((int)value);
                
            }
        }
        
        public void UpdateStatusChange(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var status = (DB.HRTermRequestStatusEnum)newValue;
            if (newValue != StatusId || status == DB.HRTermRequestStatusEnum.New)
            {
                switch (status)
                {
                    case DB.HRTermRequestStatusEnum.New:
                        break;
                    case DB.HRTermRequestStatusEnum.Submitted:

                        break;
                    case DB.HRTermRequestStatusEnum.Approved:
                        break;
                    case DB.HRTermRequestStatusEnum.PRReview:
                        break;
                    case DB.HRTermRequestStatusEnum.HRReview:
                        break;
                    case DB.HRTermRequestStatusEnum.Completed:
                        break;
                    case DB.HRTermRequestStatusEnum.Canceled:
                        break;
                    default:
                        break;
                }
                tStatusId = newValue;
                WorkFlow.CreateSequance((int)newValue);
                WorkFlow.CurrentSequance().Comments = StatusComments;
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
                HRCompanyParm.HQCompanyParm.WorkFlows.Add(workflow);
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
                var assignedusers = WorkFlow.CurrentSequance().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequance().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case DB.HRTermRequestStatusEnum.New:
                    WorkFlow.AddUser(RequestedBy);
                    break;
                case DB.HRTermRequestStatusEnum.Submitted:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    EmailStatusUpdate();
                    break;
                case DB.HRTermRequestStatusEnum.Approved:

                    break;
                case DB.HRTermRequestStatusEnum.PRReview:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case DB.HRTermRequestStatusEnum.HRReview:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case DB.HRTermRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.HRTermRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }
        #endregion

        public void EmailStatusUpdate()
        {
            //if (HttpContext.Current == null)
            //    return;

            //var result = new portal.Areas.HumanResource.Models.TermRequest.TermRequestEmailViewModel(this);
            //var viewPath = "";
            //var subject = "";
            //using var db = new VPEntities();
            //switch (Status)
            //{
            //    case DB.HRTermRequestStatusEnum.New:
            //        break;
            //    case DB.HRTermRequestStatusEnum.Submitted:
            //        viewPath = "../HR/Request/Term/Email/EmailSubmit";
            //        subject = string.Format(AppCultureInfo.CInfo(), "Employee Termination Requested By: {0} For: {1} ", result.RequestedUser, result.EmployeeName);
            //        break;
            //    case DB.HRTermRequestStatusEnum.Approved:
            //        break;
            //    case DB.HRTermRequestStatusEnum.PRReview:
            //        break;
            //    case DB.HRTermRequestStatusEnum.HRReview:
            //        break;
            //    case DB.HRTermRequestStatusEnum.Completed:
            //        break;
            //    case DB.HRTermRequestStatusEnum.Canceled:
            //        break;
            //    default:
            //        break;
            //}

            //if (!string.IsNullOrEmpty(viewPath))
            //{
            //    try
            //    {
            //        using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
            //        {
            //            Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
            //            IsBodyHtml = true,
            //            Subject = subject,
            //        };

            //        foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
            //        {
            //            var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
            //            msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
            //        }

            //        if (RequestedUser != null && RequestedUser?.Email != StaticFunctions.GetCurrentUser().Email)
            //        {
            //            msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
            //        }
            //        Code.EmailHelper.Send(msg);
            //    }
            //    catch (Exception)
            //    {
            //        //throw new Exception(ex.Message);
            //    }
            //}
        }
    }
}
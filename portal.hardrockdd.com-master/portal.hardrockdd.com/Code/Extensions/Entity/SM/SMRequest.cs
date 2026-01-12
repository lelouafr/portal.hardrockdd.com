using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace portal.Code.Data.VP
{
    public partial class SMRequest
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
                    _db ??= HQCompanyParm.db;
                    _db ??= VPEntities.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }


        public DB.SMRequestTypeEnum RequestType
        {
            get
            {
                return (DB.SMRequestTypeEnum)RequestTypeId;
            }
            set
            {
                RequestTypeId = (int)(value);
            }
        }
        
        public DB.SMRequestStatusEnum Status
        {
            get
            {
                return (DB.SMRequestStatusEnum)StatusId;
            }
            set
            {
                if ((int)value != StatusId)
                {
                    StatusId = (int)(value);
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            WorkFlow.CreateSequance(StatusId);
            GenerateWorkFlowAssignments();
            switch (Status)
            {
                case DB.SMRequestStatusEnum.Draft:
                    Lines.ToList().ForEach(e => e.Status = DB.SMRequestLineStatusEnum.Draft);
                    break;
                case DB.SMRequestStatusEnum.Submitted:
                    this.RequestDate = DateTime.Now;
                    Lines.ToList().ForEach(e => e.Status = DB.SMRequestLineStatusEnum.Pending);
                    EmailStatusUpdate();
                    break;
                case DB.SMRequestStatusEnum.Completed:
                    break;
                case DB.SMRequestStatusEnum.Canceled:
                    Lines.ToList().ForEach(e => e.Status = DB.SMRequestLineStatusEnum.Canceled);
                    break;
                default:
                    break;
            }
        }

        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = SMCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(SMCo),// HQCompanyParm.WorkFlows.DefaultIfEmpty().Max(f => f == null ? 0 : f.WorkFlowId) + 1,
                    TableName = "budSMRH",
                    Id = RequestId,
                    CreatedBy = StaticFunctions.GetUserId(),
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = HQCompanyParm,
                };

                HQCompanyParm.WorkFlows.Add(workflow);
                WorkFlow = workflow;
            }
        }

        private void GenerateWorkFlowForEquipment()
        {
            switch (Status)
            {
                case DB.SMRequestStatusEnum.Draft:
                    WorkFlow.AddUser(RequestUser);
                    break;
                case DB.SMRequestStatusEnum.Submitted:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.SMRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.SMRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }

        public void GenerateWorkFlowAssignments()
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            switch (RequestType)
            {
                case DB.SMRequestTypeEnum.Equipment:
                    GenerateWorkFlowForEquipment();
                    break;
                default:
                    break;
            }
        }

        public void EmailStatusUpdate()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPEntities();

            viewPath = "../SM/Request/Email/EmailSubmit";
            switch (Status)
            {
                case DB.SMRequestStatusEnum.Draft:
                    subject = string.Format("Equipment Request has been created by: {0} ", RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Submitted:
                    subject = string.Format("Equipment Request has been Submitted from: {0} ", RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Completed:
                    subject = string.Format("Equipment Request has been Completed by: {0} ", RequestUser.FullName());
                    break;
                case DB.SMRequestStatusEnum.Canceled:
                    subject = string.Format("Equipment Request has been Canceled by: {0} ", RequestUser.FullName());
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
                        Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    {
                        var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                        msg.To.Add(new MailAddress(user.Email));
                    }
                    msg.CC.Add(new MailAddress(this.RequestUser.Email));

                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

        }
    }
}
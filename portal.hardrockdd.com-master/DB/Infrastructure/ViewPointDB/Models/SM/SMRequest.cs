using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class SMRequest
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
                    _db ??= HQCompanyParm.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }


        public SMRequestTypeEnum RequestType
        {
            get
            {
                return (SMRequestTypeEnum)RequestTypeId;
            }
            set
            {
                RequestTypeId = (int)(value);
            }
        }
        
        public SMRequestStatusEnum Status
        {
            get
            {
                return (SMRequestStatusEnum)StatusId;
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
            WorkFlow.CreateSequence(StatusId);
            GenerateWorkFlowAssignments();
            switch (Status)
            {
                case SMRequestStatusEnum.Draft:
                    Lines.ToList().ForEach(e => e.Status = SMRequestLineStatusEnum.Draft);
                    break;
                case SMRequestStatusEnum.Submitted:
                    this.RequestDate = DateTime.Now;
                    Lines.ToList().ForEach(e => e.Status = SMRequestLineStatusEnum.Pending);
                    EmailStatusUpdate();
                    break;
                case SMRequestStatusEnum.Completed:
                    break;
                case SMRequestStatusEnum.Canceled:
                    Lines.ToList().ForEach(e => e.Status = SMRequestLineStatusEnum.Canceled);
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
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = HQCompanyParm,
                };

                //HQCompanyParm.WorkFlows.Add(workflow);
                db.WorkFlows.Add(workflow);
                WorkFlow = workflow;
            }
        }

        private void GenerateWorkFlowForEquipment()
        {
            switch (Status)
            {
                case SMRequestStatusEnum.Draft:
                    WorkFlow.AddUser(RequestUser);
                    break;
                case SMRequestStatusEnum.Submitted:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case SMRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case SMRequestStatusEnum.Canceled:
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
                case SMRequestTypeEnum.Equipment:
                    GenerateWorkFlowForEquipment();
                    break;
                default:
                    break;
            }
        }

        public void EmailStatusUpdate()
        {
            //if (HttpContext.Current == null)
            //{
            //    return;
            //}
            //var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(this);
            //var viewPath = "";
            //var subject = "";
            //using var db = new VPContext();

            //viewPath = "../SM/Request/Email/EmailSubmit";
            //switch (Status)
            //{
            //    case SMRequestStatusEnum.Draft:
            //        subject = string.Format("Equipment Request has been created by: {0} ", RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Submitted:
            //        subject = string.Format("Equipment Request has been Submitted from: {0} ", RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Completed:
            //        subject = string.Format("Equipment Request has been Completed by: {0} ", RequestUser.FullName());
            //        break;
            //    case SMRequestStatusEnum.Canceled:
            //        subject = string.Format("Equipment Request has been Canceled by: {0} ", RequestUser.FullName());
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
            //            Body = Services.EmailHelper.RenderViewToString(viewPath, result, false),
            //            IsBodyHtml = true,
            //            Subject = subject,
            //        };

            //        foreach (var workFlow in WorkFlow.CurrentSequence().AssignedUsers.ToList())
            //        {
            //            var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
            //            msg.To.Add(new MailAddress(user.Email));
            //        }
            //        msg.CC.Add(new MailAddress(this.RequestUser.Email));

            //        Services.EmailHelper.Send(msg);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(ex.Message);
            //    }
            //}

        }
    }
}
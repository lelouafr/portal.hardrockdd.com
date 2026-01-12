using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class LeaveRequest
    {
        public static string BaseTableName { get { return "budHRLR"; } }
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
                    _db ??= this.Company?.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        public LeaveRequestStatusLog AddStatus()
        {
            var statusLog = new LeaveRequestStatusLog
            {
                PRCo = PRCo,
                RequestId = RequestId,
                LineNum = StatusLogs.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                Status = (short?)Status,
                CreatedOn = DateTime.Now,
                CreatedBy = db.CurrentUserId
            };

            return statusLog;
        }


        public void GenerateWorkFlow()
        {
            var emp = db.GetCurrentHREmployee();
            var timeStamp = DateTime.Now;
            if (EmployeeId != emp.HRRef)
            {
                emp = Employee.Resource.FirstOrDefault();
            }

            foreach (var workFlow in WorkFlows.Where(f => f.Active == "Y").ToList())
            {
                workFlow.Active = "N";
                if (workFlow.AssignedTo == db.CurrentUserId)
                {
                    workFlow.CompletedOn = timeStamp;
                    workFlow.AssignedStatus = Status > workFlow.Status ? 2 : 1;
                }
            }
            
            List<WebUser> EmailList = new List<WebUser>();
            WebUser user;
            switch ((LeaveRequestStatusEnum)Status)
            {
                case LeaveRequestStatusEnum.Open:
                    EmailList.Add(db.GetCurrentUser());
                    break;
                case LeaveRequestStatusEnum.Submitted:
                    EmailList.Add(emp.Supervisor.WebUser);

                    user = db.WebUsers.FirstOrDefault(user => user.Email.ToLower() == ("donna.kurz@hardrockdd.com").ToLower());
                    EmailList.Add(user);

                    break;
                case LeaveRequestStatusEnum.Approved:
                    user = db.WebUsers.FirstOrDefault(user => user.Email.ToLower() == ("donna.kurz@hardrockdd.com").ToLower());
                    EmailList.Add(user);

                    break;
                case LeaveRequestStatusEnum.Rejected:
                    EmailList.Add(CreatedUser);
                    break;
                case LeaveRequestStatusEnum.Processed:
                    break;
                case LeaveRequestStatusEnum.Canceled:
                    break;
                default:
                    break;
            }

            foreach (var email in EmailList)
            {
                var workFlow = new LeaveRequestWorkFlow
                {
                    PRCo = PRCo,
                    RequestId = RequestId,
                    LineId = WorkFlows
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineId) + 1,
                    Status = (int)Status,
                    StatusDate = timeStamp,
                    AssignedTo = email.Id,
                    AssignedOn = timeStamp,
                    AssignedStatus = 0,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = timeStamp,
                    Active = "Y"
                };
                WorkFlows.Add(workFlow);
            }
        }
    }
}
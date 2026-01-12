using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestWorkFlowRepository : IDisposable
    {
        public static void GenerateWorkFlow(LeaveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            using var empRepo = new WP.WebUserRepository();
            var curUser = empRepo.GetUser(StaticFunctions.GetUserId());
            var emp = curUser.Employee.FirstOrDefault();
            var timeStamp = DateTime.Now;
            if (request.EmployeeId != emp.HRRef)
            {
                emp = request.Employee.Resource.FirstOrDefault();
            }

            foreach (var workFlow in request.WorkFlows.Where(f => f.Active == "Y").ToList())
            {
                workFlow.Active = "N";
                if (workFlow.AssignedTo == curUser.Id)
                {
                    workFlow.CompletedOn = timeStamp;
                    workFlow.AssignedStatus = request.Status > workFlow.Status ? 2 : 1;
                }
            }
           
            List<WebUser> EmailList = new List<WebUser>();
            WebUser user;
            switch ((DB.LeaveRequestStatusEnum)request.Status)
            {
                case DB.LeaveRequestStatusEnum.Open:
                    EmailList.Add(curUser);
                    break;
                case DB.LeaveRequestStatusEnum.Submitted:
                    EmailList.Add(emp.Supervisor.WebUser);

                    user = empRepo.GetUserbyEmail("donna.kurz@hardrockdd.com");
                    EmailList.Add(user);

                    break;
                case DB.LeaveRequestStatusEnum.Approved:
                    user = empRepo.GetUserbyEmail("donna.kurz@hardrockdd.com");
                    EmailList.Add(user);

                    break;
                case DB.LeaveRequestStatusEnum.Rejected:
                    EmailList.Add(request.CreatedUser);
                    break;
                case DB.LeaveRequestStatusEnum.Processed:
                    break;
                case DB.LeaveRequestStatusEnum.Canceled:
                    break;
                default:
                    break;
            }

            foreach (var email in EmailList)
            {
                var workFlow = new LeaveRequestWorkFlow
                {
                    PRCo = request.PRCo,
                    RequestId = request.RequestId,
                    LineId = request.WorkFlows
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineId) + 1,
                    Status = (int)request.Status,
                    StatusDate = timeStamp,
                    AssignedTo = email.Id,
                    AssignedOn = timeStamp,
                    AssignedStatus = 0,
                    CreatedBy = curUser.Id,
                    CreatedOn = timeStamp,
                    Active = "Y"
                };
                request.WorkFlows.Add(workFlow);
            }
        }
    }
}

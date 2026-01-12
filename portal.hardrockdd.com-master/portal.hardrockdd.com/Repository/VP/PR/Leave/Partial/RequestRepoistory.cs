using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Leave;
using portal.Repository.VP.DT;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestLineRepository : IDisposable
    {

        public static LeaveRequestViewModel ProcessUpdate(LeaveRequestViewModel model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.LeaveRequests.FirstOrDefault(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId);
            if (updObj != null)
            {
                var empChange = updObj.EmployeeId != model.EmployeeId;

                /****Write the changes to object****/
                updObj.EmployeeId = model.EmployeeId;
                updObj.Comments = model.Comments;
                if (empChange)
                {
                    foreach (var item in updObj.Lines)
                    {
                        item.EmployeeId = model.EmployeeId;
                    }
                }
                
                db.SaveChanges(modelState);
            }
            return new LeaveRequestViewModel(updObj);
        }

        public static void CreateTickets(int WeekId, VPContext db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            var list = db.LeaveRequestLines.Where(f => f.Calendar.Week == WeekId && f.TicketId == null && f.Request.Status == 2).ToList();
            foreach (var emp in list.GroupBy(g => g.Employee).Select(s => new { Employee = s.Key, List = s }).ToList())
            {
                var request = emp.List.FirstOrDefault().Request;
                var ticket = emp.Employee.PRCompanyParm.HQCompanyParm.AddDailyTicket((DateTime)emp.List.FirstOrDefault().WorkDate, DB.DTFormEnum.TimeOff);
                ticket.CreatedBy = request.CreatedBy;
                ticket.CreatedOn = request.CreatedOn;
                ticket.CreatedUser = request.CreatedUser;
                ticket.DailyTicketStatusLogs.ToList().ForEach(e => {
                    var status = (DB.DailyTicketStatusEnum)e.Status;
                    switch (status)
                    {
                        case DB.DailyTicketStatusEnum.Draft:
                            e.CreatedBy = request.CreatedBy;
                            e.CreatedOn = request.CreatedOn;
                            break;
                        case DB.DailyTicketStatusEnum.Submitted:
                            var subLog = request.StatusLogs.FirstOrDefault(f => f.Status == (int)DB.LeaveRequestStatusEnum.Submitted);
                            e.CreatedBy = subLog.CreatedBy ?? e.CreatedBy;
                            e.CreatedOn = subLog.CreatedOn ?? e.CreatedOn;
                            ticket.SubmittedBy = e.CreatedBy;
                            ticket.SubmittedOn = e.CreatedOn;
                            break;
                        case DB.DailyTicketStatusEnum.Approved:
                            var approvedLog = request.StatusLogs.FirstOrDefault(f => f.Status == (int)DB.LeaveRequestStatusEnum.Approved);
                            e.CreatedBy = approvedLog.CreatedBy ?? e.CreatedBy;
                            e.CreatedOn = approvedLog.CreatedOn ?? e.CreatedOn;
                            ticket.ApprovedBy = e.CreatedBy;
                            ticket.ApprovedOn = e.CreatedOn;
                            break;
                        case DB.DailyTicketStatusEnum.Processed:
                            break;
                        default:
                            break;
                    }
                });
                foreach (var line in emp.List)
                {
                    var entry = ticket.AddHoursEntry(line);
                    var perdiem = ticket.AddPerdiem(line);
                    entry.PerdiemLineNum = perdiem.LineNum;
                    line.TicketId = ticket.TicketId;
                    
                    if (line.Request.Status != (int)DB.LeaveRequestStatusEnum.Processed)
                    {
                        line.Request.Status = (int)DB.LeaveRequestStatusEnum.Processed;
                        line.Request.StatusLogs.Add(RequestStatusLogRepository.Init(line.Request));
                        RequestWorkFlowRepository.GenerateWorkFlow(line.Request);
                    }
                }
                ticket.Status = DB.DailyTicketStatusEnum.Approved;
            }
        }

        public static void SplitRequest(int WeekId, byte prco, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            //using var db = new VPContext();
            var requestList = db.LeaveRequestLines
                .Where(f => f.PRCo == prco && 
                            f.Calendar.Week == WeekId &&
                           (f.Request.Status == (int)DB.PORequestStatusEnum.Approved ||
                            f.Request.Status == (int)DB.PORequestStatusEnum.Processed))
                .GroupBy(g => g.Request)
                .Select(s => new { s.Key })
                .ToList();
            var requestId = db.LeaveRequests.Where(f => f.PRCo == prco).DefaultIfEmpty().Max(f => f == null ? 0 : f.RequestId) + 1;
            foreach (var item in requestList)
            {
                var leave = item.Key;
                var leaveLine = leave.Lines.ToList();

                leaveLine = leaveLine.Where(f => f.WorkDate != null).ToList();
                if (leaveLine.Any(f => f.Calendar.Week != WeekId))
                {
                    var newLeave = new LeaveRequest
                    {
                        PRCo = leave.PRCo,
                        RequestId = requestId,
                        EmployeeId = leave.EmployeeId,
                        CreatedBy = leave.CreatedBy,
                        CreatedOn = leave.CreatedOn,
                        
                        Status = (int)DB.PORequestStatusEnum.Approved, //Set new leave request to approved to be picked up by the next week ticket create
                        //newLeave.UniqueAttchID = leave.UniqueAttchID,
                        Comments = leave.Comments
                    };
                    foreach (var line in leave.Lines.Where(f => f.Calendar.Week != WeekId).ToList())
                    {
                        var newLine = new LeaveRequestLine
                        {
                            PRCo = line.PRCo,
                            RequestId = newLeave.RequestId,
                            LineId = line.LineId,
                            EmployeeId = line.EmployeeId,
                            LineNum = line.LineNum,
                            LeaveCodeId = line.LeaveCodeId,
                            WorkDate = line.WorkDate,
                            Comments = line.Comments,
                            Hours = line.Hours
                        };

                        newLeave.Lines.Add(newLine);
                    }
                    foreach (var log in leave.StatusLogs)
                    {
                        var newLog = new LeaveRequestStatusLog
                        {
                            PRCo = log.PRCo,
                            RequestId = newLeave.RequestId,
                            LineNum = log.LineNum,
                            CreatedOn = log.CreatedOn,
                            CreatedBy = log.CreatedBy,
                            Status = log.Status,
                            Comments = log.Comments
                        };

                        newLeave.StatusLogs.Add(newLog);
                    }
                    foreach (var flow in leave.WorkFlows)
                    {
                        var newFlow = new LeaveRequestWorkFlow
                        {
                            PRCo = flow.PRCo,
                            RequestId = newLeave.RequestId,
                            LineId = flow.LineId,
                            Status = flow.Status,
                            StatusDate = flow.StatusDate,
                            AssignedTo = flow.AssignedTo,
                            AssignedOn = flow.AssignedOn,
                            AssignedStatus = flow.AssignedStatus,
                            Active = flow.Active,
                            CompletedOn = flow.CompletedOn,
                            CreatedBy = flow.CreatedBy,
                            CreatedOn = flow.CreatedOn,
                            Comments = flow.Comments
                        };

                        newLeave.WorkFlows.Add(newFlow);
                    }
                    db.LeaveRequests.Add(newLeave);
                    requestId++;
                    foreach (var line in leave.Lines.Where(f => f.Calendar.Week != WeekId).ToList())
                    {
                        leave.Lines.Remove(line);
                    }
                    db.SaveChanges();
                }

            }
        }
    }
}
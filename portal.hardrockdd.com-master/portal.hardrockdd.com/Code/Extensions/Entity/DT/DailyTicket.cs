using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyTicket
    {
        public string BaseTableName { get { return "budDTTM"; } }

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
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public DateTime? WorkDate
        {
            get
            {
                return tWorkDate;
            }
            set
            {
                if (value != tWorkDate)
                {
                    UpdateWorkDate(value);
                }
            }
        }

        public void UpdateWorkDate(DateTime? newValue)
        {
            tWorkDate = newValue;
            if (this.DailyTruckTicket != null)
                this.DailyTruckTicket.WorkDate = newValue;

            if (this.DailyShopTicket != null)
                this.DailyShopTicket.WorkDate = newValue;

            if (this.DailyTruckTicket != null)
                this.DailyTruckTicket.WorkDate = newValue;

            this.DailyEmployeeEntries.ToList().ForEach(e => e.WorkDate = newValue);
            this.DailyEmployeePerdiems.ToList().ForEach(e => e.WorkDate = newValue);
            this.DailyJobEmployees.ToList().ForEach(e => e.WorkDate = newValue);
            //this.DailyJobTasks.ToList().ForEach(e => e.WorkDate = newValue);

            //this.DailyEquipments.ToList().ForEach(e => e.WorkDate = newValue);
            this.POUsages.ToList().ForEach(e => e.WorkDate = newValue);
        }

        private HQAttachment _Attachment;

        public HQAttachment Attachment
        {
            get
            {
                if (_Attachment == null)
                {
                    var db = VPEntities.GetDbContextFromEntity(this);
                    _Attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == this.UniqueAttchID);

                    if (_Attachment == null)
                    {
                        _Attachment = HQAttachment.Init(this.DTCo, this.BaseTableName, this.KeyID);
                        db.HQAttachments.Add(_Attachment);
                        this.UniqueAttchID = _Attachment.UniqueAttchID;
                        db.BulkSaveChanges();
                    }

                    _Attachment.BuildDefaultFolders();
                    db.BulkSaveChanges();
                }

                return _Attachment;
            }
        }

        public string StatusComments { get; set; }

        public int? StatusId
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

        public DB.DailyTicketStatusEnum Status
        {
            get
            {
                return (DB.DailyTicketStatusEnum)(this.StatusId ?? 0);
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public DB.DTFormEnum FormType
        {
            get
            {
                return (DB.DTFormEnum)(this.FormId ?? 0);
            }
            set
            {
                if (FormId != (int)value)
                {
                    UpdateFormType((int)value);
                }
            }
        }

        public void UpdateFormType(int? value)
        {
            if (value != FormId)
            {
                FormId = value;
                if (FormId == null)
                    return;

                switch (FormType)
                {
                    case DB.DTFormEnum.JobFieldTicket:
                        AddJobTicket();
                        break;
                    case DB.DTFormEnum.ProjectManager:
                        throw new NotImplementedException();
                    case DB.DTFormEnum.TruckingTicket:
                        AddTruckTicket();
                        break;
                    case DB.DTFormEnum.EmployeeDetailTicket:
                        throw new NotImplementedException();
                    case DB.DTFormEnum.EmployeeTicket:
                        AddEmployeeTicket();
                        break;
                    case DB.DTFormEnum.ShopTicket:
                        AddShopTicket();
                        break;
                    case DB.DTFormEnum.SubContractorTicket:
                        throw new NotImplementedException();
                    case DB.DTFormEnum.CrewTicket:
                        AddCrewTicket();
                        break;
                    case DB.DTFormEnum.TimeOff:
                        AddTimeOffTicket();
                        break;
                    case DB.DTFormEnum.PayrollEntriesTicket:
                        throw new NotImplementedException();
                    case DB.DTFormEnum.HolidayTicket:
                        AddHolidayTicket();
                        break;
                    default:
                        break;
                }
            }
        }


        #region Status Updates
        private void UpdateStatusChange(int? newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            if (newValue != StatusId)
            {
                var status = (DB.DailyTicketStatusEnum?)newValue;
                switch (status)
                {
                    case DB.DailyTicketStatusEnum.Draft:
                        break;
                    case DB.DailyTicketStatusEnum.Submitted:
                        SubmitTicket();
                        break;
                    case DB.DailyTicketStatusEnum.Approved:
                        ApproveTicket();
                        break;
                    case DB.DailyTicketStatusEnum.Processed:
                        break;
                    case DB.DailyTicketStatusEnum.Rejected:
                        RejectTicket();
                        break;
                    case DB.DailyTicketStatusEnum.Canceled:
                        CancelTicket();
                        break;
                    case DB.DailyTicketStatusEnum.Deleted:
                        CancelTicket();
                        break;
                    case DB.DailyTicketStatusEnum.UnPosted:
                        UnpostTicket();
                        break;
                    case DB.DailyTicketStatusEnum.Reviewed:
                        break;
                    case DB.DailyTicketStatusEnum.Report:
                        break;
                    default:
                        break;
                }
                tStatusId = newValue;
                CreateStatusLog();
                WorkFlow.CreateSequance((int)newValue);
                WorkFlow.CurrentSequance().Comments = StatusComments;
                GenerateWorkFlowAssignments();
                EmailStatusChange();
            }
        }

        private void SubmitTicket()
        {
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            switch (FormType)
            {
                case DB.DTFormEnum.JobFieldTicket:
                    DailyJobTicket.UpdateEmployeesReportTo();
                    //DailyJobTicket.CalculateRigUtilization();
                    //DailyJobTicket.Job.Status = DailyJobTicket.Job.CalculateStatus();
                    break;
                case DB.DTFormEnum.ProjectManager:
                    break;
                case DB.DTFormEnum.TruckingTicket:
                    break;
                case DB.DTFormEnum.EmployeeDetailTicket:
                    break;
                case DB.DTFormEnum.EmployeeTicket:
                    break;
                case DB.DTFormEnum.ShopTicket:
                    break;
                case DB.DTFormEnum.SubContractorTicket:
                    break;
                case DB.DTFormEnum.CrewTicket:
                    break;
                case DB.DTFormEnum.TimeOff:
                    break;
                case DB.DTFormEnum.PayrollEntriesTicket:
                    break;
                case DB.DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
            }
            this.SubmittedBy = user.Id;
            this.SubmittedOn = DateTime.Now;
            this.SubmittedUser = user;

            this.ApprovedBy = null;
            this.ApprovedOn = null;
            this.ApprovedUser = null;
        }

        public void ApproveTicket()
        {
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            switch (FormType)
            {
                case DB.DTFormEnum.JobFieldTicket:
                    DailyJobTicket.Job.PRCo = DailyJobTicket.PRCo;
                    DailyJobTicket.Job.CrewId = DailyJobTicket.CrewId;
                    DailyJobTicket.Job.Crew = DailyJobTicket.Crew;

                    DailyJobTicket.Job.EMCo = DailyJobTicket.EMCo;
                    DailyJobTicket.Job.RigId = DailyJobTicket.RigId;
                    DailyJobTicket.Job.Rig = DailyJobTicket.Rig;

                    DailyJobTicket.UpdateEmployeesReportTo();
                    DailyJobTicket.CalculateRigUtilization();
                    DailyJobTicket.Job.UpdateJobStatus();
                    break;
                case DB.DTFormEnum.ProjectManager:
                    break;
                case DB.DTFormEnum.TruckingTicket:
                    DailyTruckTicket.UpdateEmployeesReportTo();
                    break;
                case DB.DTFormEnum.EmployeeDetailTicket:
                    break;
                case DB.DTFormEnum.EmployeeTicket:
                    break;
                case DB.DTFormEnum.ShopTicket:
                    DailyShopTicket.UpdateEmployeesReportTo();
                    break;
                case DB.DTFormEnum.SubContractorTicket:
                    break;
                case DB.DTFormEnum.CrewTicket:
                    DailyShopTicket.UpdateEmployeesReportTo();
                    break;
                case DB.DTFormEnum.TimeOff:
                    break;
                case DB.DTFormEnum.PayrollEntriesTicket:
                    break;
                case DB.DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
            }

            this.SubmittedBy ??= user.Id;
            this.SubmittedOn ??= DateTime.Now;
            this.SubmittedUser ??= user;

            this.ApprovedBy = user.Id;
            this.ApprovedOn = DateTime.Now;
            this.ApprovedUser = user;

            RemoveDTPayrollEntries();
            GenerateDTPayrollEntries();
        }

        private void CancelTicket()
        {
            RemoveDTPayrollEntries();
            RemoveGanttTasks();
            if (FormType == DB.DTFormEnum.TimeOff)
            {
                DeleteTimeOffTicket();
            }
        }

        private void RejectTicket()
        {
            //SubmittedBy = null;
            //SubmittedOn = new DateTime(1900, 1, 1);

            //ApprovedBy = null;
            //ApprovedOn = new DateTime(1900, 1, 1);

            RemoveDTPayrollEntries();
            RemoveGanttTasks();
            if (FormType == DB.DTFormEnum.TimeOff)
            {
                RejectTimeOffTicket();
            }
        }

        private void UnpostTicket()
        {
            var tempEntries = db.PRBatchTimeEntries.Where(f => f.udTicketId == TicketId).ToList();
            db.BulkDelete(tempEntries);

            var hours = db.DTPayrollHours.Where(f => f.TicketId == TicketId).ToList();
            db.BulkDelete(hours);

            var perdiems = db.DTPayrollHours.Where(f => f.TicketId == TicketId).ToList();
            db.BulkDelete(perdiems);

            var entries = db.PayrollEntries.Where(f => f.udTicketId == TicketId).ToList();
            var hoursList = new List<DTPayrollHour>();
            foreach (var entry in entries)
            {
                if (entry.Hours != 0)
                {
                    var hour = Repository.VP.DT.PayrollHourRepository.Init(entry, this);
                    hour.Hours *= -1;
                    hour.Status = DB.PayrollEntryStatusEnum.Reversal;
                    DTPayrollHours.Add(hour);
                }
                if (entry.Hours != 0)
                {
                    var hour = Repository.VP.DT.PayrollHourRepository.Init(entry, this);
                    hour.Status = DB.PayrollEntryStatusEnum.Posted;
                    DTPayrollHours.Add(hour);
                }
            }
            UpdateDTPayrollHourLinenums();
            RemoveGanttTasks();
        }

        private void DeleteTimeOffTicket()
        {
            using var tmpDb = new VPEntities();
            var requestLines = tmpDb.LeaveRequestLines.Where(f => f.TicketId == TicketId).ToList();
            foreach (var line in requestLines)
            {
                line.TicketId = null;
                line.LineNum = null;
                if (line.Request.Status != (int)DB.LeaveRequestStatusEnum.Rejected)
                {
                    line.Request.Status = (int)DB.LeaveRequestStatusEnum.Approved;
                    var statusLog = Repository.VP.PR.RequestStatusLogRepository.Init(line.Request);
                    line.Request.StatusLogs.Add(statusLog);
                    Repository.VP.PR.RequestWorkFlowRepository.GenerateWorkFlow(line.Request);
                }
            }
            tmpDb.SaveChanges();
        }

        private void RejectTimeOffTicket()
        {
            using var tmpDb = new VPEntities();
            var requestLines = tmpDb.LeaveRequestLines.Where(f => f.TicketId == TicketId).ToList();
            foreach (var line in requestLines)
            {
                line.TicketId = null;
                line.LineNum = null;
                if (line.Request.Status != (int)DB.LeaveRequestStatusEnum.Rejected)
                {
                    line.Request.Status = (int)DB.LeaveRequestStatusEnum.Rejected;
                    var statusLog = Repository.VP.PR.RequestStatusLogRepository.Init(line.Request);
                    statusLog.Comments = StatusComments;
                    line.Request.StatusLogs.Add(statusLog);
                    Repository.VP.PR.RequestWorkFlowRepository.GenerateWorkFlow(line.Request);
                }
            }
            tmpDb.SaveChanges();
        }

        public void EmailStatusChange()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new portal.Models.Views.DailyTicket.DailyTicketEmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPEntities();
            switch (Status)
            {
                default:
                case DB.DailyTicketStatusEnum.Draft:
                case DB.DailyTicketStatusEnum.Submitted:
                case DB.DailyTicketStatusEnum.Approved:
                case DB.DailyTicketStatusEnum.Processed:
                case DB.DailyTicketStatusEnum.Canceled:
                case DB.DailyTicketStatusEnum.Deleted:
                case DB.DailyTicketStatusEnum.UnPosted:
                case DB.DailyTicketStatusEnum.Reviewed:
                case DB.DailyTicketStatusEnum.Report:
                    break;
                case DB.DailyTicketStatusEnum.Rejected:
                    viewPath = "../DT/Email/EmailReject";
                    subject = string.Format(AppCultureInfo.CInfo(), "{0} Rejected By {1}", Form.Description, StaticFunctions.GetCurrentEmployee().FullName(false));
                    break;
            }

            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                    {
                        Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    {
                        var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                        msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
                    }

                    if (CreatedUser.Email != StaticFunctions.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
                    }
                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void CreateStatusLog()
        {
            var log = new DailyStatusLog
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyTicketStatusLogs
                           .DefaultIfEmpty()
                           .Max(f => f == null ? 0 : f.LineNum) + 1,
                Status = (short)Status,
                CreatedOn = DateTime.Now,
                Comments = StatusComments,
                CreatedBy = StaticFunctions.GetUserId()
            };

            DailyTicketStatusLogs.Add(log);
        }

        public void CreateReviewLog()
        {
            var userId = StaticFunctions.GetUserId();
            //Only add review log if there are none for todays date and user
            if (!DailyTicketStatusLogs.Any(f => f.CreatedBy == userId &&
                                                f.Status == (short)DB.DailyTicketStatusEnum.Reviewed &&
                                                f.CreatedOn.Date == DateTime.Now.Date))
            {
                var log = new DailyStatusLog
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    LineNum = DailyTicketStatusLogs
                               .DefaultIfEmpty()
                               .Max(f => f == null ? 0 : f.LineNum) + 1,
                    Status = (short)DB.DailyTicketStatusEnum.Reviewed,
                    CreatedOn = DateTime.Now,
                    Comments = StatusComments,
                    CreatedBy = StaticFunctions.GetUserId()
                };

                DailyTicketStatusLogs.Add(log);

                db.BulkSaveChanges();
            }
        }
        #endregion

        #region Payroll Entries
        public void RemoveDTPayrollEntries()
        {
            var hourList = DTPayrollHours.Where(f => f.Status != DB.PayrollEntryStatusEnum.Posted && f.Status != DB.PayrollEntryStatusEnum.Reversal).ToList();
            var perdiemList = DTPayrollPerdiems.Where(f => f.Status != DB.PayrollEntryStatusEnum.Posted && f.Status != DB.PayrollEntryStatusEnum.Reversal).ToList();

            if (hourList.Any())
                hourList.ForEach(e => db.DTPayrollHours.Remove(e));
            if (perdiemList.Any())
                perdiemList.ForEach(e => db.DTPayrollPerdiems.Remove(e));
        }

        public void GenerateDTPayrollEntries()
        {
            var jobEntries = DailyJobEmployees.Where(f => f.PREmployee != null).ToList();
            foreach (var employee in jobEntries)
            {
                employee.GenerateDTPayrollEntries();
            }

            var employeeEntries = DailyEmployeeEntries.Where(f => f.PREmployee != null).ToList();
            foreach (var entry in employeeEntries)
            {
                entry.GenerateDTPayrollEntry();
            }

            var employeePerdiems = DailyEmployeePerdiems.Where(f => f.PREmployee != null).ToList();
            foreach (var entry in employeePerdiems)
            {
                entry.GenerateDTPayrollPerdiem();
            }

            UpdateDTPayrollHourLinenums();
            UpdateDTPayrollPerdiemLinenums();
        }

        public void UpdateDTPayrollHourLinenums()
        {
            var employeeWorkDayHours = DTPayrollHours.GroupBy(g => new { g.PRCo, g.EmployeeId, g.WorkDate })
                                                .Select(s => new { s.Key.PRCo, s.Key.EmployeeId, s.Key.WorkDate, Lines = s.ToList() })
                                                .ToList();


            foreach (var employeeWorkDay in employeeWorkDayHours)
            {
                var lineNum = db.DTPayrollHours.Where(f => f.PRCo == employeeWorkDay.PRCo &&
                                                           f.EmployeeId == employeeWorkDay.EmployeeId &&
                                                           f.WorkDate == employeeWorkDay.WorkDate)
                                               .DefaultIfEmpty()
                                               .Max(max => max == null ? 0 : max.LineNum) + 1;

                employeeWorkDay.Lines.ForEach(e => {
                    if (e.LineNum == 0)
                    {
                        e.LineNum = lineNum;
                        lineNum++;
                    }
                });
            }
        }

        public void UpdateDTPayrollPerdiemLinenums()
        {

            var employeeWorkDayPerdiems = DTPayrollPerdiems.GroupBy(g => new { g.PRCo, g.EmployeeId, g.WorkDate })
                                                .Select(s => new { s.Key.PRCo, s.Key.EmployeeId, s.Key.WorkDate, Lines = s.ToList() })
                                                .ToList();


            foreach (var employeeWorkDay in employeeWorkDayPerdiems)
            {
                var lineNum = db.DTPayrollPerdiems.Where(f => f.PRCo == employeeWorkDay.PRCo &&
                                                           f.EmployeeId == employeeWorkDay.EmployeeId &&
                                                           f.WorkDate == employeeWorkDay.WorkDate)
                                               .DefaultIfEmpty()
                                               .Max(max => max == null ? 0 : max.LineNum) + 1;

                employeeWorkDay.Lines.ForEach(e => {
                    if (e.LineNum == 0)
                    {
                        e.LineNum = lineNum;
                        lineNum++;
                    }
                });
            }
        }
        #endregion

        public void RemoveGanttTasks()
        {
            //if (DailyJobTasks.Any())
            //{
            //    var ganttTasks = DailyJobTasks.Where(f => ProjectGanttTask != null ).Select(s => s.ProjectGanttTask).ToList();
            //    DailyJobTasks.ToList().ForEach(e =>
            //    {
            //        e.GanttId = null;
            //        e.TaskId = null;
            //    });
            //    db.BulkDelete(ganttTasks);
            //}
        }

        #region Workflow
        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = DTCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(DTCo),
                    TableName = "budBDBH",
                    Id = TicketId,
                    CreatedBy = CreatedBy,
                    CreatedOn = CreatedOn ?? DateTime.Now,
                    Active = true,

                    Company = HQCompanyParm,
                };
                HQCompanyParm.WorkFlows.Add(workflow);
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
                case DB.DailyTicketStatusEnum.Draft:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case DB.DailyTicketStatusEnum.Submitted:
                    if (FormType == DB.DTFormEnum.JobFieldTicket && DailyJobTicket != null)
                    {
                        WorkFlow.AddEmployee(this.DailyJobTicket.Job.Division.WPDivision.DivisionManger);
                    }
                    else
                    {
                        WorkFlow.AddEmployee(CreatedUser.PREmployee.Supervisor);
                        WorkFlow.AddEmployee(CreatedUser.PREmployee.Supervisor?.Supervisor);
                    }
                    break;
                case DB.DailyTicketStatusEnum.Approved:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    break;
                case DB.DailyTicketStatusEnum.Processed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.DailyTicketStatusEnum.Rejected:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddUser(ApprovedUser);
                    break;
                case DB.DailyTicketStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.DailyTicketStatusEnum.Deleted:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.DailyTicketStatusEnum.UnPosted:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    break;
                case DB.DailyTicketStatusEnum.Reviewed:
                case DB.DailyTicketStatusEnum.Report:
                default:
                    break;
            }


        }
        #endregion

        #region Ticket Type Creatation
        private void AddJobTicket()
        {
            var result = this.DailyJobTicket;
            if (result == null)
            {
                var crewLeader = StaticFunctions.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.CrewLeaderID == crewLeader.EmployeeId);
                var comp = StaticFunctions.GetCurrentCompany();
                var jobParms = db.JCCompanyParms.FirstOrDefault(f => f.JCCo == comp.HQCo);

                result = new DailyJobTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    tWorkDate = WorkDate,
                    PRCo = jobParms.PRCo,
                    JCCo = jobParms.JCCo,
                    DailyTicket = this,
                };
                result.CrewId = crew?.CrewId;

                DailyJobTicket = result;
                db.DailyJobTickets.Add(result);
            }
        }

        private void AddTruckTicket()
        {
            var result = this.DailyTruckTicket;
            if (result == null)
            {
                var crewLeader = StaticFunctions.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.CrewLeaderID == crewLeader.EmployeeId);

                result = new DailyTruckTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    tWorkDate = WorkDate,
                    //PRCo = crew?.PRCo,
                    //tCrewId = crew?.CrewId,
                    //Crew = crew,
                    DailyTicket = this,
                };
                DailyTruckTicket = result;
                result.CrewId = crew?.CrewId;
            }
        }

        private void AddEmployeeTicket()
        {
            var result = this.DailyEmployeeTicket;
            if (result == null)
            {
                var cal = db.Calendars.FirstOrDefault(f => f.Date == WorkDate);
                var emp = StaticFunctions.GetCurrentEmployee();
                emp = db.Employees.FirstOrDefault(f => f.PRCo == emp.PRCo && f.EmployeeId == emp.EmployeeId);
                result = new DailyEmployeeTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    WeekId = cal.Week,
                    PRCo = emp.PRCo,
                    EmployeeId = emp.EmployeeId,
                    PREmployee = emp,
                    DailyTicket = this,
                };

                DailyEmployeeTicket = result;

                DailyEmployeeTicket.CreateDefaultEntries();
            }
        }

        private void AddShopTicket()
        {
            var result = this.DailyShopTicket;
            if (result == null)
            {
                var crewLeader = StaticFunctions.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.CrewLeaderID == crewLeader.EmployeeId);

                result = new DailyShopTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    tWorkDate = WorkDate,
                    DailyTicket = this,
                    //PRCo = crew?.PRCo,
                    //CrewId = crew?.CrewId,
                    //Crew = crew,
                };
                this.DailyShopTicket = result;
                if (crew != null)
                {
                    result.CrewId = crew?.CrewId;
                }

                result.GenerateEmployeeList();
            }
        }

        private void AddCrewTicket()
        {
            var result = this.DailyShopTicket;
            if (result == null)
            {
                var crewLeader = StaticFunctions.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.CrewLeaderID == crewLeader.EmployeeId);

                result = new DailyShopTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    tWorkDate = WorkDate,
                    DailyTicket = this,

                };
                this.DailyShopTicket = result;
                if (crew != null)
                {
                    result.CrewId = crew?.CrewId;
                }
                result.GenerateEmployeeList();
            }
        }

        private void AddTimeOffTicket(Employee employee = null)
        {
            var result = this.GeneralTicket;
            if (result == null)
            {
                result = new DailyGeneralTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    DailyTicket = this,
                };
                if (employee != null)
                {
                    result.PRCo = employee.PRCo;
                    result.EmployeeId = employee.EmployeeId;
                    result.Employee = employee;
                }
                GeneralTicket = result;
            }
        }

        private void AddHolidayTicket()
        {
            var result = this.GeneralTicket;
            if (result == null)
            {
                result = new DailyGeneralTicket
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    DailyTicket = this,
                };
                this.GeneralTicket = result;
                var employees = db.Employees.Where(f => f.PRCo == HQCompanyParm.PRCo && f.ActiveYN == "Y").ToList();
                foreach (var emp in employees)
                {
                    var entry = result.AddHoursEntry((DateTime)WorkDate, emp);
                    var perdiem = result.AddPerdiem((DateTime)WorkDate, emp);
                    entry.PerdiemLineNum = perdiem.LineNum;
                }
            }
        }
        #endregion

        public DailyEmployeeEntry AddHoursEntry()
        {
            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = WorkDate,
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                ModifiedBy = StaticFunctions.GetUserId(),
                ModifiedOn = DateTime.Now,

                DailyTicket = this,
            };

            DailyEmployeeEntries.Add(entry);


            return entry;
        }

        public DailyEmployeeEntry AddHoursEntry(Employee employee)
        {
            if (employee == null)
                return AddHoursEntry();

            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = WorkDate,
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                ModifiedBy = StaticFunctions.GetUserId(),
                ModifiedOn = DateTime.Now,
                PRCo = employee.PRCo,
                tEmployeeId = employee.EmployeeId,

                PREmployee = employee,
                DailyTicket = this,
            };

            DailyEmployeeEntries.Add(entry);


            return entry;
        }

        public DailyEmployeeEntry AddHoursEntry(LeaveRequestLine leaveLine)
        {
            if (leaveLine == null)
                return AddHoursEntry();

            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = leaveLine.WorkDate,
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                ModifiedBy = StaticFunctions.GetUserId(),
                ModifiedOn = DateTime.Now,
                PRCo = leaveLine.PRCo,
                tEmployeeId = leaveLine.EmployeeId,
                Value = leaveLine.Hours,
                PREmployee = leaveLine.Employee,
                DailyTicket = this,
            };
            var earnCode = leaveLine.LeaveCode.Usages.FirstOrDefault(f => f.EarnCode.Method == leaveLine.Employee.EarnCode.Method && f.Type == "U")?.EarnCode;
            if (earnCode == null)
            {
                earnCode = leaveLine.LeaveCode.Usages.FirstOrDefault(f => f.Type == "U")?.EarnCode;
            }
            if (earnCode != null)
            {
                entry.EarnCodeId = earnCode.EarnCodeId;
                entry.EarnCode = earnCode;
            }
            DailyEmployeeEntries.Add(entry);


            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem()
        {
            var perdiem = new DailyEmployeePerdiem
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                PerDiemId = (int)DB.PerdiemEnum.No,
                tWorkDate = WorkDate,
                ModifiedBy = StaticFunctions.GetUserId(),
                ModifiedOn = DateTime.Now,

                DailyTicket = this,
            };

            DailyEmployeePerdiems.Add(perdiem);

            return perdiem;
        }

        public DailyEmployeePerdiem AddPerdiem(Employee employee, DateTime? workDate = null)
        {
            if (employee == null)
                return AddPerdiem();
            workDate ??= WorkDate;
            var perdiem = DailyEmployeePerdiems.FirstOrDefault(f => f.WorkDate == workDate && f.EmployeeId == employee.EmployeeId && f.PRCo == employee.PRCo);
            if (perdiem == null)
            {
                perdiem = new DailyEmployeePerdiem
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    LineNum = DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                    PerDiemId = (int)DB.PerdiemEnum.No,
                    tWorkDate = workDate,
                    ModifiedBy = StaticFunctions.GetUserId(),
                    ModifiedOn = DateTime.Now,
                    PREmployee = employee,
                    PRCo = employee.PRCo,
                    tEmployeeId = employee.EmployeeId,

                    DailyTicket = this,
                };

                DailyEmployeePerdiems.Add(perdiem);

            }
            return perdiem;
        }

        public DailyEmployeePerdiem AddPerdiem(LeaveRequestLine leaveLine)
        {
            if (leaveLine == null)
                return AddPerdiem();

            var entry = new DailyEmployeePerdiem
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = leaveLine.WorkDate,
                ModifiedBy = StaticFunctions.GetUserId(),
                ModifiedOn = DateTime.Now,
                PRCo = leaveLine.PRCo,
                tEmployeeId = leaveLine.EmployeeId,
                PerDiemId = (int)DB.PerdiemEnum.No,
                PREmployee = leaveLine.Employee,
                DailyTicket = this,
            };
            DailyEmployeePerdiems.Add(entry);


            return entry;
        }

        public static int GetNextTicketId()
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

            var result = db.udNextTicketId(outParm);

            return (int)outParm.Value;
        }
    }
}
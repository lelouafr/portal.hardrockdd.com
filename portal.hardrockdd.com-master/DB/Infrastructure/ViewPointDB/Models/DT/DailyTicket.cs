using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyTicket: IAttachment
    {
        public static string BaseTableName { get { return "budDTTM"; } }

        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= HQCompanyParm.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
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

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                {
                    var root = HQAttachment.GetRootFolder();
                    if (!root.SubFolders.Any())
                    {
                        HQAttachment.BuildDefaultFolders();
                        db.BulkSaveChanges();
                    }
                    if (this.FormType == DTFormEnum.JobFieldTicket)
                    {
                        var folderLoc = GetSharePointRootFolderPath();
                        if (root.StorageLocation == DB.HQAttachmentStorageEnum.DB)
                        {
                            if (!string.IsNullOrEmpty(folderLoc))
                            {
                                var list = GetSharePointList();
                                if (list != null)
                                {
                                    HQAttachment.SharePointList = list;

                                    root.SharePointFolderPath = folderLoc;
                                    var folder = root.GetFolderSharePoint();
                                    if (folder == null)
                                        folder = root.CreateFolderSharePoint();

                                    HQAttachment.SharePointRootFolder = root.SharePointFolderPath;
                                    root.StorageLocation = DB.HQAttachmentStorageEnum.SharePoint;

                                    db.BulkSaveChanges();
                                }
                            }
                            else if (folderLoc != root.SharePointFolderPath)
                            {
                                root.SharePointFolderPath = folderLoc;
                                HQAttachment.SharePointRootFolder = root.SharePointFolderPath;
                                db.BulkSaveChanges();
                            }
                        }
                        if (folderLoc != root.SharePointFolderPath && !string.IsNullOrEmpty(folderLoc))
                        {

                            root.SharePointFolderPath = folderLoc;
                            HQAttachment.SharePointRootFolder = root.SharePointFolderPath;
                            db.BulkSaveChanges();
                        }
                       // root.SyncFromSource();
                    }
                    return HQAttachment;
                }

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCompanyParm = this.HQCompanyParm,
                    db = this.db,

                    HQCo = this.DTCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return Attachment;
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

        public DailyTicketStatusEnum Status
        {
            get
            {
                return (DailyTicketStatusEnum)(this.StatusId ?? 0);
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public DTFormEnum FormType
        {
            get
            {
                return (DTFormEnum)(this.FormId ?? 0);
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
                    case DTFormEnum.JobFieldTicket:
                        AddJobTicket();
                        break;
                    case DTFormEnum.ProjectManager:
                        throw new NotImplementedException();
                    case DTFormEnum.TruckingTicket:
                        AddTruckTicket();
                        break;
                    case DTFormEnum.EmployeeDetailTicket:
                        throw new NotImplementedException();
                    case DTFormEnum.EmployeeTicket:
                        AddEmployeeTicket();
                        break;
                    case DTFormEnum.ShopTicket:
                        AddShopTicket();
                        break;
                    case DTFormEnum.SubContractorTicket:
                        throw new NotImplementedException();
                    case DTFormEnum.CrewTicket:
                        AddCrewTicket();
                        break;
                    case DTFormEnum.TimeOff:
                        AddTimeOffTicket();
                        break;
                    case DTFormEnum.PayrollEntriesTicket:
                        throw new NotImplementedException();
                    case DTFormEnum.HolidayTicket:
                        AddHolidayTicket();
                        break;
                    default:
                        break;
                }
            }
        }

        public Dictionary<string,string> ValidationErrors { get; set; }

        #region Status Updates
        private void UpdateStatusChange(int? newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            if (newValue != StatusId)
            {
                var status = (DailyTicketStatusEnum?)newValue;
                switch (status)
                {
                    case DailyTicketStatusEnum.Draft:
						ValidationErrors = null;
						break;
                    case DailyTicketStatusEnum.Submitted:
						ValidationErrors = this.Validate();
						SubmitTicket();
                        break;
                    case DailyTicketStatusEnum.Approved:
						ValidationErrors = this.Validate();
						ApproveTicket();
                        break;
                    case DailyTicketStatusEnum.Processed:
						ValidationErrors = this.Validate();
						break;
                    case DailyTicketStatusEnum.Rejected:
						ValidationErrors = null;
						RejectTicket();
                        break;
                    case DailyTicketStatusEnum.Canceled:
						ValidationErrors = null;
						CancelTicket();
                        break;
                    case DailyTicketStatusEnum.Deleted:
						ValidationErrors = null;
						CancelTicket();
						break;
                    case DailyTicketStatusEnum.UnPosted:
						ValidationErrors = null;
						UnpostTicket();
                        break;
                    case DailyTicketStatusEnum.Reviewed:
						ValidationErrors = null;
						break;
                    case DailyTicketStatusEnum.Report:
						ValidationErrors = null;
						break;
                    default:
                        break;
                }
                tStatusId = newValue;
                CreateStatusLog();
                WorkFlow.CreateSequence((int)newValue);
                WorkFlow.CurrentSequence().Comments = StatusComments;
                GenerateWorkFlowAssignments();
                EmailStatusChange();
            }
        }

        public Dictionary<string, string> Validate()
		{
			var results = new Dictionary<string, string>();
			switch (FormType)
            {
                case DTFormEnum.JobFieldTicket:
					ValidateJobTicket().ToList().ForEach(x => results.Add(x.Key, x.Value));
					break;
                case DTFormEnum.ProjectManager:
                    break;
                case DTFormEnum.TruckingTicket:
					ValidateTruckTicket().ToList().ForEach(x => results.Add(x.Key, x.Value));
					break;
                case DTFormEnum.EmployeeDetailTicket:
                    break;
                case DTFormEnum.EmployeeTicket:
                    break;
                case DTFormEnum.ShopTicket:
					ValidateShopTicket().ToList().ForEach(x => results.Add(x.Key, x.Value));
					break;
                case DTFormEnum.SubContractorTicket:
                    break;
                case DTFormEnum.CrewTicket:
					ValidateCrewTicket().ToList().ForEach(x => results.Add(x.Key, x.Value));
					break;
                case DTFormEnum.TimeOff:
                    break;
                case DTFormEnum.PayrollEntriesTicket:
                    break;
                case DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
			}

			var audit = CreatedUser.ActiveEquipmentAudits().FirstOrDefault();

			if (audit != null && audit.IsAuditLate(this))
				results.Add("Audit", "You have a Late Equipment Audit, must complete Audit before submitting Ticket!");

			return results;

		}

        private Dictionary<string, string> ValidateJobTicket()
        {
            var results = new Dictionary<string, string>();
            if (DailyJobTicket == null)
                return results;

			var productionHours = DailyJobTicket.PayrollHourTasks();
			var taskTotHours = productionHours.Sum(s => s.PayrollValue);
			var perDiemCnt = DailyJobEmployees.Sum(sum => sum.Perdiem ?? 0);
			var userMaxHours = this.DailyJobEmployees.Max(m => m.Hours);

			if (taskTotHours != userMaxHours)
				results.Add("TasksHrs", "Task hours do not add up to max employee Hours");

			if (DailyJobTicket.Comments == null)
				results.Add("Comments", "You are missing Comments");

			if (DailyJobTicket.DTJobPhases.Count == 0 && (perDiemCnt > 0 || taskTotHours > 0))
				results.Add("Tasks", "You must have a task if hours or per diem is applied to an employee");

			if (DailyJobTicket.Job == null)
				results.Add("JobId", "You must select a Job.");

			if (DailyJobTicket.Rig == null)
				results.Add("RigId", "You must select a rig.");

			if (DailyJobTicket.Rig != null && !DailyJobTicket.Rig.RevenueCodes.Any(r => r.RevCode == (DailyJobTicket.Rig.RevenueCodeId ?? "2")))
				results.Add("RigId", "The rig selected does not have rates, cannot be use in ticket");

			foreach (var task in DailyJobTicket.DTJobPhases)
			{
				if (task.JobPhase.HasBudgetRadius() && task.ProgressBudgetCode == null)
				{
					task.UpdateProgressBudgetCode();
					task.db.SaveChanges();
				}
				if (task.JobPhase.HasBudgetRadius() && task.ProgressBudgetCode == null)
				{
					results.Add("BudgetCode", "You are missing a pilot size or ream size under tasks");
					break;
				}
			}
            
			return results;
		}

		private Dictionary<string, string> ValidateShopTicket()
		{

			var results = new Dictionary<string, string>();

			if (DailyShopTicket.Crew == null)
				results.Add("CrewId", "You are missing a Crew");

			if (DailyShopTicket.Job == null)
				results.Add("JobId", "You are missing a Shop Job");

			if (DailyShopTicket.Comments == null)
				results.Add("Comments", "You are missing Comments");

			return results;
		}

		private Dictionary<string, string> ValidateCrewTicket()
		{
			return ValidateShopTicket();
		}

        public Crew Crew 
        { 
            get
            {
				return FormType switch
				{
					DTFormEnum.JobFieldTicket => DailyJobTicket.Crew,
					DTFormEnum.ProjectManager => null,
					DTFormEnum.TruckingTicket => DailyTruckTicket.Crew,
					DTFormEnum.EmployeeDetailTicket => null,
					DTFormEnum.EmployeeTicket => this.DailyEmployeeTicket.PREmployee?.Crew,
					DTFormEnum.ShopTicket => DailyShopTicket.Crew,
					DTFormEnum.SubContractorTicket => null,
					DTFormEnum.CrewTicket => DailyShopTicket.Crew,
					DTFormEnum.TimeOff => null,
					DTFormEnum.PayrollEntriesTicket => null,
					DTFormEnum.HolidayTicket => null,
					_ => null,
				};
			}
        }

		private Dictionary<string, string> ValidateTruckTicket()
		{

			var results = new Dictionary<string, string>();

			if (DailyTruckTicket.Crew == null)
				results.Add("CrewId", "You are missing a Crew");

			if (DailyTruckTicket.Comments == null)
				results.Add("Comments", "You are missing Comments");

			return results;
		}

		private void SubmitTicket()
        {
            var userId = db.CurrentUserId;
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            switch (FormType)
            {
                case DTFormEnum.JobFieldTicket:
                    DailyJobTicket.UpdateEmployeesReportTo();
                    //DailyJobTicket.CalculateRigUtilization();
                    //DailyJobTicket.Job.Status = DailyJobTicket.Job.CalculateStatus();
                    break;
                case DTFormEnum.ProjectManager:
                    break;
                case DTFormEnum.TruckingTicket:
                    break;
                case DTFormEnum.EmployeeDetailTicket:
                    break;
                case DTFormEnum.EmployeeTicket:
                    break;
                case DTFormEnum.ShopTicket:
                    break;
                case DTFormEnum.SubContractorTicket:
                    break;
                case DTFormEnum.CrewTicket:
                    break;
                case DTFormEnum.TimeOff:
                    break;
                case DTFormEnum.PayrollEntriesTicket:
                    break;
                case DTFormEnum.HolidayTicket:
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
            var userId = db.CurrentUserId;
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            switch (FormType)
            {
                case DTFormEnum.JobFieldTicket:
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
                case DTFormEnum.ProjectManager:
                    break;
                case DTFormEnum.TruckingTicket:
                    DailyTruckTicket.UpdateEmployeesReportTo();
                    break;
                case DTFormEnum.EmployeeDetailTicket:
                    break;
                case DTFormEnum.EmployeeTicket:
                    break;
                case DTFormEnum.ShopTicket:
                    DailyShopTicket.UpdateEmployeesReportTo();
                    break;
                case DTFormEnum.SubContractorTicket:
                    break;
                case DTFormEnum.CrewTicket:
                    DailyShopTicket.UpdateEmployeesReportTo();
                    break;
                case DTFormEnum.TimeOff:
                    break;
                case DTFormEnum.PayrollEntriesTicket:
                    break;
                case DTFormEnum.HolidayTicket:
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
            if (FormType == DTFormEnum.TimeOff)
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
            if (FormType == DTFormEnum.TimeOff)
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
                    var hour = AddPayrollHour(entry);
                    hour.Hours *= -1;
                    //hour.Amt *= -1;
                    if (hour.EarnCode.Factor != 1 && hour.Rate != null)
                    {
                        hour.Rate /= hour.EarnCode.Factor;
                    }


                    hour.Status = PayrollEntryStatusEnum.Reversal;
                    DTPayrollHours.Add(hour);
                }
                if (entry.Hours != 0)
                {
                    var hour = AddPayrollHour(entry);
                    hour.Status = PayrollEntryStatusEnum.Posted;
                    DTPayrollHours.Add(hour);
                }
            }
            UpdateDTPayrollHourLinenums();
            RemoveGanttTasks();
        }

        private void DeleteTimeOffTicket()
        {
            using var tmpDb = new VPContext();
            var requestLines = tmpDb.LeaveRequestLines.Where(f => f.TicketId == TicketId).ToList();
            foreach (var line in requestLines)
            {
                line.TicketId = null;
                line.LineNum = null;
                if (line.Request.Status != (int)LeaveRequestStatusEnum.Rejected)
                {
                    line.Request.Status = (int)LeaveRequestStatusEnum.Approved;
                    var statusLog = line.Request.AddStatus();
                    line.Request.StatusLogs.Add(statusLog);
                    line.Request.GenerateWorkFlow();
                }
            }
            tmpDb.SaveChanges();
        }

        private void RejectTimeOffTicket()
        {
            using var tmpDb = new VPContext();
            var requestLines = tmpDb.LeaveRequestLines.Where(f => f.TicketId == TicketId).ToList();
            foreach (var line in requestLines)
            {
                line.TicketId = null;
                line.LineNum = null;
                if (line.Request.Status != (int)LeaveRequestStatusEnum.Rejected)
                {
                    line.Request.Status = (int)LeaveRequestStatusEnum.Rejected;
                    var statusLog = line.Request.AddStatus();
                    statusLog.Comments = StatusComments;
                    line.Request.StatusLogs.Add(statusLog);
                    line.Request.GenerateWorkFlow();
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
            var result = new DB.Infrastructure.EmailModels.DailyTicketEmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPContext();
            switch (Status)
            {
                default:
                case DailyTicketStatusEnum.Draft:
                case DailyTicketStatusEnum.Submitted:
                case DailyTicketStatusEnum.Approved:
                case DailyTicketStatusEnum.Processed:
                case DailyTicketStatusEnum.Canceled:
                case DailyTicketStatusEnum.Deleted:
                case DailyTicketStatusEnum.UnPosted:
                case DailyTicketStatusEnum.Reviewed:
                case DailyTicketStatusEnum.Report:
                    break;
                case DailyTicketStatusEnum.Rejected:
                    viewPath = "../DT/Email/EmailReject";
                    subject = string.Format("{0} Rejected By {1}", Form.Description, db.GetCurrentEmployee().FullName(false));
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

                    if (CreatedUser.Email != db.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(db.GetCurrentUser().Email));
                    }
                    Services.EmailHelper.Send(msg);
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
                CreatedBy = db.CurrentUserId
            };

            DailyTicketStatusLogs.Add(log);
        }

        public void CreateReviewLog()
        {
            var userId = db.CurrentUserId;
            //Only add review log if there are none for todays date and user
            if (!DailyTicketStatusLogs.Any(f => f.CreatedBy == userId &&
                                                f.Status == (short)DailyTicketStatusEnum.Reviewed &&
                                                f.CreatedOn.Date == DateTime.Now.Date))
            {
                var log = new DailyStatusLog
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    LineNum = DailyTicketStatusLogs
                               .DefaultIfEmpty()
                               .Max(f => f == null ? 0 : f.LineNum) + 1,
                    Status = (short)DailyTicketStatusEnum.Reviewed,
                    CreatedOn = DateTime.Now,
                    Comments = StatusComments,
                    CreatedBy = db.CurrentUserId
                };

                DailyTicketStatusLogs.Add(log);

                db.BulkSaveChanges();
            }
        }
        #endregion

        #region Payroll Entries
        public void RemoveDTPayrollEntries()
        {
            var hourList = DTPayrollHours.Where(f => f.Status != PayrollEntryStatusEnum.Posted && f.Status != PayrollEntryStatusEnum.Reversal).ToList();
            var perdiemList = DTPayrollPerdiems.Where(f => f.Status != PayrollEntryStatusEnum.Posted && f.Status != PayrollEntryStatusEnum.Reversal).ToList();

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
                //HQCompanyParm.WorkFlows.Add(workflow);
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
                case DailyTicketStatusEnum.Draft:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case DailyTicketStatusEnum.Submitted:
                    if (FormType == DTFormEnum.JobFieldTicket && DailyJobTicket != null)
                    {
                        WorkFlow.AddEmployee(this.DailyJobTicket.Job.Division.WPDivision.DivisionManger);
                    }
                    else
                    {
                        WorkFlow.AddEmployee(CreatedUser.PREmployee.Supervisor);
                        WorkFlow.AddEmployee(CreatedUser.PREmployee.Supervisor?.Supervisor);
                    }
                    break;
                case DailyTicketStatusEnum.Approved:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    break;
                case DailyTicketStatusEnum.Processed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DailyTicketStatusEnum.Rejected:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddUser(ApprovedUser);
                    break;
                case DailyTicketStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DailyTicketStatusEnum.Deleted:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DailyTicketStatusEnum.UnPosted:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    break;
                case DailyTicketStatusEnum.Reviewed:
                case DailyTicketStatusEnum.Report:
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
                var crewLeader = db.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.tCrewLeaderId == crewLeader.EmployeeId);
                var comp = db.GetCurrentCompany();
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
                var crewLeader = db.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.tCrewLeaderId == crewLeader.EmployeeId);

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
                var emp = db.GetCurrentEmployee();
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
                var crewLeader = db.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.tCrewLeaderId == crewLeader.EmployeeId);

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
                var crewLeader = db.GetCurrentEmployee();
                var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.tCrewLeaderId == crewLeader.EmployeeId);

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
                    result.JobId = crew?.JobId;
                }
                else
                {
                    result.GenerateEmployeeList();
                }
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
                    var entry = result.AddHolidayHoursEntry((DateTime)WorkDate, emp);
                    var perdiem = result.AddPerdiem((DateTime)WorkDate, emp);
                    entry.PerdiemLineNum = perdiem.LineNum;
                }
            }
        }
        #endregion

        public DTPayrollHour AddPayrollHour(PayrollEntry entry)
        {
            var hourEntry = new DTPayrollHour
            {
                EarnCode = entry.EarnCode,

                PRCo = entry.PRCo,
                Employee = entry.Employee,
                EmployeeId = (int)entry.EmployeeId,
                WorkDate = (DateTime)entry.PostDate,
                EarnCodeId = entry.Employee.EarnCodeId,
                EntryTypeId = entry.JobId != null ? (int)EntryTypeEnum.Job :
                                entry.EquipmentId != null ? (int)EntryTypeEnum.Equipment :
                                (int)EntryTypeEnum.Admin,

                DTCo = this.DTCo,
                TicketId = entry.udTicketId ?? this.TicketId,
                TicketLineNum = (int)entry.udTicketLineId,

                Job = entry.JCJob,
                JCCo = entry.JCCo,
                JobId = entry.JobId,
                PhaseGroupId = entry.PhaseGroupId ?? entry.JCJob?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId,
                PhaseId = entry.PhaseId,

                Equipment = entry.EMEquipment,
                EMCo = entry.EMCo,
                EquipmentId = entry.EquipmentId,

                Hours = entry.Hours,
                //Amt = entry.Amt,
                Rate = entry.Rate,

                ModifiedOn = this.ModifiedOn,
                ModifiedBy = this.ModifiedBy,
                Status = (int)PayrollEntryStatusEnum.Accepted
            };

           
            return hourEntry;
        }

        public DailyEmployeeEntry AddHoursEntry()
        {
            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = WorkDate,
                tEntryTypeId = (int)EntryTypeEnum.Admin,
                ModifiedBy = db.CurrentUserId,
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
                tEntryTypeId = (int)EntryTypeEnum.Admin,
                ModifiedBy = db.CurrentUserId,
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
                tEntryTypeId = (int)EntryTypeEnum.Admin,
                ModifiedBy = db.CurrentUserId,
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
                PerDiemId = (int)PerdiemEnum.No,
                tWorkDate = WorkDate,
                ModifiedBy = db.CurrentUserId,
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
                    PerDiemId = (int)PerdiemEnum.No,
                    tWorkDate = workDate,
                    ModifiedBy = db.CurrentUserId,
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
                ModifiedBy = db.CurrentUserId,
                ModifiedOn = DateTime.Now,
                PRCo = leaveLine.PRCo,
                tEmployeeId = leaveLine.EmployeeId,
                PerDiemId = (int)PerdiemEnum.No,
                PREmployee = leaveLine.Employee,
                DailyTicket = this,
            };
            DailyEmployeePerdiems.Add(entry);


            return entry;
        }

        public static int GetNextTicketId()
        {
            using var db = new VPContext();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

            var result = db.udNextTicketId(outParm);

            return (int)outParm.Value;
        }


        public string GetSharePointRootFolderPath()
        {
            string path = "";
            if (this.FormType == DTFormEnum.JobFieldTicket)
            {
                var job = this.DailyJobTicket.Job;
                if (job != null)
                {
                    path = job.GetSharePointRootFolderPath();
                    path = string.Format("{0}/DailyTickets/{1}", path, this.TicketId);
                }
            }
            return path;
        }

        public SPList GetSharePointList()
        {
            if (this.FormType == DTFormEnum.JobFieldTicket)
            {
                var job = this.DailyJobTicket.Job;
                if (job != null)
                    return job.GetSharePointList();
            }
            return null;
        }
    }
}
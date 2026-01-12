using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyJobEmployee
    {
        private VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.DailyTicket != null)
                    db = this.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
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
                    tWorkDate = value;
                }
            }
        }

        public decimal PayrollHours
        {
            get
            {
                return (HoursAdj ?? Hours) ?? 0;
            }
        }

        public short PayrolPerdiem
        {
            get
            {
                var perdiemAdj = (short?)PerdiemAdj;

                return (perdiemAdj ?? Perdiem) ?? 0;
            }
        }

        public int? EmployeeId
        {
            get
            {
                return tEmployeeId;
            }
            set
            {
                if (value != tEmployeeId)
                {
                    UpdateEmployeeInfo(value);
                }
            }
        }

        private void UpdateEmployeeInfo(int? newVal)
        {
            var origEmployeeId = tEmployeeId;


            if (PREmployee == null && tEmployeeId != null)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == tEmployeeId);
                tEmployeeId = employee.EmployeeId;
                PRCo = employee.PRCo;
                PREmployee = employee;
            }

            if (tEmployeeId != newVal)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == newVal);
                if (employee != null)
                {
                    PRCo = employee.PRCo;
                    tEmployeeId = employee.EmployeeId;
                    EarnCodeId = employee.EarnCodeId;
                    PREmployee = employee;
                }
                else
                {
                    PRCo = null;
                    tEmployeeId = null;
                    PREmployee = null;
                    EarnCodeId = null;
                }
            }
        }

        public void GenerateDTPayrollEntries()
        {
            var tasks = DailyTicket.DailyJobTicket.PayrollHourTasks();
            var taskTotHours = tasks.Sum(s => s.PayrollValue);
            var runningTotal = 0m;
            var hours = DailyTicket.DTPayrollHours.ToList();
            var perdeims = DailyTicket.DTPayrollPerdiems.ToList();

            foreach (var task in tasks)
            {
                var perdiemEntry = InitDTPayrollPerdiem(task);
                var hourEntry = InitDTPayrollHourEntry(task);
                if (taskTotHours != 0)
                {
                    if (taskTotHours != PayrollHours)
                        hourEntry.Hours = PayrollHours * (task.PayrollValue / taskTotHours);
                }
                hourEntry.Hours = Math.Round(hourEntry.Hours ?? 0, 2);
                runningTotal += hourEntry.Hours ?? 0M;

                hours.Add(hourEntry);
                perdeims.Add(perdiemEntry);
            }

            if (!tasks.Any())
            {
                var hourEntry = InitDTPayrollHourEntry();
                hours.Add(hourEntry);

                if (PayrolPerdiem != 0)
                {
                    var perdiemEntry = InitDTPayrollPerdiem();
                    perdeims.Add(perdiemEntry);
                }
            }
            if (runningTotal != PayrollHours)
            {
                var hourEntry = hours.FirstOrDefault(f => f.EmployeeId == EmployeeId);
                hourEntry.Hours += Hours - runningTotal;
            }
            if (hours.Any())
                hours.ForEach(e => DailyTicket.DTPayrollHours.Add(e));

            if (perdeims.Any())
                perdeims.ForEach(e => DailyTicket.DTPayrollPerdiems.Add(e));

            //var db = VPEntities.GetDbContextFromEntity(this);
            //db.SaveChanges();
            if (PREmployee.CrewId != DailyTicket.DailyJobTicket.CrewId)
                PREmployee.CrewId = DailyTicket.DailyJobTicket.CrewId;
        }

        public DTPayrollPerdiem InitDTPayrollPerdiem()
        {
            var entry = new DTPayrollPerdiem
            {
                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                //Employee = PREmployee,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,


                WorkDate = (DateTime)WorkDate,
                DTCo = DTCo,
                TicketId = TicketId,
                TicketLineNum = LineNum,
                //DailyTicket = DailyTicket,

                EarnCodeId = 20,

                //Job = DailyTicket.DailyJobTicket.Job,
                JCCo = DailyTicket.DailyJobTicket.Job.JCCo,
                JobId = DailyTicket.DailyJobTicket.Job.JobId,
                PhaseGroupId = DailyTicket.DailyJobTicket.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = "   100-  -",

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted
            };

            return entry;
        }

        public DTPayrollPerdiem InitDTPayrollPerdiem(DNU_DailyJobTask task)
        {
            //var lineNum = DailyTicket.DTPayrollPerdiems.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollPerdiem
            {
                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,
                //LineNum = lineNum,

                //Employee = PREmployee,
                EarnCodeId = 20,

                DTCo = task.DTCo,
                TicketId = task.TicketId,
                TicketLineNum = LineNum,
                //DailyTicket = DailyTicket,

                //Job = task.Job,
                JCCo = task.Job.JCCo,
                JobId = task.Job.JobId,
                PhaseGroupId = task.PhaseGroupId,
                PhaseId = task.PhaseId,

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Status = DB.PayrollEntryStatusEnum.Accepted
            };

            if (task.Job.JobType == DB.JCJobTypeEnum.ShopYard)
            {
                entry.Job = null;
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
            }
            return entry;
        }

        public DTPayrollPerdiem InitDTPayrollPerdiem(DTJobPhaseCost task)
        {
            //var lineNum = DailyTicket.DTPayrollPerdiems.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollPerdiem
            {
                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,
                //LineNum = lineNum,

                //Employee = PREmployee,
                EarnCodeId = 20,

                DTCo = task.DTCo,
                TicketId = task.TicketId,
                TicketLineNum = LineNum,
                //DailyTicket = DailyTicket,

                //Job = task.Job,
                JCCo = task.Job.JCCo,
                JobId = task.Job.JobId,
                PhaseGroupId = task.PhaseGroupId,
                PhaseId = task.PhaseId,

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Status = DB.PayrollEntryStatusEnum.Accepted
            };

            if (task.Job.JobType == DB.JCJobTypeEnum.ShopYard)
            {
                entry.Job = null;
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
            }
            return entry;
        }

        public DTPayrollHour InitDTPayrollHourEntry(DNU_DailyJobTask task)
        {
            if (task == null)
                return null;

            //var lineNum = DailyTicket.DTPayrollHours.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollHour()
            {
                //DailyTicket = DailyTicket,
                //Employee = PREmployee,
                //Job = DailyTicket.DailyJobTicket.Job,
                //JobPhase = task.JobPhase,

                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,

                EarnCodeId = EarnCodeId,

                //LineNum = lineNum,
                DTCo = task.DTCo,
                TicketId = task.TicketId,
                TicketLineNum = LineNum,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,

                Hours = task.Value,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted,

                JCCo = task.Job.JCCo,
                JobId = task.Job.JobId,
                PhaseGroupId = task.PhaseGroupId ?? task.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = task.JobId != null ? task.PhaseId : null,

            };

            if (task.Job.JobType == DB.JCJobTypeEnum.ShopYard)
            {
                entry.Job = null;
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
            }

            if (entry.EarnCodeId != PREmployee.EarnCodeId)
            {
                EarnCodeId = PREmployee.EarnCodeId;
                entry.EarnCodeId = PREmployee.EarnCodeId;
            }

            return entry;
        }

        public DTPayrollHour InitDTPayrollHourEntry(DTJobPhaseCost task)
        {
            if (task == null)
                return null;

            //var lineNum = DailyTicket.DTPayrollHours.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollHour()
            {
                //DailyTicket = DailyTicket,
                //Employee = PREmployee,
                //Job = DailyTicket.DailyJobTicket.Job,
                //JobPhase = task.JobPhase,

                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,

                EarnCodeId = EarnCodeId,

                //LineNum = lineNum,
                DTCo = task.DTCo,
                TicketId = task.TicketId,
                TicketLineNum = LineNum,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,

                Hours = task.Value,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted,

                JCCo = task.Job.JCCo,
                JobId = task.Job.JobId,
                PhaseGroupId = task.PhaseGroupId ?? task.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = task.JobId != null ? task.PhaseId : null,

            };

            if (task.Job.JobType == DB.JCJobTypeEnum.ShopYard)
            {
                entry.Job = null;
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
            }

            if (entry.EarnCodeId != PREmployee.EarnCodeId)
            {
                EarnCodeId = PREmployee.EarnCodeId;
                entry.EarnCodeId = PREmployee.EarnCodeId;
            }

            return entry;
        }

        public DTPayrollHour InitDTPayrollHourEntry()
        {
            //var lineNum = DailyTicket.DTPayrollHours.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollHour()
            {
                //DailyTicket = DailyTicket,
                //Employee = PREmployee,
                //Job = DailyTicket.DailyJobTicket.Job,
                JCCo = DailyTicket.DailyJobTicket.Job.JCCo,
                JobId = DailyTicket.DailyJobTicket.Job.JobId,
                PhaseGroupId = DailyTicket.DailyJobTicket.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = "   100-  -",

                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,

                EarnCodeId = EarnCodeId,

                //LineNum = lineNum,
                DTCo = DTCo,
                TicketId = TicketId,
                TicketLineNum = LineNum,
                EntryTypeId = (int)DB.EntryTypeEnum.Job,

                Hours = 0,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted,

            };

            if (entry.EarnCodeId != PREmployee.EarnCodeId)
            {
                EarnCodeId = PREmployee.EarnCodeId;
                entry.EarnCodeId = PREmployee.EarnCodeId;
            }

            return entry;
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyJobEmployeeViewModel model)
        {
            if (model == null)
                return;

            EmployeeId = model.EmployeeId;
            Hours = model.Hours;
            Perdiem = (short?)model.Perdiem;
            Comments = model.Comments;
        }
    }
}
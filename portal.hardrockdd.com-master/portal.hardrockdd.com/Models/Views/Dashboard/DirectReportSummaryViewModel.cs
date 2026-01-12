using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Dashboard
{
    public class ProcessBatchEmployeeViewModel
    {
        public ProcessBatchEmployeeViewModel()
        {

        }

        [Key]
        public byte PRCo { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [Key]
        public int WeekId { get; set; }
    }

    public class DirectReportSummaryListViewModel
    {
        public DirectReportSummaryListViewModel()
        {

        }

        public DirectReportSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DB.Infrastructure.ViewPointDB.Data.VPContext db, int weekId)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }

            #region mapping
            PRCo = (byte)company.PRCo;
            WeekId = weekId;
            #endregion

            //var dateFilter = DateTime.Now.AddDays(-7).Date;
            var calendar = db.Calendars.Where(w => w.Week == weekId).ToList();
            var lastDate = calendar.Max(max => max.Date);
            var userId = StaticFunctions.GetUserId();
            var superId = db.HRResources.FirstOrDefault(f => f.WebId == userId);
            var hourList = db.DTPayrollHours.Where(f => f.PRCo == company.PRCo &&
                                                               f.Calendar.Week >= weekId && 
                                                               f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted &&
                                                               f.Employee.Supervisor.EmployeeId == superId.PREmp)
                                                    .GroupBy(g => new { g.PRCo, g.Employee, g.WorkDate })
                                                    .Select(s => new {
                                                        s.Key.PRCo,
                                                        s.Key.Employee,
                                                        s.Key.WorkDate,
                                                        Hours = s.Sum(sum => (sum.HoursAdj ?? sum.Hours) ?? 0),
                                                        List = s.ToList()
                                                    });

            var perdiemList = db.DTPayrollPerdiems.Where(f => f.PRCo == company.PRCo && f.Calendar.Week >= weekId &&
                                                                     f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted &&
                                                                     f.Employee.Supervisor.EmployeeId == superId.PREmp)
                                                    .GroupBy(g => new { g.PRCo, g.Employee, g.WorkDate })
                                                    .Select(s => new {
                                                        s.Key.PRCo,
                                                        s.Key.Employee,
                                                        s.Key.WorkDate,
                                                        PerdiemId = s.Max(m => (m.PerdiemIdAdj ?? m.PerdiemId) ?? 0),
                                                        List = s.ToList()
                                                    });

            var hoursEntryList = db.DailyTickets.Where(f =>  (f.tStatusId ==  (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft) && f.Calendar.Week >= weekId)
                                                     .ToList()
                                                     .SelectMany(entries => entries.DailyEmployeeEntries)
                                                     .Where(f => f.PREmployee?.Supervisor?.EmployeeId == superId.PREmp)
                                                     .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                     .Select(s => new {
                                                         s.Key.PRCo,
                                                         s.Key.PREmployee,
                                                         s.Key.WorkDate,
                                                         Hours = s.Sum(m => (m.ValueAdj ?? m.Value) ?? 0),
                                                         List = s.ToList()
                                                     });

            var PerdiemEntryList = db.DailyTickets.Where(f =>  (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft) && f.Calendar.Week >= weekId)
                                                     .ToList()
                                                     .SelectMany(entries => entries.DailyEmployeePerdiems)
                                                     .Where(f => f.PREmployee?.Supervisor?.EmployeeId == superId.PREmp)
                                                     .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                     .Select(s => new {
                                                         s.Key.PRCo,
                                                         s.Key.PREmployee,
                                                         s.Key.WorkDate,
                                                         PerdiemId = s.Max(m => (m.PerDiemIdAdj ?? m.PerDiemId) ?? 0),
                                                         List = s.ToList()
                                                     });

            var jobEntries = db.DailyTickets.Where(f =>  (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft) && f.Calendar.Week >= weekId)
                                                     .ToList()
                                                     .SelectMany(entries => entries.DailyJobEmployees)
                                                     .Where(f => f.PREmployee?.Supervisor?.EmployeeId == superId.PREmp)
                                                     .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                     .Select(s => new {
                                                         s.Key.PRCo,
                                                         s.Key.PREmployee,
                                                         s.Key.WorkDate,
                                                         Hours = s.Sum(m => (m.HoursAdj ?? m.Hours) ?? 0),
                                                         PerdiemId = s.Max(m => ((short?)m.PerdiemAdj ?? m.Perdiem) ?? 0),
                                                         List = s.ToList()
                                                     });

            List = new List<DirectReportSummaryViewModel>();
            var employeeList = db.Employees.Where(f => f.PRCo == company.HQCo && f.ActiveYN == "Y" && f.ReportsToId == superId.PREmp).ToList();
            foreach (var employee in employeeList)
            {
                foreach (var day in calendar)
                {
                    var item = new DirectReportSummaryViewModel(employee, day);
                    List.Add(item);
                }
            }

            foreach (var hour in hourList)
            {
                var item = List.FirstOrDefault(f => f.PRCo == hour.PRCo && f.EmployeeId == hour.Employee.EmployeeId && f.WorkDate == hour.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                if (item == null)
                {
                    item = new DirectReportSummaryViewModel(hour.List);
                    List.Add(item);
                }
                item.Hour = hour.Hours;
                item.Status = DB.EmployeeEntryStatusEnum.Approved;
            }

            foreach (var perdiem in perdiemList)
            {
                var item = List.FirstOrDefault(f => f.PRCo == perdiem.PRCo && f.EmployeeId == perdiem.Employee.EmployeeId && f.WorkDate == perdiem.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                if (item == null)
                {
                    item = new DirectReportSummaryViewModel(perdiem.List);
                    List.Add(item);
                }
                item.PerdiemId = (DB.PerdiemEnum)perdiem.PerdiemId;
                item.Status = DB.EmployeeEntryStatusEnum.Approved;
            }

            foreach (var hour in hoursEntryList.Where(f => f.PREmployee != null).ToList())
            {
                var item = List.FirstOrDefault(f => f.PRCo == hour.PRCo && f.EmployeeId == hour.PREmployee.EmployeeId && f.WorkDate == hour.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                if (item == null)
                {
                    item = new DirectReportSummaryViewModel(hour.List);
                    List.Add(item);
                }
                item.Hour = hour.Hours;
                item.Status = DB.EmployeeEntryStatusEnum.Pending;
            }

            foreach (var perdiem in PerdiemEntryList.Where(f => f.PREmployee != null).ToList())
            {
                var item = List.FirstOrDefault(f => f.PRCo == perdiem.PRCo && f.EmployeeId == perdiem.PREmployee.EmployeeId && f.WorkDate == perdiem.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                if (item == null)
                {
                    item = new DirectReportSummaryViewModel(perdiem.List);
                    List.Add(item);
                }
                item.PerdiemId = (DB.PerdiemEnum)perdiem.PerdiemId;
                item.Status = DB.EmployeeEntryStatusEnum.Pending;
            }

            foreach (var job in jobEntries.Where(f => f.PREmployee != null).ToList())
            {
                var item = List.FirstOrDefault(f => f.PRCo == job.PRCo && f.EmployeeId == job.PREmployee.EmployeeId && f.WorkDate == job.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                if (item == null)
                {
                    item = new DirectReportSummaryViewModel(job.List);
                    List.Add(item);
                }
                else
                {
                    item.Hour = (item.Hour ?? 0) + job.Hours;
                    item.PerdiemId = (DB.PerdiemEnum)job.PerdiemId > (item.PerdiemId ?? 0) ? (DB.PerdiemEnum)job.PerdiemId : item.PerdiemId;
                }
                item.Status = DB.EmployeeEntryStatusEnum.Pending;
            }

            foreach (var empWeek in List.Where(f => f.PREndDate <= lastDate)
                                        .GroupBy(g => new { g.EmployeeId, g.PREndDate })
                                        .Select(s => new {
                                            s.Key.EmployeeId,
                                            s.Key.PREndDate,
                                            MaxDate = s.Max(max => max.WorkDate).WeekEndDate(DayOfWeek.Saturday),
                                            MinDate = s.Min(min => min.WorkDate).WeekStartDate(DayOfWeek.Saturday)
                                        }).ToList())
            {
                var postedEntries = db.PayrollEntries.Where(f => f.PRCo == company.HQCo &&
                                                                 f.EmployeeId == empWeek.EmployeeId &&
                                                                 f.PostDate <= empWeek.MaxDate &&
                                                                 f.PostDate >= empWeek.MinDate
                                                                 )
                                                    .GroupBy(g => new { g.PRCo, g.Employee, g.PostDate })
                                                    .Select(s => new {
                                                        PRCo = s.Key.PRCo,
                                                        s.Key.Employee,
                                                        WorkDate = s.Key.PostDate,
                                                        List = s.ToList()
                                                    }).ToList();

                foreach (var entry in postedEntries)
                {
                    var item = List.FirstOrDefault(f => f.PRCo == entry.PRCo && f.EmployeeId == entry.Employee.EmployeeId && f.WorkDate == entry.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new DirectReportSummaryViewModel(entry.List);
                        List.Add(item);
                    }
                    else
                    {
                        var tempItem = new DirectReportSummaryViewModel(entry.List);
                        item.Hour = tempItem.Hour;
                        item.PerdiemId = tempItem.PerdiemId;
                        item.Status = DB.EmployeeEntryStatusEnum.Posted;
                    }
                }
            }
            foreach (var empWeek in List.Where(f => f.PREndDate <= lastDate)
                                        .GroupBy(g => new { g.EmployeeId, g.PREndDate })
                                        .Select(s => new {
                                            s.Key.EmployeeId,
                                            s.Key.PREndDate,
                                            MaxDate = s.Max(max => max.WorkDate).WeekEndDate(DayOfWeek.Saturday),
                                            MinDate = s.Min(min => min.WorkDate).WeekStartDate(DayOfWeek.Saturday)
                                        }).ToList())
            {
                var postedEntries = db.PRBatchTimeEntries.Where(f => f.Co == company.PRCo &&
                                                                 f.EmployeeId == empWeek.EmployeeId &&
                                                                 f.PostDate <= empWeek.MaxDate &&
                                                                 f.PostDate >= empWeek.MinDate
                                                                 )
                                                    .GroupBy(g => new { g.Co, g.Employee, g.PostDate })
                                                    .Select(s => new {
                                                        PRCo = s.Key.Co,
                                                        s.Key.Employee,
                                                        WorkDate = s.Key.PostDate,
                                                        List = s.ToList()
                                                    }).ToList();

                foreach (var entry in postedEntries)
                {
                    var item = List.FirstOrDefault(f => f.PRCo == entry.PRCo && f.EmployeeId == entry.Employee.EmployeeId && f.WorkDate == entry.WorkDate && f.Status == DB.EmployeeEntryStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new DirectReportSummaryViewModel(entry.List);
                        List.Add(item);
                    }
                    else
                    {
                        var tempItem = new DirectReportSummaryViewModel(entry.List);
                        item.Hour = tempItem.Hour;
                        item.PerdiemId = tempItem.PerdiemId;
                        item.Status = DB.EmployeeEntryStatusEnum.Posted;
                    }
                }
            }

            foreach (var empWeek in List.Where(f => f.PREndDate <= lastDate)
                                        .GroupBy(g => new { g.EmployeeId, g.PREndDate })
                                        .Select(s => new {
                                            s.Key.EmployeeId,
                                            s.Key.PREndDate,
                                            UnApprovedCnt = s.Where(f => f.Status == DB.EmployeeEntryStatusEnum.Pending).Count(),
                                            ApprovedCnt = s.Where(f => f.Status == DB.EmployeeEntryStatusEnum.Approved).Count(),
                                            PostedCnt = s.Where(f => f.Status == DB.EmployeeEntryStatusEnum.Posted).Count(),
                                            List = s
                                        }).ToList())
            {
                var status = DB.EmployeeEntryStatusEnum.Pending;
                if (empWeek.PostedCnt != 0)
                {
                    status = DB.EmployeeEntryStatusEnum.Posted;
                }
                else if (empWeek.ApprovedCnt != 0)
                {
                    status = DB.EmployeeEntryStatusEnum.Approved;
                }
                else if (empWeek.UnApprovedCnt != 0)
                {
                    status = DB.EmployeeEntryStatusEnum.Pending;
                }
                foreach (var item in empWeek.List.Where(f => f.Status == DB.EmployeeEntryStatusEnum.Empty).ToList())
                {
                    item.Status = status;
                }
            }

        }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        public List<DirectReportSummaryViewModel> List { get; }
    }

    public class DirectReportSummaryViewModel
    {
        public DirectReportSummaryViewModel()
        {

        }

        public DirectReportSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DB.Infrastructure.ViewPointDB.Data.Calendar calendar)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            if (calendar == null)
            {
                throw new System.ArgumentNullException(nameof(calendar));
            }

            var resource = employee.Resource.FirstOrDefault();

            #region mapping
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", employee.FirstName, employee.LastName, employee.Suffix);
            if (resource != null)
            {
                Postion = resource.Position?.Description ?? "";
            }
            WorkDate = calendar.Date;
            WeekId = (int)calendar.Week;
            PREndDate = calendar.Date.WeekEndDate(DayOfWeek.Saturday);
            Status = DB.EmployeeEntryStatusEnum.Empty;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.PRBatchTimeEntry> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }
            var entry = entries.FirstOrDefault();
            var resource = entry.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = entry.Co;
            EmployeeId = entry.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", entry.Employee.FirstName, entry.Employee.LastName, entry.Employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = entry.PostDate;
            WeekId = (int)entry.Calendar.Week;
            PREndDate = entry.PostDate.WeekEndDate(DayOfWeek.Saturday);
            Hour = entries.Where(w => w.EarnCode.Method == "A" || w.EarnCode.Method == "H").Sum(sum => sum.Hours);
            var PerdiemAmt = entries.Where(w => w.tEarnCodeId == 20 || w.tEarnCodeId == 21).Sum(sum => sum.Amt);
            if (PerdiemAmt > 25)
                PerdiemId = DB.PerdiemEnum.Yes;
            else if (PerdiemAmt == 25)
                PerdiemId = DB.PerdiemEnum.Cabin;
            else if (PerdiemAmt == 0)
                PerdiemId = DB.PerdiemEnum.No;
            //Comments = entry.Comments;
            Status = DB.EmployeeEntryStatusEnum.Posted;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.PayrollEntry> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }
            var entry = entries.FirstOrDefault();
            var resource = entry.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = entry.PRCo;
            EmployeeId = entry.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", entry.Employee.FirstName, entry.Employee.LastName, entry.Employee.Suffix);
            if (resource != null)
                Postion = resource.Position?.Description ?? "";
            WorkDate = entry.PostDate;
            WeekId = (int)entry.Calendar.Week;
            PREndDate = entry.PostDate.WeekEndDate(DayOfWeek.Saturday);
            Hour = entries.Where(w => w.EarnCode.Method == "A" || w.EarnCode.Method == "H").Sum(sum => sum.Hours);
            var PerdiemAmt = entries.Where(w => w.EarnCodeId == 20 || w.EarnCodeId == 21).Sum(sum => sum.Amt);
            if (PerdiemAmt > 25)
                PerdiemId = DB.PerdiemEnum.Yes;
            else if (PerdiemAmt == 25)
                PerdiemId = DB.PerdiemEnum.Cabin;
            else if (PerdiemAmt == 0)
                PerdiemId = DB.PerdiemEnum.No;
            //Comments = entry.Comments;
            Status = DB.EmployeeEntryStatusEnum.Posted;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollHour> hours)
        {
            if (hours == null)
            {
                throw new System.ArgumentNullException(nameof(hours));
            }
            var hour = hours.FirstOrDefault();
            var resource = hour.Employee.Resource.FirstOrDefault();

            #region mapping
            PRCo = hour.PRCo;
            EmployeeId = hour.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", hour.Employee.FirstName, hour.Employee.LastName, hour.Employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = hour.WorkDate;
            WeekId = (int)hour.Calendar.Week;
            PREndDate = hour.WorkDate.WeekEndDate(DayOfWeek.Saturday);
            Hour = hours.Sum(s => s.HoursAdj ?? s.Hours);
            PerdiemId = DB.PerdiemEnum.No;
            Comments = hour.Comments;
            Status = DB.EmployeeEntryStatusEnum.Approved;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollPerdiem> perdiems)
        {
            if (perdiems == null)
            {
                throw new System.ArgumentNullException(nameof(perdiems));
            }

            var perdiem = perdiems.FirstOrDefault();
            var resource = perdiem.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = perdiem.PRCo;
            EmployeeId = perdiem.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", perdiem.Employee.FirstName, perdiem.Employee.LastName, perdiem.Employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = perdiem.WorkDate;
            WeekId = (int)perdiem.Calendar.Week;
            PREndDate = perdiem.WorkDate.WeekEndDate(DayOfWeek.Saturday);
            if (perdiems != null)
            {
                PerdiemId = perdiems.Max(max => (DB.PerdiemEnum)((max.PerdiemIdAdj ?? max.PerdiemId) ?? 0));
            }
            else
            {
                PerdiemId = DB.PerdiemEnum.No;
            }
            Comments = perdiem.Comments;
            Status = DB.EmployeeEntryStatusEnum.Approved;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry> hours)
        {
            if (hours == null)
                return;



            var hour = hours.FirstOrDefault();
            var resource = hour.PREmployee.Resource.FirstOrDefault();
            #region mapping
            PRCo = hour.PRCo ?? 0;
            EmployeeId = (int)hour.EmployeeId;
            EmployeeName = hour.PREmployee.FullName;
            Postion = resource.Position?.Description ?? "";
            WorkDate = (DateTime)hour.WorkDate;
            WeekId = (int)hour.Calendar.Week;
            PREndDate = WorkDate.WeekEndDate(DayOfWeek.Saturday);
            //Hour = hour.Value ?? 0;
            if (hour.EarnCode == null)
            {
                hour.EarnCode = new DB.Infrastructure.ViewPointDB.Data.EarnCode();
            }
            Hour = hours.Where(w => w.EarnCode?.Method == "A" || w.EarnCode?.Method == "H").Sum(sum => sum.ValueAdj ?? sum.Value);
            PerdiemId = DB.PerdiemEnum.No;
            Comments = hour.Comments;
            Status = DB.EmployeeEntryStatusEnum.Pending;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem> perdiems)
        {
            if (perdiems == null)
            {
                throw new System.ArgumentNullException(nameof(perdiems));
            }
            var perdiem = perdiems.FirstOrDefault();
            var resource = perdiem.PREmployee.Resource.FirstOrDefault();

            #region mapping
            PRCo = perdiem.PRCo ?? 0;
            EmployeeId = (int)perdiem.EmployeeId;
            EmployeeName = perdiem.PREmployee.FullName;
            Postion = resource.Position?.Description ?? "";
            WorkDate = (DateTime)perdiem.WorkDate;
            WeekId = (int)perdiem.Calendar.Week;
            PREndDate = WorkDate.WeekEndDate(DayOfWeek.Saturday);
            PerdiemId = perdiems.Max(max => (DB.PerdiemEnum)(max.PerDiemIdAdj ?? max.PerDiemId));
            Comments = perdiem.Comments;
            Status = DB.EmployeeEntryStatusEnum.Pending;
            #endregion
        }

        public DirectReportSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyJobEmployee> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }

            var entry = entries.FirstOrDefault();
            var resource = entry.PREmployee.Resource.FirstOrDefault();
            #region mapping
            PRCo = entry.PRCo ?? 0;
            EmployeeId = (int)entry.EmployeeId;
            EmployeeName = entry.PREmployee.FullName;
            Postion = resource.Position?.Description ?? "";
            WorkDate = (DateTime)entry.WorkDate;
            WeekId = (int)entry.Calendar.Week;
            PREndDate = WorkDate.WeekEndDate(DayOfWeek.Saturday);
            //Hour = entry.Hours ?? 0;
            //PerdiemId = (DB.PerdiemEnum)entry.Perdiem;
            PerdiemId = entries.Max(max => (DB.PerdiemEnum)((short?)(max.PerdiemAdj ?? max.Perdiem) ?? 0));
            Hour = entries.Sum(s => s.HoursAdj ?? s.Hours);
            Comments = entry.Comments;
            Status = DB.EmployeeEntryStatusEnum.Pending;
            #endregion
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte PRCo { get; set; }

        [Key]
        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Week Ending")]
        public DateTime PREndDate { get; set; }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int WeekId { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Display(Name = "Day")]
        public string DayDescription { get { return string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString()); } }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Required]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Hours")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Hour { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Perdiem")]
        [UIHint("EnumBox")]
        public DB.PerdiemEnum? PerdiemId { get; set; }


        [ReadOnly(true)]
        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.EmployeeEntryStatusEnum Status { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Comments")]
        [UIHint("TextBox")]
        public string Comments { get; set; }

        [Display(Name = "Postion")]
        [UIHint("TextBox")]
        public string Postion { get; set; }

        public dynamic DynaimicTicket { get; set; }
    }
}
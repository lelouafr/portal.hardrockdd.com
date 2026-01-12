using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll
{
    public class ProcessBatchEmployeeViewModel
    {
        public ProcessBatchEmployeeViewModel()
        {

        }

        [Key]
        public byte PRCo { get; set; }

        [Key]
        public int PayWeekId { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [Key]
        public int WeekId { get; set; }
    }

    public class PayrollReviewSummaryListViewModel
    {
        public PayrollReviewSummaryListViewModel()
        {


        }

        public PayrollReviewSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.VPContext db, DB.Infrastructure.ViewPointDB.Data.WebUser user)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            var emp = user.Employee.FirstOrDefault();
            List = new List<PayrollReviewSummaryViewModel>();
            var results = db.udPRTH_UserSummary(emp.PRCo, emp.HRRef).AsEnumerable().Select(s => new PayrollReviewSummaryViewModel(s)).ToList().OrderBy(o => o.WorkDate).ToList();
            List.AddRange(results);

            foreach (var empWeek in List.GroupBy(g => new { g.EmployeeId, g.PREndDate })
                                        .Select(s => new {
                                            s.Key.EmployeeId,
                                            s.Key.PREndDate,
                                            UnApprovedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Pending).Count(),
                                            ApprovedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Approved).Count(),
                                            PostedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Posted).Count(),
                                            OnHoldCnt = s.Where(f => f.PayrollStatus == DB.PayrollEntryStatusEnum.OnHold).Count(),
                                            List = s
                                        }).ToList())
            {
                var status = DB.PayrollReviewStatusEnum.Pending;
                if (empWeek.PostedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Posted;
                }
                else if (empWeek.ApprovedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Approved;
                }
                else if (empWeek.UnApprovedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Pending;
                }
                foreach (var item in empWeek.List.Where(f => f.Status == DB.PayrollReviewStatusEnum.Empty).ToList())
                {
                    item.Status = status;
                    if (empWeek.OnHoldCnt > 0)
                    {
                        item.PayrollStatus = DB.PayrollEntryStatusEnum.OnHold;
                    }
                }
            }
        }


        public PayrollReviewSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DB.Infrastructure.ViewPointDB.Data.VPContext db, int weekId, bool buildList = true)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            //Grab all Leave Request that are approved that do not have a ticket.


            var list = db.LeaveRequestLines.Where(f => f.Calendar.Week == WeekId && f.TicketId == null && f.Request.Status == 2).ToList();            
            RequestLineRepository.CreateTickets(weekId, db);
            db.SaveChanges();
            #region mapping
            PRCo = company.HQCo;
            WeekId = weekId;
            #endregion

            List = new List<PayrollReviewSummaryViewModel>();
            var calendar = db.Calendars.Where(w => w.Week == weekId).ToList();
            var lastDate = calendar.Max(max => max.Date);
            if (buildList)
            {

            //var dateFilter = DateTime.Now.AddDays(-7).Date;

                var hourList = db.DTPayrollHours.Where(f => f.Calendar.Week <= weekId && f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted)
                                                        //.Where(f => f.EmployeeId == 100865)
                                                        .GroupBy(g => new { g.PRCo, g.Employee, g.WorkDate })
                                                        .Select(s => new {
                                                            s.Key.PRCo,
                                                            s.Key.Employee,
                                                            s.Key.WorkDate,
                                                            Hours = s.Sum(sum => (sum.HoursAdj ?? sum.Hours) ?? 0),
                                                            List = s.ToList()
                                                        });
                var perdiemList = db.DTPayrollPerdiems.Where(f => f.Calendar.Week <= weekId && f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted)
                                                        //.Where(f => f.EmployeeId == 100865)
                                                        .GroupBy(g => new { g.PRCo, g.Employee, g.WorkDate })
                                                        .Select(s => new {
                                                            s.Key.PRCo,
                                                            s.Key.Employee,
                                                            s.Key.WorkDate,
                                                            PerdiemId = s.Max(m => (m.PerdiemIdAdj ?? m.PerdiemId) ?? 0),
                                                            List = s.ToList()
                                                        });

                var hoursEntryList = db.DailyTickets.Where(f => (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft))
                                                         .SelectMany(entries => entries.DailyEmployeeEntries)
                                                         .Where(f => f.Calendar.Week == weekId)
                                                         //.Where(f => f.EmployeeId == 100865)
                                                         .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                         .Select(s => new {
                                                             s.Key.PRCo,
                                                             s.Key.PREmployee,
                                                             s.Key.WorkDate,
                                                             Hours = s.Sum(m => (m.ValueAdj ?? m.Value) ?? 0),
                                                             List = s.ToList()
                                                         });
                var PerdiemEntryList = db.DailyTickets.Where(f => (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft))
                                                         .SelectMany(entries => entries.DailyEmployeePerdiems)
                                                         .Where(f => f.Calendar.Week == weekId)
                                                         //.Where(f => f.EmployeeId == 100865)
                                                         .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                         .Select(s => new {
                                                             s.Key.PRCo,
                                                             s.Key.PREmployee,
                                                             s.Key.WorkDate,
                                                             PerdiemId = s.Max(m => (m.PerDiemIdAdj ?? m.PerDiemId) ?? 0),
                                                             List = s.ToList()
                                                         });

                var jobEntries = db.DailyTickets.Where(f =>  (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft))
                                                         .SelectMany(entries => entries.DailyJobEmployees)
                                                         .Where(f => f.Calendar.Week == weekId)
                                                         //.Where(f => f.EmployeeId == 100865)
                                                         .GroupBy(g => new { g.PRCo, g.PREmployee, g.WorkDate })
                                                         .Select(s => new {
                                                             s.Key.PRCo,
                                                             s.Key.PREmployee,
                                                             s.Key.WorkDate,
                                                             Hours = s.Sum(m => (m.HoursAdj ?? m.Hours) ?? 0),
                                                             PerdiemId = s.Max(m => ((short?)m.PerdiemAdj ?? m.Perdiem) ?? 0),
                                                             List = s.ToList()
                                                         });



                foreach (var employee in db.Employees.Where(f => f.PRCo == company.HQCo && f.ActiveYN == "Y")
                                                        //.Where(f => f.EmployeeId == 100865)
                                                        .ToList())
                {
                    foreach (var day in calendar)
                    {
                        var item = new PayrollReviewSummaryViewModel(employee, day);
                        List.Add(item);
                    }
                }

                foreach (var hour in hourList)
                {
                    var item = List.FirstOrDefault(f => f.PRCo == hour.PRCo && f.EmployeeId == hour.Employee.EmployeeId && f.WorkDate == hour.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new PayrollReviewSummaryViewModel(hour.List);
                        List.Add(item);
                    }
                    item.Hour = hour.Hours;
                    item.Status = DB.PayrollReviewStatusEnum.Approved;
                    item.PayrollStatus = (DB.PayrollEntryStatusEnum)(hour.List.Max(f => f.Status));
                }

                foreach (var perdiem in perdiemList)
                {
                    var item = List.FirstOrDefault(f => f.PRCo == perdiem.PRCo && f.EmployeeId == perdiem.Employee.EmployeeId && f.WorkDate == perdiem.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new PayrollReviewSummaryViewModel(perdiem.List);
                        List.Add(item);
                    }
                    item.PerdiemId = (DB.PerdiemEnum)perdiem.PerdiemId;
                    item.Status = DB.PayrollReviewStatusEnum.Approved;
                    item.PayrollStatus = (DB.PayrollEntryStatusEnum)(perdiem.List.Max(f => f.Status));
                }

                foreach (var hour in hoursEntryList.Where(f => f.PREmployee != null).ToList())
                {
                    var item = List.FirstOrDefault(f => f.PRCo == hour.PRCo && f.EmployeeId == hour.PREmployee.EmployeeId && f.WorkDate == hour.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new PayrollReviewSummaryViewModel(hour.List);
                        List.Add(item);
                    }
                    item.Hour = hour.Hours;
                    item.Status = DB.PayrollReviewStatusEnum.Pending;
                }

                foreach (var perdiem in PerdiemEntryList.Where(f => f.PREmployee != null).ToList())
                {
                    var item = List.FirstOrDefault(f => f.PRCo == perdiem.PRCo && f.EmployeeId == perdiem.PREmployee.EmployeeId && f.WorkDate == perdiem.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new PayrollReviewSummaryViewModel(perdiem.List);
                        List.Add(item);
                    }
                    item.PerdiemId = (DB.PerdiemEnum)perdiem.PerdiemId;
                    item.Status = DB.PayrollReviewStatusEnum.Pending;
                }

                foreach (var job in jobEntries.Where(f => f.PREmployee != null).ToList())
                {
                    var item = List.FirstOrDefault(f => f.PRCo == job.PRCo && f.EmployeeId == job.PREmployee.EmployeeId && f.WorkDate == job.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                    if (item == null)
                    {
                        item = new PayrollReviewSummaryViewModel(job.List);
                        List.Add(item);
                    }
                    else
                    {
                        item.Hour = (item.Hour ?? 0) + job.Hours;
                        item.PerdiemId = (DB.PerdiemEnum)job.PerdiemId > (item.PerdiemId ?? 0) ? (DB.PerdiemEnum)job.PerdiemId : item.PerdiemId;
                    }
                    item.Status = DB.PayrollReviewStatusEnum.Pending;
                }

                //var minDate = List.Min(min => min.PREndDate).WeekStartDate(DayOfWeek.Saturday);
                //var minWeekId = db.Calendars.FirstOrDefault(f => f.Date == minDate).Week;

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
                        var item = List.FirstOrDefault(f => f.PRCo == entry.PRCo && f.EmployeeId == entry.Employee.EmployeeId && f.WorkDate == entry.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                        if (item == null)
                        {
                            item = new PayrollReviewSummaryViewModel(entry.List);
                            List.Add(item);
                        }
                        else
                        {
                            var tempItem = new PayrollReviewSummaryViewModel(entry.List);
                            item.Hour = tempItem.Hour;
                            item.PerdiemId = tempItem.PerdiemId;
                            item.Status = DB.PayrollReviewStatusEnum.Posted;
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
                    var postedEntries = db.PRBatchTimeEntries.Where(f => f.Co == company.HQCo &&
                                                                     f.EmployeeId == empWeek.EmployeeId &&
                                                                     f.PostDate <= empWeek.MaxDate &&
                                                                     f.PostDate >= empWeek.MinDate
                                                                     )
                                                        .GroupBy(g => new { g.Co, g.Employee, g.PostDate })
                                                        .Select(s => new {
                                                            PRCo =s.Key.Co,
                                                            s.Key.Employee,
                                                            WorkDate = s.Key.PostDate,
                                                            List = s.ToList()
                                                        }).ToList();

                    foreach (var entry in postedEntries)
                    {
                        var item = List.FirstOrDefault(f => f.PRCo == entry.PRCo && f.EmployeeId == entry.Employee.EmployeeId && f.WorkDate == entry.WorkDate && f.Status == DB.PayrollReviewStatusEnum.Empty);
                        if (item == null)
                        {
                            item = new PayrollReviewSummaryViewModel(entry.List);
                            List.Add(item);
                        }
                        else
                        {
                            var tempItem = new PayrollReviewSummaryViewModel(entry.List);
                            item.Hour = tempItem.Hour;
                            item.PerdiemId = tempItem.PerdiemId;
                            item.Status = DB.PayrollReviewStatusEnum.Posted;
                        }
                    }
                }
            }
            else
            {
                var results = db.udPRTH_PayrollSummary(PRCo, weekId).AsEnumerable().Select(s => new PayrollReviewSummaryViewModel(s)).ToList();
                List.AddRange(results);
            }
            
            foreach (var empWeek in List.Where(f => f.PREndDate <= lastDate)
                                        .GroupBy(g => new { g.EmployeeId, g.PREndDate })
                                        .Select(s => new {
                                            s.Key.EmployeeId,
                                            s.Key.PREndDate,
                                            UnApprovedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Pending).Count(),
                                            ApprovedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Approved).Count(),
                                            PostedCnt = s.Where(f => f.Status == DB.PayrollReviewStatusEnum.Posted).Count(),
                                            OnHoldCnt = s.Where(f => f.PayrollStatus == DB.PayrollEntryStatusEnum.OnHold).Count(),
                                            List = s
                                        }).ToList())
            {
                var status = DB.PayrollReviewStatusEnum.Pending;
                if (empWeek.PostedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Posted;
                }
                else if (empWeek.ApprovedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Approved;
                }
                else if (empWeek.UnApprovedCnt != 0)
                {
                    status = DB.PayrollReviewStatusEnum.Pending;
                }
                foreach (var item in empWeek.List.Where(f => f.Status == DB.PayrollReviewStatusEnum.Empty).ToList())
                {
                    item.Status = status;
                    if (empWeek.OnHoldCnt > 0)
                    {
                        item.PayrollStatus = DB.PayrollEntryStatusEnum.OnHold;
                    }
                }
            }
            var payPerdiod = db.PayPeriods.FirstOrDefault(f => f.PREndDate == lastDate);
            IsPayrollPeriodSetup = payPerdiod != null;

        }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }


        public bool IsPayrollPeriodSetup { get; set; }
        public List<PayrollReviewSummaryViewModel> List { get; }
    }

    public class PayrollReviewSummaryViewModel
    {
        public PayrollReviewSummaryViewModel()
        {

        }

        public PayrollReviewSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DB.Infrastructure.ViewPointDB.Data.Calendar calendar)
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
            CrewId = employee.CrewId;
            CrewName = employee.Crew == null ? "Un assigned" : employee.Crew.Description;
            EmployeeId = employee.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", employee.FirstName, employee.LastName, employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = calendar.Date;
            WeekId = (int)calendar.Week;
            PREndDate = calendar.Date.WeekEndDate(DayOfWeek.Saturday);
            //PerdiemId = DB.PerdiemEnum.No;
            Status = DB.PayrollReviewStatusEnum.Empty;
            PayrollStatus = DB.PayrollEntryStatusEnum.Accepted;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.PRBatchTimeEntry> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }
            var entry = entries.FirstOrDefault();
            var resource = entry.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = entry.Co;
            CrewId = entry.Employee.CrewId;
            CrewName = entry.Employee.Crew == null ? "Un assigned" : entry.Employee.Crew.Description;
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
            Status = DB.PayrollReviewStatusEnum.Posted;
            PayrollStatus = DB.PayrollEntryStatusEnum.Pending;
            #endregion
        }

        public PayrollReviewSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.udPRTH_PayrollSummary_Result result)
        {
            if (result == null)
            {
                throw new System.ArgumentNullException(nameof(result));
            }
            #region mapping
            PRCo = result.Co;
            CrewId = result.CrewId;
            CrewName = result.CrewName;
            EmployeeId = result.EmployeeId;
            EmployeeName = result.EmployeeName;
            Postion = result.Position;
            WorkDate = result.WorkDate;
            WeekId = (int)result.WeekId;
            PREndDate = result.PREndDate;
            Hour = result.Hours;
            if (result.PerdiemId != null)
            {
                PerdiemId = (DB.PerdiemEnum)result.PerdiemId;
            }
            Comments = result.Comments;
            Status = (DB.PayrollReviewStatusEnum)result.Status;
            PayrollStatus = (DB.PayrollEntryStatusEnum)result.PayrollStatus;
            #endregion
        }

        public PayrollReviewSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.udPRTH_UserSummary_Result result)
        {
            if (result == null)
            {
                throw new System.ArgumentNullException(nameof(result));
            }
            #region mapping
            PRCo = result.Co;
            CrewId = result.CrewId;
            CrewName = result.CrewName;
            EmployeeId = result.EmployeeId;
            EmployeeName = result.EmployeeName;
            Postion = result.Position;
            WorkDate = result.WorkDate;
            WeekId = (int)result.WeekId;
            PREndDate = result.PREndDate;
            Hour = result.Hours;
            if (result.PerdiemId != null)
            {
                PerdiemId = (DB.PerdiemEnum)result.PerdiemId;
            }
            Comments = result.Comments;
            Status = (DB.PayrollReviewStatusEnum)result.Status;
            PayrollStatus = (DB.PayrollEntryStatusEnum)result.PayrollStatus;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.PayrollEntry> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }
            var entry = entries.FirstOrDefault();
            var resource = entry.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = entry.PRCo;
            CrewId = entry.Employee.CrewId;
            CrewName = entry.Employee.Crew == null ? "Un assigned" : entry.Employee.Crew.Description;
            EmployeeId = entry.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", entry.Employee.FirstName, entry.Employee.LastName, entry.Employee.Suffix);
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
            Status = DB.PayrollReviewStatusEnum.Posted;
            PayrollStatus = DB.PayrollEntryStatusEnum.Accepted;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollHour> hours)
        {
            if (hours == null)
            {
                throw new System.ArgumentNullException(nameof(hours));
            }
            var hour = hours.FirstOrDefault();
            var resource = hour.Employee.Resource.FirstOrDefault();

            #region mapping
            PRCo = hour.PRCo;
            CrewId = hour.Employee.CrewId;
            CrewName = hour.Employee.Crew == null ? "Un assigned" : hour.Employee.Crew.Description;
            EmployeeId = hour.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", hour.Employee.FirstName, hour.Employee.LastName, hour.Employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = hour.WorkDate;
            WeekId = (int)hour.Calendar.Week;
            PREndDate = hour.WorkDate.WeekEndDate(DayOfWeek.Saturday);
            Hour = hours.Sum(s => s.HoursAdj ?? s.Hours);
            PerdiemId = DB.PerdiemEnum.No;
            Comments = hour.Comments;
            Status = DB.PayrollReviewStatusEnum.Approved;
            PayrollStatus = (DB.PayrollEntryStatusEnum)hour.Status;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollPerdiem> perdiems)
        {
            if (perdiems == null)
            {
                throw new System.ArgumentNullException(nameof(perdiems));
            }

            var perdiem = perdiems.FirstOrDefault();
            var resource = perdiem.Employee.Resource.FirstOrDefault();
            #region mapping
            PRCo = perdiem.PRCo;
            CrewId = perdiem.Employee.CrewId;
            CrewName = perdiem.Employee.Crew == null ? "Un assigned" : perdiem.Employee.Crew.Description;
            EmployeeId = perdiem.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", perdiem.Employee.FirstName, perdiem.Employee.LastName, perdiem.Employee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = perdiem.WorkDate;
            WeekId = (int)perdiem.Calendar.Week;
            PREndDate = perdiem.WorkDate.WeekEndDate(DayOfWeek.Saturday);
            PerdiemId = perdiems.Max(max => (DB.PerdiemEnum)(max.PerdiemIdAdj ?? max.PerdiemId));
            Comments = perdiem.Comments;
            Status = DB.PayrollReviewStatusEnum.Approved;
            PayrollStatus = (DB.PayrollEntryStatusEnum)perdiem.Status;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry> hours)
        {
            if (hours == null)
            {
                throw new System.ArgumentNullException(nameof(hours));
            }

            var hour = hours.FirstOrDefault();
            var resource = hour.PREmployee.Resource.FirstOrDefault();
            #region mapping
            PRCo = (byte)hour.PRCo;
            CrewId = hour.PREmployee.CrewId;
            CrewName = hour.PREmployee.Crew == null ? "Un assigned" : hour.PREmployee.Crew.Description;
            EmployeeId = (int)hour.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", hour.PREmployee.FirstName, hour.PREmployee.LastName, hour.PREmployee.Suffix);
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
            Status = DB.PayrollReviewStatusEnum.Pending;
            PayrollStatus = DB.PayrollEntryStatusEnum.Review;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem> perdiems)
        {
            if (perdiems == null)
            {
                throw new System.ArgumentNullException(nameof(perdiems));
            }
            var perdiem = perdiems.FirstOrDefault();
            var resource = perdiem.PREmployee.Resource.FirstOrDefault();

            #region mapping
            PRCo = (byte)perdiem.PRCo;
            CrewId = perdiem.PREmployee.CrewId;
            CrewName = perdiem.PREmployee.Crew == null ? "Un assigned" : perdiem.PREmployee.Crew.Description;
            EmployeeId = (int)perdiem.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", perdiem.PREmployee.FirstName, perdiem.PREmployee.LastName, perdiem.PREmployee.Suffix);
            Postion = resource.Position?.Description ?? "";
            WorkDate = (DateTime)perdiem.WorkDate;
            WeekId = (int)perdiem.Calendar.Week;
            PREndDate = WorkDate.WeekEndDate(DayOfWeek.Saturday);
            PerdiemId = perdiems.Max(max => (DB.PerdiemEnum)(max.PerDiemIdAdj ?? max.PerDiemId));
            Comments = perdiem.Comments;
            Status = DB.PayrollReviewStatusEnum.Pending;
            PayrollStatus = DB.PayrollEntryStatusEnum.Review;
            #endregion
        }

        public PayrollReviewSummaryViewModel(List<DB.Infrastructure.ViewPointDB.Data.DailyJobEmployee> entries)
        {
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }

            var entry = entries.FirstOrDefault();
            var resource = entry.PREmployee.Resource.FirstOrDefault();
            #region mapping
            PRCo = (byte)entry.PRCo;
            CrewId = entry.PREmployee.CrewId;
            CrewName = entry.PREmployee.Crew == null ? "Un assigned" : entry.PREmployee.Crew.Description;
            EmployeeId = (int)entry.EmployeeId;
            EmployeeName = entry.PREmployee.FullName;
            Postion = resource.Position?.Description ?? "";
            WorkDate = (DateTime)entry.WorkDate;
            WeekId = (int)entry.Calendar.Week;
            PREndDate = WorkDate.WeekEndDate(DayOfWeek.Saturday);
            //Hour = entry.Hours ?? 0;
            //PerdiemId = (DB.PerdiemEnum)entry.Perdiem;
            PerdiemId = entries.Max(max => (DB.PerdiemEnum)((short?)max.PerdiemAdj ?? max.Perdiem));
            Hour = entries.Sum(s => s.HoursAdj ?? s.Hours);
            Comments = entry.Comments;
            Status = DB.PayrollReviewStatusEnum.Pending;
            PayrollStatus = DB.PayrollEntryStatusEnum.Review;
            #endregion
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "PRCo")]
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
        [Display(Name = "Id")]
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
        public DB.PayrollReviewStatusEnum Status { get; set; }


        [Display(Name = "Payroll")]
        [UIHint("EnumBox")]
        public DB.PayrollEntryStatusEnum PayrollStatus { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Comments")]
        [UIHint("TextBox")]
        public string Comments { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 2, ComboUrl = "/Crew/Combo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Display(Name = "Crew")]
        [UIHint("TextBox")]
        public string CrewName { get; set; }

        [Display(Name = "Postion")]
        [UIHint("TextBox")]
        public string Postion { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Crew Count")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? CrewCount { get; set; }

        public dynamic DynaimicTicket { get; set; }
    }
}
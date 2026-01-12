using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll
{
    public class PayrollEmployeeReviewListViewModel
    {
        public PayrollEmployeeReviewListViewModel()
        {

        }

        public PayrollEmployeeReviewListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DB.Infrastructure.ViewPointDB.Data.Calendar calendar, DB.Infrastructure.ViewPointDB.Data.VPContext db)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }

            if (calendar == null)
            {
                throw new System.ArgumentNullException(nameof(calendar));
            }

            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }

            #region mapping
            PRCo = employee.PRCo;
            PREndDate = calendar.Date.WeekEndDate(DayOfWeek.Saturday);
            WeekId = (int)calendar.Week;
            EmployeeId = employee.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", employee.FirstName, employee.LastName, employee.Suffix);
            #endregion

            CalendarList = db.Calendars.Where(f => f.Week == calendar.Week).AsEnumerable().Select(s => new PayrollCalendarViewModel(s)).ToList();
            PerdiemList = employee.PayrollPerdiems.Where(f => f.Calendar.Week == calendar.Week)
                                                  .GroupBy(g => new { g.PRCo, g.EmployeeId, g.WorkDate, g.JobId, g.EquipmentId, g.TicketId })
                                                  .Select(s => new PayrollPerdiemReviewViewModel(s.ToList()))
                                                  .ToList();
            HourList = employee.PayrollHours.Where(f => f.Calendar.Week == calendar.Week)
                                            .GroupBy(g => new { g.PRCo, g.EmployeeId, g.WorkDate, g.JobId, g.EquipmentId, g.TicketId })
                                            .Select(s => new PayrollHourReviewViewModel(s.ToList()))
                                            .ToList();

            var ticketList = employee.DailyEmployeeEntries
                .Where(f => f.DailyTicket.Calendar.Week == calendar.Week &&
                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted)
                .GroupBy(g => g.DailyTicket)
                .Select(s => s.Key)
                .ToList();

            ticketList.AddRange(employee.DailyJobEmployees
                .Where(f => f.DailyTicket.Calendar.Week == calendar.Week &&
                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted)
                .GroupBy(g => g.DailyTicket)
                .Select(s => s.Key)
                .ToList());

            TicketList = ticketList.Select(s => new DailyTicket.DailyTicketViewModel(s)).ToList();

        }

        [Key]
        [ReadOnly(true)]
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
        [UIHint("LongBox")]
        [Display(Name = "Week #")]
        public int WeekId { get; set; }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Required]
        [Display(Name = "Id")]
        public string EmployeeName { get; set; }

        public List<PayrollPerdiemReviewViewModel> PerdiemList { get; }

        public List<PayrollHourReviewViewModel> HourList { get; }

        public List<PayrollCalendarViewModel> CalendarList { get; }

        public List<DailyTicket.DailyTicketViewModel> TicketList { get; }
    }

    public class PayrollCalendarViewModel
    {
        public PayrollCalendarViewModel()
        {

        }
        public PayrollCalendarViewModel(DB.Infrastructure.ViewPointDB.Data.Calendar calendar)
        {
            if (calendar == null)
            {
                throw new System.ArgumentNullException(nameof(calendar));
            }

            Date = calendar.Date;
            WeekDayName = calendar.Date.DayOfWeek.ToString();
            WeekId = (int)calendar.Week;
        }

        [Key]
        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime Date { get; set; }

        [ReadOnly(true)]
        [Display(Name = "WeekId")]
        [UIHint("IntegerBox")]
        public int WeekId { get; set; }

        [ReadOnly(true)]
        [Display(Name = "WeekDay")]
        [UIHint("TextBox")]

        public string WeekDayName { get; set; }


    }

   

}

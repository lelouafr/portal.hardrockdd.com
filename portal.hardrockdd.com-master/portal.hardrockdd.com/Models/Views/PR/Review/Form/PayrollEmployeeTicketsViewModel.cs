using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Review
{
    public class PayrollEmployeeTicketListViewModel
    {
        public PayrollEmployeeTicketListViewModel()
        {

        }

        public PayrollEmployeeTicketListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, int weekId, VPContext db)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Calendar = db.Calendars.Where(f => f.Week == weekId).AsEnumerable().Select(s => new PayrollCalendarViewModel(s)).ToList();
            var empEntries = db.DailyEmployeeEntries.Where(f => f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                                   f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                                   f.Calendar.Week == weekId &&
                                                                   f.tEmployeeId == employee.EmployeeId)
                                                           .GroupBy(g => new { g.DTCo, g.tWorkDate, g.TicketId, g.DailyTicket.tStatusId, g.DailyTicket.CreatedUser })
                                                           .Select(s => new { s.Key.DTCo, s.Key.tWorkDate, s.Key.TicketId, s.Key.tStatusId, s.Key.CreatedUser })
                                                           .ToList();
            var jobEntries = db.DailyJobEmployees.Where(f => f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                                   f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                                   f.Calendar.Week == weekId &&
                                                                   f.tEmployeeId == employee.EmployeeId)
                                                           .GroupBy(g => new { g.DTCo, g.tWorkDate, g.TicketId, g.DailyTicket.tStatusId, g.DailyTicket.CreatedUser })
                                                           .Select(s => new { s.Key.DTCo, s.Key.tWorkDate, s.Key.TicketId, s.Key.tStatusId, s.Key.CreatedUser })
                                                           .ToList();
            var entries = empEntries.Union(jobEntries);
            Tickets = entries.Select(s => new PayrollEmployeeTicketViewModel {
                DTCo = s.DTCo, 
                TicketId = s.TicketId,
                WorkDate = (DateTime)s.tWorkDate,
                EmployeeId = employee.EmployeeId,
                StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.tStatusId),
                Status = (DB.DailyTicketStatusEnum)s.tStatusId,
                CreatedBy = string.Format(AppCultureInfo.CInfo() ,"{0} {1}", s.CreatedUser.FirstName, s.CreatedUser.LastName)
            }).ToList();
            //SaturdayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Saturday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //SundayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Sunday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //MondayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Monday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //TuesdayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Tuesday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //WednesdayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Wednesday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //ThursdayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Thursday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();
            //FridayTickets = entries.Where(f => ((DateTime)f.WorkDate).DayOfWeek == DayOfWeek.Friday).Select(s => new PayrollEmployeeTicketViewModel { Co = s.Co, TicketId = s.TicketId, StatusClass = StaticFunctions.StatusClass((DB.DailyTicketStatusEnum)s.Status) }).ToList();

        }

        [Key]
        public byte DTCo { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [Key]
        public int WeekId { get; set; }

        [Key]
        public List<PayrollEmployeeTicketViewModel> Tickets { get; }

        public List<PayrollCalendarViewModel> Calendar { get; }

        //public List<PayrollEmployeeTicketViewModel> SaturdayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> SundayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> MondayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> TuesdayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> WednesdayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> ThursdayTickets { get; }

        //public List<PayrollEmployeeTicketViewModel> FridayTickets { get; }

    }

    public class PayrollEmployeeTicketViewModel
    {
        public PayrollEmployeeTicketViewModel()
        {


        }

        [Key]
        public byte DTCo { get; set; }

        [Key]
        public int TicketId { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public DB.DailyTicketStatusEnum Status { get; set; }

        public string StatusClass { get; set; }

        public string CreatedBy { get; set; }
    }

}
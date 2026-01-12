using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace portal.Models.Views.EM.Audit.Report
{
    public class AuditWeeklyReportListViewModel
    {
        public AuditWeeklyReportListViewModel()
        {

        }

        public AuditWeeklyReportListViewModel(int weekId)
        {
            using var db = new VPContext();

            var dates = db.Calendars.Where(f => f.Week == weekId).ToList();
            var weeks = db.Calendars.Where(f => f.Week <= weekId && f.Week >= weekId - 4).ToList();
            var startDate = weeks.Min(min => min.Date);
            var endDate = weeks.Max(max => max.Date);
            var audits = db.EMAudits.Where(f => f.CreatedOn >= startDate && f.CreatedOn <= endDate).ToList();
            var crews = db.Crews.OrderBy(o => o.Description).Where(f => f.CrewStatus == "ACTIVE" && f.udCrewType == "Crew").ToList();
            var employees = db.Employees.Where(f => f.ActiveYN == "Y" && f.AssignedEquipment.Any()).ToList().OrderBy(o => o.FullName()).ToList();

            crews.ForEach((Action<Crew>)(e => {
                if (employees.Any((Func<DB.Infrastructure.ViewPointDB.Data.Employee, bool>)(f => f.EmployeeId == e.tCrewLeaderId)))
                {
                    var emp = employees.FirstOrDefault((Func<DB.Infrastructure.ViewPointDB.Data.Employee, bool>)(f => f.EmployeeId == e.tCrewLeaderId));
                    employees.Remove(emp);
                }
            }));

            Headers = db.Calendars.Where(f => f.Week <= weekId && f.Week >= weekId - 4)
                             .GroupBy(g => new { g.Week, g.WeekDescription })
                             .Select(s => new AuditWeekHeaderViewModel {
                                 WeekId = (int)s.Key.Week,
                                 Label = s.Key.WeekDescription,
                                 StartDate = s.Min(min => min.Date),
                                 EndDate = s.Max(max => max.Date)
                             }).ToList();
            var crewList = crews.Select(s => new AuditWeekRowViewModel
            {
                PRCo = s.PRCo,
                CrewId = s.CrewId,
                Description = s.Description,
                AuditList = audits.Where(f => f.ParmCrewId == s.CrewId).Select(a => new EquipmentAuditViewModel(a)).ToList()
            }).ToList();
            var empList = employees.Select(s => new AuditWeekRowViewModel
            {
                PRCo = s.PRCo,
                EmployeeId = s.EmployeeId,
                Description = s.FullName(),
                AuditList = audits.Where(f => f.ParmEmployeeId == s.EmployeeId).Select(a => new EquipmentAuditViewModel(a)).ToList()
            }).ToList();

            PRCo = 1;
            List = crewList;
            List.AddRange(empList);
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int PRCo { get; set; }


        public List<AuditWeekRowViewModel> List { get; }

        public List<AuditWeekHeaderViewModel> Headers { get; }

    }

    public class AuditWeekRowViewModel
    {

        public int PRCo { get; set; }

        public string CrewId { get; set; }

        public int EmployeeId { get; set; }

        public string Description { get; set; }

        public List<EquipmentAuditViewModel> AuditList { get; set; }

    }

    public class AuditWeekHeaderViewModel
    {
        public int WeekId { get; set; }

        public string Label { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
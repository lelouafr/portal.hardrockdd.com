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
    public class AuditSubmissionReportListViewModel
    {
        public AuditSubmissionReportListViewModel()
        {

        }

        public AuditSubmissionReportListViewModel(int weekId)
        {
            using var db = new VPContext();

            var dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            var audits = db.EMAudits.Where(f => f.CreatedOn >= dates.Min() && f.CreatedOn <= dates.Max() || (f.CreatedOn < dates.Max() && f.StatusId <= 4)).ToList();
            var crews = db.Crews.OrderBy(o => o.Description).Where(f => f.CrewStatus == "ACTIVE" && f.udCrewType == "Crew").ToList();
            var employees = db.Employees.Where(f => f.ActiveYN == "Y" && f.AssignedEquipment.Any()).ToList();
            var crewList = crews.Select(s => new AuditSubmissionReportViewModel(s, weekId, audits.Where(f => f.ParmCrewId == s.CrewId).ToList(), dates)).ToList();
            var empList = employees.Select(s => new AuditSubmissionReportViewModel(s, weekId, audits.Where(f => f.ParmEmployeeId == s.EmployeeId).ToList(), dates)).ToList();

            PRCo = 1;
            WeekId = weekId;
            List = crewList;
            List.AddRange(empList);
            Dates = dates;
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int PRCo { get; set; }


        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        public List<AuditSubmissionReportViewModel> List { get; }

        public List<DateTime> Dates { get; }
    }


    public class AuditSubmissionReportViewModel
    {
        public AuditSubmissionReportViewModel()
        {

        }

        public AuditSubmissionReportViewModel(Crew crew, int weekId, List<EMAudit> audits, List<DateTime> dates)
        {
            if (audits == null) throw new ArgumentNullException(nameof(audits));
            if (crew == null) throw new ArgumentNullException(nameof(crew));
            Crew = crew.Description;

            PRCo = crew.PRCo;
            CrewId = crew.CrewId;
            WeekId = weekId;
            DayList = dates.Select(s => new AuditSubmissionReportDayViewModel(audits.Where(f => f.CreatedOn.Date == s.Date).ToList(), s)).ToList();
            Dates = dates;
        }
        public AuditSubmissionReportViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, int weekId, List<EMAudit> audits, List<DateTime> dates)
        {
            if (audits == null) throw new ArgumentNullException(nameof(audits));
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            PRCo = employee.PRCo;
            Employee = employee.FullName;
            EmployeeId = employee.EmployeeId;
            WeekId = weekId;
            DayList = dates.Select(s => new AuditSubmissionReportDayViewModel(audits.Where(f => f.CreatedOn.Date == s.Date).ToList(), s)).ToList();
            Dates = dates;
        }

        [Key]
        [HiddenInput]
        [Display(Name = "Company #")]
        public int PRCo { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        [Key]
        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [Key]
        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Employee")]
        public string Employee { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        public List<DateTime> Dates { get; }

        public List<AuditSubmissionReportDayViewModel> DayList { get; }

    }

    public class AuditSubmissionReportDayViewModel
    {
        public AuditSubmissionReportDayViewModel()
        {

        }

        public AuditSubmissionReportDayViewModel(List<EMAudit> audits, DateTime date)
        {
            WorkDate = date;
            List = audits.Select(s => new EquipmentAuditViewModel(s)).ToList();
        }

        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }

        public List<EquipmentAuditViewModel> List { get; }
    }

}
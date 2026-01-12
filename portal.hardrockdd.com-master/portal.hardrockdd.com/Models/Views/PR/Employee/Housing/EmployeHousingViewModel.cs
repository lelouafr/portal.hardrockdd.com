
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Employee.Housing
{
    public class EmployeeHousingListViewModel
    {
        public EmployeeHousingListViewModel()
        {

        }

        public EmployeeHousingListViewModel(byte co, int weekId, VPContext db)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));
            PRCo = co;
            WeekId = weekId;
            var caledar = db.Calendars.OrderByDescending(o => o.Date).FirstOrDefault(f => f.Week == weekId);
            var list = db.udDTEH_PortalReport(co, weekId, caledar.Date).OrderBy(f => f.Employee).ToList();

            List = list.Select(s => new EmployeeHousingViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        public List<EmployeeHousingViewModel> List { get; }
    }
    public class EmployeeHousingViewModel
    {
        public EmployeeHousingViewModel()
        {

        }

        public EmployeeHousingViewModel(udDTEH_PortalReport_Result line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            PRCo = line.Co;
            WeekId = line.WeekId;
            Employee = line.Employee;
            EmployeeId = line.EmployeeId;
            CrewLeader = line.CrewLeader;
            SaturdayHouse = line.SaturdayHouse;
            SundayHouse = line.SundayHouse;
            MondayHouse = line.MondayHouse;
            TuesdayHouse = line.TuesdayHouse;
            WednesdayHouse = line.WednesdayHouse;
            ThursdayHouse = line.ThursdayHouse;
            FridayHouse = line.FridayHouse;
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int? WeekId { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Crew Leader")]
        public string CrewLeader { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Saturday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? SaturdayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Sunday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? SundayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Monday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? MondayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Tuesday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? TuesdayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Wednesday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? WednesdayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Thursday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? ThursdayHouse { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Friday")]
        [Field(ComboUrl = "/PRCombo/HouseTypeCombo", ComboForeignKeys = "PRCo")]
        public int? FridayHouse { get; set; }
    }
}
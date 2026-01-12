using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll//portal.Models.Views.Payroll.PayrollEmployeeHourListViewModel
{
    public class PayrollEmployeeHourListViewModel
    {
        public PayrollEmployeeHourListViewModel()
        {

        }

        public PayrollEmployeeHourListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, int weekId, VPContext db)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            List = db.DTPayrollHours.Where(f => f.Calendar.Week == weekId && f.EmployeeId == employee.EmployeeId)
                                           .ToList()
                                           .Select(s => new PayrollEmployeeHourViewModel(s))
                                           .ToList();
        }

        public List<PayrollEmployeeHourViewModel> List { get; }

    }
    public class PayrollEmployeeHourViewModel
    {
        public PayrollEmployeeHourViewModel()
        {

        }


        public PayrollEmployeeHourViewModel(DTPayrollHour hour)
        {
            if (hour == null) throw new System.ArgumentNullException(nameof(hour));

            #region mapping
            PRCo = hour.PRCo;
            EmployeeId = hour.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", hour.Employee.FirstName, hour.Employee.LastName, hour.Employee.Suffix);
            WorkDate = hour.WorkDate;
            DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
            TicketId = hour.TicketId;
            EarnCodeId = hour.EarnCodeId;
            EntryTypeId = (DB.EntryTypeEnum)hour.EntryTypeId;
            Hour = hour.HoursAdj ?? hour.Hours;
            Comments = hour.Comments;

            EMCo = hour.EMCo;
            EquipmentId = hour.EquipmentId;

            JCCo = hour.JCCo;
            JobId = hour.JobId;
            PhaseGroupId = hour.PhaseGroupId;
            PhaseId = hour.PhaseId;
            Status = (DB.PayrollEntryStatusEnum)hour.Status;

            #endregion
        }
        public byte? PhaseGroupId { get; set; }
        public byte? JCCo { get; set; }
        public byte? EMCo { get; set; }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [Key]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Wor kDate")]
        public DateTime WorkDate { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int LineNum { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Day")]
        public string DayDescription { get; set; }//{ get { return string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString()); } }

        [Required]
        [HiddenInput]
        public int TicketId { get; set; }

        [Required]
        [HiddenInput]
        public int TicketLineNum { get; set; }


        [Required]
        [HiddenInput]
        public string EmployeeName { get; set; }

        [Display(Name = "Hours")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Hour", FormGroupRow = 1)]
        public decimal? Hour { get; set; }


        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        public decimal? Value { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Entry Type")]
        [Field(Placeholder = "Entry Type")]
        public DB.EntryTypeEnum? EntryTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/EarnCodeCombo", ComboForeignKeys = "PRCo, EmployeeId")]
        [Display(Name = "Earn Code")]
        public short? EarnCodeId { get; set; }



        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        [Field(Placeholder = "Status")]
        public DB.PayrollEntryStatusEnum? Status { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(Placeholder = "", ComboUrl = "/JCCombo/JCJobPhaseCombo", ComboForeignKeys = "JCCo,JobId,PhaseGroupId")]
        [Display(Name = "Task")]
        public string PhaseId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }
}
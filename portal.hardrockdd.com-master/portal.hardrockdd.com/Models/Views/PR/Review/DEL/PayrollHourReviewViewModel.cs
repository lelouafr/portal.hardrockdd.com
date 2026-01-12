using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll
{
    public class PayrollHourReviewViewModel
    {
        public PayrollHourReviewViewModel()
        {

        }


        public PayrollHourReviewViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollHour> hours)
        {
            if (hours == null)
            {
                throw new System.ArgumentNullException(nameof(hours));
            }

            var hour = hours.FirstOrDefault();
            #region mapping
            PRCo = hour.PRCo;
            EmployeeId = hour.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", hour.Employee.FirstName, hour.Employee.LastName, hour.Employee.Suffix);
            WorkDate = hour.WorkDate;
            DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
            LineNum = hour.LineNum;
            TicketId = hour.TicketId;
            TicketLineNum = hour.TicketLineNum;
            EarnCodeId = hour.EarnCodeId;
            EntryTypeId = (DB.EntryTypeEnum)hour.EntryTypeId;
            Hour = hours.Sum(sum => sum.HoursAdj ?? sum.Hours);
            Comments = hour.Comments;
            EquipmentId = hour.EquipmentId;
            JobId = hour.JobId;
            PhaseId = hours.OrderByDescending(o => o.Hours).FirstOrDefault().PhaseId;
            Status = (DB.PayrollEntryStatusEnum)hour.Status;

            #endregion
        }

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
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(Placeholder = "", ComboUrl = "/JCCombo/JCJobPhaseCombo", ComboForeignKeys = "PhaseGroupId,JobId")]
        [Display(Name = "Task")]
        public string PhaseId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }


        internal PayrollHourReviewViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObjs = db.DTPayrollHours
                        .Where(f => f.PRCo == PRCo &&
                                    f.EmployeeId == EmployeeId &&
                                    f.WorkDate == WorkDate &&
                                    f.JobId == JobId &&
                                    f.EquipmentId == EquipmentId)
                        .ToList();
            var totalHours = updObjs.Sum(sum => sum.Hours);
            foreach (var updObj in updObjs)
            {
                var calHour = totalHours != 0 ? (updObj.Hours / totalHours) * Hour : Hour;
                if (updObj.HoursAdj != calHour)
                {
                    updObj.HoursAdj = calHour;
                    var ticket = db.DailyTickets.FirstOrDefault(w => w.DTCo == updObj.DTCo && w.TicketId == updObj.TicketId);
                    foreach (var hour in ticket.DailyEmployeeEntries.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
                    {
                        hour.ValueAdj = updObj.HoursAdj;
                    }
                    foreach (var hour in ticket.DailyJobEmployees.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
                    {
                        if (totalHours != 0)
                            hour.HoursAdj = (hour.Hours / totalHours) * Hour;
                        else
                            hour.HoursAdj = Hour;
                    }
                }
                try
                {
                    db.BulkSaveChanges();
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            return new PayrollHourReviewViewModel(updObjs);
        }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    //LeaveEmployeeSummaryListViewModel
    public class LeaveEmployeeSummaryListViewModel
    {
        public LeaveEmployeeSummaryListViewModel()
        {

        }
        public LeaveEmployeeSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;
            List = employee.Leave.Select(s => new LeaveEmployeeCodesViewModel(s)).ToList();

            foreach (var item in employee.Leave)
            {
                var listItem = List.FirstOrDefault(f => f.LeaveCodeId == item.LeaveCodeId);
                if (item.LeaveCode.FixedFreq == "I" && listItem.PostedHours + (listItem.RequestedHours ?? 0) == 0)
                {
                    List.Remove(listItem);
                }
            }
        }

        public LeaveEmployeeSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            PRCo = request.PRCo;
            EmployeeId = request.Employee.EmployeeId;
            List = request.Employee.Leave.Select(s => new LeaveEmployeeCodesViewModel(s, request)).ToList();

            foreach (var item in request.Employee.Leave)
            {
                var listItem = List.FirstOrDefault(f => f.LeaveCodeId == item.LeaveCodeId);
                if (item.LeaveCode.FixedFreq == "I" && listItem.PostedHours + (listItem.RequestedHours ?? 0) == 0)
                {
                    List.Remove(listItem);
                }
            }
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }

        public List<LeaveEmployeeCodesViewModel> List { get; }
    }

    public class LeaveEmployeeCodesViewModel
    {
        public LeaveEmployeeCodesViewModel()
        {

        }

        public LeaveEmployeeCodesViewModel(DB.Infrastructure.ViewPointDB.Data.EmployeeLeave leave)
        {
            if (leave == null) throw new System.ArgumentNullException(nameof(leave));

            using var db = new VPContext();
            PRCo = leave.PRCo;
            EmployeeId = leave.EmployeeId;
            LeaveCodeId = leave.LeaveCodeId;

            StartBalHours = leave.AvailBalMax ?? leave.LeaveCode.FixedUnits;

            var earnCodesList = leave.LeaveCode.Usages.Where(f => f.Type == "U").Select(s => s.EarnCodeId).ToArray();
            PostedHours = db.PayrollEntries.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                        f.PostDate.Year == DateTime.Now.Year &&
                                                        earnCodesList.Contains(f.EarnCodeId))
                                            .DefaultIfEmpty()
                                            .Sum(sum => sum == null ? 0 : sum.Hours);
            RequestedHours = db.LeaveRequestLines.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                             f.LeaveCodeId == leave.LeaveCodeId &&
                                                             f.Request.Status <= (int)DB.LeaveRequestStatusEnum.Approved)
                                                 .Sum(sum => sum.Hours);
            AvailableHours = StartBalHours - PostedHours - (RequestedHours ?? 0);

            //RequestedHours = leave.req
        }

        public LeaveEmployeeCodesViewModel(DB.Infrastructure.ViewPointDB.Data.EmployeeLeave leave, LeaveRequest request)
        {
            if (leave == null) throw new System.ArgumentNullException(nameof(leave));
            if (request == null) throw new System.ArgumentNullException(nameof(request));

            using var db = new VPContext();
            PRCo = leave.PRCo;
            EmployeeId = leave.EmployeeId;
            LeaveCodeId = leave.LeaveCodeId;

            StartBalHours = leave.AvailBalMax ?? leave.LeaveCode.FixedUnits;

            var year = (request.Lines.FirstOrDefault()?.WorkDate ?? DateTime.Now).Year;

            var earnCodesList = leave.LeaveCode.Usages.Where(f => f.Type == "U").Select(s => s.EarnCodeId).ToArray();
            PostedHours = db.PayrollEntries.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                        f.PostDate.Year == year &&
                                                        earnCodesList.Contains(f.EarnCodeId))
                                            .DefaultIfEmpty()
                                            .Sum(sum => sum == null ? 0 : sum.Hours);

            var approvedHours = db.DTPayrollHours.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                             //earnCodesList.Contains(f.EarnCodeId)) &&
                                                             f.WorkDate.Year == year &&
                                                             f.StatusId != 4 &&
                                                             earnCodesList.Contains((short)f.EarnCodeId))
                                                 .DefaultIfEmpty()
                                            .Sum(sum => sum == null ? 0 : sum.Hours);
            var batchHours = db.PRBatchTimeEntries.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                        f.PostDate.Year == year &&
                                                        earnCodesList.Contains(f.tEarnCodeId))
                                            .DefaultIfEmpty()
                                            .Sum(sum => sum == null ? 0 : sum.Hours);

            PostedHours += approvedHours;
            PostedHours += batchHours;
            RequestedHours = db.LeaveRequestLines.Where(f => f.EmployeeId == leave.EmployeeId &&
                                                             f.LeaveCodeId == leave.LeaveCodeId &&
                                                             ((DateTime)f.WorkDate).Year == year &&
                                                             f.Request.Status <= (int)DB.LeaveRequestStatusEnum.Approved)
                                                 .Sum(sum => sum.Hours);
            AvailableHours = StartBalHours - PostedHours - (RequestedHours ?? 0);

            //RequestedHours = leave.req
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/LeaveCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Type")]
        public string LeaveCodeId { get; set; }


        [UIHint("IntegerBox")]
        [Display(Name = "Starting Balance")]
        public decimal? StartBalHours { get; set; }


        [UIHint("IntegerBox")]
        [Display(Name = "Used")]
        public decimal? PostedHours { get; set; }


        [UIHint("IntegerBox")]
        [Display(Name = "Requested")]
        public decimal? RequestedHours { get; set; }


        [UIHint("IntegerBox")]
        [Display(Name = "Available")]
        public decimal? AvailableHours { get; set; }


    }

    //public class LeaveEmployeeSummaryViewModel
    //{
    //    public LeaveEmployeeSummaryViewModel()
    //    {

    //    }

    //    public LeaveEmployeeSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveEmployeeSummary entry)
    //    {
    //        if (entry == null)
    //        {
    //            throw new System.ArgumentNullException(nameof(entry));
    //        }
    //        PRCo = entry.PRCo;
    //        EmployeeId = entry.EmployeeId;
    //        LeaveCodeId = entry.LeaveCodeId;

    //        Hours = entry.Hours;

    //        Comments = entry.Comments;
    //    }

    //    public bool Validate(ModelStateDictionary modelState)
    //    {
    //        if (modelState == null)
    //        {
    //            throw new System.ArgumentNullException(nameof(modelState));
    //        }
    //        var ok = modelState.IsValid;

    //        return ok;
    //    }

    //    [Key]
    //    [Required]
    //    [HiddenInput]
    //    [Display(Name = "PRCo")]
    //    public byte PRCo { get; set; }

    //    [Key]
    //    [Required]
    //    [HiddenInput]
    //    [Display(Name = "Ticket Id")]
    //    public int RequestId { get; set; }

    //    [Key]
    //    [Required]
    //    [HiddenInput]
    //    [Display(Name = "Line Num")]
    //    public int LineId { get; set; }

    //    [UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Date")]
    //    public DateTime? WorkDate { get; set; }

    //    [Required]
    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
    //    [Display(Name = "Employee")]
    //    public int? EmployeeId { get; set; }

    //    [Required]
    //    [UIHint("DropdownBox")]
    //    [Field(ComboUrl = "/PRCombo/LeaveCodeCombo", ComboForeignKeys = "PRCo")]
    //    [Display(Name = "LeaveCode")]
    //    public string LeaveCodeId { get; set; }

    //    [Required]
    //    [UIHint("IntegerBox")]
    //    [Display(Name = "Hours")]
    //    public decimal? Hours { get; set; }


    //    [UIHint("TextBox")]
    //    [Display(Name = "Comments")]
    //    [Field(LabelSize = 2, TextSize = 10)]
    //    [TableField(InternalTableRow = 2)]
    //    public string Comments { get; set; }
    //}


}
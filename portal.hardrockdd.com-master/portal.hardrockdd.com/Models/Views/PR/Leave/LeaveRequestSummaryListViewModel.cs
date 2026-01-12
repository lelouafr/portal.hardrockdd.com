using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class LeaveRequestSummaryListViewModel
    {       

        public LeaveRequestSummaryListViewModel()
        {

        }

        public LeaveRequestSummaryListViewModel(WebUser user, DB.LeaveListTypeEnum listType, DB.LeaveRequestStatusEnum status)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            

            var comp = user.Employee.FirstOrDefault().HRCompanyParm.PRCompanyParm;
            //switch (listType)
            //{
            //    case DB.LeaveListTypeEnum.User:
            //        List = user.LeaveRequests.Where(f => f.Status == (int)status).Select(s => new LeaveRequestSummaryViewModel(s)).OrderByDescending(f => f.RequestId).ToList();
            //        break;
            //    case DB.LeaveListTypeEnum.Assigned:
            //        List = user.AssignedLeaveRequests.GroupBy(g => new { g.Request }).Where(f => f.Key.Request.Status == (int)status).Select(s => new LeaveRequestSummaryViewModel(s.Key.Request)).OrderByDescending(f => f.RequestId).ToList();
            //        break;
            //    case DB.LeaveListTypeEnum.All:
            //        List = comp.LeaveRequests.Where(f => f.Status == (int)status).Select(s => new LeaveRequestSummaryViewModel(s)).OrderByDescending(f => f.RequestId).ToList();
            //        break;
            //    default:
            //        break;
            //}
            var requstList = new List<LeaveRequest>();
            switch (listType)
            {
                case DB.LeaveListTypeEnum.User:
                    requstList = user.LeaveRequests.Where(f => f.Status == (int)status).ToList();
                    requstList.AddRange(comp.PRLeaveRequests.Where(f => f.Status == (int)status && f.EmployeeId == user.Employee.FirstOrDefault().HRRef && f.CreatedBy != user.Id).ToList());

                    var yearUserList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
                    List = yearUserList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
                    break;
                case DB.LeaveListTypeEnum.Assigned:
                    requstList = user.AssignedLeaveRequests.Where(f => f.Request.Status == (int)status).Select(s => s.Request).ToList();

                    var yearAssignedList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
                    List = yearAssignedList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
                    break;
                case DB.LeaveListTypeEnum.All:
                    requstList = comp.PRLeaveRequests.Where(f => f.Status == (int)status).ToList();
                    var yearAllList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
                    //.Select(s => new LeaveRequestSummaryViewModel(s)).OrderByDescending(f => f.RequestId).ToList();

                    List = yearAllList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
                    break;
                default:
                    break;
            }
        }

        public LeaveRequestSummaryListViewModel(WebUser user, DB.LeaveListTypeEnum listType)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            var comp = user.Employee.FirstOrDefault().HRCompanyParm.PRCompanyParm;
            var requstList = new List<LeaveRequest>();
            switch (listType)
            {
                case DB.LeaveListTypeEnum.User:
                    requstList = user.LeaveRequests.Where(f => f.Status != (int)DB.LeaveRequestStatusEnum.Canceled).ToList();
                    requstList.AddRange(comp.PRLeaveRequests.Where(f => f.Status != (int)DB.LeaveRequestStatusEnum.Canceled && f.EmployeeId == user.Employee.FirstOrDefault().HRRef && f.CreatedBy != user.Id).ToList());

                   // requstList = comp.LeaveRequests.Where(f => f.Status != (int)DB.LeaveRequestStatusEnum.Canceled && f.EmployeeId == user.Employee.FirstOrDefault().HRRef).ToList();

                    var yearUserList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
                    
                    List = yearUserList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
                    break;
                case DB.LeaveListTypeEnum.Assigned:
                    requstList = user.AssignedLeaveRequests.Where(f => f.Request.Status != (int)DB.LeaveRequestStatusEnum.Canceled).Select(s => s.Request).ToList();
                    var yearAssignedList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
                    List = yearAssignedList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
                    break;
                default:
                    break;
            }

        }

        public LeaveRequestSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            var requstList = new List<LeaveRequest>();
            requstList = company.PRCompanyParm.PRLeaveRequests.Where(f => f.Status != (int)DB.LeaveRequestStatusEnum.Canceled).ToList();
            var yearAllList = requstList.SelectMany(s => s.Lines).GroupBy(g => new { (g.WorkDate ?? DateTime.Now).Year, g.Request }).Select(s => new { s.Key.Year, s.Key.Request }).ToList();
            //.Select(s => new LeaveRequestSummaryViewModel(s)).OrderByDescending(f => f.RequestId).ToList();

            List = yearAllList.Select(s => new LeaveRequestSummaryViewModel(s.Year, s.Request)).OrderByDescending(f => f.RequestId).ToList();
            //List = company.LeaveRequests.Select(s => new LeaveRequestSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new LeaveRequestSummaryViewModel(s)).ToList());
        }

        public List<LeaveRequestSummaryViewModel> List { get; }
    }

    public class LeaveRequestSummaryViewModel
    {

        public LeaveRequestSummaryViewModel()
        {

        }

        public LeaveRequestSummaryViewModel(LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            PRCo = request.PRCo;
            RequestId = request.RequestId;
            Status = (DB.LeaveRequestStatusEnum)request.Status;
            if (request.Lines.Count > 0)
            {
                var line = request.Lines.FirstOrDefault();
            }
            Description = request.Lines.FirstOrDefault().LeaveCode?.Description;
            var minDate = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Min(min => min.WorkDate) ?? DateTime.Now;
            var maxDate = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Max(max => max.WorkDate) ?? DateTime.Now;
            if (minDate != maxDate)
            {
                Description += string.Format(AppCultureInfo.CInfo(), " ({0} - {1})", minDate.ToShortDateString(), maxDate.ToShortDateString());
            }
            else if (minDate == maxDate)
            {
                Description += string.Format(AppCultureInfo.CInfo(), " ({0})", minDate.ToShortDateString());
            }
            EmployeeId = request.EmployeeId;
            EmployeeName = request.Employee?.FirstName + ' ' + request.Employee?.LastName;
            //CreatedBy = vticket.CreatedBy;
            Hours = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Sum(sum => sum.Hours);
            CreatedUser = new WebUserViewModel(request.CreatedUser);

            ContainsErrors = false;
            ErrorMsg = string.Empty;

            ErrorMsg = ErrorMsg.Trim();

            CanProcess = !ContainsErrors && Status == DB.LeaveRequestStatusEnum.Approved;
        }

        public LeaveRequestSummaryViewModel(int year, LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            Year = year;
            PRCo = request.PRCo;
            RequestId = request.RequestId;
            Status = (DB.LeaveRequestStatusEnum)request.Status;
            if (request.Lines.Count > 0)
            {
                var line = request.Lines.FirstOrDefault();
            }
            Description = request.Lines.FirstOrDefault().LeaveCode?.Description;
            var minDate = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Min(min => min.WorkDate) ?? DateTime.Now;
            var maxDate = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Max(max => max.WorkDate) ?? DateTime.Now;
            if (minDate != maxDate)
            {
                Description += string.Format(AppCultureInfo.CInfo(), " ({0} - {1})", minDate.ToShortDateString(), maxDate.ToShortDateString());
            }
            else if (minDate == maxDate)
            {
                Description += string.Format(AppCultureInfo.CInfo(), " ({0})", minDate.ToShortDateString());
            }
            EmployeeId = request.EmployeeId;
            EmployeeName = request.Employee.FullName;
            //CreatedBy = vticket.CreatedBy;
            Hours = request.Lines.Where(f => (f.WorkDate ?? DateTime.Now).Year == Year).Sum(sum => sum.Hours);
            CreatedUser = new WebUserViewModel(request.CreatedUser);

            ContainsErrors = false;
            ErrorMsg = string.Empty;

            ErrorMsg = ErrorMsg.Trim();

            CanProcess = !ContainsErrors && Status == DB.LeaveRequestStatusEnum.Approved;
        }


        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }

        public string StatusString { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3)]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.LeaveRequestStatusEnum Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Leave")]
        public string Leave { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime WorkDate { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }
               
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }
        
        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }


        [ReadOnly(true)]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        public decimal? Hours { get; set; }

        [ReadOnly(true)]
        [UIHint("IntegerBox")]
        [Display(Name = "Balance")]
        public decimal? Balance { get; set; }


        public bool ContainsErrors { get; set; }

        [UIHint("SwitchBoxGreen")]
        public bool CanProcess { get; set; }

        public string ErrorMsg { get; set; }
    }


}

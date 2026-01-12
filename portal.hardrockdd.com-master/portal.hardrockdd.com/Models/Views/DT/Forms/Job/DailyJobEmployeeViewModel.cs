using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyJobEmployeeListViewModel: AuditBaseViewModel
    {
        public DailyJobEmployeeListViewModel()
        {

        }
        
        public DailyJobEmployeeListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket): base(ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            Employees = ticket.DailyJobEmployees.Select(s => new DailyJobEmployeeViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        [Field(LabelSize = 2, TextSize = 10)]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        [Field(LabelSize = 2, TextSize = 10)]
        public int TicketId { get; set; }


        public List<DailyJobEmployeeViewModel> Employees { get; }
    }

    public class DailyJobEmployeeViewModel : AuditBaseViewModel
    {
        public DailyJobEmployeeViewModel()
        {

        }
        public DailyJobEmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.DailyJobEmployee employee) : base(employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            DTCo = employee.DTCo;
            TicketId = employee.TicketId;
            LineNum = employee.LineNum;
            PRCo = employee.PRCo ?? employee.DailyTicket?.HQCompanyParm?.PRCompanyParm.PRCo;
            EmployeeId = employee.EmployeeId;
            EmployeeName = Repository.VP.PR.EmployeeRepository.FullName(employee.PREmployee);
            Hours = employee.HoursAdj?? employee.Hours;
            Perdiem = (DB.PerdiemEnum)((short?)employee.PerdiemAdj ?? (employee.Perdiem ?? 0));
            Comments = employee.Comments;
            Status = (DB.DailyTicketStatusEnum)(employee.DailyTicket?.Status ?? 0);

            AuthDriver = employee.PREmployee?.HREmployee?.AuthDriver ?? true;

		}

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        [Field(LabelSize = 2, TextSize = 10)]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        [Field(LabelSize = 2, TextSize = 10)]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        [Field(LabelSize = 2, TextSize = 10)]
        public int LineNum { get; set; }

        public int? PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }


        [HiddenInput]
        [Display(Name = "Employee")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string EmployeeName { get; set; }


		[Display(Name = "")]
		public bool? AuthDriver { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        [Field(LabelSize = 2, TextSize = 10)]
        public decimal? Hours { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Perdiem")]
        [Field(LabelSize = 2, TextSize = 10)]
        public DB.PerdiemEnum? Perdiem { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }

        [HiddenInput]
        public bool CommentRow { get; set; }
        public DB.DailyTicketStatusEnum Status { get; set; }


        internal DailyJobEmployeeViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.EmployeeId = EmployeeId;
                updObj.Hours = Hours;
                updObj.Perdiem = (short?)Perdiem;
                updObj.Comments = Comments;

                try
                {
                    db.SaveChanges(modelState);
                    return new DailyJobEmployeeViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }
    }


}
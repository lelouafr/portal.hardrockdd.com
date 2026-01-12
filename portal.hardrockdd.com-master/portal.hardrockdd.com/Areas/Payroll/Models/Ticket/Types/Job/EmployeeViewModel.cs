using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Job
{
    public class EmployeeListViewModel
    {
        public EmployeeListViewModel()
        {

        }
        
        public EmployeeListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            List = ticket.DailyJobEmployees.Select(s => new EmployeeViewModel(s)).ToList();
        }

        [Key]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }


        public List<EmployeeViewModel> List { get; }
    }

    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {

        }
        public EmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.DailyJobEmployee employee)
        {
            if (employee == null)
                return;

            DTCo = employee.DTCo;
            TicketId = employee.TicketId;
            LineNum = employee.LineNum;
            PRCo = employee.PRCo ?? employee.DailyTicket?.HQCompanyParm?.PRCompanyParm.PRCo;
            EmployeeId = employee.EmployeeId;
            EmployeeName = employee.PREmployee.FullName(true);

            Hours = employee.HoursAdj?? employee.Hours;
            PerDiem = employee.PerDiem;
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
        [Display(Name = "PerDiem")]
        [Field(LabelSize = 2, TextSize = 10)]
        public DB.PerdiemEnum PerDiem { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }

        [HiddenInput]
        public bool CommentRow { get; set; }
        public DB.DailyTicketStatusEnum Status { get; set; }


        internal EmployeeViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.EmployeeId = EmployeeId;
                updObj.Hours = Hours;
                updObj.PerDiem = PerDiem;
                updObj.Comments = Comments;

                try
                {
                    db.SaveChanges(modelState);
                    return new EmployeeViewModel(updObj);
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
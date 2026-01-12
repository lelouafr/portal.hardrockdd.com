using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll
{
    public class PayrollPerdiemReviewViewModel
    {
        public PayrollPerdiemReviewViewModel()
        {

        }
        public PayrollPerdiemReviewViewModel(DB.Infrastructure.ViewPointDB.Data.DTPayrollPerdiem perdiem)
        {
            if (perdiem == null)
            {
                throw new System.ArgumentNullException(nameof(perdiem));
            }
            #region mapping
            PRCo = perdiem.PRCo;
            EmployeeId = perdiem.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", perdiem.Employee.FirstName, perdiem.Employee.LastName, perdiem.Employee.Suffix);
            WorkDate = perdiem.WorkDate;
            LineNum = perdiem.LineNum;
            TicketId = perdiem.TicketId;
            TicketLineNum = perdiem.TicketLineNum;
            PerdiemId = (DB.PerdiemEnum)(perdiem.PerdiemIdAdj ?? perdiem.PerdiemId);
            Comments = perdiem.Comments;
            EquipmentId = perdiem.EquipmentId;
            JobId = perdiem.JobId;
            #endregion
        }

        public PayrollPerdiemReviewViewModel(List<DB.Infrastructure.ViewPointDB.Data.DTPayrollPerdiem> perdiems)
        {
            if (perdiems == null)
            {
                throw new System.ArgumentNullException(nameof(perdiems));
            }

            var perdiem = perdiems.FirstOrDefault();

            #region mapping
            PRCo = perdiem.PRCo;
            EmployeeId = perdiem.EmployeeId;
            EmployeeName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", perdiem.Employee.FirstName, perdiem.Employee.LastName, perdiem.Employee.Suffix);
            WorkDate = perdiem.WorkDate;
            LineNum = perdiem.LineNum;
            TicketId = perdiem.TicketId;
            TicketLineNum = perdiem.TicketLineNum;
            PerdiemId = (DB.PerdiemEnum)((perdiem.PerdiemIdAdj ?? perdiem.PerdiemId) ?? 0);
            Comments = perdiem.Comments;
            EquipmentId = perdiem.EquipmentId;
            JobId = perdiem.JobId;
            #endregion
        }
        [Key]
        [HiddenInput]
        [Required]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public DateTime WorkDate { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        public int LineNum { get; set; }

        [Required]
        [HiddenInput]
        public int TicketId { get; set; }

        [HiddenInput]
        public int TicketLineNum { get; set; }

        [HiddenInput]
        public string JobId { get; set; }

        [HiddenInput]
        public string EquipmentId { get; set; }


        [HiddenInput]
        [Required]
        public string EmployeeName { get; set; }

        [Display(Name = "Perdiem")]
        [UIHint("EnumBox")]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Perdiem", FormGroupRow = 1)]
        public DB.PerdiemEnum PerdiemId { get; set; }

        [HiddenInput]
        public string Comments { get; set; }


        internal PayrollPerdiemReviewViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObjs = db.DTPayrollPerdiems
                        .Where(f => f.PRCo == PRCo &&
                                    f.EmployeeId == EmployeeId &&
                                    f.WorkDate == WorkDate &&
                                    f.JobId == JobId &&
                                    f.EquipmentId == EquipmentId)
                        .ToList();

            foreach (var updObj in updObjs)
            {
                if (updObj.PerdiemIdAdj != (int)PerdiemId)
                {
                    updObj.PerdiemIdAdj = (int)PerdiemId;
                    var ticket = db.DailyTickets.FirstOrDefault(w => w.DTCo == updObj.DTCo && w.TicketId == updObj.TicketId);
                    foreach (var hour in ticket.DailyEmployeePerdiems.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
                    {
                        hour.PerDiemIdAdj = updObj.PerdiemIdAdj;
                    }
                    foreach (var hour in ticket.DailyJobEmployees.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
                    {
                        hour.PerdiemAdj = updObj.PerdiemIdAdj;
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
            return new PayrollPerdiemReviewViewModel(updObjs);
        }
    }
}
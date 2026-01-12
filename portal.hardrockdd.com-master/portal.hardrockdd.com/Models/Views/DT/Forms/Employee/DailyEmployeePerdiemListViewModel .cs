using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyEmployeePerdeimListViewModel: DailyEmployeeEntryListViewModel
    {
        public DailyEmployeePerdeimListViewModel()
        {

        }

        public DailyEmployeePerdeimListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            //Used for Table View
            List = ticket.DailyEmployeePerdiems.Select(s => new DailyEmployeePerdiemViewModel(s)).ToList();
        }

        public new List<DailyEmployeePerdiemViewModel> List { get; }
    }

    public class DailyEmployeePerdiemViewModel
    {
        public DailyEmployeePerdiemViewModel()
        {

        }

        public DailyEmployeePerdiemViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            DTCo = entry.DTCo;
            TicketId = entry.TicketId;
            LineNum = entry.LineNum;
            WorkDate = (DateTime)entry.WorkDate;
            EmployeeId = entry.EmployeeId;
            Perdiem = (DB.PerdiemEnum)(entry.PerDiemIdAdj ?? entry.PerDiemId);

        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }


        [Required]
        [HiddenInput]
        public DateTime WorkDate { get; set; }

        [Required]
        [HiddenInput]
        public int? EmployeeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Perdiem")]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Perdiem", FormGroupRow = 1, Placeholder = "Select Perdiem")]
        public DB.PerdiemEnum? Perdiem { get; set; }

        internal DailyEmployeePerdiemViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeePerdiems.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.EmployeeId = EmployeeId;
                updObj.PerDiemId = (int)(Perdiem ?? DB.PerdiemEnum.No);
                updObj.PerDiemIdAdj = null;
                try
                {
                    db.BulkSaveChanges();
                    return new DailyEmployeePerdiemViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }


}
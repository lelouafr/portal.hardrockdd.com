using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyWeeklyEntryListViewModel
    {
        public DailyWeeklyEntryListViewModel()
        {

        }
        public DailyWeeklyEntryListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            Entries = ticket.DailyEmployeeEntries.Select(s => new DailyWeeklyEmployeeEntryViewModel(s)).OrderBy(o => o.WorkDate).ToList();
            Perdiems = ticket.DailyEmployeePerdiems.Select(s => new DailyEmployeePerdiemViewModel(s)).OrderBy(o => o.WorkDate).ToList();

            var cal = ticket.db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId).ToList();
            StartDate = cal.Min(m => m.Date);
            EndDate = cal.Max(m => m.Date);
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


        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 2, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Week")]
        public DateTime StartDate { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 2, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime EndDate { get; set; }
        public List<DailyWeeklyEmployeeEntryViewModel> Entries { get; }

        public List<DailyEmployeePerdiemViewModel> Perdiems { get; }

    }

    public class DailyWeeklyEmployeeEntryViewModel: DT.Forms.DailyEmployeeEntryAbstract
    {

        public DailyWeeklyEmployeeEntryViewModel()
        {

        }

        public DailyWeeklyEmployeeEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry):base(entry)
        {
        }

         
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo", InfoUrl = "/Project/Job/PopupForm", InfoForeignKeys = "JCCo,JobId")]
        [Display(Name = "Job")]
        public override string JobId { get; set; }

        public override string DayDescription { get ; set ; }

        public override int? EmployeeId { get ; set ; }

        public override short? EarnCodeId { get ; set ; }

        public override string EquipmentId { get ; set ; }

        public override byte PhaseGroupId { get ; set ; }

        public override string PhaseId { get ; set ; }
    }
    
    public class DailyWeeklyEmployeePerdiemViewModel: DailyEmployeePerdiemViewModel
    {
        public DailyWeeklyEmployeePerdiemViewModel()
        {

        }

        public DailyWeeklyEmployeePerdiemViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem entry): base(entry)
        {
        }  
    }


}
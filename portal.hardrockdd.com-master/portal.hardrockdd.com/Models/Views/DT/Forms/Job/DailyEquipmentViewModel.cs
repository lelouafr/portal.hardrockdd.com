using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyEquipmentListViewModel : AuditBaseViewModel
    {
        public DailyEquipmentListViewModel()
        {

        }

        public DailyEquipmentListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket): base(ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            //Equipments = ticket.DailyEquipments.Select(s => new DailyEquipmentViewModel(s)).OrderBy(o => o.LineNum).ToList();
            Equipments = new List<DailyEquipmentViewModel>();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        public List<DailyEquipmentViewModel> Equipments { get; }
    }

    public class DailyEquipmentViewModel : AuditBaseViewModel
    {
        public DailyEquipmentViewModel()
        {

        }

        public DailyEquipmentViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEquipment t):base(t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            DTCo = t.DTCo;
            TicketId = t.TicketId;
            LineNum = t.LineNum;
            WorkDate = t.tWorkDate;
            EquipmentId = t.EquipmentId;
            Value = t.Value;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }

        [HiddenInput]
        [Display(Name = "Work Date")]
        public DateTime? WorkDate { get; set; }


        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Equipment")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        public string EquipmentId { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Value")]
        [Field(LabelSize = 2, TextSize = 10)]
        public decimal? Value { get; set; }

    }
}
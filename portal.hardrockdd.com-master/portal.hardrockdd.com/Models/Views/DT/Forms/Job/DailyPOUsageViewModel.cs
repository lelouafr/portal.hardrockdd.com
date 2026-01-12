using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyPOUsageListViewModel: AuditBaseViewModel
    {
        public DailyPOUsageListViewModel()
        {

        }
        
        public DailyPOUsageListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket): base(ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            List = ticket.POUsages.Select(s => new DailyPOUsageViewModel(s)).ToList();
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


        public List<DailyPOUsageViewModel> List { get; }
    }

    public class DailyPOUsageViewModel : AuditBaseViewModel
    {
        public DailyPOUsageViewModel()
        {

        }
        public DailyPOUsageViewModel(DB.Infrastructure.ViewPointDB.Data.DailyPOUsage usage) : base(usage)
        {
            if (usage == null)
            {
                throw new System.ArgumentNullException(nameof(usage));
            }
            DTCo = usage.DTCo;
            TicketId = usage.TicketId;
            LineId = usage.LineId;
            JCCo = usage.JCCo;
            JobId = usage.JobId;
            VendGroupId = usage.Ticket.HQCompanyParm.VendorGroupId;
            VendorId = usage.VendorId;
            PO = usage.PO;
            Hours = usage.Hours;
            Quantity = usage.Qty ?? 0m;
        }

        public byte? VendGroupId { get; set; }
        public byte? JCCo { get; set; }

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
        public int LineId { get; set; }

        [Required]
        [HiddenInput]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Vendor")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/APCombo/JobVendorCombo", ComboForeignKeys = "VendGroupId, JobId")]
        public int? VendorId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "PO")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/POCombo/POJobCombo", ComboForeignKeys = "JCCo, JobId, VendorId")]
        public string PO { get; set; }

        [HiddenInput]
        [Display(Name = "Vendor")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string VendorName { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        [Field(LabelSize = 2, TextSize = 10)]
        public decimal? Hours { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Quantity")]
        [Field(LabelSize = 2, TextSize = 10)]
        public decimal? Quantity { get; set; }

        internal DailyPOUsageViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyPOUsages.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineId == LineId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                var vendorChange = updObj.VendorId != VendorId;
                updObj.VendorId = VendorId;
                updObj.PO = vendorChange ? null: PO;
                updObj.Qty = Quantity;

                try
                {
                    db.SaveChanges(modelState);
                    return new DailyPOUsageViewModel(updObj);
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
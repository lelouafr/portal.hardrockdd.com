using DB.Infrastructure.ViewPointDB.Data;
using portal.Models;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Invoice
{
    public class InvoiceSummaryListViewModel
    {       

        public InvoiceSummaryListViewModel(DB.APStatusEnum status, DB.TimeSelectionEnum timeSelection)
        {
            TimeSelection = timeSelection;
            Status = status;
            List = new List<InvoiceSummaryViewModel>();
        }

        public InvoiceSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DB.APStatusEnum status, DB.TimeSelectionEnum timeSelection)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }

            Status = status;
            VendorGroupId = (byte)company.VendorGroupId;
            TimeSelection = timeSelection;
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };
            var mthFlt = new DateTime(asofDate.Year, asofDate.Month, 1);

            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            switch (status)
            {
                case DB.APStatusEnum.Open:
                    List = db.vAPTrans.Where(f => f.APCo == company.HQCo && f.OpenYN == "Y").AsEnumerable().Select(s => new InvoiceSummaryViewModel(s)).ToList();
                    break;
                case DB.APStatusEnum.All:
                default:
                    List = db.vAPTrans.Where(f => f.APCo == company.HQCo && f.Mth >= mthFlt).AsEnumerable().Select(s => new InvoiceSummaryViewModel(s)).ToList();
                    break;
            }
            //List.AddRange(company.Invoices.Select(s => new InvoiceSummaryViewModel(s)).ToList());
        }

        public InvoiceSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor, DB.TimeSelectionEnum timeSelection)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            VendorGroupId = vendor.VendorGroupId;
            //VendorGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;
            Status = DB.APStatusEnum.All;
            TimeSelection = timeSelection;
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };
            var mthFlt = new DateTime(asofDate.Year, asofDate.Month, 1);
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            List = db.vAPTrans.Where(f => f.APCo == vendor.VendorGroupId && f.VendorId == vendor.VendorId && f.Mth >= mthFlt).AsEnumerable().Select(s => new InvoiceSummaryViewModel(s)).ToList();
            //List.AddRange(company.Invoices.Select(s => new InvoiceSummaryViewModel(s)).ToList());
        }

        public InvoiceSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, VPContext db, DB.TimeSelectionEnum timeSelection)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }

            VendorGroupId = 1;
            JobId = job.JobId;
            Status = DB.APStatusEnum.All;
            TimeSelection = timeSelection;
            var list = (from vAPTrans in db.vAPTrans
                    join APTransLines in db.APTransLines.Where(f => f.JobId == job.JobId).GroupBy(g => new { g.APCo, g.Mth, g.APTransId }) on
                    new { p1 = vAPTrans.APCo, p2 = vAPTrans.Mth, p3 = vAPTrans.APTransId }
                    equals
                    new { p1 = APTransLines.Key.APCo, p2 = APTransLines.Key.Mth, p3 = APTransLines.Key.APTransId }
                    select vAPTrans).ToList();

            List = list.Select(s => new InvoiceSummaryViewModel(s)).ToList();
            //List.AddRange(company.Invoices.Select(s => new InvoiceSummaryViewModel(s)).ToList());
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte VendorGroupId { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "VendorId")]
        public int? VendorId { get; set; }

        [HiddenInput]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Time Range")]
        public DB.TimeSelectionEnum TimeSelection { get; set; }

        public DB.APStatusEnum Status { get; set; }

        public List<InvoiceSummaryViewModel> List { get; }
    }

    public class InvoiceSummaryViewModel
    {
        public InvoiceSummaryViewModel()
        {

        }

        public InvoiceSummaryViewModel(vAPTran tran)
        {
            if (tran == null)
            {
                throw new System.ArgumentNullException(nameof(tran));
            }
            APCo = tran.APCo;
            KeyID = tran.KeyID;
            InvoiceDate = tran.InvDate;
            Mth = tran.Mth;
            APTransId = tran.APTransId;
            Description = tran.DisplayDescription;
            VendorGroupId = tran.VendorGroup;
            VendorId = tran.VendorId;
            VendorName = tran.VendorName;
            InvAmount = tran.InvTotal;
            APRef = tran.APRef;
            SearchString = tran.SearchStr;
        }

        public byte? VendorGroupId { get; set; }

        public string StatusString { get; set; }

        public string SearchString { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [UIHint("DateBox")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Mth")]
        public DateTime Mth { get; set; }

        public string MthString { get { return Mth.ToShortDateString(); } }

        [Key]
        [Required]
        [UIHint("TextBox")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int APTransId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public long KeyID { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        
        public string InvoiceDateString { get { return InvoiceDate.ToShortDateString(); } }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }
               
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "AP Ref")]
        public string APRef { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? InvAmount { get; set; }
    }


}

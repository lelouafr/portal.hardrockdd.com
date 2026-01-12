using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.Payment
{
    public class InvoicePaymentListViewModel
    {
        public InvoicePaymentListViewModel()
        {
            
        }

        public InvoicePaymentListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, int cMAcct)
        {
            if (batch == null)
            {
                List = new List<InvoicePaymentViewModel>();
                return;
            }

            APCo = batch.Co;
            CMAcct = cMAcct;
            Mth = batch.Mth.ToShortDateString();
            BatchId = batch.BatchId;

            List = batch.APPayments.Where(f => f.CMAcct == cMAcct).Select(s => new InvoicePaymentViewModel(s)).ToList();
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "")]
        public byte APCo { get; set; }



        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Mth")]
        public string Mth { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "CMAcct")]
        public int CMAcct { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Batch #")]
        public int? BatchId { get; set; }

        public List<InvoicePaymentViewModel> List { get; }

    }

    public class InvoicePaymentViewModel
    {
        public InvoicePaymentViewModel()
        {

        }

        public InvoicePaymentViewModel(DB.Infrastructure.ViewPointDB.Data.APPayment payment)
        {
            if (payment == null)
                return;

            APCo = payment.APCo;
            CMCo = payment.CMCo;
            CMAcct = payment.CMAcct;
            PayMethod = payment.PayMethod;
            CMRef = payment.CMRef;
            CMRefSeq = payment.CMRefSeq;
            EFTSeq = payment.EFTSeq;
            VendorGroup = payment.VendorGroup;
            Vendor = payment.Vendor;
            Name = payment.Name;
            ChkType = payment.ChkType;
            PaidMth = payment.PaidMth;
            Mth = payment.PaidMth.ToShortDateString();
            PaidDate = payment.PaidDate;
            Amount = payment.Amount;
            BatchId = payment.BatchId;
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name="APCo")]
        public byte APCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte CMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "CM Account")]
        public short CMAcct { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Pay Method")]
        public string PayMethod { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "CM Ref")]
        public string CMRef { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Ref Seq #")]
        public byte CMRefSeq { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "EFT Seq #")]
        public short EFTSeq { get; set; }

        public byte VendorGroup { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Vendor #")]
        public int Vendor { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Vendor")]
        public string Name { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Type")]
        public string ChkType { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Mth")]
        public DateTime PaidMth { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Mth")]
        public string Mth { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Paid Date")]
        public DateTime PaidDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Batch #")]
        public int? BatchId { get; set; }
    }
}
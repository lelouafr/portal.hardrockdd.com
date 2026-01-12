using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class TransactionLineListViewModel
    {
        public TransactionLineListViewModel()
        {

        }
        public TransactionLineListViewModel(CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            CCCo = transaction.CCCo;
            TransId = transaction.TransId;

            List = transaction.Lines.Select(s => new TransactionLineViewModel(s)).ToList();
        }
        [Key]
        public byte CCCo { get; set; }

        [Key]
        public long TransId { get; set; }

        public List<TransactionLineViewModel> List { get; }
    }
    public class TransactionLineViewModel
    {
        public TransactionLineViewModel()
        {

        }

        public TransactionLineViewModel(CreditTransactionLine line)
        {
            if (line == null) throw new System.ArgumentNullException(nameof(line));

            CCCo = line.CCCo;
            TransId = line.TransId;
            SeqId = line.SeqId;
            Description = line.NewDescription ?? line.OrigDescription;
            LineAmount = line.LineAmount;
            ProductCode = line.ProductCode;
            Quantity = line.Quantity;
            UnitCost = line.UnitCost;
            UM = line.UM;
        }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        public long TransId { get; set; }

        [Key]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int SeqId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? LineAmount { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "ProductCode")]
        public string ProductCode { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        public decimal? UnitCost { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "UM")]
        public string UM { get; set; }
    }
}
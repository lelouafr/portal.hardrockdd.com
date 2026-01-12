using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Areas.Project.Models.Contract
{
    public class RevenueListViewModel
    {

        public RevenueListViewModel()
        {

        }

        public RevenueListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) return;

            List = job.Contract.InvoiceDetails.Where(f => f.JCTransType != "OC" && f.BilledAmt != 0).Select(s => new RevenueViewModel(s)).ToList();
        }

        public RevenueListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null) return;

            List = job.Contract.InvoiceDetails.Where(f => f.JCTransType != "OC" && f.BilledAmt != 0 && f.Mth == mth).Select(s => new RevenueViewModel(s)).ToList();
        }

        public List<RevenueViewModel> List { get;  }
    }

    public class RevenueViewModel
    {
        public RevenueViewModel()
        {

        }

        public RevenueViewModel(DB.Infrastructure.ViewPointDB.Data.JCInvoiceDetail item, bool UnPosted = false)
        {
            if (item == null)
                return;

            JCCo = item.JCCo;
            Mth = item.Mth;
            ItemTrans = item.ItemTrans;
            ContractId = item.ContractId;
            Item = item.Item;
            JCTransType = item.JCTransType;
            TransSource = item.TransSource;
            Description = item.Description;
            PostedDate = item.PostedDate;
            ActualDate = item.ActualDate;
            ContractAmt = item.ContractAmt;
            ContractUnits = item.ContractUnits;
            UnitPrice = item.UnitPrice;
            BilledUnits = item.BilledUnits;
            BilledAmt = item.BilledAmt;
            ReceivedAmt = item.ReceivedAmt;
            CurrentRetainAmt = item.CurrentRetainAmt;
            BatchId = item.BatchId;
            InUseBatchId = item.InUseBatchId;
            GLCo = item.GLCo;
            GLTransAcct = item.GLTransAcct;
            GLOffsetAcct = item.GLOffsetAcct;
            ReversalStatus = item.ReversalStatus;
            ARCo = item.ARCo;
            ARTrans = item.ARTrans;
            ARTransLine = item.ARTransLine;
            ARInvoice = item.ARInvoice;
            ARCheck = item.ARCheck;
            BilledTax = item.BilledTax;
            udEquipmentNumber = item.udEquipmentNumber;
            udCrew = item.udCrew;

        }


        [Key]
        public byte JCCo { get; set; }
        [Key]
        public System.DateTime Mth { get; set; }
        [Key]
        public int ItemTrans { get; set; }
        public string ContractId { get; set; }
        public string Item { get; set; }
        public string JCTransType { get; set; }
        public string TransSource { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ActualDate { get; set; }
        public decimal ContractAmt { get; set; }
        public decimal ContractUnits { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BilledUnits { get; set; }
        public decimal BilledAmt { get; set; }
        public decimal ReceivedAmt { get; set; }
        public decimal CurrentRetainAmt { get; set; }
        public int? BatchId { get; set; }
        public int? InUseBatchId { get; set; }
        public byte? GLCo { get; set; }
        public string GLTransAcct { get; set; }
        public string GLOffsetAcct { get; set; }
        public byte ReversalStatus { get; set; }
        public byte? ARCo { get; set; }
        public int? ARTrans { get; set; }
        public short? ARTransLine { get; set; }
        public string ARInvoice { get; set; }
        public string ARCheck { get; set; }
        public decimal? BilledTax { get; set; }
        public string udEquipmentNumber { get; set; }
        public string udCrew { get; set; }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Batch.Lines
{
    public class BatchTransactionListViewModel
    {
        public BatchTransactionListViewModel()
        {

        }

        public BatchTransactionListViewModel(byte co, DateTime mth, int batchId, string source, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Co = co;
            Mth = mth;
            List = db.vCMTransactionCodes.Where(f => f.Co == co &&
                                                     f.Mth == mth &&
                                                     f.BatchId == batchId &&
                                                     f.Source == source
                                                     )
                .ToList()
                .Select(s => new BatchTransactionViewModel(s))
                .ToList();
        }
        [Key]

        public byte Co { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        public List<BatchTransactionViewModel> List { get; }
    }

    public class BatchTransactionViewModel
    {
        public BatchTransactionViewModel()
        {

        }
        
        public BatchTransactionViewModel(vCMTransactionCode tran)
        {
            if (tran == null) throw new System.ArgumentNullException(nameof(tran));

            Co = tran.Co;
            Mth = tran.Mth;
            BatchId = tran.BatchId;
            Source = tran.Source;
            SeqId = (int)tran.SeqId;
            TransId = tran.TransId;
            CrewId = tran.CrewId;
            Crew = tran.Crew;
            EmployeeId = tran.EmployeeId;
            Employee = tran.Employee;
            VendorId = tran.VendorId;
            Vendor = tran.Vendor;
            MerchantId = tran.MerchantId;
            Merchant = tran.Merchant;
            TransDate = tran.TransDate;
            TransAmt = tran.TransAmt;
            Description = tran.Description;
            CodedStatus = tran.CodedStatus;
            PictureStatusId = tran.PictureStatusId;
            PictureStatus = tran.PictureStatus;
            LineTypeId = tran.LineTypeId;
            LineType = tran.LineType;
            GLAcctId = tran.GLAcctId;
            GLAcct = tran.GLAcct;
            GLAcct2 = tran.GLAcct2;
            JobId = tran.JobId;
            Job = tran.Job;
            Job2 = tran.Job2;
            PhaseId = tran.PhaseId;
            Phase = tran.Phase;
            Phase2 = tran.Phase2;
            JCCTypeId = tran.JCCTypeId;
            JCCType = tran.JCCType;
            JCCType2 = tran.JCCType2;
            EquipmentId = tran.EquipmentId;
            Equipment = tran.Equipment;
            Equipment2 = tran.Equipment2;
            CostCodeId = tran.CostCodeId;
            CostCode = tran.CostCode;
            CostCode2 = tran.CostCode2;
            EMCTypeId = tran.EMCTypeId;
            EMCType = tran.EMCType;
            EMCType2 = tran.EMCType2;
            GrossAmt = tran.GrossAmt;
            TotalCodedAmount = tran.TotalCodedAmount;
            CodeBalance = tran.CodeBalance;
            URL = tran.URL;
        }

        [Key]
        public byte Co { get; set; }
        [Key]
        public System.DateTime Mth { get; set; }
        [Key]
        public string Source { get; set; }
        [Key]
        public long TransId { get; set; }
        [Key]
        public int BatchId { get; set; }
        [Key]
        public int SeqId { get; set; }
        public DateTime TransDate { get; set; }
        public string CrewId { get; set; }
        public string Crew { get; set; }
        public int? EmployeeId { get; set; }
        public string Employee { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string Vendor { get; set; }
        public string MerchantId { get; set; }
        public string Merchant { get; set; }
        public decimal TransAmt { get; set; }
        public string Description { get; set; }
        public string CodedStatus { get; set; }
        public Nullable<int> PictureStatusId { get; set; }
        public string PictureStatus { get; set; }
        public Nullable<byte> LineTypeId { get; set; }
        public string LineType { get; set; }
        public string GLAcctId { get; set; }
        public string GLAcct { get; set; }
        public string GLAcct2 { get; set; }
        public string JobId { get; set; }
        public string Job { get; set; }
        public string Job2 { get; set; }
        public string PhaseId { get; set; }
        public string Phase { get; set; }
        public string Phase2 { get; set; }
        public Nullable<byte> JCCTypeId { get; set; }
        public string JCCType { get; set; }
        public string JCCType2 { get; set; }
        public string EquipmentId { get; set; }
        public string Equipment { get; set; }
        public string Equipment2 { get; set; }
        public string CostCodeId { get; set; }
        public string CostCode { get; set; }
        public string CostCode2 { get; set; }
        public Nullable<byte> EMCTypeId { get; set; }
        public string EMCType { get; set; }
        public string EMCType2 { get; set; }
        public Nullable<decimal> GrossAmt { get; set; }
        public Nullable<decimal> TotalCodedAmount { get; set; }
        public Nullable<decimal> CodeBalance { get; set; }
        public string URL { get; set; }

    }

}
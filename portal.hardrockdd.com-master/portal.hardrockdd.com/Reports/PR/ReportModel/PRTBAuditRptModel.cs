using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Reports.PR.ReportModel
{
    public class PRTBAuditRptModel
    {
        public PRTBAuditRptModel()
        {

        }
        public PRTBAuditRptModel(DB.Infrastructure.ViewPointDB.Data.PRBatchTimeEntry transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping
            Co = transaction.Co;
            Mth = transaction.Mth.ToShortDateString();
            BatchId = transaction.BatchId;
            BatchSeq = transaction.BatchSeq;
            BatchTransType = transaction.BatchTransType;
            PREndDate = transaction.Batch.PREndDate ?? DateTime.Now;
            EmployeeId = transaction.EmployeeId;
            PaySeq = transaction.PaySeq;
            PostSeq = transaction.PostSeq ?? 0;
            Type = transaction.Type;
            DayNum = 0;
            PostDate = transaction.PostDate;
            JCCo = transaction.JCCo ?? transaction.Co;
            EquipmentId = transaction.EquipmentId;
            CostCode = transaction.CostCodeId;
            JobId = transaction.JobId;
            PhaseGroup = transaction.PhaseGroupId ?? transaction.Co;
            PhaseId = transaction.PhaseId;
            GLCo = transaction.GLCo;
            EMCo = transaction.EMCo ?? transaction.Co;
            EMGroup = transaction.EMGroupId ?? transaction.Co;
            TaxState = transaction.TaxState;
            UnempState = transaction.UnempState;
            InsState = transaction.InsState;
            InsCode = transaction.InsCode;
            PRDept = transaction.PRDept;
            CrewId = transaction.CrewId;
            Cert = transaction.Cert;
            EarnCodeId = transaction.EarnCodeId;
            Shift = transaction.Shift;
            Hours = transaction.Hours;
            Rate = transaction.Rate;
            Amt = transaction.Amt;
            TicketId = transaction.udTicketId ?? 0;
            TicketLineId = transaction.udTicketLineId ?? 0;

            Craft= transaction.Craft;
            #endregion

            CompanyName = transaction.Batch.Company.Name;
            EmployeeName = transaction.Employee?.FullName;
            JobName = transaction.JCJob?.DisplayName;
            PhaseName = transaction.JCJobPhase?.Description;
            EquipmentName = transaction.EMEquipment?.DisplayName;
            EMCostCodeName = transaction.EMCostCode?.Description;
            CrewName = transaction.PRCrew?.DisplayName;
            EarnCodeDescription = transaction.EarnCode?.Description;
        }

        public byte Co { get; set; }

        public string Mth { get; set; }

        public int BatchId { get; set; }

        public DateTime PREndDate { get; set; }

        public int BatchSeq { get; set; }

        public string BatchTransType { get; set; }

        public int EmployeeId { get; set; }

        public byte PaySeq { get; set; }

        public short PostSeq { get; set; }

        public string Type { get; set; }

        public short DayNum { get; set; }

        public System.DateTime PostDate { get; set; }

        public byte JCCo { get; set; }

        public string JobId { get; set; }

        public byte PhaseGroup { get; set; }

        public string PhaseId { get; set; }

        public byte GLCo { get; set; }

        public byte EMCo { get; set; }

        public string EquipmentId { get; set; }

        public byte EMGroup { get; set; }

        public string Craft { get; set; }

        public string CostCode { get; set; }

        public string TaxState { get; set; }

        public string UnempState { get; set; }

        public string InsState { get; set; }

        public string InsCode { get; set; }

        public string PRDept { get; set; }

        public string CrewId { get; set; }

        public string Cert { get; set; }

        public short EarnCodeId { get; set; }

        public byte Shift { get; set; }

        public decimal Hours { get; set; }

        public decimal Rate { get; set; }

        public decimal Amt { get; set; }

        public int TicketId { get; set; }

        public int TicketLineId { get; set; }


        public string CompanyName { get; set; }

        public string EmployeeName { get; set; }

        public string JobName { get; set; }

        public string PhaseName { get; set; }

        public string EquipmentName { get; set; }

        public string EMCostCodeName { get; set; }

        public string CrewName { get; set; }

        public string EarnCodeDescription { get; set; }
    }
}
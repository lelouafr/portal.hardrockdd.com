using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.PR
{
   
    public class PRPostedTransListViewModel
    {
        public PRPostedTransListViewModel()
        {

        }

        public PRPostedTransListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            using var db = DB.Infrastructure.ViewPointDB.Data.VPContext.GetDbContextFromEntity(batch);
            var list = db.PayrollEntries.Where(f => f.PRCo == batch.Co && f.PRGroup == batch.PRGroup && f.PREndDate == batch.PREndDate && f.BatchId == batch.BatchId).ToList();

            List = list.Select(s => new PRPostedTransViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/PRAllMthCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        public List<PRPostedTransViewModel> List { get; }
    }

    public class PRPostedTransViewModel
    {
        public PRPostedTransViewModel()
        {

        }

        public PRPostedTransViewModel(DB.Infrastructure.ViewPointDB.Data.PayrollEntry transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping

            PRCo = transaction.PRCo;
            PREndDate = transaction.PREndDate.ToShortDateString();
            PRGroup = transaction.PRGroup;
            EmployeeId = transaction.EmployeeId;
            PaySeq = transaction.PaySeq;
            PostSeq = transaction.PostSeq;
            
            BatchId = transaction.BatchId;
            Type = transaction.Type;
            DayNum = 0;
            PostDate = transaction.PostDate;
            
            EMCo = transaction.EMCo;
            EMGroupId = transaction.EMGroupId;
            EquipmentId = transaction.EquipmentId;
            CostCode = transaction.CostCode;

            JCCo = transaction.JCCo;
            JobId = transaction.JobId;
            PhaseGroupId = transaction.PhaseGroupId;
            PhaseId = transaction.PhaseId;


            GLCo = transaction.GLCo;
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
            TicketId = transaction.udTicketId;
            TicketLineId = transaction.udTicketLineId;
            #endregion

            EmployeeName = transaction.Employee?.FullName;
            JobName = transaction.JCJob?.DisplayName;
            PhaseName = transaction.JCJobPhase?.Description;
            EquipmentName = transaction.EMEquipment?.DisplayName;
            EMCostCodeName = transaction.EMCostCode?.Description;
            CrewName = transaction.PRCrew?.DisplayName;

            EarnCodeDescription = transaction.EarnCode?.Description;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PR Group")]
        public byte PRGroup { get; set; }

        [Key]
        [Required]
        [UIHint("DateBox")]
        [Display(Name = "PR End Date")]
        public string PREndDate { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Pay Seq")]
        public int PaySeq { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Post Seq")]
        public int PostSeq { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [HiddenInput]
        [Display(Name = "Batch Seq")]
        public int BatchSeq { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/HQBatchTransType", ComboForeignKeys = "")]
        [Display(Name = "Trans Type")]
        public string BatchTransType { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PRTCTypes", ComboForeignKeys = "")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Day Num")]
        public short DayNum { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Post Date")]
        public System.DateTime PostDate { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Job Co")]
        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Job")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        public string JobId { get; set; }


        [UIHint("LongBox")]
        [Display(Name = "Phase Group")]
        public byte? PhaseGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Phase")]
        [Field(ComboUrl = "/JCCombo/JCJobPhaseCombo", ComboForeignKeys = "JCCo,JobId")]
        public string PhaseId { get; set; }


        [UIHint("LongBox")]
        [Display(Name = "GL Co")]
        public byte GLCo { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "EM Co")]
        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Equipment")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        public string EquipmentId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "EM Group")]
        public byte? EMGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "EM CC")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        public string CostCode { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Tax State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "country='US'")]
        public string TaxState { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Unemp State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "country='US'")]
        public string UnempState { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Ins State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "country='US'")]
        public string InsState { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Ins Code")]
        public string InsCode { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "PR Dept")]
        [Field(ComboUrl = "/PRCombo/PRDepartmentCode", ComboForeignKeys = "PRCo")]
        public string PRDept { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Crew")]
        [Field(ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        public string CrewId { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Cert")]
        public string Cert { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Earn Code")]
        [Field(ComboUrl = "/PRCombo/EarnCodeCombo", ComboForeignKeys = "PRCo")]
        public short EarnCodeId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Shift")]
        public byte Shift { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        public decimal Hours { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amt")]
        public decimal Amt { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Ticket Id")]
        public int? TicketId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Ticket Line Id")]
        public int? TicketLineId { get; set; }


        [UIHint("Textbox")]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Job")]
        public string JobName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = " Phase")]
        public string PhaseName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Equipment")]
        public string EquipmentName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Cost Code")]
        public string EMCostCodeName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Crew")]
        public string CrewName { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Earn Code")]
        public string EarnCodeDescription { get; set; }

    }
}
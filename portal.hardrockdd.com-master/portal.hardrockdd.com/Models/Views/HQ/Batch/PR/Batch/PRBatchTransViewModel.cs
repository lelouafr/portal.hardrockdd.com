using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.PR
{
    public class PRBatchTransListViewModel
    {
        public PRBatchTransListViewModel()
        {

        }

        public PRBatchTransListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            //using var db = DB.Infrastructure.ViewPointDB.Data.VPContext.GetDbContextFromEntity(batch);
            //List = db.PRBatchTimeEntries.Where(f => f.Co)
            List = batch.PRBatchTimeEntries.Select(s => new PRBatchTransViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/PRAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        public List<PRBatchTransViewModel> List { get; }
    }

    public class PRBatchTransViewModel
    {
        public PRBatchTransViewModel()
        {

        }

        public PRBatchTransViewModel(DB.Infrastructure.ViewPointDB.Data.PRBatchTimeEntry transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping


            Co = transaction.Co;
            PRCo = transaction.Co;

            Mth = transaction.Mth.ToShortDateString();
            BatchId = transaction.BatchId;
            BatchSeq = transaction.BatchSeq;
            BatchTransType = transaction.BatchTransType;
            EmployeeId = transaction.EmployeeId;
            PaySeq = transaction.PaySeq;
            PostSeq = transaction.PostSeq;
            Type = transaction.Type;
            DayNum = 0;
            PostDate = transaction.PostDate;
            JCCo = transaction.JCCo;
            EquipmentId = transaction.EquipmentId;
            CostCode = transaction.CostCodeId;
            JobId = transaction.JobId;
            PhaseGroupId = transaction.PhaseGroupId;
            PhaseId = transaction.PhaseId;
            GLCo = transaction.GLCo;
            EMCo = transaction.EMCo;
            EMGroupId = transaction.EMGroupId;
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
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/PRAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "Batch Seq")]
        public int BatchSeq { get; set; }

        public byte PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/HQBatchTransType", ComboForeignKeys = "")]
        [Display(Name = "Trans Type")]
        public string BatchTransType { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/EmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Seq")]
        public byte PaySeq { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "POst Seq")]
        public short? PostSeq { get; set; }

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



        internal PRBatchTransViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            
            var mthDate = DateTime.TryParse(Mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var updObj = db.PRBatchTimeEntries.FirstOrDefault(f => f.Co == Co && f.Mth == mthDate && f.BatchId == BatchId && f.BatchSeq == BatchSeq);

            if (updObj != null)
            {
                var extAmtChange = updObj.Hours != Hours || updObj.Rate != Rate;

                //updObj.EmployeeId = EmployeeId;
                updObj.BatchTransType = BatchTransType;
                updObj.Type = Type;
                updObj.PostDate = PostDate;
                //updObj.JCCo = JCCo;

                updObj.CostCodeId = CostCode;
                updObj.EquipmentId = EquipmentId;

                updObj.PhaseId = PhaseId;
                updObj.JobId = JobId;

                updObj.GLCo = GLCo;

                updObj.TaxState = TaxState;
                updObj.UnempState = UnempState;
                updObj.InsState = InsState;
                updObj.InsCode = InsCode;
                updObj.PRDept = PRDept;
                updObj.CrewId = CrewId;
                updObj.EarnCodeId = EarnCodeId;
                updObj.Hours = Hours;
                updObj.Rate = Rate;
                if (updObj.EarnCode.Method == "H")
					updObj.Amt = updObj.Hours * updObj.Rate;
				else if (updObj.EarnCode.Method == "D")
					updObj.Amt = Amt;
				else if (updObj.EarnCode.Method == "A")
					updObj.Amt = Amt;

				try
                {
                    db.BulkSaveChanges();
                    return new PRBatchTransViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

    }
}
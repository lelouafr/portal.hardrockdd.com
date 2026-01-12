using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.PO
{
    public class POBatchLineListViewModel
    {
        public POBatchLineListViewModel()
        {

        }

        public POBatchLineListViewModel(DB.Infrastructure.ViewPointDB.Data.POBatchHeader transaction)
        {
            if (transaction == null)
                return;

            Co = transaction.Co;
            Mth = transaction.Mth;
            BatchId = transaction.BatchId;
            BatchSeq = transaction.BatchSeq;

            List = transaction.Items.Select(s => new POBatchLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Mth")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "BatchId")]
        public int BatchId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Seq")]
        public int BatchSeq { get; set; }

        public List<POBatchLineViewModel> List { get; set; }
    }

    public class POBatchLineViewModel
    {
        public POBatchLineViewModel()
        {

        }

        public POBatchLineViewModel(DB.Infrastructure.ViewPointDB.Data.POBatchItem entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            Co = entry.Co;
            GLCo = entry.GLCo;
            EMCo = entry.EMCo;
            JCCo = entry.JCCo;
            //PRCo = entry.PRCo
            EMGroupId = entry.EMGroup;
            PhaseGroupId = entry.PhaseGroupId;
            VendorGroupId = entry.PO.VendorGroupId;

            Mth = entry.Mth.ToShortDateString();
            MthDate = entry.Mth;
            BatchId = entry.BatchId;
            BatchSeq = entry.BatchSeq;

            PO = entry.PO.PO;
            POItem = entry.POItem;
            Status = (DB.POStatusEnum)entry.PO.Status;
            ItemTypeId = (DB.POItemTypeEnum)entry.ItemType;
            CalcType = entry.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = entry.Description;

            JobId = entry.JobId;
            PhaseId = entry.Phase;
            JobCostTypeId = entry.JCCType;
            CrewId = entry.CrewId;
            VendorName = entry.PO.Vendor?.Name;
            VendorId = entry.PO.VendorId;
            EquipmentId = entry.EquipmentId;
            CostCodeId = entry.CostCode;
            CostTypeId = entry.EMCType;

            GLAcct = entry.GLAcct;

            Units = entry.OrigUnits;
            UM = entry.UM;
            UnitCost = entry.OrigUnitCost;
            Cost = entry.OrigCost;

            TaxGroupId = entry.TaxGroup;
            TaxTypeId = (DB.TaxTypeEnum)(entry.TaxType ?? 0);
            TaxCodeId = entry.TaxCode;
            TaxRate = entry.TaxRate;
            TaxAmount = entry.OrigTax;
            if (TaxTypeId != DB.TaxTypeEnum.None && TaxAmount == 0)
            {
                TaxAmount = Cost * TaxRate;
            }


            Comments = entry.Notes;


            if (entry.JobId != null)
                JobName = entry.Job?.DisplayName;
            if (entry.Phase != null)
                PhaseName = entry.JobPhase?.Description;
            if (entry.Equipment != null)
                EquipmentName = entry.Equipment?.DisplayName;
            if (entry.CostCode != null)
                EMCostCodeName = entry.EMCostCode?.Description;
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

            //if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerId == null)
            //{
            //    modelState.AddModelError("CustomerId", "Customer Field is Required");
            //    ok &= false;
            //}
            return ok;
        }


        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? PRCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? JCCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }
        [HiddenInput]
        public byte? VendorGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/POAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int BatchSeq { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Item")]
        public int POItem { get; set; }


        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Mth Date")]
        public DateTime MthDate { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.POStatusEnum Status { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Type")]
        public DB.POItemTypeEnum ItemTypeId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JobCostTypeId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EquipmentCostType/Combo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 2)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 2, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=POCo")]
        [Display(Name = "UM")]
        public string UM { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        public decimal? UnitCost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Cost { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(LabelSize = 2, TextSize = 4, Placeholder = "Tax Type")]
        public DB.TaxTypeEnum? TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 0, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 2, TextSize = 4)]
        public decimal? TaxAmount { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }

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
    }

}
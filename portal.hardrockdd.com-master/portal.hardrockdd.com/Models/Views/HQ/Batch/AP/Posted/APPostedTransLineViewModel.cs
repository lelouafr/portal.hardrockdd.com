using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.AP
{
    public class APPostedLineListViewModel
    {
        public APPostedLineListViewModel()
        {

        }

        public APPostedLineListViewModel(DB.Infrastructure.ViewPointDB.Data.APTran transaction)
        {
            if (transaction == null)
                return;

            APCo = transaction.APCo;
            Mth = transaction.Mth;
            APTransId = transaction.APTransId;

            List = transaction.Lines.Select(s => new APPostedTransLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Mth")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int APTransId { get; set; }

        public List<APPostedTransLineViewModel> List { get; }
    }

    public class APPostedTransLineViewModel
    {
        public APPostedTransLineViewModel()
        {

        }

        public APPostedTransLineViewModel(DB.Infrastructure.ViewPointDB.Data.APTransLine entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            Co = entry.APCo;
            Mth = entry.Mth.ToShortDateString();
            MthDate = entry.Mth;
            APTransId = entry.APTransId;
            APLineId = entry.APLineId;

            VendorGroupId = entry.APTran.VendorGroupId;
            VendorName = entry.APTran.Vendor.Name;
            VendorId = entry.APTran.VendorId;
            APRef = entry.APTran.APRef;

            POCo = entry.APCo;
            PO = entry.PO;
            POItem = entry.POItem;
            POItemTypeId = (DB.POItemTypeEnum?)entry.ItemTypeId;

            //Status = (DB.POStatusEnum)entry.APTran.Status;
            LineTypeId = (DB.APLineTypeEnum)entry.LineType;
            CalcType = entry.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = entry.Description;

            JCCo = entry.JCCo;
            JobId = entry.JobId;
            PhaseGroupId = entry.PhaseGroupId;
            PhaseId = entry.PhaseId;
            JobCostTypeId = entry.JCCType;

            EMCo = entry.EMCo;
            EquipmentId = entry.EquipmentId;
            EMGroupId = entry.EMGroupId;
            CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;

            if (entry.JobId != null)
                JobName = entry.Job?.DisplayName;
            if (entry.PhaseId != null)
                PhaseName = entry.JobPhase?.Description;
            if (entry.EquipmentId != null)
                EquipmentName = entry.Equipment?.DisplayName;
            if (entry.CostCodeId != null)
                EMCostCodeName = entry.EMCostCode?.Description;

            GLCo = entry.GLCo;
            GLAcct = entry.GLAcct;

            Units = entry.Units;
            UM = entry.UM;
            UnitCost = entry.UnitCost;
            GrossAmt = entry.GrossAmt;
            MiscAmt = entry.MiscAmt;

            TaxGroupId = entry.TaxGroup;
            TaxTypeId = (DB.TaxTypeEnum)(entry.TaxTypeId ?? 0);
            TaxCodeId = entry.TaxCodeId;
            TaxAmount = entry.TaxAmt;

            Comments = entry.Notes;
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
        public string Mth { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Mth")]
        public DateTime MthDate { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int APTransId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line")]
        public int APLineId { get; set; }
        public byte VendorGroupId { get; private set; }
        [UIHint("TextAreaBox")]
        [Display(Name = "Reference")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string APRef { get; set; }
        public byte POCo { get; private set; }
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorAllCombo", ComboForeignKeys = "POCo, VendorId")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/POItemCombo", ComboForeignKeys = "POCo, PO")]
        [Display(Name = "Item")]
        public short? POItem { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Type")]
        public DB.POItemTypeEnum? POItemTypeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Line Type")]
        [Field(Placeholder = "Line Type")]
        public DB.APLineTypeEnum LineTypeId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }
        public byte? JCCo { get; private set; }
        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }
        
        public byte? PhaseGroupId { get; private set; }
        
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JobCostTypeId { get; set; }
        
        public byte? EMCo { get; private set; }
        
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Vendor Name")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string VendorName { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }
        
        public byte? EMGroupId { get; private set; }
        
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostTypeCombo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }
        public byte GLCo { get; private set; }
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 4, TextSize = 4)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 4, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=Co")]
        [Display(Name = "UM")]
        public string UM { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        [Field(LabelSize = 4, TextSize = 8)]
        public decimal? UnitCost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 4, TextSize = 8)]
        public decimal? GrossAmt { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(LabelSize = 2, TextSize = 4, Placeholder = "Tax Type")]
        public DB.TaxTypeEnum? TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        //[UIHint("IntegerBox")]
        //[Field(LabelSize = 0, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        //[Display(Name = "Tax Rate")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        //public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 2, TextSize = 4)]
        public decimal? TaxAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Misc Amt")]
        [Field(LabelSize = 2, TextSize = 4)]
        public decimal? MiscAmt { get; set; }



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
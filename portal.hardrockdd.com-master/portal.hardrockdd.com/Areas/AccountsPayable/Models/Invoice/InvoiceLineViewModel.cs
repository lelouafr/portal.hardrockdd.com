using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models
{
    public class InvoiceLineListViewModel
    {
        public InvoiceLineListViewModel()
        {

        }

        public InvoiceLineListViewModel(DB.Infrastructure.ViewPointDB.Data.APTran invoice)
        {
            if (invoice == null)
            {
                throw new System.ArgumentNullException(nameof(invoice));
            }
            APCo = invoice.APCo;
            Mth = invoice.Mth.ToShortDateString();
            APTransId = invoice.APTransId;

            List = invoice.Lines.Select(s => new InvoiceLineViewModel(s)).ToList();
        }


        public InvoiceLineListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            JCCo = job.JCCo;
            JobId = job.JobId;
            //APTransId = invoice.APTransId;

            List = job.APInvoiceLines.Select(s => new InvoiceLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Request Id")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int APTransId { get; set; }

        public byte JCCo { get; set; }

        public string JobId { get; set; }

        public List<InvoiceLineViewModel> List { get; }
    }

    public class InvoiceLineViewModel
    {
        public InvoiceLineViewModel()
        {

        }

        public InvoiceLineViewModel(DB.Infrastructure.ViewPointDB.Data.APTransLine entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            APCo = entry.APCo;
            Mth = entry.Mth.ToShortDateString();
            MthDate = entry.Mth;
            APTransId = entry.APTransId;
            LineId = entry.APLineId;

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
            EMGroupId = entry.EMGroupId ?? entry.APTran.APCompanyParm.HQCompanyParm.EMGroupId;
            EquipmentId = entry.EquipmentId;
            CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;

            GLCo = entry.GLCo;
            GLAcct = entry.GLAcct;

            Units = entry.Units;
            UM = entry.UM;
            UnitCost = entry.UnitCost;
            GrossAmt = entry.GrossAmt;
            MiscAmt = entry.MiscAmt;

            TaxGroupId = entry.TaxGroup ?? entry.APTran.APCompanyParm.HQCompanyParm.TaxGroupId;
            TaxTypeId = (DB.TaxTypeEnum)(entry.TaxTypeId ?? 0);
            TaxCodeId = entry.TaxCodeId;
            TaxAmount = entry.TaxAmt;

            Comments = entry.Notes;
        }
        [HiddenInput]
        public byte? POCo { get; set; }
        [HiddenInput]
        public byte? GLCo { get; set; }
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
        public byte APCo { get; set; }

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
        public int LineId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Reference")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string APRef { get; set; }

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

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }

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

        [UIHint("TextBox")]
        [Display(Name = "Vendor Name")]
        [Field(LabelSize = 2, TextSize = 10)]
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
        [Field(LabelSize = 4, TextSize = 4)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 4, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=APCo")]
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
    }

    
}
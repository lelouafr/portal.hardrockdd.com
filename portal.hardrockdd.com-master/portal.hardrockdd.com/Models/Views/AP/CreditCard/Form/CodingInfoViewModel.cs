using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class CodingInfoListViewModel
    {
        public CodingInfoListViewModel()
        {

        }

        public CodingInfoListViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            CCCo = transaction.CCCo;
            TransId = transaction.TransId;

            List = transaction.Coding.Select(s => new CodingInfoViewModel(s)).ToList();
        }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        public long TransId { get; set; }

        public List<CodingInfoViewModel> List { get;  }
    }
    public class CodingInfoViewModel
    {
        public CodingInfoViewModel()
        {

        }

        public CodingInfoViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransactionCode transactionCode)
        {
            if (transactionCode == null) throw new System.ArgumentNullException(nameof(transactionCode));

            CCCo = transactionCode.CCCo;
            TransId = transactionCode.TransId;
            SeqId = transactionCode.SeqId;
            LineTypeId = (DB.CMCodeLineTypeEnum?)transactionCode.tLineTypeId;

            PRCo = transactionCode.Transaction.PRCo;
            EmplopyeeId = transactionCode.Transaction.EmployeeId;

            VendorGroupId = transactionCode.Transaction.Merchant.Vendor.VendorGroupId;
            VendorId = transactionCode.Transaction.Merchant.VendorId;

            POCo = transactionCode.POCo ?? 1;
            PO = transactionCode.PO;
            POItem = transactionCode.POItemId;
            POItemTypeId = (DB.POItemTypeEnum?)transactionCode.POItemTypeId;

            Description = transactionCode.Description;
            CalcType = transactionCode.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;

            JCCo = transactionCode.JCCo ?? 1;
            JobId = transactionCode.JobId;
            PhaseGroupId = transactionCode.PhaseGroupId ?? 1;
            PhaseId = transactionCode.PhaseId;
            JobCostTypeId = transactionCode.JCCType;

            EMCo = transactionCode.EMCo ?? 1;
            EquipmentId = transactionCode.EquipmentId;
            EMGroupId = transactionCode.EMGroupId ?? 1;
            CostCodeId = transactionCode.CostCodeId ;
            CostTypeId = transactionCode.EMCType;

            GLCo = transactionCode.GLCo ?? 1;
            GLAcct = transactionCode.GLAcct;

            Units = transactionCode.Units;
            UM = transactionCode.UM;
            UnitCost = transactionCode.UnitCost;
            GrossAmt = transactionCode.GrossAmt;
            
            TaxGroupId = 1;
            TaxTypeId = (DB.TaxTypeEnum?)transactionCode.TaxTypeId;
            TaxRate = transactionCode.TaxRate;
            TaxCodeId = transactionCode.TaxCodeId;
            TaxAmount = transactionCode.TaxAmount;
        }


        [HiddenInput]
        public byte? POCo { get; set; }
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
        [UIHint("LongBox")]
        public byte CCCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public long TransId { get; set; }
        
        [Key]
        [UIHint("LongBox")]
        public int SeqId { get; set; }

        [Display(Name = "Emplopyee Id")]
        public int? EmplopyeeId { get; set; }

        [UIHint("DropDownBox")]
        [Display(Name = "Vendor")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        public int? VendorId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(LabelSize = 2, TextSize = 4, Placeholder = "Type")]
        public DB.CMCodeLineTypeEnum? LineTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo, VendorId")]
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

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/JCCombo/CCCombo", ComboForeignKeys = "CCCo, TransId", CacheUrl = false)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/CCCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/CCCombo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "Cost Type")]
        public byte? JobCostTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/EMCombo/CCCombo", ComboForeignKeys = "CCCo, TransId", CacheUrl = false)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostTypeCombo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/CCCombo", ComboForeignKeys = "GLCo, TransId", CacheUrl = false)]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=CCCo")]
        [Display(Name = "UM")]
        public string UM { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        public decimal? UnitCost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? GrossAmt { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Tax Type")]
        [Field(LabelSize = 2, TextSize = 4, Placeholder = "Tax Type")]
        public DB.TaxTypeEnum? TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 0, TextSize = 4, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 2, TextSize = 4)]
        public decimal? TaxAmount { get; set; }

    }
}
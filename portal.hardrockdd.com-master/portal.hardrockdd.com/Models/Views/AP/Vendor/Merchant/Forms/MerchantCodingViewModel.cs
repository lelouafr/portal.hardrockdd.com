using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantCodingViewModel
    {
        public MerchantCodingViewModel()
        {

        }

        public MerchantCodingViewModel(CreditMerchant merchant)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));
            VendGroupId = merchant.VendGroupId;
            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            MerchantId = merchant.MerchantId;
            IsReoccurring = merchant.IsReoccurring ?? false;
            IsReceiptRequired = merchant.IsReceiptRequired ?? false;
            IsAPRefRequired = merchant.IsAPRefRequired ?? false;
            DefaultGLAcct = merchant.DefaultGLAcct;
            DefaultJCPhaseId = merchant.DefaultJCPhaseId;
            DefaultJCCType = merchant.DefaultJCCType;
            DefaultEMCostCodeId = merchant.DefaultEMCostCodeId;
            DefaultEMCType = merchant.DefaultEMCType;
        }

        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public string MerchantId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Reoccuring?")]
        [Field(LabelSize = 3, TextSize = 3)]
        public bool? IsReoccurring { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Receipt Required?")]
        [Field(LabelSize = 3, TextSize = 3)]
        public bool? IsReceiptRequired { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "AP Ref Required?")]
        [Field(LabelSize = 3, TextSize = 3)]
        public bool? IsAPRefRequired { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Default GL Acct")]
        public string DefaultGLAcct { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/PhaseMaster/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Phase")]
        public string DefaultJCPhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/PhaseMasterCostType/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Cost Type")]
        public byte? DefaultJCCType { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/EMCombo/EMCostCodeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Code")]
        public string DefaultEMCostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/EquipmentCostType/APCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }
        
        public string CategoryGroup { get; set; }
        
        public int? CategoryCodeId { get; set; }

    }
}
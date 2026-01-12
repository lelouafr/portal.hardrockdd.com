using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantCategoryCodingViewModel
    {
        public MerchantCategoryCodingViewModel()
        {

        }

        public MerchantCategoryCodingViewModel(CreditMerchantCategory category)
        {
            if (category == null) throw new System.ArgumentNullException(nameof(category));
            VendGroupId = category.VendGroupId;
            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            CategoryCodeId = category.CategoryCodeId;
            CategoryGroup = category.CategoryGroup;
            //IsReoccurring = category.IsReoccurring ?? false;
            //IsReceiptRequired = category.IsReceiptRequired ?? false;
            DefaultGLAcct = category.DefaultGLAcct;
            DefaultJCPhaseId = category.DefaultJCPhaseId;
            DefaultJCCType = category.DefaultJCCType;
            DefaultEMCostCodeId = category.DefaultEMCostCodeId;
            DefaultEMCType = category.DefaultEMCType;
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
        public string CategoryGroup { get; set; }

        [HiddenInput]
        [Key]
        public int CategoryCodeId { get; set; }

        //[UIHint("SwitchBox")]
        //public bool? IsReoccurring { get; set; }

        //[UIHint("SwitchBox")]
        //public bool? IsReceiptRequired { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Default GL Acct")]
        public string DefaultGLAcct { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Phase")]
        public string DefaultJCPhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Cost Type")]
        public byte? DefaultJCCType { get; set; }

        public byte? EMGroup { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Code")]
        public string DefaultEMCostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostTypeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }
        

    }
}
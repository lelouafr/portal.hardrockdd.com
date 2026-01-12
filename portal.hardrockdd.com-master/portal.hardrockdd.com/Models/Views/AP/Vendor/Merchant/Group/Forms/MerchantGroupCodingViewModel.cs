using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantGroupCodingViewModel
    {
        public MerchantGroupCodingViewModel()
        {

        }

        public MerchantGroupCodingViewModel(CreditMerchantGroup group)
        {
            if (group == null) throw new System.ArgumentNullException(nameof(group));
            VendGroupId = group.VendGroupId;

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            CategoryGroup = group.CategoryGroup;
            //IsReoccurring = group.IsReoccurring ?? false;
            //IsReceiptRequired = group.IsReceiptRequired ?? false;
            DefaultGLAcct = group.DefaultGLAcct;
            DefaultJCPhaseId = group.DefaultJCPhaseId;
            DefaultJCCType = group.DefaultJCCType;
            DefaultEMCostCodeId = group.DefaultEMCostCodeId;
            DefaultEMCType = group.DefaultEMCType;
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

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Code")]
        public string DefaultEMCostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EquipmentCostType/APCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }
        

    }
}
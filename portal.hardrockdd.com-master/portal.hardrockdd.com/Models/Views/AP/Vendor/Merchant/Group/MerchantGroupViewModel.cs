using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant
{
    public class MerchantGroupListViewModel
    {
        public MerchantGroupListViewModel()
        {

        }

        public MerchantGroupListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            VendGroupId = (byte)company.VendorGroupId;
            List = company.MerchantGroups.Select(s => new MerchantGroupViewModel(s)).ToList();
        }

        public byte VendGroupId { get; set; }

        public List<MerchantGroupViewModel> List { get; }
    
    }
    public class MerchantGroupViewModel
    {
        public MerchantGroupViewModel()
        {

        }

        public MerchantGroupViewModel(CreditMerchantGroup group)
        {
            if (group == null) throw new System.ArgumentNullException(nameof(group));

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            VendGroupId = group.VendGroupId;
            CategoryGroup = group.CategoryGroup;
            Description = group.Description;
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

        [Key]
        public string CategoryGroup { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }


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
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Code")]
        public string DefaultEMCostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EquipmentCostType/Combo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }

    }
}
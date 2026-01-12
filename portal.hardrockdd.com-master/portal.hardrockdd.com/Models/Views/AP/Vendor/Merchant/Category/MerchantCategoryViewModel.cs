using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant
{
    public class MerchantCategoryListViewModel
    {
        public MerchantCategoryListViewModel()
        {

        }

        public MerchantCategoryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            VendGroupId = (byte)company.VendorGroupId;
            List = company.MerchantCategories.Select(s => new MerchantCategoryViewModel(s)).ToList();
        }

        public MerchantCategoryListViewModel(CreditMerchantGroup group)
        {
            if (group == null) throw new System.ArgumentNullException(nameof(group));
            VendGroupId = group.VendGroupId;
            CategoryGroup = group.CategoryGroup;

            List = group.Categories.Select(s => new MerchantCategoryViewModel(s)).ToList();
        }

        public byte VendGroupId { get; set; }

        public string CategoryGroup { get; set; }


        public List<MerchantCategoryViewModel> List { get; }
    
    }
    public class MerchantCategoryViewModel
    {
        public MerchantCategoryViewModel()
        {

        }

        public MerchantCategoryViewModel(CreditMerchantCategory category)
        {
            if (category == null) throw new System.ArgumentNullException(nameof(category));
            VendGroupId = category.VendGroupId;
            EMGroup = category.Company.EMGroupId;

            CategoryGroup = category.CategoryGroup;
            CategoryCodeId = category.CategoryCodeId;
            Description = category.Description;
            DefaultGLAcct = category.DefaultGLAcct;
            DefaultJCPhaseId = category.DefaultJCPhaseId;
            DefaultJCCType = category.DefaultJCCType;
            DefaultEMCostCodeId = category.DefaultEMCostCodeId;
            DefaultEMCType = category.DefaultEMCType;
        }


        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [Key]
        public string CategoryGroup { get; set; }
        [Key]
        public int CategoryCodeId { get; set; }

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
        [Field(ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "Job Cost Type")]
        public byte? DefaultJCCType { get; set; }

        public byte? EMGroup { get; set; }

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
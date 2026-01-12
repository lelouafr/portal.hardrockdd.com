using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant
{
    public class MerchantListViewModel
    {
        public MerchantListViewModel()
        {
            List = new List<MerchantViewModel>();
        }

        public MerchantListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            VendGroupId = (byte)company.VendorGroupId;

            List = company.Merchants.Select(s => new MerchantViewModel(s)).ToList();
        }

        public MerchantListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            VendGroupId = (byte)company.VendorGroupId;

            List = db.vCreditMerchants.Where(f => f.VendGroupId == VendGroupId).AsEnumerable().Select(s => new MerchantViewModel(s)).ToList();
        }

        public MerchantListViewModel(CreditMerchantCategory category)
        {
            if (category == null) throw new System.ArgumentNullException(nameof(category));

            VendGroupId = category.VendGroupId;
            CategoryId = category.CategoryCodeId;

            List = category.Merchants.Select(s => new MerchantViewModel(s)).ToList();
        }

        [Key]
        public byte VendGroupId { get; set; }
        
        [Key]
        public int CategoryId { get; set; }

        public List<MerchantViewModel> List { get; }
    
    }
    public class MerchantViewModel
    {
        public MerchantViewModel()
        {

        }

        public MerchantViewModel(CreditMerchant merchant)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            VendGroupId = merchant.VendGroupId;
            MerchantId = merchant.MerchantId;
            CategoryCodeId = merchant.CategoryCodeId;
            CategoryGroup = merchant.CategoryGroup;
            CategoryGroupDescription = merchant.Category.Group.Description;
            CategoryCodeDescription = merchant.Category.Description;
            Name = merchant.Name;
            Address = merchant.Address;
            City = merchant.City;
            State = merchant.State;
            Zip = merchant.Zip;
            PhoneNumber = merchant.PhoneNumber;
            CountryCode = merchant.CountryCode;
            VendorId = merchant.VendorId;
            VendorName = merchant.Vendor?.Name;
            IsReoccurring = merchant.IsReoccurring;
            IsReceiptRequired = merchant.IsReceiptRequired;
            IsAPRefRequired = merchant.IsAPRefRequired;
            DefaultGLAcct = merchant.DefaultGLAcct;
            DefaultJCPhaseId = merchant.DefaultJCPhaseId;
            DefaultJCCType = merchant.DefaultJCCType;
            DefaultEMCostCodeId = merchant.DefaultEMCostCodeId;
            DefaultEMCType = merchant.DefaultEMCType;
            //var startDate = DateTime.Now.AddYears(-1);
            //var trans = merchant.Transactions.Where(f => f.TransDate >= startDate);
            //TransTotal = trans.Sum(sum => sum.TransAmt);
            //TransCount = trans.Count();
        }

        public MerchantViewModel(vCreditMerchant merchant)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            VendGroupId = merchant.VendGroupId;
            MerchantId = merchant.MerchantId;
            CategoryCodeId = merchant.CategoryCodeId;
            CategoryGroup = merchant.CategoryGroup;
            CategoryGroupDescription = merchant.CategoryGroupDescription;
            CategoryCodeDescription = merchant.CategoryCodeDescription;
            Name = merchant.Name;
            Address = merchant.Address;
            City = merchant.City;
            State = merchant.State;
            Zip = merchant.Zip;
            PhoneNumber = merchant.PhoneNumber;
            CountryCode = merchant.CountryCode;
            VendorId = merchant.VendorId;
            VendorName = merchant.VendorName;
            IsReoccurring = merchant.IsReoccurring;
            IsReceiptRequired = merchant.IsReceiptRequired;
            IsAPRefRequired = merchant.IsAPRefRequired;
            DefaultGLAcct = merchant.DefaultGLAcct;
            DefaultJCPhaseId = merchant.DefaultJCPhaseId;
            DefaultJCCType = merchant.DefaultJCCType;
            DefaultEMCostCodeId = merchant.DefaultEMCostCodeId;
            DefaultEMCType = merchant.DefaultEMCType;
            TransTotal = merchant.TransTotal ?? 0;
            TransCount = merchant.TransCount ?? 0;
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

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "Country=CountryCode")]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [UIHint("DropDownBox")]
        [Display(Name = "Vendor")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReoccurring { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReceiptRequired { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsAPRefRequired { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string DefaultGLAcct { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Phase")]
        public string DefaultJCPhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
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

        public string CategoryGroup { get; set; }

        [UIHint("TextBox")]
        public string CategoryGroupDescription { get; set; }

        public int? CategoryCodeId { get; set; }

        [UIHint("TextBox")]
        public string CategoryCodeDescription { get; set; }

        public int TransCount { get; set; }

        public decimal TransTotal { get; set; }
    }
}
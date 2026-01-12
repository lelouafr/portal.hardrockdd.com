using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor
{
    public class VendorListViewModel
    {
        public VendorListViewModel()
        {
            List = new List<VendorViewModel>();
        }

        public VendorListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;

            List = company.VendorGroup.APVendors.Select(s => new VendorViewModel(s)).ToList();
        }
        [Key]
        public byte Co { get; set; }
        
        public List<VendorViewModel> List { get; }
    
    }
    public class VendorViewModel
    {
        public VendorViewModel()
        {

        }

        public VendorViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            VendorId = vendor.VendorId;

            StatusString = vendor.ActiveYN == "Y" ? "Active" : "Inactive";

            Name = vendor.Name;
            Address = vendor.Address;
            City = vendor.City;
            State = vendor.State;
            Zip = vendor.Zip;
            Phone = vendor.Phone;
            Country = vendor.Country;
            VendorName = vendor.DisplayName;
            IsReoccurring = vendor.IsReoccurring ?? false;
            IsReceiptRequired = vendor.IsReceiptRequired ?? false;
            DefaultGLAcct = vendor.DefaultGLAcct;
            DefaultJCPhaseId = vendor.DefaultJCPhaseId;
            DefaultJCCType = vendor.DefaultJCCType;
            DefaultEMCostCodeId = vendor.DefaultEMCostCodeId;
            DefaultEMCType = vendor.DefaultEMCType;
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
        public int VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string StatusString { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "Country=Country")]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Country Code")]
        public string Country { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReoccurring { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReceiptRequired { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
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
        [Field(ComboUrl = "/EMCombo/EMCostTypeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }

    }
}
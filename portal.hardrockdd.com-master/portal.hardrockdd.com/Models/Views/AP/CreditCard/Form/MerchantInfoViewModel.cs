using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class MerchantInfoViewModel
    {
        public MerchantInfoViewModel()
        {

        }

        public MerchantInfoViewModel(DB.Infrastructure.ViewPointDB.Data.CreditMerchant merchant)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));

            VendGroupId = merchant.VendGroupId;
            MerchantId = merchant.MerchantId;
            Name = merchant.Name;
            Address = merchant.Address;
            City = merchant.City;
            State = merchant.State;
            Zip = merchant.Zip;
            PhoneNumber = merchant.PhoneNumber;
            CountryCode = merchant.CountryCode;
            VendorId = merchant.VendorId;

            VendorGroupId = merchant.Company.VendorGroupId;
        }

        [HiddenInput]
        public byte? VendorGroupId { get; set; }

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
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        public int? VendorId { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
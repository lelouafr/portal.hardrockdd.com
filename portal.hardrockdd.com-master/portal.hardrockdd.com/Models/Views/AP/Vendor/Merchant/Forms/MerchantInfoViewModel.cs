using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantInfoViewModel
    {
        public MerchantInfoViewModel()
        {

        }

        public MerchantInfoViewModel(CreditMerchant merchant)
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
            VendorName = merchant.Vendor?.Name;

        }

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
        [Field(TextSize = 10,ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


    }
}
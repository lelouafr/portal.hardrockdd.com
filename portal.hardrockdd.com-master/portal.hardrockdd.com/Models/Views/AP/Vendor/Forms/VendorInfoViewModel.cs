using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Forms
{
    public class VendorInfoViewModel
    {
        public VendorInfoViewModel()
        {

        }

        public VendorInfoViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;
            Name = vendor.Name;
            Address = vendor.Address;
            City = vendor.City;
            State = vendor.State;
            Zip = vendor.Zip;
            Phone = vendor.Phone;
            Country = vendor.Country;
            VendorName = vendor.DisplayName;

        }


        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        public int VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("TextBox")]
        [Field(TextSize = 10)]
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
        [Field(TextSize = 10)]
        [Display(Name = "Display Name")]
        public string VendorName { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


    }
}
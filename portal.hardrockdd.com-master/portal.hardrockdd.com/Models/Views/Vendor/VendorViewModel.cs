using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Vendor
{
    public class VendorListViewModel
    {
        public VendorListViewModel()
        {

        }

        public VendorListViewModel(DB.Infrastructure.ViewPointDB.Data.HQGroup APGroup)
        {
            if (APGroup == null)
            {
                throw new System.ArgumentNullException(nameof(APGroup));
            }
            #region mapping
            VendorGroupId = APGroup.GroupId;
            #endregion

            List = APGroup.APVendors.Select(s => new VendorViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte VendorGroupId { get; set; }

        public List<VendorViewModel> List { get; }

        public string ListJSON { get; }

    }

    public class VendorViewModel
    {
        public VendorViewModel()
        {

        }

        public VendorViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor Vendor)
        {
            if (Vendor == null)
            {
                throw new ArgumentNullException(nameof(Vendor));
            }
            #region mapping
            VendorGroupId = Vendor.VendorGroupId;
            VendorId = Vendor.VendorId;
            Name = Vendor.Name;
            SortName = Vendor.SortName;
            DisplayName = Vendor.DisplayName;
            Contact = Vendor.Contact;
            Address = Vendor.Address;
            Address2 = Vendor.Address2;
            City = Vendor.City;
            State = Vendor.State;
            Zip = Vendor.Zip;
            Phone = Vendor.Phone;
            Fax = Vendor.Fax;
            EMail = Vendor.EMail;
            URL = Vendor.URL;
            Notes = Vendor.Notes;
            #endregion
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Group")]
        public byte VendorGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Number")]
        public int VendorId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Sort Name")]
        public string SortName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 2)]
        [Display(Name = "Contact")]
        public string Contact { get; set; }
        
        [UIHint("PhoneBox")]
        [Field(FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Field(FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [UIHint("EMailBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 4)]
        [Display(Name = "EMail")]
        public string EMail { get; set; }

        //[UIHint("URLBox")]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 5)]
        [Display(Name = "Web Site")]
        public string URL { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 1)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 2)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [UIHint("TextBox")]
        [Field(FormGroup = "Address", FormGroupRow = 3)]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        [Field(FormGroup = "Address", FormGroupRow = 3, Placeholder = "Select State", ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo=VendorGroupId")]
        [Display(Name = "State")]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 4)]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Notes", FormGroupRow = 1)]
        [UIHint("TextAreaBox")]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}

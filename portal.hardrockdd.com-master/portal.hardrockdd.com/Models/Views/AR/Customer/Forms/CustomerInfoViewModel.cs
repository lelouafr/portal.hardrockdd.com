using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Forms
{
    public class CustomerInfoViewModel
    {
        public CustomerInfoViewModel()
        {

        }

        public CustomerInfoViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;
            Name = customer.Name;
            Contact = customer.Contact;
            Address = customer.Address;
            Address2 = customer.Address2;
            City = customer.City;
            State = customer.State;
            Zip = customer.Zip;
            Phone = customer.Phone;
            Fax = customer.Fax;
            EMail = customer.EMail;
            URL = customer.URL;
            Notes = customer.Notes;
        }

        [HiddenInput]
        [Key]
        public byte CustGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public int CustomerId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Name")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Name { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Contact")]
        public string Contact { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [UIHint("EMailBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "EMail")]
        public string EMail { get; set; }

        //[UIHint("URLBox")]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Web Site")]
        public string URL { get; set; }


        [UIHint("TextBox")]
        [Field(TextSize = 10)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "Country=Country")]
        //[Field(ComboUrl = "/State/Combo", ComboForeignKeys = "")]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Country Code")]
        public string Country { get; set; }

        [Field(LabelSize = 2, TextSize = 10)]
        [UIHint("TextAreaBox")]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
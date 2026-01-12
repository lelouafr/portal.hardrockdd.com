using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Firm
{
   
    [DisplayClass("PM Firm")]
    public class FirmViewModel
    {
        public FirmViewModel()
        {

        }

        public FirmViewModel(DB.Infrastructure.ViewPointDB.Data.Firm firm)
        {
            if (firm != null)
            {
                #region mapping
                VendorGroupId = firm.VendorGroupId;
                FirmNumber = firm.FirmNumber;
                FirmName = firm.FirmName;
                ContactName = firm.ContactName;
                MailAddress = firm.MailAddress;
                MailCity = firm.MailCity;
                MailState = firm.MailState;
                MailZip = firm.MailZip;
                MailAddress2 = firm.MailAddress2;
                Phone = firm.Phone;
                Fax = firm.Fax;
                EMail = firm.EMail;
                URL = firm.URL;
                Notes = firm.Notes;
                #endregion
            }
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Group")]
        public byte VendorGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Firm Number")]
        public int FirmNumber { get; set; }

        [HiddenInput]
        public string FirmTypeId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Name")]
        public string FirmName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 2)]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        
        [UIHint("PhoneBox")]
        [Field(FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Field(FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
               
        [UIHint("EmailBox")]
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
        public string MailAddress { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 2)]
        [Display(Name = "Address 2")]
        public string MailAddress2 { get; set; }

        [UIHint("TextBox")]
        [Field(FormGroup = "Address", FormGroupRow = 3)]
        [Display(Name = "City")]
        public string MailCity { get; set; }

        [UIHint("DropdownBox")]
        [Field(FormGroup = "Address", FormGroupRow = 3, ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "Country=Country")]
        //[Field(FormGroup = "Address", FormGroupRow = 3, Placeholder = "Select State", ComboUrl = "/State/Combo", ComboForeignKeys = "")]
        [Display(Name = "State")]
        public string MailState { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 4)]
        [Display(Name = "Zip")]
        public string MailZip { get; set; }

        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Notes", FormGroupRow = 1)]
        [UIHint("TextAreaBox")]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}

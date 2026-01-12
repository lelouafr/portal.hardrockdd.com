using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Contact
{
    public class CustomerContactListViewModel
    {
        public CustomerContactListViewModel()
        {

        }

        public CustomerContactListViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null)
            {
                throw new System.ArgumentNullException(nameof(customer));
            }
            #region mapping
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;
            #endregion

            List = customer.Contacts.Select(s => new ContactViewModel(s.Contact, customer)).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte CustGroupId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int CustomerId { get; set; }

        public List<ContactViewModel> List { get; }
    }

    public class ContactViewModel
     {
        public ContactViewModel()
        {

        }

        public ContactViewModel(DB.Infrastructure.ViewPointDB.Data.Contact contact, DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            #region mapping
            ContactGroupId = contact.ContactGroupId;
            ContactId = contact.ContactId;
            CustomerId = customer.CustomerId;
            FirstName = contact.FirstName;
            MiddleInitial = contact.MiddleInitial;
            LastName = contact.LastName;
            Title = contact.Title;
            Address = contact.Address;
            AddressAdditional = contact.AddressAdditional;
            City = contact.City;
            State = contact.State;
            Zip = contact.Zip;
            Phone = contact.Phone;
            Fax = contact.Fax;
            Email = contact.Email;
            Notes = contact.Notes;
            #endregion
        }

        public ContactViewModel(DB.Infrastructure.ViewPointDB.Data.Contact contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            #region mapping
            ContactGroupId = contact.ContactGroupId;
            ContactId = contact.ContactId;
            FirstName = contact.FirstName;
            MiddleInitial = contact.MiddleInitial;
            LastName = contact.LastName;
            FullName = FullName + " " + LastName;
            Title = contact.Title;
            Address = contact.Address;
            AddressAdditional = contact.AddressAdditional;
            City = contact.City;
            State = contact.State;
            Zip = contact.Zip;
            Phone = contact.Phone;
            Fax = contact.Fax;
            Email = contact.Email;
            Notes = contact.Notes;
            #endregion
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Group")]
        public byte ContactGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Contact Number")]
        public int ContactId { get; set; }
        
        [HiddenInput]
        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Middle")]
        public string MiddleInitial { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 0)]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 3)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Contact", FormGroupRow = 1)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Contact", FormGroupRow = 1)]
        [Display(Name = "Cell")]
        public string Cell { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Contact", FormGroupRow = 2)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [UIHint("EMailBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Email")]
        public string Email { get; set; }        

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 1)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Address", FormGroupRow = 2)]
        [Display(Name = "Address 2")]
        public string AddressAdditional { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Address", FormGroupRow = 3)]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 4, FormGroup = "Address", FormGroupRow = 3, Placeholder = "Select State", ComboUrl = "/State/Combo", ComboForeignKeys = "")]
        [Field(FormGroup = "Address", FormGroupRow = 3, ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "Country=Country")]
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

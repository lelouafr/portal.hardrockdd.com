using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer
{
    public class CustomerListViewModel
    {
        public CustomerListViewModel()
        {
            List = new List<CustomerViewModel>();
        }

        public CustomerListViewModel(DB.Infrastructure.ViewPointDB.Data.HQGroup company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            CustGroupId = (byte)company.GroupId;

            List = company.ARCustomers.Where(f => f.CustomerId < 90000).Select(s => new CustomerViewModel(s)).ToList();
        }

        [Key]
        public byte CustGroupId { get; set; }

        public List<CustomerViewModel> List { get; }


    }
    public class CustomerViewModel
    {
        public CustomerViewModel()
        {

        }

        public CustomerViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            #region mapping
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;
            Name = customer.Name;
            Status = customer.Status switch
            {
                "I" => "Inactive",
                "A" => "Active",
                "H" => "Hold",
                _ => "Unknown",
            };
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
            if (customer.ARTrans.Where(f => f.ARTransType =="I").Any())
            {
                LastInvoiceDate = customer.ARTrans.Where(f => f.ARTransType == "I").Max(max => max.TransDate);
                Amount = customer.ARTrans.Where(f => f.ARTransType == "I").Sum(sum => sum.Invoiced);
                PaidAmount = customer.ARTrans.Where(f => f.ARTransType == "I").Sum(sum => sum.Paid);
                Balance = (Amount ?? 0) - (PaidAmount ?? 0);
            }
            #endregion
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Group")]
        public byte CustGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int CustomerId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Info", FormGroupRow = 2)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Contact", FormGroupRow = 2)]
        [Display(Name = "Contact")]
        public string Contact { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Contact", FormGroupRow = 3)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Contact", FormGroupRow = 3)]
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
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Address", FormGroupRow = 3)]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Address", FormGroupRow = 3, Placeholder = "Select State", ComboUrl = "/State/Combo", ComboForeignKeys = "")]
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

        [UIHint("DateBox")]
        [Display(Name = "Last Invoice Date")]
        public DateTime? LastInvoiceDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Paid")]
        public decimal? PaidAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Balance")]
        public decimal? Balance { get; set; }
    }
}

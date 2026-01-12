using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Project.Models.Bid
{
    public class CustomerListViewModel
    {
        public CustomerListViewModel()
        {

        }

        public CustomerListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion

            List = bid.Customers.Select(s => new CustomerViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        public List<CustomerViewModel> List { get; }
    }

    public class CustomerViewModel
    {
        public CustomerViewModel()
        {

        }

        public CustomerViewModel(DB.Infrastructure.ViewPointDB.Data.BidCustomer bidCustomer)
        {
            if (bidCustomer == null)
            {
                throw new System.ArgumentNullException(nameof(bidCustomer));
            }
            BDCo = bidCustomer.BDCo;
            BidId = bidCustomer.BidId;
            LineId = bidCustomer.LineId;
            CustomerId = bidCustomer.CustomerId;
            ContactId = bidCustomer.ContactId;

            if (bidCustomer.Customer != null)
                Customer = new portal.Models.Views.AR.Customer.CustomerViewModel(bidCustomer.Customer);

            if (bidCustomer.Contact != null)
                Contact = new portal.Models.Views.AR.Contact.ContactViewModel(bidCustomer.Contact);
        }

        [Key]
        [Required]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        public int BidId { get; set; }

        [Key]
        [Required]
        public int LineId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/AllCombo", ComboForeignKeys = "CustGroupId=BDCo", AddUrl = "/Customer/FormAdd", AddForeignKeys = "CustGroupId=BDCo")]
        public int? CustomerId { get; set; }


        [Required]
        [Display(Name = "Contact")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/ContactCombo", ComboForeignKeys = "CustGroupId=BDCo, CustomerId", AddUrl = "/Contact/Add", AddForeignKeys = "CustGroupId=BDCo, CustomerId", EditUrl = "/Contact/Form", EditForeignKeys = "CustGroupId=BDCo, CustomerId, ContactId")]
        public int? ContactId { get; set; }


        public portal.Models.Views.AR.Customer.CustomerViewModel Customer { get; set; }

        public portal.Models.Views.AR.Contact.ContactViewModel Contact { get; set; }


        internal CustomerViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.BidCustomers.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.LineId == LineId);

            if (updObj != null)
            {
                if (CustomerId != updObj.CustomerId)
                    ContactId = null;

                updObj.CustomerId = CustomerId;
                updObj.ContactId = ContactId;
                try
                {
                    db.BulkSaveChanges();
                    return new CustomerViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidCustomerListViewModel
//    {
//        public BidCustomerListViewModel()
//        {

//        }

//        public BidCustomerListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion

//            List = bid.Customers.Select(s => new BidCustomerViewModel(s)).ToList();
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        public List<BidCustomerViewModel> List { get; }
//    }

//    public class BidCustomerViewModel
//    {
//        public BidCustomerViewModel()
//        {

//        }

//        public BidCustomerViewModel(DB.Infrastructure.ViewPointDB.Data.BidCustomer bidCustomer)
//        {
//            if (bidCustomer == null)
//            {
//                throw new System.ArgumentNullException(nameof(bidCustomer));
//            }
//            Co = bidCustomer.Co;
//            BidId = bidCustomer.BidId;
//            LineId = bidCustomer.LineId;
//            CustomerId = bidCustomer.CustomerId;
//            ContactId = bidCustomer.ContactId;
//            Status = (DB.BidStatusEnum)(bidCustomer.BidStatus ?? bidCustomer.Bid.Status);
//            if (bidCustomer.Customer != null)
//            {
//                Customer = new AR.Customer.CustomerViewModel(bidCustomer.Customer);
//            }

//            if (bidCustomer.Contact != null)
//            {
//                Contact = new AR.Contact.ContactViewModel(bidCustomer.Contact);
//            }
//        }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int BidId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int LineId { get; set; }

//        [Required]
//        [Display(Name = "Customer")]
//        [UIHint("DropDownBox")]
//        [Field(ComboUrl = "/ARCombo/AllCombo", ComboForeignKeys = "Co", AddUrl = "/Customer/FormAdd", AddForeignKeys = "Co")]
//        public int? CustomerId { get; set; }

//        [Required]
//        [Display(Name = "Contact")]
//        [UIHint("DropDownBox")]
//        [Field(ComboUrl = "/ARCombo/ContactCombo", ComboForeignKeys = "Co, CustomerId", AddUrl = "/Contact/Add", AddForeignKeys = "Co, CustomerId", EditUrl = "/Contact/Form", EditForeignKeys = "Co, CustomerId, ContactId")]
//        public int? ContactId { get; set; }

//        [ReadOnly(true)]
//        [UIHint("EnumBox")]
//        [Display(Name = "Status")]
//        public DB.BidStatusEnum Status { get; set; }

//        public AR.Customer.CustomerViewModel Customer { get; set; }

//        public AR.Contact.ContactViewModel Contact { get; set; }
//    }
//}
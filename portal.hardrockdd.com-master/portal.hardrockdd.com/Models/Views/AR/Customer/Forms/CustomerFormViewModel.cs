using portal.Models.Views.AR.Invoice;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Forms
{
    public class CustomerFormViewModel
    {
        public CustomerFormViewModel()
        {
            Info = new CustomerInfoViewModel();
            Invoices = new InvoiceListViewModel();
        }

        public CustomerFormViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;

            Info = new CustomerInfoViewModel(customer);
            Invoices = new InvoiceListViewModel(customer);
            TotalStats = new CustomerStatViewModel(customer);
            YearStats = new CustomerStatListViewModel(customer);
            Jobs = new CustomerJobListViewModel(customer);
        }

        [HiddenInput]
        [Key]
        public byte CustGroupId { get; set; }

        [HiddenInput]
        [Key]
        public int CustomerId { get; set; }

        public CustomerInfoViewModel Info { get; set; }

        public InvoiceListViewModel Invoices { get; set; }

        public CustomerStatViewModel TotalStats { get; set; }

        public CustomerStatListViewModel YearStats { get; set; }

        public CustomerJobListViewModel Jobs { get; set; }

    }
}
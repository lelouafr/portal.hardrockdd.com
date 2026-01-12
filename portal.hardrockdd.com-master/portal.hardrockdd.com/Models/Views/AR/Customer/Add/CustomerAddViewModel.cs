using portal.Models.Views.AR.Invoice;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Forms
{
    public class CustomerAddViewModel: CustomerInfoViewModel
    {
        public CustomerAddViewModel()
        {

        }

        public CustomerAddViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer): base(customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            
        }
    }
}
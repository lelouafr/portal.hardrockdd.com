using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Invoice;
using portal.Models.Views.Purchase.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Forms
{
    public class VendorFormViewModel
    {
        public VendorFormViewModel()
        {

        }

        public VendorFormViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;

            Info = new VendorInfoViewModel(vendor);
            Coding = new VendorCodingViewModel(vendor);
            Merchant = new VendorMerchantViewModel(vendor);
            Invoices = new InvoiceListViewModel(vendor);
            PurchaseOrders = new PurchaseOrderListViewModel(vendor);
        }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public int VendorId { get; set; }

        public VendorInfoViewModel Info { get; set; }
        
        public VendorCodingViewModel Coding { get; set; }

        public VendorMerchantViewModel Merchant { get; set; }

        public InvoiceListViewModel Invoices { get; set; }
        
        public PurchaseOrderListViewModel PurchaseOrders { get; set; }
    }
}
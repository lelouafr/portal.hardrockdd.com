using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Forms
{
    public class VendorInvoiceViewModel
    {
        public VendorInvoiceViewModel()
        {

        }

        public VendorInvoiceViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;
        }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public int VendorId { get; set; }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantFormViewModel
    {
        public MerchantFormViewModel()
        {

        }

        public MerchantFormViewModel(CreditMerchant merchant)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));
            VendGroupId = merchant.VendGroupId;
            MerchantId = merchant.MerchantId;

            Info = new MerchantInfoViewModel(merchant);
            Coding = new MerchantCodingViewModel(merchant);
        }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public string MerchantId { get; set; }

        public MerchantInfoViewModel Info { get; set; }
        
        public MerchantCodingViewModel Coding { get; set; }
    }
}
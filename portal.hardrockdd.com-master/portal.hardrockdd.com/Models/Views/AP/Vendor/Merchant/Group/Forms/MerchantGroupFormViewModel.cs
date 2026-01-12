using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantGroupFormViewModel
    {
        public MerchantGroupFormViewModel()
        {

        }

        public MerchantGroupFormViewModel(CreditMerchantGroup group)
        {
            if (group == null) throw new System.ArgumentNullException(nameof(group));
            VendGroupId = group.VendGroupId;
            CategoryGroup = group.CategoryGroup;

            Info = new MerchantGroupInfoViewModel(group);
            Coding = new MerchantGroupCodingViewModel(group);
        }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public string CategoryGroup { get; set; }

        public MerchantGroupInfoViewModel Info { get; set; }
        
        public MerchantGroupCodingViewModel Coding { get; set; }
    }
}
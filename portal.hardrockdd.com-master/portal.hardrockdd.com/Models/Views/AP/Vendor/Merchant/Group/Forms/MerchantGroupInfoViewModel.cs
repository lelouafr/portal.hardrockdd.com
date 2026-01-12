using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantGroupInfoViewModel
    {
        public MerchantGroupInfoViewModel()
        {

        }

        public MerchantGroupInfoViewModel(CreditMerchantGroup group)
        {
            if (group == null) throw new System.ArgumentNullException(nameof(group));
            VendGroupId = group.VendGroupId;
            CategoryGroup = group.CategoryGroup;
            Name = group.Description;

        }


        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Group")]
        public string CategoryGroup { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }



    }
}
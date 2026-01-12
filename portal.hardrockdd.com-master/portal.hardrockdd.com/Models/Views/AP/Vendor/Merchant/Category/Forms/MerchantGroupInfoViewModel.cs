using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantCategoryInfoViewModel
    {
        public MerchantCategoryInfoViewModel()
        {

        }

        public MerchantCategoryInfoViewModel(CreditMerchantCategory category)
        {
            if (category == null) throw new System.ArgumentNullException(nameof(category));
            VendGroupId = category.VendGroupId;
            CategoryGroup = category.CategoryGroup;
            CategoryCodeId = category.CategoryCodeId;
            Name = category.Description;

        }


        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public string CategoryGroup { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Category")]
        public int CategoryCodeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }



    }
}
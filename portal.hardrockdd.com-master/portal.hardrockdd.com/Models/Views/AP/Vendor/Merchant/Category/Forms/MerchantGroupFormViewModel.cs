using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantCategoryFormViewModel
    {
        public MerchantCategoryFormViewModel()
        {

        }

        public MerchantCategoryFormViewModel(CreditMerchantCategory category)
        {
            if (category == null) throw new System.ArgumentNullException(nameof(category));
            VendGroupId = category.VendGroupId;
            CategoryGroup = category.CategoryGroup;
            CategoryCodeId = category.CategoryCodeId;

            Info = new MerchantCategoryInfoViewModel(category);
            Coding = new MerchantCategoryCodingViewModel(category);
        }

        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public string CategoryGroup { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Key]
        public int CategoryCodeId { get; set; }

        public MerchantCategoryInfoViewModel Info { get; set; }
        public MerchantCategoryCodingViewModel Coding { get; set; }
    }
}
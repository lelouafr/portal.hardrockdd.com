using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Forms
{
    public class VendorMerchantViewModel
    {
        public VendorMerchantViewModel()
        {

        }

        public VendorMerchantViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;
            MatchString = vendor.MerchantMatchString;
            if (!string.IsNullOrEmpty(MatchString))
            {
                MatchList = MatchString.Split(';').ToList();
            }
            else
            {
                MatchList = new List<string>();
                MatchList.Add("");
            }
        }


        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        public int VendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Match String")]
        [Field(TextSize = 10)]
        public string MatchString { get; set; }

        public List<string> MatchList { get; set; }


    }
}
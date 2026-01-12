using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Asset.Form
{
    

    public class AssetCreateViewModel
    {
        public AssetCreateViewModel()
        {

        }
        
        //public AssetViewModel(Code.Data.VP.HRCompanyAsset asset)
        //{
        //    if (asset == null) throw new System.ArgumentNullException(nameof(asset));

        //    Co = asset.HRCo;
        //    AssetId = asset.AssetId;


        //    AssetCategory = asset.AssetCategory;
        //    AssetDesc = asset.AssetDesc;
        //    Manufacturer = asset.Manufacturer;
        //    PurchDate = asset.PurchDate;
        //    BookValue = asset.BookValue;
        //    Identifier = asset.Identifier;
        //    Serial = asset.udSerial;

        //    Employee = asset.CurrentAssignedEmployee().FullName();
        //    EmployeeId = asset.CurrentAssignedEmployee().HRRef;
        //}

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Id")]
        public string AssetId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Assigned Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HRResourceForm/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int EmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Assigned Employee")]
        public string Employee { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Category")]
        [Field(ComboUrl = "/HRCombo/AssetCategoryCombo", ComboForeignKeys = "HRCo")]
        public string AssetCategory { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string AssetDesc { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Purchase Date")]
        public DateTime? PurchDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Status")]
        public decimal? BookValue { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Id")]
        public string Identifier { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Serial")]
        public string Serial { get; set; }
    }
}
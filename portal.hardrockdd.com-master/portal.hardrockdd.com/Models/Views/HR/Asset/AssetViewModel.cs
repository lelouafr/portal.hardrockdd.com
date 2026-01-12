using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Asset
{
    public class AssetListViewModel
    {
        public AssetListViewModel()
        {
            List = new List<AssetViewModel>();
        }

        public AssetListViewModel(Code.Data.VP.HRCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HRCo;

            List = company.HRCompanyAssets.Select(s => new AssetViewModel(s)).ToList();
        }


        [Key]
        public byte Co { get; set; }
        
        public List<AssetViewModel> List { get;  }


        
    }

    public class AssetViewModel
    {
        public AssetViewModel()
        {

        }
        
        public AssetViewModel(Code.Data.VP.HRCompanyAsset asset)
        {
            if (asset == null) throw new System.ArgumentNullException(nameof(asset));

            HRCo = asset.HRCo;
            PRCo = (byte)asset.Assigned.PRCo;
            AssetId = asset.AssetId;


            AssetCategory = asset.AssetCategory;
            AssetDesc = asset.AssetDesc;
            Manufacturer = asset.Manufacturer;
            PurchDate = asset.PurchDate;
            BookValue = asset.BookValue;
            Identifier = asset.Identifier;
            Serial = asset.Serial;
            Status = asset.tStatusId;
            AssignedId = asset.tAssignedId;

            Employee = asset.Assigned.FullName();
            AssignedId = asset.AssignedId;
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Id")]
        public string AssetId { get; set; }

        [HiddenInput]
        public byte PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Assigned Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HRResourceForm/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? AssignedId { get; set; }

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

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public byte? Status { get; set; }
    }
}
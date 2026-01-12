using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class AssignedAssetListViewModel
    {
        public AssignedAssetListViewModel()
        {
            List = new List<AssignedAssetViewModel>();
        }


        public AssignedAssetListViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;

            List = resource.AssignedAssets.Select(s => new AssignedAssetViewModel(s)).ToList();
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public List<AssignedAssetViewModel> List { get;  }
    }

    public class AssignedAssetViewModel
    {
        public AssignedAssetViewModel()
        {

        }
        
        public AssignedAssetViewModel(HRAssetAssignment assetAssignment)
        {
            if (assetAssignment == null) throw new System.ArgumentNullException(nameof(assetAssignment));

            HRCo = assetAssignment.HRCo;
            ResourceId = assetAssignment.HRRef;

            AssetId = assetAssignment.AssetId;
            Category = assetAssignment.Asset.AssetCategory;
            Description = assetAssignment.Asset.AssetDesc;
            Id = assetAssignment.Asset.Identifier;
            Serial = assetAssignment.Asset.Serial;
            Date = assetAssignment.DateOut;
            ReturnDate = assetAssignment.DateIn;
            Status = assetAssignment.DateIn != null ? "Returned" : "Assigned";
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Asset Id")]
        public string AssetId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Serial")]
        public string Serial { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string Status { get; set; }

    }
}
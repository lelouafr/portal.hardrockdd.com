using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Assets
{
    public class AssetListViewModel
    {
        public AssetListViewModel()
        {

        }

        public AssetListViewModel(HRCompanyParm company)
        {
            if (company == null)
                return;

            List = company.HRCompanyAssets.Select(s => new AssetViewModel(s)).ToList();
        }

        public List<AssetViewModel>? List { get; }
    }

    public class AssetViewModel
    {
        public AssetViewModel()
        {
        }

        public AssetViewModel(HRCompanyParm company)
        {
            if (company == null)
                return;

            HRCo = company.HRCo;
        }

        public AssetViewModel(HRCompanyAsset asset)
        {
            if (asset == null)
                return;

            HRCo = asset.HRCo;
            AssetId = asset.AssetId;


            AssetCategory = asset.AssetCategory;
            AssetDesc = asset.AssetDesc;
            Manufacturer = asset.Manufacturer;
            ModelYear = asset.ModelYear;
            ModelName = asset.Model;
            PurchDate = asset.PurchDate;
            BookValue = asset.BookValue;
            Identifier = asset.Identifier;
            Phone = asset.Phone;
            Serial = asset.Serial?.Replace("\n", "").Replace("\r", "");
            Status = asset.Status;

            Assigned = asset.tAssignedId;

            AssignedName = asset.Assigned?.FullName(false);

        }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte HRCo { get; set; }

        [Key]
        [Required]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public string AssetId { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Category")]
        [Field(ComboUrl = "/HRCombo/AssetCategoryCombo", ComboForeignKeys = "HRCo")]
        public string AssetCategory { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string AssetDesc { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Model Year")]
        public string ModelYear { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Identifier")]
        public string Identifier { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Purchase Date")]
        public DateTime? PurchDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Value")]
        public decimal? BookValue { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Assigned To")]
        [Field(ComboUrl = "/HRCombo/EmployeeCombo", ComboForeignKeys = "HRCo")]

        public int? Assigned { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Assigned To")]
        public string AssignedName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "MemoInOut")]
        public string MemoInOut { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRAssestStatusEnum Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Serial")]
        public string Serial { get; set; }


        public AssetViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.HRCompanyAssets.FirstOrDefault(f => f.HRCo == this.HRCo && f.AssetId == this.AssetId);

            if (updObj != null)
            {
                updObj.AssetDesc = this.AssetDesc;
                if( updObj.AssignedId != this.Assigned)
                    updObj.AssignedId = this.Assigned;



                updObj.AssetCategory = this.AssetCategory;
                updObj.AssetDesc = this.AssetDesc;
                updObj.Manufacturer = this.Manufacturer;
                updObj.ModelYear = this.ModelYear;
                updObj.Model = this.ModelName;
                updObj.PurchDate = this.PurchDate;
                updObj.BookValue = this.BookValue;
                updObj.Identifier = this.Identifier;
                updObj.Phone = this.Phone;
                updObj.Serial = this.Serial;
                if ((Status == DB.HRAssestStatusEnum.Disposed || Status == DB.HRAssestStatusEnum.Available))
                {
                    updObj.Status = this.Status;

                }
                else
                {
                    updObj.Status = updObj.Assigned != null ? DB.HRAssestStatusEnum.Assigned : this.Status;

                }

                if (updObj.AssetCategory == "Service" && updObj.Assigned != null)
                {
                    if (updObj.Phone != null && updObj.Status == DB.HRAssestStatusEnum.Assigned && updObj.Assigned.CellPhone != this.Phone)
                        updObj.Assigned.CellPhone = this.Phone;
                    else if (updObj.Status != DB.HRAssestStatusEnum.Assigned && updObj.Assigned.CellPhone == this.Phone)
                        updObj.Assigned.CellPhone = null;
                }
                else if (updObj.AssetCategory == "Phone" && updObj.Assigned != null)
                {
                    if (updObj.Phone != null && updObj.Status == DB.HRAssestStatusEnum.Assigned && updObj.Assigned.WorkPhone != this.Phone)
                        updObj.Assigned.WorkPhone = this.Phone;
                    else if (updObj.Status != DB.HRAssestStatusEnum.Assigned && updObj.Assigned.WorkPhone == this.Phone)
                        updObj.Assigned.WorkPhone = null;
                }
                if ((Status == DB.HRAssestStatusEnum.Disposed || Status == DB.HRAssestStatusEnum.Available))
                    if (updObj.Assigned != null)
                        updObj.Assigned = null;

                try
                {
                    db.BulkSaveChanges();
                    return new AssetViewModel(updObj);
                }
                catch (Exception ex)
                {
                    if(modelState != null)
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                if (modelState != null)
                    modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
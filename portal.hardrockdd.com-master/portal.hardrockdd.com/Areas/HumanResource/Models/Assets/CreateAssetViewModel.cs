using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Assets
{
    
    public class CreateViewModel
    {
        public CreateViewModel()
        {
        }

        public CreateViewModel(HRCompanyParm company)
        {
            HRCo = company.HRCo;
        }

        public CreateViewModel(HRCompanyAsset asset)
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
            Serial = asset.Serial;
            //Status = asset.tStatusId;

            Assigned = asset.AssignedId;

            AssignedName = asset.Assigned.FullName(false);

        }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte HRCo { get; set; }

        [Key]
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
        [Display(Name = "ModelYear")]
        public string ModelYear { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Identifier")]
        public string Identifier { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "PurchDate")]
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

        //[UIHint("TextBox")]
        //[Display(Name = "MemoInOut")]
        //public string MemoInOut { get; set; }

        //[UIHint("TextBox")]
        //[Display(Name = "Status")]
        //public byte? Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Serial")]
        public string Serial { get; set; }


        internal CreateViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.HRCompanyAssets.FirstOrDefault(f => f.HRCo == this.HRCo && f.AssetId == this.AssetId);

            if (updObj != null)
            {
                updObj.AssetDesc = this.AssetDesc;
                updObj.AssignedId = this.Assigned;
                updObj.Identifier = this.Identifier;
                updObj.Serial = this.Serial;
                updObj.Phone = this.Phone;
                updObj.Manufacturer = this.Manufacturer;
                updObj.ModelYear = this.ModelYear;
                updObj.Model = this.ModelName;
                updObj.PurchDate = this.PurchDate;
                updObj.BookValue = this.BookValue;

                try
                {
                    db.BulkSaveChanges();
                    return new CreateViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
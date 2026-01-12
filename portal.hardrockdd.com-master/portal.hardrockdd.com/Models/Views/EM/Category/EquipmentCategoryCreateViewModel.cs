using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Category
{
    public class EquipmentCategoryCreateViewModel
    {
        public EquipmentCategoryCreateViewModel()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();

            Co = emp.HRCo;

        }

        public EquipmentCategoryCreateViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;
        }
        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            using var db = new VPContext();
            if (!db.EquipmentCategories.Any(f => f.EMCo == Co && f.CategoryId == CategoryId))
            {
                modelState.AddModelError("CategoryId", "Category Id already exist.");
            }

            return modelState.IsValid;
        }
        [Key]
        [Required]
        [HiddenInput]
        public byte Co { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string CategoryId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Category
{
    public class EquipmentCategoryListViewModel
    {
        public EquipmentCategoryListViewModel()
        {

        }

        public EquipmentCategoryListViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceItem serviceItem)
        {
            if (serviceItem == null) throw new System.ArgumentNullException(nameof(serviceItem));

            Co = serviceItem.EMCo;
            ServiceItemId = serviceItem.ServiceItemId;

            List = serviceItem.CategoryLinks.Select(s => new EquipmentCategoryViewModel(s)).ToList();
        }


        public EquipmentCategoryListViewModel(DB.Infrastructure.ViewPointDB.Data.EMCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.EMCo;

            List = company.Categories.Select(s => new EquipmentCategoryViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }


        [Key]
        [HiddenInput]
        public int ServiceItemId { get; set; }


        public List<EquipmentCategoryViewModel> List { get;  }
    }

    public class EquipmentCategoryViewModel
    {

        public EquipmentCategoryViewModel()
        {

        }
        public EquipmentCategoryViewModel(EMServiceCategoryLink link)
        {
            if (link == null) 
                throw new System.ArgumentNullException(nameof(link));
            var cat = link.Category;

            Co = cat.EMCo;
            CategoryId = cat.CategoryId;
            Description = cat.Description;

            AssignedEquipment = new EquipmentListViewModel(cat);
        }

        public EquipmentCategoryViewModel(EquipmentCategory cat)
        {
            if (cat == null) throw new System.ArgumentNullException(nameof(cat));

            Co = cat.EMCo;
            CategoryId = cat.CategoryId;
            Description = cat.Description;

            AssignedEquipment = new EquipmentListViewModel(cat);
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name ="Category")]
        public string CategoryId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public EquipmentListViewModel AssignedEquipment { get; }
    }
}
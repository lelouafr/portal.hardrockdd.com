using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Service.Forms
{
    public class CategoryLinkListViewModel
    {
        public CategoryLinkListViewModel()
        {

        }

        public CategoryLinkListViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceItem serviceItem)
        {
            if (serviceItem == null) 
                throw new System.ArgumentNullException(nameof(serviceItem));

            Co = serviceItem.EMCo;
            ServiceItemId = serviceItem.ServiceItemId;

            List = serviceItem.CategoryLinks.Select(s => new CategoryLinkViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public int ServiceItemId { get; set; }


        public List<CategoryLinkViewModel> List { get;  }
    }
    public class CategoryLinkViewModel
    {

        public CategoryLinkViewModel()
        {

        }

        public CategoryLinkViewModel(EMServiceCategoryLink link)
        {
            if (link == null) 
                throw new System.ArgumentNullException(nameof(link));

            EMCo = link.EMCo;
            LinkId = link.LinkId;
            ServiceItemId = link.ServiceItemId;
            CategoryId = link.CategoryId;

        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Link Id")]
        public int LinkId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Service Item Id")]
        public int? ServiceItemId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/CategoryCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }
    }
}
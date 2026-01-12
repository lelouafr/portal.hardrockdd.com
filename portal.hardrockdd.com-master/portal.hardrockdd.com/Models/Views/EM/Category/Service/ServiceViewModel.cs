using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Category.Service
{
    public class ServiceListViewModel
    {
        public ServiceListViewModel()
        {

        }

        public ServiceListViewModel(DB.Infrastructure.ViewPointDB.Data.EquipmentCategory category)
        {
            if (category == null) 
                throw new System.ArgumentNullException(nameof(category));

            Co = category.EMCo;

            List = category.ServiceLinks.Select(s => new ServiceLinkViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }


        public List<ServiceLinkViewModel> List { get;  }
    }
    public class ServiceLinkViewModel
    {

        public ServiceLinkViewModel()
        {

        }

        public ServiceLinkViewModel(EMServiceCategoryLink link)
        {
            if (link == null) 
                throw new System.ArgumentNullException(nameof(link));

            Co = link.EMCo;
            CategoryId = link.CategoryId;
            LinkId = link.LinkId;
            CategoryId = link.CategoryId;
            ServiceItemId = link.ServiceItemId;
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int LinkId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }


        [UIHint("LongBox")]
        [Display(Name = "Service")]
        public int? ServiceItemId { get; set; }

        public EquipmentListViewModel AssignedEquipment { get; }
    }
}
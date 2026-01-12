using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Service
{
    public class EquipmentServiceListViewModel
    {
        public EquipmentServiceListViewModel()
        {

        }

        public EquipmentServiceListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;

            List = company.ServiceItems.Select(s => new EquipmentServiceViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }


        public List<EquipmentServiceViewModel> List { get;  }
    }
    public class EquipmentServiceViewModel
    {

        public EquipmentServiceViewModel()
        {

        }

        public EquipmentServiceViewModel(EMServiceItem service)
        {
            if (service == null) throw new System.ArgumentNullException(nameof(service));

            Co = service.EMCo;
            ServiceItemId = service.ServiceItemId;
            Description = service.Description;

        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name ="#")]
        public int ServiceItemId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}
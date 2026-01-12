using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Location
{
    public class EquipmentLocationListViewModel
    {
        public EquipmentLocationListViewModel()
        {

        }

        public EquipmentLocationListViewModel(DB.Infrastructure.ViewPointDB.Data.EMCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            EMCo = company.EMCo;

            List = company.Locations.Select(s => new EquipmentLocationViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }


        public List<EquipmentLocationViewModel> List { get; }
    }
    public class EquipmentLocationViewModel
    {

        public EquipmentLocationViewModel()
        {

        }

        public EquipmentLocationViewModel(EMLocation loc)
        {
            if (loc == null) throw new System.ArgumentNullException(nameof(loc));

            EMCo = loc.EMCo;
            LocationId = loc.LocationId;
            Description = loc.Description;

            AssignedEquipment = new EquipmentListViewModel(loc);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name ="Location")]
        public string LocationId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public EquipmentListViewModel AssignedEquipment { get;  }
    }
}
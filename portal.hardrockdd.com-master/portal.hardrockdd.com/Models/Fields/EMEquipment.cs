using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.Fields
{
    public interface IEMEquipmentField
    {

        public byte EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/EmployeeCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public int EquipmentId { get; set; }

        public string EquipmentName { get; set; }
    }
}
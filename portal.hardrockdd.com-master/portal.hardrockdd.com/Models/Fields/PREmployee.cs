using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Fields

{
    public interface IPREmployeeField
    {

        public byte PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/EmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Safety.Models.Safety
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.HRResource resource)
        {
            if (resource == null)
                return;

            HRCo = resource.HRCo;
            HRRef = resource.HRRef;

            EmployeeInfo = new EmployeeInfoViewModel(resource);
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        public int HRRef { get; set; }

        public EmployeeInfoViewModel EmployeeInfo { get; set; }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Admin.Models.Office
{
    public class OfficeFormViewModel
    {
        public OfficeFormViewModel()
        {

        }

        public OfficeFormViewModel(HQOffice office)
        {
            Info = new OfficeViewModel(office);
            OfficeId = office.OfficeId;
        }

        [Key]
        public int OfficeId { get; set; }

        public OfficeViewModel Info { get; set; }
    }
}
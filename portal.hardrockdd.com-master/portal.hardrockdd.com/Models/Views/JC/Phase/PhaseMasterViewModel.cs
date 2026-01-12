using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PhaseMaster
{
    public class PhaseMasterListViewModel
    {
        public PhaseMasterListViewModel()
        {

        }

        public PhaseMasterListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, int vendorId)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            #region mapping
            Co = company.HQCo;
            VendorId = vendorId;
            #endregion

            List = company.PhaseGroup.JCPhases.Select(s => new PhaseMasterViewModel(s)).ToList();
            List.ForEach(c => c.VendorId = vendorId);
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public PhaseMasterListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            #region mapping
            Co = company.HQCo;
            #endregion

            List = company.PhaseGroup.JCPhases.Select(s => new PhaseMasterViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Description = "Co")]
        public byte Co { get; set; }

        public List<PhaseMasterViewModel> List { get; }

        public string JobId { get; set; }

        public int? VendorId { get; set; }

        public string ListJSON { get; }
    }

    public class PhaseMasterViewModel
    {
        public PhaseMasterViewModel()
        {

        }

        public PhaseMasterViewModel(DB.Infrastructure.ViewPointDB.Data.PhaseMaster PhaseMaster)
        {
            if (PhaseMaster == null)
            {
                throw new ArgumentNullException(nameof(PhaseMaster));
            }
            #region mapping
            PhaseGroupId = PhaseMaster.PhaseGroupId;
            PhaseId = PhaseMaster.PhaseId;
            Description = PhaseMaster.Description;
            #endregion
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Description = "PhaseMaster Group")]
        public byte PhaseGroupId { get; set; }

        [Key]
        [Required]
        [Display(Description = "PhaseMaster Number")]
        public string PhaseId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Description = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Description = "Sort Description")]
        public string SortDescription { get; set; }


        public int? VendorId { get; set; }
    }
}

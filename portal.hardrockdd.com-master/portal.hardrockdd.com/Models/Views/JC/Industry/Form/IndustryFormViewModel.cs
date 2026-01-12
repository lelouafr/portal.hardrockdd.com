using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Industry.Form
{
    public class IndustryFormViewModel
    {
        public IndustryFormViewModel()
        {

        }

        public IndustryFormViewModel(JCIndustry industry)
        {
            if (industry == null) throw new System.ArgumentNullException(nameof(industry));

            JCCo = industry.JCCo;
            IndustryId = industry.IndustryId;
            Info = new IndustryInfoViewModel(industry);
            Markets = new IndustryAssignedMarketListViewModel(industry);
        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int IndustryId { get; set; }

        public IndustryInfoViewModel Info { get; set; }

        public IndustryAssignedMarketListViewModel Markets { get; set; }

    }
}
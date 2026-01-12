using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Industry
{
    public class IndustryListViewModel
    {
        public IndustryListViewModel()
        {

        }

        public IndustryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            JCCo = company.HQCo;

            List = company.JobIndustries.Select(s => new IndustryViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "JCCo")]
        public byte JCCo { get; set; }

        public List<IndustryViewModel> List { get; }
    }

    public class IndustryViewModel
    {
        public IndustryViewModel()
        {

        }

        public IndustryViewModel(DB.Infrastructure.ViewPointDB.Data.JCIndustry industry)
        {
            if (industry == null)
                throw new System.ArgumentNullException(nameof(industry));

            JCCo = industry.JCCo;
            IndustryId = industry.IndustryId;
            Description = industry.Description;
        }

        [Key]
        [Required]
        [Display(Name = "JCCo")]
        public byte JCCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int IndustryId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }



        internal IndustryViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.JCIndustries.FirstOrDefault(f => f.JCCo == JCCo && f.IndustryId == IndustryId);

            if (updObj != null)
            {
                updObj.Description = Description;

                try
                {
                    db.BulkSaveChanges();
                    return new IndustryViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

    }
}
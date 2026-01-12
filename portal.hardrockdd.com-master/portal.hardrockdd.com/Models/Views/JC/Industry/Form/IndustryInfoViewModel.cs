using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Industry.Form
{
    public class IndustryInfoViewModel
    {
        public IndustryInfoViewModel()
        {

        }

        public IndustryInfoViewModel(JCIndustry industry)
        {
            if (industry == null) throw new System.ArgumentNullException(nameof(industry));

            JCCo = industry.JCCo;
            IndustryId = industry.IndustryId;

            Description = industry.Description;
        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        [Field(LabelSize = 4, TextSize = 8)]
        public int IndustryId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Description { get; set; }



        internal IndustryInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.JCIndustries.FirstOrDefault(f => f.JCCo == JCCo && f.IndustryId == IndustryId);

            if (updObj != null)
            {
                updObj.Description = Description;

                try
                {
                    db.BulkSaveChanges();
                    return new IndustryInfoViewModel(updObj);
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
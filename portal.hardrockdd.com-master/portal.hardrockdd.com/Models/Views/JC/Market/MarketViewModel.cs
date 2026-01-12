using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Market
{
    public class MarketListViewModel
    {
        public MarketListViewModel()
        {

        }

        public MarketListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            JCCo = company.HQCo;
            List = company.JobMarkets.Select(s => new MarketViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "JCCo")]
        public byte JCCo { get; set; }

        public List<MarketViewModel> List { get; }
    }

    public class MarketViewModel
    {
        public MarketViewModel()
        {

        }

        public MarketViewModel(DB.Infrastructure.ViewPointDB.Data.JCMarket industry)
        {
            if (industry == null)
                throw new System.ArgumentNullException(nameof(industry));

            JCCo = industry.JCCo;
            MarketId = industry.MarketId;
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
        public int MarketId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        internal MarketViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.JCMarkets.FirstOrDefault(f => f.JCCo == JCCo && f.MarketId == MarketId);

            if (updObj != null)
            {
                updObj.Description = Description;

                try
                {
                    db.BulkSaveChanges();
                    return new MarketViewModel(updObj);
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
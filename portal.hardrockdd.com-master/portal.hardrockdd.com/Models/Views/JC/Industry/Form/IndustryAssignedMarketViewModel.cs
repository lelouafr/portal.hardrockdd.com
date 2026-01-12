using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Market;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Industry.Form
{
    public class IndustryAssignedMarketListViewModel
    {
        public IndustryAssignedMarketListViewModel()
        {
            List = new List<IndustryAssignedMarketViewModel>();
        }


        public IndustryAssignedMarketListViewModel(JCIndustry industry)
        {
            if (industry == null) throw new System.ArgumentNullException(nameof(industry));

            JCCo = industry.JCCo;
            IndustryId = industry.IndustryId;

            List = industry.Markets.Select(s => new IndustryAssignedMarketViewModel(s)).ToList();
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int IndustryId { get; set; }

        public List<IndustryAssignedMarketViewModel> List { get;  }
    }
    public class IndustryAssignedMarketViewModel
    {
        public IndustryAssignedMarketViewModel()
        {
        }


        public IndustryAssignedMarketViewModel(JCIndustryMarket market)
        {
            if (market == null) throw new System.ArgumentNullException(nameof(market));

            JCCo = market.JCCo;
            IndustryId = market.IndustryId;
            SeqId = market.SeqId;

            MarketId = market.MarketId;
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int IndustryId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/JCMarketCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Market")]
        public int? MarketId { get; set; }
    }

}
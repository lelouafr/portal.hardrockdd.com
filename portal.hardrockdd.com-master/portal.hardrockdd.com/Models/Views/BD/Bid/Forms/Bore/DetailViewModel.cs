using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Bore
{
    public class DetailListViewModel
    {
        public DetailListViewModel()
        {

        }

        public DetailListViewModel(BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion
            //using var db = new VPContext();

            var boreSummary = package.vBidBoreLines.ToList();
            List = package.ActiveBoreLines.Select(s => new DetailViewModel(s, boreSummary.FirstOrDefault(f => f.BoreId == s.BoreId) )).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }


        public List<DetailViewModel> List { get; set; }

    }

    public class DetailViewModel: BoreLineViewModel
    {
        public DetailViewModel()
        {

        }

        public DetailViewModel(BidBoreLine line, vBidBoreLine lineSummary) : base(line)
        {

            //var boreSummary = db.vBidBoreLines.Where(f => f.Co == line.Co && f.BidId == line.BidId && f.BoreId == line.BoreId);
            if (lineSummary == null)
            {
                return;
            }
            DirtCost = 0;
            RockCost = 0;
            if (lineSummary.Footage != 0)
            {
                DirtCost = Math.Round(lineSummary.DirtCost / lineSummary.Footage, 2);
                RockCost = Math.Round(lineSummary.RockCost / lineSummary.Footage, 2);
            }
            DirtDays = Math.Round(lineSummary.DirtDays, 2, MidpointRounding.AwayFromZero);
            RockDays = Math.Round(lineSummary.RockDays, 2, MidpointRounding.AwayFromZero);

        }

        [Display(Name = "Dirt Cost")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? DirtCost { get; set; }

        [Display(Name = "Rock Cost")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? RockCost { get; set; }

        [Display(Name = "Dirt Days")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? DirtDays { get; set; }

        [Display(Name = "Rock Days")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? RockDays { get; set; }


        internal new DetailViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            base.ProcessUpdate(db, modelState);

            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);
            if (updObj != null)
            {
                var boreSummary = updObj.Package.vBidBoreLines.FirstOrDefault(f => f.BoreId == updObj.BoreId);
                return new DetailViewModel(updObj, boreSummary);
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
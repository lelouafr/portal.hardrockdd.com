using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid
{

    public class BidTrackerViewModel
    {
        public BidTrackerViewModel()
        {

        }

        public BidTrackerViewModel(DB.Infrastructure.ViewPointDB.Data.vBid bid, Web.WebUserViewModel CurrentUser)
        {
            if (bid == null) throw new System.ArgumentNullException(nameof(bid));
            if (CurrentUser == null) throw new System.ArgumentNullException(nameof(CurrentUser));

            BDCo = bid.BDCo;
            BidId = bid.BidId;
            StartDate = bid.StartDate ?? new DateTime(2000, 1, 1);
            StartDateString = StartDate?.ToString("MM/dd/yyyy", AppCultureInfo.CInfo());
            BidDate = bid.BidDate ?? DateTime.Now.Date;
            BidDateString = BidDate.ToString("MM/dd/yyyy", AppCultureInfo.CInfo());
            Description = bid.Description;
            FirmName = bid.Owner;
            Location = bid.Location;
            Status = (DB.BidStatusEnum)(bid.Status ?? 0);
            StatusString = Status.DisplayName();

            CreatedUser = bid.CreatedByUser;// Repository.VP.WP.WebUserRepository.EntityToModel(bid.CreatedUser, "");

            //var pricing = BidPackagePricingViewModel()

            //foreach (var bore in bid.ActiveBoreLines.GroupBy(g => g.PipeSize).ToList())
            //{
            //    if (!String.IsNullOrEmpty(PipeSize))
            //    {
            //        PipeSize += "/";
            //    }
            //    PipeSize += bore.Key.ToString();
            //}
            Division = bid.Division;
            PercentChance = bid.PercentChance;
            PipeSize = bid.PipeSizeStr;
            Footage = bid.Footage ?? 0;
            //Footage = bid.ActiveBoreLines.Sum(s => s.Footage ?? 0);
            //Sum(cs => cs.Multiplier * cs.Cost * cs.Units)
            DirtRevenue = bid.DirtRevenue ?? 0;// bid.Packages.Where(f => f.Status != (int)DB.BidStatusEnum.Deleted).SelectMany(bore => bore.BoreLines.Where(f => f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled)).SelectMany(cost => cost.CostItems).Where(f => f.GroundDensityId == 0).Sum(s => (s.Multiplier * s.Cost * s.Units) ?? 0);
            RockRevenue = bid.RockRevenue ?? 0;//bid.Packages.Where(f => f.Status != (int)DB.BidStatusEnum.Deleted).SelectMany(bore => bore.BoreLines.Where(f => f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled)).SelectMany(cost => cost.CostItems).Where(f => f.GroundDensityId != 0).Sum(s => (s.Multiplier * s.Cost * s.Units) ?? 0);

            AssigedTo = bid.AssignedToStr;
            SearchString = bid.SearchString;
            CanAwarded = false;
            if (Status == DB.BidStatusEnum.Proposal && AssigedTo != null)
            {
                if (AssigedTo.Contains(CurrentUser.FullName))
                {
                    CanAwarded = true;
                }
            }
        //    AssigedTo = "";

            //    foreach (var flow in bid.WorkFlows.Where(f => f.Active == "Y").GroupBy(g => g.AssignedUser).Select(s => new { AssignedUser = s.Key }).ToList())
            //    {
            //        if (!string.IsNullOrEmpty(AssigedTo))
            //        {
            //            AssigedTo += "; ";
            //        }
            //        AssigedTo += flow.AssignedUser.FirstName + " " + flow.AssignedUser.LastName;
            //    }
        }

        [Key]
        [HiddenInput]
        [ReadOnly(true)]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [ReadOnly(true)]
        [Display(Name = "BidId")]
        public int BidId { get; set; }

        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }

        [ReadOnly(true)]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidStatusEnum Status { get; set; }

        public string StatusString { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Bid Date")]
        public DateTime BidDate { get; set; }

        public string BidDateString { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        public string StartDateString { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Division")]
        [UIHint("TextBox")]
        public string Division { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Owner")]
        [UIHint("TextBox")]
        public string FirmName { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Location")]
        [UIHint("TextBox")]
        public string Location { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Pipe Size")]
        [UIHint("LongBox")]
        public string PipeSize { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Total Footage")]
        [UIHint("IntegerBox")]
        public decimal Footage { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Chance %")]
        [UIHint("PercentBox")]
        public decimal? PercentChance { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Dirt Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal DirtRevenue { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal RockRevenue { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Display(Name = "CreatedUser")]
        public string CreatedUser { get; set; }


        [ReadOnly(true)]
        [Display(Name = "Assigned To")]
        [UIHint("TextBox")]
        public string AssigedTo { get; set; }


        [ReadOnly(true)]
        [Display(Name = "Search String")]
        [UIHint("LongBox")]
        public string SearchString { get; set; }

        public bool CanAwarded { get; set; }


    }
}
using System;
using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Project.Models.Bid
{
    public class TrackerListViewModel
    {
        public TrackerListViewModel()
        {
            List = new List<TrackerViewModel>();
        }

        public TrackerListViewModel(VPContext db)
        {
            List = new List<TrackerViewModel>();
            if (db == null)
                return;

            var bids = db.vBids.Where(f => f.Status != (int)DB.BidStatusEnum.Template && f.Status != (int)DB.BidStatusEnum.Deleted && f.Status != (int)DB.BidStatusEnum.Canceled).OrderByDescending(o => o.BidDate).ToList();
            var user = db.GetCurrentUser();

            List = bids.Select(s => new TrackerViewModel(s, user)).ToList();
        }


        public TrackerListViewModel(WebUser user)
        {
            List = new List<TrackerViewModel>();
            if (user == null)
                return;

            var db = user.db;

            var workFlows = db.WorkFlowUsers
                            .Include("Sequence")
                            .Include("Sequence.WorkFlow")
                            .Include("Sequence.WorkFlow.Bids")
                            .Where(f => f.AssignedTo == user.Id && f.Sequence.Active && f.Sequence.WorkFlow.Bids.Any());

            var wfSeq = workFlows.Select(s => s.Sequence).Distinct();
            var wf = wfSeq.Select(s => s.WorkFlow).Distinct();
            var bids = wf.SelectMany(s => s.Bids).Distinct().ToList();
            var bidIds = bids.Select(s => s.BidId).ToList();

            var vbids = db.vBids.Where(f => bidIds.Contains(f.BidId)).ToList();
            
            List = vbids.Select(s => new TrackerViewModel(s, user)).ToList();
        }

        public List<TrackerViewModel> List { get; }
    }

    public class TrackerViewModel
    {
        public TrackerViewModel()
        {

        }

        public TrackerViewModel(vBid bid, WebUser user)
        {
            if (bid == null)
                return;

            if (user == null)
                user = new WebUser();

            BDCo = bid.BDCo;
            BidId = bid.BidId;
            StartDate = bid.StartDate ?? new DateTime(2000, 1, 1);
            StartDateString = StartDate?.ToString("MM/dd/yyyy", AppCultureInfo.CInfo());
            DueDate = bid.DueDate ?? new DateTime(2000, 1, 1);
            BidDate = bid.BidDate ?? DateTime.Now.Date;
            BidDateString = BidDate.ToString("MM/dd/yyyy", AppCultureInfo.CInfo());
            Description = bid.Description;
            FirmName = bid.Owner;
            Customer = bid.Customer;
            Location = bid.Location;
            Status = (DB.BidStatusEnum)(bid.Status ?? 0);
            StatusString = Status.DisplayName();

            CreatedUser = bid.CreatedByUser;

            Division = bid.Division;
            PercentChance = bid.PercentChance;
            PipeSize = bid.PipeSizeStr;
            Footage = bid.Footage ?? 0;
            DirtRevenue = bid.DirtRevenue ?? 0;
            RockRevenue = bid.RockRevenue ?? 0;

            

            AssigedTo = bid.AssignedToStr;
            SearchString = bid.SearchString;
            CanAwarded = false;
            if (Status == DB.BidStatusEnum.Proposal && AssigedTo != null)
            {
                if (AssigedTo.Contains(user.FullName()))
                {
                    CanAwarded = true;
                }
            }
        }

        [Key]
        [ReadOnly(true)]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [ReadOnly(true)]
        [Display(Name = "BidId")]
        public int BidId { get; set; }

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

        [UIHint("DateBox")]
        [Display(Name = "Bid Date")]
        public DateTime BidDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        public string BidDateString { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        public string StartDateString { get; set; }

        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [Display(Name = "Division")]
        [UIHint("TextBox")]
        public string Division { get; set; }

        [Display(Name = "Owner")]
        [UIHint("TextBox")]
        public string FirmName { get; set; }


        [Display(Name = "Customer")]
        [UIHint("TextBox")]
        public string Customer { get; set; }

        [Display(Name = "Location")]
        [UIHint("TextBox")]
        public string Location { get; set; }

        [Display(Name = "Pipe Size")]
        [UIHint("LongBox")]
        public string PipeSize { get; set; }

        [Display(Name = "Total Footage")]
        [UIHint("IntegerBox")]
        public decimal Footage { get; set; }

        [Display(Name = "Chance %")]
        [UIHint("PercentBox")]
        public decimal? PercentChance { get; set; }

        [Display(Name = "Dirt Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal DirtRevenue { get; set; }

        [Display(Name = "Rock Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal RockRevenue { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "CreatedUser")]
        public string CreatedUser { get; set; }


        [Display(Name = "Assigned To")]
        [UIHint("TextBox")]
        public string AssigedTo { get; set; }


        [Display(Name = "Search String")]
        [UIHint("LongBox")]
        public string SearchString { get; set; }

        public bool CanAwarded { get; set; }


    }
}
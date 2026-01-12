using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Bid
{

    public class BidUserSummaryViewModel
    {
        public BidUserSummaryViewModel()
        {

        }

        public BidUserSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            BidDate = bid.BidDate ?? DateTime.Now.Date;
            DueDate = bid.DueDate ?? BidDate;
            Description = bid.Description;
            CustomerName = bid?.Customer?.Name;
            FirmName = bid?.Firm?.FirmName;
            Location = bid.Location;
            Status = bid.Status;
            CreatedUser = new Web.WebUserViewModel(bid.CreatedUser);
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


        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Bid Date")]
        public DateTime BidDate { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Customer")]
        [UIHint("DropdownBox")]
        public string CustomerName { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Owner")]
        [UIHint("TextBox")]
        public string FirmName { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Location")]
        [UIHint("TextBox")]
        public string Location { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "CreatedUser")]
        public Web.WebUserViewModel CreatedUser { get; set; }


    }
}
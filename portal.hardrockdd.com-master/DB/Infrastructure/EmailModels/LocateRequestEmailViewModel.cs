using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DB.Infrastructure.EmailModels
{
    public class LocateRequestEmailViewModel
    {
        public LocateRequestEmailViewModel()
        {

        }

        public LocateRequestEmailViewModel(PMLocateRequest request)
        {
            if (request == null)
                return;
            
            RequestId = request.RequestId;

            Comments = request.Comments;
            RequestedOn = request.RequestedOn;
            RequestedBy = request.RequestedBy;

            RequestedOnStr = RequestedOn.ToShortDateString();
            Status = request.Status;
            if (request.Bid != null)
            {
                ProjectDesc = string.Format("{0}: {1}", request.Bid.BidId, request.Bid.Description);
                ProjectStartDate = request.Bid.StartDate;
                ProjectStartDateStr = ProjectStartDate.Value.ToShortDateString();
            }

            if (request.RequestedUser != null)
                RequestedUser = request.RequestedUser.FullName();

            Locates = request.Locates.Select(s => new LocateRequestLocateEmailViewModel(s)).ToList();
        }


        public int RequestId { get; set; }

        public string Comments { get; set; }

        public System.DateTime RequestedOn { get; set; }
        public string RequestedOnStr { get; set; }

        public string RequestedBy { get; set; }
        
        public string RequestedUser { get; set; }

        public DB.PMRequestStatusEnum Status { get; set; }


        public string ProjectDesc { get; set; }

        public DateTime? ProjectStartDate { get; set; }
        public string ProjectStartDateStr { get; set; }


        public List<LocateRequestLocateEmailViewModel> Locates { get; }

    }
    public class LocateRequestLocateEmailViewModel
    {

        public LocateRequestLocateEmailViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocate locate)
        {
            if (locate == null) throw new System.ArgumentNullException(nameof(locate));

            var cur = locate.CurrentSequence();
            LocateId = locate.LocateId;
            Description = locate.Description;
            Status = cur.Status.ToString();
            Owner = locate.Bid?.Firm?.FirmName;

            BidId = locate.BidId;

            LocateRefId = cur.LocateRefId;
            StartDate = cur.StartDate;
            ExpDate = cur.EndDate;

            //RequestBy = locate.RequestedBy;
            //RequestOn = locate.RequestedOn;
            //CreatedBy = locate.CreatedUser?.FullName();

            GPSCoords = locate.GPS;

            CrossStreet = locate.CrossStreet;
            City = locate.City;
            County = locate.County;
            Location = locate.Location;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Location { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "St. Cross")]
        public string CrossStreet { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "County")]
        public string County { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Ref Id")]
        public string LocateRefId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Bid")]
        public int? BidId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        public string Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Expire Date")]
        public DateTime? ExpDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "GPS Coords")]
        public string GPSCoords { get; set; }
    }
}
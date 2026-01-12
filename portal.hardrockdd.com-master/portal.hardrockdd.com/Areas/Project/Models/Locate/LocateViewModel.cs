using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class LocateListViewModel
    {
        public LocateListViewModel()
        {
            List = new List<LocateViewModel>
            {
                new LocateViewModel()
            };
        }

        public LocateListViewModel(List<PMLocate> list)
        {
            if (list == null)
                return;

            List = list.Select(s => new LocateViewModel(s)).ToList();
        }
        public LocateListViewModel(List<vPMLocate> list)
        {
            if (list == null)
                return;

            List = list.Select(s => new LocateViewModel(s)).ToList();
        }

        public LocateListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                return;
            BDCo = bid.BDCo;
            BidId = bid.BidId;

            var request = bid.PMLocateRequests.FirstOrDefault();
            
            if (request != null)
            {
                RequestId = request.RequestId;
                List = request.Locates.Select(s => new LocateViewModel(s)).ToList();
                return;
            }
            else if( bid.PMLocates.Any())
            {
                RequestId = bid.PMLocates.FirstOrDefault().RequestId;
                List = bid.PMLocates.Select(s => new LocateViewModel(s)).ToList();
            }
            else
            {
                List = new List<LocateViewModel>();
            }


           
        }

        public LocateListViewModel(PMLocateRequest request)
        {
            if (request == null)
                return;

            RequestId= request.RequestId;
            List = request.Locates.Select(s => new LocateViewModel(s)).ToList();
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }



        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Request Id")]
        public int BDCo { get; set; }


        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Request Id")]
        public int BidId { get; set; }



        public List<LocateViewModel>? List { get; }

    }

    public class LocateViewModel
    {
        public LocateViewModel()
        {

        }

        public LocateViewModel(DB.Infrastructure.ViewPointDB.Data.vPMLocate locate)
        {
            if (locate == null) throw new System.ArgumentNullException(nameof(locate));

            LocateId = locate.LocateId;
            Description = locate.Description;
            StatusId = (DB.PMLocateStatusEnum)(locate.StatusId ?? 0);
            Status = StatusId.ToString();
            Owner = locate.Owner;

            BidId = locate.BidId;

            OriginalStartDate = locate.OriginalStartDate;
            LocateRefId = locate.LocateRefId;
            LocateRefIds = locate.LocateRefIds;
            StartDate = locate.StartDate;
            ExpDate = locate.EndDate;

            RequestBy = locate.RequestBy;
            RequestOn = locate.RequestOn;
            CreatedBy = locate.CreatedByName;

            GPSCoords = locate.GPSCoords;
            MapSetId = locate.MapSetId ?? 0;

            CrossStreet = locate.CrossStreet;
            City = locate.City;
            County = locate.County;
            Location = locate.Location;

            UniqueAttchId = locate.UniqueAttchId;
            AttachmentId = locate.AttachmentId;
        }


        public LocateViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocate locate)
        {
            if (locate == null) throw new System.ArgumentNullException(nameof(locate));

            var cur = locate.CurrentSequence();
            LocateId = locate.LocateId;
            Description = locate.Description;
            StatusId = cur.Status;
            Status = cur.Status.ToString();
            Owner = locate.Bid?.Firm?.FirmName;

            BidId = locate.BidId;

            OriginalStartDate = locate.Sequences.FirstOrDefault()?.StartDate;
            LocateRefId = cur.LocateRefId;
            LocateRefIds = locate.LocateRefIds;
            StartDate = cur.StartDate;
            ExpDate = cur.EndDate;

            RequestBy = locate.RequestedBy;
            RequestOn = locate.RequestedOn;
            CreatedBy = locate.CreatedUser?.FullName();

            GPSCoords = locate.GPS;
            MapSetId = locate.MapSet.MapSetId;

            CrossStreet = locate.CrossStreet;
            City = locate.City;
            County = locate.County;
            Location = locate.Location;

            UniqueAttchId = cur.HQAttachment?.UniqueAttchID;
            AttachmentId = cur.HQAttachment?.Files?.FirstOrDefault()?.AttachmentId;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Ref Id")]
        public string LocateRefId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Ref Ids")]
        public string LocateRefIds { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/BDCombo/BidCombo", ComboForeignKeys = "", InfoUrl = "/BidForm/BidPopupForm", InfoForeignKeys = "BDCo='1',BidId", SearchUrl = "/Project/Bid/BidSearch", SearchForeignKeys = "")]
        [Display(Name = "Project")]
        public int? BidId { get; set; }


        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.PMLocateStatusEnum? StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        public string Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Division")]
        public string Division { get; set; }

        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo=JCCo")]
        public int? DivisionId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Org. Date")]
        public DateTime? OriginalStartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Expire Date")]
        public DateTime? ExpDate { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo", ComboForeignKeys = "")]
        [Display(Name = "Request By")]
        public string RequestBy { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime? RequestOn { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "GPS Coords")]
        public string GPSCoords { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "GPS Longitude")]
        public string GPSLong { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "GPS Latitude")]
        public string GPSLat { get; set; }

        public int MapSetId { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "Location")]
        [Field(LabelSize = 2, TextSize = 10)]
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

        public Guid? UniqueAttchId { get; set; }

        public int? AttachmentId { get; set; }

        internal LocateViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.PMLocates.FirstOrDefault(f => f.LocateId == this.LocateId);

            if (updObj != null)
            {
                var currentSeq = updObj.CurrentSequence();
                if (currentSeq.LocateRefId != this.LocateRefId &&
                    (currentSeq.Status == DB.PMLocateStatusEnum.Expired ||
                     currentSeq.Status == DB.PMLocateStatusEnum.Active ||
                     currentSeq.Status == DB.PMLocateStatusEnum.Closed))
                {
                    currentSeq.Status = DB.PMLocateStatusEnum.Closed;
                    currentSeq = updObj.AddSequence();
                    currentSeq.LocateRefId = this.LocateRefId;
                }
                else if ((currentSeq.Status == DB.PMLocateStatusEnum.Expired ||
                         currentSeq.Status == DB.PMLocateStatusEnum.Active ||
                         currentSeq.Status == DB.PMLocateStatusEnum.Closed) &&
                         currentSeq.Status != this.StatusId)
                {
                    currentSeq.Status = this.StatusId ?? DB.PMLocateStatusEnum.New;
                }
                else if (currentSeq.LocateRefId == this.LocateRefId &&
                         (currentSeq.Status == DB.PMLocateStatusEnum.New ||
                         currentSeq.Status == DB.PMLocateStatusEnum.Active))
                {
                    //currentSeq.Status = this.StatusId ?? currentSeq.Status;
                    currentSeq.LocateRefId = this.LocateRefId;
                    currentSeq.StartDate = this.StartDate;
                    currentSeq.EndDate = this.ExpDate;
                }
                
                updObj.Description = this.Description;
                updObj.GPS = this.GPSCoords;
                updObj.BidId = this.BidId;
                updObj.CrossStreet = CrossStreet;
                updObj.City = City;
                updObj.County = County;
                
                try
                {
                    db.BulkSaveChanges();
                    return new LocateViewModel(updObj);
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
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Project.Models.Locate
{
    public class CreateRequestViewModel
    {
        public CreateRequestViewModel()
        {

        }

        public CreateRequestViewModel(VPContext db)
        {
            if (db == null)
                return;

            RequestedOn = DateTime.Now;
            RequestedBy = db.CurrentUserId;
            LocateType = DB.PMLocateTypeEnum.State;

            CreatedBy = db.CurrentUserId;

        }

        public CreateRequestViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                return;

            OldBidId = bid.BidId;
            BidId = bid.BidId;
            StateId = bid.StateCodeId;
            LocateType = DB.PMLocateTypeEnum.State;
            Description = bid.Description;
            PiggyBackLocateId = "";
            GPS = bid.GPS;

            RequestedBy = bid.db.CurrentUserId;
            RequestedOn = DateTime.Now;
            RequestedName = bid.db.GetCurrentUser().FullName();

            CreatedBy = RequestedBy;
            CreatedOn = RequestedOn;
            CreatedName = RequestedName;

            AnticipatedStartDate = bid.StartDate;
            StateId = bid.StateCodeId;
            Owner = bid.Firm?.FirmName;
        }

        public CreateRequestViewModel(PMLocateRequest request)
        {
            if (request == null)
                return;

            OldBidId = request.tBidId;

            OldBidId = request.BidId;
            BidId = request.BidId;
            StateId = request.StateId;
            LocateType = request.LocateType;
            Description = request.Description;
            PiggyBackLocateId = request.PiggyBackLocateRefId;
            GPS = request.GPS;

            RequestedBy = request.RequestedBy;
            RequestedOn = request.RequestedOn;
            //RequestedName = request.RequestedName;

            Comments = request.Comments;
            CreatedBy = RequestedBy;
            CreatedOn = RequestedOn;
            CreatedName = RequestedName;

            AnticipatedStartDate = request.AnticipatedStartDate;
        }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Piggyback Locate")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string PiggyBackLocateId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/BDCombo/BidCombo", ComboForeignKeys = "")]
        [Display(Name = "Project")]
        public int? BidId { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Project Start Date")]
        public DateTime? AnticipatedStartDate { get; set; }
        public int? OldBidId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.PMLocateTypeEnum LocateType { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo", ComboForeignKeys = "")]
        [Display(Name = "Request By")]
        public string RequestedBy { get; set; }


        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.PMRequestStatusEnum Status { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Comments { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Owner")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string Owner { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo='1'")]
        public string StateId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "GPS")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string GPS { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Req. Date")]
        public System.DateTime RequestedOn { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created By")]
        public string RequestedName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Created Date")]
        public System.DateTime CreatedOn { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created By")]
        public string CreatedName { get; set; }
        public PMLocateRequest ToDBObject(VPContext db)
        {
            if (db == null)
                return null;

            var obj = new PMLocateRequest()
            {
                db = db,
                RequestId = this.RequestId,
                Description = this.Description,

                AnticipatedStartDate = this.AnticipatedStartDate,

                BidId = this.BidId,
                StateId = this.StateId,
                Comments = this.Comments,
                LocateType = this.LocateType,
                Status = this.Status,

                RequestedBy = this.RequestedBy,
                RequestedOn = this.RequestedOn,
                RequestedUser = db.WebUsers.FirstOrDefault(f => f.Id == RequestedBy),

                CreatedBy = this.CreatedBy,
                CreatedOn = this.CreatedOn,
                CreatedUser = db.WebUsers.FirstOrDefault(f => f.Id == CreatedBy),
            };
            return obj;
        }

        public CreateRequestViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (db == null || modelState == null)
                return this;

            var updObj = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.BidId = this.BidId;
                updObj.Comments = this.Comments;
                updObj.StateId = this.StateId;
                updObj.LocateType = this.LocateType;
                updObj.RequestedBy = this.RequestedBy;
                updObj.RequestedOn = this.RequestedOn;
                updObj.GPS = this.GPS;
                updObj.PiggyBackLocateRefId = this.PiggyBackLocateId;
                try
                {
                    db.BulkSaveChanges();
                    return new CreateRequestViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {                
                updObj = this.ToDBObject(db);
                updObj.tBidId = OldBidId;

                updObj.Comments = this.Comments;
                updObj.StateId = this.StateId;
                updObj.LocateType = this.LocateType;
                updObj.RequestedBy = this.RequestedBy;
                updObj.RequestedOn = this.RequestedOn;
                updObj.GPS = this.GPS;
                updObj.PiggyBackLocateRefId = this.PiggyBackLocateId;
                if (BidId != OldBidId)
                {
                    updObj.BidId = BidId;
                }
                return new CreateRequestViewModel(updObj);
            }
        }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class RequestListViewModel
    {
        public RequestListViewModel()
        {
            List = new List<RequestViewModel>();
        }

        public RequestListViewModel(VPContext db)
        {
            if (db == null)
            {
                List = new List<RequestViewModel>();
                return;
            }

            List = db.PMLocateRequests.Select(s => new RequestViewModel(s)).ToList();
        }
        public RequestListViewModel(List<PMLocateRequest> list)
        {
            if (list == null)
            {
                List = new List<RequestViewModel>();
                return;
            }

            List = list.Select(s => new RequestViewModel(s)).ToList();
        }

        public List<RequestViewModel> List { get; }
    }
    
    public class RequestViewModel
    {
        public RequestViewModel()
        {

        }

        public RequestViewModel(PMLocateRequest request)
        {
            if (request == null)
                return;

            RequestId = request.RequestId;
            Description = request.Description;

            RequestedBy = request.RequestedBy;
            RequestedOn = request.RequestedOn;
            RequestedName = request.RequestedUser?.FullName();

            CreatedBy = request.CreatedBy;
            CreatedOn = request.CreatedOn;
            CreatedName = request.CreatedUser?.FullName();

            AnticipatedStartDate = request.AnticipatedStartDate;

            BidId = request.BidId;
            StateId = request.Bid?.StateCodeId;
            Comments = request.Comments;
            LocateType = request.LocateType;

            Owner = request.Bid?.Firm?.FirmName;

            PiggyBackLocateId = request.PiggyBackLocateRefId;
            GPS = request.GPS;

            Actions = new RequestActionViewModel(request);
        }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.PMLocateTypeEnum LocateType { get; set; }


        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.PMRequestStatusEnum Status { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Comments { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/BDCombo/BidCombo", ComboForeignKeys = "", InfoUrl = "/BidForm/BidPopupForm", InfoForeignKeys = "BDCo='1',BidId", SearchUrl = "/Project/Bid/BidSearch", SearchForeignKeys = "")]
        [Display(Name = "Project")]
        public int? BidId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Owner")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string Owner { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo='1'")]
        public string StateId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Project Start Date")]
        public DateTime? AnticipatedStartDate { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Req. Date")]
        public System.DateTime RequestedOn { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo", ComboForeignKeys = "")]
        [Display(Name = "Request By")]
        public string RequestedBy { get; set; }

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


        [UIHint("TextBox")]
        [Display(Name = "GPS")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string GPS { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Piggyback Locate")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string PiggyBackLocateId { get; set; }

        public RequestActionViewModel Actions { get; set; }

        internal RequestViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == this.RequestId);

            if (updObj != null)
            {
                if (updObj.BidId != this.BidId)
                {
                    updObj.BidId = this.BidId;
                }
                else
                {
                    updObj.Description = this.Description;
                    updObj.Comments = this.Comments;
                    updObj.StateId = this.StateId;
                    updObj.LocateType = this.LocateType;
                    updObj.RequestedBy = this.RequestedBy;
                    updObj.RequestedOn = this.RequestedOn;

                    updObj.GPS = this.GPS;
                    updObj.PiggyBackLocateRefId = this.PiggyBackLocateId;
                }

                try
                {
                    db.BulkSaveChanges();
                    return new RequestViewModel(updObj);
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
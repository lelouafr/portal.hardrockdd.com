using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.PositionRequest
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

            var emp = db.GetCurrentEmployee();

            HRCo = (byte)db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCo;
            PRCo = emp.PRCo;
            RequestedDate = DateTime.Now;
            RequestedBy = db.CurrentUserId;
            if (emp.Crew != null)
                ForCrewId = emp.Crew.CrewId;

        }

        public CreateRequestViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            PositionCodeId = request.PositionCodeId;
            PRCo = request.PRCo;

            ForCrewId = request.ForCrewId;
            PriorEmployeeId = request.PriorEmployeeId;

            Comments = request.Comments;
            RequestedDate = request.RequestedDate;
            RequestedBy = request.RequestedBy;



        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Position")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo", LabelSize = 3, TextSize = 9)]
        public string PositionCodeId { get; set; }


        [UIHint("LongBox")]
        public byte? PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "For Crew")]
        [Field(ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo", LabelSize = 3, TextSize = 9)]
        public string ForCrewId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Replacing Employee")]
        [Field(ComboUrl = "/HRCombo/EmployeeCombo", LabelSize = 3, TextSize = 9)]
        public int? PriorEmployeeId { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 3, TextSize = 9)]
        public string Comments { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Requested Date")]
        public System.DateTime RequestedDate { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Requestor")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo")]
        public string RequestedBy { get; set; }

        public HRPositionRequest ToDBObject()
        {
            var obj = new HRPositionRequest()
            {                
                HRCo = this.HRCo,
                RequestId = this.RequestId,
                PositionCodeId = this.PositionCodeId,
                PRCo = this.PRCo,

                tForCrewId = this.ForCrewId,
                tPriorEmployeeId = this.PriorEmployeeId,

                Comments = this.Comments,
                RequestedDate = this.RequestedDate,
                RequestedBy = this.RequestedBy,
            };

            return obj;
        }

        public CreateRequestViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (db == null || modelState == null)
                return this;

            var updObj = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.PositionCodeId = this.PositionCodeId;
                updObj.ForCrewId = this.ForCrewId;
                updObj.PriorEmployeeId = this.PriorEmployeeId;
                updObj.Comments = this.Comments;

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
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
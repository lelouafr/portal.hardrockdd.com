using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.TermRequest
{
    public class CreateTermRequestViewModel
    {
        public CreateTermRequestViewModel()
        {

        }
        public CreateTermRequestViewModel(VPContext db)
        {
            if (db == null)
                return;

            HRCo = (byte)db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCo;

            TermDate = DateTime.Now.Date;
            RequestedDate = DateTime.Now;
            RequestedBy = db.CurrentUserId;
        }

        public CreateTermRequestViewModel(HRTermRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            HRRef = request.HRRef;
            TermDate = request.TermDate;
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
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/HRCombo/TermEmployeeCombo", LabelSize = 3, TextSize = 9)]
        public int? HRRef { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Term Date")]
        [Field(LabelSize = 3, TextSize = 9)]
        public System.DateTime? TermDate { get; set; }

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

        public HRTermRequest ToDBObject()
        {
            var obj = new HRTermRequest()
            {
                
                HRCo = this.HRCo,
                RequestId = this.RequestId,
                HRRef = this.HRRef,
                TermDate = this.TermDate,
                Comments = this.Comments,
                RequestedDate = this.RequestedDate,
                RequestedBy = this.RequestedBy,
            };

            return obj;
        }
        public CreateTermRequestViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (db == null || modelState == null)
                return this;

            var updObj = db.HRTermRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.HRRef = this.HRRef;
                updObj.Comments = this.Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new CreateTermRequestViewModel(updObj);
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
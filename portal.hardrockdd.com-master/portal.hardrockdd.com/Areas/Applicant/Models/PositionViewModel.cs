using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Applicant.Models
{
    public class PositionListViewModel
    {
        public PositionListViewModel()
        {

        }

        public PositionListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.AppliedPositions.Select(s => new PositionViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        public List<PositionViewModel> List { get; }
    }

    public class PositionViewModel
    {
        public PositionViewModel()
        {

        }

        public PositionViewModel(DB.Infrastructure.ViewPointDB.Data.WAAppliedPosition position)
        {
            if (position == null)
                return;

            ApplicantId = position.ApplicantId;
            ApplicationId = position.ApplicationId;
            SeqId = position.SeqId;
            AppliedDate = position.AppliedDate ?? position.Application.ApplicationDate;
            HRCo = position.HRCo ?? 1;
            PositionCodeId = position.tPositionCodeId;
        }

        [Key]
        public int ApplicantId { get; set; }

        [Key]
        public int ApplicationId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Applied Date")]
        public DateTime AppliedDate { get; set; }

        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [UIHint("DropDownBox")]
        [Display(Name = "Position")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        public string PositionCodeId { get; set; }


        internal PositionViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.WAAppliedPositions.FirstOrDefault(f => f.ApplicantId == this.ApplicantId &&
                                                                     f.ApplicationId == this.ApplicationId &&
                                                                     f.SeqId == this.SeqId);


            if (updObj != null)
            {
                //this.Validate(modelState);

                updObj.PositionCodeId = this.PositionCodeId;
                try
                {
                    db.BulkSaveChanges();
                    return new PositionViewModel(updObj);
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
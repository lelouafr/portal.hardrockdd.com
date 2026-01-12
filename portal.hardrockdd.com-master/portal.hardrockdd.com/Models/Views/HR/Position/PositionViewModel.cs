using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Position
{
    public class PositionListViewModel
    {
        public PositionListViewModel()
        {

        }

        public PositionListViewModel(Code.Data.VP.HRCompanyParm companyParm)
        {
            if (companyParm == null)
                return;

            HRCo = companyParm.HRCo;
            List = companyParm.HRPositions.Select(s => new PositionViewModel(s)).ToList();
        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        public List<PositionViewModel> List { get; }
    }

    public class PositionViewModel
    {
        public PositionViewModel()
        {

        }

        public PositionViewModel(Code.Data.VP.HRPosition position)
        {
            if (position == null)
                return;

            HRCo = position.HRCo;
            PositionCodeId = position.PositionCodeId;
            JobTitle = position.JobTitle;
            Description = position.Description;
            ReportPosition = position.ReportPosition;
            OpenJobs = position.OpenJobs;
            POLimit = position.POLimit;
            AutoAssignOrg = position.AutoAssignOrg == "Y";

        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Code")]
        public string PositionCodeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("DropDownBox")]
        [Display(Name = "Report TO Pos.")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo", LabelSize = 3, TextSize = 9)]
        public string ReportPosition { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Open Positions")]
        public int? OpenJobs { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "PO Limit")]
        public decimal? POLimit { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Auto Assign Sup?")]
        public bool AutoAssignOrg { get; set; }

        internal PositionViewModel ProcessUpdate(Code.Data.VP.VPEntities db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HRPositions.FirstOrDefault(f => f.HRCo == this.HRCo && f.PositionCodeId == this.PositionCodeId);

            if (updObj != null)
            {
                updObj.JobTitle = this.JobTitle;
                updObj.Description = this.Description;
                updObj.ReportPosition = this.ReportPosition;
                updObj.OpenJobs = this.OpenJobs;
                updObj.POLimit = this.POLimit;
                updObj.AutoAssignOrg = this.AutoAssignOrg ? "Y" : "N";
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
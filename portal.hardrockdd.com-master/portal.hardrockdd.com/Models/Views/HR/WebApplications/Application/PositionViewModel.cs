using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.WA.Application
{
    public class PositionListViewModel
    {
        public PositionListViewModel()
        {

        }

        public PositionListViewModel(Code.Data.VP.WebApplication application)
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

        public PositionViewModel(Code.Data.VP.WAAppliedPosition position)
        {
            if (position == null)
                return;

            ApplicantId = position.ApplicantId;
            ApplicationId = position.ApplicationId;
            SeqId = position.SeqId;
            AppliedDate = position.AppliedDate ?? position.Application.ApplicationDate;
            HRCo = position.HRCo ?? 1;
            PositionCodeId = position.PositionCodeId;
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
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "PRCo=HRCo")]
        public string PositionCodeId { get; set; }
    }
}
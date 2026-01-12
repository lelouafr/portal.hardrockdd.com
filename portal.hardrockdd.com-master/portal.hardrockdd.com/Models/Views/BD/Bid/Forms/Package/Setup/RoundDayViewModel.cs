using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Setup
{
    public class RoundDayViewModel 
    {
        public RoundDayViewModel()
        {

        }

        public RoundDayViewModel(BidPackage package)
        {
            if (package == null)
                throw new System.ArgumentNullException(nameof(package));

            #region mapping
            BDCo = package.BDCo;
            if (package.Division == null)
            {
                package.UpdateDivision(package.DivisionId);
                package.db.BulkSaveChanges();
            }
            PhaseGroupId = package.Division.WPDivision.HQCompany.PhaseGroupId;

            BidId = package.BidId;
            PackageId = package.PackageId;
            DayRounding = DB.BDDayRoundEnum.None;
            if ((package.TotalDayRounding ?? 0m) == 0m)
            {
                DayRounding = DB.BDDayRoundEnum.None;
            }
            else if (package.TotalDayRounding == .25m)
            {
                DayRounding = DB.BDDayRoundEnum.QuarterDay;
            }
            else if (package.TotalDayRounding == .5m)
            {
                DayRounding = DB.BDDayRoundEnum.HalfDay;
            }
            else if (package.TotalDayRounding == 1m)
            {
                DayRounding = DB.BDDayRoundEnum.FullDay;
            }
            RoundingPhaseId = package.RoundingPhaseId;
            #endregion
        }

        public byte? PhaseGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int PackageId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Rounding")]
        public DB.BDDayRoundEnum DayRounding { get; set; }

        [Display(Name = "Phase")]
        [UIHint("DropdownBox")]
        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        public string RoundingPhaseId { get; set; }

        internal RoundDayViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                var bidParms = updObj.Bid.Company.BDCompanyParm;

                updObj.RoundingPhaseId = RoundingPhaseId ?? bidParms.PilotPhaseId;
                updObj.TotalDayRounding = DayRounding switch
                {
                    DB.BDDayRoundEnum.None => 0m,
                    DB.BDDayRoundEnum.QuarterDay => .25m,
                    DB.BDDayRoundEnum.HalfDay => .5m,
                    DB.BDDayRoundEnum.FullDay => 1m,
                    _ => 0m,
                };
                updObj.ActiveBoreLines.ForEach(e => e.RecalcNeeded = true);

                try
                {
                    db.BulkSaveChanges();
                    return new RoundDayViewModel(updObj);
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
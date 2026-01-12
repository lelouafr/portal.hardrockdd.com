//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackageDaySetupViewModel
//    {

//        public BidPackageDaySetupViewModel()
//        {

//        }

//        public BidPackageDaySetupViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
//        {
//            if (package == null) throw new System.ArgumentNullException(nameof(package));
            
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            DayRounding = DB.BDDayRoundEnum.None;
//            if ((package.TotalDayRounding ?? 0m) == 0m)
//            {
//                DayRounding = DB.BDDayRoundEnum.None;
//            }
//            else if (package.TotalDayRounding == .25m)
//            {
//                DayRounding = DB.BDDayRoundEnum.QuarterDay;
//            }
//            else if (package.TotalDayRounding == .5m)
//            {
//                DayRounding = DB.BDDayRoundEnum.HalfDay;
//            }
//            else if (package.TotalDayRounding == 1m)
//            {
//                DayRounding = DB.BDDayRoundEnum.FullDay;
//            }
//            RoundingPhaseId = package.RoundingPhaseId;

//        }
        

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Package Id")]
//        public int PackageId { get; set; }


//        [Required]
//        [UIHint("EnumBox")]
//        [Display(Name = "Rounding")]
//        public DB.BDDayRoundEnum DayRounding { get; set; }

//        [Display(Name = "Phase")]
//        [UIHint("DropdownBox")]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string RoundingPhaseId { get; set; }



//    }
//}
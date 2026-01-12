using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Schedule
{
    public class ScheduleListViewModel
    {
        public ScheduleListViewModel()
        {

        }

        public ScheduleListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion
            List = bid.ActivePackages.Select(s => new ScheduleViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        public List<ScheduleViewModel> List { get; }
    }

    public class ScheduleViewModel : PackageViewModel
    {
        public ScheduleViewModel()
        {

        }

        public ScheduleViewModel(BidPackage package): base(package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping
            StartDate = package.StartDate;
            #endregion

        }
        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

            
            return ok;
        }



        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        internal ScheduleViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == this.BDCo && f.BidId == this.BidId && f.PackageId == this.PackageId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = Description;
                updObj.NumberOfBores = NumberOfBores;
                updObj.RigCategoryId = RigCategoryId;
                updObj.StartDate = StartDate;
                try
                {
                    db.SaveChanges(modelState);
                    return new ScheduleViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }


    }


}
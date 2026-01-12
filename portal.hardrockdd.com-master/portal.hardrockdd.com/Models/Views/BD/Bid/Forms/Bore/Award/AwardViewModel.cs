using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Bore.Award
{
    public class AwardListViewModel
    {
        public AwardListViewModel()
        {

        }

        public AwardListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion
            List = package.ActiveBoreLines.Select(s => new AwardViewModel(s)).ToList();
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

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        public List<AwardViewModel> List { get; }
    }

    public class AwardViewModel : BoreLineViewModel
    {
        public AwardViewModel()
        {

        }

        public AwardViewModel(BidBoreLine line) : base(line)
        {
            if (line == null)
                throw new System.ArgumentNullException(nameof(line));
            #region mapping

            AwardStatus = line.AwardStatus;
            #endregion

        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                throw new System.ArgumentNullException(nameof(modelState));

            var ok = true;

            if (AwardStatus == DB.BidAwardStatusEnum.Pending)
            {
                modelState.AddModelError("Status", "Status may not be pending");
                ok &= false;
            }
            return ok;
        }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidAwardStatusEnum AwardStatus { get; set; }

        internal AwardViewModel ProcessUpdate( VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = Description;
                updObj.Footage = Footage;
                updObj.AwardStatus = AwardStatus;

                if (updObj.RecalcNeeded == true)
                {
                    updObj.RecalculateCostUnits();
                    updObj.Package.ApplyPackageCost();
                }

                try
                {
                    db.BulkSaveChanges();
                    return new AwardViewModel(updObj);
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
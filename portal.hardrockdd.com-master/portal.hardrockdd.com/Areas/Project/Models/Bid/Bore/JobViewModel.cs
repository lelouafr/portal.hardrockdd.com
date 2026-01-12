using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Bore
{
    public class JobListViewModel
    {
        public JobListViewModel()
        {

        }

        public JobListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion
            List = package.ActiveBoreLines.Select(s => new JobViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "BDCo")]
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

        public List<JobViewModel> List { get; }
    }

    public class JobViewModel : BoreLineViewModel
    {
        public JobViewModel()
        {

        }

        public JobViewModel(BidBoreLine line) : base(line)
        {
            if (line == null)
                return;
            #region mapping
            JCCo = line.JCCo ?? 1;
            JobId = line.JobId ?? "00-0000-00";
            ParentJobId = line.Package.JobId ?? "00-0000-00";
            AwardStatus = line.AwardStatus;

            #endregion

        }

        public new bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                return false;

            var ok = base.Validate(modelState);

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

        public byte? JCCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/SubJobCombo", ComboForeignKeys = "JCCo,ParentJobId")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [Required]
        [Display(Name = "ParentJobId")]
        [UIHint("TextBox")]
        public string ParentJobId { get; set; }

        internal new JobViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            //var recalculateUnits = false;
            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = Description;
                updObj.JobId = JobId;
                updObj.AwardStatus = AwardStatus;

                if (updObj.RecalcNeeded == true)
                {
                    updObj.RecalculateCostUnits();
                    updObj.Package.ApplyPackageCost();
                }
                try
                {
                    db.BulkSaveChanges();
                    return new JobViewModel(updObj);
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
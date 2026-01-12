using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class JobListViewModel
    {
        public JobListViewModel()
        {

        }

        public JobListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                return;

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion
            List = bid.ActivePackages.Select(s => new JobViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        public List<JobViewModel> List { get; }
    }

    public class JobViewModel: PriceViewModel 
    {
        public JobViewModel()
        {

        }

        public JobViewModel(BidPackage package): base(package)
        {
            if (package == null)
                return;

            #region mapping

            CustomerReference = package.CustomerReference;
            if (package.Division == null)
            {
                package.UpdateDivision(package.DivisionId);
                package.db.BulkSaveChanges();
            }
            JCCo = package.JCCo ?? package.Division.WPDivision.HQCompany.JCCo;
            JobId = package.JobId;

            TempCustomerId = package.CustomerId;
            CustomerId = package.CustomerId > 90000 ? null : package.CustomerId;
            AwardStatus = package.AwardStatus;
            #endregion

        }
        public new bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                return false;

            var ok = base.Validate(modelState);

            if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerId == null)
            {
                modelState.AddModelError("CustomerId", "Customer Field is Required");
                ok &= false;
            }
            if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerReference == null)
            {
                modelState.AddModelError("CustomerReference", "Customer Reference Field is Required");
                ok &= false;
            }
            if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerId >= 90000)
            {
                modelState.AddModelError("CustomerId", "This is a new customer, please setup then reselect the customer");
                ok &= false;
            }
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

        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/BidCombo", ComboForeignKeys = "BDCo,BidId")]
        public int? TempCustomerId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/BidCombo", ComboForeignKeys = "BDCo,BidId")]
        public int? CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer PO#")]
        [UIHint("TextBox")]
        public string CustomerReference { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/BidProjectCombo", ComboForeignKeys = "JCCo,BidId")]
        [Display(Name = "Project")]
        public string JobId { get; set; }


        internal new JobViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                updObj.DivisionId = DivisionId;
                updObj.CustomerReference = CustomerReference;
                updObj.AwardStatus = AwardStatus;
                updObj.JobId = JobId;
                updObj.Description = Description;
                updObj.CustomerId = CustomerId;

                if (updObj.Bid.BidType == (int)DB.BidTypeEnum.QuickBid)
                {
                    updObj.DirtPrice = DirtLFPrice;
                    updObj.RockPrice = RockLFPrice;
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
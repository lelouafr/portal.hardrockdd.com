using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class AwardListViewModel
    {
        public AwardListViewModel()
        {

        }

        public AwardListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                return;

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion
            List = bid.ActivePackages.Select(s => new AwardViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        public List<AwardViewModel> List { get; }
    }

    public class AwardViewModel : PriceViewModel 
    {
        public AwardViewModel()
        {

        }

        public AwardViewModel(BidPackage package): base(package)
        {
            if (package == null)
                return;

            #region mapping
            CustomerReference = package.CustomerReference;
            CustomerId = package.CustomerId;
            AwardStatus = package.AwardStatus;
            #endregion

        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

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
            if (AwardStatus == DB.BidAwardStatusEnum.Pending)
            {
                modelState.AddModelError("AwardStatus", "Status may not be pending");
                ok &= false;
            }
            return ok;
        }


        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidAwardStatusEnum AwardStatus { get; set; }

        //[Required]
        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/BidCombo", ComboForeignKeys = "BDCo,BidId")]
        public int? CustomerId { get; set; }

        //[Required]
        [Display(Name = "Customer PO#")]
        [UIHint("TextBox")]
        public string CustomerReference { get; set; }


        internal new AwardViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                updObj.DivisionId = DivisionId;
                updObj.CustomerReference = CustomerReference;
                updObj.AwardStatus = AwardStatus;
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
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package
{
    public class PackageListViewModel
    {
        public PackageListViewModel()
        {

        }

        public PackageListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid, bool includeDetails = false)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion
            List = bid.ActivePackages.Select(s => new PackageViewModel(s, includeDetails)).ToList();
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

        public List<PackageViewModel> List { get; }
    }

    public class PackageViewModel
    {
        public PackageViewModel()
        {

        }

        public PackageViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package, bool includeDetails = false)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            Status = package.Status;
            Description = package.Description;
            BoreTypeId = package.BoreTypeId; 
            NumberOfBores = package.NumberOfBores;
            PipeSize = package.PipeSize;
            RigCategoryId = package.RigCategoryId;
            GroundDensityId = package.GroundDensityId;
            MarketId = package.MarketId;
            IndustryId = package.IndustryId;
            DivisionId = package.DivisionId;

            EMCo = package.Bid.Company.EMCo;
            #endregion

            BidTypeId = (DB.BidTypeEnum)(package.Bid.BidType ?? 0);


            if (includeDetails)
            {
                CostList = new Summary.PackageCostListViewModel(package);
                Price = new Price.PriceViewModel(package);
                Bores = new Bore.Job.JobListViewModel(package);
            }

        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

            if (Status != DB.BidStatusEnum.Draft && RigCategoryId == null)
            {
                modelState.AddModelError("RigCategoryId", "Rig Category Field is Required");
                ok &= false;
            }
            return ok;
        }
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
        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Bore Type")]
        [Field(ComboUrl = "/BDCombo/BoreTypeCombo", ComboForeignKeys = "BDCo")]
        [UIHint("DropdownBox")]
        public int? BoreTypeId { get; set; }

        [Required]
        [Display(Name = "# Of Bores")]
        [UIHint("LongBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public int? NumberOfBores { get; set; }

        [Required]
        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? PipeSize { get; set; }

        [Required]
        [Display(Name = "Rock Density")]
        [Field(ComboUrl = "/GroundDensity/ComboNoDirt", ComboForeignKeys = "BDCo")]
        [UIHint("DropdownBox")]
        public int? GroundDensityId { get; set; }

        public byte? EMCo { get; set; }

        //[Required]
        [Display(Name = "Rig Type")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/RigCatCombo", ComboForeignKeys = "EMCo=BDCo")]
        public string RigCategoryId { get; set; }


        [Required]
        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo=BDCo")]
        public int? DivisionId { get; set; }


        [Required]
        [Display(Name = "Industry")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/IndustryCombo", ComboForeignKeys = "JCCo=BDCo")]
        public int? IndustryId { get; set; }

        [Required]
        [Display(Name = "Market")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/IndustryMarketCombo", ComboForeignKeys = "JCCo=BDCo, IndustryId")]
        public int? MarketId { get; set; }

        public DB.BidTypeEnum BidTypeId { get; set; }

        public Summary.PackageCostListViewModel CostList { get; }

        public Price.PriceViewModel Price { get;  }

        public Bore.Job.JobListViewModel Bores { get; }

        internal PackageViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                updObj.Description = Description;
                updObj.BoreTypeId = BoreTypeId;
                updObj.NumberOfBores = NumberOfBores;
                updObj.PipeSize = PipeSize;
                updObj.RigCategoryId = RigCategoryId;
                updObj.GroundDensityId = GroundDensityId;
                updObj.MarketId = MarketId;
                updObj.DivisionId = DivisionId;

                try
                {
                    db.SaveChanges(modelState);
                    return new PackageViewModel(updObj);
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
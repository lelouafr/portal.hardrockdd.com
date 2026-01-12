using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Bore
{
    public class BoreLineListViewModel
    {
        public BoreLineListViewModel()
        {

        }

        public BoreLineListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion

            var bores = package.ActiveBoreLines;
            List = bores.Where(f => f.IntersectBoreId == null).Select(s => new BoreLineViewModel(s)).ToList();

            foreach (var bore in bores)
            {
                if (bore.IntersectBoreId != null)
                {
                    var boreline = List.FirstOrDefault(f => f.BoreId == bore.IntersectBoreId);
                    if (boreline != null)
                    {
                        boreline.IntersectBore = new BoreLineViewModel(bore);
                    }
                }
            }

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

        public List<BoreLineViewModel> List { get; }
    }

    public class BoreLineViewModel
    {
        public BoreLineViewModel()
        {

        }

        public BoreLineViewModel(BidBoreLine line)
        {
            if (line == null)
                return;

            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            PackageId = (int)line.PackageId;
            Status = line.Package.Status;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            BoreTypeId = line.BoreTypeId;
            RigCategoryId = line.RigCategoryId;
            CrewCount = line.CrewCount;
            EMCo = line.Bid.Company.EMCo;
            #endregion
        }

        public BoreLineViewModel IntersectBore { get; set; }

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
            if (Status != DB.BidStatusEnum.Draft && (CrewCount ?? 0) <=0)
            {
                modelState.AddModelError("CrewCount", "Crew Count Field is Required");
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
        public int BoreId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Package Id")]
        [UIHint("LongBox")]
        public int PackageId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidStatusEnum Status { get; set; }


        [Required]
        [Display(Name = "Description")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Length")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal? Footage { get; set; }

        [Required]
        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? PipeSize { get; set; }

        [Required]
        [Display(Name = "Bore Type")]
        [Field(ComboUrl = "/BDCombo/BoreTypeLineCombo", ComboForeignKeys = "BDCo")]
        [UIHint("DropdownBox")]
        public int? BoreTypeId { get; set; }

        public byte? EMCo { get; set; }

        //[Required]
        [Display(Name = "Rig Type")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/RigCatCombo", ComboForeignKeys = "EMCo=BDCo")]
        public string RigCategoryId { get; set; }

        //[Required]
        [Display(Name = "Crew Size")]
        [UIHint("LongBox")]
        public int? CrewCount { get; set; }
        
        internal BoreLineViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                if (updObj.IntersectBoreId == null)
                {
                    updObj.BoreTypeId = BoreTypeId;
                    updObj.Footage = Footage;
                    updObj.PipeSize = PipeSize;
                }
                updObj.Description = Description;
                updObj.CrewCount = CrewCount;
                updObj.RigCategoryId = RigCategoryId;

                if (updObj.RecalcNeeded == true)
                {
                    updObj.RecalculateCostUnits();
                    updObj.Package.ApplyPackageCost();
                }
                try
                {
                    db.BulkSaveChanges();
                    return new BoreLineViewModel(updObj);
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
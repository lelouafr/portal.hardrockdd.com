using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Bore
{
    public class ScheduleListViewModel
    {
        public ScheduleListViewModel()
        {

        }

        public ScheduleListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion
            List = package.ActiveBoreLines.Select(s => new ScheduleViewModel(s)).ToList();
        }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        public List<ScheduleViewModel> List { get; }
    }

    public class ScheduleViewModel : BoreLineViewModel
    {
        public ScheduleViewModel()
        {

        }

        public ScheduleViewModel(BidBoreLine line) : base(line)
        {
            if (line == null)
                return;

            #region mapping
            RigId = line.RigId;
            StartDate = line.StartDate;
            #endregion

        }

        public new bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                return false;


            var ok = base.Validate(modelState);

            return ok;
        }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Ticket", FormGroupRow = 4, ComboUrl = "/EMCombo/CategoryEquipmentCombo", ComboForeignKeys = "categoryId=RigCategoryId")]
        [Display(Name = "Rig")]
        public string RigId { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        internal new ScheduleViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == this.BDCo && f.BidId == this.BidId && f.BoreId == this.BoreId);

            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.RigCategoryId = RigCategoryId;
                updObj.Description = Description;
                updObj.RigId = RigId;
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
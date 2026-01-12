using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Bore.Setup
{
    public class SetupFormView
    {
        public SetupFormView()
        {

        }
        public SetupFormView(BidBoreLine line)
        {
            if (line == null)
                return;

            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            PackageId = line.PackageId ?? 0;
            BoreId = line.BoreId;
            #endregion

            ProductionRates = new ProductionRateListViewModel(line);
            CostItems = new CostItemListViewModel(line);
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

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bore Id")]
        public int BoreId { get; set; }



        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));

            ProductionRates.List.ForEach(e => e.Validate(modelState));

            return modelState.IsValid;
        }

        public DetailViewModel BoreInfo { get; set; }

        public ProductionRateListViewModel ProductionRates { get; set; }

        public CostItemListViewModel CostItems { get; set; }
        //public CostItemListViewModel RentalItems { get; set; }
    }

}
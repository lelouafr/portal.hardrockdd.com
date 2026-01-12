using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Web;

namespace portal.Models.API.PM.BidSchedule
{
	public class BidScheduleApiModel
	{
        public BidScheduleApiModel()
        {
            
        }

        public BidScheduleApiModel(Bid bid)
        {
            if (bid == null)
                return;

            BDCo = bid.BDCo;
            BidId = bid.BidId;
            StartDate = bid.StartDate ?? DateTime.Now.Date;

            Packages = bid.ActivePackages.Select(s => new BidPackageApiModel(s)).ToList();
        }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public DateTime StartDate { get; set; }

		public List<BidPackageApiModel> Packages { get; }
	}

    public class BidPackageApiModel
    {
        public BidPackageApiModel()
        {
            
        }

        public BidPackageApiModel(BidPackage bidPackage)
        {
            if (bidPackage == null)
                return;

			BDCo = bidPackage.BDCo;
			BidId = bidPackage.BidId;
			PackageId = bidPackage.PackageId;
			Description = bidPackage.Description;

			Bores = bidPackage.ActiveBoreLines.Select(s => new BidBoreApiModel(s)).ToList();
		}

		public byte BDCo { get; set; }

		public int BidId { get; set; }

        public int PackageId { get; set; }

        public string Description { get; set; }

		public List<BidBoreApiModel> Bores { get; }

	}

    public class BidBoreApiModel
    {
        public BidBoreApiModel()
        {
            
        }

        public BidBoreApiModel(BidBoreLine boreLine)
        {

			if (boreLine == null)
				return;
            var rig = boreLine.db.Equipments.FirstOrDefault(f => f.EquipmentId == boreLine.RigId);
			BDCo = boreLine.BDCo;
			BidId = boreLine.BidId;
			PackageId = boreLine.PackageId ?? 0;
			BoreId = boreLine.BoreId;
			Footage = (int)(boreLine.Footage ?? 0);
            RigName = rig?.DisplayName;
			Description = boreLine.Description;

            RockPhases = boreLine.RockPhases.Select(s => new BidBorePhaseApiModel(s)).ToList();
			DirtPhases = boreLine.DirtPhases.Select(s => new BidBorePhaseApiModel(s)).ToList();
		}


		public byte BDCo { get; set; }

		public int BidId { get; set; }

		public int PackageId { get; set; }

		public int BoreId { get; set; }

        public int Footage { get; set; }

        public string RigName { get; set; }

        public string Description { get; set; }

		public List<BidBorePhaseApiModel> RockPhases { get; }

		public List<BidBorePhaseApiModel> DirtPhases { get; }
	}

    public class BidBorePhaseApiModel
    {
        public BidBorePhaseApiModel()
        {
            
        }

        public BidBorePhaseApiModel(BidBoreLinePass phase)
        {
			if (phase == null)
				return;
            var footage = phase.BoreLine.Footage ?? 1;

			BDCo = phase.BDCo;
			BidId = phase.BidId;
			BoreId = phase.BoreId;
            Description = phase.Description;
            ProductionRate = phase.ProductionRate ?? 0;
			ProductionDays = phase.ProductionDays ?? 0;

            var productRateCals = ProductionRate;
            if (phase.ShiftCnt > 1)
				productRateCals += (productRateCals * phase.Shift2ProductionPerc) ?? 0;

			ProductionRate = productRateCals;

            Duration = footage / productRateCals;
		}

        public byte BDCo { get; set; }

		public int BidId { get; set; }

		public int PackageId { get; set; }

		public int BoreId { get; set; }

		public int GroundId { get; set; }

		public string Phase { get; set; }
		
        public string Description { get; set; }

		public int PassId { get; set; }

		public decimal ProductionRate { get; set; }

		public decimal ProductionDays { get; set; }

		public decimal Duration { get; set; }


	}
}
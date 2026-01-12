using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.EmailModels
{
    public class BidEmailViewModel
    {
        public BidEmailViewModel()
        {

        }

        public BidEmailViewModel(Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            BDCo = bid.BDCo;
            BidId = bid.BidId;

            BidInfo = new BidInfoViewModel(bid);
            Packages = new PackageListViewModel(bid, true);

            var workFlow = bid.WorkFlow.CurrentSequence();
            if (workFlow != null)
            {
                Comments = workFlow?.Comments;
            }
        }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public string Comments { get; set; }

        public BidInfoViewModel BidInfo { get; set; }

        public PackageListViewModel Packages { get; set; }

    }

    public class BidInfoViewModel
    {
        public BidInfoViewModel()
        {
        }

        public BidInfoViewModel(Bid bid)
        {
            if (bid == null)
                throw new System.ArgumentNullException(nameof(bid));

            #region mapping
            JCCo = bid.Company.JCCompanyParm.JCCo;
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            BidDate = bid.BidDate;
            DueDate = bid.DueDate;
            StartDate = bid.StartDate;
            Description = bid.tDescription;
            Comments = bid.Comments;
            CustomerId = bid.CustomerId;
            ContactId = bid.ContactId;
            FirmNumber = bid.tFirmId;
            DivisionId = bid.DivisionId;
            Location = bid.Location;
            StateCodeId = bid.StateCodeId;
            FirmType = "OWNER";
            FirmName = bid.Firm?.FirmName;

            Status = bid.Status;
            BidType = (BidTypeEnum)(bid.BidType ?? 0);
            MobePrice = bid.MobePrice ?? 0;
            DeMobePrice = bid.DeMobePrice ?? 0;
            DelayDeMobePrice = bid.DelayDeMobePrice ?? 0;
            IndustryId = bid.IndustryId;
            ProjectManagerId = bid.ProjectMangerId;
            CreatedUserName = bid.CreatedUser.FullName();
            #endregion
        }

        public byte BDCo { get; set; }

        public byte JCCo { get; set; }

        public int BidId { get; set; }

        public string FirmType { get; set; }

        public BidStatusEnum Status { get; set; }

        public DateTime? BidDate { get; set; }

        public BidTypeEnum BidType { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int? FirmNumber { get; set; }


        public string FirmName { get; set; }

        public int? DivisionId { get; set; }

        public int? IndustryId { get; set; }

        public string Location { get; set; }

        public string StateCodeId { get; set; }

        public int? ProjectManagerId { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }


        public string CreatedUserName { get; set; }

        public decimal MobePrice { get; set; }

        public decimal DeMobePrice { get; set; }

        public decimal DelayDeMobePrice { get; set; }

        public int? CustomerId { get; set; }

        public int? ContactId { get; set; }

    }

    public class PackageListViewModel
    {
        public PackageListViewModel()
        {

        }

        public PackageListViewModel(Bid bid, bool includeDetails = false)
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

        public byte BDCo { get; set; }
        public int BidId { get; set; }

        public List<PackageViewModel> List { get; }
    }

    public class PackageViewModel
    {
        public PackageViewModel()
        {

        }

        public PackageViewModel(BidPackage package, bool includeDetails = false)
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

            BidTypeId = (BidTypeEnum)(package.Bid.BidType ?? 0);
            if (includeDetails)
            {
                CostList = new PackageCostListViewModel(package);
                Price = new PriceViewModel(package);
                Bores = new BoreLineListViewModel(package);
            }
        }

        public byte BDCo { get; set; }
        public int BidId { get; set; }
        public int PackageId { get; set; }
        public string Description { get; set; }
        public BidStatusEnum Status { get; set; }
        public int? BoreTypeId { get; set; }
        public int? NumberOfBores { get; set; }
        public decimal? PipeSize { get; set; }
        public int? GroundDensityId { get; set; }
        public byte? EMCo { get; set; }
        public string RigCategoryId { get; set; }
        public int? DivisionId { get; set; }
        public int? IndustryId { get; set; }
        public int? MarketId { get; set; }

        public BidTypeEnum BidTypeId { get; set; }

        public PackageCostListViewModel CostList { get; }

        public PriceViewModel Price { get; }

        public BoreLineListViewModel Bores { get; }

    }

    public class PackageCostListViewModel
    {
        public PackageCostListViewModel()
        {

        }

        public PackageCostListViewModel(BidPackage package)
        {
            if (package == null)
                throw new System.ArgumentNullException(nameof(package));

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            Description = package.Description;
            #endregion

            List = package.fvBoreLineCostItems().Select(s => new CostDetailViewModel(s)).ToList();
            //List = package.vBoreLineCostItems.Select(s => new CostDetailViewModel(s)).ToList();
        }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        public string Description { get; set; }

        public List<CostDetailViewModel> List { get; }
    }

    public class CostDetailViewModel
    {
        public CostDetailViewModel()
        {

        }
        public CostDetailViewModel(vBidBoreLineCostItem item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = item.BDCo;
            BidId = item.BidId;
            BoreId = item.BoreId;
            Description = item.Description;
            Footage = item.Footage;
            PipeSize = item.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensityDescription;
            BudgetCategory = item.BudgetSubCategory;
            BudgetCodeDescription = item.BudgetCodeDescription;
            //if (shiftNum > 1)
            //{
            //    BudgetCodeDescription = item.BudgetCodeDescription + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            //}
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseDescription;
            PassId = 1;
            Units = item.Units;
            Multiplier = item.Multiplier;
            ExtUnits = item.Units * item.Multiplier;
            Cost = item.Cost;
            ExtCost = Math.Round(item.ExtCost ?? 0, 2);
            UM = item.UM;

            IntersectBoreId = item.BidBoreLine.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(tfudBDBI_SUMMARY_Result item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = (byte)item.Co;
            BidId = (int)item.BidId;
            BoreId = (int)item.BoreId;
            Description = item.Description;
            Footage = item.Footage;
            PipeSize = item.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensityDescription;
            BudgetCategory = item.BudgetSubCategory;
            BudgetCodeDescription = item.BudgetCodeDescription;
            //if (shiftNum > 1)
            //{
            //    BudgetCodeDescription = item.BudgetCodeDescription + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            //}
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseDescription;
            PassId = 1;
            Units = item.Units;
            Multiplier = item.Multiplier;
            ExtUnits = item.Units * item.Multiplier;
            Cost = item.Cost;
            ExtCost = Math.Round(item.ExtCost ?? 0, 2);
            UM = item.UM;

            IntersectBoreId = item.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(BidBoreLine line, BidBoreLineCostItem item, int shiftNum)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensity.Description;
            BudgetCategory = item.BudgetCode.CostType?.Description;
            BudgetCodeDescription = item.BudgetCode.Description;
            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.BudgetCode.Description + string.Format(VPContext.AppCultureInfo, " Shift# {0}", shiftNum);
            }
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.BudgetCode.PhaseId;
            PhaseDescription = item.BudgetCode.Phase?.Description;
            PassId = 1;
            Units = item.Units ?? 0;
            Multiplier = item.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.Multiplier ?? 1);
            Cost = item.Cost ?? 0;
            ExtCost = Math.Round(Units * Cost * Multiplier, 2);
            //if (item.ItemPhases.Count > 0)
            //{
            //    ExtCost = 0;
            //    ExtUnits = 0;
            //    Units = 0;
            //    foreach (var phase in item.ItemPhases)
            //    {
            //        Units += (phase.Units ?? 0);
            //        ExtUnits += (phase.Units ?? 0) * Multiplier;
            //        ExtCost += (phase.Units ?? 0) * (phase.Cost ?? 0) * (item.Multiplier ?? 1);
            //    }
            //}
            UM = item.BudgetCode.UM;

            IntersectBoreId = line.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(BidBoreLine line, BidBoreLineCostItemPhase item, int shiftNum)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensity.Description;

            BudgetCategory = item.CostItem.BudgetCode.CostType.Description;
            BudgetCodeDescription = item.CostItem.BudgetCode.Description;

            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.CostItem.BudgetCode.Description + string.Format(VPContext.AppCultureInfo, " Shift# {0}", shiftNum);
            }
            BudgetCodeId = item.CostItem.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseMaster.Description;
            PassId = item.PassId;
            Units = item.Units ?? 0;
            Multiplier = item.CostItem.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.CostItem.Multiplier ?? 1);
            Cost = item.CostItem.Cost ?? 0;
            ExtCost = Math.Round(Units * Cost * Multiplier, 0);
            UM = item.CostItem.BudgetCode.UM;
            IntersectBoreId = line.IntersectBoreId;
            #endregion mapping

        }


        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int BoreId { get; set; }

        public int? IntersectBoreId { get; set; }

        public string Description { get; set; }

        public decimal? Footage { get; set; }

        public decimal? PipeSize { get; set; }

        public string GroundDensityType { get; set; }

        public string GroundDensityDescription { get; set; }

        public string BudgetCategory { get; set; }

        public string BudgetCodeId { get; set; }

        public string BudgetCodeDescription { get; set; }

        public string PhaseId { get; set; }

        public string PhaseDescription { get; set; }

        public int PassId { get; set; }

        public decimal Units { get; set; }

        public decimal ExtUnits { get; set; }

        public decimal Multiplier { get; set; }

        public string UM { get; set; }

        public decimal Cost { get; set; }

        public decimal ExtCost { get; set; }


        public decimal? IntersectFootage { get; set; }



        public decimal? IntersectFootage2 { get; set; }
    }

    public class PriceViewModel : PackageViewModel
    {
        public PriceViewModel()
        {

        }

        public PriceViewModel(BidPackage package) : base(package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping

            StandardGM = .4M;
            DirtGM = package.DirtGM ?? StandardGM;
            RockGM = package.RockGM ?? StandardGM;

            DirtGM = DirtGM == 0 ? StandardGM : DirtGM;
            RockGM = RockGM == 0 ? StandardGM : RockGM;


            var packageSummary = new BidPackageSummary(package);

            DirtCost = packageSummary.DirtCost;
            RockCost = packageSummary.RockCost;
            DirtBidDays = packageSummary.DirtBidDays;
            RockBidDays = packageSummary.RockBidDays;
            TotalFootage = packageSummary.TotalFootage;
            UniqueBoreCount = packageSummary.UniqueBoreCount;

            if (package.ParentPackageId != null)
            {
                package.DirtGM = package.ParentPackage.DirtGM;
                package.DirtPrice = package.ParentPackage.DirtPrice;
                package.RockGM = package.ParentPackage.RockGM;
                package.RockPrice = package.ParentPackage.RockPrice;
            }

            if (package.DirtPrice != null)// && DirtRevenue != 0
            {
                DirtRevenue = (decimal)package.DirtPrice * TotalFootage;
                DirtGM = Math.Round((DirtRevenue - DirtCost) / DirtRevenue, 4);
                DirtLFPrice = (decimal)package.DirtPrice;
            }
            else
            {
                DirtRevenue = Math.Round(DirtCost * (1 / (1 - DirtGM)), 0);
                if (DirtRevenue != 0 && TotalFootage != 0)
                {
                    DirtLFPrice = Math.Round(DirtRevenue / TotalFootage, 2);
                }
            }

            if (package.RockPrice != null)// && RockRevenue != 0
            {
                RockRevenue = (decimal)package.RockPrice * TotalFootage;
                RockGM = Math.Round((RockRevenue - RockCost) / RockRevenue, 4);
                RockLFPrice = (decimal)package.RockPrice;
            }
            else
            {
                RockRevenue = Math.Round(RockCost * (1 / (1 - RockGM)), 0);
                if (RockRevenue != 0 && TotalFootage != 0)
                {
                    RockLFPrice = Math.Round(RockRevenue / TotalFootage, 2);
                }
            }

            if (TotalFootage != 0)
            {
                DirtLFCost = Math.Round(DirtCost / TotalFootage, 2);
                RockLFCost = Math.Round(RockCost / TotalFootage, 2);

            }

            DirtProfit = DirtRevenue - DirtCost;
            RockProfit = RockRevenue - RockCost;

            RockRevenueAdder = RockLFPrice - DirtLFPrice;
            #endregion

        }

        public decimal DirtBidDays { get; set; }

        public decimal RockBidDays { get; set; }

        public decimal DirtCost { get; set; }

        public decimal RockCost { get; set; }

        public decimal DirtLFCost { get; set; }

        public decimal RockLFCost { get; set; }

        public decimal StandardGM { get; set; }

        public decimal DirtGM { get; set; }

        public int UniqueBoreCount { get; set; }

        public decimal TotalFootage { get; set; }

        public decimal RockGM { get; set; }

        public decimal DirtRevenue { get; set; }

        public decimal RockRevenue { get; set; }

        public decimal DirtLFPrice { get; set; }

        public decimal RockLFPrice { get; set; }

        public decimal RockRevenueAdder { get; set; }

        public decimal DirtProfit { get; set; }

        public decimal RockProfit { get; set; }


    }

    public class BoreLineListViewModel
    {
        public BoreLineListViewModel()
        {

        }

        public BoreLineListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
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
            List = package.ActiveBoreLines.Select(s => new BoreLineViewModel(s)).ToList();
        }


        public byte BDCo { get; set; }

        public int BidId { get; set; }

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
                throw new System.ArgumentNullException(nameof(line));
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
            JCCo = line.JCCo ?? 1;
            JobId = line.JobId ?? "00-0000-00";
            ParentJobId = line.Package.JobId ?? "00-0000-00";
            AwardStatus = line.AwardStatus;

            #endregion

        }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int BoreId { get; set; }

        public int PackageId { get; set; }

        public DB.BidStatusEnum Status { get; set; }

        public string Description { get; set; }

        public decimal? Footage { get; set; }

        public decimal? PipeSize { get; set; }

        public int? BoreTypeId { get; set; }

        public byte? EMCo { get; set; }

        public string RigCategoryId { get; set; }

        public int? CrewCount { get; set; }

        public DB.BidAwardStatusEnum AwardStatus { get; set; }

        public byte? JCCo { get; set; }


        public string JobId { get; set; }

        public string ParentJobId { get; set; }

    }
}

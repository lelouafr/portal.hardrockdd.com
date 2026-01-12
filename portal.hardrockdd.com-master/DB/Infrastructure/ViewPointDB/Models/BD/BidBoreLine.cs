using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class BidBoreLine
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Package.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public List<tfudBDBI_SUMMARY_Result> fvBoreLineCostItems()
        {
            return db.tfudBDBI_SUMMARY(this.BDCo, this.BidId, this.PackageId, this.BoreId).ToList();
        }

        public int StatusId
        {
            get
            {
                return tStatusId ?? Bid.StatusId;
            }
            set
            {
                if (value != tStatusId)
                {
                    tStatusId = value;
                    Package.BoreLines.Where(f => f.IntersectBoreId == this.BoreId).ToList().ForEach(e => e.StatusId = value);
                }
            }
        }

        public BidStatusEnum Status
        {
            get
            {
                return (BidStatusEnum)StatusId;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public BidAwardStatusEnum AwardStatus
        {
            get
            {
                if (Status == BidStatusEnum.Awarded ||
                    Status == BidStatusEnum.NotAwarded)
                {
                    return (BidAwardStatusEnum)StatusId;
                }

                return BidAwardStatusEnum.Pending;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public string Description { get => tDescription; set => tDescription = value?.Trim(); }

        public int? BoreTypeId
        {
            get
            {
                return tBoreTypeId;
            }
            set
            {
                if (tBoreTypeId != value)
                    UpdateBoreType(value);
            }
        }

        public void UpdateBoreType(int? value)
        {
            if (tBoreTypeId == 3 && value != 3)
            {
                Package.BoreLines.ToList().ForEach(bore => {
                    if (bore.IntersectBoreId == this.BoreId)
                        bore.Status = BidStatusEnum.Deleted;
                });
            }
            else if (value == 3)
            {
                var intersectBore = Package.BoreLines.FirstOrDefault(f => f.IntersectBoreId == this.BoreId);
                if (intersectBore == null)
                {
                    intersectBore = Package.AddBoreLine();
                    intersectBore.Description = string.Format("{0} Intersect", this.Description);
                    intersectBore.IntersectBoreId = this.BoreId; 
                    intersectBore.tFootage = this.Footage;
                    intersectBore.tPipeSize = this.PipeSize;
                    intersectBore.RecalcNeeded = true;
                    intersectBore.tBoreTypeId = value;
                    //intersectBore.IntersectBore.Add(this);
                }
                else
                {
                    intersectBore.tFootage = this.Footage;
                    intersectBore.tPipeSize = this.PipeSize;
                    intersectBore.RecalcNeeded = true;
                    intersectBore.tBoreTypeId = value;
                    intersectBore.tStatusId = this.tStatusId;
                }
            }

            tBoreTypeId = value;
        }

        public int? DivisionId
        {
            get
            {
                return tDivisionId;
            }
            set
            {
                UpdateDivision(value);
            }
        }

        public void UpdateDivision(int? value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;
            var job = Job;
            if ((Division == null && tDivisionId != null) || Division?.PMCo != Division?.WPDivision?.HQCompany?.JCCo)
            {
                var division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == tDivisionId);
                if (division != null)
                {
                    division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == division.DivisionId && f.PMCo == division.WPDivision.HQCompany.JCCo);
                    Division = division;
                    JCCo = division.PMCo;
                    tDivisionId = division.DivisionId;
                }
                else
                {
                    Division = null;
                    JCCo = null;
                    tDivisionId = null;
                }
            }
            if (tDivisionId != value)
            {
                var division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == value);
                if (division != null)
                {
                    division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == division.DivisionId && f.PMCo == division.WPDivision.HQCompany.JCCo);
                    Division = division;
                    JCCo = division.PMCo;
                    tDivisionId = division.DivisionId;
                }
                else
                {
                    Division = null;
                    JCCo = null;
                    tDivisionId = null;
                }
            }

            //Correct the Job Template Assigned to he Bid
            if (job != null && Division != null)
            {
                if (job?.JCCo != JCCo && Job?.JobType == JCJobTypeEnum.Template)
                {
                    Job = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob;
                    tJobId = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob.JobId;
                }
            }
        }

        public int? CustomerId
        {
            get
            {
                return tCustomerId;
            }
            set
            {
                UpdateCustomer(value);
            }
        }

        public void UpdateCustomer(int? value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;

            if (Customer == null && tCustomerId != null)
            {
                var customer = db.Customers.FirstOrDefault(f => f.CustomerId == tCustomerId);
                if (customer != null)
                {
                    Customer = customer;
                    //CustGroupId = customer.CustGroupId;
                    tCustomerId = customer.CustomerId;
                }
                else
                {
                    Customer = null;
                    JCCo = null;
                    tCustomerId = null;
                }
            }
            if (tCustomerId != value)
            {
                var customer = db.Customers.FirstOrDefault(f => f.CustomerId == value);
                if (customer != null)
                {
                    Customer = customer;
                    //CustGroupId = customer.CustGroupId;
                    tCustomerId = customer.CustomerId;
                }
                else
                {
                    Customer = null;
                    JCCo = null;
                    tCustomerId = null;
                }
            }

        }

        public int? IndustryId
        {
            get
            {
                return tIndustryId;
            }
            set
            {
                UpdateIndustry(value);
            }
        }

        public void UpdateIndustry(int? value)
        {
            if (tIndustryId != value)
            {
                tMarketId = value;
                tMarketId = null;
            }
        }

        public int? MarketId
        {
            get
            {
                return tMarketId;
            }
            set
            {
                UpdateMarket(value);
            }
        }

        public void UpdateMarket(int? value)
        {
            UpdateIndustry(tIndustryId);
            if (tMarketId != value)
            {
                tMarketId = value;
            }
        }

        public decimal? PipeSize
        {
            get
            {
                return tPipeSize;
            }
            set
            {
                UpdatePipeSize(value);
            }
        }

        public void UpdatePipeSize(decimal? value)
        {
            var originalValue = tPipeSize;
            if (tPipeSize != value)
            {
                UpdatePullHeadBudget(value);
                tPipeSize = value;
                UpdateMudBudget();
                RecalcNeeded = true;
                Package.BoreLines.Where(f => f.IntersectBoreId == BoreId).ToList().ForEach(intBore => {
                    intBore.PipeSize = value;
                });
            }
        }

        public string RigCategoryId
        {
            get
            {
                return tRigCategoryId;
            }
            set
            {
                UpdateRigCategory(value);
                UpdateLaborBudget();
            }
        }

        public void UpdateRigCategory(string value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;

            if (RigCategory == null && tRigCategoryId != null)
            {
                var category = db.EquipmentCategories.FirstOrDefault(f => f.CategoryId == tRigCategoryId);
                if (category != null)
                {
                    RigCategory = category;
                    //EMCo = category.EMCo;
                    tRigCategoryId = category.CategoryId;
                }
                else
                {
                    RigCategory = null;
                    //EMCo = null;
                    tRigCategoryId = null;
                }
            }
            if (tRigCategoryId != value)
            {
                var category = db.EquipmentCategories.FirstOrDefault(f => f.CategoryId == value);
                if (category != null)
                {
                    RigCategory = category;
                    //EMCo = category.EMCo;
                    CrewCount = category.CrewSize;
                    tRigCategoryId = category.CategoryId;
                }
                else
                {
                    RigCategory = null;
                    //EMCo = null;
                    tRigCategoryId = null;
                }

                UpdateLaborBudget();
                UpdateEquipmentBudget();
            }
        }

        public int? GroundDensityId
        {
            get
            {
                return tGroundDensityId;
            }
            set
            {
                UpdateGroundDensity(value);
            }
        }

        public void UpdateGroundDensity(int? value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;

            if (GroundDensity == null && tGroundDensityId != null)
            {
                var groundDensity = db.GroundDensities.FirstOrDefault(f => f.GroundDensityId == tGroundDensityId);
                if (groundDensity != null)
                {
                    GroundDensity = groundDensity;
                    tGroundDensityId = groundDensity.GroundDensityId;
                }
                else
                {
                    GroundDensity = null;
                    tGroundDensityId = null;
                }
            }
            if (tGroundDensityId != value)
            {
                var orgGroundDensityId = tGroundDensityId;

                var groundDensity = db.GroundDensities.FirstOrDefault(f => f.GroundDensityId == value);
                if (groundDensity != null)
                {

                    var updCostItems = CostItems.Where(f => f.GroundDensityId == orgGroundDensityId).ToList();
                    foreach (var item in updCostItems)
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Detached;
                        item.GroundDensityId = groundDensity.GroundDensityId;
                        CostItems.Add(item);
                    }

                    var updProductions = Passes.Where(f => f.GroundDensityId == orgGroundDensityId).ToList();
                    foreach (var item in updProductions)
                    {
                        var newItem = Passes.FirstOrDefault(f => f.GroundDensityId == groundDensity.GroundDensityId &&
                                                                 f.PhaseId == item.PhaseId &&
                                                                 f.PassId == item.PassId);
                        if (newItem == null)
                        {
                            db.Entry(item).State = System.Data.Entity.EntityState.Detached;
                            item.GroundDensityId = groundDensity.GroundDensityId;
                            Passes.Add(item);
                        }
                        else
                        {
                            newItem.BoreSize = item.BoreSize;
                            newItem.ProductionRate = item.ProductionRate;
                            newItem.ProductionDays = item.ProductionDays;
                            newItem.ProductionCalTypeId = item.ProductionCalTypeId;
                            newItem.BudgetCodeId = item.BudgetCodeId;
                            newItem.Multiplier = item.Multiplier;
                            
                        }
                    }
                    updCostItems = CostItems.Where(f => f.GroundDensityId != groundDensity.GroundDensityId && f.GroundDensityId != 0).ToList();
                    updProductions = Passes.Where(f => f.GroundDensityId != groundDensity.GroundDensityId && f.GroundDensityId != 0).ToList();
                    updCostItems.ForEach(e => { CostItems.Remove(e); });
                    updProductions.ForEach(e => { Passes.Remove(e); });

                    GroundDensity = groundDensity;
                    tGroundDensityId = groundDensity.GroundDensityId;
                }
                else
                {
                    GroundDensity = null;
                    tGroundDensityId = null;
                }
                RecalcNeeded = true;
                ApplyPackageProductionDefaults();
            }
        }

        public string CustomerReference
        {
            get
            {
                return tCustomerReference;
            }
            set
            {
                tCustomerReference = value;
            }
        }

        public string? JobId
        {
            get
            {
                return tJobId;
            }
            set
            {
                UpdateJob(value);
            }
        }

        public int? CrewCount
        {
            get
            {
                return tCrewCount;
            }
            set
            {
                if (tCrewCount != value)
                {
                    tCrewCount = value ?? 0;
                    UpdateLaborBudget();
                    RecalcNeeded = true;
                }
            }
        }

        public decimal? Footage
        {
            get
            {
                return tFootage;
            }
            set
            {
                if ((tFootage ?? 0) != (value ?? 0))
                {
                    tFootage = value ?? 0;
                    UpdateMudBudget();
                    UpdateDirtToolingBudget();
                    RecalcNeeded = true;

                    Package.BoreLines.Where(f => f.IntersectBoreId == BoreId).ToList().ForEach(intBore => {
                        intBore.Footage = value;
                    });
                }
            }
        }

        public void UpdateJob(string? value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;

            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                if (job != null)
                {
                    Job = job;
                    JCCo = job.JCCo;
                    tJobId = job.JobId;
                }
                else
                {
                    Job = null;
                    JCCo = null;
                    tJobId = null;
                }
            }
            if (tJobId != value)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == value);
                if (job != null)
                {
                    Job = job;
                    JCCo = job.JCCo;
                    tJobId = job.JobId;
                }
                else
                {
                    Job = null;
                    JCCo = null;
                    tJobId = null;
                }
            }

        }

        public List<BidBoreLinePass> RockPhases
        {
            get
            {
                return Passes.Where(f => f.GroundDensityId == this.GroundDensityId &&
                                         f.Deleted != true)
                            .OrderBy(o => o.PhaseMaster.SortId)
                            .ThenBy(o => o.PassId)
                            .ToList();


            }
        }

        public List<BidBoreLinePass> DirtPhases
        {
            get
            {
                return Passes.Where(f => f.GroundDensityId == 0 &&
                                         f.Deleted != true)
							.OrderBy(o => o.PhaseMaster.SortId)
							.ThenBy(o => o.PassId)
							.ToList();


			}
        }

        private List<BidBoreLineCost> _CostDetails;

        public List<BidBoreLineCost> CostDetails
        {
            get
            {
                if (_CostDetails == null)
                {
                    _CostDetails = GetCostDetails();
                }
                return _CostDetails;
            }

        }

        private List<BidBoreLineCost> GetCostDetails()
        {
            var result = new List<BidBoreLineCost>();

            if (this == null)
                return result;

            foreach (var cost in this.CostItems)
            {
                foreach (var item in cost.ItemPhases)
                {
                    for (int i = 1; i <= (item.Shift ?? 1); i++)
                    {
                        result.Add(new BidBoreLineCost(this, item, i));
                    }
                }
                if (!cost.ItemPhases.Any())
                {
                    result.Add(new BidBoreLineCost(this, cost, 1));
                }
            }

            return result;
        }

        public void CreateProjectBudget()
        {
            /****Dirt Budget ****/
            var dirtBudgetNo = string.Format("Dirt-{0}", BidId);
            var dirtBgt = Job.AddBudget(this, dirtBudgetNo);
            dirtBgt.CreateBudgetLines(this, 0);

            /****Rock Budget ****/
            var rockBudgetNo = string.Format("Rock-{0}", BidId);
            var rockBgt = Job.AddBudget(this, rockBudgetNo);
            rockBgt.CreateBudgetLines(this, (int)GroundDensityId);
        }

        public void CreateJob()
        {
            var boreCnt = 1;
            var description = string.Format("{0} - {1}", Package.Project.Description, Description);
            if (Job?.JobType == JCJobTypeEnum.Template)
            {
                Job = null;
            }
            if (Job?.Description != null && Job?.JobType != JCJobTypeEnum.Template)
            {
                description = Job?.Description;
            }
            if (description.Length >= 60)
            {
                description = description.Substring(0, 59);
            }
            if (Job == null)
            {
                var jobId = string.Format("{0}{1}", Package.Project.JobId, boreCnt.ToString().PadLeft(2, '0'));
                while (Package.Project.SubJobs.Any(f => f.JobId == jobId))
                {
                    boreCnt++;
                    jobId = string.Format("{0}{1}", Package.Project.JobId, boreCnt.ToString().PadLeft(2, '0'));
                }

                Job = new Job
                {
                    JCCo = (byte)Division.WPDivision.HQCompany.JCCo,
                    JobId = jobId,
                    Description = description,
                    JobStatus = 1,
                    Status = JCJobStatusEnum.Open,
                    //VendorGroup = bore.Co,
                    DivisionDesc = Division.Description,
                    DivisionId = (byte)DivisionId,
                    Owner = Bid.Firm.FirmName.Length >= 30 ? Bid.Firm.FirmName.Substring(0, 29) : Bid.Firm.FirmName,
                    OwnerId = Bid.tFirmId,
                    Footage = (short)Footage,
                    PipeSize = (byte)PipeSize,
                    JCCompanyParm = Division.WPDivision.HQCompany.JCCompanyParm,
                    JobTypeId = "1",
                    RigId = RigId,
                    CrewId = CrewId,
                    ProjectMgr = Bid.ProjectMangerId ?? Division?.WPDivision?.ManagerId,
					ParentJobId = Package.Project.JobId,

                    PRCo = Division.WPDivision.HQCompany.JCCompanyParm.PRCo,
                    EMCo = Division.WPDivision.HQCompany.EMCo,

                    LockPhases = "N",
                    LiabTemplate = 1,
                    TaxGroup = (byte)Division.WPDivision.HQCompany.TaxGroupId,
                    MarkUpDiscRate = 0,
                    PRStateCode = Bid.StateCodeId,
                    Certified = "N",
                    HaulTaxOpt = 0,
                    BaseTaxOn = "J",
                    UpdatePlugs = "N",
                    ClosePurgeFlag = "N",
                    AutoAddItemYN = "N",
                    WghtAvgOT = "N",
                    HrsPerManDay = 8,
                    AutoGenSubNo = "T",
                    UpdateAPActualsYN = "Y",
                    UpdateMSActualsYN = "Y",
                    AutoGenPCONo = "P",
                    AutoGenMTGNo = "P",
                    AutoGenRFINo = "P",
                    AutoGenRFQNo = "T",
                    ApplyEscalators = "N",
                    UseTaxYN = "N",
                    PCVisibleInJC = "Y",
                    SubmittalReviewDaysAutoCalcYN = "Y",
                };
                Package.Project.SubJobs.Add(Job);
            }
            else
            {
                Job.ParentJobId = Package.Project.JobId;
                Job.DivisionDesc = Division.Description;
                Job.DivisionId = (byte)Division.DivisionId;
                Job.Owner = Bid.Firm.FirmName;
                Job.PipeSize = (byte)PipeSize;
                Job.CrewId ??= CrewId;
                Job.RigId ??= RigId;
                Job.Footage = (short)Footage;
                Job.PRStateCode = Bid.StateCodeId;
                //CurFootage = (decimal)(Job.Footage ?? Footage);
            }
            Job.BidNumber = BidId.ToString();
            JCCo = Job.JCCo;
            tJobId = Job.JobId;
            if (!Job.BidBoreLines.Any(f=> f.BoreId == this.BoreId))
			{
				Job.BidBoreLines.Add(this);
			}

            foreach (var asm in this.budPMLAs.ToList())
            {
                asm.JCCo = this.JCCo;
                asm.JobId = this.JobId;
            }
            
            Job.BuildProgressTrackingFromBid();
        }

        public void CreateJobContract()
        {
            if (Job == null)
                return;
            if (Job.Contract?.ContractId == "00-0000-00")
            {
                Job.Contract = null;
            }

            if (Job.Contract == null)
            {
                Job.Contract = new JCContract
                {
                    JCCo = Job.JCCo,
                    ContractId = Job.JobId,
                    Description = Job.Description,
                    ContractStatus = 1,

                    Customer = Package.Customer,
                    CustGroupId = Package.Customer.CustGroupId,
                    CustomerId = Package.CustomerId,
                    CustomerReference = Package.CustomerReference,
                    StartDate = Package.StartDate ?? Bid.StartDate,

                    DepartmentId = "1",
                    OriginalDays = 0,
                    CurrentDays = 0,
                    TaxInterface = "N",
                    TaxGroup = Job.JCCompanyParm.HQCompanyParm.TaxGroupId,
                    RetainagePCT = 0,
                    DefaultBillType = "N",
                    OrigContractAmt = 0,
                    ContractAmt = 0,
                    BilledAmt = 0,
                    ReceivedAmt = 0,
                    CurrentRetainAmt = 0,
                    SIMetric = "N",
                    BillOnCompletionYN = "N",
                    CompleteYN = "N",
                    RoundOpt = "N",
                    ReportRetgItemYN = "N",
                    JBFlatBillingAmt = 0,
                    JBLimitOpt = "N",
                    ClosePurgeFlag = "N",
                    UpdateJCCI = "N",
                    MaxRetgOpt = "N",
                    MaxRetgPct = 0,
                    MaxRetgAmt = 0,
                    InclACOinMaxYN = "Y",
                    MaxRetgDistStyle = "C",
                    AUUseTrustAccounts = "N",
                };
                Job.ContractId = Job.Contract.ContractId;
                Job.Contract.JCCompanyParm = Division.WPDivision.HQCompany.JCCompanyParm;
                Job.Contract.StartMonth = new DateTime(Job.Contract.StartDate.Value.Year, Job.Contract.StartDate.Value.Month, 1);
            }
        }

        public void UpdateContractItems()
        {
            foreach (var item in Job.Contract.ContractItems)
            {
                var dirtPrice = Package.ParentPackageId == null ? (Package.DirtPrice ?? 0) : (Package.ParentPackage.DirtPrice ?? 0);
                var rockPrice = Package.ParentPackageId == null ? (Package.RockPrice ?? 0) : (Package.ParentPackage.RockPrice ?? 0);
                if (item.UM == "LF")
                {
                    item.OrigContractUnits = (CurFootage ?? (Footage ?? 0));
                }
                if (item.Item.Contains("1"))
                {
                    item.OrigContractUnits = (CurFootage ?? (Footage ?? 0));
                    item.OrigUnitPrice = dirtPrice;
                    item.Description = string.Format("{0} {1:F0} Inch", BoreType.Description, PipeSize);
                }
                if (item.Item.Contains("2"))
                {
                    item.OrigContractUnits = (CurFootage ?? (Footage ?? 0));
                    item.OrigUnitPrice = Package.ParentPackageId == null ? (Package.DirtPrice ?? 0) : (Package.ParentPackage.DirtPrice ?? 0);
                    item.OrigUnitPrice = (rockPrice - dirtPrice);
                    item.Description = string.Format("Rock Adder {0:F0} Inch", PipeSize);
                }
            }
        }

        public BidBoreLineCostItem AddCostItem(ProjectBudgetCode bgtItem, int groundDensityId)
        {
            var costItem = CostItems.FirstOrDefault(f => f.GroundDensityId == groundDensityId && f.BudgetCodeId == bgtItem.BudgetCodeId && f.IsPackageCost == false);
            if (costItem == null)
            {
                costItem = new BidBoreLineCostItem()
                {
                    BoreLine = this,
                    BudgetCode = bgtItem,

                    BDCo = BDCo,
                    BidId = BidId,
                    BoreId = BoreId,
                    LineId = CostItems.Where(f => f.GroundDensityId == groundDensityId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                    GroundDensityId = groundDensityId,
                    BudgetCodeId = bgtItem.BudgetCodeId,
                    Units = 1,
                    CurUnits = 1,
                    Multiplier = 1,
                    Cost = bgtItem.UnitCost,
                    IsPackageCost = false,
                };
                CostItems.Add(costItem);
                RecalcNeeded = true;
            }

            return costItem;
        }
        
        public List<BidBoreLineCostItem> AddCostItems(ProjectBudgetCode bgtItem)
        {
            if (bgtItem == null)
                return new List<BidBoreLineCostItem>();

            var results = new List<BidBoreLineCostItem>();
            if (bgtItem.RockOnly != "Y")
            {
                var dirtCostItem = AddCostItem(bgtItem, 0);
                results.Add(dirtCostItem);
            }
            if (GroundDensityId != null)
            {
                var groundDensityId = (int)GroundDensityId;
                var rockCostItem = AddCostItem(bgtItem, groundDensityId);
                results.Add(rockCostItem);
            }
            return results;
        }

        public List<BidBoreLinePass> AddProductionPasses(PhaseMaster phase, int? passId = null)
        {
            var results = new List<BidBoreLinePass>();
            if (passId == null)
                passId = Passes.Where(f => f.PhaseId == phase.PhaseId).DefaultIfEmpty().Max(max => max == null ? 0 : max.PassId) + 1;


            var dirtPass = DirtPhases.FirstOrDefault(f => f.PhaseId == phase.PhaseId && f.PassId == passId);
            if (dirtPass == null)
            {
                dirtPass = new BidBoreLinePass
                {
                    BoreLine = this,
                    db = db,
                    PhaseMaster = phase,

                    BDCo = BDCo,
                    BidId = BidId,
                    BoreId = BoreId,
                    UM = "LF",
                    Multiplier = 1,
                    Deleted = false,
                    ProductionDays = 0,
                    ProductionRate = 0,
                    ProductionCalTypeId = 0,
                    tBoreSize = 0,
                    PhaseId = phase.PhaseId,
                    //PhaseGroupId = phase.PhaseGroupId,
                    PassId = (int)passId,
                    GroundDensityId = 0,
                };
                Passes.Add(dirtPass);
            }

            var rockPass = RockPhases.FirstOrDefault(f => f.PhaseId == phase.PhaseId && f.PassId == passId);
            if (rockPass == null)
            {
                rockPass = new BidBoreLinePass
                {
                    BoreLine = this,
                    db = db,
                    PhaseMaster = phase,

                    BDCo = BDCo,
                    BidId = BidId,
                    BoreId = BoreId,
                    UM = "LF",
                    Multiplier = 1,
                    Deleted = false,
                    ProductionDays = 0,
                    ProductionRate = 0,
                    ProductionCalTypeId = 0,
                    tBoreSize = 0,

                    PhaseId = phase.PhaseId,
                    //PhaseGroupId = phase.PhaseGroupId,
                    PassId = (int)passId,
                    GroundDensityId = (int)(GroundDensityId ?? 0),
                };
                Passes.Add(rockPass);
            }


            results.Add(dirtPass);
            results.Add(rockPass);

            return results;
        }

        public void UpdateLaborBudget()
        {
            if (RigCategory == null)
                UpdateRigCategory(tRigCategoryId);

            var pmCompany = Bid.Company.PMCompanyParm;
            if (CrewCount != null && RigCategoryId != null)
            {
                
                var rigCat = RigCategory;

                #region update labor budget
                var laborDesc = string.Format("Labor Per Man - {0}", rigCat.Description);
                var stdBgt = pmCompany.AddBudgetCode("LB", "Labor Per Man");
                var bgt = pmCompany.AddBudgetCode("LB", laborDesc, stdBgt);
                AddCostItems(bgt).ForEach(e => {
                        var maxShift = 1;
                        if (this.Passes.Any())
                            maxShift = this.Passes.Max(max => (int?)max.ShiftCnt ?? 1);

                        e.ShiftCnt = maxShift;
                        e.Cost = bgt.UnitCost;
                        e.Multiplier = (CrewCount * (e.ShiftCnt ?? 1));
                    });

                var delList = CostItems.Where(f => f.BudgetCode.Description.Contains("Labor Per Man") && f.BudgetCodeId != bgt.BudgetCodeId).ToList();
                delList.ForEach(e => CostItems.Remove(e));

                #endregion update labor budget

                #region update labor Indirect budget
                laborDesc = string.Format("Labor Indirect - {0}", rigCat.Description);
                stdBgt = pmCompany.AddBudgetCode("LB", "Labor Indirect");
                bgt = pmCompany.AddBudgetCode("LB", laborDesc, stdBgt);               
                AddCostItems(bgt).ForEach(e => {
                        var maxShift = 1;
                        if (this.Passes.Any())
                            maxShift = this.Passes.Max(max => (int?)max.ShiftCnt ?? 1);

                        e.ShiftCnt = maxShift;
                        e.Cost = bgt.UnitCost;
                        e.Multiplier = (CrewCount * (e.ShiftCnt ?? 1));
                    });

                delList = CostItems.Where(f => f.BudgetCode.Description.Contains("Labor Indirect") && f.BudgetCodeId != bgt.BudgetCodeId).ToList();
                delList.ForEach(e => CostItems.Remove(e));
                #endregion update labor budget

                #region update Labor Prevailing Wage budget

                if (Bid.BidId == 2052)
                {
                    laborDesc = string.Format("Labor Prevailing Wage");
                    stdBgt = pmCompany.AddBudgetCode("LB", "Labor Prevailing Wage");
                    bgt = pmCompany.AddBudgetCode("LB", laborDesc, stdBgt);
                    
                    AddCostItems(bgt).ForEach(e => {
                            var maxShift = 1;
                            if (this.Passes.Any())
                                maxShift = this.Passes.Max(max => (int?)max.ShiftCnt ?? 1);

                            e.ShiftCnt = maxShift;
                            e.Cost = bgt.UnitCost;
                            e.Multiplier = (CrewCount * (e.ShiftCnt ?? 1));
                        });
                }
                #endregion update labor budget               
            }
        }

        public void UpdateEquipmentBudget()
        {
            if (RigCategory == null)
                UpdateRigCategory(tRigCategoryId);

            if (RigCategory == null)
                return;


            var budgetDesc = string.Format("{0}", RigCategory.Description);
            var stdBgt = Bid.Company.PMCompanyParm.AddBudgetCode("EQ", "Rig Equipment");
            var bgt = Bid.Company.PMCompanyParm.AddBudgetCode("EQ", budgetDesc, stdBgt);
            AddCostItems(bgt).ForEach(e => {                
                e.Cost = bgt.UnitCost;
                e.Multiplier = e.Multiplier < 1 ? 1 : e.Multiplier;
            });

            var delList = CostItems.Where(f => f.BudgetCodeId.Contains("EQ") && f.BudgetCodeId != bgt.BudgetCodeId).ToList();
            delList.ForEach(e => CostItems.Remove(e));
        }

        public void UpdatePullHeadBudget(decimal? newPipeSize)
        {
            #region update Pull Head
            var bgt = newPipeSize == null ? null : Bid.Company.PMCompanyParm.AddBudgetCode("MT", string.Format("Pull Head {0} inch", Math.Round((decimal)newPipeSize, 0)));                        
            AddCostItems(bgt).ForEach(e => {
                e.Cost = bgt.UnitCost;
                e.Units = 1;
                e.CurUnits = 1;
                e.Multiplier = e.Multiplier < 1 ? 1 : e.Multiplier;
            });

            var oldbgt = PipeSize == null ? null : Bid.Company.PMCompanyParm.AddBudgetCode("MT", string.Format("Pull Head {0} inch", Math.Round((decimal)PipeSize, 0)));
            if (oldbgt.BudgetCodeId != bgt.BudgetCodeId)
            {
                CostItems.Where(f => f.BudgetCodeId == oldbgt?.BudgetCodeId && f.IsPackageCost != true).ToList().ForEach(e => CostItems.Remove(e));
            }

            #endregion update Pull Head
        }

        public void UpdateMudBudget()
        {
            #region update Mud budget
            var bgt = Bid.Company.PMCompanyParm.AddBudgetCode("MT", "Drilling mud");
            AddCostItems(bgt).ForEach(e => {
                e.Cost = bgt.UnitCost;
                e.Units = Footage;
                e.CurUnits = CurFootage ?? Footage;
                e.Multiplier = decimal.ToInt32(PipeSize ?? 0);
            });
            #endregion update labor budget
        }

        public void UpdateRockToolingBudget()
        {
            if (this.Intersect != null)
                    return;

            var rockPasses = Passes.Where(f => f.ProductionRate != 0 && f.tBoreSize > 0 && f.GroundDensityId == this.GroundDensityId).ToList();
            var groundDensityId = (int)GroundDensityId;
            var rockCosts = new List<BidBoreLineCostItem>();

            

            foreach (var pass in rockPasses)
            {
                var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == pass.BDCo &&
                                                                    f.PhaseId == pass.PhaseId &&
                                                                    f.Radius == pass.tBoreSize &&
                                                                    f.Hardness == pass.GroundDensityId.ToString() &&
                                                                    f.BudgetCodeId.StartsWith("TM"));

                if (bgt != null)
                {
                    var rockToolCost = AddCostItem(bgt, groundDensityId);
                    rockToolCost.Units = this.Footage;
                    rockToolCost.CurUnits = CurFootage ?? Footage;

                    rockCosts.Add(rockToolCost);

                }
            }

            var costs = this.CostItems.Where(f => f.GroundDensityId != 0 && f.BudgetCodeId.StartsWith("TM")).ToList();            
            var delList = costs.Where(f => !rockCosts.Any(a => a.BudgetCodeId == f.BudgetCodeId)).ToList();
            delList.ForEach(e => CostItems.Remove(e));
        }
        
        public void UpdateDirtToolingBudget()
        {
            if (this.Intersect != null)
                return;

            #region update dirt tooling budget

            var bgt = Bid.Company.PMCompanyParm.AddBudgetCode("TM", "Dirt Tooling");
            var dirtToolCost = AddCostItem(bgt, 0);

            ////Why am I removing the intersect bore footage????
            //var footage = Footage;
            //var intersect = this.IntersectBore.FirstOrDefault();
            //if (intersect != null)
            //{
            //    var costIntersect = intersect.CostItems.FirstOrDefault(w => w.BudgetCodeId == dirtToolCost.BudgetCodeId && w.GroundDensityId == dirtToolCost.GroundDensityId);
            //    if (costIntersect != null)
            //        footage -= intersect.Footage * costIntersect.Multiplier;
            //    else
            //        footage -= intersect.Footage;
            //}

            dirtToolCost.Cost = bgt.UnitCost;
            dirtToolCost.Units = Footage;
            dirtToolCost.CurUnits = CurFootage ?? Footage;
            dirtToolCost.Multiplier = 1;

            #endregion update dirt tooling budget
        }

        public void RecalculateCostUnits()
        {
            AddMissingCostItems();
            CostItems.Where(f => (  f.BudgetCodeId.StartsWith("RE-", StringComparison.Ordinal) ||
                                    f.BudgetCodeId.StartsWith("CI-", StringComparison.Ordinal) ||
                                    f.BudgetCodeId.StartsWith("LB-", StringComparison.Ordinal) ||
                                    f.BudgetCodeId.StartsWith("EQ-", StringComparison.Ordinal) ||
                                    f.BudgetCodeId.StartsWith("TM-", StringComparison.Ordinal) ||
                                    f.BudgetCodeId.StartsWith("MT-", StringComparison.Ordinal)) && 
                                f.IsPackageCost != true).ToList()
                            .ForEach(e => e.ReCalulateCostItem());
            RecalcNeeded = false;
        }

        public void AddMissingCostItems()
        {
            var bgtCodeItems = CostItems
                .Where(f => f.IsPackageCost != true && !f.BudgetCodeId.StartsWith("TM"))
                .GroupBy(g => g.BudgetCode)
                .Select(s => new
                {
                    BudgetCode = s.Key,
                    Multipler = s.DefaultIfEmpty().Max(max => max == null ? 1 : max.Multiplier ?? 0),
                    CostItems = s.Where(f => f.GroundDensityId == 0 || f.GroundDensityId == GroundDensityId).ToList()
                })
                .ToList();
            foreach (var bgt in bgtCodeItems)
            {
                AddCostItems(bgt.BudgetCode).ForEach(e => {
                    if (e.Multiplier != bgt.Multipler)
                    {
                        e.Multiplier = bgt.Multipler;
                    }
                });
            }
        }

        public void ApplyPackageProductionDefaults()
        {
            if (db != null)
            {
                db.BulkSaveChanges();
                db.udBDBL_DefaultProductionRateForLineInsert(BDCo, BidId, BoreId);
                if (db.Entry(this).State != System.Data.Entity.EntityState.Added)
                {
                    db.Entry(this).Reload();
                }

            }
        }

        public void ApplyPackageCostItems()
        {
            var package = this.Package;

            foreach (var cost in package.CostItems)
            {
                if (cost.tApplied == 1 && cost.BudgetCode != null)
                {
                    AddCostItems(cost.BudgetCode).ForEach(e => {
                        e.Multiplier = (int)cost.tMultiplier;
                    });
                }
                else
                {
                    var delList = CostItems.Where(f => f.BudgetCodeId == cost.tBudgetCodeId).ToList();
                    delList.ForEach(e => CostItems.Remove(e));
                }
            }
        }

        public void Recalculate()
        {
            if (RecalcNeeded == true)
            {
                UpdateMudBudget();
                UpdateDirtToolingBudget();
                UpdateRockToolingBudget();
                UpdateLaborBudget();
                UpdateEquipmentBudget();                
                UpdatePullHeadBudget((decimal)PipeSize);
                ApplyPackageCostItems();
                RecalculateCostUnits();
            }
        }

        //ScheduleViewModel

        //public void UpdateFromModel(Models.Views.Bid.Forms.Bore.Schedule.ScheduleViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    RigCategoryId = model.RigCategoryId;
        //    Description = model.Description;
        //    RigId = model.RigId;
        //    StartDate = model.StartDate;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class BidBoreLine
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

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

        public DB.BidStatusEnum Status
        {
            get
            {
                return (DB.BidStatusEnum)StatusId;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public DB.BidAwardStatusEnum AwardStatus
        {
            get
            {
                if (Status == DB.BidStatusEnum.Awarded ||
                    Status == DB.BidStatusEnum.NotAwarded)
                {
                    return (DB.BidAwardStatusEnum)StatusId;
                }

                return DB.BidAwardStatusEnum.Pending;
            }
            set
            {
                StatusId = (int)value;
            }
        }

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
                        bore.Status = DB.BidStatusEnum.Deleted;
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
            //var db = VPEntities.GetDbContextFromEntity(this);;
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
                if (job?.JCCo != JCCo && Job?.JobType == DB.JCJobTypeEnum.Template)
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
            //var db = VPEntities.GetDbContextFromEntity(this);;

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
            //var db = VPEntities.GetDbContextFromEntity(this);;

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
                ApplyPackageProductionDefaults();
            }
        }

        public void UpdateGroundDensity(int? value)
        {
            //var db = VPEntities.GetDbContextFromEntity(this);;

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
                        db.Entry(item).State = System.Data.Entity.EntityState.Detached;
                        item.GroundDensityId = groundDensity.GroundDensityId;
                        Passes.Add(item);
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
            //var db = VPEntities.GetDbContextFromEntity(this);;

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
                                         f.Deleted != true
                                         ).ToList();


            }
        }

        public List<BidBoreLinePass> DirtPhases
        {
            get
            {
                return Passes.Where(f => f.GroundDensityId == 0 &&
                                         f.Deleted != true
                                         ).ToList();


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
            var dirtBudgetNo = string.Format(AppCultureInfo.CInfo(), "Dirt-{0}", BidId);
            var dirtBgt = Job.AddBudget(this, dirtBudgetNo);
            dirtBgt.CreateBudgetLines(this, 0);

            /****Rock Budget ****/
            var rockBudgetNo = string.Format(AppCultureInfo.CInfo(), "Rock-{0}", BidId);
            var rockBgt = Job.AddBudget(this, rockBudgetNo);
            rockBgt.CreateBudgetLines(this, (int)GroundDensityId);
        }

        public void CreateJob()
        {
            var boreCnt = 1;
            var description = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", Package.Project.Description, Description);
            if (Job?.JobType == DB.JCJobTypeEnum.Template)
            {
                Job = null;
            }
            if (Job?.Description != null && Job?.JobType != DB.JCJobTypeEnum.Template)
            {
                description = Job?.Description;
            }
            if (description.Length >= 60)
            {
                description = description.Substring(0, 59);
            }
            if (Job == null)
            {
                var jobId = string.Format(AppCultureInfo.CInfo(), "{0}{1}", Package.Project.JobId, boreCnt.ToString(AppCultureInfo.CInfo()).PadLeft(2, '0'));
                while (Package.Project.SubJobs.Any(f => f.JobId == jobId))
                {
                    boreCnt++;
                    jobId = string.Format(AppCultureInfo.CInfo(), "{0}{1}", Package.Project.JobId, boreCnt.ToString(AppCultureInfo.CInfo()).PadLeft(2, '0'));
                }

                Job = new Job
                {
                    JCCo = (byte)Division.WPDivision.HQCompany.JCCo,
                    JobId = jobId,
                    Description = description,
                    JobStatus = 1,
                    Status = DB.JCJobStatusEnum.Open,
                    //VendorGroup = bore.Co,
                    DivisionDesc = Division.Description,
                    DivisionId = (byte)DivisionId,
                    Owner = Bid.Firm.FirmName.Length >= 30 ? Bid.Firm.FirmName.Substring(0, 29) : Bid.Firm.FirmName,
                    OwnerId = Bid.FirmId,
                    Footage = (short)Footage,
                    PipeSize = (byte)PipeSize,
                    JCCompanyParm = Division.WPDivision.HQCompany.JCCompanyParm,
                    JobTypeId = "1",
                    RigId = RigId,
                    CrewId = CrewId,
                    ProjectMgr = Bid.ProjectMangerId,
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
            Job.BidBoreLines.Add(this);
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
                    item.Description = string.Format(AppCultureInfo.CInfo(), "{0} {1:F0} Inch", BoreType.Description, PipeSize);
                }
                if (item.Item.Contains("2"))
                {
                    item.OrigContractUnits = (CurFootage ?? (Footage ?? 0));
                    item.OrigUnitPrice = Package.ParentPackageId == null ? (Package.DirtPrice ?? 0) : (Package.ParentPackage.DirtPrice ?? 0);
                    item.OrigUnitPrice = (rockPrice - dirtPrice);
                    item.Description = string.Format(AppCultureInfo.CInfo(), "Rock Adder {0:F0} Inch", PipeSize);
                }
            }
        }

        public void UpdateLaborBudget()
        {
            if (CrewCount != null && RigCategoryId != null)
            {
                var costItems = CostItems.Where(f => f.BudgetCodeId != null).Where(f => f.BudgetCodeId.Contains("LB-") && f.IsPackageCost != true).ToList();

                var rigCat = RigCategory;
                if (rigCat == null)
                    UpdateRigCategory(tRigCategoryId);


                #region update labor budget
                var laborDesc = string.Format(AppCultureInfo.CInfo(), "Labor Per Man - {0}", rigCat.Description);
                var stdBgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", "Labor Per Man");
                var bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", laborDesc);

                foreach (var costItem in costItems.Where(f => f.BudgetCodeId != bgt.BudgetCodeId && f.BudgetCodeId != "LB-0099").ToList())
                {
                    CostItems.Remove(costItem);
                }
                if (bgt.CostTypeId == null)
                {
                    //bgt = db.ProjectBudgetCodes.Where(f => f.PMCo == bgt.PMCo && f.BudgetCodeId == bgt.BudgetCodeId).FirstOrDefault();
                    bgt.UM = stdBgt.UM;
                    bgt.UnitCost = stdBgt.UnitCost;
                    bgt.CostTypeId = stdBgt.CostTypeId;
                    bgt.CostLevel = stdBgt.CostLevel;

                    foreach (var phase in stdBgt.Phases)
                    {
                        var bgtPhase = new ProjectBudgetCodeCalPhase
                        {
                            PMCo = phase.PMCo,
                            BudgetCodeId = bgt.BudgetCodeId,
                            ActiveYN = "Y",
                            PhaseId = phase.PhaseId,

                        };
                        bgt.Phases.Add(bgtPhase);
                    }
                    //db.SaveChanges(modelState);
                }

                Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, (int)this.CrewCount);

                foreach (var costObj in CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList())
                {
                    var maxShift = 1;
                    if (this.Passes.Any())
                        maxShift = this.Passes.Max(max => (int?)max.ShiftCnt ?? 1);

                    costObj.ShiftCnt = maxShift;
                    costObj.Multiplier = (CrewCount * (costObj.ShiftCnt ?? 1));
                }

                #endregion update labor budget

                #region update labor budget
                laborDesc = string.Format(AppCultureInfo.CInfo(), "Labor Indirect - {0}", rigCat.Description);
                //stdBgt = bgtRepo.FindCreate(model.Co, "LB", "Labor Indirect");
                //bgt = bgtRepo.FindCreate(model.Co, "LB", laborDesc);

                stdBgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", "Labor Indirect");
                bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", laborDesc);
                if (bgt.CostTypeId == null)
                {
                    //bgt = db.ProjectBudgetCodes.Where(f => f.PMCo == bgt.PMCo && f.BudgetCodeId == bgt.BudgetCodeId).FirstOrDefault();
                    bgt.UM = stdBgt.UM;
                    bgt.UnitCost = stdBgt.UnitCost;
                    bgt.CostTypeId = stdBgt.CostTypeId;
                    bgt.CostLevel = stdBgt.CostLevel;

                    foreach (var phase in stdBgt.Phases)
                    {
                        var bgtPhase = new ProjectBudgetCodeCalPhase
                        {
                            PMCo = phase.PMCo,
                            BudgetCodeId = bgt.BudgetCodeId,
                            ActiveYN = "Y",
                            PhaseId = phase.PhaseId
                        };
                        bgt.Phases.Add(bgtPhase);
                    }
                    //db.SaveChanges(modelState);
                }
                Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, (int)CrewCount);
                #endregion update labor budget
                #region update Labor Prevailing Wage budget

                if (Bid.BidId == 2052)
                {
                    laborDesc = string.Format(AppCultureInfo.CInfo(), "Labor Prevailing Wage");
                    stdBgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", "Labor Prevailing Wage");
                    bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "LB", laborDesc);
                    if (bgt.CostTypeId == null)
                    {
                        //bgt = db.ProjectBudgetCodes.Where(f => f.PMCo == bgt.PMCo && f.BudgetCodeId == bgt.BudgetCodeId).FirstOrDefault();
                        bgt.UM = stdBgt.UM;
                        bgt.UnitCost = stdBgt.UnitCost;
                        bgt.CostTypeId = stdBgt.CostTypeId;
                        bgt.CostLevel = stdBgt.CostLevel;

                        foreach (var phase in stdBgt.Phases)
                        {
                            var bgtPhase = new ProjectBudgetCodeCalPhase
                            {
                                PMCo = phase.PMCo,
                                BudgetCodeId = bgt.BudgetCodeId,
                                ActiveYN = "Y",
                                PhaseId = phase.PhaseId
                            };
                            bgt.Phases.Add(bgtPhase);
                        }
                        //db.SaveChanges(modelState);
                    }
                    Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, (int)CrewCount);
                }
                #endregion update labor budget
                //var costItems = db.BidBoreLineCostItems.Where(f => f.Co == model.Co && f.BidId == model.BidId && f.BoreId == model.BoreId && f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList();
                costItems = CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList();
                foreach (var costObj in costItems)
                {
                    var maxShift = 1;
                    if (this.Passes.Any())
                        maxShift = this.Passes.Max(max => (int?)max.ShiftCnt ?? 1);
                    costObj.ShiftCnt = maxShift;
                    costObj.Multiplier = (CrewCount * (costObj.ShiftCnt ?? 1));
                }
            }
        }

        public void UpdateEquipmentBudget()
        {
            using var db = new VPEntities();

            if (RigCategoryId == null)
            {
                return;
            }
            //var rigCat = model.EMCategory;
            //if (rigCat == null)
            //    rigCat = db.EquipmentCategories.FirstOrDefault(f => f.EMCo == model.Co && f.CategoryId == model.RigCategoryId);
            var costItems = CostItems.Where(f => f.BudgetCodeId.Contains("EQ-") && f.IsPackageCost != true).ToList();

            var cat = RigCategory;
            var laborDesc = string.Format(AppCultureInfo.CInfo(), "{0}", cat.Description);
            var stdBgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "EQ", "Rig Equipment");
            var bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "EQ", laborDesc);


            foreach (var costItem in costItems.Where(f => f.BudgetCodeId != bgt.BudgetCodeId).ToList())
            {
                CostItems.Remove(costItem);
            }

            if (bgt.CostTypeId == null || bgt.UnitCost == 0)
            {
                bgt = db.ProjectBudgetCodes.Where(f => f.PMCo == bgt.PMCo && f.BudgetCodeId == bgt.BudgetCodeId).FirstOrDefault();
                bgt.UM = stdBgt.UM;
                bgt.UnitCost = cat.CostRates.Where(f => f.UM == "DYS").FirstOrDefault()?.Rate ?? 99999;
                bgt.CostTypeId = stdBgt.CostTypeId;
                bgt.CostLevel = stdBgt.CostLevel;

                foreach (var phase in stdBgt.Phases)
                {
                    var bgtPhase = new ProjectBudgetCodeCalPhase
                    {
                        PMCo = phase.PMCo,
                        BudgetCodeId = bgt.BudgetCodeId,
                        ActiveYN = "Y",
                        PhaseId = phase.PhaseId
                    };
                    bgt.Phases.Add(bgtPhase);
                }
                //db.SaveChanges(modelState);
            }
            Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, 1);
            foreach (var costObj in CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList())
            {
                costObj.Cost = bgt.UnitCost;
                costObj.Multiplier = (costObj.Multiplier < 1 ? 1 : costObj.Multiplier);
            }
        }

        public void UpdatePullHeadBudget(decimal? newPipeSize)
        {
            #region update Pull Head
            var bgt = newPipeSize == null ? null : Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "MT", string.Format(AppCultureInfo.CInfo(), "Pull Head {0} inch", Math.Round((decimal)newPipeSize, 0)));
            var oldbgt = PipeSize == null ? null : Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "MT", string.Format(AppCultureInfo.CInfo(), "Pull Head {0} inch", Math.Round((decimal)PipeSize, 0)));

            if (bgt != null)
            {
                Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, 1);
                foreach (var costObj in CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList())
                {
                    costObj.Multiplier = 1;
                    costObj.Units = 1;
                    costObj.CurUnits = 1;
                    costObj.Cost = bgt.UnitCost;
                }
            }

            if (oldbgt != null && PipeSize != newPipeSize)
            {
                foreach (var costObj in CostItems.Where(f => f.BudgetCodeId == oldbgt.BudgetCodeId && f.IsPackageCost != true).ToList())
                {
                    CostItems.Remove(costObj);
                }
            }
            #endregion update Pull Head
        }

        public void UpdateMudBudget()
        {
            #region update Mud budget

            var bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "MT", "Drilling mud");
            Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, 0);

            foreach (var costObj in CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost != true).ToList())
            {
                costObj.Multiplier = decimal.ToInt32(PipeSize ?? 0);
                if (Bid.Status <= DB.BidStatusEnum.ContractApproval)
                {
                    costObj.Units = Footage;
                    costObj.CurUnits = Footage;
                }
                else
                {
                    costObj.Units = Footage;
                    costObj.CurUnits = CurFootage ?? Footage;
                }
                costObj.Cost = bgt.UnitCost;
            }
            //db.SaveChanges(modelState);
            #endregion update labor budget
        }

        public void UpdateDirtToolingBudget()
        {

            #region update dirt tooling budget
            var bgt = Repository.VP.PM.ProjectBudgetCodeRepository.FindCreate(Bid.Company, BDCo, "TM", "Dirt Tooling");

            Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgt.BudgetCodeId, 0);
            var costItems = CostItems.Where(f => f.BudgetCodeId.Contains("TM-") && f.IsPackageCost != true).ToList();
            foreach (var costObj in costItems)
            {
                var footage = Footage;
                var intersect = Bid.BoreLines.FirstOrDefault(f => f.IntersectBoreId == BoreId);
                if (intersect != null)
                {
                    var costIntersect = intersect.CostItems.FirstOrDefault(w => w.BudgetCodeId == costObj.BudgetCodeId && w.GroundDensityId == costObj.GroundDensityId);
                    if (costIntersect != null)
                    {
                        footage -= intersect.Footage * costIntersect.Multiplier;
                    }
                }
                if (bgt.BudgetCodeId == costObj.BudgetCodeId)
                {
                    costObj.Multiplier = costObj.GroundDensityId != 0 ? 0 : 1;
                    if (Bid.Status <= DB.BidStatusEnum.ContractApproval)
                    {
                        costObj.Units = costObj.GroundDensityId != 0 ? 0 : footage;
                        costObj.CurUnits = costObj.GroundDensityId != 0 ? 0 : footage;
                    }
                    else
                    {
                        costObj.Units = costObj.GroundDensityId != 0 ? 0 : footage;
                        costObj.CurUnits = costObj.GroundDensityId != 0 ? 0 : costObj.BoreLine?.CurFootage ?? footage;
                    }
                    costObj.Cost = costObj.GroundDensityId != 0 ? 0 : bgt.UnitCost;
                }
                else
                {
                    if (Bid.Status <= DB.BidStatusEnum.ContractApproval)
                    {
                        costObj.Units = footage;
                        costObj.CurUnits = footage;
                    }
                    else
                    {
                        costObj.Units = footage;
                        costObj.CurUnits = costObj.BoreLine.CurFootage ?? footage;
                    }
                    costObj.Multiplier = 1;
                }
            }
            #endregion update dirt tooling budget


        }

        public void RecalculateCostUnits()
        {
            AddMissingCostItems();
            var costItems = CostItems.Where(f => f.IsPackageCost != true).ToList();
            costItems = costItems.Where(f => (f.BudgetCodeId.StartsWith("RE-", StringComparison.Ordinal) ||
                                                f.BudgetCodeId.StartsWith("CI-", StringComparison.Ordinal) ||
                                                f.BudgetCodeId.StartsWith("LB-", StringComparison.Ordinal) ||
                                                f.BudgetCodeId.StartsWith("EQ-", StringComparison.Ordinal) ||
                                                f.BudgetCodeId.StartsWith("TM-", StringComparison.Ordinal) ||
                                                f.BudgetCodeId.StartsWith("MT-", StringComparison.Ordinal))
                                                ).ToList();

            foreach (var costItem in costItems)
            {
                costItem.ReCalulateCostItem();
            }

            RecalcNeeded = false;
        }

        public void AddMissingCostItems()
        {
            var bgtCodeItems = CostItems
                .Where(f => f.IsPackageCost != true)
                .GroupBy(g => g.BudgetCodeId)
                .Select(s => new
                {
                    BudgetCodeId = s.Key,
                    Multipler = s.DefaultIfEmpty().Max(max => max == null ? 1 : max.Multiplier ?? 0),
                    CostItems = s.Where(f => f.GroundDensityId == 0 || f.GroundDensityId == GroundDensityId).ToList()
                })
                .ToList();
            foreach (var bgtItem in bgtCodeItems)
            {
                Repository.VP.BD.BidBoreLineCostItemRepository.CreateCostItems(this, bgtItem.BudgetCodeId, bgtItem.Multipler);
                bgtItem.CostItems.ForEach(e => {
                    if (e.Multiplier != bgtItem.Multipler)
                    {
                        e.Multiplier = bgtItem.Multipler;
                    }
                });
            }
        }

        public void ApplyPackageProductionDefaults()
        {
            //var db = VPEntities.GetDbContextFromEntity(this);;
            if (db != null)
            {
                db.udBDBL_DefaultProductionRateForLineInsert(BDCo, BidId, BoreId);
            }
        }

        //ScheduleViewModel

        public void UpdateFromModel(Models.Views.Bid.Forms.Bore.Schedule.ScheduleViewModel model)
        {
            if (model == null)
                return;

            RigCategoryId = model.RigCategoryId;
            Description = model.Description;
            RigId = model.RigId;
            StartDate = model.StartDate;
        }
    }
}
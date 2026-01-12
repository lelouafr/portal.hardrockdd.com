using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class BidPackage
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
                _db ??= this.Bid.db;
                _db ??= VPEntities.GetDbContextFromEntity(this);
                return _db;
            }
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
                }
            }
        }

        public List<tfudBDBI_SUMMARY_Result> fvBoreLineCostItems()
        {
            return db.tfudBDBI_SUMMARY(this.BDCo, this.BidId, this.PackageId, null).ToList();
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
                UpdateBoreType(value);
            }
        }

        public void UpdateBoreType(int? value)
        {
            ////var db = VPEntities.GetDbContextFromEntity(this);;


            if (tBoreTypeId != value)
            {
                tBoreTypeId = value;
                BoreLines.ToList().ForEach(bore =>
                {
                    bore.BoreTypeId = value;
                });
            }
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
            value ??= Bid.DivisionId;
            ////var db = VPEntities.GetDbContextFromEntity(this);;
            var project = this.Project;
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

            if (project != null && Division != null)
            {
                if (project?.JCCo != JCCo && project?.JobType == DB.JCJobTypeEnum.Template)
                {
                    Project = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob;
                    tJobId = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob.JobId;
                }
            }

            BoreLines.ToList().ForEach(bore =>
            {
                bore.DivisionId = value;
                //if (bore.JCCo != JCCo)
                //    bore.JCCo = JCCo;

                //if (bore.DivisionId != value)
                //{
                //    bore.Division = Division;
                //}

            });
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
            ////var db = VPEntities.GetDbContextFromEntity(this);;

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

            BoreLines.ToList().ForEach(bore =>
            {
                bore.Customer = Customer;
                bore.JCCo = JCCo;
                bore.CustomerId = value;

            });
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
            UpdateDivision(DivisionId);
            ////var db = VPEntities.GetDbContextFromEntity(this);;

            if (Industry == null && tIndustryId != null)
            {
                var industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == tIndustryId && f.JCCo == (JCCo ?? BDCo));
                if (industry != null)
                {
                    industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == industry.IndustryId && f.JCCo == Division.WPDivision.HQCompany.JCCo);
                    Industry = industry;
                    JCCo = industry.JCCo;
                    tIndustryId = industry.IndustryId;
                }
                else
                {
                    Industry = null;
                    JCCo = null;
                    tIndustryId = null;

                }
            }
            if (tIndustryId != value)
            {
                var industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == value && f.JCCo == (JCCo ?? BDCo));
                if (industry != null)
                {
                    if (Division != null)
                        industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == industry.IndustryId && f.JCCo == Division.WPDivision.HQCompany.JCCo);
                    Industry = industry;
                    JCCo = industry.JCCo;
                    tIndustryId = industry.IndustryId;
                }
                else
                {
                    Industry = null;
                    JCCo = null;
                    tIndustryId = null;
                }
                tMarketId = null;
                BoreLines.ToList().ForEach(bore =>
                {
                    bore.JCCo = JCCo;
                    bore.IndustryId = value;
                    bore.tMarketId = null;
                });
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
            ////var db = VPEntities.GetDbContextFromEntity(this);;
            UpdateIndustry(tIndustryId);

            if (Market == null && tMarketId != null)
            {
                var market = db.JCMarkets.FirstOrDefault(f => f.MarketId == tMarketId && f.JCCo == (JCCo ?? Division.WPDivision.HQCompany.JCCo));
                if (market != null)
                {
                    Market = market;
                    JCCo = market.JCCo;
                    tMarketId = market.MarketId;
                }
                else
                {
                    Market = null;
                    JCCo = null;
                    tMarketId = null;

                }
            }
            if (tMarketId != value)
            {
                var market = db.JCMarkets.FirstOrDefault(f => f.MarketId == value && f.JCCo == (JCCo ?? Division.WPDivision.HQCompany.JCCo));
                if (market != null)
                {
                    Market = market;
                    JCCo = market.JCCo;
                    tMarketId = market.MarketId;
                }
                else
                {
                    Market = null;
                    JCCo = null;
                    tMarketId = null;
                }
                BoreLines.ToList().ForEach(bore =>
                {
                    if (bore.JCCo != JCCo)
                        bore.JCCo = JCCo;
                    if (bore.MarketId != value)
                        bore.MarketId = value;
                });
            }
        }

        public int? NumberOfBores
        {
            get
            {
                return tNumberOfBores;
            }
            set
            {
                if (tNumberOfBores != value)
                {
                    tNumberOfBores = value;
                    GenerateBoreLines();
                }
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

            if (tPipeSize != value)
            {
                tPipeSize = value;

            }

            BoreLines.ToList().ForEach(bore =>
            {
                bore.tPipeSize = value;
                bore.RecalcNeeded = true;
            });
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
            }
        }

        public void UpdateRigCategory(string value)
        {
            ////var db = VPEntities.GetDbContextFromEntity(this);;

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
                    tRigCategoryId = category.CategoryId;
                }
                else
                {
                    RigCategory = null;
                    //EMCo = null;
                    tRigCategoryId = null;
                }
                BoreLines.ToList().ForEach(bore =>
                {
                    bore.RigCategory = RigCategory;
                    //bore.EMCo = EMCo;
                    bore.RigCategoryId = value;

                });
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
                var groundDensity = db.GroundDensities.FirstOrDefault(f => f.GroundDensityId == value);
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

            BoreLines.ToList().ForEach(bore =>
            {
                bore.GroundDensity = GroundDensity;
                bore.GroundDensityId = value;
            });
        }

        public string CustomerReference
        {
            get
            {
                return tCustomerReference;
            }
            set
            {
                UpdateCustomerReference(value);
            }
        }

        public void UpdateCustomerReference(string value)
        {

            if (tCustomerReference != value)
            {
                tCustomerReference = value;

            }

            BoreLines.ToList().ForEach(bore =>
            {
                bore.tCustomerReference = value;
            });
        }

        public string JobId
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

        public void UpdateJob(string? value)
        {
            //var db = VPEntities.GetDbContextFromEntity(this);;

            if (Project == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                if (job != null)
                {
                    Project = job;
                    JCCo = job.JCCo;
                    tJobId = job.JobId;
                }
                else
                {
                    Project = null;
                    JCCo = null;
                    tJobId = null;
                }
            }
            if (tJobId != value)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == value);
                if (job != null)
                {
                    Project = job;
                    JCCo = job.JCCo;
                    tJobId = job.JobId;
                }
                else
                {
                    Project = null;
                    JCCo = null;
                    tJobId = null;
                }
            }

            BoreLines.ToList().ForEach(bore =>
            {

                bore.JCCo = JCCo;
                if (Project?.JobType == DB.JCJobTypeEnum.Template)
                {
                    bore.JobId = value;
                }

            });
        }

        public DateTime? StartDate
        {
            get
            {
                return tStartDate;
            }
            set
            {
                if (tStartDate != value)
                {
                    tStartDate = value;
                    foreach (var boreLine in BoreLines)
                    {
                        boreLine.StartDate = tStartDate;
                    }
                }
            }
        }
        public List<BidBoreLine> ActiveBoreLines
        {
            get
            {
                return BoreLines.Where(f => f.Status != DB.BidStatusEnum.Deleted && f.Status != DB.BidStatusEnum.Canceled).ToList();
            }
        }

        public List<BidBoreLine> AwardedBoreLines
        {
            get
            {
                return ActiveBoreLines.Where(f => f.AwardStatus == DB.BidAwardStatusEnum.Awarded).ToList();
            }
        }

        public List<BidBoreLine> PendingBoreLines
        {
            get
            {
                return ActiveBoreLines.Where(f => f.AwardStatus == DB.BidAwardStatusEnum.Pending).ToList();
            }
        }

        public decimal TotalFootage
        {
            get
            {
                var result = ActiveBoreLines.Where(f => f.IntersectBoreId == null).Sum(s => s.Footage ?? 0);

                foreach (var subPackage in SubPackages)
                {
                    var totalSubFootage = subPackage.TotalFootage;
                    result += totalSubFootage;
                }
                return result;
            }
        }

        public void CreateProject()
        {
            //var db = VPEntities.GetDbContextFromEntity(this);;
            UpdateDivision(DivisionId);
            CreateJob();
            CreateJobContract();
            Project.Contract.SyncTemplatedContractItems();
            Project.SyncMasterPhase();

            var boreLines = AwardedBoreLines;
            foreach (var bore in boreLines)
            {
                bore.UpdateDivision(bore.DivisionId);
                bore.CreateJob();
                bore.CreateJobContract();
                bore.Job.Contract.SyncTemplatedContractItems();
                bore.Job.SyncMasterPhase();
                bore.UpdateContractItems();
                bore.CreateProjectBudget();
            }

            foreach (var bore in AwardedBoreLines)
            {
                if (bore.IntersectBoreId != null)
                {
                    var intersectJob = AwardedBoreLines.FirstOrDefault(f => f.BoreId == bore.IntersectBoreId);
                    bore.Job.IntersectJobId = intersectJob.Job.JobId;
                }
            }
        }

        public void CreateJob()
        {
            if (Customer == null)
            {
                //Customer = CustomerId
            }
            var custName = Customer.Name.Split(' ').First();
            var description = string.Format(AppCultureInfo.CInfo(), "{0} - {1} {2}", custName, Bid.Firm.FirmName, Description);
            if (Project?.JobType == DB.JCJobTypeEnum.Template)
            {
                Project = null;
            }
            if (Project?.Description != null && JobId != "00-0000-00")
            {
                description = Project?.Description;
            }
            if (description.Length >= 60)
            {
                description = description.Substring(0, 59);
            }
            if (Project == null)
            {
                var jobId = Repository.VP.JC.JobRepository.GetNextParentJobId(Division, Bid);

                var newProject = new Job
                {
                    JCCo = (byte)Division.WPDivision.HQCompany.JCCo,
                    JobId = jobId,
                    Description = description,
                    JobStatus = 2,
                    Status = DB.JCJobStatusEnum.Open,
                    //VendorGroup = package.Co,
                    DivisionDesc = Division.Description,
                    DivisionId = (byte)DivisionId,
                    Owner = Bid.Firm.FirmName.Length >= 30 ? Bid.Firm.FirmName.Substring(0, 29) : Bid.Firm.FirmName,
                    OwnerId = Bid.FirmId,
                    JCCompanyParm = Division.WPDivision.HQCompany.JCCompanyParm,
                    JobTypeId = "7",
                    RigId = RigId,
                    CrewId = CrewId,
                    ProjectMgr = Bid.ProjectMangerId,

                    PRCo = Division.WPDivision.HQCompany.PRCo,
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

                Project = newProject;
            }
            JCCo = Project.JCCo;
            tJobId = Project.JobId;
            Project.BidNumber = BidId.ToString();
            //throw new Exception();
        }

        public void CreateJobContract()
        {
            if (Project == null)
                return;
            if (Project.Contract?.ContractId == "00-0000-00")
            {
                Project.Contract = null;
            }

            if (Project.Contract == null)
            {
                Project.Contract = new JCContract
                {
                    JCCo = (byte)Division.WPDivision.HQCompany.JCCo,
                    ContractId = Project.JobId,
                    Description = Project.Description,
                    ContractStatus = 2,
                    Customer = Customer,
                    CustGroupId = Customer.CustGroupId,
                    CustomerId = CustomerId,
                    StartDate = StartDate ?? Bid.StartDate,

                    DepartmentId = "1",
                    OriginalDays = 0,
                    CurrentDays = 0,
                    TaxInterface = "N",
                    TaxGroup = Project.JCCompanyParm.HQCompanyParm.TaxGroupId,
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
                Project.ContractId = Project.Contract.ContractId;
                Project.Contract.JCCompanyParm = Project.JCCompanyParm;
                Project.Contract.StartMonth = new DateTime(Project.Contract.StartDate.Value.Year, Project.Contract.StartDate.Value.Month, 1);
            }
        }

        public void GenerateBoreLines()
        {
            var boreLines = ActiveBoreLines.Where(f => f.IntersectBoreId == null).ToList();
            var actCount = boreLines.Count();
            if (actCount < NumberOfBores)
            {

                for (int i = actCount; i < NumberOfBores; i++)
                {
                    AddBoreLine();
                }
            }

            else if (actCount > NumberOfBores)
            {
                //using var boreRepo = new BidBoreLineRepository();
                var borelist = boreLines.Skip((int)NumberOfBores).ToList();
                foreach (var bore in borelist)
                {
                    bore.Status = DB.BidStatusEnum.Deleted;
                }
            }

            if (NumberOfBores >= 20)
            {
                var cnt = 1;
                foreach (var bore in boreLines)//.Where(f => f.Description == null).ToList()
                {
                    if (bore.Description == null)
                        bore.Description = string.Format(AppCultureInfo.CInfo(), "Bore {0}", cnt);

                    cnt++;
                }
            }

            tNumberOfBores = ActiveBoreLines.Count;
        }

        public BidBoreLine AddBoreLine()
        {
            var bore = new BidBoreLine
            {
                BDCo = BDCo,
                BidId = BidId,
                BoreId = Bid.BoreLines.DefaultIfEmpty().Max(f => f == null ? 0 : f.BoreId) + 1,
                PackageId = PackageId,
                tDivisionId = DivisionId,
                tIndustryId = IndustryId,
                tMarketId = MarketId,
                Description = null,
                tBoreTypeId = BoreTypeId,
                tRigCategoryId = RigCategoryId,
                tGroundDensityId = GroundDensityId,
                tCrewCount = RigCategory?.CrewSize,
                tPipeSize = PipeSize,
                tCustomerId = CustomerId,
                tCustomerReference = CustomerReference,
                tStatusId = Bid.StatusId,
                RecalcNeeded = true,

                Bid = Bid,
                Package = this,
                RigCategory = RigCategory,

            };
            if (Bid.BidType == (int)DB.BidTypeEnum.QuickBid)
            {
                bore.tStatusId = (int)DB.BidAwardStatusEnum.Awarded;
            }

            bore.BoreId = Bid.BoreLines.DefaultIfEmpty().Max(f => f == null ? 0 : f.BoreId) + 1;
            BoreLines.Add(bore);
            Bid.BoreLines.Add(bore);
            return bore;
        }

        public void ApplyPackageCost()
        {
            if (!PackageCosts.Any(f => f.CostAllocationTypeId != null))
            {
                return;
            }

            var boreLines = ActiveBoreLines.OrderBy(o => o.BoreId).ToList();
            var totFootage = boreLines.Sum(s => s.Footage);

            boreLines.SelectMany(s => s.CostItems.Where(f => f.IsPackageCost == true)).ToList().ForEach(f => f.Cost = 0);
            if (!PackageCosts.Any(f => f.BudgetCodeId != null))
            {
                return;
            }
            var costList = PackageCosts.Where(f => f.CostAllocationTypeId != null && f.BudgetCodeId != null);
            foreach (var allocatedCost in costList)
            {
                BidBoreLineCostItem costItem;
                BidBoreLine bore;
                switch ((DB.BDPackageCostAllocationType)allocatedCost.CostAllocationTypeId)
                {
                    case DB.BDPackageCostAllocationType.FirstBore:
                        bore = boreLines.FirstOrDefault();
                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
                        if (costItem == null)
                        {

                            costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, bore, 0);
                            bore.CostItems.Add(costItem);
                        }
                        costItem.Cost = allocatedCost.Cost;
                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)bore.GroundDensityId);
                        if (costItem == null)
                        {
                            costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, bore, (int)bore.GroundDensityId);
                            bore.CostItems.Add(costItem);
                        }
                        costItem.Cost = allocatedCost.Cost;
                        break;
                    case DB.BDPackageCostAllocationType.LastBore:
                        bore = boreLines.LastOrDefault();
                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
                        if (costItem == null)
                        {
                            costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, bore, 0);
                            bore.CostItems.Add(costItem);
                        }
                        costItem.Cost = allocatedCost.Cost;
                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)bore.GroundDensityId);
                        if (costItem == null)
                        {
                            costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, bore, (int)bore.GroundDensityId);
                            bore.CostItems.Add(costItem);
                        }
                        costItem.Cost = allocatedCost.Cost;
                        break;
                    case DB.BDPackageCostAllocationType.AllBoreLF:
                        foreach (var boreLine in boreLines)
                        {
                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
                            if (costItem == null)
                            {
                                costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, 0);
                                boreLine.CostItems.Add(costItem);
                            }
                            costItem.Cost = allocatedCost.Cost / totFootage * boreLine.Footage;

                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)boreLine.GroundDensityId);
                            if (costItem == null)
                            {
                                costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, (int)boreLine.GroundDensityId);
                                boreLine.CostItems.Add(costItem);
                            }
                            costItem.Cost = allocatedCost.Cost / totFootage * boreLine.Footage;
                        }
                        break;
                    case DB.BDPackageCostAllocationType.AllBoreDay:
                        var boreListViewModel = new Models.Views.Bid.Forms.Bore.DetailListViewModel(this);
                        var totalDirtDays = boreListViewModel.List.Sum(f => f.DirtDays);
                        var totalRockDays = boreListViewModel.List.Sum(f => f.RockDays);
                        foreach (var boreLine in boreLines)
                        {
                            var bidViewModel = boreListViewModel.List.FirstOrDefault(f => f.BoreId == boreLine.BoreId);

                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
                            if (costItem == null)
                            {
                                costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, 0);
                                boreLine.CostItems.Add(costItem);
                            }
                            costItem.Cost = allocatedCost.Cost / totalDirtDays * bidViewModel.DirtDays;

                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)boreLine.GroundDensityId);
                            if (costItem == null)
                            {
                                costItem = Repository.VP.BD.BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, (int)boreLine.GroundDensityId);
                                boreLine.CostItems.Add(costItem);
                            }
                            costItem.Cost = allocatedCost.Cost / totalRockDays * bidViewModel.RockDays;
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        public BidPackageScope AddScope(DB.ScopeTypeEnum scopeType)
        {
            var result = new BidPackageScope
            {
                BDCo = BDCo,
                BidId = BidId,
                PackageId = PackageId,
                LineId = Scopes.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                ScopeTypeId = (byte)scopeType,
                Notes = string.Empty,
                Title = string.Empty
            };
            Scopes.Add(result);
            return result;
        }

        public BidPackageCost AddPackageCost()
        {
            var cost = new BidPackageCost
            {
                BDCo = BDCo,
                BidId = BidId,
                PackageId = PackageId,
                LineId = PackageCosts.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                Package = this,
            };
            PackageCosts.Add(cost);
            return cost;
        }

        public void UpdateFromModel(Models.Views.Bid.Forms.Package.Schedule.ScheduleViewModel model)
        {
            if (model == null)
                return;

            Description = model.Description;
            NumberOfBores = model.NumberOfBores;
            RigCategoryId = model.RigCategoryId;
            StartDate = model.StartDate;
        }
    }
}
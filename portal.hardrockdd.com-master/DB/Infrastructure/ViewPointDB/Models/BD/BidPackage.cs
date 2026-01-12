using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class BidPackage
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
                _db ??= this.Bid.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
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

        private List<tfudBDBI_SUMMARY_Result> _fvBoreLineCostItems;
        public List<tfudBDBI_SUMMARY_Result> fvBoreLineCostItems()
        {
            if (_fvBoreLineCostItems == null)
                _fvBoreLineCostItems = db.tfudBDBI_SUMMARY(this.BDCo, this.BidId, this.PackageId, null).ToList();

            return _fvBoreLineCostItems;
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
                if ((int)value != StatusId)
                {
                    StatusId = (int)value;
                    ActiveBoreLines.ForEach(e => e.AwardStatus = value);
                }
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
            ////var db = VPContext.GetDbContextFromEntity(this);;


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
            ////var db = VPContext.GetDbContextFromEntity(this);;
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


                BoreLines.ToList().ForEach(bore =>
                {
                    bore.DivisionId = value;
                });
            }

            if (project != null && Division != null)
            {
                if (project?.JCCo != JCCo && project?.JobType == JCJobTypeEnum.Template)
                {
                    Project = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob;
                    tJobId = Division.WPDivision.HQCompany.JCCompanyParm.TemplateJob.JobId;
                }
            }
        }

		public string Description { get => tDescription; set => tDescription = value?.Trim(); }

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
            ////var db = VPContext.GetDbContextFromEntity(this);;

            if (Customer == null && tCustomerId != null)
            {
                var customer = db.Customers.FirstOrDefault(f => f.CustomerId == tCustomerId);
                if (customer != null)
                {
                    Customer = customer;
                    CustGroupId = customer.CustGroupId;
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
                    CustGroupId = customer.CustGroupId;
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
            ////var db = VPContext.GetDbContextFromEntity(this);;

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
            ////var db = VPContext.GetDbContextFromEntity(this);;
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

                ProductionRates.ToList().ForEach(rate => {
                    rate.PipeSize = value;
                });

                BoreLines.ToList().ForEach(bore =>
                {
                    bore.PipeSize = value;
                    bore.RecalcNeeded = true;
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
            }
        }

        public void UpdateRigCategory(string value)
        {
            ////var db = VPContext.GetDbContextFromEntity(this);;

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
                    GroundDensity = groundDensity;
                    tGroundDensityId = groundDensity.GroundDensityId;

                    var rates = this.ProductionRates.Where(f => f.GroundDensityId != orgGroundDensityId && f.GroundDensityId != 0).ToList();
                    rates.ForEach(e => this.ProductionRates.Remove(e));

                    rates = this.ProductionRates.Where(f => f.GroundDensityId == orgGroundDensityId).ToList();
                    rates.ForEach(e => e.GroundDensityId = tGroundDensityId);
                    //foreach (var rate in rates)
                    //{
                    //    if (!this.ProductionRates.Any(f => f.GroundDensityId == tGroundDensityId && f.PhaseId == rate.PhaseId && f.PassId == rate.PassId && f.PipeSize == rate.PipeSize))
                    //    {
                    //        rate.GroundDensityId = this.GroundDensityId;
                    //    }
                    //    else
                    //    {
                    //        this.ProductionRates.Remove(rate);
                    //    }
                    //}
                }
                else
                {
                    GroundDensity = null;
                    tGroundDensityId = null;
                }

                BoreLines.ToList().ForEach(bore =>
                {
                    bore.GroundDensity = GroundDensity;
                    bore.GroundDensityId = value;
                });
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

        public void UpdateJob(string value)
        {
            //var db = VPContext.GetDbContextFromEntity(this);;

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
                if (Project?.JobType == JCJobTypeEnum.Template)
					bore.JobId = value;
			});
        }

        public DateTime? StartDate
        {
            get
            {
                return tStartDate ?? Bid.StartDate;
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
                return BoreLines.Where(f => f.Status != BidStatusEnum.Deleted && f.Status != BidStatusEnum.Canceled).ToList();
            }
        }

        public List<BidBoreLine> AwardedBoreLines
        {
            get
            {
                return ActiveBoreLines.Where(f => f.AwardStatus == BidAwardStatusEnum.Awarded).ToList();
            }
        }

        public List<BidBoreLine> PendingBoreLines
        {
            get
            {
                return ActiveBoreLines.Where(f => f.AwardStatus == BidAwardStatusEnum.Pending).ToList();
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
            //var db = VPContext.GetDbContextFromEntity(this);;
            UpdateDivision(DivisionId);
            CreateJob();
            Project.AddContract(Customer);            
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
            var company = Division.WPDivision.HQCompany;
            if (Customer == null)
            {
                //Customer = CustomerId
            }
            var custName = Customer.Name.Split(' ').First();
            var description = string.Format("{0} - {1} {2}", custName, Bid.Firm.FirmName, Description);
            if (Project?.JobType == JCJobTypeEnum.Template)
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
                var JCParm = company.JCCompanyParm;
                var jobId = JCParm.NextProjectId(Division,Bid);
                
                var newProject = new Job
                {
                    JCCo = JCParm.JCCo,
                    JobId = jobId,
                    Description = description,
                    JobStatus = 2,
                    Status = JCJobStatusEnum.Open,
                    //VendorGroup = package.Co,
                    DivisionDesc = Division.Description,
                    DivisionId = (byte)DivisionId,
                    Owner = Bid.Firm.FirmName.Length >= 30 ? Bid.Firm.FirmName.Substring(0, 29) : Bid.Firm.FirmName,
                    OwnerId = Bid.tFirmId,
                    JCCompanyParm = company.JCCompanyParm,
                    JobTypeId = "7",
                    RigId = RigId,
                    CrewId = CrewId,
                    ProjectMgr = Bid.ProjectMangerId ?? Division?.WPDivision?.ManagerId,
                    PRCo = company.PRCo,
                    EMCo = company.EMCo,
                    LockPhases = "N",
                    LiabTemplate = 1,
                    TaxGroup = (byte)company.TaxGroupId,
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
                    StartDate = this.StartDate,
                    Division = this.Division
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

        public void ApplyProductionDefaults(bool force)
        {
            this.db.udBDBL_DefaultProductionRateForPackageInsert(this.BDCo, this.BidId, this.PackageId, force ? 1 : 0);
            db.Entry(this).Reload();
        }

        public void GenerateBoreLines()
        {
            var boreLines = ActiveBoreLines.Where(f => f.IntersectBoreId == null).ToList();
            var actCount = boreLines.Count();
            if (actCount < NumberOfBores)
            {
                var cnt = NumberOfBores;
                for (int i = actCount; i < cnt; i++)
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
                    bore.Status = BidStatusEnum.Deleted;
                }
            }

            if (NumberOfBores >= 20)
            {
                var cnt = 1;
                decimal? footage = null;
                foreach (var bore in ActiveBoreLines)//.Where(f => f.Description == null).ToList()
                {
                    footage ??= bore.Footage;

                    if (string.IsNullOrEmpty(bore.Description))
                        bore.Description = string.Format("Bore {0}", cnt);
                    bore.Footage ??= footage;
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
            if (Bid.BidType == (int)BidTypeEnum.QuickBid)
            {
                bore.tStatusId = (int)BidAwardStatusEnum.Awarded;
            }

            bore.BoreId = Bid.BoreLines.DefaultIfEmpty().Max(f => f == null ? 0 : f.BoreId) + 1;
            BoreLines.Add(bore);
            Bid.BoreLines.Add(bore);
            //Bid.ActiveBoreLines.Add(bore);
            Bid.ActiveBoreLines = null;

            tNumberOfBores = this.ActiveBoreLines.Count;
            return bore;
        }
       
        public void Recalculate()
        {
            ApplyProductionDefaults(false);
            ActiveBoreLines.Where(f => !f.Passes.Any()).ToList().ForEach(e => {
                e.RecalcNeeded = true;
                e.ApplyPackageProductionDefaults();


                });
            if (ActiveBoreLines.Any(f => f.RecalcNeeded == true))
            {
                ApplyPackageCost();
                ActiveBoreLines.ForEach(e => e.Recalculate());
            }
        }

        public void ApplyPackageCost()
        {
            if (!PackageCosts.Any(f => f.CostAllocationTypeId != null))
            {
                return;
            }

            var boreLines = ActiveBoreLines.OrderBy(o => o.BoreId).ToList();
            var totFootage = boreLines.Sum(s => s.Footage);

            if (!PackageCosts.Any(f => f.BudgetCodeId != null))
            {
                return;
            }
            var costList = PackageCosts.Where(f => f.CostAllocationTypeId != null && f.BudgetCodeId != null);
            foreach (var allocatedCost in costList)
            {
                boreLines.SelectMany(s => s.CostItems.Where(f => f.IsPackageCost == true && f.BudgetCodeId == allocatedCost.BudgetCodeId))
                    .ToList()
                    .ForEach(f => { 
                        f.Cost = 0;
                        f.Multiplier = 0;
                        f.Units = 0;
                    });

                switch ((BDPackageCostAllocationType)allocatedCost.CostAllocationTypeId)
                {
                    case BDPackageCostAllocationType.FirstBore:
                        if (boreLines.FirstOrDefault() != null)
                        {
                            var bore = boreLines.FirstOrDefault();
                            var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == this.BDCo && f.BudgetCodeId == allocatedCost.BudgetCodeId);
                            var costItems = bore.AddCostItems(bgt);
                            costItems.ForEach(e => { e.Cost = allocatedCost.Cost; e.IsPackageCost = true; e.Units = 1; e.Multiplier = 1; });

                            var delList = boreLines.Where(f => f.BoreId != bore.BoreId).SelectMany(s => s.CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost == true)).ToList();
                            delList.ForEach(e => bore.CostItems.Remove(e)); 
                        }
                        break;
                    case BDPackageCostAllocationType.LastBore:                       
                        if (boreLines.LastOrDefault() != null)
                        {
                            var bore = boreLines.LastOrDefault();
                            var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == this.BDCo && f.BudgetCodeId == allocatedCost.BudgetCodeId);
                            var costItems = bore.AddCostItems(bgt);
                            costItems.ForEach(e => { e.Cost = allocatedCost.Cost; e.IsPackageCost = true; e.Units = 1; e.Multiplier = 1; });

                            var delList = boreLines.Where(f => f.BoreId != bore.BoreId).SelectMany(s => s.CostItems.Where(f => f.BudgetCodeId == bgt.BudgetCodeId && f.IsPackageCost == true)).ToList();
                            delList.ForEach(e => bore.CostItems.Remove(e));
                        }
                        break;
                    case BDPackageCostAllocationType.AllBoreLF:
                        foreach (var bore in boreLines)
                        {
                            var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == this.BDCo && f.BudgetCodeId == allocatedCost.BudgetCodeId);
                            var costItems = bore.AddCostItems(bgt);
                            var amt = allocatedCost.Cost / totFootage * bore.Footage;
                            costItems.ForEach(e => { e.Cost = amt; e.IsPackageCost = true; e.Units = 1; e.Multiplier = 1; });
                        }
                        break;
                    case BDPackageCostAllocationType.AllBoreDay:
                        var vBoreList = this.vBidBoreLines.ToList();
                        var totalDirtDays = vBoreList.Sum(f => f.DirtDays);
                        var totalRockDays = vBoreList.Sum(f => f.RockDays);
                        foreach (var bore in boreLines)
                        {
                            var vBore = vBoreList.FirstOrDefault(f => f.BoreId == bore.BoreId);
                            if (bore != null)
                            {
                                var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == this.BDCo && f.BudgetCodeId == allocatedCost.BudgetCodeId);
                                var costItems = bore.AddCostItems(bgt);
                                var dirtAmt = allocatedCost.Cost / totalDirtDays * vBore.DirtDays;
                                var rockAmt = allocatedCost.Cost / totalRockDays * vBore.RockDays;

                                costItems.ForEach(e => { e.Cost = e.GroundDensityId == 0 ? dirtAmt : rockAmt; e.IsPackageCost = true; e.Units = 1; e.Multiplier = 1; });
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        public BidPackageScope AddScope(ScopeTypeEnum scopeType)
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

        public BidPackageCostItem AddPackageCostItem()
        {
            var cost = new BidPackageCostItem
            {
                BDCo = BDCo,
                BidId = BidId,
                PackageId = PackageId,
                LineId = PackageCosts.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                Package = this,
            };
            CostItems.Add(cost);
            return cost;
        }

        public BidPackageCostItem AddPackageCostItem(ProjectBudgetCode bgt)
        {
            if (bgt == null)
                return AddPackageCostItem();

            var cost = this.CostItems.FirstOrDefault(f => f.tBudgetCodeId == bgt.BudgetCodeId);
            if (cost == null)
            {
                cost = new BidPackageCostItem
                {
                    Package = this,
                    BudgetCode = bgt,
                    
                    BDCo = BDCo,
                    BidId = BidId,
                    PackageId = PackageId,
                    LineId = CostItems.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                    tBudgetCodeId = bgt.BudgetCodeId,
                    Cost = bgt.UnitCost,
                    ShiftCnt = 1,
                    tMultiplier = 1,
                    Units = 0,
                    tApplied = 0,
                };
                CostItems.Add(cost);
            }
            
            return cost;
        }

        public void ImportDefaultProductionRates()
        {
            if (PipeSize == 0 || PipeSize >= 100)
                return;

            var template = this.db.Bids.FirstOrDefault(f => f.BidId == 0);
            var templatePackage = template.Packages.FirstOrDefault(f => f.PipeSize == this.PipeSize);

            if (templatePackage == null)
            {
                templatePackage = template.AddPackage();
                templatePackage.PipeSize = this.PipeSize;
                templatePackage.Description = string.Format("Template for {0:N2}", this.PipeSize);
                var copyPackage = template.Packages.FirstOrDefault(f => f.ProductionRates.Any());

                templatePackage.CopyProductionRates(copyPackage);
            }
            if (!templatePackage.ProductionRates.Any(f => f.GroundDensityId == 0))
                templatePackage.AddProductionRatesForGroundDensity(0);
            if (!templatePackage.ProductionRates.Any(f => f.GroundDensityId == this.GroundDensityId))
                templatePackage.AddProductionRatesForGroundDensity(this.GroundDensityId ?? 1);
            this.CopyProductionRates(templatePackage);
        }

        private void AddProductionRatesForGroundDensity(int groundDensityId)
        {
            var bidParms = db.BDCompanyParms.FirstOrDefault(f => f.BDCo == BDCo);
            var rates = new List<BidPackageProductionRate>
            {
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.MobePhaseId,
                    PassId = 1,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days,
                    ProductionDays = 1
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.PilotPhaseId,
                    PassId = 1,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = "   004-01-",//Pilot Casing
                    PassId = 1,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.ReamPhaseId,
                    PassId = 1,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.ReamPhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate,
                    ProductionDays = (decimal?)null,
                    PassId = 2
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.ReamPhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate,
                    PassId = 3,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.PullPipePhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days,
                    PassId = 1,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.SwabPhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days,
                    PassId = 1,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.TripInOutPhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate,
                    PassId = 1,
                    ProductionDays = (decimal?)null
                },
                new BidPackageProductionRate
                {
                    PipeSize = PipeSize,
                    GroundDensityId = groundDensityId,
                    PhaseId = bidParms.DeMobePhaseId,
                    ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days,
                    ProductionDays = 1,
                    PassId = 1,
                }
            };

            foreach (var rate in rates)
            {
                var dbRate = this.ProductionRates.FirstOrDefault(f => f.PipeSize == rate.PipeSize &&
                                                                      f.GroundDensityId == rate.GroundDensityId &&
                                                                      f.PhaseId == rate.PhaseId &&
                                                                      f.PassId == rate.PassId);
                if (dbRate == null)
                {
                    dbRate = AddProductionRate();
                    dbRate.PipeSize = rate.PipeSize;
                    dbRate.GroundDensityId = rate.GroundDensityId;
                    dbRate.PhaseId = rate.PhaseId;
                    dbRate.ProductionCalTypeId = rate.ProductionCalTypeId;
                    dbRate.ProductionDays = rate.ProductionDays ?? 0;
                    dbRate.ProductionRate = rate.ProductionRate ?? 0;
                    dbRate.PassId = rate.PassId;
                }
                else
                {
                    dbRate.ProductionDays = rate.ProductionDays ?? 0;
                    dbRate.ProductionRate = rate.ProductionRate ?? 0;
                }
            }
        }


        public void ImportDefaultCostItems(string prefix)
        {
            if (CostItems.Any(f => f.tBudgetCodeId.StartsWith(prefix)))
                return;

            var template = db.Bids.FirstOrDefault(f => f.BidId == 0);
            var templatePackage = template.Packages.FirstOrDefault();

            var stdList = templatePackage.CostItems.Where(f => f.tBudgetCodeId.StartsWith(prefix, StringComparison.Ordinal)).ToList();

            foreach (var costitem in stdList)
            {
                this.AddPackageCostItem(costitem.BudgetCode);
            }
        }

        private void CopyProductionRates(BidPackage srcPackage)
        {
            var srcRates = srcPackage.ProductionRates.Where(f => (f.GroundDensityId == 0 || f.GroundDensityId == this.GroundDensityId)).ToList();
            foreach (var packageRate in srcRates)
            {
                var rate = this.ProductionRates.FirstOrDefault(f => f.GroundDensityId == packageRate.GroundDensityId &&
                                                                    f.PipeSize == packageRate.PipeSize &&
                                                                    f.PhaseId == packageRate.PhaseId &&
                                                                    f.PassId == packageRate.PassId);
                if (rate == null)
                {
                    rate = this.AddProductionRate();
                    rate.GroundDensityId = packageRate.GroundDensityId;
                    rate.PhaseId = packageRate.PhaseId;
                    rate.PassId = packageRate.PassId;
                    rate.ProductionCalTypeId = packageRate.ProductionCalTypeId;
                    rate.ProductionDays = packageRate.ProductionDays;
                    rate.ProductionRate = packageRate.ProductionRate;
                }
                else
                {
                    rate.ProductionDays ??= packageRate.ProductionDays;
                    rate.ProductionRate ??= packageRate.ProductionRate;
                }
            }
        }

        public BidPackageProductionRate AddProductionRate()
        {
            var rate = new BidPackageProductionRate()
            {
                Package = this,

                BDCo = this.BDCo,
                BidId = this.BidId,
                PackageId = this.PackageId,
                LineId = this.ProductionRates.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                PipeSize = this.PipeSize,
                UM = "LF",
                BoreSize = 0,
            };
            this.ProductionRates.Add(rate);
            return rate;
        }



        //public void UpdateFromModel(Models.Views.Bid.Forms.Package.Schedule.ScheduleViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    Description = model.Description;
        //    NumberOfBores = model.NumberOfBores;
        //    RigCategoryId = model.RigCategoryId;
        //    StartDate = model.StartDate;
        //}
    }
}
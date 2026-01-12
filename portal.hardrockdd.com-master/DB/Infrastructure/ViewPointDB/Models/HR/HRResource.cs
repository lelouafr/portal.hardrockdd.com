using System;
using System.Collections.Generic;
using System.Linq;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HRResource
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
                    _db ??= VPContext.GetDbContextFromEntity(this);


                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "bHRRM"; } }

        public HQAttachment Attachment
        {
            get
            {
                
                if (HQAttachment != null)
                {
                    if (HQAttachment.TableName != BaseTableName)
                    {
                        HQAttachment.TableName = BaseTableName;
                        db.BulkSaveChanges();
                    }
                    return HQAttachment;
                }

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = this.HRCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                    HQCompanyParm = this.HRCompanyParm.HQCompanyParm,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return HQAttachment;
            }
        }

        public List<HRResource> Supervisors()
        {
            var supList = new List<HRResource>();

            var sup = Supervisor;
            if (sup == null)
                return supList;
            supList.Add(sup);
            while (!supList.Any(f => f.HRRef == sup.ReportsTo))
            {
                //Get Supervisor boss
                sup = sup.Supervisor;
                if (sup == null)
                    return supList;
                supList.Add(sup);
            }
            return supList;
        }

        public bool AuthDriver { get => this.tAuthDriverYN == "Y"; set => tAuthDriverYN = value ? "Y" : "N"; }

        public bool IsSupervisor(int supervisorId)
        {
            var currentEmpPosition = db.GetCurrentHREmployee().PositionCode;
            if (currentEmpPosition == "HR-PRMGR" ||
                currentEmpPosition == "HR-MGR" ||
                currentEmpPosition == "IT-DIR")
                return true;

            return Supervisors().Any(f => f.HRRef == supervisorId);
        }

        private string _FullName;
        public string FullName(bool includeMiddle = true)
        {
            if (!string.IsNullOrEmpty(_FullName))
                return _FullName;

            string fullName;
            string firstName = string.IsNullOrEmpty(Nickname) ? FirstName : Nickname;
            if (string.IsNullOrEmpty(MiddleName) || includeMiddle == false)
            {
                fullName = string.Format(VPContext.AppCultureInfo, "{0} {1}", firstName, LastName);
            }
            else
            {
                if (FirstName.ToLower(VPContext.AppCultureInfo).Contains(MiddleName.ToLower(VPContext.AppCultureInfo)) ||
                    FirstName.ToLower(VPContext.AppCultureInfo).Contains(MiddleName.ToLower(VPContext.AppCultureInfo)))
                {
                    fullName = string.Format(VPContext.AppCultureInfo, "{0} {1}", firstName, LastName);
                }
                else
                {
                    fullName = string.Format(VPContext.AppCultureInfo, "{0} {1} {2}", firstName, MiddleName, LastName);
                }
            }

            if (!string.IsNullOrEmpty(Suffix))
            {
                fullName = string.Format(VPContext.AppCultureInfo, "{0} {1}", fullName, Suffix);
            }
            _FullName = fullName;
            return _FullName;

        }

        public HRActiveStatusEnum EmployeeStatus()
        {
            return ActiveYN == "Y" ? HRActiveStatusEnum.Active : HRActiveStatusEnum.Inactive;
        }

        public HRResourceBenefit AddBenefit(PRImportBenefit importLine)
        {
            var benefit = this.HRResourceBenefits.FirstOrDefault(f => f.BenefitCodeId == importLine.HRBenefitCodeId && f.EffectDate == importLine.CoverageStartDate);
            if (benefit == null)
            {
                benefit = new HRResourceBenefit()
                {
                    HRCo = this.HRCo,
                    HRRef = this.HRRef,
                    BenefitCodeId = importLine.HRBenefitCodeId,
                    DependentSeq = 0,
                    EligDate = this.HireDate,
                    EffectDate = importLine.CoverageStartDate,
                    EndDate = importLine.CoverageEndDate,
                    ActiveYN = "N",
                    CafePlanYN = "N",
                    ReinstateYN = "N",
                    SmokerYN = "N",
                    UpdatedYN = "N",

                };

                HRResourceBenefits.Add(benefit);
            }

            return benefit;
        }

        public HRDrugTest AddDrugTest()
        {
            var test = new HRDrugTest()
            {
                HRCo = this.HRCo,
                HRRef = this.HRRef,
                Date = DateTime.Now.Date,
                HistSeq = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.HistSeq) + 1,
                TestStatus = "P",

            };
            this.DrugTests.Add(test);

            return test;
        }

        public HRDrugTest AddDrugTest(WAApplicantDrugTest drugTest)
        {
            if (drugTest == null)
                return AddDrugTest();

            if (drugTest.TestType == null)
            {
                return null;
            }
            var test = this.DrugTests.FirstOrDefault(f => f.Date == drugTest.TestDate);
            if (test == null)
            {
                test = new HRDrugTest()
                {
                    HRCo = this.HRCo,
                    HRRef = this.HRRef,
                    Date = (DateTime)drugTest.TestDate,
                    TestType = drugTest.TestType,
                    HistSeq = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.HistSeq) + 1,
                    TestStatus = drugTest.TestStatusId,
                    Location = drugTest.Location,
                    Results = drugTest.Results,
                    ActionTaken = drugTest.ActionTaken,
                    UniqueAttchID = drugTest.UniqueAttchID,

                    HRResource = this,


                };
                this.DrugTests.Add(test);
            }
            else
            {
                test.TestStatus = drugTest.TestStatusId ?? test.TestStatus;
                test.Location = drugTest.Location ?? test.Location;
                test.Results = drugTest.Results ?? test.Results;
                test.ActionTaken = drugTest.ActionTaken ?? test.ActionTaken;
                test.UniqueAttchID = drugTest.UniqueAttchID ?? test.UniqueAttchID;

            }

            return test;
        }

        public void PrepareForDelete()
        {
            if (HRRef >= 900000)
            {
                this.DrugTests.Clear();
                //db.SaveChanges();
                //this.DrugTests.ToList().ForEach(f => this.DrugTests.Remove(f));
                //db.SaveChanges();
                this.HRResourceBenefits.Clear();
                this.WorkExperiences.Clear();
                this.AppliedPositions.Clear();
                this.TrafficTickets.Clear();
                this.TrafficAccidents.Clear();
                this.DrivingExperiences.Clear();
                this.EmployeeTickets.Clear();

                this.EmploymentHistory.Clear();
                this.SaleryHistory.Clear();
                this.TermRequests.Clear();
                this.HRResourceBenefits.Clear();
                this.AssignedAssets.Clear();
                //db.SaveChanges();

                this.DirectReports.ToList().ForEach(e => e.ReportsTo = e.Supervisor?.HRRef);
                this.budWAAMs.ToList().ForEach(e => e.HRRefId = null);
                this.budWAAAs.ToList().ForEach(e => e.HRRef = null);
                this.bEMEMs.ToList().ForEach(e => e.OperatorId = null);
                this.bEMEMs.ToList().ForEach(e => e.AssignedEmployeeId = null);

                //db.SaveChanges();
            }
        }

        public Employee CreatePREmployee()
        {
            if (this.PREmployee == null)
            {
                this.PREmployee = this.db.Employees.FirstOrDefault(f => f.PRCo == this.PRCo && f.EmployeeId == this.HRRef);

                if (this.PREmployee != null)
                {
                    this.PRGroup = this.PREmployee.PRGroupId;
                    this.PRCo = this.PREmployee.PRCo;
                    this.PREmp = this.PREmployee.EmployeeId;
                    //this.PREmployee = employee;

                }
            }
            var employee = this.PREmployee;
            if (employee == null)
            {
                employee = new Employee()
                {
                    PRCo = this.HRCo,
                    EmployeeId = this.HRRef,
                    PRGroupId = this.HRCo,

                    LastName = this.LastName,
                    FirstName = this.FirstName,
                    MidName = this.MiddleName,
                    SortName = this.SortName,
                    Address = this.Address,
                    City = this.City,
                    State = this.State,
                    Zip = this.Zip,
                    Address2 = this.Address2,
                    Phone = this.Phone,
                    SSN = this.SSN,
                    RaceId = this.Race ?? "99",
                    Sex = this.Sex,
                    BirthDate = this.BirthDate,
                    HireDate = this.HireDate,
                    TermDate = this.TermDate,
                    PRDept = this.PRDept,
                    EarnCode = this.EarnCode,
                    OTOpt = this.OTOpt,
                    OTSched = this.OTSched,
                    OccupCat = this.OccupCat,
                    CatStatus = this.CatStatus,
                    ActiveYN = this.ActiveYN,
                    Notes = this.Notes,
                    UniqueAttchID = this.UniqueAttchID,
                    Email = this.Email,
                    Suffix = this.Suffix,
                    Shift = this.Shift,
                    NonResAlienYN = this.NonResAlienYN,
                    KeyID = this.KeyID,
                    Country = this.Country,
                    HDAmt = this.HDAmt,
                    F1Amt = this.F1Amt,
                    LCFStock = this.LCFStock,
                    LCPStock = this.LCPStock,
                    CellPhone = this.CellPhone,
                    WOTaxState = this.WOTaxState,
                    WOLocalCode = this.WOLocalCode,
                    Nickname = this.Nickname,
                    ReportsToId = this.ReportsTo,

                    ADPnumber = null,
                    PaycomNumber = null,
                    PerDiemRate = null,
                    ZionsCC = null,
                    WEXID = null,


                    InsCode = this.StdInsCode,
                    TaxState = this.StdTaxState,
                    UnempState = this.StdUnempState,
                    InsState = this.StdInsState,
                    tDivisionId = 1,
                    GLCo = 1,
                    JCCo = 1,
                    EMCo = 1,
                    EMGroup = 1,

                    HrlyRate = 0,
                    SalaryAmt = 0,
                    udDailyRate = 0,
                    CrewId = null,

                    Craft = null,
                    Class = null,
                    LocalCode = null,
                    Job = null,
                    LastUpdated = DateTime.Now,
                    DDPaySeq = null,
                    TradeSeq = null,
                    CSLimit = null,
                    CSGarnGroup = null,
                    RoutingId = null,
                    BankAcct = null,
                    AcctType = null,
                    ChkSort = null,
                    NAICS = null,
                    AUAccountNumber = null,
                    AUBSB = null,
                    AUReference = null,

                    EquipmentId = null,
                    TimesheetRevGroup = null,
                    NewHireActStartDate = null,
                    NewHireActEndDate = null,
                    RecentRehireDate = null,
                    RecentSeparationDate = null,
                    SeparationReason = null,
                    SeparationReasonExplanation = null,
                    CAProgramAccount = null,
                    AdditionalSourceDedns = null,
                    AuthorizedSourceDedns = null,
                    QCFileNumber = null,
                    PAYGIncomeType = null,

                    SMFixedRate = 0,
                    WeeklyHours = 0,
                    JCFixedRate = 0,
                    EMFixedRate = 0,
                    YTDSUI = 0,

                    ExemptHealthContribution = "N",
                    AlwaysCalcQPIP = "N",
                    PrintInFrench = "N",
                    UseState = "N",
                    UseIns = "N",
                    DirDeposit = "N",
                    PostToAll = "N",
                    CSAllocMethod = "P",
                    PensionYN = "N",
                    CertYN = "Y",
                    AUEFTYN = "N",
                    AuditYN = "Y",
                    UpdatePRAEYN = "N",
                    ArrearsActiveYN = "N",
                    ETPPostedYN = "N",
                    EmailW2YN = "N",
                    Email1095CYN = "N",
                    EmailT4YN = "N",
                    EmailPAYGSumYN = "N",
                    UseInsState = "N",
                    UseLocal = "N",
                    UseUnempState = "N",
                    PPIPExempt = "N",
                    CPPQPPExempt = "N",
                    EIExempt = "N",
                    PayMethodDelivery = "N",
                    DefaultPaySeq = "N",

                    PRCompanyParm = this.HRCompanyParm.HQCompanyParm.PRCompanyParm,
                    db = this.db,
                };
                this.PRGroup = employee.PRGroupId;
                this.PRCo = employee.PRCo;
                this.PREmp = employee.EmployeeId;
                this.PREmployee = employee;
            }

            return employee;
        }

        internal void AddEmploymentHistory(string code)
        {
            var hist = this.EmploymentHistory.FirstOrDefault(f => f.DateChanged == DateTime.Now.Date && f.Code == code);


            if (hist == null)
            {
                var seq = this.EmploymentHistory.DefaultIfEmpty().Max(max => max == null ? 0 : max.Seq) + 1;

                hist = new EmploymentHistory()
                {
                    HRCo = this.HRCo,
                    HRRef = this.HRRef,
                    Seq = (short)(seq),
                    Type = "H",
                    DateChanged = DateTime.Now.Date,
                    Code = code,
                };
                var list = EmploymentHistory.ToList();
                this.EmploymentHistory.Add(hist);

                list = EmploymentHistory.ToList();
            }
        }

        internal HRSalaryHistory AddSalaryHistory()
        {
            var sHist = this.SaleryHistory.FirstOrDefault(f => f.EffectiveDate == DateTime.Now.Date);
            if (sHist == null)
            {
                sHist = new HRSalaryHistory()
                {
                    HRCo = this.HRCo,
                    HRRef = this.HRRef,
                    EffectiveDate = DateTime.Now.Date,
                    Type = "S",
                    NewPositionCode = this.PositionCode,
                    CalcYN = "N",
                    UpdatedYN = "N",

                };

                this.SaleryHistory.Add(sHist);
            }

            return sHist;
        }

        internal HRAppliedPosition AddAppliedPosition(WAAppliedPosition postition)
        {
            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.PositionCode == postition.PositionCodeId);
            if (appliedPosition == null)
            {
                appliedPosition = new HRAppliedPosition()
                {
                    HRCo = this.HRCo,
                    HRRef = this.HRRef,
                    PositionCode = postition.PositionCodeId,
                    PartTimeYN = "F",
                    ApplyDate = postition.AppliedDate,

                };

                this.AppliedPositions.Add(appliedPosition);
            }
            else if (appliedPosition.ApplyDate != postition.AppliedDate)
            {
                appliedPosition.ApplyDate = postition.AppliedDate;
            }

            return appliedPosition;

        }
    }
    //    private VPContext _db;

    //    public VPContext db
    //    {
    //        set
    //        {
    //            _db = value;
    //        }
    //        get
    //        {
    //            if (_db == null)
    //            {
    //                _db = VPContext.GetDbContextFromEntity(this);

    //                if (_db == null)
    //                    throw new NullReferenceException("GetDbContextFromEntity is null");

    //            }
    //            return _db;
    //        }
    //    }

    //    public static string BaseTableName { get { return "bHRRM"; } }


    //    public HQAttachment Attachment
    //    {
    //        get
    //        {

    //            if (HQAttachment == null && UniqueAttchID != null)
    //            {
    //                HQAttachment = new HQAttachment()
    //                {
    //                    HQCo = this.HRCo,
    //                    UniqueAttchID = (Guid)UniqueAttchID,
    //                    TableKeyId = this.KeyID,
    //                    TableName = BaseTableName,
    //                    HQCompanyParm = this.HRCompanyParm.HQCompanyParm,
    //                };

    //                HRCompanyParm.HQCompanyParm.HQAttatchments.Add(HQAttachment);
    //            }
    //            else if (HQAttachment == null && UniqueAttchID == null)
    //            {
    //                HQAttachment = new HQAttachment()
    //                {
    //                    HQCo = this.HRCo,
    //                    UniqueAttchID = Guid.NewGuid(),
    //                    TableKeyId = this.KeyID,
    //                    TableName = BaseTableName,
    //                    HQCompanyParm = this.HRCompanyParm.HQCompanyParm,
    //                };
    //                HRCompanyParm.HQCompanyParm.HQAttatchments.Add(HQAttachment);
    //                this.UniqueAttchID = HQAttachment.UniqueAttchID;
    //            }

    //            return HQAttachment;
    //        }
    //    }


    //    public List<HRResource> Supervisors()
    //    {
    //        var supList = new List<HRResource>();

    //        var sup = Supervisor;
    //        if (sup == null)
    //            return supList;
    //        supList.Add(sup);
    //        while (!supList.Any(f => f.HRRef == sup.ReportsTo))
    //        {
    //            //Get Supervisor boss
    //            sup = sup.Supervisor;
    //            if (sup == null)
    //                return supList;
    //            supList.Add(sup);
    //        }
    //        return supList;
    //    }

    //    public bool IsSupervisor(int supervisorId)
    //    {
    //        var currentEmpPosition = db.GetCurrentHREmployee().PositionCode;
    //        if (currentEmpPosition == "HR-PRMGR" ||
    //            currentEmpPosition == "HR-MGR" ||
    //            currentEmpPosition == "IT-DIR")
    //            return true;

    //        return Supervisors().Any(f => f.HRRef == supervisorId);
    //    }

    //    private string _FullName;
    //    public string FullName(bool includeMiddle = true)
    //    {
    //        if (!string.IsNullOrEmpty(_FullName))
    //            return _FullName;

    //        string fullName;
    //        string firstName = string.IsNullOrEmpty(Nickname) ? FirstName : Nickname;
    //        if (string.IsNullOrEmpty(MiddleName) || includeMiddle == false)
    //        {
    //            fullName = string.Format("{0} {1}", firstName, LastName);
    //        }
    //        else
    //        {
    //            if (FirstName.ToLower(VPContext.AppCultureInfo).Contains(MiddleName.ToLower(VPContext.AppCultureInfo)) ||
    //                FirstName.ToLower(VPContext.AppCultureInfo).Contains(MiddleName.ToLower(VPContext.AppCultureInfo)))
    //            {
    //                fullName = string.Format("{0} {1}", firstName, LastName);
    //            }
    //            else
    //            {
    //                fullName = string.Format("{0} {1} {2}", firstName, MiddleName, LastName);
    //            }
    //        }

    //        if (!string.IsNullOrEmpty(Suffix))
    //        {
    //            fullName = string.Format("{0} {1}", fullName, Suffix);
    //        }
    //        _FullName = fullName;
    //        return _FullName;

    //    }

    //    public HRActiveStatusEnum EmployeeStatus()
    //    {
    //        return ActiveYN == "Y" ? HRActiveStatusEnum.Active : HRActiveStatusEnum.Inactive;
    //    }

    //    public HRResourceBenefit AddBenefit(PRImportBenefit importLine)
    //    {
    //        var benefit = this.HRResourceBenefits.FirstOrDefault(f => f.BenefitCodeId == importLine.HRBenefitCodeId && f.EffectDate == importLine.CoverageStartDate);
    //        if (benefit == null)
    //        {
    //            benefit = new HRResourceBenefit()
    //            {
    //                HRCo = this.HRCo,
    //                HRRef = this.HRRef,
    //                BenefitCodeId = importLine.HRBenefitCodeId,
    //                DependentSeq = 0,
    //                EligDate = this.HireDate,
    //                EffectDate = importLine.CoverageStartDate,
    //                EndDate = importLine.CoverageEndDate,
    //                ActiveYN = "N",
    //                CafePlanYN = "N",
    //                ReinstateYN = "N",
    //                SmokerYN = "N",
    //                UpdatedYN = "N",

    //            };

    //            HRResourceBenefits.Add(benefit);
    //        }

    //        return benefit;
    //    }

    //    public HRDrugTest AddDrugTest()
    //    {
    //        var test = new HRDrugTest()
    //        {
    //            HRCo = this.HRCo,
    //            HRRef = this.HRRef,
    //            Date = DateTime.Now.Date,
    //            HistSeq = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.HistSeq) + 1,
    //            TestStatus = "P",

    //        };
    //        this.DrugTests.Add(test);

    //        return test;
    //    }

    //    public HRDrugTest AddDrugTest(WAApplicantDrugTest drugTest)
    //    {
    //        if (drugTest == null)
    //            return AddDrugTest();

    //        var test = this.DrugTests.FirstOrDefault(f => f.Date == drugTest.TestDate);
    //        if (test == null)
    //        {
    //            test = new HRDrugTest()
    //            {
    //                HRCo = this.HRCo,
    //                HRRef = this.HRRef,
    //                Date = (DateTime)drugTest.TestDate,
    //                TestType = drugTest.TestType,
    //                HistSeq = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.HistSeq) + 1,
    //                TestStatus = drugTest.TestStatusId,
    //                Location = drugTest.Location,
    //                Results = drugTest.Results,
    //                ActionTaken = drugTest.ActionTaken,
    //                UniqueAttchID = drugTest.UniqueAttchID,

    //                HRResource = this,


    //            };
    //            this.DrugTests.Add(test);
    //        }
    //        else
    //        {
    //            test.TestStatus = drugTest.TestStatusId;
    //            test.Location = drugTest.Location;
    //            test.Results = drugTest.Results;
    //            test.ActionTaken = drugTest.ActionTaken;
    //            test.UniqueAttchID = drugTest.UniqueAttchID;

    //        }

    //        return test;
    //    }

    //    public void PrepareForDelete()
    //    {
    //        if (HRRef >= 900000)
    //        {
    //            this.DrugTests.Clear();
    //            this.HRResourceBenefits.Clear();
    //            this.WorkExperiences.Clear();
    //            this.AppliedPositions.Clear();
    //            this.TrafficTickets.Clear();
    //            this.TrafficAccidents.Clear();
    //            this.DrivingExperiences.Clear();
    //            this.EmployeeTickets.Clear();

    //            this.EmploymentHistory.Clear();
    //            this.SaleryHistory.Clear();
    //            this.TermRequests.Clear();
    //            this.HRResourceBenefits.Clear();
    //            this.AssignedAssets.Clear();

    //            this.DirectReports.ToList().ForEach(e => e.ReportsTo = e.Supervisor?.HRRef);
    //            this.budWAAMs.ToList().ForEach(e => e.HRRefId = null);
    //            this.budWAAAs.ToList().ForEach(e => e.HRRef = null);
    //            this.bEMEMs.ToList().ForEach(e => e.OperatorId = null);
    //            this.bEMEMs.ToList().ForEach(e => e.AssignedEmployeeId = null);

    //            //db.SaveChanges();
    //        }
    //    }

    //    public Employee CreatePREmployee()
    //    {
    //        var employee = this.PREmployee;
    //        if (employee == null)
    //        {
    //            employee = new Employee()
    //            {
    //                PRCo = this.HRCo,
    //                EmployeeId = this.HRRef,
    //                PRGroupId = this.HRCo,

    //                LastName = this.LastName,
    //                FirstName = this.FirstName,
    //                MidName = this.MiddleName,
    //                SortName = this.SortName,
    //                Address = this.Address,
    //                City = this.City,
    //                State = this.State,
    //                Zip = this.Zip,
    //                Address2 = this.Address2,
    //                Phone = this.Phone,
    //                SSN = this.SSN,
    //                RaceId = this.Race,
    //                Sex = this.Sex,
    //                BirthDate = this.BirthDate,
    //                HireDate = this.HireDate,
    //                TermDate = this.TermDate,
    //                PRDept = this.PRDept,
    //                EarnCode = this.EarnCode,
    //                OTOpt = this.OTOpt,
    //                OTSched = this.OTSched,
    //                OccupCat = this.OccupCat,
    //                CatStatus = this.CatStatus,
    //                ActiveYN = this.ActiveYN,
    //                Notes = this.Notes,
    //                UniqueAttchID = this.UniqueAttchID,
    //                Email = this.Email,
    //                Suffix = this.Suffix,
    //                Shift = this.Shift,
    //                NonResAlienYN = this.NonResAlienYN,
    //                KeyID = this.KeyID,
    //                Country = this.Country,
    //                HDAmt = this.HDAmt,
    //                F1Amt = this.F1Amt,
    //                LCFStock = this.LCFStock,
    //                LCPStock = this.LCPStock,
    //                CellPhone = this.CellPhone,
    //                WOTaxState = this.WOTaxState,
    //                WOLocalCode = this.WOLocalCode,
    //                Nickname = this.Nickname,
    //                ReportsToId = this.ReportsTo,

    //                ADPnumber = null,
    //                PaycomNumber = null,
    //                PerDiemRate = null,
    //                ZionsCC = null,
    //                WEXID = null,

    //                InsCode = this.StdInsCode,
    //                TaxState = this.StdTaxState,
    //                UnempState = this.StdUnempState,
    //                InsState = this.StdInsState,

    //                DivisionId = 1,
    //                GLCo = 1,
    //                JCCo = 1,
    //                EMCo = 1,
    //                EMGroup = 1,

    //                HrlyRate = 0,
    //                SalaryAmt = 0,
    //                udDailyRate = 0,
    //                CrewId = null,

    //                Craft = null,
    //                Class = null,
    //                LocalCode = null,
    //                Job = null,
    //                LastUpdated = DateTime.Now,
    //                DDPaySeq = null,
    //                TradeSeq = null,
    //                CSLimit = null,
    //                CSGarnGroup = null,
    //                RoutingId = null,
    //                BankAcct = null,
    //                AcctType = null,
    //                ChkSort = null,
    //                NAICS = null,
    //                AUAccountNumber = null,
    //                AUBSB = null,
    //                AUReference = null,

    //                EquipmentId = null,
    //                TimesheetRevGroup = null,
    //                NewHireActStartDate = null,
    //                NewHireActEndDate = null,
    //                RecentRehireDate = null,
    //                RecentSeparationDate = null,
    //                SeparationReason = null,
    //                SeparationReasonExplanation = null,
    //                CAProgramAccount = null,
    //                AdditionalSourceDedns = null,
    //                AuthorizedSourceDedns = null,
    //                QCFileNumber = null,
    //                PAYGIncomeType = null,

    //                SMFixedRate = 0,
    //                WeeklyHours = 0,
    //                JCFixedRate = 0,
    //                EMFixedRate = 0,
    //                YTDSUI = 0,

    //                ExemptHealthContribution = "N",
    //                AlwaysCalcQPIP = "N",
    //                PrintInFrench = "N",
    //                UseState = "N",
    //                UseIns = "N",
    //                DirDeposit = "N",
    //                PostToAll = "N",
    //                CSAllocMethod = "P",
    //                PensionYN = "N",
    //                CertYN = "Y",
    //                AUEFTYN = "N",
    //                AuditYN = "Y",
    //                UpdatePRAEYN = "N",
    //                ArrearsActiveYN = "N",
    //                ETPPostedYN = "N",
    //                EmailW2YN = "N",
    //                Email1095CYN = "N",
    //                EmailT4YN = "N",
    //                EmailPAYGSumYN = "N",
    //                UseInsState = "N",
    //                UseLocal = "N",
    //                UseUnempState = "N",
    //                PPIPExempt = "N",
    //                CPPQPPExempt = "N",
    //                EIExempt = "N",
    //                PayMethodDelivery = "N",
    //                DefaultPaySeq = "N",

    //                PRCompanyParm = this.HRCompanyParm.HQCompanyParm.PRCompanyParm,
    //                db = this.db,                    
    //            };
    //            this.PRGroup = employee.PRGroupId;
    //            this.PRCo = employee.PRCo;
    //            this.PREmp = employee.EmployeeId;
    //            this.PREmployee = employee;
    //        }


    //        return employee;
    //    }

    //    internal void AddEmploymentHistory(string code)
    //    {
    //        var hist = this.EmploymentHistory.FirstOrDefault(f => f.DateChanged == DateTime.Now.Date && f.Code == code);


    //        if (hist == null)
    //        {
    //            hist = new EmploymentHistory()
    //            {
    //                HRCo = this.HRCo,
    //                HRRef = this.HRRef,
    //                Seq = (short)(this.EmploymentHistory.DefaultIfEmpty().Max(max => max == null ? 0 : max.Seq) + 1),
    //                Type = "H",

    //            };

    //            this.EmploymentHistory.Add(hist);
    //        }
    //    }

    //    internal HRSalaryHistory AddSalaryHistory()
    //    {
    //        var sHist = this.SaleryHistory.FirstOrDefault(f => f.EffectiveDate == DateTime.Now.Date);
    //        if (sHist == null)
    //        { 
    //            sHist = new HRSalaryHistory()
    //            {
    //                HRCo = this.HRCo,
    //                HRRef = this.HRRef,
    //                EffectiveDate= DateTime.Now.Date,
    //                Type = "S",
    //                NewPositionCode = this.PositionCode,


    //            };

    //            this.SaleryHistory.Add(sHist);
    //        }

    //        return sHist;
    //    }
    //}

}
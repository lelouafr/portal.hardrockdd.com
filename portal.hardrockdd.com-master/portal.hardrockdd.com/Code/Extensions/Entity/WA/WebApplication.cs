using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    

    public partial class WebApplication
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
                        _db = this.Applicant.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budWAAA"; } }

        public DB.WAApplicationStatusEnum Status
        {
            get
            {
                return (DB.WAApplicationStatusEnum)tStatusId;

            }
            set
            {
                StatusId = (int)value;
            }
        }

        public int StatusId
        {
            get
            {
                return tStatusId;
            }
            set
            {
                if (value != tStatusId)
                {
                    UpdateStatus(value);
                }
            }
        }


        public DB.WAApplicationDeniedReasonEnum? DeniedReason
        {
            get
            {
                return (DB.WAApplicationDeniedReasonEnum?)tDeniedReasonId;

            }
            set
            {
                if (value == null)
                    tDeniedReasonId = null;
                else
                    tDeniedReasonId = (int)value;
            }
        }

        public int? DeniedReasonId
        {
            get
            {
                return tDeniedReasonId;
            }
            set
            {
                if (value != tDeniedReasonId)
                {
                    UpdateDeniedReason(value);
                }
            }
        }

        public bool Citiations
        {
            get
            {
                return tCitiations ?? false;
            }
            set
            {
                tCitiations = value;
            }
        }

        public bool Accidents
        {
            get
            {
                return tAccidents ?? false;
            }
            set
            {
                tAccidents = value;
            }
        }

        public void UpdateStatus(int value)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            tStatusId = value;

            switch (Status)
            {
                case DB.WAApplicationStatusEnum.Draft:
                case DB.WAApplicationStatusEnum.Started:
                    tIsActive = false;
                    ActiveYN = "N";
                    break;
                case DB.WAApplicationStatusEnum.Submitted:
                case DB.WAApplicationStatusEnum.Approved:
                    tIsActive = true;
                    ActiveYN = "Y";
                    break;
                case DB.WAApplicationStatusEnum.Denied:
                case DB.WAApplicationStatusEnum.Shelved:
                case DB.WAApplicationStatusEnum.Hired:
                case DB.WAApplicationStatusEnum.Canceled:
                default:
                    tIsActive = false;
                    ActiveYN = "N";
                    break;
            }
            WorkFlow.CreateSequance(value);
            //WorkFlow.CurrentSequance().Comments = StatusComments;
            GenerateWorkFlowAssignments();
        }

        public void UpdateDeniedReason(int? value)
        {
            tDeniedReasonId = value;

        }

        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == WFCo);
                var workflow = new WorkFlow
                {
                    WFCo = (byte)WFCo,
                    WorkFlowId = db.GetNextId(portal.Code.Data.VP.WorkFlow.BaseTableName),
                    TableName = WebApplication.BaseTableName,
                    Id = ApplicantId,
                    LineId = ApplicationId,
                    CreatedBy = StaticFunctions.GetUserId(),
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = company,
                };
                db.WorkFlows.Add(workflow);
                WorkFlow = workflow;
                WorkFlowId = workflow.WorkFlowId;
            }
        }

        public void GenerateWorkFlowAssignments()
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            switch (Status)
            {
                case DB.WAApplicationStatusEnum.Draft:
                    break;
                case DB.WAApplicationStatusEnum.Started:
                    break;
                case DB.WAApplicationStatusEnum.Submitted:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    break;
                case DB.WAApplicationStatusEnum.Approved:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    break;
                case DB.WAApplicationStatusEnum.Denied:
                case DB.WAApplicationStatusEnum.Hired:
                case DB.WAApplicationStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;

                default:
                    break;
            }
        }

        public WATrafficIncident AddTrafficAccident()
        {
            var accident = new WATrafficIncident()
            {
                ApplicantId = this.ApplicantId,
                ApplicationId = this.ApplicationId,
                SeqId = this.TrafficIncidents.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                IncidentType = DB.WAIncidentTypeEnum.Accident,

                Application = this,
            };

            this.TrafficIncidents.Add(accident);

            return accident;
        }

        public WATrafficIncident AddTrafficAccident(HRTrafficAccident hrAccident)
        {
            if (hrAccident == null)
                return null;

            var accident = this.TrafficIncidents.FirstOrDefault(f => f.IncidentDate == hrAccident.Date && f.Description == hrAccident.Description);
            if (accident == null)
            {
                accident = new WATrafficIncident()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.TrafficIncidents.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    IncidentType = DB.WAIncidentTypeEnum.Accident,
                    Description = hrAccident.Description,
                    IncidentDate = hrAccident.Date,
                    NumberOfInjuries = (byte?)hrAccident.NumberOfInjuries ?? 0,

                    Application = this,
                };

                this.TrafficIncidents.Add(accident);
            }

            return accident;
        }

        public WATrafficIncident AddTrafficTicket()
        {
            var ticket = new WATrafficIncident()
            {
                ApplicantId = this.ApplicantId,
                ApplicationId = this.ApplicationId,
                SeqId = this.TrafficIncidents.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                IncidentType = DB.WAIncidentTypeEnum.Citation,

                Application = this,
            };

            this.TrafficIncidents.Add(ticket);

            return ticket;
        }

        public WATrafficIncident AddTrafficTicket(HRTrafficTicket hrTicket)
        {
            if (hrTicket == null)
                return null;

            var ticket = this.TrafficIncidents.FirstOrDefault(f => f.IncidentDate == hrTicket.Date && f.CitationType == hrTicket.Charge);
            if (ticket == null)
            {
                ticket = new WATrafficIncident()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.TrafficIncidents.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    IncidentType = DB.WAIncidentTypeEnum.Citation,
                    Description = hrTicket.Charge,
                    IncidentDate = hrTicket.Date,
                    CitationType = hrTicket.Charge,
                    Location = hrTicket.Location,

                    Application = this,

                };

                this.TrafficIncidents.Add(ticket);
            }

            return ticket;
        }

        public WAWorkExperience AddWorkHistory()
        {
            var work = new WAWorkExperience()
            {
                ApplicantId = this.ApplicantId,
                ApplicationId = this.ApplicationId,
                SeqId = this.WorkExperiences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

                Application = this,

            };

            this.WorkExperiences.Add(work);

            return work;
        }

        public WAWorkExperience AddWorkHistory(HRWorkExperience experience)
        {
            if (experience == null)
                return null;

            var work = this.WorkExperiences.FirstOrDefault(f => f.WorkFromDate == experience.WorkFrom && f.Employer == experience.Employer);
            if (work == null)
            {
                work = new WAWorkExperience()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.WorkExperiences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    Employer = experience.Employer,
                    AddressCity = experience.AddressCity,
                    AddressState = experience.AddressState,
                    AddressStreet = experience.AddressStreet,
                    AddressZip = experience.AddressZip,
                    JobDuties = experience.JobDuties,
                    Position = experience.Position,
                    ReasonForLeaving = experience.ReasonForLeaving,
                    Salary = experience.Salary,
                    SupervisorName = experience.SupervisorName,
                    WorkFromDate = experience.WorkFrom,
                    WorkToDate = experience.WorkTo,
                    WorkPhone = experience.WorkPhone,

                    Application = this,
                };

                this.WorkExperiences.Add(work);
            }

            return work;
        }

        public WAAppliedPosition AddPosition()
        {
            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.tPositionCodeId == null);
            if (appliedPosition == null)
            {
                appliedPosition = new WAAppliedPosition()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.AppliedPositions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,                   
                    AppliedDate = this.ApplicationDate,
                    Application = this,
                };
                this.AppliedPositions.Add(appliedPosition);
            }
            return appliedPosition;
        }

        public WAAppliedPosition AddPosition(HRPosition position)
        {
            if (position == null)
                return null;

            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.HRCo == position.HRCo && f.tPositionCodeId == position.PositionCodeId);
            if (appliedPosition == null)
            {
                appliedPosition = new WAAppliedPosition()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.AppliedPositions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    HRCo = position.HRCo,
                    tPositionCodeId = position.PositionCodeId,
                    AppliedDate = this.ApplicationDate,

                    Application = this,
                };
                this.AppliedPositions.Add(appliedPosition);
            }
            return appliedPosition;
        }

        public WAAppliedPosition AddPosition(HRAppliedPosition position)
        {
            if (position == null)
                return null;
            var appliedDate = position.ApplyDate ?? this.ApplicationDate;
            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.HRCo == position.HRCo && f.tPositionCodeId == position.PositionCode && f.AppliedDate == appliedDate);
            if (appliedPosition == null)
            {
                appliedPosition = new WAAppliedPosition()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.AppliedPositions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    HRCo = position.HRCo,
                    tPositionCodeId = position.PositionCode,
                    AppliedDate = appliedDate,

                    Application = this,
                };
                this.AppliedPositions.Add(appliedPosition);
            }
            return appliedPosition;
        }

        public WADrivingExperience AddDrivingExperience()
        {
            var work = new WADrivingExperience()
            {
                ApplicantId = this.ApplicantId,
                ApplicationId = this.ApplicationId,
                SeqId = this.WorkExperiences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

                Application = this,

            };

            this.DrivingExperiences.Add(work);

            return work;
        }

        public WADrivingExperience AddDrivingExperience(HRDrivingExperience experience)
        {
            if (experience == null)
                return null;

            var work = this.DrivingExperiences.FirstOrDefault(f => f.FromDate == experience.DateFrom && f.TypeOfEquipment == experience.TypeOfEquipment);
            if (work == null)
            {
                work = new WADrivingExperience()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.DrivingExperiences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    FromDate = experience.DateFrom,
                    ToDate = experience.DateTo,
                    Miliage = experience.Miliage,
                    TypeOfEquipment = experience.TypeOfEquipment,

                    Application = this,
                };

                this.DrivingExperiences.Add(work);
            }

            return work;
        }

        public WAApplicantDrugTest AddDrugTest(DateTime testDate)
        {
            var test = this.DrugTests.FirstOrDefault(f => f.TestDate == testDate);
            if (test == null)
            {
                test = new WAApplicantDrugTest()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    TestDate = DateTime.Now.Date,
                    SeqId = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    TestStatusId = "P",

                };
                this.DrugTests.Add(test);
            }

            return test;
        }

        public WAApplicantDrugTest AddDrugTest(HRDrugTest drugTest)
        {
            if (drugTest == null)
                return AddDrugTest(DateTime.Now.Date);

            var test = this.DrugTests.FirstOrDefault(f => f.TestDate == drugTest.Date);
            if (test == null)
            {
                test = new WAApplicantDrugTest()
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    TestDate = drugTest.Date,
                    TestType = drugTest.TestType,
                    SeqId = this.DrugTests.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    TestStatusId = drugTest.TestStatus,
                    Location = drugTest.Location,
                    Results = drugTest.Results,
                    ActionTaken = drugTest.ActionTaken,
                    UniqueAttchID = drugTest.UniqueAttchID,
                    
                    Applicaiton = this,
                    

                };
                this.DrugTests.Add(test);
            }
            else
            {
                test.TestType = drugTest.TestType;
                test.TestStatusId = drugTest.TestStatus;
                test.Location = drugTest.Location;
                test.Results = drugTest.Results;
                test.ActionTaken = drugTest.ActionTaken;
                test.UniqueAttchID = drugTest.UniqueAttchID;

            }

            return test;
        }

        private WAAppliedPosition? _postition;
        
        public WAAppliedPosition? CurrentPosition()
        {
            if (_postition == null)
                _postition = this.AppliedPositions.OrderByDescending(s => s.SeqId).FirstOrDefault();

            return _postition;
        }

        public HRResource CreateEmployee(HRPositionRequest request)
        {
            throw new NotImplementedException();
            var resource = this.HRResource;
            if (resource == null)
                resource = db.HRResources.FirstOrDefault(f => f.SSN == this.Applicant.SSN);

            //Delete the old 9 Series Record if exists
            if (resource?.HRRef >= 900000)
            {
                resource.PrepareForDelete();
                db.HRResources.Remove(resource);
                
                resource = null;

                db.SaveChanges();
            }

            var sortName = string.Concat(this.Applicant.LastName.ToUpper().Replace(" ", string.Empty), this.Applicant.FirstName.ToUpper().Replace(" ", string.Empty));
            sortName =  System.Text.RegularExpressions.Regex.Replace(sortName, "[^a-zA-Z0-9_.]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
            var note = string.Format("Application Notes --{0}--", DateTime.Now.Date);
            note += "\r\n";
            note += this.Applicant.Notes;
            note += "\r\n";
            if (resource == null)
            {
                resource = new HRResource()
                {
                    HRCo = request.HRCo,
                    HRRef = db.GetNextId("bHRRM"),
                    SortName = sortName,
                    Address = this.Applicant.Address,
                    Address2 = this.Applicant.Address2,
                    AltContactPhone = this.Applicant.AltContactPhone,
                    AvailableDate = this.Applicant.AvailableDate,
                    BirthDate = this.Applicant.BirthDate,
                    CellPhone = this.Applicant.CellPhone,
                    City = this.Applicant.City,
                    ContactPhone = this.Applicant.ContactPhone,
                    Country = this.Applicant.Country,
                    CurrentEmployer = this.Applicant.CurrentEmployer,
                    Email = this.Applicant.Email,
                    ExpectedSalary = this.Applicant.ExpectedSalary,
                    FirstName = this.Applicant.FirstName,
                    LastContactDate = this.Applicant.LastContactDate,
                    LastName = this.Applicant.LastName,
                    LicClass = this.Applicant.LicClass,
                    LicCountry = this.Applicant.LicCountry,
                    LicExpDate = this.Applicant.LicExpDate,
                    LicNumber = this.Applicant.LicNumber,
                    LicState = this.Applicant.LicState,
                    LicType = this.Applicant.LicType,
                    MaidenName = this.Applicant.MaidenName,
                    MaritalStatus = this.Applicant.MaritalStatus,
                    MiddleName = this.Applicant.MiddleName,
                    NoContactEmplYN = this.Applicant.NoContactEmplYN,
                    Notes = note,
                    PassPort = this.Applicant.PassPort,
                    Phone = this.Applicant.Phone,
                    Race = this.Applicant.Race ?? "99",
                    SSN = this.Applicant.SSN,
                    Sex = this.Applicant.Sex ?? "M",
                    SpouseName = this.Applicant.SpouseName,
                    State = this.Applicant.State,
                    StatusCode = this.Applicant.StatusCode,
                    Suffix = this.Applicant.Suffix,
                    WorkPhone = this.Applicant.WorkPhone,
                    Zip = this.Applicant.Zip,

                    AppliedBeforeDate = this.Applicant.AppliedBeforeDate,
                    AppliedBeforeYN = this.Applicant.AppliedBeforeYN,
                    DOTSap = this.Applicant.DOTSap,
                    EmegencyContactPhone = this.Applicant.EmegencyContactPhone,
                    EmegencyRelationship = this.Applicant.EmegencyRelationship,
                    EmergencyContact = this.Applicant.EmergencyContact,
                    FaildDOTDrugDate = this.Applicant.FaildDOTDrugDate,
                    FailedDOTDrug = this.Applicant.FailedDOTDrug,
                    FelonyConviction = this.Applicant.FelonyConviction,
                    FelonyNote = this.Applicant.FelonyNote,
                    LicDenied = this.Applicant.LicDenied,
                    LicDeniedRevokedReason = this.Applicant.LicDeniedRevokedReason,
                    LicRevoked = this.Applicant.LicRevoked,
                    Nickname = this.Applicant.Nickname,
                    PortalAccountActive = this.Applicant.PortalAccountActive,
                    Referral = this.Applicant.Referral,
                    SAPDate = this.Applicant.SAPDate,
                    WebId = this.Applicant.WebId,

                    ActiveYN = "N",
                    ApplicationDate = this.ApplicationDate,
                    
                    HireDate = this.HireDate,
                    PRDept = this.PRDeptId,
                    StdInsState = "TX",
                    StdInsCode= this.PRInsCodeId,
                    W4CompleteYN = "N",
                    NoRehireYN = "N",
                    RelativesYN = "N",
                    HandicapYN = "N",
                    PhysicalYN = "N",
                    DriveCoVehiclesYN = "N",
                    ExistsInPR = "N",
                    TempWorker = "N",
                    NonResAlienYN = "N",
                    AFServiceMedalVetYN = "N",
                    Status = "A",
                    DisabledVetYN = "N",
                    VietnamVetYN = "N",
                    OtherVetYN = "N",
                    StdTaxState = "TX",
                    StdUnempState = "TX",
                    OTOpt = "N",
                    EarnCodeId = this.PREarnCodeId,
                    HRCompanyParm = request.HRCompanyParm,
                    db = db,
                    
                };

                request.HRCompanyParm.HRResources.Add(resource);
            }
            else
            {

                resource.Sex = "M";
                resource.ActiveYN = "N";
                resource.W4CompleteYN = "N";
                resource.NoRehireYN = "N";
                resource.PassPort = "N";
                resource.RelativesYN = "N";
                resource.HandicapYN = "N";
                resource.PhysicalYN = "N";
                resource.DriveCoVehiclesYN = "N";
                resource.NoContactEmplYN = "N";
                resource.ExistsInPR = "N";
                resource.TempWorker = "N";
                resource.NonResAlienYN = "N";
                resource.AFServiceMedalVetYN = "N";
                resource.Status = "A";
                resource.DisabledVetYN = "N";
                resource.VietnamVetYN = "N";
                resource.OtherVetYN = "N";
                resource.StdTaxState = "TX";
                resource.StdUnempState = "TX";
                resource.StdInsState = "TX";
                resource.LicDenied = "N";
                resource.LicRevoked = "N";
                resource.TermDate = null;
                resource.TermReason = null;

                resource.BirthDate = this.Applicant.BirthDate;
                resource.FirstName = this.Applicant.FirstName;
                resource.LastName = this.Applicant.LastName;
                resource.SortName = sortName;
                resource.Suffix = this.Applicant.Suffix;
                resource.MiddleName = this.Applicant.MiddleName;
                resource.Nickname = this.Applicant.Nickname;
                resource.MaidenName = this.Applicant.MaidenName;

                resource.Phone = this.Applicant.Phone;
                resource.CellPhone = this.Applicant.CellPhone;
                resource.WorkPhone = this.Applicant.WorkPhone;
                resource.AltContactPhone = this.Applicant.AltContactPhone;

                resource.Email = this.Applicant.Email;

                resource.SSN = this.Applicant.SSN;

                resource.Address = this.Applicant.Address;
                resource.Address2 = this.Applicant.Address2;
                resource.City = this.Applicant.City;
                resource.State = this.Applicant.State;
                resource.Zip = this.Applicant.Zip;
                resource.Country = this.Applicant.Country;

                resource.PassPort = this.Applicant.PassPort;

                if (string.IsNullOrEmpty(resource.Notes))
                    resource.Notes = note;
                else
                {
                    note = "\r\n" + note;
                    resource.Notes += note;
                }

                resource.Race ??= this.Applicant.Race ?? "99";
                resource.Sex ??= this.Applicant.Sex ?? "M";

                resource.AvailableDate = this.Applicant.AvailableDate;
                resource.CurrentEmployer = this.Applicant.CurrentEmployer;
                resource.ExpectedSalary = this.Applicant.ExpectedSalary;
                resource.LastContactDate = this.Applicant.LastContactDate;
                resource.NoContactEmplYN = this.Applicant.NoContactEmplYN;

                resource.MaritalStatus = this.Applicant.MaritalStatus;
                resource.SpouseName = this.Applicant.SpouseName;

                resource.StatusCode = this.Applicant.StatusCode;

                resource.AppliedBeforeDate = this.Applicant.AppliedBeforeDate;
                resource.AppliedBeforeYN = this.Applicant.AppliedBeforeYN;
                resource.DOTSap = this.Applicant.DOTSap;
                resource.SAPDate = this.Applicant.SAPDate;

                resource.EmegencyContactPhone = this.Applicant.EmegencyContactPhone;
                resource.EmegencyRelationship = this.Applicant.EmegencyRelationship;
                resource.EmergencyContact = this.Applicant.EmergencyContact;
                resource.ContactPhone = this.Applicant.ContactPhone;


                resource.FaildDOTDrugDate = this.Applicant.FaildDOTDrugDate;
                resource.FailedDOTDrug = this.Applicant.FailedDOTDrug;
                resource.FelonyConviction = this.Applicant.FelonyConviction;
                resource.FelonyNote = this.Applicant.FelonyNote;

                resource.LicClass = this.Applicant.LicClass;
                resource.LicCountry = this.Applicant.LicCountry;
                resource.LicExpDate = this.Applicant.LicExpDate;
                resource.LicNumber = this.Applicant.LicNumber;
                resource.LicState = this.Applicant.LicState;
                resource.LicType = this.Applicant.LicType;
                resource.LicDenied = this.Applicant.LicDenied;
                resource.LicDeniedRevokedReason = this.Applicant.LicDeniedRevokedReason;
                resource.LicRevoked = this.Applicant.LicRevoked;

                resource.PortalAccountActive = "Y";
                resource.Referral = this.Applicant.Referral;
                resource.ApplicationDate = this.ApplicationDate;
            }
            resource.PositionCode = request.HRPosition.PositionCodeId;
            resource.Position = request.HRPosition;
            resource.ActiveYN = "Y";
            resource.ReportsTo = request.ForCrew.CrewLeaderID;
            //db.SaveChanges();
            this.DrugTests.ToList().ForEach(e => resource.AddDrugTest(e));

            resource.CreatePREmployee();
            if (resource.PREmployee != null)
            {
                var employee = resource.PREmployee;
                if (employee.HireDate == null || employee.HireDate == this.HireDate)
                {
                    employee.HireDate = this.HireDate;
                    //resource.AddEmploymentHistory("HIRED");
                }
                else
                {
                    employee.RecentRehireDate = this.HireDate;
                    //resource.AddEmploymentHistory("REHIRED");
                }
                employee.EarnCodeId = (short)this.PREarnCodeId;
                employee.InsCode = this.PRInsCodeId;
                employee.PRDept = this.PRDeptId;
                employee.PerDiemRate = decimal.ToByte(this.PRPerDeimRate ?? 0);
                employee.HrlyRate = this.PRHrlyRate ?? 0;
                employee.SalaryAmt = this.PRSalaryAmt ?? 0;
                employee.PRGroupId = this.PRGroupId ?? 1;

                var shist = resource.AddSalaryHistory();
                if (employee.EarnCodeId == 1)
                {
                    shist.NewSalary = employee.HrlyRate;
                    shist.Type = "H";
                }
                else
                {
                    shist.NewSalary = employee.SalaryAmt;
                    shist.Type = "S";
                }
                //resource.AddEmploymentHistory("SALARY");

                request.PRCo = employee.PRCo;
                request.tNewEmployeeId = employee.EmployeeId;
                request.NewPREmployee = employee;

                employee.CrewId = request.ForCrewId;
                employee.Crew = request.ForCrew;

            }


            this.Status = DB.WAApplicationStatusEnum.Hired;
            return resource;
        }

        public Forum GetForum()
        {
            if (this.Forum == null)
            {
                var forum = new Forum()
                {
                    Co = 1,
                    TableName = BaseTableName,
                    ForumId = db.Forums.DefaultIfEmpty().Max(max => max == null ? 0 : max.ForumId) + 1
                };
                this.Forum = forum;

                db.BulkSaveChanges();
            }

            return this.Forum;
        }

    }
}
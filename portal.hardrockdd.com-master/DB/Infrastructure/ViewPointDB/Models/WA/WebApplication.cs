using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    

    public partial class WebApplication
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
                _db ??= this.Applicant.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budWAAA"; } }

		public HQAttachment Attachment
		{
			get
			{
				if (HQAttachment != null)
					return HQAttachment;

				UniqueAttchID ??= Guid.NewGuid();
				var attachment = new HQAttachment()
				{
					HQCo = this.HRCo ?? 1,
					UniqueAttchID = (Guid)UniqueAttchID,
					TableKeyId = this.ApplicantId,
					TableName = BaseTableName,
					HQCompanyParm = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1),

					db = this.db,
				};
				db.HQAttachments.Add(attachment);
				attachment.BuildDefaultFolders();
				HQAttachment = attachment;
				db.BulkSaveChanges();

				return HQAttachment;
			}
		}

		public WAApplicationStatusEnum Status
        {
            get
            {
                return (WAApplicationStatusEnum)tStatusId;

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


        public WAApplicationDeniedReasonEnum? DeniedReason
        {
            get
            {
                return (WAApplicationDeniedReasonEnum?)tDeniedReasonId;

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
                case WAApplicationStatusEnum.Draft:
                case WAApplicationStatusEnum.Started:
                    tIsActive = false;
                    ActiveYN = "N";
                    break;
                case WAApplicationStatusEnum.Submitted:
                case WAApplicationStatusEnum.Approved:
                    tIsActive = true;
                    ActiveYN = "Y";
                    break;
                case WAApplicationStatusEnum.Denied:
                case WAApplicationStatusEnum.Shelved:
                case WAApplicationStatusEnum.Hired:
                case WAApplicationStatusEnum.Canceled:
                default:
                    tIsActive = false;
                    ActiveYN = "N";
                    break;
            }
            WorkFlow.CreateSequence(value);
            //WorkFlow.CurrentSequence().Comments = StatusComments;
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
                    WorkFlowId = db.GetNextId(DB.Infrastructure.ViewPointDB.Data.WorkFlow.BaseTableName),
                    TableName = WebApplication.BaseTableName,
                    Id = ApplicantId,
                    LineId = ApplicationId,
                    CreatedBy = db.CurrentUserId ?? "System",
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
                case WAApplicationStatusEnum.Draft:
                    break;
                case WAApplicationStatusEnum.Started:
                    break;
                case WAApplicationStatusEnum.Submitted:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    break;
                case WAApplicationStatusEnum.Approved:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    break;
                case WAApplicationStatusEnum.Denied:
                case WAApplicationStatusEnum.Hired:
                case WAApplicationStatusEnum.Canceled:
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
                IncidentType = WAIncidentTypeEnum.Accident,

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
                    IncidentType = WAIncidentTypeEnum.Accident,
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
                IncidentType = WAIncidentTypeEnum.Citation,

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
                    IncidentType = WAIncidentTypeEnum.Citation,
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

            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.HRCo == position.HRCo && f.PositionCodeId == position.PositionCodeId);
            if (appliedPosition == null)
            {
                appliedPosition = new WAAppliedPosition()
				{
					db = this.db,
					Application = this,

					ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.AppliedPositions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    HRCo = position.HRCo,
                    PositionCodeId = position.PositionCodeId,
                    AppliedDate = this.ApplicationDate,

                    HRPositionCode= position,

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
            var appliedPosition = this.AppliedPositions.FirstOrDefault(f => f.HRCo == position.HRCo && f.PositionCodeId == position.PositionCode && f.AppliedDate == appliedDate);
            if (appliedPosition == null)
            {
                appliedPosition = new WAAppliedPosition()
                {
                    db = position.Resource.db,
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.ApplicationId,
                    SeqId = this.AppliedPositions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    HRCo = position.HRCo,
                    PositionCodeId = position.PositionCode,
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
                SeqId = this.DrivingExperiences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

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

        #nullable enable
        private WAAppliedPosition? _postition;        
        public WAAppliedPosition? CurrentPosition()
        {
            if (_postition == null)
                _postition = this.AppliedPositions.OrderByDescending(s => s.SeqId).FirstOrDefault();

            return _postition;
        }

        public HRResource? CreateEmployee(HRPositionRequest request)
        {
            if (request == null)
                return null;

            var resource = HRResource;
            if (resource == null)
                resource = db.HRResources.FirstOrDefault(f => f.SSN == Applicant.SSN);

            //Delete the old 9 Series Record if exists
            if (resource?.HRRef >= 900000)
            {
                resource.PrepareForDelete();
                db.HRResources.Remove(resource);
                
                resource = null;

                db.SaveChanges();
            }

            var sortName = string.Concat(Applicant.LastName.ToUpper().Replace(" ", string.Empty), Applicant.FirstName.ToUpper().Replace(" ", string.Empty));
            sortName =  System.Text.RegularExpressions.Regex.Replace(sortName, "[^a-zA-Z0-9_.]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
            sortName = sortName.Length >= 15 ? sortName.Substring(0, 14) : sortName;

            //Check that the sort name doesn't already exists if so add a cnt to the name
            var sortCnt = 0;
            while (db.HRResources.Any(f => f.SortName == sortName))
            {
                sortCnt++;
                sortName = sortName.Length >= 13 ? sortName.Substring(0, 12) : sortName;
                sortName = string.Format("{0}{1}", sortName, sortCnt.ToString().PadLeft(2,'0'));
            }

            var note = string.Format("Application Notes --{0}--", DateTime.Now.Date);
            note += "\r\n";
            note += Applicant.Notes;
            note += "\r\n";
            if (resource == null)
            {
                resource = new HRResource()
                {
                    HRCo = request.HRCo,
                    HRRef = db.GetNextId("bHRRM"),
                    SortName = sortName,
                    Address = Applicant.Address,
                    Address2 = Applicant.Address2,
                    AltContactPhone = Applicant.AltContactPhone,
                    AvailableDate = Applicant.AvailableDate,
                    BirthDate = Applicant.BirthDate,
                    CellPhone = Applicant.CellPhone,
                    City = Applicant.City,
                    ContactPhone = Applicant.ContactPhone,
                    Country = Applicant.Country,
                    CurrentEmployer = Applicant.CurrentEmployer,
                    Email = Applicant.Email ?? "",
                    ExpectedSalary = Applicant.ExpectedSalary,
                    FirstName = Applicant.FirstName,
                    LastContactDate = Applicant.LastContactDate,
                    LastName = Applicant.LastName,
                    LicClass = Applicant.LicClass,
                    LicCountry = Applicant.LicCountry,
                    LicExpDate = Applicant.LicExpDate,
                    LicNumber = Applicant.LicNumber,
                    LicState = Applicant.LicState,
                    LicType = Applicant.LicType,
                    MaidenName = Applicant.MaidenName,
                    MaritalStatus = Applicant.MaritalStatus,
                    MiddleName = Applicant.MiddleName,
                    NoContactEmplYN = Applicant.NoContactEmplYN ?? "N",
                    Notes = note,
                    PassPort = Applicant.PassPort ?? "N",
                    Phone = Applicant.Phone,
                    Race = Applicant.Race ?? "99",
                    SSN = Applicant.SSN,
                    Sex = Applicant.Sex ?? "M",
                    SpouseName = Applicant.SpouseName,
                    State = Applicant.State,
                    StatusCode = Applicant.StatusCode,
                    Suffix = Applicant.Suffix,
                    WorkPhone = Applicant.WorkPhone,
                    Zip = Applicant.Zip,

                    AppliedBeforeDate = Applicant.AppliedBeforeDate,
                    AppliedBeforeYN = Applicant.AppliedBeforeYN,
                    DOTSap = Applicant.DOTSap,
                    udFMCSAPHMSA = HRDotStatus,

                    EmegencyContactPhone = Applicant.EmegencyContactPhone,
                    EmegencyRelationship = Applicant.EmegencyRelationship,
                    EmergencyContact = Applicant.EmergencyContact,
                    FaildDOTDrugDate = Applicant.FaildDOTDrugDate,
                    FailedDOTDrug = Applicant.FailedDOTDrug,
                    FelonyConviction = Applicant.FelonyConviction,
                    FelonyNote = Applicant.FelonyNote,
                    LicDenied = Applicant.LicDenied,
                    LicDeniedRevokedReason = Applicant.LicDeniedRevokedReason,
                    LicRevoked = Applicant.LicRevoked,
                    Nickname = Applicant.Nickname,
                    PortalAccountActive = Applicant.PortalAccountActive,
                    Referral = Applicant.Referral,
                    SAPDate = Applicant.SAPDate,
                    WebId = Applicant.WebId,

                    ActiveYN = "N",
                    ApplicationDate = ApplicationDate,
                    
                    HireDate = HireDate,
                    PRDept = PRDeptId,
                    StdInsState = "TX",
                    StdInsCode= PRInsCodeId,
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
                    HasCompanyEmail = "N",
                    AddActiveDirectory = "N",
                    ADActive = "N",
                    OTOpt = "W",
					EarnCodeId = PREarnCodeId,
                    HRCompanyParm = request.HRCompanyParm,
                    db = db,
                };

                request.HRCompanyParm.HRResources.Add(resource);
            }
            else
            {

                resource.OTOpt = "W";
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
                resource.NoContactEmplYN = "N";
                //resource.Race = "99";
                resource.TermDate = null;
                resource.TermReason = null;

                resource.BirthDate = Applicant.BirthDate;
                resource.FirstName = Applicant.FirstName;
                resource.LastName = Applicant.LastName;
                resource.SortName = sortName;
                resource.Suffix = Applicant.Suffix;
                resource.MiddleName = Applicant.MiddleName;
                resource.Nickname = Applicant.Nickname;
                resource.MaidenName = Applicant.MaidenName;

                resource.Phone = Applicant.Phone;
                resource.CellPhone = Applicant.CellPhone;
                resource.WorkPhone = Applicant.WorkPhone;
                resource.AltContactPhone = Applicant.AltContactPhone;

                resource.Email = Applicant.Email;

                resource.SSN = Applicant.SSN;

                resource.Address = Applicant.Address;
                resource.Address2 = Applicant.Address2;
                resource.City = Applicant.City;
                resource.State = Applicant.State;
                resource.Zip = Applicant.Zip;
                resource.Country = Applicant.Country;

                resource.PassPort = Applicant.PassPort ?? "N";

                resource.udFMCSAPHMSA = HRDotStatus;


				if (string.IsNullOrEmpty(resource.Notes))
                    resource.Notes = note;
                else
                {
                    note = "\r\n" + note;
                    resource.Notes += note;
                }

                resource.Race ??= Applicant.Race ?? "99";
                resource.Sex ??= Applicant.Sex ?? "M";

                resource.AvailableDate = Applicant.AvailableDate;
                resource.CurrentEmployer = Applicant.CurrentEmployer;
                resource.ExpectedSalary = Applicant.ExpectedSalary;
                resource.LastContactDate = Applicant.LastContactDate;
                resource.NoContactEmplYN = Applicant.NoContactEmplYN ?? "N";

                resource.MaritalStatus = Applicant.MaritalStatus;
                resource.SpouseName = Applicant.SpouseName;

                resource.StatusCode = Applicant.StatusCode;

                resource.AppliedBeforeDate = Applicant.AppliedBeforeDate;
                resource.AppliedBeforeYN = Applicant.AppliedBeforeYN ?? "N";
                resource.DOTSap = Applicant.DOTSap;
                resource.SAPDate = Applicant.SAPDate;

                resource.EmegencyContactPhone = Applicant.EmegencyContactPhone;
                resource.EmegencyRelationship = Applicant.EmegencyRelationship;
                resource.EmergencyContact = Applicant.EmergencyContact;
                resource.ContactPhone = Applicant.ContactPhone;


                resource.FaildDOTDrugDate = Applicant.FaildDOTDrugDate;
                resource.FailedDOTDrug = Applicant.FailedDOTDrug;
                resource.FelonyConviction = Applicant.FelonyConviction;
                resource.FelonyNote = Applicant.FelonyNote;

                resource.LicClass = Applicant.LicClass;
                resource.LicCountry = Applicant.LicCountry;
                resource.LicExpDate = Applicant.LicExpDate;
                resource.LicNumber = Applicant.LicNumber;
                resource.LicState = Applicant.LicState;
                resource.LicType = Applicant.LicType;
                resource.LicDenied = Applicant.LicDenied;
                resource.LicDeniedRevokedReason = Applicant.LicDeniedRevokedReason;
                resource.LicRevoked = Applicant.LicRevoked;

                resource.Referral = Applicant.Referral;

                //resource.ActiveYN = "N";
                resource.ApplicationDate = ApplicationDate;
                resource.HasCompanyEmail ??= "N";
                resource.AddActiveDirectory ??= "N";
                resource.ADActive ??= "N";
            }
            resource.PositionCode = request.HRPosition.PositionCodeId;
            resource.Position = request.HRPosition;
            resource.PortalAccountActive = "Y";
            resource.ActiveYN = "Y";
            resource.ReportsTo = request.ForCrew.CrewLeaderId;
            resource.Status = "A";

            DrugTests.ToList().ForEach(e => resource.AddDrugTest(e));

            resource.CreatePREmployee();
            if (resource.PREmployee != null)
            {
                var employee = resource.PREmployee;
                if (employee.HireDate == null || employee.HireDate == HireDate)
                {
                    employee.HireDate = HireDate;
                    //resource.AddEmploymentHistory("HIRED");
                }
                else
                {
                    employee.RecentRehireDate = HireDate;
                    //resource.AddEmploymentHistory("REHIRED");
                }
                employee.EarnCodeId = (short)PREarnCodeId;
                employee.InsCode = PRInsCodeId;
                employee.PRDept = PRDeptId;
                employee.PerDiemRate = decimal.ToByte(PRPerDeimRate ?? 0);
                employee.HrlyRate = PRHrlyRate ?? 0;
                employee.SalaryAmt = PRSalaryAmt ?? 0;                

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

            var posList = AppliedPositions.Where(f => f.PositionCodeId == request.PositionCodeId).ToList();
            posList.ForEach(e => resource.AddAppliedPosition(e));

            Status = WAApplicationStatusEnum.Hired;
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
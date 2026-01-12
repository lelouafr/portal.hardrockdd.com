using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Code.Data.VP
{
    public partial class WebApplicant
    {
        public static string BaseTableName { get { return "budWAAM"; } }

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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }


        private string _FullName;
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_FullName))
                {
                    _FullName = this.FullNameBuilder(false);
                }
                return _FullName;
            }
        }
        private DB.WAApplicationStatusEnum ConvertResourceStatusToEnum(string resourceStatus)
        {
            switch (resourceStatus)
            {
                case "A":
                    return DB.WAApplicationStatusEnum.Hired;
                case "APP":
                    return DB.WAApplicationStatusEnum.Submitted;
                case "APPDND":
                    return DB.WAApplicationStatusEnum.Denied;
                case "APPDRAFT":
                    return DB.WAApplicationStatusEnum.Draft;
                case "DECLINE":
                case "FAIL":
                case "NH":
                case "NS":
                case "LS":
                    return DB.WAApplicationStatusEnum.Denied;
                case "RET":
                case "RH":
                case "S":
                case "Seasonal":
                case "TEMP":
                case "WC":
                case "LO":
                case "MIL":
                case "MT":
                default:
                    return DB.WAApplicationStatusEnum.Draft;
            }
        }

        private DB.WAApplicationDeniedReasonEnum? ConvertResourceDeniedReasonToEnum(string resourceStatus)
        {
            switch (resourceStatus)
            {
                case "A":
                case "APP":
                case "APPDRAFT":
                    return null;
                case "APPDND":
                    return DB.WAApplicationDeniedReasonEnum.ApplicationDenied;
                case "DECLINE":
                    return DB.WAApplicationDeniedReasonEnum.DeniedJob;
                case "FAIL":
                    return DB.WAApplicationDeniedReasonEnum.FailedDrugTest;
                case "NH":
                    return DB.WAApplicationDeniedReasonEnum.NotHired;
                case "NS":
                    return DB.WAApplicationDeniedReasonEnum.NoShow;
                case "LS":
                    return DB.WAApplicationDeniedReasonEnum.SuspendedLicense;
                case "RET":
                case "RH":
                case "S":
                case "Seasonal":
                case "TEMP":
                case "WC":
                case "LO":
                case "MIL":
                case "MT":
                default:
                    return null;
            }
        }

        public WebApplication AddApplication(HRAppliedPosition appliedPosition)
        {
            var appliedDate = (appliedPosition.ApplyDate ?? appliedPosition.Resource.HireDate) ?? DateTime.Now;

            var application = this.Applications.FirstOrDefault(f => f.ApplicationDate.Date == appliedDate.Date);
            if (application == null)
            {
                application = new WebApplication
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.Applications.DefaultIfEmpty().Max(max => max == null ? 0 : max.ApplicationId) + 1,
                    ApplicationDate = appliedDate,
                    ActiveYN = "Y",
                    Applicant = this,
                    db = this.db,
                    WFCo = 1,
                };
                application.ExpireDate = application.ApplicationDate.AddYears(1);

                Applications.Add(application);
            }
            Applications
                .Where(f => (f.Status == DB.WAApplicationStatusEnum.Submitted ||
                            f.Status == DB.WAApplicationStatusEnum.Draft ||
                            f.Status == DB.WAApplicationStatusEnum.Started ) &&
                            f.ApplicationDate < appliedDate
                )
                .ToList()
                .ForEach(e =>
                {
                    e.Status = DB.WAApplicationStatusEnum.Canceled;
                });
            application.Status = ConvertResourceStatusToEnum(appliedPosition.Resource.Status);
            application.DeniedReason = ConvertResourceDeniedReasonToEnum(appliedPosition.Resource.Status);
            application.AddPosition(appliedPosition);

            return application;
        }

        public WebApplication AddApplication()
        {
            var application = this.Applications.FirstOrDefault(f => f.Status == DB.WAApplicationStatusEnum.Draft);
            if (application == null)
            {
                application = new WebApplication
                {
                    ApplicantId = this.ApplicantId,
                    ApplicationId = this.Applications.DefaultIfEmpty().Max(max => max == null ? 0 : max.ApplicationId) + 1,
                    ApplicationDate = DateTime.Now,
                    ActiveYN = "Y",
                    tStatusId = 0,
                    Applicant = this,
                    db = this.db,
                };


                Applications.Add(application);

            }

            return application;
        }

        public static WebApplicant AddApplicant(HRResource resource)
        {
            if (resource == null)
                return null;

            var db = resource.db;
            var applicant = db.AddApplicant(resource);

            //var crossRef = db.budWA_CrossRef_Temp.FirstOrDefault(f => f.HRRef == resource.HRRef);
            //WebApplicant applicant = null;
            //applicant = db.WebApplicants.FirstOrDefault(f => f.SSN == resource.SSN);
            //if (crossRef != null && applicant == null)
            //{
            //    applicant = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == crossRef.ApplicantId);
            //}
            //if (applicant == null)
            //{
            //    //var dbId = db.WebApplicants.DefaultIfEmpty().Max(max => max == null ? 0 : max.ApplicantId) + 1;
            //    //var localId = db.WebApplicants.Local.DefaultIfEmpty().Max(max => max == null ? 0 : max.ApplicantId) + 1;
            //    var applicantId = crossRef?.ApplicantId ?? db.GetNextId(BaseTableName);

            //    applicant = new WebApplicant()
            //    {
            //        db = resource.db,

            //        ApplicantId = applicantId,
            //        UserEmail = resource.Email,
            //        WebId = resource.WebId,
            //        //HRCo = resource.HRCo,
            //        //HRRefId = resource.HRRef,
            //        PRCo = resource.PRCo,
            //        PREmployeeId = resource.PREmp,
            //        LastName = resource.LastName,
            //        FirstName = resource.FirstName,
            //        MiddleName = resource.MiddleName,
            //        Nickname = resource.Nickname,
            //        Address = resource.Address,
            //        City = resource.City,
            //        State = resource.State,
            //        Zip = resource.Zip,
            //        Address2 = resource.Address2,
            //        Phone = resource.Phone,
            //        WorkPhone = resource.WorkPhone,
            //        CellPhone = resource.CellPhone,
            //        SSN = resource.SSN,
            //        Sex = resource.Sex,
            //        Race = resource.Race,
            //        BirthDate = resource.BirthDate,
            //        MaritalStatus = resource.MaritalStatus,
            //        MaidenName = resource.MaidenName,
            //        SpouseName = resource.SpouseName,
            //        PassPort = resource.PassPort,
            //        LicNumber = resource.LicNumber,
            //        LicType = resource.LicType,
            //        LicState = resource.LicState,
            //        LicExpDate = resource.LicExpDate,
            //        AvailableDate = resource.AvailableDate,
            //        LastContactDate = resource.LastContactDate,
            //        ContactPhone = resource.ContactPhone,
            //        AltContactPhone = resource.AltContactPhone,
            //        ExpectedSalary = resource.ExpectedSalary,
            //        CurrentEmployer = resource.CurrentEmployer,
            //        NoContactEmplYN = resource.NoContactEmplYN,
            //        Notes = resource.Notes,
            //        Email = resource.Email,
            //        Suffix = resource.Suffix,
            //        LicClass = resource.LicClass,
            //        Country = resource.Country,
            //        LicCountry = resource.LicCountry,
            //        StatusCode = resource.StatusCode,
            //        EmergencyContact = resource.EmergencyContact,
            //        EmegencyRelationship = resource.EmegencyRelationship,
            //        LicDenied = resource.LicDenied,
            //        LicRevoked = resource.LicRevoked,
            //        LicDeniedRevokedReason = resource.LicDeniedRevokedReason,
            //        EmegencyContactPhone = resource.EmegencyContactPhone,
            //        FailedDOTDrug = resource.FailedDOTDrug,
            //        FaildDOTDrugDate = resource.FaildDOTDrugDate,
            //        DOTSap = resource.DOTSap,
            //        SAPDate = resource.SAPDate,
            //        FelonyConviction = resource.FelonyConviction,
            //        FelonyNote = resource.FelonyNote,
            //        AppliedBeforeYN = resource.AppliedBeforeYN,
            //        AppliedBeforeDate = resource.AppliedBeforeDate,
            //        Referral = resource.Referral,
            //        PortalAccountActive = resource.PortalAccountActive,
            //    };
            //    if (crossRef == null)
            //    {
            //        db.budWA_CrossRef_Temp.Add(new budWA_CrossRef_Temp()
            //        {
            //            ApplicantId = applicant.ApplicantId,
            //            HRRef = resource.HRRef
            //        });
            //    }
            //    db.WebApplicants.Add(applicant);
            //}
            //else
            //{
            //    applicant.LastName = resource.LastName;
            //    applicant.FirstName = resource.FirstName;
            //    applicant.MiddleName = resource.MiddleName;
            //    applicant.Nickname = resource.Nickname;
            //    applicant.Address = resource.Address;
            //    applicant.City = resource.City;
            //    applicant.State = resource.State;
            //    applicant.Zip = resource.Zip;
            //    applicant.Address2 = resource.Address2;
            //    applicant.Phone = resource.Phone;
            //    applicant.WorkPhone = resource.WorkPhone;
            //    applicant.CellPhone = resource.CellPhone;
            //    applicant.SSN = resource.SSN;
            //    applicant.Sex = resource.Sex;
            //    applicant.Race = resource.Race;
            //    applicant.BirthDate = resource.BirthDate;
            //    applicant.MaritalStatus = resource.MaritalStatus;
            //    applicant.MaidenName = resource.MaidenName;
            //    applicant.SpouseName = resource.SpouseName;
            //    applicant.PassPort = resource.PassPort;
            //    applicant.LicNumber = resource.LicNumber;
            //    applicant.LicType = resource.LicType;
            //    applicant.LicState = resource.LicState;
            //    applicant.LicExpDate = resource.LicExpDate;
            //    applicant.AvailableDate = resource.AvailableDate;
            //    applicant.LastContactDate = resource.LastContactDate;
            //    applicant.ContactPhone = resource.ContactPhone;
            //    applicant.AltContactPhone = resource.AltContactPhone;
            //    applicant.ExpectedSalary = resource.ExpectedSalary;
            //    applicant.CurrentEmployer = resource.CurrentEmployer;
            //    applicant.NoContactEmplYN = resource.NoContactEmplYN;
            //    applicant.Notes = resource.Notes;
            //    applicant.Email = resource.Email;
            //    applicant.Suffix = resource.Suffix;
            //    applicant.LicClass = resource.LicClass;
            //    applicant.Country = resource.Country;
            //    applicant.LicCountry = resource.LicCountry;
            //    applicant.StatusCode = resource.StatusCode;
            //    applicant.EmergencyContact = resource.EmergencyContact;
            //    applicant.EmegencyRelationship = resource.EmegencyRelationship;
            //    applicant.LicDenied = resource.LicDenied;
            //    applicant.LicRevoked = resource.LicRevoked;
            //    applicant.LicDeniedRevokedReason = resource.LicDeniedRevokedReason;
            //    applicant.EmegencyContactPhone = resource.EmegencyContactPhone;
            //    applicant.FailedDOTDrug = resource.FailedDOTDrug;
            //    applicant.FaildDOTDrugDate = resource.FaildDOTDrugDate;
            //    applicant.DOTSap = resource.DOTSap;
            //    applicant.SAPDate = resource.SAPDate;
            //    applicant.FelonyConviction = resource.FelonyConviction;
            //    applicant.FelonyNote = resource.FelonyNote;
            //    applicant.AppliedBeforeYN = resource.AppliedBeforeYN;
            //    applicant.AppliedBeforeDate = resource.AppliedBeforeDate;
            //    applicant.Referral = resource.Referral;
            //    applicant.PortalAccountActive = resource.PortalAccountActive;
            //    if (resource.HRRef < 900000)
            //    {
            //        applicant.HRCo = resource.HRCo;
            //        applicant.HRRefId = resource.HRRef;
            //    }
            //}
            //foreach (var appliedPosition in resource.AppliedPositions)
            //{
            //    var application = applicant.AddApplication(appliedPosition);
            //    foreach (var hrAccident in resource.TrafficAccidents)
            //    {
            //        application.AddTrafficAccident(hrAccident);
            //    }
            //    foreach (var hrTicket in resource.TrafficTickets)
            //    {
            //        application.AddTrafficTicket(hrTicket);
            //    }
            //    foreach (var employement in resource.WorkExperiences)
            //    {
            //        if (!string.IsNullOrEmpty(employement.Employer))
            //            application.AddWorkHistory(employement);
            //    }
            //    foreach (var hrDrivingExperiance in resource.DrivingExperiences)
            //    {
            //        application.AddDrivingExperience(hrDrivingExperiance);
            //    }
            //    foreach (var  durgTest in resource.DrugTests)
            //    {
            //        application.AddDrugTest(durgTest);
            //    }
            //    application.AddPosition(appliedPosition.Position);

            //    application.tAccidents = application.TrafficIncidents.Where(f => f.IncidentType == DB.WAIncidentTypeEnum.Accident).Any();
            //    application.tCitiations = application.TrafficIncidents.Where(f => f.IncidentType == DB.WAIncidentTypeEnum.Citation).Any();

            //}
            //if (applicant.Applications.Any() )
            //{
            //    var appliedDate = applicant.Applications.Max(f => f.ApplicationDate);

            //    applicant.Applications
            //        .Where(f => (f.Status == DB.WAApplicationStatusEnum.Submitted ||
            //                    f.Status == DB.WAApplicationStatusEnum.Draft ||
            //                    f.Status == DB.WAApplicationStatusEnum.Started) &&
            //                    f.ApplicationDate < appliedDate
            //        )
            //        .ToList()
            //        .ForEach(e =>
            //        {
            //            e.Status = DB.WAApplicationStatusEnum.Canceled;
            //        });

            //}
            ////foreach (var ticket in appliedPosition.Resource.DrivingAccidentHistory.ToList())
            ////{
            ////    if (!string.IsNullOrEmpty(ticket.Description))
            ////    {

            ////    }
            ////}


            return applicant;
        }

        private WebApplication? _app;
        public WebApplication? CurrentApplication()
        {
            if (_app == null)
                _app = this.Applications
                    .Where(f => f.Status == DB.WAApplicationStatusEnum.Submitted || f.Status == DB.WAApplicationStatusEnum.Approved)
                    .OrderByDescending(s => s.ApplicationId)
                    .FirstOrDefault();

            if (_app == null)
                _app = this.Applications
                    .OrderByDescending(s => s.ApplicationId)
                    .FirstOrDefault();
            return _app;
        }

        internal SelectListItem SelectListItem()
        {
            return new SelectListItem() {
                Value = this.ApplicantId.ToString(),
                Text = this.FullName,
            };
        }


        private string FullNameBuilder(bool includeMiddle = true)
        {

            string fullName;
            string firstName = string.IsNullOrEmpty(this.Nickname) ? this.FirstName : this.Nickname;
            if (string.IsNullOrEmpty(this.MiddleName) || includeMiddle == false)
            {
                fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", firstName, this.LastName);
            }
            else
            {
                if (this.FirstName.ToLower(AppCultureInfo.CInfo()).Contains(this.MiddleName.ToLower(AppCultureInfo.CInfo())) ||
                    this.FirstName.ToLower(AppCultureInfo.CInfo()).Contains(this.MiddleName.ToLower(AppCultureInfo.CInfo())))
                {
                    fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", firstName, this.LastName);
                }
                else
                {
                    fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", firstName, this.MiddleName, this.LastName);
                }
            }

            if (!string.IsNullOrEmpty(this.Suffix))
            {
                fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", fullName, this.Suffix);
            }

            return fullName;

        }

    }
}
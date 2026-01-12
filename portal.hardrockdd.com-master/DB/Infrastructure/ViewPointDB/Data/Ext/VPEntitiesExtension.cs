using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class VPContext : DbContext
    {
        public VPContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
               : base(objectContext, dbContextOwnsObjectContext)
        {
        }
        private Dictionary<string, int> IdList;
        private ObjectCache systemCache = MemoryCache.Default;

        internal string AttachmentShare = @"\\192.168.30.20\Attachment";


        internal string AttachmentUserDomain = "SRV-VISTA";

        internal string AttachmentUser = "PortalShare";

        internal string AttachmentPass = "P0rt@l$h@r3$";

        public static System.Globalization.CultureInfo AppCultureInfo => new System.Globalization.CultureInfo("en-US", false);

        public void RejectChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        
        public ObjectContext ThisObjectContext
        {
            get
            {
                return ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext;
            }
        }

        public void Detach(object entity)
        {
            ThisObjectContext.Detach(entity);
        }

        public static VPContext GetDbContextFromEntity(object entity)
        {
            //if (entity == null)
            //{
            //    throw new ArgumentNullException(nameof(entity));
            //}
            //var object_context = GetObjectContextFromEntity(entity);

            //if (object_context == null)
            //    return null;

            return DbContextExtension.GetDbContextFromEntity(entity);
        }

        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            this.BulkInsert(auditEntries);
            //this.BulkSaveChanges();
            return base.SaveChanges();
        }

        private List<Audit> OnBeforeSaveChanges()
        {
            var auditEntries = new List<Audit>();
            try
            {
                ChangeTracker.DetectChanges();
                var changeCnt = ChangeTracker.Entries().Count();

                var changeTime = DateTime.Now;
                using var db = new VPContext();
                var userId = this.CurrentUserId;
                if (userId == null)
                {
                    userId = "SYSTEM";
                }

                var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);

                if (webUser != null)
                {
                    userId = webUser.Email;
                }
                var entries = ChangeTracker.Entries().ToList();
                foreach (var entry in entries)
                {
                    if (entry.Entity is Audit ||
                        entry.Entity is DailyStatusLog ||
                        entry.Entity is WebController ||
                        entry.Entity is WebUserLog ||
                        entry.Entity.GetType().Name.Contains("TimeTemp") ||
                        entry.Entity.GetType().Name.Contains("EMBatchUsage") ||
                        entry.Entity.GetType().Name.Contains("WebUserAccess") ||
                        entry.Entity.GetType().Name.Contains("WebUserLog") ||
                        //entry.Entity.GetType().Name.Contains("DailyEquipmentUsage") ||
                        entry.State == EntityState.Detached ||
                        entry.State == EntityState.Unchanged)
                        continue;

                    string tableName = entry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() is TableAttribute tableAttr ? tableAttr.Name : entry.Entity.GetType().Name;
                    if (tableName.IndexOf('_') > 0)
                    {
                        tableName = tableName.Substring(0, tableName.IndexOf('_'));
                    }

                    tableName = tableName.Substring(0, tableName.Length > 30 ? 30 : tableName.Length);

                    var keyNames = entry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).ToList();
                    var keyString = "<KeyString";

                    foreach (var key in keyNames)
                    {
                        if (!string.IsNullOrEmpty(keyString))
                        {
                            keyString += " ";
                        }
                        var dbEntity = entry.CurrentValues;
                        if (entry.State == EntityState.Deleted)
                            dbEntity = entry.OriginalValues;

                        var prop = dbEntity.GetType().GetProperty(key.Name);

                        if (prop == typeof(DateTime))
                            keyString += string.Format("[{0}] = \"{1:MM/dd/yy}\"", key.Name, dbEntity.GetValue<object>(key.Name));
                        else
                            keyString += string.Format("[{0}] = \"{1}\"", key.Name, dbEntity.GetValue<object>(key.Name));
                    }
                    if (entry.Entity.GetType().GetProperties().Any(f => f.Name == "KeyID"))
                    {
                        if (!string.IsNullOrEmpty(keyString))
                        {
                            keyString += " ";
                        }
                        keyString += string.Format("[{0}] = \"{1}\"", "KeyID", entry.CurrentValues.GetValue<object>("KeyID"));
                    }
                    keyString += " />";
                    if (keyString == null)
                    {
                        keyString = "";
                    }
                    if (entry.State == EntityState.Added)
                    {
                        var coField = entry.CurrentValues.PropertyNames.FirstOrDefault(f => f.Contains("Co") && f.Length <= 4);
                        var co = coField != null ? (byte?)entry.CurrentValues.GetValue<object>(coField) : null;
                        auditEntries.Add(new Audit()
                        {
                            TableName = tableName,
                            Co = co,
                            KeyString = keyString,
                            DateTime = changeTime,
                            RecType = "A",
                            FieldName = "All Fields",
                            OldValue = null,
                            NewValue = "default",
                            UserName = userId,
                        });
                        //foreach (string propertyName in entry.CurrentValues.PropertyNames)
                        //{
                        //    var newValue = entry.CurrentValues.GetValue<object>(propertyName)?.ToString();
                        //    if (newValue != null)
                        //    {
                        //        auditEntries.Add(new Audit()
                        //        {
                        //            TableName = tableName,
                        //            Co = co,
                        //            KeyString = keyString,
                        //            DateTime = changeTime,
                        //            RecType = "A",
                        //            FieldName = propertyName,
                        //            OldValue = null,
                        //            NewValue = newValue,
                        //            UserName = userId,
                        //        });
                        //    }
                        //}
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        //auditEntries.Add(new Audit()
                        //{
                        //    TableName = tableName,
                        //    Co = null,
                        //    KeyString = keyString,
                        //    DateTime = changeTime,
                        //    RecType = "D",
                        //    FieldName = "All*",
                        //    OldValue = null,
                        //    NewValue = null,
                        //    UserName = userId,
                        //});
                        foreach (string propertyName in entry.OriginalValues.PropertyNames)
                        {
                            var oldValue = entry.OriginalValues.GetValue<object>(propertyName)?.ToString();

                            var coField = entry.OriginalValues.PropertyNames.FirstOrDefault(f => f.Contains("Co") && f.Length <= 4);
                            var co = coField != null ? (byte?)entry.OriginalValues.GetValue<object>(coField) : null;
                            if (oldValue != null)
                            {
                                auditEntries.Add(new Audit()
                                {
                                    TableName = tableName,
                                    Co = co,
                                    KeyString = keyString,
                                    DateTime = changeTime,
                                    RecType = "D",
                                    FieldName = propertyName,
                                    OldValue = oldValue,
                                    NewValue = null,
                                    UserName = userId,
                                });
                            }
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        foreach (string propertyName in entry.CurrentValues.PropertyNames)
                        {
                            var coField = entry.OriginalValues.PropertyNames.FirstOrDefault(f => f.Contains("Co") && f.Length <= 4);
                            var co = coField != null ? (byte?)entry.OriginalValues.GetValue<object>(coField) : null;
                            var oldValue = entry.OriginalValues.GetValue<object>(propertyName)?.ToString();
                            var newValue = entry.CurrentValues.GetValue<object>(propertyName)?.ToString();
                            if (newValue != oldValue)
                            {
                                auditEntries.Add(new Audit()
                                {
                                    TableName = tableName,
                                    Co = co,
                                    KeyString = keyString,
                                    DateTime = changeTime,
                                    RecType = "C",
                                    FieldName = propertyName,
                                    OldValue = oldValue,
                                    NewValue = newValue,
                                    UserName = userId,
                                });
                            }
                        }
                    }
                    // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

                }
            }
            catch (Exception ex)
            {

                return auditEntries;
            }

            return auditEntries;
        }

        public int GetNextAttachmentId()
        {
            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("attachmentId", typeof(int));
            var result = udNextAttachmentId(outParm);

            return (int)outParm.Value;
        }

        private int _RetryCountGetNextId = 0;
        public int GetNextId(string tableName,int blockOutCnt = 1)
        {
            var randSleep = new Random();
            try
            {
                System.Threading.Thread.Sleep(randSleep.Next(1, 250));
                var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

                var result = udNextId(tableName, blockOutCnt, outParm);

                _RetryCountGetNextId = 0;
                return (int)outParm.Value;
            }
            catch (Exception ex)
            {
                if (_RetryCountGetNextId <= 5)
                {
                    _RetryCountGetNextId++;
                    System.Threading.Thread.Sleep(randSleep.Next(100, 500));
                    return GetNextId(tableName, blockOutCnt);
                }
                throw;
            }
        }

        private static string GetHttpContextUserId()
        {
            if (HttpContext.Current != null)
            {
                var curentUserId = HttpContext.Current.User.Identity.GetUserId();
                var memKey = "UserIdOverRide_" + curentUserId;

                if ((MemoryCache.Default[memKey] is string OverrideUserId))
                {
                    return OverrideUserId;
                }
                return HttpContext.Current.User.Identity.GetUserId();
            }
            else
            {
                return "caac7817-4e69-48a9-b8a8-795ac8e32867";
            }
        }

        private string? _CurrentUserId;

        public string CurrentUserId 
        { 
            get
            {
                if (_CurrentUserId == null)
                    _CurrentUserId = GetHttpContextUserId();
                return _CurrentUserId;
            }
            set
            {
                var curentUserId = HttpContext.Current.User.Identity.GetUserId();
                var memKey = "UserIdOverRide_" + curentUserId;
                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
                };
                //_CurrentUserId
                systemCache.Set(memKey, value, policy);
            }
        }

        private Employee _employee;

        public Employee GetCurrentEmployee()
        {
            var resource = GetCurrentHREmployee();

            if (_employee != null)
            {
                if (CurrentUserId != resource?.WebId)
                    _employee = null;
                if (_employee?.EmployeeId != resource?.PREmp)
                    _employee = null;
            }
            else
            {
                if (resource != null)
                    _employee = this.Employees.FirstOrDefault(f => f.EmployeeId == resource.PREmp && f.PRCo == resource.PRCo);
            }

            if (_employee == null)
                _employee = new Employee();
            return _employee;
        }

        private HRResource _resource;
        public HRResource GetCurrentHREmployee()
        {
            if (_resource != null)
            {
                if (CurrentUserId != _resource?.WebId)
                    _resource = null;

            }
            else
            {
                if (!string.IsNullOrEmpty(CurrentUserId))
                    _resource = this.HRResources.FirstOrDefault(f => f.WebId == CurrentUserId);
            }

            if (_resource == null)
                _resource = new HRResource();
            return _resource;
        }

        private HQCompanyParm? _CurrentCompany;
        public HQCompanyParm? GetCurrentCompany(byte? overrideCompany = null)
        {
            if (overrideCompany != null && _CurrentCompany != null)
            {
                if (_CurrentCompany.HQCo != overrideCompany)
                    _CurrentCompany = null;
            }
            if (_CurrentCompany == null)
            {
                if (overrideCompany != null)
                {
                    _CurrentCompany = this.HQCompanyParms.FirstOrDefault(f => f.HQCo == overrideCompany);
                }
                else if (!string.IsNullOrEmpty(CurrentUserId))
                {
                    var resource = this.HRResources.FirstOrDefault(f => f.WebId == CurrentUserId);
                    if (resource != null)
                    {
                        var employee = this.Employees.FirstOrDefault(f => f.EmployeeId == resource.PREmp && f.PRCo == resource.PRCo);
                        var empComp = employee?.Division?.HQCompany;
                        if (empComp.HQCo != _CurrentCompany?.HQCo && _CurrentCompany != null)
                        {
                            _CurrentCompany = this.HQCompanyParms.FirstOrDefault(f => f.HQCo == _CurrentCompany.HQCo);
                        }
                        else
                        {
                            _CurrentCompany = empComp;
                        }
                    }
                    if (_CurrentCompany == null)
                        _CurrentCompany = this.HQCompanyParms.FirstOrDefault();
                }
            }
            
            return _CurrentCompany;
        }

        public WebUser? GetCurrentUser()
        {
            var user = new WebUser();
            if (!string.IsNullOrEmpty(CurrentUserId))
            {
                user = this.WebUsers.FirstOrDefault(f => f.Id == CurrentUserId);
            }

            return user;
        }

        public WebApplicant AddApplicant()
        {
			var app = new WebApplicant
			{
                db = this, 

				ApplicantId = this.GetNextId(WebApplicant.BaseTableName, 1),
				HRCo = 1,               
                
			};

			this.WebApplicants.Add(app);
            return app;
        }

        public WebApplicant AddApplicant(HRResource resource, bool removeResource = false)
        {
            var crossRef = this.budWA_CrossRef_Temp.FirstOrDefault(f => f.HRRef == resource.HRRef);
            WebApplicant applicant = null;
            applicant = this.WebApplicants.FirstOrDefault(f => f.SSN == resource.SSN);
            if (crossRef != null && applicant == null)
            {
                applicant = this.WebApplicants.FirstOrDefault(f => f.ApplicantId == crossRef.ApplicantId);
            }
            if (applicant == null)
            {
                var applicantId = crossRef?.ApplicantId ?? this.GetNextId(WebApplicant.BaseTableName);
                applicant = new WebApplicant()
                {
                    db = resource.db,

                    ApplicantId = applicantId,
                    UserEmail = resource.Email,
                    WebId = resource.WebId,
                    PRCo = resource.PRCo,
                    PREmployeeId = resource.PREmp,
                    LastName = resource.LastName,
                    FirstName = resource.FirstName,
                    MiddleName = resource.MiddleName,
                    Nickname = resource.Nickname,
                    Address = resource.Address,
                    City = resource.City,
                    State = resource.State,
                    Zip = resource.Zip,
                    Address2 = resource.Address2,
                    Phone = resource.Phone,
                    WorkPhone = resource.WorkPhone,
                    CellPhone = resource.CellPhone,
                    SSN = resource.SSN,
                    Sex = resource.Sex,
                    Race = resource.Race,
                    BirthDate = resource.BirthDate,
                    MaritalStatus = resource.MaritalStatus,
                    MaidenName = resource.MaidenName,
                    SpouseName = resource.SpouseName,
                    PassPort = resource.PassPort,
                    LicNumber = resource.LicNumber,
                    LicType = resource.LicType,
                    LicState = resource.LicState,
                    LicExpDate = resource.LicExpDate,
                    AvailableDate = resource.AvailableDate,
                    LastContactDate = resource.LastContactDate,
                    ContactPhone = resource.ContactPhone,
                    AltContactPhone = resource.AltContactPhone,
                    ExpectedSalary = resource.ExpectedSalary,
                    CurrentEmployer = resource.CurrentEmployer,
                    NoContactEmplYN = resource.NoContactEmplYN,
                    Notes = resource.Notes,
                    Email = resource.Email,
                    Suffix = resource.Suffix,
                    LicClass = resource.LicClass,
                    Country = resource.Country,
                    LicCountry = resource.LicCountry,
                    StatusCode = resource.StatusCode,
                    EmergencyContact = resource.EmergencyContact,
                    EmegencyRelationship = resource.EmegencyRelationship,
                    LicDenied = resource.LicDenied,
                    LicRevoked = resource.LicRevoked,
                    LicDeniedRevokedReason = resource.LicDeniedRevokedReason,
                    EmegencyContactPhone = resource.EmegencyContactPhone,
                    FailedDOTDrug = resource.FailedDOTDrug,
                    FaildDOTDrugDate = resource.FaildDOTDrugDate,
                    DOTSap = resource.DOTSap,
                    SAPDate = resource.SAPDate,
                    FelonyConviction = resource.FelonyConviction,
                    FelonyNote = resource.FelonyNote,
                    AppliedBeforeYN = resource.AppliedBeforeYN,
                    AppliedBeforeDate = resource.AppliedBeforeDate,
                    Referral = resource.Referral,
                    PortalAccountActive = resource.PortalAccountActive,

                    HRCo = resource.HRCo,
                };
                if (crossRef == null)
                {
                    this.budWA_CrossRef_Temp.Add(new budWA_CrossRef_Temp()
                    {
                        ApplicantId = applicant.ApplicantId,
                        HRRef = resource.HRRef
                    });
                }
                if (resource.HRRef < 900000)
                {
                    applicant.HRCo = resource.HRCo;
                    applicant.HRRefId = resource.HRRef;
                    
                }
                this.WebApplicants.Add(applicant);
            }
            else
            {
                applicant.LastName = resource.LastName;
                applicant.FirstName = resource.FirstName;
                applicant.MiddleName = resource.MiddleName;
                applicant.Nickname = resource.Nickname;
                applicant.Address = resource.Address;
                applicant.City = resource.City;
                applicant.State = resource.State;
                applicant.Zip = resource.Zip;
                applicant.Address2 = resource.Address2;
                applicant.Phone = resource.Phone;
                applicant.WorkPhone = resource.WorkPhone;
                applicant.CellPhone = resource.CellPhone;
                applicant.SSN = resource.SSN;
                applicant.Sex = resource.Sex;
                applicant.Race = resource.Race;
                applicant.BirthDate = resource.BirthDate;
                applicant.MaritalStatus = resource.MaritalStatus;
                applicant.MaidenName = resource.MaidenName;
                applicant.SpouseName = resource.SpouseName;
                applicant.PassPort = resource.PassPort;
                applicant.LicNumber = resource.LicNumber;
                applicant.LicType = resource.LicType;
                applicant.LicState = resource.LicState;
                applicant.LicExpDate = resource.LicExpDate;
                applicant.AvailableDate = resource.AvailableDate;
                applicant.LastContactDate = resource.LastContactDate;
                applicant.ContactPhone = resource.ContactPhone;
                applicant.AltContactPhone = resource.AltContactPhone;
                applicant.ExpectedSalary = resource.ExpectedSalary;
                applicant.CurrentEmployer = resource.CurrentEmployer;
                applicant.NoContactEmplYN = resource.NoContactEmplYN;
                applicant.Notes = resource.Notes;
                applicant.Email = resource.Email;
                applicant.Suffix = resource.Suffix;
                applicant.LicClass = resource.LicClass;
                applicant.Country = resource.Country;
                applicant.LicCountry = resource.LicCountry;
                applicant.StatusCode = resource.StatusCode;
                applicant.EmergencyContact = resource.EmergencyContact;
                applicant.EmegencyRelationship = resource.EmegencyRelationship;
                applicant.LicDenied = resource.LicDenied;
                applicant.LicRevoked = resource.LicRevoked;
                applicant.LicDeniedRevokedReason = resource.LicDeniedRevokedReason;
                applicant.EmegencyContactPhone = resource.EmegencyContactPhone;
                applicant.FailedDOTDrug = resource.FailedDOTDrug;
                applicant.FaildDOTDrugDate = resource.FaildDOTDrugDate;
                applicant.DOTSap = resource.DOTSap;
                applicant.SAPDate = resource.SAPDate;
                applicant.FelonyConviction = resource.FelonyConviction;
                applicant.FelonyNote = resource.FelonyNote;
                applicant.AppliedBeforeYN = resource.AppliedBeforeYN;
                applicant.AppliedBeforeDate = resource.AppliedBeforeDate;
                applicant.Referral = resource.Referral;
                applicant.PortalAccountActive = resource.PortalAccountActive;
				applicant.HRCo = resource.HRCo;
				if (resource.HRRef < 900000)
                {
                    applicant.HRRefId = resource.HRRef;
                }
            }

            foreach (var appliedPosition in resource.AppliedPositions)
            {
                var application = applicant.AddApplication(appliedPosition);
                foreach (var hrAccident in resource.TrafficAccidents)
                {
                    application.AddTrafficAccident(hrAccident);
                }
                foreach (var hrTicket in resource.TrafficTickets)
                {
                    application.AddTrafficTicket(hrTicket);
                }
                foreach (var employement in resource.WorkExperiences)
                {
                    if (!string.IsNullOrEmpty(employement.Employer))
                        application.AddWorkHistory(employement);
                }
                foreach (var hrDrivingExperiance in resource.DrivingExperiences)
                {
                    application.AddDrivingExperience(hrDrivingExperiance);
                }
                foreach (var durgTest in resource.DrugTests)
                {
                    application.AddDrugTest(durgTest);
                }
                application.AddPosition(appliedPosition.Position);

                application.tAccidents = application.TrafficIncidents.Where(f => f.IncidentType == WAIncidentTypeEnum.Accident).Any();
                application.tCitiations = application.TrafficIncidents.Where(f => f.IncidentType == WAIncidentTypeEnum.Citation).Any();

            }

            if (applicant.Applications.Any())
            {
                var appliedDate = applicant.Applications.Max(f => f.ApplicationDate);

                applicant.Applications
                    .Where(f => (f.Status == WAApplicationStatusEnum.Submitted ||
                                f.Status == WAApplicationStatusEnum.Draft ||
                                f.Status == WAApplicationStatusEnum.Started) &&
                                f.ApplicationDate < appliedDate
                    )
                    .ToList()
                    .ForEach(e =>
                    {
                        e.Status = WAApplicationStatusEnum.Canceled;
                    });

                var lastApp = applicant.Applications.First(f => f.ApplicationDate == appliedDate);
                if (applicant.HRRefId < 900000 && resource.ActiveYN == "Y")
                {
                    lastApp.Status = WAApplicationStatusEnum.Hired;
                }
            }

            if (removeResource)
            {
                this.HRResources.Remove(resource);
                applicant.HRRefId = null;
                applicant.HRResource = null;
            }
            return applicant;
        }

    }
}
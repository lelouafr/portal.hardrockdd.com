using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WebApplicant
    {
        public static string BaseTableName { get { return "budWAAM"; } }

        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
			{
				_db ??= VPContext.GetDbContextFromEntity(this);
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
        private WAApplicationStatusEnum ConvertResourceStatusToEnum(HRResource resource)
        {
            switch (resource.Status)
            {
                case "A":
                    return WAApplicationStatusEnum.Hired;
                    //if (resource.ActiveYN == "Y")
                    //    return WAApplicationStatusEnum.Hired;
                    //else
                    //    return WAApplicationStatusEnum.Submitted;
                case "APP":
                    return WAApplicationStatusEnum.Submitted;
                case "APPDND":
                    return WAApplicationStatusEnum.Denied;
                case "APPDRAFT":
                    return WAApplicationStatusEnum.Draft;
                case "DECLINE":
                case "FAIL":
                case "NH":
                case "NS":
                case "LS":
                    return WAApplicationStatusEnum.Denied;
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
                    return WAApplicationStatusEnum.Draft;
            }
        }

        private WAApplicationDeniedReasonEnum? ConvertResourceDeniedReasonToEnum(string resourceStatus)
        {
            switch (resourceStatus)
            {
                case "A":
                case "APP":
                case "APPDRAFT":
                    return null;
                case "APPDND":
                    return WAApplicationDeniedReasonEnum.ApplicationDenied;
                case "DECLINE":
                    return WAApplicationDeniedReasonEnum.DeniedJob;
                case "FAIL":
                    return WAApplicationDeniedReasonEnum.FailedDrugTest;
                case "NH":
                    return WAApplicationDeniedReasonEnum.NotHired;
                case "NS":
                    return WAApplicationDeniedReasonEnum.NoShow;
                case "LS":
                    return WAApplicationDeniedReasonEnum.SuspendedLicense;
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

		public WebApplication AddApplication(HRPosition appliedPosition)
		{
			var application = this.Applications.FirstOrDefault(f => f.ApplicationDate >= DateTime.Now.Date.AddDays(-7) && (f.Status == WAApplicationStatusEnum.Draft || f.Status == WAApplicationStatusEnum.Submitted));
			if (application == null)
			{
				application = new WebApplication
				{
					Applicant = this,
					db = this.db,
					ApplicantId = this.ApplicantId,
					ApplicationId = this.Applications.DefaultIfEmpty().Max(max => max == null ? 0 : max.ApplicationId) + 1,
					ApplicationDate = DateTime.Now.Date,
					ActiveYN = "Y",
					WFCo = 1,
                    HRCo = this.HRCo,
				};
				application.ExpireDate = application.ApplicationDate.AddYears(1);

				Applications.Add(application);
			}
			Applications
				.Where(f => (f.Status == WAApplicationStatusEnum.Submitted ||
							f.Status == WAApplicationStatusEnum.Draft ||
							f.Status == WAApplicationStatusEnum.Started) &&
							f.ApplicationDate < application.ApplicationDate
				)
				.ToList()
				.ForEach(e =>
				{
					e.Status = WAApplicationStatusEnum.Canceled;
				});
			
			application.AddPosition(appliedPosition);
            
            if (!application.WorkExperiences.Any())
            {
				application.AddWorkHistory();
				application.AddWorkHistory();
				application.AddWorkHistory();

				if (!application.DrivingExperiences.Any())
				{
					application.AddDrivingExperience();
					application.AddDrivingExperience();
					application.AddDrivingExperience();
				}
				if (!application.TrafficIncidents.Any())
				{
					application.AddTrafficTicket();
					application.AddTrafficTicket();
					application.AddTrafficTicket();

					application.AddTrafficAccident();
					application.AddTrafficAccident();
					application.AddTrafficAccident();
				}
			}
			return application;
		}

		public WebApplication AddApplication(HRAppliedPosition appliedPosition)
        {
            appliedPosition.ApplyDate ??= appliedPosition.Resource.ApplicationDate;
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
                .Where(f => (f.Status == WAApplicationStatusEnum.Submitted ||
                            f.Status == WAApplicationStatusEnum.Draft ||
                            f.Status == WAApplicationStatusEnum.Started ) &&
                            f.ApplicationDate < appliedDate
                )
                .ToList()
                .ForEach(e =>
                {
                    e.Status = WAApplicationStatusEnum.Canceled;
                });
            if (application.Status != WAApplicationStatusEnum.Submitted &&
                application.Status != WAApplicationStatusEnum.Approved &&
                application.Status != WAApplicationStatusEnum.Shelved)
            {
                application.Status = ConvertResourceStatusToEnum(appliedPosition.Resource);
                application.DeniedReason = ConvertResourceDeniedReasonToEnum(appliedPosition.Resource.Status);
            }
            application.AddPosition(appliedPosition);

            return application;
        }

        public WebApplication AddApplication()
        {
            var application = this.Applications.FirstOrDefault(f => f.Status == WAApplicationStatusEnum.Draft);
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


            return applicant;
        }

        private WebApplication? _app;
        public WebApplication? CurrentApplication()
        {
            if (_app == null)
                _app = this.Applications
                    .Where(f => f.Status == WAApplicationStatusEnum.Submitted || f.Status == WAApplicationStatusEnum.Approved)
                    .OrderByDescending(s => s.ApplicationId)
                    .FirstOrDefault();

            if (_app == null)
                _app = this.Applications
                    .OrderByDescending(s => s.ApplicationId)
                    .FirstOrDefault();
            return _app;
        }

        public SelectListItem SelectListItem()
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
                fullName = string.Format("{0} {1}", firstName, this.LastName);
            }
            else
            {
                if (this.FirstName.ToLower(VPContext.AppCultureInfo).Contains(this.MiddleName.ToLower(VPContext.AppCultureInfo)) ||
                    this.FirstName.ToLower(VPContext.AppCultureInfo).Contains(this.MiddleName.ToLower(VPContext.AppCultureInfo)))
                {
                    fullName = string.Format("{0} {1}", firstName, this.LastName);
                }
                else
                {
                    fullName = string.Format("{0} {1} {2}", firstName, this.MiddleName, this.LastName);
                }
            }

            if (!string.IsNullOrEmpty(this.Suffix))
            {
                fullName = string.Format("{0} {1}", fullName, this.Suffix);
            }

            return fullName;

        }

    }
}
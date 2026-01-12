//using portal.Repository.VP.AP.CreditCard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Employee
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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        private byte? _PortalCompanyCode;
        public byte PortalCompanyCode
        {
            get
            {
                if (_PortalCompanyCode == null)
                {
                    _PortalCompanyCode = GLCo ?? PRCo;
                }

                return (byte)_PortalCompanyCode;
            }
            set
            {
                _PortalCompanyCode = value;
            }
        }


        private string _FullName;
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_FullName))
                {
                    _FullName = this.FullName(false);
                }
                return _FullName;
            }
        }

        public HRResource HREmployee 
        {
            get
            {
                return Resource.FirstOrDefault();
            }
        }

		public int DivisionId
		{
			get
			{
				return tDivisionId ?? 1;
			}
			set
			{
				if (tDivisionId != value)
				{
					var divHist = new PREmployeeDivision()
					{
						PRCo = this.PRCo,
						EmployeeId = this.EmployeeId,
						PREmployee = this,
						SeqId = this.Divisions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
						DivisionId = tDivisionId,
                        
						StartDate = DateTime.Now.Date,
					};

                    var division = this.db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == value);
                    if (division != null)
                    {
                       this.GLCo = division.WPDivision.GLCo;
                    }

					this.Divisions.Add(divHist);
					PREmployeeDivision prior = null;
					this.Divisions.ToList().ForEach(e => {
						if (e.EndDate == null && e.SeqId != divHist.SeqId)
						{
							e.EndDate = prior?.EndDate;
						}
						prior = e;
					});
					tDivisionId = value;
				}
			}
		}


		public string CrewId
		{
			get
			{
				return tCrewId;
			}
			set
			{
				if (tCrewId != value)
				{
					var crewHist = new PREmployeeCrew()
					{
						PRCo = this.PRCo,
						EmployeeId = this.EmployeeId,
						PREmployee = this,
						SeqId = this.Crews.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
						CrewId = tCrewId,
						StartDate = DateTime.Now.Date,
					};

					this.Crews.Add(crewHist);
					PREmployeeCrew prior = null;
					this.Crews.ToList().ForEach(e => {
						if (e.EndDate == null && e.SeqId != crewHist.SeqId)
						{
							e.EndDate = prior?.EndDate;
						}
						prior = e;
					});
					tCrewId = value;
				}
			}
		}

		public CreditCardImage AddCreditCardImage(HttpPostedFileBase fileUpload, DateTime mth)
        {
            if (fileUpload == null)
                return null;

            var isThumbnail = fileUpload.FileName.ToLower().Contains("thumbnail");
            var filename = fileUpload.FileName;
            if (filename.Contains("image") && !isThumbnail)
                filename = string.Format("{0}-{1}", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5), filename);
            var name = Path.GetFileNameWithoutExtension(filename);
            if (isThumbnail)
                filename = name.Replace("_THUMBNAIL", "");


            var image = this.CreditCardImages.FirstOrDefault(f => f.Mth == mth && f.ImageName.StartsWith(filename));
            if (image == null)
            {
                image = new CreditCardImage
                {
                    CCCo = PRCo,
                    Mth = mth,
                    EmployeeId = EmployeeId,
                    ImageId = CreditCardImages.DefaultIfEmpty().Max(f => f == null ? 0 : f.ImageId) + 1,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    ImageName = filename,
                    db = this.db,
                };
                this.CreditCardImages.Add(image);
            }
            var file = image.Attachment.GetRootFolder().AddFile(fileUpload);
            if (isThumbnail)
                image.ThumbAttachmentId = file.AttachmentId;
            else
                image.AttachmentId = file.AttachmentId;
            return image;
        }

        public Job CurrentJob(DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now.Date;

            var dtJobEmployee = db.DailyJobEmployees
                .Include("DailyTicket")
                .Include("DailyTicket.DailyJobTicket")
                .Include("DailyTicket.DailyJobTicket.Job")
                .FirstOrDefault(f => f.tEmployeeId == this.EmployeeId && 
                                     f.tWorkDate == date &&
                                     f.DailyTicket.DailyJobTicket.Job != null);
            if (dtJobEmployee == null)
                return null;
            
            if (dtJobEmployee?.DailyTicket?.DailyJobTicket?.Job != null)
            {
                return dtJobEmployee.DailyTicket.DailyJobTicket.Job;
            }
            return null;
        }

        public HRResourceBenefit AddBenefit(PRImportBenefit importLine)
        {
            var benefit = this.HREmployee.AddBenefit(importLine);
            return benefit;
        }

        public HRActiveStatusEnum Status { get => ActiveYN == "Y" ? HRActiveStatusEnum.Active : HRActiveStatusEnum.Inactive; set => ActiveYN = value == HRActiveStatusEnum.Inactive ? "N" : "Y"; }

        public List<Employee> Supervisors()
        {

            var supList = new List<Employee>();
            var sup = Supervisor;
            if (sup == null)
                return supList;
            supList.Add(sup);
            while (!supList.Any(f => f.EmployeeId == sup.ReportsToId))
            {
                sup = sup.Supervisor;
                if (sup == null)
                    return supList;
                supList.Add(sup);
            }
            return supList;
        }

        public bool IsSupervisor(int supervisorId)
        {
            return Supervisors().Any(f => f.EmployeeId == supervisorId);
        }

        private List<DB.Infrastructure.ViewPointDB.Data.Job> _JobList;
        public List<DB.Infrastructure.ViewPointDB.Data.Job> JobList(DateTime? minDate = null)
        {
            if (_JobList == null)
            {
                var memKey = "EmployeeJobList";
                memKey = string.Format("{0}_{1}", memKey, EmployeeId);
                if (MemoryCache.Default[memKey] is List<Job> list)
                {
                    _JobList = list;
                }
                else
                {
                    //minDate = DateTime.Now.AddYears(-1);
                    if (minDate == null)
                        minDate = DateTime.Now.AddMonths(-2);
                    _JobList = db.DTPayrollHours.Where(f => f.PRCo == PRCo &&
                                                 f.EmployeeId == EmployeeId &&
                                                 f.Job != null &&
                                                 f.WorkDate >= minDate)
                                    .GroupBy(g => g.Job)
                                    .Select(s => s.Key)
                                    .ToList();


                    ObjectCache systemCache = MemoryCache.Default;
                    CacheItemPolicy policy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
                    };

                    systemCache.Set(memKey, _JobList, policy);
                }
            }

            return _JobList;
        }
    }

    public static class EmployeeExtension
    {



        public static string FullName(this Employee employee, bool includeMiddle = true)
        {
            if (employee == null) return ""; ;
            string fullName;
            string firstName = string.IsNullOrEmpty(employee.Nickname) ? employee.FirstName : employee.Nickname;
            if (string.IsNullOrEmpty(employee.MidName) || includeMiddle == false)
            {
                fullName = string.Format("{0} {1}", firstName, employee.LastName);
            }
            else
            {
                if (employee.FirstName.ToLower(VPContext.AppCultureInfo).Contains(employee.MidName.ToLower(VPContext.AppCultureInfo)) || 
                    employee.FirstName.ToLower(VPContext.AppCultureInfo).Contains(employee.MidName.ToLower(VPContext.AppCultureInfo)))
                {
                    fullName = string.Format("{0} {1}", firstName, employee.LastName);
                }
                else
                {
                    fullName = string.Format("{0} {1} {2}", firstName, employee.MidName, employee.LastName);
                }
            }

            if (!string.IsNullOrEmpty(employee.Suffix))
            {
                fullName = string.Format("{0} {1}", fullName, employee.Suffix);
            }

            return fullName;

        }



    }
}
using portal.Repository.VP.AP.CreditCard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Employee
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

        public CreditCardImage AddCreditCardImage(HttpPostedFileBase fileUpload, DateTime mth)
        {
            if (fileUpload == null)
                return null;

            var isThumbnail = fileUpload.FileName.ToLower().Contains("thumbnail");
            var filename = fileUpload.FileName;
            if (filename.Contains("image") && !isThumbnail)
                filename = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5), filename);
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
                    CreatedBy = StaticFunctions.GetUserId(),
                    CreatedOn = DateTime.Now,
                    ImageName = filename,
                    db = this.db,
                };
                this.CreditCardImages.Add(image);
            }
            var file = image.Attachment.AddFile(fileUpload);
            if (isThumbnail)
                image.ThumbAttachmentId = file.AttachmentId;
            else
                image.AttachmentId = file.AttachmentId;
            return image;
        }

        public Job? CurrentJob(DateTime? date = null)
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }

            var dtJobEmployee = db.DailyJobEmployees.FirstOrDefault(f => f.tEmployeeId == this.EmployeeId && f.tWorkDate == date);
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
    }

    public static class EmployeeExtension
    {

        public static List<Employee> Supervisors(this Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            var supList = new List<Employee>();
            var sup = employee.Supervisor;
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

        public static bool IsSupervisor(this Employee employee, int supervisorId)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            return employee.Supervisors().Any(f => f.EmployeeId == supervisorId);
        }

        public static string FullName(this Employee employee, bool includeMiddle = true)
        {
            if (employee == null) return ""; ;
            string fullName;
            string firstName = string.IsNullOrEmpty(employee.Nickname) ? employee.FirstName : employee.Nickname;
            if (string.IsNullOrEmpty(employee.MidName) || includeMiddle == false)
            {
                fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", firstName, employee.LastName);
            }
            else
            {
                if (employee.FirstName.ToLower(AppCultureInfo.CInfo()).Contains(employee.MidName.ToLower(AppCultureInfo.CInfo())) || 
                    employee.FirstName.ToLower(AppCultureInfo.CInfo()).Contains(employee.MidName.ToLower(AppCultureInfo.CInfo())))
                {
                    fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", firstName, employee.LastName);
                }
                else
                {
                    fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1} {2}", firstName, employee.MidName, employee.LastName);
                }
            }

            if (!string.IsNullOrEmpty(employee.Suffix))
            {
                fullName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", fullName, employee.Suffix);
            }

            return fullName;

        }

        public static DB.HRActiveStatusEnum Status(this Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            return employee.ActiveYN == "Y" ? DB.HRActiveStatusEnum.Active : DB.HRActiveStatusEnum.Inactive;
        }


    }
}
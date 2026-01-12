using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.HQ;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views
{
    public class LoggedInUserViewModel
    {
        public LoggedInUserViewModel()
        {

        }

        public LoggedInUserViewModel(HRResource model)
        {
            if (model == null)
            {
                return;
                //throw new System.ArgumentNullException(nameof(model));
            }
            HRCo = model.HRCo;
            HRRef = model.HRRef;
            LastName = model.LastName;
            FirstName = model.FirstName;
            MiddleName = model.MiddleName;
            Suffix = model.Suffix;

            Email = model.CompanyEmail;
            if (model.UniqueAttchID != null)
            {
                using var fileRepo = new FileRepository();
                var file = fileRepo.GetFiles((Guid)model.UniqueAttchID).Where(f => f.AttachmentTypeID == 50005).FirstOrDefault();// && f.OrigFileName == fileName

                if (file != null)
                {
                    using var attchRepo = new AttachmentRepository();
                    var attchment = attchRepo.GetAttachment(file.AttachmentId);

                    ProfileImage = attchment.AttachmentData;
                }
            }

            if (model.Position != null)
            {
                JobTitle = model.Position.JobTitle;
            }

        }

        public LoggedInUserViewModel(DB.Infrastructure.ViewPointDB.Data.Employee model)
        {
            if (model == null)
            {
                return;
                //throw new System.ArgumentNullException(nameof(model));
            }
            var resource = model.Resource.FirstOrDefault();
            HRCo = model.PRCo;
            HRRef = model.EmployeeId;
            LastName = model.LastName;
            FirstName = model.FirstName;
            MiddleName = model.MidName;
            Suffix = model.Suffix;
            Email = resource.CompanyEmail;

            if (model.UniqueAttchID != null)
            {
                using var fileRepo = new FileRepository();
                var file = fileRepo.GetFiles((Guid)model.UniqueAttchID).Where(f => f.AttachmentTypeID == 50005).FirstOrDefault();// && f.OrigFileName == fileName

                if (file != null)
                {
                    using var attchRepo = new AttachmentRepository();
                    var attchment = attchRepo.GetAttachment(file.AttachmentId);

                    ProfileImage = attchment.AttachmentData;
                }
            }

            if (resource != null)
            {
                JobTitle = resource.Position.JobTitle;
            }

        }


        [Required]
        [Display(Name = "HR Co")]
        public byte HRCo { get; set; }

        [Required]
        [Display(Name = "HR Ref")]
        public int HRRef { get; set; }

        [Display(Name = "PR Co")]
        public byte? PRCo { get; set; }

        [Display(Name = "PR Emp")]
        public int? PREmp { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Suffix")]
        public string Suffix { get; set; }

        public string FullName
        {
            get
            {
                var result = FirstName;
                //result += MiddleName != string.Empty ? " " + MiddleName : "";
                result += " " + LastName;
                result += !string.IsNullOrEmpty(Suffix) ? " " + Suffix : "";

                return result;
            }
        }

        public string FileName { get; set; }

        public string ProfileUri
        {
            get
            {
                if (ProfileImage == null)
                {
                    return null;
                }
                return "data:image/png;base64," + Convert.ToBase64String(ProfileImage);
            }
            set
            {
                var dataUri = value;
                if (dataUri != null)
                {
                    var encodedImage = dataUri.Split(',')[1];
                    var decodedImage = Convert.FromBase64String(encodedImage);
                    ProfileImage = decodedImage;
                }
                else
                {
                    ProfileImage = null;
                }
            }
        }

        [HiddenInput]
        public byte[] ProfileImage { get; set; }

        public string JobTitle { get; set; }


    }
}
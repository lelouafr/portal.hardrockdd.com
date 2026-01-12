using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Safety.Models.Safety
{
    public class EmployeeInfoViewModel
    {
        public EmployeeInfoViewModel()
        {

        }

        public EmployeeInfoViewModel(DB.Infrastructure.ViewPointDB.Data.HRResource resource)
        {
            if (resource == null)
                return;

            HRCo = resource.HRCo;
            HRRef = resource.HRRef;
            SSN = resource.SSN;
            FullName = resource.FullName();
            NickName = resource.Nickname;
            FirstName = resource.FirstName;
            LastName = resource.LastName;
            MiddleName = resource.MiddleName;
            Suffix = resource.Suffix;
            BirthDate = resource.BirthDate;



            LicNumber = resource.LicNumber;
            LicType = resource.LicType;
            LicState = resource.LicState;
            LicExpDate = resource.LicExpDate;
            LicClass = resource.LicClass;
            LicCountry = resource.LicCountry;
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int HRRef { get; set; }

        [UIHint("SSNBox")]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Suffix")]
        public string Suffix { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "License #")]
        public string LicNumber { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "License Type")]
        public string LicType { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "State")]
        public string LicState { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Exp Date")]
        public DateTime? LicExpDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "License Class")]
        public string LicClass { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "License Country")]
        public string LicCountry { get; set; }
    }
}
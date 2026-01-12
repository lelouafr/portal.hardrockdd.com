using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class ApplicantInfoViewModel
    {
        public ApplicantInfoViewModel()
        {

        }

        public ApplicantInfoViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;
            FirstName = applicant.FirstName;
            LastName = applicant.LastName;
            MiddleName = applicant.MiddleName;
            Nickname = applicant.Nickname;
            Address = applicant.Address;
            Address2 = applicant.Address2;
            City = applicant.City;
            State = applicant.State;
            Zip = applicant.Zip;
            Phone = applicant.Phone;
            WorkPhone = applicant.WorkPhone;
            CellPhone = applicant.CellPhone;
            SSN = applicant.SSN;
            Sex = applicant.Sex;
            Race = applicant.Race;
            BirthDate = applicant.BirthDate;
            Email = applicant.Email;
            EmergencyContact = applicant.EmergencyContact;
            EmegencyRelationship = applicant.EmegencyRelationship;
            ContactPhone = applicant.ContactPhone;
            EmegencyContactPhone = applicant.EmegencyContactPhone;
            ApplicantId = applicant.ApplicantId;
            LicNumber = applicant.LicNumber;
            LicType = applicant.LicType;
            LicState = applicant.LicState;
            LicExpDate = applicant.LicExpDate;
            LicClass = applicant.LicClass;
            LicCountry = applicant.LicCountry;

            Notes = applicant.Notes;

            Actions = new ActionViewModel(applicant.CurrentApplication());
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Middle")]
        public string MiddleName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Nick name")]
        public string Nickname { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo='1'")]
        public string State { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Required]
        [UIHint("EmailBox")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [UIHint("PhoneBox")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Work Phone")]
        public string WorkPhone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Cell Phone")]
        public string CellPhone { get; set; }

        [Required]
        [UIHint("SSN")]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [UIHint("DropBox")]
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [UIHint("DropBox")]
        [Display(Name = "Race")]
        public string Race { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "BirthDate")]
        public DateTime? BirthDate { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Relationship")]
        public string EmegencyRelationship { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Contact Phone")]
        public string EmegencyContactPhone { get; set; }


        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public string LicNumber { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Type")]
        public string LicType { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Country")]
        public string LicCountry { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo='1'")]
        public string LicState { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Exp Date")]
        public DateTime? LicExpDate { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Class")]
        public string LicClass { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Notes")]
        [Field(TextSize = 12, LabelSize = 0)]
        public string Notes { get; set; }

        public ActionViewModel Actions { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(FirstName))
                modelState.AddModelError("FirstName", "First Name may not be blank");

            if (string.IsNullOrEmpty(LastName))
                modelState.AddModelError("LastName", "Last Name may not be blank");

            if (string.IsNullOrEmpty(Address))
                modelState.AddModelError("Address", "Address may not be blank");

            if (string.IsNullOrEmpty(City))
                modelState.AddModelError("City", "City may not be blank");

            if (string.IsNullOrEmpty(State))
                modelState.AddModelError("State", "State may not be blank");

            if (string.IsNullOrEmpty(Zip))
                modelState.AddModelError("Zip", "Zip may not be blank");

            if (string.IsNullOrEmpty(Phone))
                modelState.AddModelError("Phone", "Phone may not be blank");

            if (BirthDate == null)
                modelState.AddModelError("BirthDate", "Birth Date may not be blank");

            if (string.IsNullOrEmpty(LicNumber))
                modelState.AddModelError("LicNumber", "License Number may not be blank");

            if (string.IsNullOrEmpty(LicState))
                modelState.AddModelError("LicState", "License State may not be blank");

            if (LicExpDate == null)
                modelState.AddModelError("LicExpDate", "License Expire Date may not be blank");

            if (string.IsNullOrEmpty(LicType))
                modelState.AddModelError("LicType", "License Type may not be blank");


        }

        internal ApplicantInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == this.ApplicantId);

            if (updObj != null)
            {
                updObj.FirstName = this.FirstName;
                updObj.LastName = this.LastName;
                updObj.MiddleName = this.MiddleName;
                updObj.Nickname = this.Nickname;
                updObj.Address = this.Address;
                updObj.Address2 = this.Address2;
                updObj.City = this.City;
                updObj.State = this.State;
                updObj.Zip = this.Zip;
                updObj.Phone = this.Phone;
                updObj.Email = this.Email;
                updObj.WorkPhone = this.WorkPhone;
                updObj.CellPhone = this.CellPhone;
                if (!string.IsNullOrEmpty(this.SSN)) 
                    updObj.SSN = this.SSN;
                updObj.Sex = this.Sex;
                updObj.Race = this.Race;
                updObj.BirthDate = this.BirthDate;
                updObj.EmergencyContact = this.EmergencyContact;
                updObj.EmegencyRelationship = this.EmegencyRelationship;
                updObj.ContactPhone = this.ContactPhone;
                updObj.EmegencyContactPhone = this.EmegencyContactPhone;
                updObj.LicNumber = this.LicNumber;
                updObj.LicType = this.LicType;
                updObj.LicState = this.LicState;
                updObj.LicExpDate = this.LicExpDate;
                updObj.LicClass = this.LicClass;
                updObj.LicCountry = this.LicCountry;

                try
                {
                    db.BulkSaveChanges();
                    return new ApplicantInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

    }
}
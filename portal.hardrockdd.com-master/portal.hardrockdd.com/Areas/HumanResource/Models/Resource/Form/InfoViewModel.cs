using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class InfoViewModel : portal.Models.Views.AuditBaseViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(HRResource resource) : base(resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;

            FullName = resource.FullName();
            FirstName = resource.FirstName;
            NickName = resource.Nickname;
            LastName = resource.LastName;
            MiddleName = resource.MiddleName;
            Suffix = resource.Suffix;

            Address = resource.Address;
            Address2 = resource.Address2;
            City = resource.City;
            State = resource.State;
            Zip = resource.Zip;

            Email = resource.Email;
            Phone = resource.Phone;
            WorkPhone = resource.WorkPhone;
            CellPhone = resource.CellPhone;
            //SSN = resource.SSN;
            Sex = resource.Sex;
            Race = resource.Race;
            BirthDate = resource.BirthDate;


            EmergencyContact = resource.EmergencyContact;
            EmegencyContactPhone = resource.EmegencyContactPhone;
            EmegencyRelationship = resource.EmegencyRelationship;
                
            var uniqueAttchID = resource.UniqueAttchID ?? resource.PREmployee.UniqueAttchID;
            if (uniqueAttchID != null)
            {
                using var db = new VPContext();
                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID && f.AttachmentTypeID == 50005);
                ProfileAttachmentId = file?.AttachmentId;
            }
            
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        [Field(LabelSize = 4, TextSize = 8)]
        public int ResourceId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string FullName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Nick Name")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string NickName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "First Name")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string FirstName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Last Name")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string LastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Middle Name")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string MiddleName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Suffix")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Suffix { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Address")]
        [Field (LabelSize = 4, TextSize = 8)]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Add'l Address")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Address2 { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string City { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "State")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Zip { get; set; }

        [UIHint("EmailBox")]
        [Display(Name = "Email")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Email { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Phone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Work Phone")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string WorkPhone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Pager")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Pager { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Cell Phone")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string CellPhone { get; set; }

        [UIHint("SSNBox")]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Race")]
        public string Race { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "BirthDate")]
        [Field(LabelSize = 4, TextSize = 8)]
        public DateTime? BirthDate { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Contact")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string EmergencyContact { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Contact #")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string EmegencyContactPhone { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Relationship")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string EmegencyRelationship { get; set; }

        public int? ProfileAttachmentId { get; set; }

        internal InfoViewModel ProcessUpdate(ModelStateDictionary modelState, VPContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == this.ResourceId);

            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.FirstName = this.FirstName;
                updObj.Nickname = this.NickName;
                updObj.LastName = this.LastName;
                updObj.MiddleName = this.MiddleName;
                updObj.Suffix = this.Suffix;

                updObj.Address = this.Address;
                updObj.Address2 = this.Address2;
                updObj.City = this.City;
                updObj.State = this.State;
                updObj.Zip = this.Zip;

                updObj.Email = this.Email;
                updObj.Phone = this.Phone;
                updObj.BirthDate = this.BirthDate;
                updObj.WorkPhone = this.WorkPhone;
                updObj.CellPhone = this.CellPhone;

                updObj.EmergencyContact = this.EmergencyContact;
                updObj.EmegencyRelationship = this.EmegencyRelationship;
                updObj.EmegencyContactPhone = this.EmegencyContactPhone;
                try
                {
                    db.SaveChanges(modelState);
                    return new InfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }

    }
}
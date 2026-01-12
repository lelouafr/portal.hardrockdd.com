using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource.Form
{
    public class ResourceDrivingInfoViewModel
    {
        public ResourceDrivingInfoViewModel()
        {

        }

        public ResourceDrivingInfoViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            EmployeeId = resource.HRRef;

            LicNumber = resource.LicNumber;
            LicType = resource.LicType;
            LicState = resource.LicState;
            LicExpDate = resource.LicExpDate;
            LicClass = resource.LicClass;
            LicCountry = resource.LicCountry;

            DOTStatus = resource.udFMCSAPHMSA;
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

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

        [UIHint("TextBox")]
        [Display(Name = "DOT Status")]
        public string DOTStatus { get; set; }

        public ResourceDrivingInfoViewModel ProcessUpdate(ModelStateDictionary modelState, VPEntities db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.HRResources.Where(f => f.HRCo == this.HRCo && f.HRRef == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.LicNumber = this.LicNumber;
                updObj.LicType = this.LicType;
                updObj.LicState = this.LicState;
                updObj.LicExpDate = this.LicExpDate;
                updObj.LicClass = this.LicClass;
                updObj.LicCountry = this.LicCountry;

                updObj.udFMCSAPHMSA = this.DOTStatus;

                db.SaveChanges(modelState);
            }
            return this;
        }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class EquipmentHistoryListViewModel
    {
        public EquipmentHistoryListViewModel()
        {

        }

        public EquipmentHistoryListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.DrivingExperiences.Select(s => new EquipmentHistoryViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }


        public List<EquipmentHistoryViewModel>? List { get; }
    }

    public class EquipmentHistoryViewModel
    {
        public EquipmentHistoryViewModel()
        {

        }

        public EquipmentHistoryViewModel(DB.Infrastructure.ViewPointDB.Data.WADrivingExperience experience)
        {
            if (experience == null)
                return;

            ApplicantId = experience.ApplicantId;
            ApplicationId = experience.ApplicationId;
            SeqId = experience.SeqId;
            TypeOfEquipment = experience.TypeOfEquipment;
            FromDate = experience.FromDate;
            ToDate = experience.ToDate;
            Miliage = experience.Miliage;

        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Equipment Type")]
        public string TypeOfEquipment { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Required]
        [UIHint("LongBox")]
        [Display(Name = "Miliage/Hour")]
        public long? Miliage { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (TypeOfEquipment == null) modelState.AddModelError("TypeOfEquipment", "Equipment may not be blank");
            if (FromDate == null) modelState.AddModelError("FromDate", "From Date may not be blank");
            if (ToDate == null) modelState.AddModelError("ToDate", "To Date may not be blank");
            if (Miliage == null) modelState.AddModelError("Miliage", "Miliage/Hours may not be blank");
        }

        internal EquipmentHistoryViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WADrivingExperiences.FirstOrDefault(f => f.ApplicantId == this.ApplicantId &&
                                                                     f.ApplicationId == this.ApplicationId &&
                                                                     f.SeqId == this.SeqId);
            

            if (updObj != null)
            {
                this.Validate(modelState);

                updObj.TypeOfEquipment = this.TypeOfEquipment;
                updObj.FromDate = this.FromDate;
                updObj.ToDate = this.ToDate;
                updObj.Miliage = this.Miliage;
                try
                {
                    db.BulkSaveChanges();
                    return new EquipmentHistoryViewModel(updObj);
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
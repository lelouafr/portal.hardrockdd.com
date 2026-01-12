using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class TrafficAccidentListViewModel
    {
        public TrafficAccidentListViewModel()
        {

        }

        public TrafficAccidentListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.TrafficIncidents.Where(f => f.IncidentType == DB.WAIncidentTypeEnum.Accident).Select(s => new TrafficAccidentViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        public List<TrafficAccidentViewModel> List { get; }
    }

    public class TrafficAccidentViewModel
    {
        public TrafficAccidentViewModel()
        {

        }

        public TrafficAccidentViewModel(DB.Infrastructure.ViewPointDB.Data.WATrafficIncident incident)
        {
            if (incident == null)
                return;


            ApplicantId = incident.ApplicantId;
            ApplicationId = incident.ApplicationId;
            SeqId = incident.SeqId;
            IncidentType = incident.IncidentType;
            IncidentDate = incident.IncidentDate;
            Description = incident.Description;
            StateId = incident.StateId;
            Location = incident.Location;
            AtFaultTypeId = incident.AtFaultTypeId;
            AnyoneInjured = incident.AnyoneInjured ?? false;
            NumberOfInjuries = incident.NumberOfInjuries;
            CitationIssued = incident.CitationIssued ?? false;
        }

        [Key]
        public int ApplicantId { get; set; }

        [Key]
        public int ApplicationId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.WAIncidentTypeEnum IncidentType { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public DateTime? IncidentDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        public string StateId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Fault Type")]
        public byte? AtFaultTypeId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Anyone Injured?")]
        public bool? AnyoneInjured { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Number Of Injuries")]
        public byte? NumberOfInjuries { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Citation Issued?")]
        public bool? CitationIssued { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (IncidentDate == null) modelState.AddModelError("IncidentDate", "Incident Date may not be blank");
            if (string.IsNullOrEmpty(Description)) modelState.AddModelError("Description", "Description may not be blank");
            if (string.IsNullOrEmpty(StateId)) modelState.AddModelError("StateId", "State may not be blank");
            if (string.IsNullOrEmpty(Location)) modelState.AddModelError("Location", "Employer may not be blank");
            if (AtFaultTypeId == null) modelState.AddModelError("AtFaultTypeId", "At Fault Type may not be blank");
            if (AnyoneInjured == null) modelState.AddModelError("AnyoneInjured", "Anyone Injured may not be blank");
            if (AnyoneInjured == true && NumberOfInjuries == null) modelState.AddModelError("NumberOfInjuries", "Number of Injured may not be blank");
            if (CitationIssued == null) modelState.AddModelError("CitationIssued", "Citation Issued may not be blank");
        }

        internal TrafficAccidentViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WATrafficIncidents.FirstOrDefault(f => f.ApplicantId == this.ApplicantId &&
                                                                  f.ApplicationId == this.ApplicationId &&
                                                                  f.SeqId == this.SeqId);

            if (updObj != null)
            {
                this.Validate(modelState);

                updObj.IncidentDate = this.IncidentDate;
                updObj.Description = this.Description;
                updObj.StateId = this.StateId;
                updObj.Location = this.Location;
                updObj.AtFaultTypeId = this.AtFaultTypeId;
                updObj.AnyoneInjured = this.AnyoneInjured;
                updObj.NumberOfInjuries = this.NumberOfInjuries;
                updObj.CitationIssued = this.CitationIssued;
                try
                {
                    db.BulkSaveChanges();
                    return new TrafficAccidentViewModel(updObj);
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
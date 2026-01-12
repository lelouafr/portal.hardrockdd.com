using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class TrafficCitationListViewModel
    {
        public TrafficCitationListViewModel()
        {

        }

        public TrafficCitationListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.TrafficIncidents.Where(f => f.IncidentType == DB.WAIncidentTypeEnum.Citation).Select(s => new TrafficCitationViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }

        [Key]
        public int ApplicationId { get; set; }

        public List<TrafficCitationViewModel> List { get; }
    }

    public class TrafficCitationViewModel
    {
        public TrafficCitationViewModel()
        {

        }

        public TrafficCitationViewModel(DB.Infrastructure.ViewPointDB.Data.WATrafficIncident incident)
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
            CitationIssued = incident.CitationIssued;
            CitationType = incident.CitationType;
            LicDenied = incident.LicDenied;
            LicRevoked = incident.LicRevoked;
            LicDeniedRevokedReason = incident.LicDeniedRevokedReason;
            Penalty = incident.Penalty;
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
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.WAIncidentTypeEnum IncidentType { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Type")]
        public DateTime? IncidentDate { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        public string StateId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Fault Type")]
        public byte? AtFaultTypeId { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Display(Name = "Anyone Injured?")]
        public bool? AnyoneInjured { get; set; }

        [UIHint("EnumLongBoxBox")]
        [Display(Name = "Number Of Injuries")]
        public byte? NumberOfInjuries { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Display(Name = "Citation Issued?")]
        public bool? CitationIssued { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Citation Type")]
        public string CitationType { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "License Denied?")]
        public bool? LicDenied { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "License Revoked?")]
        public bool? LicRevoked { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Denied/Revoked Reason")]
        public string LicDeniedRevokedReason { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Penalty")]
        public string Penalty { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {            
            if (IncidentDate == null) modelState.AddModelError("IncidentDate", "Incident Date may not be blank");
            if (string.IsNullOrEmpty(Description)) modelState.AddModelError("Description", "Description may not be blank");
            if (string.IsNullOrEmpty(StateId)) modelState.AddModelError("StateId", "State may not be blank");
            if (string.IsNullOrEmpty(Location)) modelState.AddModelError("Location", "Location may not be blank");
            if (AtFaultTypeId == null) modelState.AddModelError("AtFaultTypeId", "At Fault Type may not be blank");
            if (CitationIssued == null) modelState.AddModelError("CitationIssued", "Citation Issued may not be blank");
            if (CitationIssued == true && string.IsNullOrEmpty(CitationType)) modelState.AddModelError("CitationType", "Citation Type may not be blank");
            if (CitationIssued == true && LicDenied == null) modelState.AddModelError("LicDenied", "License Denied may not be blank");
            if (CitationIssued == true && LicRevoked == null) modelState.AddModelError("LicRevoked", "License Revoked may not be blank");
            if (LicRevoked == true && string.IsNullOrEmpty(LicDeniedRevokedReason)) modelState.AddModelError("LicDeniedRevokedReason", "Revoked Reason may not be blank");
            if (CitationIssued == true && string.IsNullOrEmpty(Penalty)) modelState.AddModelError("Penalty", "Penalty may not be blank");
        }

        internal TrafficCitationViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
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
                updObj.CitationIssued = this.CitationIssued;
                updObj.CitationType = this.CitationType;
                updObj.LicDenied = this.LicDenied;
                updObj.LicRevoked = this.LicRevoked;
                updObj.LicDeniedRevokedReason = this.LicDeniedRevokedReason;
                updObj.Penalty = this.Penalty;
                try
                {
                    db.BulkSaveChanges();
                    return new TrafficCitationViewModel(updObj);
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
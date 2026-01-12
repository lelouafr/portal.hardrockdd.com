//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace portal.Models.Views.WA.Application
//{
//    public class TrafficIncidentViewModel
//    {
//        public TrafficIncidentViewModel()
//        {

//        }

//        public TrafficIncidentViewModel(Code.Data.VP.WATrafficIncident incident)
//        {
//            if (incident == null)
//                return;


//            ApplicantId = incident.ApplicantId;
//            ApplicationId = incident.ApplicationId;
//            SeqId = incident.SeqId;
//            IncidentType = incident.IncidentType;
//            IncidentDate = incident.IncidentDate;
//            StateId = incident.StateId;
//            Location = incident.Location;
//            AtFaultTypeId = incident.AtFaultTypeId;
//            AnyoneInjured = incident.AnyoneInjured;
//            NumberOfInjuries = incident.NumberOfInjuries;
//            CitationIssued = incident.CitationIssued;
//            CitationType = incident.CitationType;
//            LicDenied = incident.LicDenied;
//            LicRevoked = incident.LicRevoked;
//            LicDeniedRevokedReason = incident.LicDeniedRevokedReason;
//            Penalty = incident.Penalty;
//        }

//        [Key]
//        public int ApplicantId { get; set; }

//        [Key]
//        public int ApplicationId { get; set; }

//        [Key]
//        [UIHint("LongBox")]
//        [Display(Name = "#")]
//        public int SeqId { get; set; }

//        [UIHint("EnumBox")]
//        [Display(Name = "Type")]
//        public WAIncidentTypeEnum IncidentType { get; set; }

//        [UIHint("DateBox")]
//        [Display(Name = "Type")]
//        public DateTime? IncidentDate { get; set; }

//        [UIHint("TextBox")]
//        [Display(Name = "Description")]
//        public string Description { get; set; }

//        [UIHint("DropDownBox")]
//        [Display(Name = "State")]
//        public string StateId { get; set; }

//        [UIHint("TextBox")]
//        [Display(Name = "Location")]
//        public string Location { get; set; }

//        [UIHint("SwitchBox")]
//        [Display(Name = "Fault Type")]
//        public byte? AtFaultTypeId { get; set; }

//        [UIHint("SwitchBox")]
//        [Display(Name = "Anyone Injured?")]
//        public bool? AnyoneInjured { get; set; }

//        [UIHint("EnumLongBoxBox")]
//        [Display(Name = "Number Of Injuries")]
//        public byte? NumberOfInjuries { get; set; }

//        [UIHint("SwitchBox")]
//        [Display(Name = "Citation Issued?")]
//        public bool? CitationIssued { get; set; }

//        [UIHint("TextBox")]
//        [Display(Name = "Citation Type")]
//        public string CitationType { get; set; }

//        [UIHint("SwitchBox")]
//        [Display(Name = "License Denied?")]
//        public bool? LicDenied { get; set; }

//        [UIHint("SwitchBox")]
//        [Display(Name = "License Revoked?")]
//        public bool? LicRevoked { get; set; }

//        [UIHint("TextAreaBox")]
//        [Display(Name = "Denied/Revoked Reason")]
//        public string LicDeniedRevokedReason { get; set; }

//        [UIHint("TextBox")]
//        [Display(Name = "Penalty")]
//        public string Penalty { get; set; }

//    }
//}
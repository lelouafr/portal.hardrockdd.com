using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Applicant.Models
{
    public class ApplicationFormViewModel
    {
        public ApplicationFormViewModel()
        {

        }

        public ApplicationFormViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            Info = new ApplicationViewModel(application);
            WorkHistory = new WorkHistoryListViewModel(application);
            EquipmentHistory = new EquipmentHistoryListViewModel(application);
            AppliedPositions = new PositionListViewModel(application);
            Accidents = new TrafficAccidentListViewModel(application);
            Citations = new TrafficCitationListViewModel(application);
        }

        [Key]
        public int ApplicantId { get; set; }

        [Key]
        public int ApplicationId { get; set; }

        public ApplicationViewModel Info { get; set; }

        public WorkHistoryListViewModel WorkHistory { get; set; }

        public EquipmentHistoryListViewModel EquipmentHistory { get; set; }

        public PositionListViewModel AppliedPositions { get; set; }

        public TrafficAccidentListViewModel Accidents { get; set; }

        public TrafficCitationListViewModel Citations { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (!WorkHistory.List.Any())
                modelState.AddModelError("", "No work history is listed!");
            else
                WorkHistory.List.ForEach(e => e.Validate(modelState));

            EquipmentHistory.List.ForEach(e => e.Validate(modelState));

            if (Info.Accidents && !Accidents.List.Any())
                modelState.AddModelError("", "No Accidents listed!");
            else if (Info.Accidents && Accidents.List.Any())
                Accidents.List.ForEach(e => e.Validate(modelState));

            if (Info.Citiations && !Citations.List.Any())
                modelState.AddModelError("", "No Citations listed!");
            else if (Info.Citiations && Citations.List.Any())
                Citations.List.ForEach(e => e.Validate(modelState));

        }
    }
}
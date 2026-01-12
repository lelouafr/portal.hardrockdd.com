using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class ProjectCrewListViewModel
    {
        public ProjectCrewListViewModel()
        {
            List = new List<ProjectCrewViewModel>();
            List.Add(new ProjectCrewViewModel());
        }

        public ProjectCrewListViewModel(DB.Infrastructure.ViewPointDB.Data.Job project)
        {
            if (project == null) throw new System.ArgumentNullException(nameof(project));

            PMCo = project.JCCo;
            ProjectId = project.JobId;
            List = project.Crews.Select(s => new ProjectCrewViewModel(s)).ToList();
        }

        [Key]
        public byte PMCo { get; set; }

        [Key]
        public string ProjectId { get; set; }

        public List<ProjectCrewViewModel> List { get; }
    }

    public class ProjectCrewViewModel
    {
        public ProjectCrewViewModel()
        {

        }

        public ProjectCrewViewModel(DB.Infrastructure.ViewPointDB.Data.PMProjectCrew crew)
        {
            if (crew == null) throw new System.ArgumentNullException(nameof(crew));

            PMCo = crew.PMCo;
            ProjectId = crew.ProjectId;
            SeqId = crew.SeqId;
            CrewId = crew.CrewId;
            StartDate = crew.StartDate;

            
        }

        [Key]
        [HiddenInput]
        public byte PMCo { get; set; }

        [Key]
        public int SeqId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Project")]
        public string ProjectId { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }



    }
}
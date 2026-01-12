using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DB.Infrastructure.ViewPointDB.Data;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class JobListViewModel
    {
        public JobListViewModel()
        {
                
        }

        public JobListViewModel(DB.Infrastructure.ViewPointDB.Data.Job project)
        {
            if (project == null) return;

            List = project.SubJobs.Select(s => new JobViewModel(s)).ToList();
        }

        public List<JobViewModel> List { get; set; }
    }
    public class JobViewModel
    {
        public JobViewModel()
        {

        }

        public JobViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            JobName = job.Description;
            CrewId = job.CrewId;
            EMCo = job.Rig?.EMCo;
            RigId = job.RigId;
            StatusId = job.Status;
            StatusString = StatusId.ToString();
            Footage = job.Footage;
            PipeSize = job.PipeSize;
        }

        public byte? EMCo { get; set; }

        [HiddenInput]
        public byte JCCo { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Description")]
        public string JobName { get; set; }

        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.JCJobStatusEnum StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        public string StatusString { get; set; }

        [Display(Name = "Progress")]
        [UIHint("PercentBox")]
        public decimal? Progress { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        //[Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/RigCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Rig")]
        public string RigId { get; set; }


        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Length of Crossing")]
        public int? Footage { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Pipe Size")]
        public byte? PipeSize { get; set; }

        public decimal ActualCost { get; set; }

        public decimal BudgetCost { get; set; }

    }
}
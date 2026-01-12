using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Schedule
{
    public class ScheduleListViewModel
    {
        public ScheduleListViewModel()
        {
            List = new List<ScheduleViewModel>();
        }

        public ScheduleListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;
            var projectList = company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7" && (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).Select(s => new ScheduleViewModel(s)).ToList();
            var packageList = company.Bids.SelectMany(s => s.Packages).Where(s => s.Bid.tStatusId == (int)(DB.BidStatusEnum.PendingAward)).Select(s => new ScheduleViewModel(s)).ToList();

            List = projectList;
            List.AddRange(packageList);
        }

        public ScheduleListViewModel(List<DB.Infrastructure.ViewPointDB.Data.Job> jobs, List<DB.Infrastructure.ViewPointDB.Data.BidPackage> bidPackages)
        {
            if (jobs == null) throw new System.ArgumentNullException(nameof(jobs));
            if (bidPackages == null) throw new System.ArgumentNullException(nameof(bidPackages));

            var projectList = jobs.Select(s => new ScheduleViewModel(s)).ToList();
            var packageList = bidPackages.Select(s => new ScheduleViewModel(s)).ToList();

            List = projectList;
            List.AddRange(packageList);
        }

        public byte Co { get; set; }

        public List<ScheduleViewModel> List { get; }
    }

    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {

        }

        public ScheduleViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            JCCo = job.JCCo;
            BidId = job.BidPackages.LastOrDefault()?.BidId ?? 0;
            ProjectId = job.JobId;
            ProjectName = job.Description;
            Customer = job.Contract.CustomerId;
            CrewId = job.CrewId;
            RigId = job.RigId;
            DivisionId = job.DivisionId;
            Division = job.DivisionDesc;
            ProjectStartDate = job.CalculatedStartDate();
            ProjectEndDate = job.CalculatedEndDate();
        }

        public ScheduleViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null) throw new System.ArgumentNullException(nameof(package));

            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            ProjectName = package.Description;
            ProjectId = package.JobId;
            Customer = package.CustomerId;
            CrewId = package.CrewId;
            RigId = package.RigId;
            DivisionId = package.DivisionId;
            Division = package.Division.Description;
            ProjectStartDate = package.StartDate ?? package.Bid.StartDate;
        }

        [HiddenInput]
        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        [HiddenInput]
        public byte JCCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Project")]
        public string ProjectId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Project")]
        public string ProjectName { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? ProjectStartDate { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? ProjectEndDate { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Customer")]
        public int? Customer { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Division")]
        public string Division { get; set; }

        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo")]
        public int? DivisionId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Required]
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



    }
}
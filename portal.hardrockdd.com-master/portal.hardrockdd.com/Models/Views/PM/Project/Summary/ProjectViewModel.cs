using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Summary
{
    public class ProjectListViewModel
    {
        public ProjectListViewModel()
        {
            List = new List<ProjectViewModel>();
            List.Add(new ProjectViewModel());
        }

        public ProjectListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;
            var projectList = company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7").Select(s => new ProjectViewModel(s)).ToList();

            List = projectList;
        }

        public byte Co { get; set; }

        public List<ProjectViewModel> List { get; }
    }

    public class ProjectViewModel
    {
        public ProjectViewModel()
        {

        }

        public ProjectViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            Co = job.JCCo;
            ProjectId = job.JobId;
            ProjectName = job.Description;
            CustomerId = job.Contract?.CustomerId;
            CustomerName = job.Contract?.Customer.Name;
            //OwnerId= job.Owner
            Division = job.Division?.Description;
            DivisionId = job.DivisionId;

            Status = job.Status;
            StatusStr = string.Format("{0}", Status);
            //ProjectStartDate = job.CalculatedStartDate();
            ProjectManagerId = job.ProjectMgr;
        }

        [HiddenInput]
        public byte Co { get; set; }

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

        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/CustomerCombo", ComboForeignKeys = "CustGroupId")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.JCJobStatusEnum Status { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string StatusStr { get; set; }


        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo")]
        public int? DivisionId { get; set; }

        [Display(Name = "Division")]
        [UIHint("TextBox")]
        public string Division { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field()]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [Display(Name = "Owner")]
        [UIHint("DropdownBox")]
        [Field(FormGroup = "Project Info", FormGroupRow = 3, Placeholder = "Select Owner", ComboUrl = "/Firm/OwnerCombo", ComboForeignKeys = "VendGroupId", AddUrl = "/Firm/Add", AddForeignKeys = "VendGroupId, FirmType", EditUrl = "/Firm/Form", EditForeignKeys = "Co, FirmNumber")]
        public int? OwnerId { get; set; }

        [UIHint("TextBox")]
        [Field(ComboUrl = "/PRCombo/ProjectMangerCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Project Manager")]
        public int? ProjectManagerId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/RigCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Rig")]
        public string RigId { get; set; }

        [UIHint("IntegerBox")]
        [Field()]
        [Display(Name = "Length of Crossing")]
        public int? Footage { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Pipe Size")]
        public byte? PipeSize { get; set; }



    }
}
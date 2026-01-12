using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job
{
    public class JobListViewModel
    {
        public JobListViewModel()
        {
            List = new List<JobViewModel>();
        }

        public JobListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Co = company.HQCo;
            List = db.vJobs.OrderBy(f => f.JobId).AsEnumerable().Select(s => new JobViewModel(s)).ToList();//s.Where(f => f.JCCo == company.HQCo)
        }

        public JobListViewModel(List<DB.Infrastructure.ViewPointDB.Data.vJob> jobs, VPContext db)
        {
            if (jobs == null) throw new System.ArgumentNullException(nameof(jobs));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            List = jobs.OrderBy(f => f.JobId).AsEnumerable().Select(s => new JobViewModel(s)).ToList();//s.Where(f => f.JCCo == company.HQCo)
        }


        public JobListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, byte status, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Co = company.HQCo;
            List = db.vJobs.Where(f =>f.JobStatus == status).OrderBy(f => f.JobId).AsEnumerable().Select(s => new JobViewModel(s)).ToList();// f.JCCo == company.HQCo && 
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public byte Co { get; set; }

        public List<JobViewModel> List { get; }
        public string ListJSON { get; }
    }

    public class JobViewModel
    {
        public JobViewModel()
        {

        }

        public JobViewModel(DB.Infrastructure.ViewPointDB.Data.vJob job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            id = job.KeyID;
            //parent = jobs.FirstOrDefault(f => f.KeyID == job.ParentKeyId)?.KeyID;


            JCCo = job.JCCo;
            JobId = job.JobId;
            JobName = job.JobDescription;
            Customer = job.Customer;
            CustomerName = job.CustName;
            CrewId = job.CrewId;
            Crew = job.CrewDescription;
            Rig = job.RigDescription;
            RigId = job.RigId;
            Footage = job.Footage;
            PipeSize = job.PipeSize;
            HasAttachment = job.HasAttachments > 0 ? true : false;
            DivisionId = job.DivisionId;
            Division = job.Division;
            StatusId = job.StatusId;
            StatusString = job.StatusDescription;
            JobType = job.JobType;
            ProjectId = job.ParentJobId;
            ProjectName = job.ParentJobDescription;

            
        }


        public JobViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            id = job.KeyID;
            //parent = jobs.FirstOrDefault(f => f.KeyID == job.ParentKeyId)?.KeyID;


            JCCo = job.JCCo;
            JobId = job.JobId;
            JobName = job.Description;
            Customer = job.Contract.CustomerId;
            CustomerName = job.Contract.Customer.Name;
            CrewId = job.CrewId;
            Crew = job.Crew?.Description;
            Footage = job.Footage;
            PipeSize = job.PipeSize;
            Rig = job.Rig?.Description;
            RigId = job.RigId;
            HasAttachment = job.HasAttachments();
            DivisionId = job.DivisionId;
            Division = job.Division?.Description;
            StatusId = int.TryParse(job.StatusId, out int outStatusId)? outStatusId: 0;
            StatusString = ((DB.JCJobStatusEnum)StatusId).ToString();
            JobType = job.JobTypeId;

            ProjectId = job.ParentJobId;
            ProjectName = job.ParentJob?.Description;

            StartDate = job.CalculatedStartDate(true);
        }

        [HiddenInput]
        public long id { get; set; }

        [HiddenInput]
        public long? parent { get; set; }

        [HiddenInput]
        public byte JCCo { get; set; }

        [HiddenInput]
        public bool HasAttachment { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Project")]
        public string ProjectId { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Description")]
        public string JobName { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/JCCombo/StatusCombo", ComboForeignKeys = "")]
        public int? StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        public string StatusString { get; set; }

        [Display(Name = "Job Type")]
        [UIHint("TextBox")]
        public string JobType { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Parent Job")]
        public string ParentJobId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Parent Job")]
        public string ParentJobName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Customer")]
        public int? Customer { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Division")]
        public string Division { get; set; }

        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo=JCCo")]
        public int? DivisionId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "End Market")]
        public string EndMarket { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/RigCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Rig")]
        public string RigId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Rig")]
        public string Rig { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Footage")]
        public int? Footage { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Pipe Size")]
        public byte? PipeSize { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "StartDate")]
        public DateTime StartDate { get; set; }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job
{
    public class JobSummaryListViewModel
    {
        public JobSummaryListViewModel()
        {
            List = new List<JobSummaryViewModel>();
        }

        public JobSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var jobs = db.vJobs.Where(f => f.JCCo == company.HQCo)
                               .ToList()
                               .Where(f => f.JobId.StartsWith("20-", System.StringComparison.CurrentCulture))
                               .ToList();
            Co = company.HQCo;
            List = jobs.Where(f => f.ParentKeyId == null).Select(s => new JobSummaryViewModel(s, jobs)).ToList();
            List.AddRange(jobs.Where(f => List.Any(s=> s.id == f.ParentKeyId)).Select(s => new JobSummaryViewModel(s, jobs)).ToList());

        }

        public byte Co { get; set; }

        public  List<JobSummaryViewModel> List { get;  }
    }

    public class JobSummaryViewModel
    {
        public JobSummaryViewModel()
        {

        }
        public JobSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.vJob job, List<DB.Infrastructure.ViewPointDB.Data.vJob> jobs)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));
            if (jobs == null) throw new System.ArgumentNullException(nameof(jobs));

            Co = job.JCCo;
            JobId = job.JobId;
            JobName = job.JobDescription;
            Customer = job.Customer;
            CustomerName = job.CustName;
            id = job.KeyID;
            parent = jobs.FirstOrDefault(f => f.KeyID == job.ParentKeyId)?.KeyID;
            CrewId = job.CrewId;
            Crew = job.CrewDescription;
            
            Rig = job.RigDescription;
            RigId = job.RigId;
            DivisionId = job.DivisionId;
            StatusId = job.StatusId;

            if (job.ParentKeyId != null)
            {
                ParentJobId = job.ParentJobId;
                ParentJobName = job.ParentJobDescription;
                Footage = job.Footage;
                PipeSize = job.PipeSize;

                Invoiced = job.Inv_Amount;
                ActCost = job.Job_Cost;
                ActGross = job.Act_Gross;
                ActGM = job.ACTGM;

                BgtRevenue = job.BgtRevenue;
                BgtCost = job.BgtCost;
                BgtGross = job.BGT_Gross;
                BgtGM = job.BGTGM;


                VarRevenue = Invoiced - BgtRevenue;
                VarCost = ActCost - BgtCost;
                VarGross = ActGross - BgtGross;
            }
            else
            {
                Footage = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.Footage);
                Invoiced = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.Inv_Amount);
                ActCost = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.Job_Cost);
                ActGross = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.Act_Gross);
                ActGM = Invoiced ==0 ? 0 : ActGross / Invoiced;

                BgtRevenue = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.BgtRevenue);
                BgtCost = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.BgtCost);
                BgtGross = jobs.Where(f => f.ParentKeyId == job.KeyID).Sum(sum => sum.BGT_Gross);
                BgtGM = BgtRevenue == 0 ? 0 : BgtGross / BgtRevenue;
            }
            //Invoiced = job.A

        }
        [HiddenInput]
        public byte Co { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Description")]
        public string JobName { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/JobStatus/Combo", ComboForeignKeys = "")]
        public int? StatusId { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Parent Job")]
        public string ParentJobId { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Parent Job")]
        public string ParentJobName { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Customer")]
        public int? Customer { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

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
        [Display(Name = "End Market")]
        public string EndMarket { get; set; }

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
        
        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/RigCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Rig")]
        public string RigId { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Rig")]
        public string Rig { get; set; }



        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Length of Crossing")]
        public int? Footage { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Pipe Size")]
        public byte? PipeSize { get; set; }

        [UIHint("IntegerBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Actual", FormGroupRow = 2)]
        [Display(Name = "Days")]

        public decimal? ActDays { get; set; }

        [UIHint("CurrencyBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Actual", FormGroupRow = 2)]
        [Display(Name = "Invoiced")]
        public decimal? Invoiced { get; set; }

        [UIHint("CurrencyBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Actual", FormGroupRow = 2)]
        [Display(Name = "Cost")]
        public decimal? ActCost { get; set; }

        [UIHint("CurrencyBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Actual", FormGroupRow = 2)]
        [Display(Name = "Gross")]
        public decimal? ActGross { get; set; }

        [ReadOnly(true)]
        [UIHint("PercentBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Actual", FormGroupRow = 2)]
        [Display(Name = "Gross Margin")]
        public decimal? ActGM { get; set; }


        [ReadOnly(true)]
        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Days")]
        public decimal? BgtDays { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Revnue")]
        public decimal? BgtRevenue { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Cost")]
        public decimal? BgtCost { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Gross")]
        public decimal? BgtGross { get; set; }

        [ReadOnly(true)]
        [UIHint("PercentBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Gross Margin")]
        public decimal? BgtGM { get; set; }

        [ReadOnly(true)]
        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Days")]
        public decimal? VarDays { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Revnue")]
        public decimal? VarRevenue { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Cost")]
        public decimal? VarCost { get; set; }

        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Budget", FormGroupRow = 2)]
        [Display(Name = "Gross")]
        public decimal? VarGross { get; set; }

        [ReadOnly(true)]
        [HiddenInput]
        public long id { get; set; }

        [ReadOnly(true)]
        [HiddenInput]
        public long? parent { get; set; }



    }
}
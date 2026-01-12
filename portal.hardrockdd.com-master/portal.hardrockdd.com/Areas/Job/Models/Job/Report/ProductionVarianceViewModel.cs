using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Job.Models.Job.Report
{
    public class ProductionVarianceListViewModel
    {
        public ProductionVarianceListViewModel()
        {

        }

        public ProductionVarianceListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            List = new List<ProductionVarianceViewModel>();
            var job = ticket?.DailyJobTicket?.Job;
            if (job == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            List = job.db.udBDPR_BgtVsAct(job.JCCo, job.JobId).ToList().Select(s => new ProductionVarianceViewModel(s)).ToList();
        }

        public ProductionVarianceListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            List = new List<ProductionVarianceViewModel>();
            if (job == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            List = job.db.udBDPR_BgtVsAct(job.JCCo, job.JobId).ToList().Select(s => new ProductionVarianceViewModel(s)).ToList();
        }

        public ProductionVarianceListViewModel(DB.Infrastructure.ViewPointDB.Data.vJob job)
        {
            List = new List<ProductionVarianceViewModel>();
            if (job == null)
                return;

            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            JCCo = job.JCCo;
            JobId = job.JobId;
            List = db.udBDPR_BgtVsAct(job.JCCo, job.JobId).ToList().Select(s => new ProductionVarianceViewModel(s)).ToList();
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        public List<ProductionVarianceViewModel> List { get; }

    }
    public class ProductionVarianceViewModel
    {
        public ProductionVarianceViewModel()
        {

        }

        public ProductionVarianceViewModel(DB.Infrastructure.ViewPointDB.Data.udBDPR_BgtVsAct_Result result)
        {
            if (result == null)
            {
                throw new System.ArgumentNullException(nameof(result));
            }
            JCCo = (byte)result.Co;
            JobId = result.JobId;
            PhaseGroupId = result.PhaseGroupId;
            PhaseId = result.PhaseId;
            PassId = result.PassId;
            SortId = result.SortId;
            ActBoreSize = (decimal?)result.ActBoreSize;
            BgtBoreSize = (decimal?)result.BgtBoreSize;
            BoreSize = (decimal?)result.BoreSize;
            BudgetDays = result.BudgetDays;
            BudgetTrucks = result.BudgetTrucks;
            StartDate = result.StartDate;
            ActualDays = result.ActualDays ?? 0;
            Dif = result.Dif;
            BudgetProductionRate = result.BudgetProductionRate;
            BudgetFootage = result.BudgetFootage;
            ActualFootage = result.ActualFootage;
            RemainingFootage = result.RemainingFootage;
            ActualProdutionRate = result.ActualProdutionRate;
            ActualTrucks = result.ActualTrucks ?? 0;
            TruckDif = result.TruckDif;
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        [Key]
        public byte PhaseGroupId { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }

        [Key]
        [Display(Name = "Pass Id")]
        [UIHint("IntegerBox")]
        public int PassId { get; set; }

        [Key]
        [Display(Name = "Sort Id")]
        [UIHint("IntegerBox")]
        public int SortId { get; set; }

        [Display(Name = "Size")]
        [UIHint("IntegerBox")]
        public decimal? BoreSize { get; set; }


        [Display(Name = "Act Size")]
        [UIHint("IntegerBox")]
        public decimal? ActBoreSize { get; set; }


        [Display(Name = "Bgt Size")]
        [UIHint("IntegerBox")]
        public decimal? BgtBoreSize { get; set; }

        [Display(Name = "Bgt")]
        [UIHint("IntegerBox")]
        public decimal? BudgetDays { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Act")]
        [UIHint("IntegerBox")]
        public decimal ActualDays { get; set; }

        [Display(Name = "Under (Over)")]
        [UIHint("IntegerBox")]
        public decimal? Dif { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Budget /Day")]
        [UIHint("IntegerBox")]
        public decimal? BudgetProductionRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Total")]
        [UIHint("IntegerBox")]
        public decimal? BudgetFootage { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Total to Date")]
        [UIHint("IntegerBox")]
        public decimal? ActualFootage { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Remaining")]
        [UIHint("IntegerBox")]
        public decimal? RemainingFootage { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Act per Day Avg")]
        [UIHint("IntegerBox")]
        public decimal? ActualProdutionRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Bgt")]
        [UIHint("IntegerBox")]
        public decimal? BudgetTrucks { get; set; }


        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Act")]
        [UIHint("IntegerBox")]
        public decimal ActualTrucks { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [Display(Name = "Under (Over)")]
        [UIHint("IntegerBox")]
        public decimal? TruckDif { get; set; }
    }
}
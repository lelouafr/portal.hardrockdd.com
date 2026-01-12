using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.JC.Job
{
    public class JobProductionBgtVsActListViewModel
    {
        public JobProductionBgtVsActListViewModel()
        {

        }
        public JobProductionBgtVsActListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            using var db = new VPContext();
            List = new List<JobProductionBgtVsActViewModel>();
            if (ticket.DailyJobTicket != null)
            {
                JCCo = (byte)(ticket.DailyJobTicket.JCCo ?? 1);
                JobId = ticket.DailyJobTicket.JobId;
                if (ticket.DailyJobTicket.JobId != null)
                {
                    List = db.udBDPR_BgtVsAct(ticket.DailyJobTicket.DTCo, ticket.DailyJobTicket.JobId).AsEnumerable().Select(s => new JobProductionBgtVsActViewModel(s)).ToList();
                }
            }
        }

        public JobProductionBgtVsActListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
            {
                List = new List<JobProductionBgtVsActViewModel>();
                return;
            }
            using var db = new VPContext();

            JCCo = job.JCCo;
            JobId = job.JobId;
            List = db.udBDPR_BgtVsAct(job.JCCo, job.JobId).AsEnumerable().Select(s => new JobProductionBgtVsActViewModel(s)).ToList();
        }

        public JobProductionBgtVsActListViewModel(DB.Infrastructure.ViewPointDB.Data.vJob job)
        {
            if (job == null)
            {
                List = new List<JobProductionBgtVsActViewModel>();
                return;
            }
            using var db = new VPContext();

            JCCo = job.JCCo;
            JobId = job.JobId;
            List = db.udBDPR_BgtVsAct(job.JCCo, job.JobId).AsEnumerable().Select(s => new JobProductionBgtVsActViewModel(s)).ToList();
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        public List<JobProductionBgtVsActViewModel> List { get; }

    }
    public class JobProductionBgtVsActViewModel
    {
        public JobProductionBgtVsActViewModel()
        {

        }




        public JobProductionBgtVsActViewModel(DB.Infrastructure.ViewPointDB.Data.udBDPR_BgtVsAct_Result result)
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
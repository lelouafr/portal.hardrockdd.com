using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class SummaryViewModel
    {
        public SummaryViewModel()
        {

        }

        public SummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) 
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            ActualDays = job.ActualDays;
            BudgetDays = job.BudgetDays;
            RemainingDays = job.RemainingDays;
            VendorAmount = job.APAmount;
            VendorPaid = job.APPaidAmount;
            VendorBalance = VendorAmount - VendorPaid;




            //Description = job.Description;
            //ProjectOwner = job.Owner;
            //CrewId = job.CrewId;
            //PipeSize = job.PipeSize;
            //DivisionId = (int)(job.DivisionId ?? 0);
            //EndMarketId = job.EndMarket;

            //StartDate = job.StartDate;

        }


        public SummaryViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package) 
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }

            JCCo = (byte)package.JCCo;
            JobId = package.JobId;
            BidId = package.BidId;
            PackageId = package.PackageId;
            //Description = package.Description;
            //ProjectOwner = package?.Bid?.Firm?.FirmName;
            //CrewId = package.CrewId;
            //PipeSize = package.PipeSize;
            
            //DivisionId = (int)(package.DivisionId ?? 0);
            //EndMarketId = package.Market?.Description;
            //StartDate = package.StartDate;

        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        public decimal? ActualDays { get; set; }

        public decimal? BudgetDays { get; set; }

        public decimal? RemainingDays { get; set; }

        public decimal POAmount { get; set; }

        public decimal POBalance { get; set; }

        public int OpenPOCount { get; set; }

        public decimal BudgetCost { get; set; }

        public decimal ActualCost { get; set; }

        public decimal ProjectedCost { get; set; }

        public decimal BudgetRevenue { get; set; }

        public decimal ActualRevenue { get; set; }
        
        public decimal CustomerPaid { get; set; }

        public decimal CustomerBalance { get; set; }

        public decimal VendorPaid { get; set; }

        public decimal VendorAmount { get; set; }

        public decimal VendorBalance { get; set; }

        public decimal CachBalance { get; set; }




        //[UIHint("TextBox")]
        //[Field(LabelSize = 0, TextSize = 6)]
        //[Display(Name = "Description")]
        //public string Description { get; set; }

        //[Display(Name = "Project Owner")]
        //[UIHint("TextBox")]
        //[Field(LabelSize = 2, TextSize = 4)]
        //public string ProjectOwner { get; set; }

        //[Display(Name = "Crew")]
        //[UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Crew/Combo", ComboForeignKeys = "JCCo")]
        //public string CrewId { get; set; }

        //[Display(Name = "PipeSize")]
        //[UIHint("LongBox")]
        //[Field(LabelSize = 2, TextSize = 4)]
        //public decimal? PipeSize { get; set; }

        //[Display(Name = "Division")]
        //[UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/Division/Combo", ComboForeignKeys = "JCCo")]
        //public int DivisionId { get; set; }

        //[Display(Name = "End Market")]
        //[UIHint("TextBox")]
        //[Field(LabelSize = 2, TextSize = 4)]
        //public string EndMarketId { get; set; }



        //[UIHint("DateBox")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "Start Date")]
        //public DateTime? StartDate { get; set; }


    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Job
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            Info = new InfoViewModel(job);
            POLines = new PurchaseOrder.Models.ItemListViewModel(job);
            APLines = new AccountsPayable.Models.InvoiceLineListViewModel(job);
            JobProgress = new Report.ProductionVarianceListViewModel(job);
            JobCostSummary = new CostListViewModel(job);

            ARTrans = new AccountsReceivable.Models.InvoiceListViewModel(job);

            Contract = new Contract.ContractViewModel(job.Contract);
            ContractItems = new Contract.ContractItemListViewModel(job.Contract);

            ISData = new Report.ISFormViewModel(job);

            UniqueAttchId = job.Attachment.UniqueAttchID;
            //Attachments = new Attachment.Models.ExplorerListViewModel(job.Attachment);
        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            Info = new InfoViewModel(job);
            Mth = mth;

            POLines = new PurchaseOrder.Models.ItemListViewModel(job);
            APLines = new AccountsPayable.Models.InvoiceLineListViewModel(job);
            JobProgress = new Report.ProductionVarianceListViewModel(job);
            JobCostSummary = new CostListViewModel(job);

            ARTrans = new AccountsReceivable.Models.InvoiceListViewModel(job);

            Contract = new Contract.ContractViewModel(job.Contract);
            ContractItems = new Contract.ContractItemListViewModel(job.Contract);

            ISData = new Report.ISFormViewModel(job, mth);

            UniqueAttchId = job.Attachment.UniqueAttchID;

            //Attachments = new Attachment.Models.ExplorerListViewModel(job.Attachment);
        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [HiddenInput]
        public string JobId { get; set; }

        public DateTime? Mth { get; set; }

        public InfoViewModel Info { get; set; }

        public Guid? UniqueAttchId { get; set; }

        public PurchaseOrder.Models.ItemListViewModel POLines { get; set; }

        public AccountsPayable.Models.InvoiceLineListViewModel APLines { get; set; }

        public Report.ProductionVarianceListViewModel JobProgress { get; set; }

        public CostListViewModel JobCostSummary { get; set; }

        public AccountsReceivable.Models.InvoiceListViewModel ARTrans { get; set; }

        public Contract.ContractViewModel Contract { get; set; }

        public Contract.ContractItemListViewModel ContractItems { get; set; }

        public Report.ISFormViewModel ISData { get; set; }

        //public Attachment.Models.ExplorerListViewModel Attachments { get; set; }

    }
}
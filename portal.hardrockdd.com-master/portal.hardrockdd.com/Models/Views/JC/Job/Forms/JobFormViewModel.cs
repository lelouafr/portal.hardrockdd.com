using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Invoice;
using portal.Models.Views.Attachment;
using portal.Models.Views.Bid;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.Purchase.Order;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job.Forms
{

    public class JobFormViewModel : AuditBaseViewModel
    {
        public JobFormViewModel()
        {

        }

        public JobFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            Co = job.JCCo;
            JobId = job.JobId;
            Info = new JobInfoViewModel(job);
            //Ownership = new JobOwnershipViewModel(job);
            //Assignment = new JobAssignmentViewModel(job);
            //Meter = new JobMeterViewModel(job);
            POLines = new PurchaseOrderItemListViewModel(job);
            APLines = new InvoiceLineListViewModel(job);
            Attachments = new AttachmentListViewModel(job);
            JobProgress = new JobProductionBgtVsActListViewModel(job);
            JobCostSummary = new Cost.JobCostListViewModel(job);

            ARTrans = new AR.Invoice.InvoiceListViewModel(job);

            Contract = new JobContractViewModel(job.Contract);
            ContractItems = new ContractItemListViewModel(job.Contract);

            ISData = new JobISFormViewModel(job);
        }

        public JobFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth) : base(job)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            Co = job.JCCo;
            JobId = job.JobId;
            Info = new JobInfoViewModel(job);
            Mth = mth;
            //Ownership = new JobOwnershipViewModel(job);
            //Assignment = new JobAssignmentViewModel(job);
            //Meter = new JobMeterViewModel(job);
            POLines = new PurchaseOrderItemListViewModel(job);
            APLines = new InvoiceLineListViewModel(job);
            Attachments = new AttachmentListViewModel(job);
            JobProgress = new JobProductionBgtVsActListViewModel(job);
            JobCostSummary = new Cost.JobCostListViewModel(job);

            ARTrans = new AR.Invoice.InvoiceListViewModel(job);

            Contract = new JobContractViewModel(job.Contract);
            ContractItems = new ContractItemListViewModel(job.Contract);

            ISData = new JobISFormViewModel(job, mth);
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public string JobId { get; set; }

        public DateTime? Mth { get; set; }

        public JobInfoViewModel Info { get; set; }

        //public JobOwnershipViewModel Ownership { get; set; }

        //public JobAssignmentViewModel Assignment { get; set; }

        //public JobMeterViewModel Meter { get; set; }

        public PurchaseOrderItemListViewModel POLines { get; set; }

        public InvoiceLineListViewModel APLines { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public JobProductionBgtVsActListViewModel JobProgress { get; set; }

        public Cost.JobCostListViewModel JobCostSummary { get; set; }

        public AR.Invoice.InvoiceListViewModel ARTrans { get; set; }

        public JobContractViewModel Contract { get; set; }

        public ContractItemListViewModel ContractItems { get; set; }


        public JobISFormViewModel ISData { get; set; }

    }
}
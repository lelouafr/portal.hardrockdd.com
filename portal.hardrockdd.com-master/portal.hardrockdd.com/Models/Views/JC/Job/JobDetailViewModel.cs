using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Bid;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.Purchase.Order;
using System.Linq;

namespace portal.Models.Views.JC.Job
{

    public class JobDetailViewModel
    {
        public JobDetailViewModel()
        {

        }

        public JobDetailViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, VPContext db)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            var vJobs = db.vJobs.Where(f => (f.JCCo == job.JCCo && f.JobId == job.JobId) || f.ParentKeyId == job.KeyID).ToList();
            JobSummary = new JobSummaryViewModel(vJobs.FirstOrDefault(f => f.KeyID == job.KeyID), vJobs);
            PurchaseOrderItems = new PurchaseOrderItemListViewModel(job);
            //Bid = new BidBoreLineFormViewModel(job.BidBoreLine);
            Tickets = new DailyTicketListSummaryViewModel(job, db, DB.TimeSelectionEnum.All);
            Production = new JobProductionBgtVsActListViewModel(job);
        }

        public JobSummaryViewModel JobSummary { get; set; }

        public DailyTicketListSummaryViewModel Tickets { get; }
                
        public PurchaseOrderItemListViewModel PurchaseOrderItems { get; }

        public PurchaseOrderSummaryListViewModel Invoices { get; }

        //public BidBoreLineFormViewModel Bid { get; }

        public JobProductionBgtVsActListViewModel Production { get; set; }

    }
}
//using DB.Infrastructure.ViewPointDB.Data;

//namespace portal.Repository.VP.DT
//{
//    public static class DailyJobTicketRepository
//    {        
//        public static void UpdateJobIdOnTicket(DailyTicket ticket, Job srcJob, Job dstJob, VPContext db)
//        {
//            if (ticket == null || dstJob == null || srcJob == null || db == null)
//            {
//                return;
//            }
//            if (ticket.Status == DB.DailyTicketStatusEnum.Processed && srcJob.JCCo == dstJob.JCCo)
//            {
//                using var threadDB = new VPContext();
//                threadDB.udDTUpdateJob(ticket.DTCo, ticket.TicketId, srcJob.JobId, dstJob.JobId);

//            }
//            else if (ticket.Status != DB.DailyTicketStatusEnum.Processed)
//            {
//                using var threadDB = new VPContext();
//                threadDB.udDTUpdateJob(ticket.DTCo, ticket.TicketId, srcJob.JobId, dstJob.JobId);
//            }
//        }
//    }
//}

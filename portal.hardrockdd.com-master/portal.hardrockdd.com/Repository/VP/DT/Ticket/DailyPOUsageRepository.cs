//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace portal.Repository.VP.DT.Ticket
//{
//    public static class DailyPOUsageRepository
//    {
//        public static DailyPOUsage Init(DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            var result = new DailyPOUsage
//            {
//                DTCo = ticket.DTCo,
//                TicketId = ticket.TicketId,
//                LineId = ticket.POUsages.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
//                JobId = ticket.DailyJobTicket.JobId,
//                WorkDate = ticket.WorkDate,
//                Qty = 0,
//                Hours = 0
//            };
//            return result;
//        }

//        public static Models.Views.DailyTicket.DailyPOUsageViewModel ProcessUpdate(Models.Views.DailyTicket.DailyPOUsageViewModel model, VPContext db)
//        {
//            if (model == null)
//                throw new System.ArgumentNullException(nameof(model));
//            if (db == null)
//                throw new System.ArgumentNullException(nameof(db));

//            var updObj = db.DailyPOUsages.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineId == model.LineId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                var vendorChange = updObj.VendorId != model.VendorId;
//                updObj.VendorId = model.VendorId;
//                updObj.PO = model.PO;
//                updObj.Qty = model.Quantity;

//                if (vendorChange)
//                {
//                    updObj.PO = null;
//                }
//            }
//            return new Models.Views.DailyTicket.DailyPOUsageViewModel(updObj);
//        }

//    }
//}
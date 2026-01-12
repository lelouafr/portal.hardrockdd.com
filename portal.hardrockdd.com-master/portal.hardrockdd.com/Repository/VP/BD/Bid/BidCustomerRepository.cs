//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.AR;
//using portal.Repository.VP.HQ;

//namespace portal.Repository.VP.BD
//{
//    public static class BidCustomerRepository
//    {
//        public static BidCustomer Init(Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            var model = new BidCustomer
//            {
//                BDCo = bid.BDCo,
//                BidId = bid.BidId,
//                LineId = bid.Customers.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//                CustomerId = 0,
//                Bid = bid,
//            };

//            return model;
//        }

//        //public static Models.Views.Bid.Forms.Header.CustomerViewModel ProcessUpdate(Models.Views.Bid.Forms.Header.CustomerViewModel model, VPContext db)
//        //{
//        //    if (model == null)
//        //        throw new ArgumentNullException(nameof(model));
//        //    if (db == null)
//        //        throw new ArgumentNullException(nameof(db));

//        //    var updObj = db.BidCustomers.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.LineId == model.LineId);

//        //    if (updObj != null)
//        //    {
//        //        if (model.CustomerId != updObj.CustomerId)
//        //        {
//        //            model.ContactId = null;
//        //        }
//        //        /****Write the changes to object****/
//        //        updObj.CustomerId = model.CustomerId;
//        //        updObj.ContactId = model.ContactId;
//        //    }
//        //    return new Models.Views.Bid.Forms.Header.CustomerViewModel(updObj);
//        //}

//    }
//}
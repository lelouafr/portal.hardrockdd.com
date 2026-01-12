//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.AR.Customer;
//using portal.Models.Views.JC.Market;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.JC
//{
//    public static class MarketRepository
//    {

//        public static JCMarket Init(MarketViewModel industry)
//        {
//            if (industry == null)
//                throw new ArgumentNullException(nameof(industry));

//            var model = new JCMarket
//            {
//                JCCo = industry.JCCo,
//                MarketId = industry.MarketId,
//                Description = industry.Description,
//            };
//            return model;
//        }

//        public static MarketViewModel ProcessUpdate(MarketViewModel model, VPContext db)
//        {
//            if (model == null) throw new ArgumentNullException(nameof(model));
//            if (db == null) throw new ArgumentNullException(nameof(db));

//            var updObj = db.JCMarkets.FirstOrDefault(f => f.JCCo == model.JCCo && f.MarketId == model.MarketId);

//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//            }
//            return new MarketViewModel(updObj);
//        }

//        public static JCMarket Create(JCMarket entity, VPContext db)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            if (db == null) throw new ArgumentNullException(nameof(db));

//            if (entity.MarketId == 0)
//            {
//                entity.MarketId = db.JCMarkets
//                                .Where(f => f.JCCo == entity.JCCo)
//                                .DefaultIfEmpty()
//                                .Max(f => f == null ? 0 : f.MarketId) + 1;
//            }


//            db.JCMarkets.Add(entity);
//            return entity;
//        }

//        public static void Delete(JCMarket entity, VPContext db)
//        {
//            if (entity == null) throw new ArgumentNullException(nameof(entity));
//            if (db == null) throw new ArgumentNullException(nameof(db));

//            db.JCMarkets.Remove(entity);
//        }

//    }
//}
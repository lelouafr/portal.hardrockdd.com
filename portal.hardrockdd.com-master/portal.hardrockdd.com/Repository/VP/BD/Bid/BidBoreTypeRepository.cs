//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.BD
//{
//    public class BidBoreTypeRepository 
//    {
//        private VPContext db = new VPContext();

//        public BidBoreTypeRepository()
//        {

//        }

//        public static BidBoreType Init()
//        {
//            var model = new BidBoreType
//            {

//            };

//            return model;
//        }

//        public List<BidBoreType> GetBidBoreTypes(byte Co)
//        {
//            var qry = db.BidBoreTypes
//                        .Where(f => f.BDCo == Co)
//                        .AsEnumerable()
//                        .Select(s => s);

//            return qry.ToList();
//        }

//        public static List<SelectListItem> GetSelectList(List<BidBoreType> List, string selected = "")
//        {
//            var result = List.Select(s => new SelectListItem
//            {
//                Value = s.BoreTypeId.ToString(AppCultureInfo.CInfo()),
//                Text = s.Description,
//                Selected = s.BoreTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//            }).ToList();

//            return result;
//        }

//    }
//}

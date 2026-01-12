//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//namespace portal.Repository.VP.BD
//{
//    public static class BidBoreLineRepository 
//    {           
//        public static BidBoreLine ProcessUpdate(Models.Views.Bid.Forms.Bore.BoreLineViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            //var recalculateUnits = false;
//            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.BoreId == model.BoreId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.BoreTypeId = model.BoreTypeId;
//                updObj.Description = model.Description;
//                updObj.Footage = model.Footage;
//                updObj.PipeSize = model.PipeSize;
//                updObj.RigCategoryId = model.RigCategoryId;
//                updObj.CrewCount = model.CrewCount;


//                if (updObj.RecalcNeeded == true)
//                {
//                    updObj.RecalculateCostUnits();
//                    updObj.Package.ApplyPackageCost();
//                }
//            }
//            return updObj;
//        }

//        public static BidBoreLine ProcessUpdate(Models.Views.Bid.Forms.Bore.Job.JobViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            //var recalculateUnits = false;
//            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.BoreId == model.BoreId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//                updObj.JobId = model.JobId;
//                updObj.AwardStatus = model.AwardStatus;

//                if (updObj.RecalcNeeded == true)
//                {
//                    updObj.RecalculateCostUnits();
//                    updObj.Package.ApplyPackageCost();
//                }
//            }
//            return updObj;
//        }

//        public static BidBoreLine ProcessUpdate(Models.Views.Bid.Forms.Bore.Award.AwardViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            //var recalculateUnits = false;
//            var updObj = db.BidBoreLines.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.BoreId == model.BoreId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//                updObj.Footage = model.Footage;
//                updObj.AwardStatus = model.AwardStatus;

//                if (updObj.RecalcNeeded == true)
//                {
//                    updObj.RecalculateCostUnits();
//                    updObj.Package.ApplyPackageCost();
//                }

//            }
//            return updObj;
//        }

//    }
//}

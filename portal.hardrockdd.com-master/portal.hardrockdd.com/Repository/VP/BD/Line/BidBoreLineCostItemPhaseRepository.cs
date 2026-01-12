//using DB.Infrastructure.ViewPointDB.Data;
//using System;

//namespace portal.Repository.VP.BD
//{
//    public static class BidBoreLineCostItemPhaseRepository 
//    {
//        public static BidBoreLineCostItemPhase Init(BidBoreLineCostItem costItem)
//        {
//            if (costItem == null)
//            {
//                throw new ArgumentNullException(nameof(costItem));
//            }
//            var model = new BidBoreLineCostItemPhase
//            {
//                BDCo = costItem.BDCo,
//                BidId = costItem.BidId,
//                BoreId = costItem.BoreId,
//                GroundDensityId = costItem.GroundDensityId,
//                LineId = costItem.LineId,
//                Locked = "N"
//            };

//            return model;
//        }
        
//    }
//}
//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.AP
//{
//    public static class APDocumentStatusLogRepository
//    {
//        //public static APDocumentStatusLog Init(APDocument request)
//        //{
//        //    if (request == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(request));
//        //    }
//        //    var model = new APDocumentStatusLog
//        //    {
//        //        APCo = request.APCo,
//        //        DocId = request.DocId,
//        //        LineNum = NextId(request),
//        //        Status = (short?)request.Status,
//        //        CreatedOn = DateTime.Now,
//        //        CreatedBy = StaticFunctions.GetUserId()
//        //    };

//        //    return model;
//        //}

//        public static int NextId(APDocument model)
//        {
//            using var db = new VPContext();
//            return db.APDocumentStatusLogs
//                            .Where(f => f.APCo == model.APCo && f.DocId == model.DocId)
//                            .DefaultIfEmpty()
//                            .Max(f => f == null ? 0 : f.LineNum) + 1;
//        }

//    }
//}
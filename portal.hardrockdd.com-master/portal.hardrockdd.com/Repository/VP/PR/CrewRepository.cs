//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.PR
//{
//    public partial class CrewRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public CrewRepository()
//        {

//        }
//        public bool Exists(Crew model)
//        {
//            var qry = from f in db.Crews
//                      where f.PRCo == model.PRCo && f.CrewId == model.CrewId
//                      select f;

//            if (qry.Any())
//                return true;

//            return false;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~CrewRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}

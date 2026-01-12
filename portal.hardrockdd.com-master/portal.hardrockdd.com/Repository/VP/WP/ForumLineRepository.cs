//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.WP

//{
//    public partial class ForumLineRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public ForumLineRepository()
//        {

//        }

//        public static ForumLine Init(Forum forum)
//        {
//            if (forum == null)
//            {
//                //throw new System.ArgumentNullException(nameof(forum));
//                return null;
//            }
//            var model = new ForumLine
//            {
//                Co = forum.Co,
//                ForumId = forum.ForumId,
//                LineId = NextId(forum),
//                CreatedBy = StaticFunctions.GetUserId(),
//                CreatedOn = DateTime.Now,
//                UniqueAttchID = Guid.NewGuid(),
//                ParentForumId = forum.ForumId,
//            };

//            return model;
//        }
//        public static ForumLine Init()
//        {
//            var model = new ForumLine
//            {

//            };

//            return model;
//        }

//        public static int NextId(Forum forum)
//        {
//            if (forum == null)
//            {
//                throw new System.ArgumentNullException(nameof(forum));
//            }
//            using var db = new VPContext();
//            return forum.Lines
//                            .DefaultIfEmpty()
//                            .Max(f => f == null ? 0 : f.LineId) + 1;
//        }

//        public ForumLine Create(ForumLine model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            model.LineId = db.ForumLines
//                               .Where(f => f.Co == model.Co && f.ForumId == model.ForumId)
//                               .DefaultIfEmpty()
//                               .Max(f => f == null ? 0 : f.LineId) + 1;


//            db.ForumLines.Add(model);
//            db.SaveChanges(modelState);

//            return model;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~ForumLineRepository()
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

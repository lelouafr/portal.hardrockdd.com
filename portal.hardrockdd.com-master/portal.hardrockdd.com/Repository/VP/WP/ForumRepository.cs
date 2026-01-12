//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.WP
//{
//    public partial class ForumRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public ForumRepository()
//        {

//        }

//        public static Attachment Init()
//        {
//            var model = new Attachment
//            {

//            };

//            return model;
//        }

//        public static Forum Generate(Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new ArgumentNullException(nameof(bid));
//            }
//            if (bid.Forum == null)
//            {
//                var forum = new Forum
//                {
//                    Co = bid.BDCo,
//                    TableName = "udBDBH",
//                    RelKeyID = bid.KeyID,
//                    ForumId = NextId(bid.BDCo)
//                };
               
//                bid.Forum = forum;
//            }
//            return bid.Forum;
//        }


//        public static Forum Generate(APDocument doc)
//        {
//            if (doc == null)
//            {
//                throw new ArgumentNullException(nameof(doc));
//            }
//            if (doc.Forum == null)
//            {
//                var forum = new Forum
//                {
//                    Co = doc.APCo,
//                    TableName = "udBDBH",
//                    RelKeyID = doc.KeyID,
//                    ForumId = NextId(doc.APCo)
//                };

//                //foreach (var workFlow in doc.WorkFlows.Where(f => f.Comments != null))
//                //{
//                //    var line = ForumLineRepository.Init(forum);
//                //    line.CreatedBy = workFlow.AssignedTo;
//                //    line.CreatedOn = workFlow.AssignedOn;
//                //    line.Comment = workFlow.Comments;
//                //    line.HtmlComment = workFlow.Comments;

//                //    forum.Lines.Add(line);
//                //}

//                doc.Forum = forum;
//            }
//            return doc.Forum;
//        }

//        public Forum ProcessUpdate(Forum model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            var updObj = db.Forums.FirstOrDefault(f => f.Co == model.Co && f.ForumId == model.ForumId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.TableName = model.TableName;
//                updObj.RelKeyID = model.RelKeyID;

//                db.SaveChanges(modelState);
//            }
//            return updObj;
//        }


//        public static int NextId(byte co)
//        {
//            using var db = new VPContext();
//            return db.Forums
//                            .Where(f => f.Co == co)
//                            .DefaultIfEmpty()
//                            .Max(f => f == null ? 0 : f.ForumId) + 1;
//        }

//        public Forum Create(Forum model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            model.ForumId = NextId(model.Co);

//            db.Forums.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }


//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~ForumRepository()
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
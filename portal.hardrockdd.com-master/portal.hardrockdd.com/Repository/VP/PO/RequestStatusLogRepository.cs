using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public partial class RequestStatusLogRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public RequestStatusLogRepository()
        {

        }
        
        public static PORequestStatusLog Init()
        {
            var model = new PORequestStatusLog
            {

            };

            return model;
        }

        public static PORequestStatusLog Init(PORequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            var model = new PORequestStatusLog
            {
                POCo = request.POCo,
                RequestId = request.RequestId,
                LineNum = NextId(request),
                Status = (short?)request.Status,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            return model;
        }
        public static PORequestStatusLog Init(PORequest request, PORequestRejectViewModel reject)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            if (reject == null)
            {
                throw new System.ArgumentNullException(nameof(reject));
            }
            var model = new PORequestStatusLog
            {
                POCo = request.POCo,
                RequestId = request.RequestId,
                LineNum = NextId(request),
                Status = (short?)request.Status,
                Comments = reject.Comments,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            return model;
        }

        public static int NextId(PORequest model)
        {
            using var db = new VPContext();
            return db.PORequestStatusLogs
                            .Where(f => f.POCo == model.POCo && f.RequestId == model.RequestId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineNum) + 1;
        }


        public PORequestStatusLog ProcessUpdate(PORequestStatusLog model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.PORequestStatusLogs.FirstOrDefault(f => f.POCo == model.POCo && f.RequestId == model.RequestId && f.LineNum == model.LineNum);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Status = model.Status;
                if (model.CreatedOn >= new DateTime(1900, 1, 1))
                {
                    updObj.CreatedOn = model.CreatedOn ?? updObj.CreatedOn;
                }
                updObj.CreatedBy = model.CreatedBy;
                updObj.NotificationId = model.NotificationId;
                updObj.Comments = model.Comments;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.KeyID = model.KeyID;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public PORequestStatusLog Create(PORequestStatusLog model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.LineNum = db.PORequestStatusLogs
                           .Where(f => f.POCo == model.POCo && f.RequestId == model.RequestId)
                           .DefaultIfEmpty()
                           .Max(f => f == null ? 0 : f.LineNum) + 1;

            db.PORequestStatusLogs.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RequestStatusLogRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Leave;
using portal.Models.Views.Purchase.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestStatusLogRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public RequestStatusLogRepository()
        {

        }
        
        public static LeaveRequestStatusLog Init()
        {
            var model = new LeaveRequestStatusLog
            {

            };

            return model;
        }

        public static LeaveRequestStatusLog Init(LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            var model = new LeaveRequestStatusLog
            {
                PRCo = request.PRCo,
                RequestId = request.RequestId,
                LineNum = request.StatusLogs.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                Status = (short?)request.Status,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            return model;
        }
        public static LeaveRequestStatusLog Init(LeaveRequest request, LeaveRequestRejectViewModel reject)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            if (reject == null)
            {
                throw new System.ArgumentNullException(nameof(reject));
            }
            var model = new LeaveRequestStatusLog
            {
                PRCo = request.PRCo,
                RequestId = request.RequestId,
                LineNum = NextId(request),
                Status = (short?)request.Status,
                Comments = reject.Comments,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            return model;
        }

        public static int NextId(LeaveRequest model)
        {
            using var db = new VPContext();
            return db.LeaveRequestStatusLogs
                            .Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineNum) + 1;
        }


        public LeaveRequestStatusLog ProcessUpdate(LeaveRequestStatusLog model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.LeaveRequestStatusLogs.FirstOrDefault(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId && f.LineNum == model.LineNum);
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
        
        public LeaveRequestStatusLog Create(LeaveRequestStatusLog model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.LineNum = db.LeaveRequestStatusLogs
                           .Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId)
                           .DefaultIfEmpty()
                           .Max(f => f == null ? 0 : f.LineNum) + 1;

            db.LeaveRequestStatusLogs.Add(model);
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
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestLineRepository : IDisposable
    {
        public RequestLineRepository()
        {

        }

        public static LeaveRequestLine Init(LeaveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //using var db = new VPContext();

            var model = new LeaveRequestLine();
            model.PRCo = request.PRCo;
            model.RequestId = request.RequestId;
            model.EmployeeId = request.EmployeeId;
            model.Hours = 8;

            return model;
        }
        
        public static int NextId(LeaveRequestLine model)
        {
            using var db = new VPContext();
            return db.LeaveRequestLines
                            .Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineId) + 1;
        }
        
        public static LeaveRequestLine ProcessUpdate(LeaveRequestLine model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.LeaveRequestLines.FirstOrDefault(f => f.PRCo == model.PRCo && f.PRCo == model.PRCo && f.RequestId == model.RequestId);
            if (updObj != null)
            {
               

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public static LeaveRequestLine Create(LeaveRequestLine model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            model.RequestId = db.LeaveRequestLines
                            .Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineId) + 1;

            db.LeaveRequestLines.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RequestLineRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (db != null)
                //{
                //    db.Dispose();
                //    db = null;
                //}
            }
        }
    }
}
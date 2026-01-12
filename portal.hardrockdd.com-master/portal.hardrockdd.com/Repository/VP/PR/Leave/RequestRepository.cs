using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestRepository : IDisposable
    {
        //private VPContext db = new VPContext();

        public RequestRepository()
        {

        }

        public static LeaveRequest Init()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var usr = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = usr.Employee.FirstOrDefault();
            var model = new LeaveRequest();

            model.PRCo = emp.HRCompanyParm.PRCo;
            model.Status = (int)DB.LeaveRequestStatusEnum.Open;
            model.CreatedBy = userId;
            model.CreatedOn = DateTime.Now;
            return model;
        }

        public static LeaveRequest ProcessUpdate(LeaveRequest model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.LeaveRequests.FirstOrDefault(f => f.PRCo == model.PRCo && f.PRCo == model.PRCo && f.RequestId == model.RequestId);
            if (updObj != null)
            {
                var addWorkFlow = updObj.Status != model.Status;

                /****Write the changes to object****/
                updObj.Status = model.Status;

                if (addWorkFlow)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(updObj);                    
                }
                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public static LeaveRequest Create(ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            var model = Init();
            model.RequestId = db.LeaveRequests
                            .Where(f => f.PRCo == model.PRCo)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.RequestId) + 1;

            db.LeaveRequests.Add(model);
            db.SaveChanges(modelState);

            RequestWorkFlowRepository.GenerateWorkFlow(model);
            return model;
        }

        public static int NextId(LeaveRequest model)
        {
            using var db = new VPContext();
            return db.LeaveRequests
                            .Where(f => f.PRCo == model.PRCo)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.RequestId) + 1;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RequestRepository()
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
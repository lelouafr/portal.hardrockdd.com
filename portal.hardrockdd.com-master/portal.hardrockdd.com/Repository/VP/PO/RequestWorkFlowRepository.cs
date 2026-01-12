using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public partial class RequestWorkFlowRepository : IDisposable
    {
        public RequestWorkFlowRepository()
        {

        }
        public static BidWorkFlowAssignment Init()
        {
            var model = new BidWorkFlowAssignment
            {

            };

            return model;
        }


        public static POWorkFlow ProcessUpdate(POWorkFlow model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.POWorkFlows.FirstOrDefault(m => m.POCo == model.POCo && m.RequestId == model.RequestId && m.LineId == model.LineId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.POCo = model.POCo;
                updObj.RequestId = model.RequestId;
                updObj.LineId = model.LineId;
                updObj.Status = model.Status;
                if (model.StatusDate >= new DateTime(1900, 1, 1))
                {
                    updObj.StatusDate = model.StatusDate;
                }
                updObj.AssignedTo = model.AssignedTo;
                if (model.AssignedOn >= new DateTime(1900, 1, 1))
                {
                    updObj.AssignedOn = model.AssignedOn;
                }
                updObj.AssignedStatus = model.AssignedStatus;
                if (model.EmailedDate >= new DateTime(1900, 1, 1))
                {
                    updObj.EmailedDate = model.EmailedDate ?? updObj.EmailedDate;
                }
                updObj.NotificationId = model.NotificationId;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.KeyID = model.KeyID;
                updObj.Active = model.Active;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        public static POWorkFlow Create(POWorkFlow model, ModelStateDictionary modelState = null)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.LineId = db.POWorkFlows
                            .Where(f => f.POCo == model.POCo && f.RequestId == model.RequestId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.LineId) + 1;

            db.POWorkFlows.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public static bool Delete(POWorkFlow model, ModelStateDictionary modelState = null)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.POWorkFlows.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RequestWorkFlowRepository()
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

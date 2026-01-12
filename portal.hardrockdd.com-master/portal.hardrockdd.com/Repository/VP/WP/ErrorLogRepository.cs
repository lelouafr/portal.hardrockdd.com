using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.WP
{
    public partial class ErrorLogRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public List<ErrorLog> GetErrorLogs()
        {
            return db.ErrorLogs.ToList();
        }

        public ErrorLog GetErrorLog(int ErrorId)
        {
            var qry = db.ErrorLogs
                        .FirstOrDefault(f => f.ErrorId == ErrorId);

            return qry;
        }

        public ErrorLog Create(ErrorLog model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            model.ErrorId = db.GetNextId("budWPEL", 1);

            if (model.StackTrace != null)
            {
                model.StackTrace = model.StackTrace.Substring(0, model.StackTrace.Length > 4000 ? 4000 : model.StackTrace.Length); 
            }


            db.ErrorLogs.Add(model);
            try
            {
                db.SaveChanges(modelState);
            }
            catch (Exception)
            {

            }
            return model;
        }

        public bool Delete(ErrorLog model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.ErrorLogs.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;           
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ErrorLogRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
            // free native resources if there are any.
            //if (nativeResource != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(nativeResource);
            //    nativeResource = IntPtr.Zero;
            //}
        }

    }
}
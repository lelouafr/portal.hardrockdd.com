using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Repository.VP.WP
{
    public partial class NotificationRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public Notification GetNotification(int Id)
        {
            var result = db.Notifications.FirstOrDefault(f => f.Id == Id);

            return result;
        }
        
        public Notification ProcessUpdate(Notification model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetNotification(model.Id);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Title = model.Title;
                updObj.Note = model.Note;
                updObj.CreatedOn = model.CreatedOn;
                updObj.CreatedBy = model.CreatedBy;
                updObj.AssignedTo = model.AssignedTo;
                updObj.IsRead = model.IsRead;
                updObj.IsEmailed = model.IsEmailed;
                updObj.IsDeleted = model.IsDeleted;
                updObj.NotificationLevel = model.NotificationLevel;
                updObj.WorkflowId = model.WorkflowId;
                updObj.TaskId = model.TaskId;
                updObj.Url = model.Url;
                updObj.ObjectTable = model.ObjectTable;
                updObj.KeyValue1 = model.KeyValue1;
                updObj.KeyValue2 = model.KeyValue2;
                updObj.KeyValue3 = model.KeyValue3;
                updObj.Controller = model.Controller;
                updObj.Action = model.Action;
                updObj.RoutedValues = model.RoutedValues;

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public Notification Create(Notification model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            model.Id = db.Notifications.DefaultIfEmpty().Max(f => f == null ? 0 : f.Id) + 1;

            db.Notifications.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Notification model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Notifications.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(Notification model)
        {
            var qry = from obj in db.Notifications
                      where
                         obj.Id == model.Id
                      select obj;

            if (qry.Any())
                return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NotificationRepository()
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
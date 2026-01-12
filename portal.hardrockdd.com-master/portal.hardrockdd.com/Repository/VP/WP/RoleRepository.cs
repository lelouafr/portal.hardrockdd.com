using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.WP
{
    public partial class WebRoleRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public List<WebRole> GetRoles()
        {
            var qry = db.WebRoles.ToList();
            return qry;
        }

        public WebRole GetRole(string roleId)
        {
            var qry = db.WebRoles.FirstOrDefault(f => f.Id == roleId);

            return qry;
        }

        public WebRole Create(WebRole model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            db.WebRoles.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(WebRole model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.WebRoles.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;

        }

        public bool Exists(string roleId)
        {
            var qry = from obj in db.WebRoles
                      where
                         obj.Id == roleId
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

        ~WebRoleRepository()
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
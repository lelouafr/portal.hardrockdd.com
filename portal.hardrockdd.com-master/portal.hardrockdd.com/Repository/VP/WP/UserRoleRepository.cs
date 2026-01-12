using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Web.Mvc;

namespace portal.Repository.VP.WP
{
    public partial class WebUserRoleRepository : IDisposable
    {
        private VPContext db = new VPContext();
        private WebUserRepository usrRepo = new WebUserRepository();
        private WebRoleRepository rolRepo = new WebRoleRepository();

        public WebUserRole Create(WebUserRole model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            db.WebUserRoles.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(WebUserRole model, ModelStateDictionary modelState = null)
        {
            db.WebUserRoles.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebUserRoleRepository()
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
                if (usrRepo != null)
                {
                    usrRepo.Dispose();
                    usrRepo = null;
                }
                if (rolRepo != null)
                {
                    rolRepo.Dispose();
                    rolRepo = null;
                }
            }
        }
    }
}
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public static class DbContextExtension
    {
        

        public static int SaveChanges(this DbContext db, ModelStateDictionary modelState)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            try
            {                
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Data.Entity.Validation.DbEntityValidationException exDb = null;
                System.Data.Entity.Infrastructure.DbUpdateException exDU = null;
                if (ex is System.Data.Entity.Validation.DbEntityValidationException exception)
                {
                    exDb = exception;
                }
                if (ex is System.Data.Entity.Infrastructure.DbUpdateException exception1)
                {
                    exDU = exception1;
                }

                if (modelState != null)
                {
                    try
                    {
                        if (exDb != null)
                        {
                            if (exDb.EntityValidationErrors != null)
                            {
                                foreach (var valError in exDb.EntityValidationErrors)
                                {
                                    foreach (var key in valError.ValidationErrors)
                                    {
                                        modelState.AddModelError(key.PropertyName, key.ErrorMessage);
                                    }
                                }
                            }
                        }

                        else if (exDU != null)
                        {
                            if (exDU.InnerException != null)
                            {
                                if (exDU.InnerException.InnerException != null)
                                {
                                    modelState.AddModelError("", exDU.InnerException.InnerException.Message);
                                }
                                else
                                {
                                    modelState.AddModelError("", exDU.InnerException.Message);
                                }
                            }
                            else
                            {
                                modelState.AddModelError("", exDU.GetBaseException().ToString());
                            }
                        }
                        else
                        {
                            modelState.AddModelError("", ex.GetBaseException().ToString());
                        }
                    }
                    catch (Exception)
                    {

                        modelState.AddModelError("", ex.GetBaseException().ToString());
                    }
                    return 0;
                }
                else
                {
                    //throw new System.ArgumentException(ex.GetBaseException().ToString());
                    return 0;
                }

            }
        }

        public static VPContext GetDbContextFromEntity(object entity)
        {
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null)
                return null;

            return new VPContext(object_context, dbContextOwnsObjectContext: false);
        }

        private static ObjectContext GetObjectContextFromEntity(object entity)
        {
            var field = entity.GetType().GetField("_entityWrapper");

            if (field == null)
                return null;

            var wrapper = field.GetValue(entity);
            var property = wrapper.GetType().GetProperty("Context");
            var context = (ObjectContext)property.GetValue(wrapper, null);

            return context;
        }
    }
}
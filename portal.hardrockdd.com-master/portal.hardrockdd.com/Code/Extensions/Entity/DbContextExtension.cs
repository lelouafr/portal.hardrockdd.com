using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace portal.Code.Data.VP
{
    public static class DbContextExtension
    {
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.GetTableName<T>();
        }

        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            string sql = context.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var isVirtual = property.GetGetMethod().IsVirtual;
                if (isVirtual)// && properties.FirstOrDefault(c => c.Name == property.Name + "Id") != null
                {
                    queryable = queryable.Include(property.Name);

                }
            }
            return queryable;
        }

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

        public static VPEntities GetDbContextFromEntity(object entity)
        {
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null)
                return null;

            return new VPEntities(object_context, dbContextOwnsObjectContext: false);
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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace ImportPortal.Data
{

    public partial class PortalEntities : DbContext
    {
        public void RejectChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        public System.Data.Entity.Core.Objects.ObjectContext ThisObjectContext
        {
            get
            {
                return ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext;
            }
        }

        public void Detach(object entity)
        {
            ThisObjectContext.Detach(entity);
        }
        public static DbContext GetDbContextFromEntity(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null)
                return null;

            return new DbContext(object_context, dbContextOwnsObjectContext: false);
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
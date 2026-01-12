using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AR
{
    public partial class CustomerContactRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public CustomerContactRepository()
        {

        }
                        
        public static CustomerContact Init(Customer customer, Contact contact)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            var model = new CustomerContact
            {
                CustGroupId = customer.CustGroupId,
                CustomerId = customer.CustomerId,
                ContactId = contact.ContactId,
            };

            return model;
        }

        public CustomerContact Create(CustomerContact model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.SeqId = db.CustomerContacts
                            .Where(f => f.CustGroupId == model.CustGroupId && f.CustomerId == model.CustomerId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.SeqId) + 1;

            db.CustomerContacts.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CustomerContactRepository()
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
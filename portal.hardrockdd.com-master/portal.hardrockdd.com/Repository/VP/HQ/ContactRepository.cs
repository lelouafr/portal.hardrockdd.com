using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.HQ
{
    public partial class ContactRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public ContactRepository()
        {

        }
       
        public static Contact Init()
        {
            var model = new Contact
            {

            };

            return model;
        }

        public static Contact Init(Customer customer)
        {
            if (customer == null)
            {
                throw new System.ArgumentNullException(nameof(customer));
            }
            var nameParts = customer.Contact.Split(' ');

            var model = new Contact
            {
                ContactGroupId = customer.CustGroupId,
                FirstName = nameParts.Length >= 1 ? nameParts[0] : "",
                MiddleInitial = "",
                LastName = nameParts.Length >= 2 ? nameParts[1] : "",
                //CourtesyTitle = customer.CourtesyTitle,
                //Title = customer.Title,
                //Organization = customer.Organization,
                Phone = customer.Phone,
                //PhoneExtension = customer.PhoneExtension,
                Cell = customer.Phone,
                Fax = customer.Fax,
                Email = customer.EMail,
                Address = customer.Address,
                AddressAdditional = customer.Address2,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                Zip = customer.Zip,
                Notes = customer.Notes,

            };

            return model;
        }

        public Models.Views.AR.Contact.ContactViewModel Create(Models.Views.AR.Contact.ContactViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = Init();
            updObj.ContactGroupId = model.ContactGroupId;
            updObj.ContactId = model.ContactId;
            updObj.FirstName = model.FirstName;
            updObj.MiddleInitial = model.MiddleInitial;
            updObj.LastName = model.LastName;
            //updObj.CourtesyTitle = model.CourtesyTitle;
            updObj.Title = model.Title;
            //updObj.Organization = model.Organization;
            updObj.Phone = model.Phone;
            //updObj.PhoneExtension = model.PhoneExtension;
            updObj.Cell = model.Cell;
            updObj.Fax = model.Fax;
            updObj.Email = model.Email;
            updObj.Address = model.Address;
            updObj.AddressAdditional = model.AddressAdditional;
            updObj.City = model.City;
            updObj.State = model.State;
            //updObj.Country = model.Country;
            updObj.Zip = model.Zip;
            updObj.Notes = model.Notes;
            //updObj.UniqueAttchID = model.UniqueAttchID;
            //updObj.HQContactID = model.HQContactID;

            var result = Create(updObj, modelState);

            var custContact = new CustomerContact
            {
                CustGroupId = result.ContactGroupId,
                CustomerId = model.CustomerId,
                ContactId = result.ContactId,
                SeqId = db.CustomerContacts
                                    .Where(f => f.CustGroupId == model.ContactGroupId && f.CustomerId == model.CustomerId)
                                    .DefaultIfEmpty()
                                    .Max(f => f == null ? 0 : f.SeqId) + 1
            };

            db.CustomerContacts.Add(custContact);
            db.SaveChanges(modelState);
            return new Models.Views.AR.Contact.ContactViewModel(result);
        }

        public Models.Views.AR.Contact.ContactViewModel ProcessUpdate(Models.Views.AR.Contact.ContactViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = GetContact(model.ContactGroupId, model.ContactId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.FirstName = model.FirstName;
                updObj.MiddleInitial = model.MiddleInitial;
                updObj.LastName = model.LastName;
                //updObj.CourtesyTitle = model.CourtesyTitle;
                updObj.Title = model.Title;
                //updObj.Organization = model.Organization;
                updObj.Phone = model.Phone;
                //updObj.PhoneExtension = model.PhoneExtension;
                updObj.Cell = model.Cell;
                updObj.Fax = model.Fax;
                updObj.Email = model.Email;
                updObj.Address = model.Address;
                updObj.AddressAdditional = model.AddressAdditional;
                updObj.City = model.City;
                updObj.State = model.State;
                //updObj.Country = model.Country;
                updObj.Zip = model.Zip;
                updObj.Notes = model.Notes;
                //updObj.UniqueAttchID = model.UniqueAttchID;
                //updObj.HQContactID = model.HQContactID;

                db.SaveChanges(modelState);
            }
            return new Models.Views.AR.Contact.ContactViewModel(updObj);
        }

        public List<Contact> GetContacts(byte contactGroupId)
        {
            var qry = db.Contacts
                        .Where(f => f.ContactGroupId == contactGroupId)
                        .ToList();

            return qry;
        }
       
        public Contact GetContact(byte contactGroupId, int contactId)
        {
            var qry = db.Contacts
                        .Where(f => f.ContactGroupId == contactGroupId && f.ContactId == contactId)
                        .FirstOrDefault();

            return qry;
        }
        
        public static List<SelectListItem> GetSelectList(List<Contact> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.ContactId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(),"{0} {1}", s.FirstName, s.LastName),
                Selected = s.ContactId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();

            return result;
        }

        public Contact ProcessUpdate(Contact model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = GetContact(model.ContactGroupId, model.ContactId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.ContactGroupId = model.ContactGroupId;
                updObj.ContactId = model.ContactId;
                updObj.FirstName = model.FirstName;
                updObj.MiddleInitial = model.MiddleInitial;
                updObj.LastName = model.LastName;
                updObj.CourtesyTitle = model.CourtesyTitle;
                updObj.Title = model.Title;
                updObj.Organization = model.Organization;
                updObj.Phone = model.Phone;
                updObj.PhoneExtension = model.PhoneExtension;
                updObj.Cell = model.Cell;
                updObj.Fax = model.Fax;
                updObj.Email = model.Email;
                updObj.Address = model.Address;
                updObj.AddressAdditional = model.AddressAdditional;
                updObj.City = model.City;
                updObj.State = model.State;
                updObj.Country = model.Country;
                updObj.Zip = model.Zip;
                updObj.Notes = model.Notes;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.HQContactID = model.HQContactID;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public Contact Create(Contact model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.ContactId = db.Contacts
                            .Where(f => f.ContactGroupId == model.ContactGroupId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.ContactId) + 1;

            db.Contacts.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Contact model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Contacts.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(Contact model)
        {
            var qry = from f in db.Contacts
                      where f.ContactGroupId == model.ContactGroupId && f.ContactId == model.ContactId
                      select f;

            if (qry.Any())
                return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ContactRepository()
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
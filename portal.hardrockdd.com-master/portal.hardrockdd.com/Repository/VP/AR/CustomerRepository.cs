using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Customer.Forms;
using portal.Models.Views.AR.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AR
{
    public class CustomerRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public CustomerRepository()
        {

        }

        public static Customer Init(CustomerAddViewModel customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            var model = new Customer
            {
                CustGroupId = customer.CustGroupId,
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Phone = customer.Phone,
                //Fax = customer.Fax,
                //EMail = customer.EMail,
                //URL = customer.URL,
                //Contact = customer.Contact,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Zip = customer.Zip,
                //Address2 = customer.Address2,
                //Notes = customer.Notes,
                TempYN = "Y",
                Status = "I",
                RecType = 1,
                PayTerms = "00",
                TaxGroup = 1,
                CreditLimit = 0,
                SelPurge = "N",
                StmntPrint = "N",
                StmtType = "O",
                FCType = "N",
                FCPct = 0,
                MarkupDiscPct = 0,
                DateOpened = DateTime.Now.Date,
                HaulTaxOpt = 0,
                InvLvl = 0,
                MiscOnInv = "N",
                MiscOnPay = "N",
                PrintLvl = 1,
                SubtotalLvl = 1,
                SepHaul = "Y",
                ExclContFromFC = "N"
            };
            model.SortName = customer.Name.ToUpper(AppCultureInfo.CInfo()).Replace(" ", "");
            if (model.SortName.Length > 15)
            {
                model.SortName = model.SortName.Substring(0, 15);
            }
            return model;
        }

        public static CustomerViewModel ProcessUpdate(CustomerViewModel model, VPContext db)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            var updObj = db.Customers.FirstOrDefault(f => f.CustGroupId == model.CustGroupId && f.CustomerId == model.CustomerId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                //updObj.CustGroupId = model.CustGroupId;
                //updObj.CustomerId = model.CustomerId;
                updObj.Name = model.Name;
                updObj.Phone = model.Phone;
                updObj.Fax = model.Fax;
                updObj.EMail = model.EMail;
                updObj.URL = model.URL;
                updObj.Contact = model.Contact;
                updObj.Address = model.Address;
                updObj.City = model.City;
                updObj.State = model.State;
                updObj.Zip = model.Zip;
                updObj.Address2 = model.Address2;
                updObj.Notes = model.Notes;
            }
            return new CustomerViewModel(updObj);
        }


        public static CustomerInfoViewModel ProcessUpdate(CustomerInfoViewModel model, VPContext db)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (db == null)
                throw new ArgumentNullException(nameof(db));

            var updObj = db.Customers.FirstOrDefault(f => f.CustGroupId == model.CustGroupId && f.CustomerId == model.CustomerId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Name = model.Name;
                updObj.Phone = model.Phone;
                //updObj.Fax = model.Fax;
                //updObj.EMail = model.EMail;
                //updObj.URL = model.URL;
                //updObj.Contact = model.Contact;
                updObj.Address = model.Address;
                updObj.City = model.City;
                updObj.State = model.State;
                updObj.Zip = model.Zip;
                //updObj.Address2 = model.Address2;
                //updObj.Notes = model.Notes;
            }

            return new CustomerInfoViewModel(updObj);
        }

        public Customer GetCustomer(byte CustGroupId, int CustomerId)
        {
            var qry = db.Customers
                        .Where(f => f.CustGroupId == CustGroupId && f.CustomerId == CustomerId)
                        .FirstOrDefault();

            return qry;
        }

        public Customer Create(Customer model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.CustomerId = db.Customers
                            .Where(f => f.CustGroupId == model.CustGroupId && f.CustomerId >= 90000)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 89999 : f.CustomerId) + 1;


            db.Customers.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Customer model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Customers.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CustomerRepository()
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
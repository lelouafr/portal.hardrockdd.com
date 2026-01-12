using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.PM
{
    public partial class FirmRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public FirmRepository()
        {

        }
                
        public List<Firm> GetFirms(byte VendorGroup)
        {
            var qry = db.Firms
                        .Where(f => f.VendorGroupId == VendorGroup)
                        .ToList();

            return qry;
        }
        
        public Firm GetFirm(byte VendorGroup, int FirmNumber)
        {
            var qry = db.Firms
                        .FirstOrDefault(f => f.VendorGroupId == VendorGroup && f.FirmNumber == FirmNumber);

            return qry;
        }

        public List<Firm> GetFirms(byte VendorGroup, string FirmTypeId)
        {
            var qry = db.Firms
                        .Where(f => f.VendorGroupId == VendorGroup && f.FirmTypeId == FirmTypeId)
                        .ToList();

            return qry;
        }

        public Models.Views.Firm.FirmViewModel ProcessUpdate(Models.Views.Firm.FirmViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetFirm(model.VendorGroupId, model.FirmNumber);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.VendorGroupId = model.VendorGroupId;
                updObj.FirmNumber = model.FirmNumber;
                updObj.FirmName = model.FirmName;
                updObj.ContactName = model.ContactName;
                updObj.MailAddress = model.MailAddress;
                updObj.MailCity = model.MailCity;
                updObj.MailState = model.MailState;
                updObj.MailZip = model.MailZip;
                updObj.MailAddress2 = model.MailAddress2;
                updObj.Phone = model.Phone;
                updObj.Fax = model.Fax;
                updObj.EMail = model.EMail;
                updObj.URL = model.URL;
                updObj.Notes = model.Notes;

                db.SaveChanges(modelState);
            }
            return new Models.Views.Firm.FirmViewModel(updObj);
        }

        public Firm Create(Models.Views.Firm.FirmViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            var updObj = new Firm
            {
                VendorGroupId = model.VendorGroupId,
                FirmNumber = model.FirmNumber,
                FirmName = model.FirmName,
                FirmTypeId = model.FirmTypeId,
                ContactName = model.ContactName,
                MailAddress = model.MailAddress,
                MailCity = model.MailCity,
                MailState = model.MailState == "null" ? null : model.MailState,
                MailZip = model.MailZip,
                MailAddress2 = model.MailAddress2,
                Phone = model.Phone,
                Fax = model.Fax,
                EMail = model.EMail,
                URL = model.URL,
                Notes = model.Notes,
                ExcludeYN = "N",
                UpdateAP = "N",
            };
            updObj.SortName = updObj.FirmName.ToUpper(AppCultureInfo.CInfo()).Replace(" ", "");
            updObj.FirmTypeId = model.FirmTypeId;
            if (updObj.SortName.Length > 15)
            {
                updObj.SortName = updObj.SortName.Substring(0, 15);
            }


            return Create(updObj, modelState);
        }
        
        public static List<SelectListItem> GetSelectList(List<Firm> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.FirmNumber.ToString(AppCultureInfo.CInfo()),
                Text = s.FirmName,
                Selected = s.FirmNumber.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();
            return result;
        }

        public Firm Create(Firm model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            model.FirmNumber = db.Firms
                            .Where(f => f.VendorGroupId == model.VendorGroupId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 1000 : f.FirmNumber) + 1;
            db.Firms.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Firm model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Firms.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FirmRepository()
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

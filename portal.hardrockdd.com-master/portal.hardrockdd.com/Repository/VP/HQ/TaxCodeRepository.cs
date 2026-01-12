using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.HR
{
    public class TaxCodeRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public TaxCodeRepository()
        {

        }

        public static TaxCode Init()
        {
            var model = new TaxCode
            {

            };

            return model;
        }
               
        public static List<SelectListItem> GetSelectList(List<TaxCode> List, string selected = "")
        {

            var result = List.OrderBy(o => o.TaxCodeId)
                           .Select(s => new SelectListItem
                           {
                               Value = s.TaxCodeId.ToString(AppCultureInfo.CInfo()),
                               Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description),
                               Selected = s.TaxCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                           }).ToList();
            return result;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TaxCodeRepository()
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
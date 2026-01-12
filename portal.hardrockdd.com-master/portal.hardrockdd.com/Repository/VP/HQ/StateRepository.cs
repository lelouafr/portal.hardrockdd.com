using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.HR
{
    public class StateRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public StateRepository()
        {

        }

        public static HQState Init()
        {
            var model = new HQState
            {

            };

            return model;
        }
               
        public List<HQState> GetStates(string Country = "US")
        {
            var qry = db.HQStates
                        .Where(f => f.Country == Country)
                        .OrderBy(o => o.State)
                        .ToList();

            return qry;
        }
        
        public HQState GetState(string Country = "US", string State = "")
        {
            var qry = db.HQStates
                        .FirstOrDefault(f => f.Country == Country && f.State == State);

            return qry;
        }

        public List<SelectListItem> GetSelectList(string Country, string selected = "")
        {
            return GetStates(Country).OrderBy(o => o.State)
                                .Where(f => f.Country == Country)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.State.ToString(AppCultureInfo.CInfo()),
                                    Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Name),
                                    Selected = s.State.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<HQState> List, string selected = "")
        {

            var result = List.OrderBy(o => o.State)
                           .Select(s => new SelectListItem
                           {
                               Value = s.State.ToString(AppCultureInfo.CInfo()),
                               Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Name),
                               Selected = s.State.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                           }).ToList();
            return result;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StateRepository()
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
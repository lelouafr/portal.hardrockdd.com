using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.WP
{
    public partial class CalendarRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public CalendarRepository()
        {

        }
        
        public static Calendar Init()
        {
            var model = new Calendar
            {

            };

            return model;
        }

        public List<Calendar> GetCalendars(int weekId)
        {
            var qry = db.Calendars
                        .Where(f => f.Week == weekId)
                        .ToList();

            return qry;
        }
        
        public List<Calendar> GetCalendars()
        {
            var qry = db.Calendars.ToList();

            return qry;
        }
        
        public Calendar GetCalendar(DateTime Date)
        {
            var qry = db.Calendars.FirstOrDefault(f => f.Date == Date);

            return qry;
        }
        
        public List<SelectListItem> GetSelectList(string selected = "")
        {
            return db.Calendars.Select(s => new SelectListItem
                                {
                                    Value = s.Date.ToString(AppCultureInfo.CInfo()),
                                    Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.Date, s.Date),
                                    Selected = s.Date.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<Calendar> List, string selected = "")
        {

            var result = List.Select(s => new SelectListItem
            {
                Value = s.Date.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.Date, s.Date),
                Selected = s.Date.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();

            return result;
        }

        public Calendar ProcessUpdate(Calendar model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetCalendar(model.Date);
            if (updObj != null)
            {
                /****Write the changes to object****/
                if (model.Date >= new DateTime(1900, 1, 1))
                {
                    updObj.Date = model.Date;
                }
                updObj.Year = model.Year;
                updObj.Month = model.Month;
                updObj.Week = model.Week;
                updObj.Holiday = model.Holiday;
                updObj.Weekday = model.Weekday;
                updObj.WeekDescription = model.WeekDescription;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.KeyID = model.KeyID;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public Calendar Create(Calendar model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            db.Calendars.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Calendar model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Calendars.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
            
        }

        public bool Exists(Calendar model)
        {
            var qry = from f in db.Calendars
                      where f.Date == model.Date
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

        ~CalendarRepository()
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

using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.PR
{
    public partial class RaceRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public RaceRepository()
        {

        }
        
        public static Race Init()
        {
            var model = new Race
            {

            };

            return model;
        }

        public List<Race> GetRaces(byte PRCo)
        {
            var qry = db.Races
                        .Where(f => f.PRCo == PRCo)
                        .ToList();

            return qry;
        }
        
        public Race GetRace(byte PRCo, string RaceId)
        {
            var qry = db.Races
                        .FirstOrDefault(f => f.PRCo == PRCo && f.RaceId == RaceId);

            return qry;
        }
        
        public List<SelectListItem> GetSelectList(byte PRCo, string selected = "")
        {
            return db.Races.Where(f => f.PRCo == PRCo)
                            .Select(s => new SelectListItem
                            {
                                Value = s.RaceId,
                                Text = s.Description,
                                Selected = s.RaceId == selected ? true : false
                            }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<Race> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.RaceId,
                Text = s.Description,
                Selected = s.RaceId== selected ? true : false
            }).ToList();
            return result;
        }

        public Race ProcessUpdate(Race model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetRace(model.PRCo, model.RaceId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.EEOCat = model.EEOCat;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public Race Create(Race model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            db.Races.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Race model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Races.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(Race model)
        {
            var qry = from f in db.Races
                      where f.PRCo == model.PRCo && f.RaceId == model.RaceId
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

        ~RaceRepository()
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

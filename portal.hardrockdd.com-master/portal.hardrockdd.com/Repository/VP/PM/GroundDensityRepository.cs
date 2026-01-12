using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.PM
{
    public partial class GroundDensityRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public GroundDensityRepository()
        {

        }

        public static GroundDensity Init()
        {
            var model = new GroundDensity
            {

            };

            return model;
        }

        public List<GroundDensity> GetGroundDensities(byte Co)
        {
            var qry = db.GroundDensities
                        .Where(f => f.Co == Co)
                        .ToList();

            return qry;
        }
        
        public GroundDensity GetGroundDensity(byte Co, int GroundDensityId)
        {
            var qry = db.GroundDensities
                        .FirstOrDefault(f => f.Co == Co && f.GroundDensityId == GroundDensityId);

            return qry;
        }
        
        public List<SelectListItem> GetSelectList(byte Co, string selected = "")
        {
            return db.GroundDensities
                                .Where(f => f.Co == Co)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.GroundDensityId.ToString(AppCultureInfo.CInfo()),
                                    Text = s.Description,
                                    Selected = s.GroundDensityId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<GroundDensity> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.GroundDensityId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = s.GroundDensityId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();

            return result;
        }

        public GroundDensity ProcessUpdate(GroundDensity model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetGroundDensity(model.Co, model.GroundDensityId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.MinPSI = model.MinPSI;
                updObj.MaxPSI = model.MaxPSI;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public GroundDensity Create(GroundDensity model, ModelStateDictionary modelState = null)
        {
            db.GroundDensities.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(GroundDensity model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.GroundDensities.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(GroundDensity model)
        {
            var qry = from f in db.GroundDensities
                      where f.Co == model.Co && f.GroundDensityId == model.GroundDensityId
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

        ~GroundDensityRepository()
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

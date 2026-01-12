using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.EM
{
    public partial class EquipmentCategoryRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public EquipmentCategoryRepository()
        {

        }
        
        public static EquipmentCategory Init()
        {
            var model = new EquipmentCategory
            {

            };

            return model;
        }

        public List<EquipmentCategory> GetEquipmentCategories(byte EMCo)
        {
            var qry = db.EquipmentCategories
                        .Where(f => f.EMCo == EMCo)
                        .ToList();

            return qry;
        }
        public EquipmentCategory FindEquipmentCategory(byte EMCo, string CategoryName)
        {
            var qry = db.EquipmentCategories
                        .FirstOrDefault(f => f.EMCo == EMCo && f.Description.ToLower(AppCultureInfo.CInfo()).Contains(CategoryName.ToLower(AppCultureInfo.CInfo())));

            return qry;
        }
        public EquipmentCategory GetEquipmentCategory(byte EMCo, string CategoryId)
        {
            var qry = db.EquipmentCategories
                        .Where(f => f.EMCo == EMCo && f.CategoryId == CategoryId)
                        .FirstOrDefault();

            return qry;
        }
        
        public List<SelectListItem> GetSelectList(byte EMCo, string selected = "")
        {
            return db.EquipmentCategories
                                .Where(f => f.EMCo == EMCo)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.CategoryId.ToString(AppCultureInfo.CInfo()),
                                    Text = s.Description,
                                    Selected = s.CategoryId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<EquipmentCategory> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.CategoryId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = s.CategoryId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();

            return result;
        }

        public EquipmentCategory ProcessUpdate(EquipmentCategory model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = GetEquipmentCategory(model.EMCo, model.CategoryId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.JobFlag = model.JobFlag;
                updObj.PRClass = model.PRClass;
                updObj.PRCo = model.PRCo;
                updObj.Notes = model.Notes;
                updObj.UnitOfMeasure = model.UnitOfMeasure;
                updObj.RevCat = model.RevCat;
                updObj.CrewSize = model.CrewSize;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public EquipmentCategory Create(EquipmentCategory model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            //model.EMCo = db.EquipmentCategories
            //                //.Where(f => f.HRCo == model.HRCo)
            //                .DefaultIfEmpty()
            //                .Max(f => f == null ? 0 : f.EMCo) + 1;

            db.EquipmentCategories.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(EquipmentCategory model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.EquipmentCategories.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(EquipmentCategory model)
        {
            var qry = from f in db.EquipmentCategories
                      where f.EMCo == model.EMCo && f.CategoryId == model.CategoryId
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

        ~EquipmentCategoryRepository()
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
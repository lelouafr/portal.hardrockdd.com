//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class CostTypeRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public CostTypeRepository()
//        {

//        }

//        public static CostType Init()
//        {
//            var model = new CostType
//            {

//            };

//            return model;
//        }

//        public List<CostType> GetCostTypes(byte phaseGroupId)
//        {
//            var qry = db.CostTypes
//                        .Where(f => f.PhaseGroupId == phaseGroupId)
//                        .ToList();

//            return qry;
//        }
        
//        public CostType GetCostType(byte phaseGroupId, byte costTypeId)
//        {
//            var qry = db.CostTypes.FirstOrDefault(f => f.PhaseGroupId == phaseGroupId && f.CostTypeId == costTypeId);

//            return qry;
//        }
        
//        public List<SelectListItem> GetSelectList(byte phaseGroupId, string selected = "")
//        {
//            return db.CostTypes.Where(f => f.PhaseGroupId == phaseGroupId)
//                               .Select(s => new SelectListItem
//                               {
//                                   Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
//                                   Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CostTypeId, s.Description),
//                                   Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//                               }).ToList();

//        }

//        public static List<SelectListItem> GetSelectList(List<CostType> List, string selected = "")
//        {
//            var result = List.Select(s => new SelectListItem
//            {
//                Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CostTypeId, s.Description),
//                Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//            }).ToList();

//            return result;
//        }

//        public CostType ProcessUpdate(CostType model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var updObj = GetCostType(model.PhaseGroupId, model.CostTypeId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//                updObj.Abbreviation = model.Abbreviation;
//                updObj.TrackHours = model.TrackHours;
//                updObj.LinkProgress = model.LinkProgress;
//                updObj.Notes = model.Notes;
//                updObj.JBCostTypeCategory = model.JBCostTypeCategory;
//                updObj.UniqueAttchID = model.UniqueAttchID;
//                updObj.KeyID = model.KeyID;

//                db.SaveChanges(modelState);
//            }
//            return updObj;
//        }
        
//        public CostType Create(CostType model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            db.CostTypes.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }

//        public bool Delete(CostType model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            db.CostTypes.Remove(model);
//            return db.SaveChanges(modelState) == 0 ? false : true;
//        }

//        public bool Exists(CostType model)
//        {
//            var qry = from f in db.CostTypes
//                      where f.PhaseGroupId == model.PhaseGroupId && f.CostTypeId == model.CostTypeId
//                      select f;

//            if (qry.Any())
//                return true;

//            return false;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~CostTypeRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}

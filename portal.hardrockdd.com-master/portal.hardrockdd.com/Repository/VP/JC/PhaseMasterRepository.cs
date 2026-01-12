//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class PhaseMasterRepository : IDisposable
//    {
//        private VPContext db = new VPContext();
        
//        public PhaseMasterRepository()
//        {

//        }
        
//        public static PhaseMaster Init()
//        {
//            var model = new PhaseMaster
//            {

//            };

//            return model;
//        }

//        public List<PhaseMaster> GetPhaseMasters(byte PhaseGroupId)
//        {
//            var qry = db.PhaseMasters
//                        .Where(f => f.PhaseGroupId == PhaseGroupId)
//                        .ToList();

//            return qry;
//        }

//        public PhaseMaster GetPhaseMaster(byte PhaseGroupId, string PhaseId)
//        {
//            var qry = db.PhaseMasters
//                        .FirstOrDefault(f => f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId);

//            return qry;
//        }
        
//        public List<SelectListItem> GetSelectList(byte PhaseGroupId, string selected = "")
//        {
//            return db.PhaseMasters.Where(f => f.PhaseGroupId == PhaseGroupId)
//                                .Select(s => new SelectListItem
//                                {
//                                    Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
//                                    Text = s.Description,
//                                    Selected = s.PhaseId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//                                }).ToList();

//        }

//        //public static List<SelectListItem> GetSelectList(List<PhaseMaster> List, string selected = "")
//        //{
//        //    var result = List.Select(s => new SelectListItem
//        //    {
//        //        Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
//        //        Text = s.Description,
//        //        Selected = s.PhaseId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//        //    }).ToList();
//        //    return result;
//        //}

//        //public PhaseMaster ProcessUpdate(PhaseMaster model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    var updObj = GetPhaseMaster(model.PhaseGroupId, model.PhaseId);
//        //    if (updObj != null)
//        //    {
//        //        /****Write the changes to object****/
//        //        updObj.Description = model.Description;
//        //        updObj.ProjMinPct = model.ProjMinPct;
//        //        updObj.Notes = model.Notes;
//        //        updObj.UniqueAttchID = model.UniqueAttchID;
//        //        updObj.KeyID = model.KeyID;
//        //        updObj.ParentPhaseId = model.ParentPhaseId;
//        //        updObj.PhaseType = model.PhaseType;
//        //        updObj.GroundConditionType = model.GroundConditionType;
//        //        updObj.ActiveYN = model.ActiveYN;
//        //        updObj.ComboLabel = model.ComboLabel;

//        //        db.SaveChanges(modelState);
//        //    }
//        //    return updObj;
//        //}
        
//        //public PhaseMaster Create(PhaseMaster model, ModelStateDictionary modelState = null)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    db.PhaseMasters.Add(model);
//        //    db.SaveChanges(modelState);
//        //    return model;
//        //}
        
//        //public bool Delete(PhaseMaster model, ModelStateDictionary modelState = null)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    db.PhaseMasters.Remove(model);
//        //    return db.SaveChanges(modelState) == 0 ? false : true;
//        //}

//        public bool Exists(PhaseMaster model)
//        {
//            var qry = from f in db.PhaseMasters
//                      where f.PhaseGroupId == model.PhaseGroupId && f.PhaseId == model.PhaseId
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

//        ~PhaseMasterRepository()
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

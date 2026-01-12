//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class JobPhaseRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public JobPhaseRepository()
//        {

//        }

//        public static JobPhase Init()
//        {
//            var model = new JobPhase
//            {

//            };

//            return model;
//        }

//        public static List<JobPhase> GetJobPhases(byte Co)
//        {
//            ObjectCache systemCache = MemoryCache.Default;
//            CacheItemPolicy policy = new CacheItemPolicy
//            {
//                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(15)
//            };

//            var memKey = "JobPhaseList_" + Co.ToString();
//            if (!(MemoryCache.Default[memKey] is List<JobPhase> phaseList))
//            {
//                using var db = new VPContext();
//                phaseList = db.JobPhases
//                                .Where(f => f.JCCo == Co)
//                                .ToList();
//                systemCache.Set(memKey, phaseList, policy);
//            }

//            return phaseList;
//        }

//        public List<JobPhase> GetJobPhases(byte JCCo, string JobId, byte PhaseGroupId)
//        {
//            var qry = db.JobPhases
//                        .Where(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId)
//                        .ToList();

//            return qry;
//        }
        
//        public JobPhase GetJobPhase(byte JCCo, string JobId, byte PhaseGroupId, string PhaseId)
//        {
//            var qry = db.JobPhases
//                        .FirstOrDefault(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId);

//            return qry;
//        }

//        //public List<SelectListItem> GetSelectList(byte JCCo, string JobId, byte PhaseGroupId, string selected = "")
//        //{
//        //    return GetJobPhases(JCCo, JobId, PhaseGroupId)
//        //                        .Where(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId)
//        //                        .Select(s => new SelectListItem
//        //                        {
//        //                            Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
//        //                            Text = s.Description,
//        //                            Selected = s.PhaseId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//        //                        }).ToList();

//        //}

//        //public static List<SelectListItem> GetSelectList(List<JobPhase> List, string selected = "")
//        //{
//        //    var result = List.Select(s => new SelectListItem
//        //    {
//        //        Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
//        //        Text = s.Description,
//        //        Selected = s.PhaseId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//        //    }).ToList();

//        //    //if (!result.Where(j => j.Value == selected).Any() && selected != string.Empty && selected != null && selected != "0")
//        //    //{
//        //    //    int.TryParse(selected, out int selectInt);
//        //    //    var obj = new JCJobPhaseRepository().GetJobPhase(1, selectInt);
//        //    //    var text = string.Format(AppCultureInfo.CInfo(),"{0}: {1}", s.PhaseGroupId, s.Description);
//        //    //
//        //    //    result.Add(new SelectListItem()
//        //    //    {
//        //    //        Value = selected,
//        //    //        Text = text,
//        //    //        Selected = true
//        //    //    });
//        //    //}
//        //    return result;
//        //}

//        //public JobPhase ProcessUpdate(JobPhase model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    var updObj = GetJobPhase(model.JCCo, model.JobId, model.PhaseGroupId, model.PhaseId);
//        //    if (updObj != null)
//        //    {
//        //        /****Write the changes to object****/
//        //        updObj.Description = model.Description;
//        //        updObj.ContractId = model.ContractId;
//        //        updObj.Item = model.Item;
//        //        updObj.ProjMinPct = model.ProjMinPct;
//        //        updObj.ActiveYN = model.ActiveYN;
//        //        updObj.Notes = model.Notes;
//        //        updObj.UniqueAttchID = model.UniqueAttchID;
//        //        updObj.KeyID = model.KeyID;
//        //        updObj.InsCode = model.InsCode;
//        //        updObj.PhaseType = model.PhaseType;
//        //        updObj.ParentPhaseId = model.ParentPhaseId;

//        //        db.SaveChanges(modelState);
//        //    }
//        //    return updObj;
//        //}
        
//        //public JobPhase Create(JobPhase model, ModelStateDictionary modelState = null)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    db.JobPhases.Add(model);
//        //    db.SaveChanges(modelState);
//        //    return model;
//        //}

//        //public bool Delete(JobPhase model, ModelStateDictionary modelState = null)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    db.JobPhases.Remove(model);
//        //    return db.SaveChanges(modelState) == 0 ? false : true;            
//        //}

//        //public bool Exists(JobPhase model)
//        //{
//        //    var qry = from f in db.JobPhases
//        //              where f.JCCo == model.JCCo && f.JobId == model.JobId && f.PhaseGroupId == model.PhaseGroupId && f.PhaseId == model.PhaseId
//        //              select f;

//        //    if (qry.Any())
//        //        return true;

//        //    return false;
//        //}

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~JobPhaseRepository()
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

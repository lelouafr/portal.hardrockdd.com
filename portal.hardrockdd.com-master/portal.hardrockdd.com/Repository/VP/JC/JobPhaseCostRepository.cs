//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class JobPhaseCostRepository : IDisposable
//    {
//        private VPContext db = new VPContext();
        
//        public JobPhaseCostRepository()
//        {

//        }
        
//        public static JobPhaseCost Init()
//        {
//            var model = new JobPhaseCost
//            {
//                BillFlag = "C",
//                ItemUnitFlag = "N",
//                PhaseUnitFlag = "N",
//                BuyOutYN = "N",
//                LastProjDate = null,
//                Plugged = "N",
//                ActiveYN = "Y",
//                OrigHours = 0m,
//                OrigUnits = 0m,
//                OrigCost = 0m,
//                ProjNotes = null,
//                SourceStatus = "J",
//                InterfaceDate = null,
//                Notes = null,
//            };

//            return model;
//        }

//        public List<JobPhaseCost> GetJobPhaseCosts(byte JCCo, string JobId, byte PhaseGroupId, string PhaseId)
//        {
//            var qry = db.JobPhaseCosts.Where(f => f.ActiveYN == "Y")
//                        .Where(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId)
//                        .ToList();

//            return qry;
//        }

//        public JobPhaseCost GetJobPhaseCost(byte JCCo, string JobId, byte PhaseGroupId, string PhaseId, byte CostTypeId)
//        {
//            var qry = db.JobPhaseCosts
//                        .FirstOrDefault(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId && f.CostTypeId == CostTypeId);

//            return qry;
//        }

//        public JobPhaseCost ProcessUpdate(JobPhaseCost model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var updObj = GetJobPhaseCost(model.JCCo, model.JobId, model.PhaseGroupId, model.PhaseId, model.CostTypeId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.PhaseGroupId = model.PhaseGroupId;
//                updObj.PhaseId = model.PhaseId;
//                updObj.CostTypeId = model.CostTypeId;
//                updObj.UM = model.UM;
//                updObj.BillFlag = model.BillFlag;
//                updObj.ItemUnitFlag = model.ItemUnitFlag;
//                updObj.PhaseUnitFlag = model.PhaseUnitFlag;
//                updObj.BuyOutYN = model.BuyOutYN;
//                if (model.LastProjDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.LastProjDate = model.LastProjDate ?? updObj.LastProjDate;
//                }
//                updObj.Plugged = model.Plugged;
//                updObj.ActiveYN = model.ActiveYN;
//                updObj.OrigHours = model.OrigHours;
//                updObj.OrigUnits = model.OrigUnits;
//                updObj.OrigCost = model.OrigCost;
//                updObj.ProjNotes = model.ProjNotes;
//                updObj.SourceStatus = model.SourceStatus;
//                if (model.InterfaceDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.InterfaceDate = model.InterfaceDate ?? updObj.InterfaceDate;
//                }
//                updObj.Notes = model.Notes;

//                db.SaveChanges(modelState);
//            }
//            return updObj;
//        }

//        public JobPhaseCost Create(JobPhaseCost model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }

//            db.JobPhaseCosts.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }
       
//        public bool Delete(JobPhaseCost model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            db.JobPhaseCosts.Remove(model);
//            return db.SaveChanges(modelState) == 0 ? false : true;
//        }

//        public bool Exists(JobPhaseCost model)
//        {
//            var qry = from f in db.JobPhaseCosts
//                      where f.JCCo == model.JCCo && f.JobId == model.JobId && f.PhaseGroupId == model.PhaseGroupId && f.PhaseId == model.PhaseId && f.CostTypeId == model.CostTypeId
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

//        ~JobPhaseCostRepository()
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

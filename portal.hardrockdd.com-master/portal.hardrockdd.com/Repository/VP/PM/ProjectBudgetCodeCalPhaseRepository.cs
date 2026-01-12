using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.PM
{
    public partial class ProjectBudgetCodeCalPhaseRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public ProjectBudgetCodeCalPhaseRepository()
        {

        }
        
        public static ProjectBudgetCodeCalPhase Init()
        {
            var model = new ProjectBudgetCodeCalPhase
            {

            };

            return model;
        }

        public List<ProjectBudgetCodeCalPhase> GetProjectBudgetCodeCalPhases(byte PMCo, string BudgetCodeId)
        {
            var qry = db.ProjectBudgetCodeCalPhases
                        .Where(f => f.PMCo == PMCo && f.BudgetCodeId == BudgetCodeId)
                        .ToList();

            return qry;
        }
        
        public ProjectBudgetCodeCalPhase GetProjectBudgetCodeCalPhase(byte PMCo, string BudgetCodeId, string PhaseId)
        {
            var qry = db.ProjectBudgetCodeCalPhases
                        .FirstOrDefault(f => f.PMCo == PMCo && f.BudgetCodeId == BudgetCodeId && f.PhaseId == PhaseId);

            return qry;
        }

        public ProjectBudgetCodeCalPhase ProcessUpdate(ProjectBudgetCodeCalPhase model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetProjectBudgetCodeCalPhase(model.PMCo, model.BudgetCodeId, model.PhaseId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.PhaseId = model.PhaseId;
                updObj.ActiveYN = model.ActiveYN;

                db.SaveChanges(modelState);
            }
            return updObj;
        }
        
        public ProjectBudgetCodeCalPhase Create(ProjectBudgetCodeCalPhase model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            db.ProjectBudgetCodeCalPhases.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(ProjectBudgetCodeCalPhase model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.ProjectBudgetCodeCalPhases.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(ProjectBudgetCodeCalPhase model)
        {
            var qry = from f in db.ProjectBudgetCodeCalPhases
                      where f.PMCo == model.PMCo && f.BudgetCodeId == model.BudgetCodeId && f.PhaseId == model.PhaseId
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

        ~ProjectBudgetCodeCalPhaseRepository()
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

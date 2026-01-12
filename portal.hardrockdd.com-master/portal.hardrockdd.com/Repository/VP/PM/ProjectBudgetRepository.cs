using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PM
{
    public partial class ProjectBudgetRepository : IDisposable
    {
        private VPContext db = new VPContext();        

        public ProjectBudgetRepository()
        {

        }
        
        public static ProjectBudget Init(BidBoreLine bore)
        {
            if (bore == null)
            {
                throw new ArgumentNullException(nameof(bore));
            }
            var model = new ProjectBudget
            {
                PMCo = bore.Job.JCCo,
                ProjectId = bore.JobId,
                BDCo = bore.BDCo,
                BidId = bore.BidId,
                BoreId = bore.BoreId,
                Description = bore.Package.Description,

                Job = bore.Job,
                BidBoreLine = bore,
                PMCompanyParm = bore.Job.JCCompanyParm.PMCompanyParm,
            };
            return model;
        }
        
        public ProjectBudget GetProjectBudget(byte PMCo, string ProjectId, string BudgetNo)
        {
            var qry = db.ProjectBudgets
                        .Where(f => f.PMCo == PMCo && f.ProjectId == ProjectId && f.BudgetNo == BudgetNo)
                        .FirstOrDefault();

            return qry;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ProjectBudgetRepository()
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

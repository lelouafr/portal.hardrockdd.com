using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class PRBatchTimeEntryRepository
    {

        public static PRBatchTimeEntry ProcessUpdate(Models.Views.HQ.Batch.PR.PRBatchTransViewModel model, VPContext db)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var mthDate = DateTime.TryParse(model.Mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            //var updObj = db.BidPackages.FirstOrDefault(f => f.Co == model.Co && f.BidId == model.BidId && f.PackageId == model.PackageId);
            var updObj = db.PRBatchTimeEntries.FirstOrDefault(f => f.Co == model.Co && f.Mth == mthDate && f.BatchId == model.BatchId && f.BatchSeq == model.BatchSeq);
            

            if (updObj != null)
            {
                var jobChanged = updObj.JobId != updObj.JobId;
                var jobPhaseChanged = updObj.PhaseId != updObj.PhaseId;
                var eqpChanged = updObj.EquipmentId != updObj.EquipmentId;

                var extAmtChange = updObj.Hours != model.Hours || updObj.Rate != model.Rate;

                if (jobChanged)
                {
                    var job = db.Jobs.FirstOrDefault(f => f.JCCo == model.JCCo && f.JobId == model.JobId);
                    model.JCCo = job.JCCo;
                    if (!job.Phases.Any(f => f.PhaseId == model.PhaseId))
                    {
                        var jobPhase = job.AddMasterPhase(model.PhaseId);
                        
                        model.PhaseGroupId = jobPhase.PhaseGroupId;
                        model.PhaseId = jobPhase.PhaseId;
                    }
                }
                if (eqpChanged)
                {
                    var eqp = db.Equipments.FirstOrDefault(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId);
                    model.EMCo = eqp.EMCo;
                    model.EMGroupId = eqp.EMGroupId;
                }

                /****Write the changes to temp object****/

                updObj.EmployeeId = model.EmployeeId;
                updObj.BatchTransType = model.BatchTransType;
                updObj.Type = model.Type;
                updObj.PostDate = model.PostDate;
                updObj.JCCo = model.JCCo;
                updObj.EquipmentId = model.EquipmentId;
                updObj.CostCodeId = model.CostCode;
                updObj.JobId = model.JobId;
                updObj.PhaseGroupId = model.PhaseGroupId;
                updObj.PhaseId = model.PhaseId;
                updObj.GLCo = model.GLCo;
                updObj.EMCo = model.EMCo;
                updObj.EMGroupId = model.EMGroupId;
                updObj.TaxState = model.TaxState;
                updObj.UnempState = model.UnempState;
                updObj.InsState = model.InsState;
                updObj.InsCode = model.InsCode;
                updObj.PRDept = model.PRDept;
                updObj.CrewId = model.CrewId;
                updObj.EarnCodeId = model.EarnCodeId;
                updObj.Hours = model.Hours;
                updObj.Rate = model.Rate;
                if (extAmtChange)
                {
                    updObj.Amt = updObj.Hours * updObj.Rate;
                }

                //updObj = ProcessUpdate(updObj, db);
            }
            return updObj;
        }
    }
}
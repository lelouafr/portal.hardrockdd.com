using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyEquipmentUsage
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.DailyTicket != null)
                    db = this.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public void GetEMGLAccount()
        {
            var glParm = new System.Data.Entity.Core.Objects.ObjectParameter("glacct", typeof(string));
            var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
            var funcErr = db.bspEMUsageGlacctDflt(
                EMCo,
                EMGroupId,
                EMTransType,
                JCCo,
                JobId,
                JCPhaseId,
                JCCostType,
                null,
                null,
                null,
                null,
                glParm,
                msgParm);
            if (funcErr == -1)
            {
                GLCo = Equipment.EMCompanyParm.GLCo;
                GLOffsetAcct = (string)glParm.Value;
            }
            else
            {
                GLCo = Equipment.EMCompanyParm.GLCo;
                GLOffsetAcct = null;
            }
        }


        public EMBatchUsage AddToBatch()
        {
            var mth = new DateTime(ActualDate.Value.Year, ActualDate.Value.Month, 1);

            var batch = Batch.FindCreate(this.Equipment.EMCompanyParm.HQCompanyParm, "EMBF", "EMRev", mth);
            batch.InUseBy = "WebPortalUser";

            var batchSeq = new EMBatchUsage
            {
                Co = batch.Co,
                BatchId = batch.BatchId,
                BatchSeq = batch.EMUsageBatches.DefaultIfEmpty().Max(f => f == null ? 0 : f.BatchSeq) + 1,
                Description = "RIG USAGE POSTING",
                Source = "EMRev",
                ActualDate = (DateTime)ActualDate,
                BatchTransType = BatchTransType,
                EMTransType = EMTransType,
                EMGroup = EMGroupId,
                EquipmentId = EquipmentId,
                RevCodeId = RevCodeId,
                PRCo = PRCo,
                PREmployee = PREmployee,
                JCCo = JCCo,
                JobId = JobId,
                JCPhaseId = JCPhaseId,
                JCCostType = JCCostType,
                PhaseGroupId = PhaseGroupId,
                UnitPrice = 0,
                RevTimeUnits = RevTimeUnits,
                RevWorkUnits = RevWorkUnits,
                RevRate = RevRate,
                RevDollars = RevDollars,
                UM = RevCode.Basis == "U" ? RevCode.WorkUM : RevCode.TimeUM,
                TimeUM = RevCode.Basis == "U" ? RevCode.WorkUM : RevCode.TimeUM,
                GLCo = GLCo ?? Equipment.EMCompanyParm.GLCo,
                GLOffsetAcct = GLOffsetAcct,
                OffsetGLCo = GLCo ?? Equipment.EMCompanyParm.GLCo,
                PreviousHourMeter = PreviousHourMeter ?? Equipment.HourReading,
                PreviousOdometer = PreviousOdometer ?? Equipment.OdoReading,
                CurrentHourMeter = CurrentHourMeter ?? Equipment.HourReading,
                CurrentOdometer = CurrentOdometer ?? Equipment.OdoReading,
                AutoUsage = "N",
                TicketId = TicketId,
                TicketSeqId = SeqId,

                Batch = batch
            };
            Status = 1;
            batch.EMUsageBatches.Add(batchSeq);

            return batchSeq;
        }
    }
}
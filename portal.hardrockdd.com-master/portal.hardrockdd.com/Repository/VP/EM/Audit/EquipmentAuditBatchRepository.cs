using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.EM;
using portal.Repository.VP.HQ;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;

namespace portal.Repository.VP.EM
{
    public static class EquipmentAuditBatchRepository
    {
        public static EMBatchUsage Init(Batch batch, EMAuditLine line)
        {
            if (batch == null) throw new System.ArgumentNullException(nameof(batch));
            if (line == null) throw new System.ArgumentNullException(nameof(line));

           // using var db = new VPContext();

            var parms = line.Equipment.EMCompanyParm;

            var model = new EMBatchUsage();
            model.Co = line.EMCo;
            model.BatchId = batch.BatchId;
            model.BatchSeq = batch.EMUsageBatches.DefaultIfEmpty().Max(f => f == null ? 0 : f.BatchSeq) + 1;
            model.Description = "Equipment Meter Posting";
            model.Source = batch.Source;
            model.ActualDate = ((line.HourDate ?? line.OdoDate) ?? DateTime.Now.Date);
            model.BatchTransType = "A";
            model.EMTransType = "Equip";
            model.EMGroup = line.EMCo;
            model.EquipmentId = line.EquipmentId;

            model.INStkUnitCost = 0;
            model.UnitPrice = 0;
            model.RevRate = 0;
            
            model.GLCo = parms.GLCo;
            model.OffsetGLCo = parms.GLCo;
            model.PreviousHourMeter = line.Equipment.HourReading;
            model.PreviousOdometer = line.Equipment.OdoReading;
            model.PreviousTotalHourMeter = line.Equipment.HourReading;
            model.PreviousTotalOdometer = line.Equipment.OdoReading;
            model.MeterReadDate = line.HourReading != null ? line.HourDate : line.OdoDate;
            
            model.CurrentHourMeter = line.HourReading ?? line.Equipment.HourReading;
            model.CurrentOdometer = line.OdoReading ?? line.Equipment.OdoReading;
            model.CurrentTotalHourMeter = line.HourReading ?? line.Equipment.HourReading;
            model.CurrentTotalOdometer = line.OdoReading ?? line.Equipment.OdoReading;

            model.MeterMiles = model.CurrentTotalOdometer - model.PreviousTotalOdometer;
            model.MeterHrs = model.CurrentTotalHourMeter - model.PreviousTotalHourMeter;
            model.AutoUsage = "N";

            model.TicketId = line.AuditId;
            model.TicketSeqId = line.SeqId;

            //JobPhaseCostRepository.FindCreateJobPhaseCost(model.EMCo, model.JobId, (byte)model.PhaseGrp, model.JCPhaseId, (byte)model.JCCostType, db);
            //var glParm = new ObjectParameter("glacct", typeof(string));
            //var msgParm = new ObjectParameter("msg", typeof(string));
            //var glErr = db.bspEMUsageGlacctDflt(model.EMCo, model.EMGroup, model.EMTransType, model.JCCo, model.JobId, model.JCPhaseId, model.JCCostType, null, null, null, null, glParm, msgParm);
            //if (glErr == -1)
            //{
            //    model.GLOffsetAcct = (string)glParm.Value;
            //}

            return model;
        }


        public static Batch AddtoBatch(EMAudit audit, VPContext db, ModelStateDictionary modelState)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));

            if (audit.Lines.Any(f => f.ActionId == (byte)DB.EMAuditLineActionEnum.Add || f.ActionId == (byte)DB.EMAuditLineActionEnum.Update))
            {
                //var tempDb = new VPContext();
                var mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
                var EMParms = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == audit.EMCo);
                var batch = BatchRepository.FindCreate("EMBF", "EMMeter", mth, db);
               // batch = tempDb.Batches.FirstOrDefault(f => f.EMCo == batch.EMCo && f.Mth == batch.Mth && f.BatchId == batch.BatchId);
                batch.InUseBy = "WebPortalUser";
                //tempDb.SaveChanges(modelState);

                foreach (var seq in audit.Lines.ToList())
                {
                    if (seq.Completed && (seq.ActionId == (byte)DB.EMAuditLineActionEnum.Add || seq.ActionId == (byte)DB.EMAuditLineActionEnum.Update) && seq.MeterTypeId != ((int)DB.EMMeterTypeEnum.None).ToString())
                    {
                        var exisitingEqp = db.EMBatchUsages.Where(f => f.EquipmentId == seq.EquipmentId && f.Co == seq.EMCo && f.Batch.TableName == batch.TableName && f.Batch.Source == batch.Source);
                        if (exisitingEqp.Any())
                        {
                            db.EMBatchUsages.RemoveRange(exisitingEqp);
                        }
                        var batchSeq = Init(batch, seq);
                        seq.BatchId = batchSeq.BatchId;
                        seq.BatchSeq = batchSeq.BatchSeq;
                        batch.EMUsageBatches.Add(batchSeq);

                        if (seq.Equipment.MeterTypeId != seq.MeterTypeId)
                        {
                            seq.Equipment.MeterTypeId = seq.MeterTypeId;
                        }
                    }
                }

                //tempDb.SaveChanges(modelState);
                //if (!modelState.IsValid)
                //{
                //    tempDb.Dispose();
                //    tempDb = new VPContext();
                //    batch = tempDb.Batches.FirstOrDefault(f => f.EMCo == batch.EMCo && f.Mth == batch.Mth && f.BatchId == batch.BatchId);
                //}
                //else
                //{
                //}

                if (EMParms.AuditAutoBatch == "Y")
                {
                    //Have to save the batch for the procedure to validate it.
                    db.SaveChanges(modelState);
                    var errorMsg = "";
                    var error = ValidateBatch(batch, db, ref errorMsg);
                    if (error)
                    {
                        modelState.AddModelError("", errorMsg);
                        batch.Status = 0;
                        batch.InUseBy = null;
                    }
                    else
                    {
                        error = PostBatch(batch, db, ref errorMsg);
                        if (error)
                        {
                            modelState.AddModelError("", errorMsg);
                            batch.Status = 0;
                            batch.InUseBy = null;
                        }
                    }
                }
                //batch.InUseBy = null;
                //tempDb.SaveChanges(modelState);
                //tempDb.Dispose();
                return batch;
            }
            return null;
        }


        public static bool ValidateBatch(Batch batch, VPContext db, ref string errorMsg)
        {
            if (batch == null) throw new System.ArgumentNullException(nameof(batch));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            try
            {
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = db.bspEMVal_Meters_Main(batch.Co, batch.Mth, batch.BatchId, msgParm);
                if (error != -1)
                {
                    errorMsg = (string)(msgParm.Value);
                }
                return error != -1;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return true;
            }

        }

        public static bool PostBatch(Batch batch, VPContext db, ref string errorMsg)
        {
            if (batch == null) throw new System.ArgumentNullException(nameof(batch));
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            try
            {
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = db.bspEMPost_Meters_Main(batch.Co, batch.Mth, batch.BatchId, DateTime.Now, msgParm);
                if (error != -1)
                {
                    errorMsg = (string)(msgParm.Value);
                }
                
                return error != -1;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return true;
            }

        }

    }
}
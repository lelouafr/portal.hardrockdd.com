//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Equipment;
//using portal.Repository.VP.HQ;
//using portal.Repository.VP.JC;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity.Core.Objects;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.EM
//{
//    public static class EquipmentUsageRepository
//    {
//        public static EMBatchUsage Init(DailyEquipmentUsage dailyUsage, Batch batch, VPContext db)
//        {
//            if (dailyUsage == null) throw new System.ArgumentNullException(nameof(dailyUsage));
//            if (batch == null) throw new System.ArgumentNullException(nameof(batch));
//            if (db == null) throw new System.ArgumentNullException(nameof(db));

//            if (dailyUsage == null)
//            {
//                throw new ArgumentNullException(nameof(dailyUsage));
//            }
//            var model = new EMBatchUsage();
//            model.Co = batch.Co;
//            model.BatchId = batch.BatchId;
//            model.BatchSeq = batch.EMUsageBatches.DefaultIfEmpty()
//                                             .Max(f => f == null ? 0 : f.BatchSeq) + 1;
//            model.Description = "RIG USAGE POSTING";
//            model.Source = "EMRev";
//            model.ActualDate = (DateTime)dailyUsage.ActualDate;
//            model.BatchTransType = dailyUsage.BatchTransType;
//            model.EMTransType = dailyUsage.EMTransType;
//            model.EMGroup = dailyUsage.EMGroupId;
//            model.EquipmentId = dailyUsage.EquipmentId;
//            model.RevCodeId = dailyUsage.RevCodeId;
//            model.PRCo = dailyUsage.PRCo;
//            model.PREmployee = dailyUsage.PREmployee;
//            model.JCCo = dailyUsage.DailyTicket.DailyJobTicket.Job.JCCo;
//            model.JobId = dailyUsage.DailyTicket.DailyJobTicket.JobId;
//            model.JCPhaseId = dailyUsage.JCPhaseId;
//            model.JCCostType = dailyUsage.JCCostType;
//            model.PhaseGroupId = dailyUsage.PhaseGroupId;

//            model.UnitPrice = 0;
//            model.RevTimeUnits = dailyUsage.RevTimeUnits;
//            model.RevWorkUnits = dailyUsage.RevWorkUnits;
//            model.RevRate = dailyUsage.RevRate;
//            model.RevDollars = dailyUsage.RevDollars;
//            model.UM = dailyUsage.RevCode.Basis == "U" ? dailyUsage.RevCode.WorkUM : dailyUsage.RevCode.TimeUM;
//            model.TimeUM = dailyUsage.RevCode.Basis == "U" ? dailyUsage.RevCode.WorkUM : dailyUsage.RevCode.TimeUM;

//            model.GLCo = dailyUsage.GLCo ?? dailyUsage.Equipment.EMCompanyParm.GLCo;
//            model.OffsetGLCo = dailyUsage.GLCo ?? dailyUsage.Equipment.EMCompanyParm.GLCo;
//            model.PreviousHourMeter = dailyUsage.PreviousHourMeter ?? dailyUsage.Equipment.HourReading;
//            model.PreviousOdometer = dailyUsage.PreviousOdometer ?? dailyUsage.Equipment.OdoReading;
//            model.CurrentHourMeter = dailyUsage.CurrentHourMeter ?? dailyUsage.Equipment.HourReading;
//            model.CurrentOdometer = dailyUsage.CurrentOdometer ?? dailyUsage.Equipment.OdoReading;

//            model.AutoUsage = "N";

//            model.TicketId = dailyUsage.TicketId;
//            model.TicketSeqId = dailyUsage.SeqId;

//            JobPhaseCostRepository.FindCreateJobPhaseCost((byte)model.JCCo, model.JobId, (byte)model.PhaseGroupId, model.JCPhaseId, (byte)model.JCCostType, db);
//            var glParm = new ObjectParameter("glacct", typeof(string));
//            var msgParm = new ObjectParameter("msg", typeof(string));
//            var glErr = db.bspEMUsageGlacctDflt(model.Co, model.EMGroup, model.EMTransType, model.JCCo, model.JobId, model.JCPhaseId, model.JCCostType, null, null, null, null, glParm, msgParm);
//            if (glErr == -1)
//            {
//                model.GLOffsetAcct = (string)glParm.Value;
//            }

//            dailyUsage.Status = 1;
//            return model;
//        }
         
//        public static void AddtoBatch(List<DailyEquipmentUsageListViewModel> list, VPContext db, ModelStateDictionary modelState)
//        {
//            if (list == null) throw new System.ArgumentNullException(nameof(list));
//            if (db == null) throw new System.ArgumentNullException(nameof(db));
//            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));



//            //var weeks = list.GroupBy(g => g.WeekId).Select(s => s.Key).ToList();
//            //var months = db.Calendars.Where(f => weeks.Contains(f.Week ?? 0)).GroupBy(g => new { g.Year, g.Month }).Select(s => new { s.Key.Year, s.Key.Month }).ToList();
//            //foreach (var month in months)
//            //{
//            //    var mth = new DateTime((int)month.Year, (int)month.Month, 1);
//            //    var batch = BatchRepository.FindCreate("EMBF", "EMRev", mth, db);
//            //    var seqList = new List<EMBatchUsage>();
//            //    batch.InUseBy = "WebPortalUser";
//            //    db.SaveChanges(modelState);

//            //    foreach (var item in list)
//            //    {
//            //        var entries = db.DailyEquipmentUsages.Where(f => f.DTCo == item.DTCo &&
//            //                                                         f.Status == (int)DB.EMUsageTransStatusEnum.New &&
//            //                                                         f.Calendar.Week == item.WeekId &&
//            //                                                         f.EquipmentId == item.EquipmentId &&
//            //                                                         f.Calendar.Year == month.Year &&
//            //                                                         f.Calendar.Month == month.Month).ToList();

//            //        foreach (var entry in entries)
//            //        {
//            //            if (!batch.EMUsageBatches.Any(f => f.TicketId == entry.TicketId && f.TicketSeqId == entry.SeqId))
//            //            {

//            //                var batchSeq = Init(entry, batch, db);
//            //                batch.EMUsageBatches.Add(batchSeq);
//            //                seqList.Add(batchSeq);
//            //            }
//            //            entry.Status = (int)DB.EMUsageTransStatusEnum.Posted;

//            //        }
//            //    }
//            //    db.SaveChanges(modelState);
//            //    if (modelState.IsValid == false)
//            //    {
//            //        foreach (var seq in seqList)
//            //        {
//            //            batch.EMUsageBatches.Remove(seq);
//            //            var dailyEntry = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == seq.Co && f.TicketId == seq.TicketId && f.SeqId == seq.TicketSeqId);
//            //            dailyEntry.Status = (int)DB.EMUsageTransStatusEnum.New;
//            //        }
//            //        db.SaveChanges(modelState);
//            //    }
//            //    batch.InUseBy = null;
//            //    db.SaveChanges(modelState);
//            //}
            
//        }

//        public static DailyEquipmentUsageViewModel ProcessUpdate(DailyEquipmentUsage updObj, DailyEquipmentUsageViewModel model)
//        {
//            if (updObj == null) throw new System.ArgumentNullException(nameof(updObj));
//            if (model == null) throw new System.ArgumentNullException(nameof(model));

//            using var db = new VPContext();
//            //var updObj = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.SeqId == model.SeqId);
//            if (updObj != null)
//            {
                
//                updObj.BatchTransType = model.BatchTransType;
//                updObj.EquipmentId = model.EquipmentId;
//                updObj.RevCodeId = model.RevCode;
//                updObj.JobId = model.JobId;
//                updObj.JCPhaseId = model.PhaseId;
//                updObj.JCCostType = model.JCCostType;
//                updObj.Status = (int)model.Status;
//                updObj.RevRate = model.RevRate;
//                var glParm = new ObjectParameter("glacct", typeof(string));
//                var msgParm = new ObjectParameter("msg", typeof(string));
//                var glErr = db.bspEMUsageGlacctDflt(updObj.EMCo, updObj.EMGroupId, updObj.BatchTransType, updObj.JCCo, updObj.JobId, updObj.JCPhaseId, updObj.JCCostType, null, null, null, null, glParm, msgParm);
//                if (glErr == -1)
//                {
//                    model.GLOffsetAcct = (string)glParm.Value;
//                }
                
//            }
//            return new DailyEquipmentUsageViewModel(updObj);
//        }
//    }
//}
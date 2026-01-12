using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Equipment;
using System;
using System.Data.Entity.Core.Objects;

namespace portal.Repository.VP.DT
{
    public static class DailyEquipmentUsageRepository
    {
        //public static DailyEquipmentUsage Init(DailyJobTask task, VPContext db)
        //{
        //    if (task == null) throw new System.ArgumentNullException(nameof(task));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));


        //    if (task == null)
        //    {
        //        throw new ArgumentNullException(nameof(task));
        //    }
        //    var rig = task.DailyTicket.DailyJobTicket.Rig;
        //    var model = new DailyEquipmentUsage
        //    {
        //        DTCo = task.DTCo,
        //        TicketId = task.TicketId,
        //        SeqId = task.DailyTicket.EquipmentUsages.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
        //        ActualDate = task.WorkDate,
        //        BatchTransType = "A",
        //        EMTransType = "J",
        //        EMGroupId = task.DailyTicket.DailyJobTicket.Rig.EMGroupId,
        //        EquipmentId = task.DailyTicket.DailyJobTicket.RigId,
        //        RevCodeId = rig.RevenueCodeId ?? "2",
        //        PRCo = task.DTCo,
        //        JCCo = task.DailyTicket.DailyJobTicket.Job.JCCo,
        //        JobId = task.DailyTicket.DailyJobTicket.JobId,
        //        PhaseGroupId = task.PhaseGroupId ?? task.DTCo,
        //        JCPhaseId = task.ParentPhaseId,
        //        JCCostType = rig.UsageCostType ?? rig.EMCompanyParm.udJCUsageCt,
        //        Status = 0,
        //        RevTimeUnits = task.ValueAdj ?? task.Value,
        //        RevRate = 0,
        //        GLCo = task.DTCo,
        //        PreviousHourMeter = rig.HourReading,
        //        PreviousOdometer = rig.OdoReading,
        //        CurrentHourMeter = rig.HourReading,
        //        CurrentOdometer = rig.OdoReading
        //    };

        //    var revCode = rig.RevenueCodes.FirstOrDefault(f => f.RevCode == model.RevCodeId);
        //    if (revCode != null)
        //    {
        //        model.RevRate = revCode.Rate;
        //    }
        //    JobPhaseCostRepository.FindCreateJobPhaseCost((byte)model.JCCo, model.JobId, (byte)model.PhaseGroupId, model.JCPhaseId, (byte)model.JCCostType, db);
        //    var glParm = new ObjectParameter("glacct", typeof(string));
        //    var msgParm = new ObjectParameter("msg", typeof(string));
        //    var glErr = db.bspEMUsageGlacctDflt(model.EMCo, model.EMGroupId, model.EMTransType, model.JCCo, model.JobId, model.JCPhaseId, model.JCCostType, null, null, null, null, glParm, msgParm);
        //    if (glErr == -1)
        //    {
        //        model.GLOffsetAcct = (string)glParm.Value;
        //    }
        //    return model;
        //}

        //public static void CreateBatchEntries(DailyTicket ticket, VPContext db)
        //{
        //    if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));



        //    if (ticket.FormId == 1)
        //    {
        //        var usages = db.DailyEquipmentUsages.Where(f => f.ActualDate == ticket.WorkDate && f.EquipmentId == ticket.DailyJobTicket.RigId).ToList();
        //        foreach (var usage in usages)
        //        {
        //            usage.RevTimeUnits = 0;
        //            usage.RevWorkUnits = 0;
        //            usage.RevDollars = 0;
        //        }
        //        var tasks = db.DailyJobTasks.Where(f => (f.JobPhaseCost.UnitofMeasure.UM == "Hours" || f.JobPhaseCost.UnitofMeasure.UM == "HRS") && 
        //                                                 f.CostType.JBCostTypeCategory == "L" &&
        //                                                 f.WorkDate == ticket.WorkDate && 
        //                                                 //f.ParentPhaseId != null &&
        //                                                 f.DailyTicket.DailyJobTicket.tRigId == ticket.DailyJobTicket.tRigId &&
        //                                                 (
        //                                                    f.DailyTicket.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted ||
        //                                                    f.DailyTicket.tStatusId == (int)DB.DailyTicketStatusEnum.Approved ||
        //                                                    f.DailyTicket.tStatusId == (int)DB.DailyTicketStatusEnum.Processed
        //                                                 )
        //                                                 ).ToList();

        //        var totalHours = tasks.Sum(s => (s.ValueAdj ?? s.Value)) ?? 0;
        //        var totalUsage = 0m;
        //        var acumHours = 0m;
        //        foreach (var task in tasks)
        //        {
        //            var hours = (task.ValueAdj ?? task.Value) ?? 0;
        //            acumHours += hours;
        //            if (hours > 0 && totalHours > 0)
        //            {
        //                var usage = db.DailyEquipmentUsages.FirstOrDefault(f => f.TicketId == task.TicketId && f.JCPhaseId == task.ParentPhaseId);
        //                if (usage == null)
        //                {
        //                    usage = db.DailyEquipmentUsages.Local.FirstOrDefault(f => f.TicketId == task.TicketId && f.JCPhaseId == task.ParentPhaseId);
        //                }
        //                var unitCal = hours / totalHours;
        //                unitCal = Math.Round(unitCal, 3);
        //                if (usage == null)
        //                {
        //                    usage = Init(task, db);
        //                    usage.RevWorkUnits = (unitCal );
        //                    usage.RevDollars = usage.RevRate * usage.RevWorkUnits;
        //                    db.DailyEquipmentUsages.Add(usage);
        //                }
        //                else
        //                {
        //                    usage.RevWorkUnits += unitCal;
        //                    usage.RevTimeUnits += hours;
        //                    usage.RevDollars = usage.RevRate * usage.RevWorkUnits;
        //                }
        //                totalUsage += unitCal;
        //            }
        //        }

        //        if (totalUsage != 1m)
        //        {
        //            var dif = 1 - totalUsage;
        //            var firstUsage = db.DailyEquipmentUsages.FirstOrDefault(f => f.ActualDate == ticket.WorkDate && f.EquipmentId == ticket.DailyJobTicket.RigId);
        //            if (firstUsage == null)
        //            {
        //                firstUsage = db.DailyEquipmentUsages.Local.FirstOrDefault(f => f.ActualDate == ticket.WorkDate && f.EquipmentId == ticket.DailyJobTicket.RigId);
        //            }
        //            if (firstUsage != null)
        //            {
        //                firstUsage.RevWorkUnits += dif;
        //                firstUsage.RevDollars = firstUsage.RevRate * firstUsage.RevWorkUnits;
        //            }
        //        }
        //    }
        //}


        public static DailyEquipmentUsageViewModel ProcessUpdate(DailyEquipmentUsage updObj,DailyEquipmentUsageViewModel model)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            //var updObj = db.DailyEquipmentUsages.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.SeqId == model.SeqId);
            if (updObj != null)
            {
                
                updObj.BatchTransType = model.BatchTransType;
                updObj.EquipmentId = model.EquipmentId;
                updObj.RevCodeId = model.RevCode;
                updObj.JobId = model.JobId;
                updObj.JCPhaseId = model.PhaseId;
                updObj.JCCostType = model.JCCostType;
                updObj.Status = (int)model.Status;
                updObj.RevRate = model.RevRate;
                var glParm = new ObjectParameter("glacct", typeof(string));
                var msgParm = new ObjectParameter("msg", typeof(string));
                var glErr = db.bspEMUsageGlacctDflt(updObj.EMCo, updObj.EMGroupId, updObj.BatchTransType, updObj.JCCo, updObj.JobId, updObj.JCPhaseId, updObj.JCCostType, null, null, null, null, glParm, msgParm);
                if (glErr == -1)
                {
                    model.GLOffsetAcct = (string)glParm.Value;
                }
                
            }
            return new DailyEquipmentUsageViewModel(updObj);
        }
    }
}
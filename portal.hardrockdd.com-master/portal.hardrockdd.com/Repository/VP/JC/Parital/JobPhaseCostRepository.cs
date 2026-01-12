//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class JobPhaseCostRepository //: IDisposable
//    {
//        //public static JobPhaseCost Init(JobPhaseCost jobPhaseCost)
//        //{
//        //    if (jobPhaseCost == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhaseCost));
//        //    }
//        //    var model = new JobPhaseCost
//        //    {
//        //        //JCCo = jobPhaseCost.JCCo,
//        //        //JobId = jobPhaseCost.JobId,
//        //        PhaseGroupId = jobPhaseCost.PhaseGroupId,
//        //        PhaseId = jobPhaseCost.PhaseId,
//        //        CostTypeId = jobPhaseCost.CostTypeId,
//        //        UM = jobPhaseCost.UM,
//        //        BillFlag = jobPhaseCost.BillFlag,
//        //        ItemUnitFlag = jobPhaseCost.ItemUnitFlag,
//        //        PhaseUnitFlag = jobPhaseCost.PhaseUnitFlag,
//        //        BuyOutYN = jobPhaseCost.BuyOutYN,
//        //        LastProjDate = jobPhaseCost.LastProjDate,
//        //        Plugged = jobPhaseCost.Plugged,
//        //        ActiveYN = jobPhaseCost.ActiveYN,
//        //        OrigHours = jobPhaseCost.OrigHours,
//        //        OrigUnits = jobPhaseCost.OrigUnits,
//        //        OrigCost = jobPhaseCost.OrigCost,
//        //        ProjNotes = jobPhaseCost.ProjNotes,
//        //        SourceStatus = jobPhaseCost.SourceStatus,
//        //        InterfaceDate = jobPhaseCost.InterfaceDate,
//        //        Notes = jobPhaseCost.Notes,
//        //    };
//        //    return model;
//        //}

//        //public static JobPhaseCost Init(PhaseMasterCost jobPhaseCost, Job job)
//        //{
//        //    if (jobPhaseCost == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhaseCost));
//        //    }
//        //    if (job == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(job));
//        //    }
//        //    var model = new JobPhaseCost
//        //    {
//        //        JCCo = job.JCCo,
//        //        JobId = job.JobId,
//        //        PhaseGroupId = jobPhaseCost.PhaseGroupId,
//        //        PhaseId = jobPhaseCost.PhaseId,
//        //        CostTypeId = jobPhaseCost.CostTypeId,
//        //        UM = jobPhaseCost.UM,
//        //        BillFlag = jobPhaseCost.BillFlag,
//        //        ItemUnitFlag = jobPhaseCost.ItemUnitFlag,
//        //        PhaseUnitFlag = jobPhaseCost.PhaseUnitFlag,
//        //        BuyOutYN = "N",
//        //        Plugged = "N",
//        //        ActiveYN = jobPhaseCost.Phase.ActiveYN,
//        //        OrigHours = 0,
//        //        OrigUnits = 0,
//        //        OrigCost = 0,
//        //        ProjNotes = null,
//        //        SourceStatus = "J",
//        //    };




//        //    return model;
//        //}

//        //public static JobPhaseCost Init(PhaseMasterCost jobPhaseCost, JobPhase jobPhase)
//        //{
//        //    if (jobPhaseCost == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhaseCost));
//        //    }
//        //    if (jobPhase == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhase));
//        //    }
//        //    var model = new JobPhaseCost
//        //    {
//        //        JCCo = jobPhase.JCCo,
//        //        JobId = jobPhase.JobId,
//        //        PhaseGroupId = jobPhase.PhaseGroupId,
//        //        PhaseId = jobPhase.PhaseId,
//        //        CostTypeId = jobPhaseCost.CostTypeId,
//        //        UM = jobPhaseCost.UM,
//        //        BillFlag = jobPhaseCost.BillFlag,
//        //        ItemUnitFlag = jobPhaseCost.ItemUnitFlag,
//        //        PhaseUnitFlag = jobPhaseCost.PhaseUnitFlag,
//        //        BuyOutYN = "N",
//        //        Plugged = "N",
//        //        ActiveYN = jobPhaseCost.Phase.ActiveYN,
//        //        OrigHours = 0,
//        //        OrigUnits = 0,
//        //        OrigCost = 0,
//        //        ProjNotes = null,
//        //        SourceStatus = "J",
//        //    };




//        //    return model;
//        //}

//        //public static JobPhaseCost Init(PhaseMasterCost jobPhaseCost, JobPhase jobPhase)
//        //{
//        //    if (jobPhaseCost == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhaseCost));
//        //    }
//        //    if (jobPhase == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhase));
//        //    }
//        //    var model = new JobPhaseCost
//        //    {
//        //        JCCo = jobPhase.JCCo,
//        //        JobId = jobPhase.JobId,
//        //        JobPhase = jobPhase,

//        //        PhaseGroupId = jobPhaseCost.PhaseGroupId,
//        //        PhaseId = jobPhaseCost.PhaseId,
//        //        CostTypeId = jobPhaseCost.CostTypeId,
//        //        UM = jobPhaseCost.UM,
//        //        BillFlag = jobPhaseCost.BillFlag,
//        //        ItemUnitFlag = jobPhaseCost.ItemUnitFlag,
//        //        PhaseUnitFlag = jobPhaseCost.PhaseUnitFlag,
//        //        BuyOutYN = "N",
//        //        Plugged = "N",
//        //        ActiveYN = jobPhaseCost.Phase.ActiveYN,
//        //        OrigHours = 0,
//        //        OrigUnits = 0,
//        //        OrigCost = 0,
//        //        ProjNotes = null,
//        //        SourceStatus = "J",
//        //    };
//        //    return model;
//        //}

//        //public static void CopyJob(Job srcJob, Job dstJob)
//        //{
//        //    if (srcJob == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(srcJob));
//        //    }
//        //    if (dstJob == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(dstJob));
//        //    }

//        //    foreach (var phase in srcJob.Phases)
//        //    {
//        //        var dstPhase = dstJob.Phases.FirstOrDefault(f => f.PhaseId.Trim() == phase.PhaseId.Trim());
//        //        if (dstPhase == null)
//        //        {
//        //            var listItem = JobPhaseRepository.Init(phase);
//        //            listItem.JobId = dstJob.JobId;
//        //            listItem.JCCo = dstJob.JCCo;
//        //            listItem.ContractId = dstJob.ContractId;
//        //            dstJob.Phases.Add(listItem);
//        //            dstPhase = dstJob.Phases.FirstOrDefault(f => f.PhaseId.Trim() == phase.PhaseId.Trim());
//        //        }
//        //        var list = phase.JobPhaseCosts
//        //                 .Where(f => f.ActiveYN == "Y" &&
//        //                             !dstPhase.JobPhaseCosts.Any(a => a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                              a.PhaseId.Trim() == f.PhaseId.Trim() &&
//        //                                                              a.CostTypeId == f.CostTypeId)).ToList();
//        //        foreach (var item in list)
//        //        {
//        //            var listItem = Init(item);
//        //            listItem.JCCo = dstJob.JCCo;
//        //            listItem.JobId = dstJob.JobId;
//        //            dstPhase.JobPhaseCosts.Add(listItem);
//        //        }
//        //    }
            
//        //}

//        //public void CopyMaster(byte dstCo, string dstJobId, string dstPhaseId, ModelStateDictionary modelState)
//        //{
//        //    var job = db.Jobs.FirstOrDefault(f => f.JCCo == dstCo && f.JobId == dstJobId);
//        //    var list = db.PhaseMasterCosts
//        //                 .Where(f => f.Phase.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == dstCo &&
//        //                             f.PhaseId == dstPhaseId &&
//        //                             !db.JobPhaseCosts.Any(a => a.JCCo == job.JCCo &&
//        //                                                   a.JobId == job.JobId &&
//        //                                                   a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                   a.PhaseId == f.PhaseId &&
//        //                                                   a.CostTypeId == f.CostTypeId))
//        //                 .ToList();

//        //    var newList = new List<JobPhaseCost>();
//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, job);
//        //        listItem.JCCo = job.JCCo;
//        //        listItem.JobId = job.JobId;
//        //        newList.Add(listItem);
//        //    }

//        //    db.JobPhaseCosts.AddRange(newList);
//        //    db.SaveChanges(modelState);
//        //}


//        //public static void CopyMaster(JobPhase jobPhase)
//        //{
//        //    using var db = new VPContext();

//        //    var list = db.PhaseMasterCosts
//        //                 .Where(f => f.Phase.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == jobPhase.PhaseGroupId &&
//        //                             f.PhaseId == jobPhase.PhaseId &&
//        //                             !db.JobPhaseCosts.Any(a => a.JCCo == jobPhase.JCCo &&
//        //                                                   a.JobId == jobPhase.JobId &&
//        //                                                   a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                   a.PhaseId == f.PhaseId &&
//        //                                                   a.CostTypeId == f.CostTypeId))
//        //                 .ToList();

//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, jobPhase);
//        //        jobPhase.JobPhaseCosts.Add(listItem);
//        //        //listItem.CostType = item.CostType;
//        //    }

//        //}

//        //public static List<JobPhaseCost> CopyTemplate(JobPhase jobPhase)
//        //{
//        //    if (jobPhase == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhase));
//        //    }
//        //    var newList = jobPhase.JobPhaseCosts.ToList();
//        //    var src = jobPhase.Job.Company.JobParameter.TemplateJob.Phases.FirstOrDefault(f => f.PhaseId == jobPhase.PhaseId);
//        //    if (src != null)
//        //    {
//        //        var list = src.JobPhaseCosts
//        //                     .Where(f => f.JobPhase.ActiveYN == "Y" &&
//        //                                 !jobPhase.JobPhaseCosts.Any(a => a.CostTypeId == f.CostTypeId))
//        //                     .ToList();

//        //        foreach (var item in list)
//        //        {
//        //            var listItem = Init(item);
//        //            listItem.JCCo = jobPhase.JCCo;
//        //            listItem.JobId = jobPhase.JobId;
//        //            newList.Add(listItem);
//        //        }
//        //    }

//        //    return newList;
//        //}

//        //public static void CopyTemplate(JobPhase jobPhase)
//        //{
//        //    if (jobPhase == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhase));
//        //    }
//        //    var src = jobPhase.Job.JCCompanyParm.TemplateJob.Phases.FirstOrDefault(f => f.PhaseId == jobPhase.PhaseId);
//        //    if (src != null)
//        //    {
//        //        var list = src.JobPhaseCosts
//        //                     .Where(f => f.ActiveYN == "Y")
//        //                     .ToList();

//        //        foreach (var item in list)
//        //        {
//        //            var jobPhaseCost = jobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == item.CostTypeId);
//        //            if (jobPhaseCost == null)
//        //            {
//        //                jobPhaseCost = Init(item);
//        //                jobPhaseCost.JCCo = jobPhase.JCCo;
//        //                jobPhaseCost.JobId = jobPhase.JobId;
//        //                jobPhaseCost.JobPhase = jobPhase;
//        //                jobPhase.JobPhaseCosts.Add(jobPhaseCost);
//        //            }
//        //        }
//        //    }

//        //}

//        //public static List<JobPhaseCost> CopyMaster(JobPhase jobPhase)
//        //{
//        //    using var db = new VPContext();
//        //    var list = db.PhaseMasterCosts
//        //                 .Where(f => f.Phase.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == jobPhase.PhaseGroupId &&
//        //                             f.PhaseId == jobPhase.PhaseId &&
//        //                             !db.JobPhaseCosts.Any(a => a.JCCo == jobPhase.JCCo &&
//        //                                                   a.JobId == jobPhase.JobId &&
//        //                                                   a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                   a.PhaseId == f.PhaseId &&
//        //                                                   a.CostTypeId == f.CostTypeId))
//        //                 .ToList();

//        //    var newList = new List<JobPhaseCost>();
//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, jobPhase);
//        //        listItem.JCCo = jobPhase.JCCo;
//        //        listItem.JobId = jobPhase.JobId;

//        //        newList.Add(listItem);
//        //    }

//        //    return newList;
//        //}



//        //public static JobPhaseCost FindCreateJobPhaseCost(byte co, string jobId, byte phaseGroup, string phaseId, byte costTypeId, VPContext db)
//        //{
//        //    if (db == null) throw new System.ArgumentNullException(nameof(db));
            
//        //    var jobPhase = db.JobPhases.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId && f.PhaseGroupId == phaseGroup && f.PhaseId == phaseId);
//        //    if (jobPhase == null)
//        //    {
//        //        jobPhase = db.JobPhases.Local.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId && f.PhaseGroupId == phaseGroup && f.PhaseId == phaseId);
//        //        if (jobPhase == null)
//        //        {
//        //            jobPhase = JobPhaseRepository.FindCreateJobPhase(co, jobId, phaseGroup, phaseId, db);
//        //        }
//        //    }

//        //    var jobPhaseCost = jobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == costTypeId);
//        //    if (jobPhaseCost == null)
//        //    {
//        //        jobPhaseCost = db.JobPhaseCosts.Local.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId && f.PhaseGroupId == phaseGroup && f.PhaseId == phaseId && f.CostTypeId == costTypeId);
//        //    }
//        //    if (jobPhaseCost == null)
//        //    {
//        //        var job = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId);
//        //        var jobPhaseCostTemp = job.JCCompanyParm.TemplateJob.Phases.FirstOrDefault(f => f.PhaseId == phaseId ).JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == costTypeId);
//        //        if (jobPhaseCostTemp != null)
//        //        {
//        //            jobPhaseCost = Init(jobPhaseCostTemp);
//        //            jobPhaseCost.PhaseGroupId = (byte)job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
//        //            jobPhaseCost.JobId = job.JobId;
//        //            jobPhase.JobPhaseCosts.Add(jobPhaseCost);
//        //        }
//        //    }

//        //    return jobPhaseCost;
//        //}
//    }
//}

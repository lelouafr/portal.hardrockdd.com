//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class JobPhaseRepository //: IDisposable
//    {
//        //public List<JobPhase> GetJobPhases(byte JCCo, string JobId, byte PhaseGroupId, string PhaseType, bool parentOnly = true)
//        //{
//        //    var qry = db.JobPhases
//        //                .Where(f => f.JCCo == JCCo && f.JobId == JobId && f.PhaseGroupId == PhaseGroupId && f.PhaseType == PhaseType && f.ParentPhaseId == (parentOnly ? null : f.ParentPhaseId))
//        //                .ToList();

//        //    return qry;
//        //}

//        public static JobPhase Init(JobPhase jobPhase)
//        {
//            if (jobPhase == null)
//            {
//                throw new System.ArgumentNullException(nameof(jobPhase));
//            }
//            var model = new JobPhase
//            {
//                PhaseGroupId = jobPhase.PhaseGroupId,
//                PhaseId = jobPhase.PhaseId,
//                Description = jobPhase.Description,
//                Item = jobPhase.Item,
//                ProjMinPct = jobPhase.ProjMinPct,
//                ActiveYN = jobPhase.ActiveYN,
//                Notes = jobPhase.Notes,
//                InsCode = jobPhase.InsCode,
//                PhaseType = jobPhase.PhaseType,
//                ParentPhaseId = jobPhase.ParentPhaseId,
//                IsMilestone = jobPhase.IsMilestone,
//                ProductionSortId = jobPhase.ProductionSortId,

//            };

//            return model;
//        }

//        //public static JobPhase Init(PhaseMaster jobPhase, Job job)
//        //{
//        //    if (job == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(job));
//        //    }
//        //    if (jobPhase == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(jobPhase));
//        //    }
//        //    var model = new JobPhase
//        //    {
//        //        JCCo = job.JCCo,
//        //        JobId = job.JobId,
//        //        ContractId = job.ContractId,
//        //        PhaseGroupId = jobPhase.PhaseGroupId,
//        //        PhaseId = jobPhase.PhaseId,
//        //        Item = "               1",
//        //        Description = jobPhase.Description,
//        //        ProjMinPct = jobPhase.ProjMinPct,
//        //        ActiveYN = jobPhase.ActiveYN,
//        //        Notes = jobPhase.Notes,
//        //        PhaseType = jobPhase.PhaseType,
//        //        ParentPhaseId = jobPhase.ParentPhaseId,
//        //        IsMilestone = jobPhase.IsMilestone,
//        //        ProductionSortId = jobPhase.SortId,
//        //    };
//        //    model.JobPhaseCosts = new List<JobPhaseCost>();
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
//        //    var list = srcJob.Phases
//        //                 .Where(f => !dstJob.Phases.Any(a => a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                    a.PhaseId == f.PhaseId))
//        //                 .ToList();
//        //    var newList = new List<JobPhase>();
//        //    foreach (var jobPhase in list)
//        //    {
//        //        var listItem = Init(jobPhase);
//        //        listItem.JobId = dstJob.JobId;
//        //        listItem.JCCo = dstJob.JCCo;
//        //        listItem.ContractId = dstJob.ContractId;
//        //        dstJob.Phases.Add(listItem);
//        //    }             
//        //}

//        //public static JobPhase FindCreateJobPhase(byte co, string jobId, byte phaseGroup, string phaseId, VPContext db)
//        //{
//        //    if (db == null) throw new System.ArgumentNullException(nameof(db));
//        //    var jobPhase = db.JobPhases.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId && f.PhaseGroupId == phaseGroup && f.PhaseId == phaseId);
//        //    if (jobPhase == null)
//        //    {
//        //        var job = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId);
//        //        var jobPhaseTemp = job.JCCompanyParm.TemplateJob.Phases.FirstOrDefault(f => f.PhaseId == phaseId);
//        //        if (jobPhaseTemp != null)
//        //        {
//        //            jobPhase = Init(jobPhaseTemp);
//        //            jobPhase.PhaseGroupId = (byte)job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
//        //            jobPhase.JobId = job.JobId;
//        //            jobPhase.ContractId = job.ContractId;
//        //            jobPhase.Job = job;
//        //            job.Phases.Add(jobPhase);

//        //        }
//        //    }

//        //    return jobPhase;
//        //}

//        //public static JobPhase FindCreateJobPhase(Job job, byte phaseGroup, string phaseId, VPContext db)
//        //{
//        //    var jobPhase = job.Phases.FirstOrDefault(f => f.PhaseGroupId == phaseGroup && f.PhaseId == phaseId);
//        //    if (jobPhase == null)
//        //    {
//        //        var jobPhaseTemp = job.JCCompanyParm.TemplateJob.Phases.FirstOrDefault(f => f.PhaseId == phaseId);
//        //        if (jobPhaseTemp != null)
//        //        {
//        //            jobPhase = Init(jobPhaseTemp);
//        //            jobPhase.PhaseGroupId = (byte)job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
//        //            jobPhase.JobId = job.JobId;
//        //            jobPhase.ContractId = job.ContractId;
//        //            jobPhase.Job = job;
//        //            jobPhase.JCCo = job.JCCo;
//        //            job.Phases.Add(jobPhase);

//        //        }
//        //    }

//        //    return jobPhase;
//        //}

//        //public void CopyMaster(byte dstCo, string dstJobId, string dstPhaseId, ModelStateDictionary modelState)
//        //{
//        //    var job = db.Jobs.FirstOrDefault(f => f.JCCo == dstCo && f.JobId == dstJobId);
//        //    var list = db.PhaseMasters
//        //                 .Where(f => f.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == dstCo &&
//        //                             f.PhaseId == dstPhaseId &&
//        //                             !db.JobPhases.Any(a => a.JCCo == job.JCCo &&
//        //                                                   a.JobId == job.JobId &&
//        //                                                   a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                   a.PhaseId == f.PhaseId))
//        //                 .ToList();

//        //    var newList = new List<JobPhase>();
//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, job);
//        //        newList.Add(listItem);
//        //    }

//        //    db.JobPhases.AddRange(newList);
//        //    db.SaveChanges(modelState);
//        //}

//        //public static JobPhase CopyMaster(Job job, string phaseId)
//        //{
//        //    if (job == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(job));
//        //    }
//        //    using var db = new VPContext();
//        //    var phase = job.JCCompanyParm.HQCompanyParm.PhaseGroup.JCPhases
//        //                 .Where(f => f.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == job.JCCo &&
//        //                             f.PhaseId == phaseId)
//        //                 .FirstOrDefault();
//        //    if (phase != null)
//        //    {

//        //        var jobPhase = job.Phases.FirstOrDefault(a => a.PhaseGroupId == phase.PhaseGroupId && a.PhaseId == phase.PhaseId);
//        //        if (jobPhase == null)
//        //        {
//        //            jobPhase = Init(phase, job);
//        //            job.Phases.Add(jobPhase);
//        //            //jobPhase.Job = job;
//        //        }
//        //        return jobPhase;
//        //    }
//        //    return null;
//        //}

//        //public static List<JobPhase> CopyMaster(Job job)
//        //{
//        //    using var db = new VPContext();
//        //    var list = db.PhaseMasters
//        //                 .Where(f => f.ActiveYN == "Y" &&
//        //                             f.PhaseGroupId == job.JCCo &&
//        //                             !db.JobPhases.Any(a => a.JCCo == job.JCCo &&
//        //                                                    a.JobId == job.JobId &&
//        //                                                    a.PhaseGroupId == f.PhaseGroupId &&
//        //                                                    a.PhaseId == f.PhaseId))
//        //                 .ToList();

//        //    var newList = new List<JobPhase>();
//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, job);

//        //        newList.Add(listItem);
//        //    }

//        //    return newList;
//        //}
        
//        //public static void CopyTemplate(Job job)
//        //{
//        //    if (job == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(job));
//        //    }
//        //    var src = job.JCCompanyParm.TemplateJob;
//        //    var list = src.Phases.Where(f => f.ActiveYN == "Y").ToList();

//        //    foreach (var item in list)
//        //    {
//        //        var jobPhase = job.Phases.FirstOrDefault(f => f.PhaseGroupId == item.PhaseGroupId && f.PhaseId == item.PhaseId);
//        //        if (jobPhase == null)
//        //        {
//        //            jobPhase = Init(item);
//        //            jobPhase.PhaseGroupId = (byte)job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
//        //            jobPhase.JobId = job.JobId;
//        //            jobPhase.ContractId = job.ContractId;
//        //            jobPhase.Job = job;
//        //            job.Phases.Add(jobPhase);
//        //        }

//        //    }
//        //}
//    }
//}

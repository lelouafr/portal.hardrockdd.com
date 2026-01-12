//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace portal.Repository.VP.JC
//{
//    public static class JobProgressRepository
//    {

//        #region BidBoreline
//        //public static JobPhaseTrack Init(BidBoreLinePass borePass)
//        //{
//        //    var result = new JobPhaseTrack
//        //    {
//        //        JCCo = borePass.BoreLine.Job.JCCo,
//        //        JobId = borePass.BoreLine.Job.JobId,
//        //        PhaseGroupId = (byte)borePass.BoreLine.Bid.Company.PhaseGroupId,
//        //        PhaseId = borePass.PhaseId,
//        //        PassId = (byte)borePass.PassId,
//        //        ActualDuration = 0,
//        //        PercentCompletion = 0,
//        //        ActualUnits = 0,
                
//        //    };

//        //    return result;
//        //}
        
//        //public static JobPhaseTrack CreateUpdate(Job job, BidBoreLinePass borePass, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    var result = job.BuildProgressTrackingFromBidPhase(borePass);            
//        //    ProjectGanttRepository.CreateUpdate(result, db, dbSaveVerbose);
//        //    return result;
//        //}
//        #endregion

//        #region DailyJobTask
//        //public static JobPhaseTrack Init(DailyJobTask task)
//        //{
//        //    var result = new JobPhaseTrack
//        //    {
//        //        JCCo = task.Job.JCCo,
//        //        JobId = task.Job.JobId,
//        //        PhaseGroupId = task.PhaseGroupId ?? (byte)task.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
//        //        PhaseId = task.ParentPhaseId,
//        //        PassId = Convert.ToByte(task.PassId ?? 1),
//        //        ActualDuration = 0,
//        //        PercentCompletion = 0,
//        //        ActualUnits = 0,
//        //        ActualBudgetCodeId = task.ParentBudgetCodeId,
                
//        //    };

//        //    return result;
//        //}

//        //private static void Update(JobPhaseTrack jobProgress, DailyJobTask hourTask, DailyJobTask progressTask, Job job, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    var dayTotal = job.DailyJobTasks.Where(f => f.WorkDate == hourTask.WorkDate && f.CostTypeId == 1).Sum(sum => sum.PayrollValue);
//        //    var taskValue = hourTask.PayrollValue;
//        //    var taskValueWeight = dayTotal == 0 || taskValue == 0 ? 0 : taskValue / dayTotal;
            
//        //    jobProgress.ActualDuration += hourTask.CostTypeId == 1 ? taskValueWeight : 0;

//        //    var footage = job.Footage ?? 0;
//        //    var actualDuration = jobProgress.ActualDuration ?? 0;
//        //    var projectDuration = jobProgress.ProjectedDuration ?? 0;
//        //    //var remainingDuration = 0m;
//        //    var percentage = 0m;

//        //    if (progressTask != null)
//        //    {
//        //        var progressValue = progressTask.PayrollValue;
//        //        jobProgress.ActualUnits += progressTask.CostTypeId == 34 && progressTask.ParentPhaseId == progressTask.PhaseId ? progressValue : 0;
//        //        var actualUnits = jobProgress.ActualUnits ?? 0;

//        //        percentage = footage == 0 ? 0 : actualUnits / footage;
//        //        percentage = percentage > 1 ? 1 : percentage;

//        //        var remainingUnits = footage - actualUnits;
//        //        remainingUnits = remainingUnits < 0 ? 0 : remainingUnits;

//        //        jobProgress.RemainingUnits = remainingUnits;
//        //        jobProgress.PercentCompletion = percentage;

//        //    }
//        //    else
//        //    {
//        //        //if (projectDuration != 0)
//        //        //{
//        //        //    percentage = actualDuration == 0 ? 1 : actualDuration / projectDuration;
//        //        //    percentage = percentage > 1 ? 1 : percentage;
//        //        //    jobProgress.RemainingDuration = projectDuration * (1 - percentage);
//        //        //}
//        //    }

//        //    if (hourTask.ParentBudgetCodeId != null)
//        //    {
//        //        jobProgress.ActualBudgetCodeId = hourTask.ParentBudgetCodeId;
//        //        //if (hourTask.ParentBudgetCode != null)
//        //        //{
//        //        //    jobProgress.ActualBudgetCode = hourTask.ParentBudgetCode;
//        //        //}
//        //    }

//        //    if (jobProgress.ActualStartDate > hourTask.WorkDate || jobProgress.ActualStartDate == null)
//        //    {
//        //        jobProgress.ActualStartDate = hourTask.WorkDate;
//        //    }
            
//        //    //var priorTask = job.JobPhaseTracks.LastOrDefault(f => f.SortId < jobProgress.SortId);
//        //    //if (priorTask != null)
//        //    //{
//        //    //    //priorTask.PercentCompletion = 1;
//        //    //};
//        //    if (dbSaveVerbose) db.SaveChanges();
//        //}

//        //public static JobPhaseTrack CreateUpdate(DailyJobTask hourTask, DailyJobTask progressTask, Job job, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (job == null || hourTask == null || db == null)
//        //    {
//        //        return null;
//        //    }

//        //    var result = job.JobPhaseTracks.FirstOrDefault(f => f.PhaseId == hourTask.ParentPhaseId && f.PassId == hourTask.PassId);

//        //    if (result == null)
//        //    {
//        //        result = Init(hourTask);

//        //        result.Job = job;
//        //        result.SortId = (job.JobPhaseTracks.Max(max => max.SortId) ?? 0) + 1;
//        //        job.JobPhaseTracks.Add(result);
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }
//        //    //ProjectGanttRepository.CreateUpdate(result, db, dbSaveVerbose);
//        //    //Update(result, hourTask, progressTask, job, db, dbSaveVerbose);
//        //    ProjectGanttRepository.CreateUpdate(result, db, dbSaveVerbose);

//        //    return result;
//        //}
//        #endregion

//        //public static void GenerateProgress(BidBoreLine bore, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (bore?.Job == null)
//        //        return;

//        //    bore.Job.BuildProgressTrackingFromBid();
//        //}

//        //public static void UpdateProgressFromTicket(DailyTicket ticket, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (ticket?.DailyJobTicket?.Job == null)
//        //        return;
//        //    var job = ticket.DailyJobTicket.Job;
//        //    //UpdateProgressFromActuals(job, db, dbSaveVerbose);
//        //    //UpdateJobPhaseTracks(job, db, dbSaveVerbose);
//        //}

//        //public static void RebuildProgress(Job job, VPContext db, bool dbSaveVerbose = false, bool fullFlush = false)
//        //{
//        //    if (job == null)
//        //        return;
//        //    if (db == null)
//        //        return;


//        //    if (fullFlush)
//        //    {
//        //        DeleteGanttTasks(job, db, dbSaveVerbose);
//        //        foreach (var subJob in job.SubJobs)
//        //        {
//        //            DeleteGanttTasks(subJob, db, dbSaveVerbose);
//        //        }
//        //    }

//        //    /** Create/Update Job Gantt Task **/
//        //    //ProjectGanttRepository.CreateUpdate(job, db, dbSaveVerbose);

//        //    /** if job is project **/
//        //    if (job.SubJobs.Any())
//        //    {
//        //        //ProjectGanttRepository.BuildCrews(job, db, dbSaveVerbose);
//        //        if (dbSaveVerbose) db.SaveChanges();

//        //        //ProjectGanttRepository.AssignCrews(job, db, dbSaveVerbose);
//        //        if (dbSaveVerbose) db.SaveChanges();

//        //        foreach (var subJob in job.SubJobs)
//        //        {
//        //            RebuildProgress(subJob, db, dbSaveVerbose);
//        //            if (dbSaveVerbose) db.SaveChanges();
//        //        }

//        //        //ProjectGanttRepository.LinkJobs(job, db, dbSaveVerbose);
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //        return;
//        //    }

//        //    /** Update Job Progress with Budget First **/
//        //    var bore = job.BidBoreLine;
//        //    GenerateProgress(bore, db, dbSaveVerbose);

//        //    /** Update progress Task with Actual Daily Tickets **/
//        //    //UpdateProgressFromActuals(job, db, dbSaveVerbose);

//        //    /** Update progress Task with Actual Daily Tickets **/
//        //    UpdateJobPhaseTracks(job, db, dbSaveVerbose);

//        //}

//        //private static void DeleteGanttTasks(Job job, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    /** Flush All Task from the System **/
//        //    var ganttTasks = db.PMProjectGanttTasks.Where(f => f.GanttId == job.GanttId).ToList();

//        //    //Disassociate Task from objects
//        //    foreach (var task in ganttTasks)
//        //    {
//        //        task.BidBorePackages.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.BidBorePackages = null;

//        //        task.BidBoreLineTasks.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.BidBoreLineTasks = null;

//        //        task.BidBoreLines.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.BidBoreLines = null;

//        //        task.DailyTasks.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.DailyTasks = null;

//        //        task.DailyTicket.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.DailyTicket = null;

//        //        task.JobPhases.ToList().ForEach(e => { 
//        //            e.GanttId = null; 
//        //            e.ProjectGanttTask = null;
//        //            e.TaskId = null;
//        //            e.ActualProjectGanttTask = null;
//        //            e.ActualGanttId = null;
//        //            e.ActualTaskId = null;
//        //            e.RemainingGanttId = null;
//        //            e.RemainingProjectGanttTask = null;
//        //            e.RemainingTaskId = null;
//        //        });
//        //        task.JobPhases = null;

//        //        task.JobBudgetTasks.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.JobBudgetTasks = null;

//        //        task.Jobs.ToList().ForEach(e => { e.GanttId = null; e.ProjectGanttTask = null; });
//        //        task.Jobs = null;

//        //        task.Owners.ToList().ForEach(e => { task.Owners.Remove(e); });
//        //        task.Owners = null;

//        //        task.TaskLinkSources.ToList().ForEach(e => { task.TaskLinkSources.Remove(e); });
//        //        task.TaskLinkSources = null;

//        //        task.TaskLinkTargets.ToList().ForEach(e => { task.TaskLinkTargets.Remove(e); });
//        //        task.TaskLinkTargets = null;

//        //        task.SubTasks.ToList().ForEach(e => { task.SubTasks.Remove(e); });
//        //        task.SubTasks = null;

//        //        task.ParentTask = null;
//        //        //db.BulkSaveChanges();
//        //    }
//        //    db.PMProjectGanttTasks.RemoveRange(ganttTasks);

//        //    //db.BulkSaveChanges();
//        //    if (dbSaveVerbose) db.SaveChanges();
//        //}

//        //public static void UpdateProgressFromActuals(Job job, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (job == null)
//        //        return;
//        //    if (db == null)
//        //        return;
//        //    job.JobPhaseTracks.ToList().ForEach(e =>
//        //    {
//        //        e.ActualDuration = 0;
//        //        e.ActualUnits = 0;
//        //        e.ActualBudgetCodeId = null;
//        //        e.PercentCompletion = 0;
//        //        e.ActualStartDate = null;
//        //        e.RemainingDuration = 0;
//        //        e.RemainingUnits = 0;
//        //        e.RemainingStartDate = null;
//        //    });

//        //    if (dbSaveVerbose) db.SaveChanges();

//        //    /**Loop through all Active Tickets for the job and there daily task**/
//        //    ProjectGanttRepository.ClearJobLinks(job, db, dbSaveVerbose);
//        //    var jobTasks = job.ActiveTickets()
//        //                        .SelectMany(s => s.DailyJobTasks)
//        //                        .OrderBy(o => o.WorkDate)
//        //                        .OrderBy(o => o.SortOrder)
//        //                        .ThenByDescending(o => o.CostTypeId)
//        //                        .ToList();
//        //    var JobTaskLines = jobTasks.GroupBy(g => new { g.TicketId, g.LineNum, g.WorkDate })
//        //                                .Select(s => new { s.Key.TicketId, s.Key.LineNum, s.Key.WorkDate, Tasks = s })
//        //                                .ToList();
//        //    foreach (var line in JobTaskLines)
//        //    {
//        //        var hoursTask = line.Tasks.FirstOrDefault(f => f.CostTypeId == 1);
//        //        if (hoursTask != null)
//        //        {
//        //            var progressTask = line.Tasks.FirstOrDefault(f => f.CostTypeId != 1);
//        //            var jobProgress = CreateUpdate(hoursTask, progressTask, job, db, dbSaveVerbose);

//        //            /** Update Daily Task Gantt Info**/
//        //            if (jobProgress != null && hoursTask != null)
//        //            {
//        //                ProjectGanttRepository.CreateUpdate(hoursTask, jobProgress, db, dbSaveVerbose);
//        //            }
//        //        }
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }

//        //    /** Remove Progress Task that are not planned and have no actual**/
//        //    var delList = job.JobPhaseTracks.Where(f => f.PlannedBudgetCodeId == null && f.ActualBudgetCodeId == null).ToList();
//        //    if (delList.Any())
//        //    {
//        //        lock (delList)
//        //        {
//        //            foreach (var del in delList)
//        //            {
//        //                if (del.ProjectGanttTask != null)
//        //                {
//        //                    //var taskDel = db.PMProjectGanttTasks.FirstOrDefault(f => f.KeyID == del.ProjectGanttTask.KeyID);
//        //                    del.ProjectGanttTask = null;
//        //                    //db.PMProjectGanttTasks.Remove(taskDel);
//        //                }
//        //                job.JobPhaseTracks.Remove(del);
//        //            }
//        //            if (dbSaveVerbose) db.SaveChanges();

//        //            foreach (var del in delList)
//        //            {
//        //                if (del.ProjectGanttTask != null)
//        //                {
//        //                    var taskDel = db.PMProjectGanttTasks.FirstOrDefault(f => f.KeyID == del.ProjectGanttTask.KeyID);
//        //                    db.PMProjectGanttTasks.Remove(taskDel);
//        //                    if (dbSaveVerbose) db.SaveChanges();
//        //                }
//        //                //job.JobPhaseTracks.Remove(del);
//        //            }
//        //            if (dbSaveVerbose) db.SaveChanges();
//        //        }
//        //    }
//        //}

//        //public static void UpdateProgressFromActuals_OLD(Job job, VPContext db)
//        //{
//        //    //if (job == null)
//        //    //    return;
//        //    //if (db == null)
//        //    //    return;
//        //    //var dbSaveVerbose = false;

//        //    ///** if job is project **/
//        //    //if (job.SubJobs.Any())
//        //    //{
//        //    //    ProjectGanttRepository.BuildCrews(job, db);
//        //    //    ProjectGanttRepository.AssignCrews(job, db);
//        //    //    foreach (var subJob in job.SubJobs)
//        //    //    {
//        //    //        UpdateProgressFromActuals(subJob, db);
//        //    //    }
//        //    //    ProjectGanttRepository.LinkJobs(job, db);
//        //    //    return;
//        //    //}


//        //    //if (dbSaveVerbose)
//        //    //    db.SaveChanges();

//        //    //job.JobPhaseTracks.ToList().ForEach(e =>
//        //    //{
//        //    //    e.ActualDuration = 0;
//        //    //    e.ActualUnits = 0;
//        //    //    e.ActualBudgetCodeId = null;
//        //    //    e.PercentCompletion = 0;
//        //    //    e.ActualStartDate = null;
//        //    //    e.RemainingDuration = 0;
//        //    //    e.RemainingUnits = 0;
//        //    //    e.RemainingStartDate = null;
//        //    //});

//        //    //if (dbSaveVerbose)
//        //    //    db.SaveChanges();

//        //    ///** Update Job Progress with Budget First **/
//        //    //var bore = job.BidBoreLine;
//        //    //GenerateProgress(bore, db);

//        //    //if (dbSaveVerbose)
//        //    //    db.SaveChanges();

//        //    //ProjectGanttRepository.CreateUpdate(job, db);

//        //    //if (dbSaveVerbose)
//        //    //    db.SaveChanges();

//        //    ///**Loop through all Active Tickets for the job and there daily task**/
//        //    //var jobTasks = job.ActiveTickets()
//        //    //                    .SelectMany(s => s.DailyJobTasks)
//        //    //                    .OrderBy(o => o.WorkDate)
//        //    //                    .OrderBy(o => o.SortOrder)
//        //    //                    .ThenByDescending(o => o.CostTypeId)
//        //    //                    .ToList();
//        //    //var JobTaskLines = jobTasks.GroupBy(g => new { g.TicketId, g.LineNum, g.WorkDate })
//        //    //                            .Select(s => new { s.Key.TicketId, s.Key.LineNum, s.Key.WorkDate, Tasks = s })
//        //    //                            .ToList();
//        //    //foreach (var line in JobTaskLines)
//        //    //{
//        //    //    var hoursTask = line.Tasks.FirstOrDefault(f => f.CostTypeId == 1);
//        //    //    if (hoursTask != null)
//        //    //    {
//        //    //        var progressTask = line.Tasks.FirstOrDefault(f => f.CostTypeId != 1);
//        //    //        var jobProgress = CreateUpdate(hoursTask, progressTask, job, db);
//        //    //        if (dbSaveVerbose)
//        //    //            db.SaveChanges();
//        //    //        if (jobProgress != null && hoursTask != null)
//        //    //        {
//        //    //            ProjectGanttRepository.CreateUpdate(hoursTask, jobProgress, db);
//        //    //        }
//        //    //    }
//        //    //    if (dbSaveVerbose)
//        //    //        db.SaveChanges();
//        //    //}

//        //    ///** Remove Progress Task that are not planned and have no actual**/            
//        //    //var delList = job.JobPhaseTracks.Where(f => f.PlannedBudgetCodeId == null && f.ActualBudgetCodeId == null).ToList();
//        //    //lock (delList)
//        //    //{
//        //    //    foreach (var del in delList)
//        //    //    {
//        //    //        if (del.ProjectGanttTask != null)
//        //    //        {
//        //    //            var taskDel = db.PMProjectGanttTasks.FirstOrDefault(f => f.KeyID == del.ProjectGanttTask.KeyID);
//        //    //            del.ProjectGanttTask = null;
//        //    //            //db.PMProjectGanttTasks.Remove(taskDel);
//        //    //        }
//        //    //        job.JobPhaseTracks.Remove(del);
//        //    //    }
//        //    //    if (dbSaveVerbose)
//        //    //        db.SaveChanges();

//        //    //    foreach (var del in delList)
//        //    //    {
//        //    //        if (del.ProjectGanttTask != null)
//        //    //        {
//        //    //            var taskDel = db.PMProjectGanttTasks.FirstOrDefault(f => f.KeyID == del.ProjectGanttTask.KeyID);
//        //    //            db.PMProjectGanttTasks.Remove(taskDel);
//        //    //        }
//        //    //        //job.JobPhaseTracks.Remove(del);
//        //    //    }
//        //    //    if (dbSaveVerbose)
//        //    //        db.SaveChanges();
//        //    //}

//        //    //UpdateJobPhaseTracks(job, db);
//        //}

//        /*
//         * 
//         * Set Start dates for each Phase and Pass
//         * Update Remaining Days/Units
//         * Update Progress Percentage
//         * Update Related Gantt Task
//         * 
//         */
       
//    }
//}
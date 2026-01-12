//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace portal.Repository.VP.PM
//{
//    public static class ProjectGanttRepository
//    {
//        //public static void GenerateTasks(DB.Infrastructure.ViewPointDB.Data.Job job, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (job == null)
//        //        return;
            
//        //    if (job.ProjectGanttTask == null)
//        //        CreateUpdate(job, db, dbSaveVerbose);

//        //    BuildTasks(job, db, dbSaveVerbose);
//        //}

//        //public static void GenerateTasks(DB.Infrastructure.ViewPointDB.Data.BidBoreLine boreLine, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (boreLine == null)
//        //        return;

//        //    if (boreLine.GanttId == null)
//        //    {
//        //        var jobTask = Init(boreLine, db, dbSaveVerbose);
//        //        //job.GanttId = jobTask.GanttId;
//        //        boreLine.ProjectGanttTask = jobTask;
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }
//        //    BuildTasks(boreLine, db, dbSaveVerbose);
//        //}

//        #region Project
        
//        public static void LinkJobs(Job project, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (project == null)
//                return;
//            if (db == null)
//                return;
//            if (!project.SubJobs.Any())
//            {
//                LinkJobs(project.ParentJob, db, dbSaveVerbose);
//                if (dbSaveVerbose) db.SaveChanges();
//                return;
//            }

           
//            var crews = project.SubJobs.GroupBy(g => g.CrewId).Select(s => new { Jobs = s.ToList() }).ToList();

//            var startDate = project.CalculatedStartDate();
//            foreach (var crew in crews)
//            {
//                Job priorJob = null;
//                var duration = 0m;
//                foreach (var job in crew.Jobs)
//                {
//                    var task = job.ProjectGanttTask;
//                    if (task != null)
//                    {
//                        if (job.DailyFieldTickets.Any() || priorJob == null)
//                        {
//                            ClearLinkTask(job.ProjectGanttTask, db, dbSaveVerbose);
//                            if (dbSaveVerbose) db.SaveChanges();
//                        }
//                        else if (priorJob != null)
//                        {
//                            var priorTask = priorJob.ProjectGanttTask;
//                            var taskLink = new PMProjectTaskLink()
//                            {
//                                GTCo = task.GTCo,
//                                GanttId = task.GanttId,
//                                LinkId = NextLinkId(task.GTCo, task.GanttId, db),
//                                LinkType = 0,

//                                SourceGanttId = priorTask.GanttId,
//                                SourceId = priorTask.TaskId,
//                                Source = priorTask,

//                                TargetId = task.TaskId,
//                                TargetGanttId = task.GanttId,
//                                Target = task,
//                            };
//                            task.TaskLinkTargets.Add(taskLink);
//                            if (dbSaveVerbose) db.SaveChanges();
//                        }
//                        task.StartDate = startDate.AddDays((double)duration);
//                        job.StartDate = task.StartDate;
//                    }
//                    if (dbSaveVerbose) db.SaveChanges();

//                    duration += job.CalculatedDays;
//                    priorJob = job;
//                }
//            }
//        }


//        public static void ClearJobLinks(Job job, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (job == null)
//                return;
//            if (db == null)
//                return;
//            if (!job.SubJobs.Any())
//            {
//                LinkJobs(job.ParentJob, db, dbSaveVerbose);
//                if (dbSaveVerbose) db.SaveChanges();
//                return;
//            }

//            if (job.DailyFieldTickets.Any())
//            {
//                ClearLinkTask(job.ProjectGanttTask, db, dbSaveVerbose);
//                if (dbSaveVerbose) db.SaveChanges();
//            }
//        }
        
//        public static void BuildCrews(Job project, VPContext db, bool dbSaveVerbose = false)
//        {
//            return;
//            foreach (var job in project.SubJobs.Where(f => f.JobId != f.ParentJobId).ToList())
//            {
//                if (!project.Crews.Any(f => f.CrewId == job.CrewId) && job.CrewId != null)
//                {
//                    var crew = new PMProjectCrew()
//                    {
//                        PMCo = job.JCCo,
//                        ProjectId = project.JobId,
//                        SeqId = project.Crews.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
//                        CrewId = job.CrewId,
//                        Crew = job.Crew,
//                        Project = project,
//                        StartDate = project.StartDate,
//                        AutoSchedule = true,
//                    };

//                    project.Crews.Add(crew);
//                    if (dbSaveVerbose) db.SaveChanges();
//                }
//                var taskCrew = job.DailyFieldTickets.Where(f => f.Crew != null).GroupBy(f => f.Crew).Select(s => new { Crew = s.Key }).ToList();

//                foreach (var crew in taskCrew)
//                {
//                    if (!project.Crews.Any(f => f.CrewId == crew.Crew.CrewId) && crew.Crew != null)
//                    {
//                        var projectCrew = new PMProjectCrew()
//                        {
//                            PMCo = crew.Crew.PRCo,
//                            ProjectId = project.JobId,
//                            SeqId = project.Crews.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
//                            CrewId = crew.Crew.CrewId,
//                            Crew = crew.Crew,
//                            Project = project,
//                            StartDate = project.StartDate,
//                            AutoSchedule = true,
//                        };

//                        project.Crews.Add(projectCrew);
//                        if (dbSaveVerbose) db.SaveChanges();
//                    }
//                    job.Crew = crew.Crew;
//                    job.CrewId = crew.Crew.CrewId;
//                }
//                if (dbSaveVerbose) db.SaveChanges();
//            }

            
//        }

//        public static void AssignCrews(Job project, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (project == null)
//                return;
//            if (!project.SubJobs.Any())
//                return;
//            if (db == null)
//                return;

//            /** Auto Assign Crews to open jobs**/
//            var crewList = project.Crews.Where(f => f.AutoSchedule == true).ToArray();

//            if (crewList.Length > 0)
//            {
//                var crewId = 0;
//                foreach (var subjob in project.SubJobs.Where(f => f.JobId != f.ParentJobId).ToList())
//                {
//                    if (subjob.CrewId == null || !subjob.DailyJobTasks.Any())
//                    {
//                        subjob.CrewId = crewList[crewId].CrewId;
//                        crewId++;
//                        if (crewId > crewList.Length - 1)
//                        {
//                            crewId = 0;
//                        }
//                        if (dbSaveVerbose) db.SaveChanges();
//                    }
//                    UpdateOwners(subjob.ProjectGanttTask, subjob, db, dbSaveVerbose);
//                }
//            }
//            UpdateOwners(project.ProjectGanttTask, project, db, dbSaveVerbose);
//        }

//        public static void LinkPriorTask_DEL(JobPhaseTrack task, VPContext db)
//        {
//            var jobProgessTasks = task.Job.JobPhaseTracks.Where(w => w.SortId != 0).OrderBy(o => o.SortId).ToList();
//            var priorTask = jobProgessTasks.LastOrDefault(f => f.SortId < task.SortId && f.TaskId != null);
//            if (priorTask != null)
//            {
//                if (!task.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == priorTask.ProjectGanttTask.TaskId))
//                {
//                    var taskLink = new PMProjectTaskLink()
//                    {
//                        GTCo = task.ProjectGanttTask.GTCo,
//                        GanttId = task.ProjectGanttTask.GanttId,
//                        LinkId = NextLinkId(task.ProjectGanttTask.GTCo, task.ProjectGanttTask.GanttId, db),
//                        LinkType = 0,

//                        SourceGanttId = priorTask.ProjectGanttTask.GanttId,
//                        SourceId = priorTask.ProjectGanttTask.TaskId,
//                        Source = priorTask.ProjectGanttTask,

//                        TargetId = task.ProjectGanttTask.TaskId,
//                        TargetGanttId = task.ProjectGanttTask.GanttId,
//                        Target = task.ProjectGanttTask,
//                    };
//                    task.ProjectGanttTask.TaskLinkTargets.Add(taskLink);
//                }
//            }
//        }
//        #endregion

//        #region Job
//        public static PMProjectGanttTask Init(Job job, VPContext db)
//        {
//            if (job == null)
//                return null;
//            if (db == null)
//                return null;
//            var result = new PMProjectGanttTask();
//            result.GTCo = job.JCCo;
//            result.GanttId = NextGanttId(job.JCCo, db);
//            result.TaskId = NextTaskId(result.GTCo, result.GanttId ,db);
//            //Update(result, job);
//            return result;
//        }

//        public static void Update(PMProjectGanttTask ganttTask, Job job, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (ganttTask == null || job == null || db == null)
//                return;

//            ganttTask.StartDate = job.CalculatedStartDate();
//            ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Job;
//            if (job.SubJobs.Any())
//            {
//                ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Project;
//            }
//            ganttTask.TaskType = "project";
//            ganttTask.Text = job.Description;
//            if (dbSaveVerbose) db.SaveChanges();
//            //ganttTask.ParentTaskId = jobPhase.ProjectGanttTask.TaskId;
//        }

//        public static void CreateUpdate(Job job, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (job == null)
//                return;
//            if (db == null)
//                return;

//            if (job.ProjectGanttTask == null)
//            {
//                job.ProjectGanttTask = Init(job, db);
//                if (dbSaveVerbose) db.SaveChanges();
//            }
//            Update(job.ProjectGanttTask, job, db, dbSaveVerbose);
//            UpdateOwners(job.ProjectGanttTask, job, db, dbSaveVerbose);
//            ClearLinkTask(job.ProjectGanttTask, db, dbSaveVerbose);
//        }
               
//        public static void UpdateOwners(PMProjectGanttTask ganttTask, Job job, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (job == null || ganttTask == null || db == null)
//                return;

//            List<Crew> crewList = new List<Crew>();
//            var subJobs = job.SubJobs.Where(f => f.JobId != f.ParentJobId).ToList();
//            if (subJobs.Any(f => f.JobId != f.ParentJobId))
//            {
//                var subJobsCrews = subJobs.Where(f => f.Crew != null).ToList();
//                if (subJobsCrews.Count > 0)
//                {
//                    crewList = subJobsCrews.Select(s => s.Crew).Distinct().ToList();
//                }                
//            }
//            else {
//                var subJobsCrews = job.DailyFieldTickets.Where(f => f.Crew != null).ToList();
//                if (subJobsCrews.Count > 0)
//                {
//                    crewList = subJobsCrews.Select(s => s.Crew).Distinct().ToList();
//                }
//            }

//            foreach (var crew in crewList)
//            {
//                if (!ganttTask.Owners.Any(f => f.OwnerId == crew.CrewId))
//                {
//                    var owner = new PMProjectGantTaskOwner
//                    {
//                        GTCo = ganttTask.GTCo,
//                        GanttId = ganttTask.GanttId,
//                        TaskId = ganttTask.TaskId,
//                        SeqId = ganttTask.Owners.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
//                        OwnerId = crew.CrewId,
//                        OwnerDesc = crew?.Label(),
//                        OwnerTypeId = 1,
//                    };
//                    ganttTask.Owners.Add(owner);
//                    if (dbSaveVerbose) db.SaveChanges();
//                }
//            }

//            foreach (var owner in ganttTask.Owners.ToList())
//            {
//                if (!crewList.Any(f => f.CrewId == owner.OwnerId))
//                {
//                    ganttTask.Owners.Remove(owner);
//                    if (dbSaveVerbose) db.SaveChanges();
//                }
//            }
//        }
//        #endregion

//        #region JobPhaseTrack
//        public static PMProjectGanttTask Init(JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (task == null)
//                return null;
//            if (db == null)
//                return null;

//            if (task.Job.ProjectGanttTask == null)
//                CreateUpdate(task.Job, db, dbSaveVerbose);

//            var result = new PMProjectGanttTask();
//            result.GTCo = task.JCCo;
//            result.GanttId = task.Job.ProjectGanttTask.GanttId;
//            result.TaskId = NextTaskId(task.JCCo, task.Job.ProjectGanttTask.GanttId, db);
//            return result;
//        }

//        public static void Update(PMProjectGanttTask ganttTask, JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (ganttTask == null)
//                return;
//            if (task == null)
//                return;
//            //var isComplete = (task.IsComplete ?? task.ManualIsComplete) ?? false;
//            var budgetCode = task.ActualBudgetCode ?? task.PlannedBudgetCode;
//            var budgetCodeId = task.ActualBudgetCodeId ?? task.PlannedBudgetCodeId;
//            var progress = task.ManualPercentComplete > task.PercentCompletion ? task.ManualPercentComplete: task.PercentCompletion;

//            ganttTask.ParentTaskId = task.Job.ProjectGanttTask.TaskId;
//            ganttTask.StartDate = task.ActualStartDate ?? task.ProjectedStartDate;
//            ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Phase;
//            ganttTask.TaskType = "project";
//            ganttTask.Duration = task.ActualDuration == 0 ? task.ProjectedDuration : task.ActualDuration;
//            ganttTask.Duration = ganttTask.Duration * 24 * 60;
//            ganttTask.RenderType = "";//split
//            ganttTask.Progress = progress;
//            if (progress == 1 && task.ActualDuration == 0)
//            {
//                ganttTask.Duration = 0;
//            }

//            if (budgetCode == null)
//            {
//                //using var db = new VPContext();
//                budgetCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == task.JCCo && f.BudgetCodeId == budgetCodeId);
//            }

//            ganttTask.Text = budgetCode?.Description;
//            if (task.PassId > 1 && (budgetCode?.Radius ?? 0) == 0)
//            {
//                ganttTask.Text += string.Format(" {0}", task.PassId);
//            }
//            if (dbSaveVerbose) db.SaveChanges();
//        }

//        public static void CreateUpdateRemaining(JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (task == null)
//                return;
//            if (db == null)
//                return;

//            var isComplete = (task.IsComplete == true || task.ManualIsComplete == true);
//            if (task.RemainingDuration > 0 && task.ActualBudgetCodeId != null && !isComplete)
//            {
//                if (task.RemainingProjectGanttTask == null)
//                    task.RemainingProjectGanttTask = Init(task, db, dbSaveVerbose);

//                UpdateRemainingTask(task.RemainingProjectGanttTask, task, db, dbSaveVerbose);
//                ClearLinkTask(task.RemainingProjectGanttTask, db, dbSaveVerbose);
//                if (task.ActualProjectGanttTask != null)
//                {
//                    if (!task.RemainingProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == task.ActualProjectGanttTask.TaskId))
//                    {
//                        task.RemainingProjectGanttTask.TaskLinkTargets.Add(new PMProjectTaskLink
//                        {
//                            //SourceId = task.ActualProjectGanttTask.TaskId,
//                            //LinkId = NextLinkId(task.ProjectGanttTask.GTCo, task.RemainingProjectGanttTask.GanttId, db),
//                            //LinkType = 0,

//                            GTCo = task.ActualProjectGanttTask.GTCo,
//                            GanttId = task.ActualProjectGanttTask.GanttId,
//                            LinkId = NextLinkId(task.ActualProjectGanttTask.GTCo, task.ActualProjectGanttTask.GanttId, db),
//                            LinkType = 0,

//                            SourceGanttId = task.ActualProjectGanttTask.GanttId,
//                            SourceId = task.ActualProjectGanttTask.TaskId,
//                            Source = task.ActualProjectGanttTask,

//                            TargetId = task.RemainingProjectGanttTask.TaskId,
//                            TargetGanttId = task.RemainingProjectGanttTask.GanttId,
//                            Target = task.RemainingProjectGanttTask,
//                        });
//                        if (dbSaveVerbose) db.SaveChanges();
//                    }
//                }
//                UpdateOwners(task.RemainingProjectGanttTask, task, db, dbSaveVerbose);
//            }
//            else
//            {
//                if (task.RemainingProjectGanttTask != null)
//                {
//                    db.PMProjectGanttTasks.Remove(task.RemainingProjectGanttTask);
//                    task.RemainingProjectGanttTask = null;
//                }
//            }
//        }
       
//        public static void UpdateRemainingTask(PMProjectGanttTask ganttTask, JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (ganttTask == null || task == null)
//                return;

//            var budgetCode = task.ActualBudgetCode ?? task.PlannedBudgetCode;
//            var budgetCodeId = task.ActualBudgetCodeId ?? task.ActualBudgetCodeId;

//            ganttTask.ParentTaskId = task.TaskId;
//            ganttTask.StartDate = task.RemainingStartDate;
//            ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Phase;
//            ganttTask.TaskType = "task";
//            ganttTask.Duration = task.RemainingDuration;
//            ganttTask.Duration = ganttTask.Duration * 24 * 60;
//            ganttTask.RenderType = "";

//            if (budgetCode == null)
//            {
//                budgetCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == task.JCCo && f.BudgetCodeId == budgetCodeId);
//            }
//            ganttTask.Text = string.Format("Projected {0} ", budgetCode?.Description);
//            if (task.PassId > 1 && (budgetCode?.Radius ?? 0) == 0)
//            {                
//                ganttTask.Text += string.Format(" {0}", task.PassId);
//            }
//            if (dbSaveVerbose) db.SaveChanges();

//        }

//        public static void UpdateActualTask(PMProjectGanttTask ganttTask, JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            ganttTask.ParentTaskId = task.TaskId;
//            ganttTask.StartDate = task.ActualStartDate;
//            ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Phase;
//            ganttTask.TaskType = "project";
//            ganttTask.Progress = 1;
//            var budgetCode = task.ActualBudgetCode ?? task.PlannedBudgetCode;
//            if (budgetCode == null)
//            {
//                var budgetCodeId = task.ActualBudgetCodeId ?? task.ActualBudgetCodeId;
//                budgetCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == task.JCCo && f.BudgetCodeId == budgetCodeId);
//            }
//            ganttTask.Text = string.Format("Actual {0} ", budgetCode?.Description);
//            if (task.PassId > 1 && (budgetCode?.Radius ?? 0) == 0)
//            {
//                ganttTask.Text += string.Format(" {0}", task.PassId);
//            }
//            ganttTask.Duration = task.ActualDuration;

//            ganttTask.Duration = ganttTask.Duration * 24 * 60;
//            ganttTask.RenderType = "";
//            if (dbSaveVerbose) db.SaveChanges();

//            //UpdateOwners(ganttTask, task, db, dbSaveVerbose);
//        }

//        /*
//         * 
//         * 
//         * 
//         * 
//         */
//        public static void CreateUpdate(JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (task == null)
//                return;
//            if (db == null)
//                return;

//            if (task.ProjectGanttTask == null)
//                task.ProjectGanttTask = Init(task, db, dbSaveVerbose);
//            if (dbSaveVerbose) db.SaveChanges();

//            Update(task.ProjectGanttTask, task, db, dbSaveVerbose);
//            UpdateOwners(task.ProjectGanttTask, task, db, dbSaveVerbose);
//            ClearLinkTask(task.ProjectGanttTask, db, dbSaveVerbose);
//            task.IsComplete ??= false;
//            task.ManualIsComplete ??= false;
//            if ((task.ActualDuration ?? 0) == 0 && task.IsComplete == false && task.ManualIsComplete == false)
//                LinkPriorTask(task, db, dbSaveVerbose);

//            CreateUpdateRemaining(task, db, dbSaveVerbose);
//        }

//        public static void LinkPriorTask(JobPhaseTrack task, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (task == null)
//                return;
//            if (db == null)
//                return;

//            var jobProgessTasks = task.Job.JobPhaseTracks.Where(w => w.SortId != 0).OrderBy(o => o.SortId).ToList();
//            var priorTask = jobProgessTasks.LastOrDefault(f => f.SortId < task.SortId && f.TaskId != null);
//            if (priorTask != null)
//            {
//                if (!task.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == priorTask.ProjectGanttTask.TaskId))
//                {
//                    var taskLink = new PMProjectTaskLink()
//                    {
//                        GTCo = task.ProjectGanttTask.GTCo,
//                        GanttId = task.ProjectGanttTask.GanttId,
//                        LinkId = NextLinkId(task.ProjectGanttTask.GTCo, task.ProjectGanttTask.GanttId, db),
//                        LinkType = 0,

//                        SourceGanttId = priorTask.ProjectGanttTask.GanttId,
//                        SourceId = priorTask.ProjectGanttTask.TaskId,
//                        Source = priorTask.ProjectGanttTask,

//                        TargetId = task.ProjectGanttTask.TaskId,
//                        TargetGanttId = task.ProjectGanttTask.GanttId,
//                        Target = task.ProjectGanttTask,
//                    };
//                    task.ProjectGanttTask.TaskLinkTargets.Add(taskLink);
//                    if (dbSaveVerbose) db.SaveChanges();
//                }
//            }
//        }

//        public static void UpdateOwners(PMProjectGanttTask ganttTask, JobPhaseTrack phase, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (phase == null || ganttTask == null || db == null)
//                return;

//            if (phase.ActualStartDate != null)
//            {
//                var crewList = phase.DailyJobPhases.Select( s=> s.JobTicket.Crew).Distinct().ToList();

//                foreach (var crew in crewList)
//                {
//                    if (!ganttTask.Owners.Any(f => f.OwnerId == crew.CrewId))
//                    {
//                        var owner = new PMProjectGantTaskOwner
//                        {
//                            GTCo = ganttTask.GTCo,
//                            GanttId = ganttTask.GanttId,
//                            TaskId = ganttTask.TaskId,
//                            SeqId = ganttTask.Owners.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
//                            OwnerId = crew.CrewId,
//                            OwnerDesc = crew?.Label(),
//                            OwnerTypeId = 1,
//                        };
//                        ganttTask.Owners.Add(owner);
//                        if (dbSaveVerbose) db.SaveChanges();
//                    }
//                }
//                foreach (var owner in ganttTask.Owners.ToList())
//                {
//                    if (!crewList.Any(f => f.CrewId == owner.OwnerId))
//                    {
//                        ganttTask.Owners.Remove(owner);
//                        if (dbSaveVerbose) db.SaveChanges();
//                    }
//                }
//            }
//            else
//            {
//                UpdateOwners(ganttTask, phase.Job, db, dbSaveVerbose);
//            }
//        }
//        #endregion

//        #region DailyJobTask

//        //public static PMProjectGanttTask Init(DailyJobTask task, VPContext db, int? taskId )
//        //{
//        //    if (task == null)
//        //        return null;
//        //    if (db == null)
//        //        return null;
//        //    if (taskId == null)
//        //        taskId = NextTaskId(task.DTCo, task.Job.ProjectGanttTask.GanttId, db);

//        //    var result = new PMProjectGanttTask();
//        //    result.GTCo = task.DTCo;
//        //    result.GanttId = task.Job.ProjectGanttTask.GanttId;
//        //    result.TaskId = (int)taskId;
//        //    //Update(result, task);
//        //    return result;
//        //}
        
//        //public static void Update(PMProjectGanttTask ganttTask, DailyJobTask task, JobPhaseTrack jobPhase, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    var dayTotal = task.DailyTicket.DailyJobTasks.Where(f => f.CostTypeId == 1).Sum(sum => sum.ValueAdj ?? sum.Value);
//        //    var duration = task.Value;
//        //    if (dayTotal != 0)
//        //    {
//        //        duration = task.Value / dayTotal;
//        //    }

//        //    ganttTask.StartDate = task.WorkDate;
//        //    ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Daily;
//        //    ganttTask.TaskType = "task";
//        //    ganttTask.Text = string.Format("DT:{0}", task.JobPhase.Description);
//        //    ganttTask.Progress = 1;
//        //    ganttTask.Duration = duration * 24 * 60;
//        //    ganttTask.ParentTaskId = jobPhase.ActualProjectGanttTask?.TaskId;
//        //    if (dbSaveVerbose) db.SaveChanges();
//        //}
        
//        //public static void CreateUpdate(DailyJobTask task, JobPhaseTrack jobPhaseTrack, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (task == null)
//        //        return;
//        //    if (db == null)
//        //        return;

//        //    if (task.Job.GanttId != task.GanttId && task.ProjectGanttTask != null)
//        //    {
//        //        db.PMProjectGanttTasks.Remove(task.ProjectGanttTask);
//        //        task.ProjectGanttTask = null;
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }

//        //    if (jobPhaseTrack.ActualProjectGanttTask == null)
//        //    {
//        //        jobPhaseTrack.ActualProjectGanttTask = Init(jobPhaseTrack, db, dbSaveVerbose);
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }
//        //    UpdateActualTask(jobPhaseTrack.ActualProjectGanttTask, jobPhaseTrack, db, dbSaveVerbose);

//        //    if (task.ProjectGanttTask == null)
//        //    {
//        //        task.ProjectGanttTask = Init(task, db, null);
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }
//        //    Update(task.ProjectGanttTask, task, jobPhaseTrack, db, dbSaveVerbose);

//        //    UpdateOwners(task.ProjectGanttTask, task, db, dbSaveVerbose);
//        //    ClearLinkTask(task.ProjectGanttTask, db, dbSaveVerbose);
//        //    LinkPriorTask(task, db, dbSaveVerbose);
//        //}

//        //public static void LinkPriorTask(DailyJobTask task, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    var priorTask = task.Job.DailyJobTasks.OrderBy(o => o.SortOrder).LastOrDefault(f => f.TicketId == task.TicketId && f.SortOrder < task.SortOrder && f.TaskId != null);
//        //    if (priorTask != null)
//        //    {
//        //        if (!task.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == priorTask.ProjectGanttTask.TaskId))
//        //        {
//        //            task.ProjectGanttTask.TaskLinkTargets.Add(new PMProjectTaskLink
//        //            {
//        //                GTCo = task.ProjectGanttTask.GTCo,
//        //                GanttId= task.ProjectGanttTask.GanttId,
//        //                LinkId = NextLinkId(task.ProjectGanttTask.GTCo, task.ProjectGanttTask.GanttId, db),
//        //                LinkType = 0,

//        //                SourceGanttId = priorTask.ProjectGanttTask.GanttId,
//        //                SourceId = priorTask.ProjectGanttTask.TaskId,
//        //                Source = priorTask.ProjectGanttTask,

//        //                TargetId = task.ProjectGanttTask.TaskId,
//        //                TargetGanttId = task.ProjectGanttTask.GanttId,
//        //                Target = task.ProjectGanttTask,
//        //            });
//        //            if (dbSaveVerbose) db.SaveChanges();
//        //        }
//        //    }
//        //}

//        //public static void UpdateOwners(PMProjectGanttTask ganttTask, DailyJobTask task, VPContext db, bool dbSaveVerbose = false)
//        //{
//        //    if (task?.DailyTicket?.DailyJobTicket?.CrewId == null || ganttTask == null)
//        //        return;

//        //    var jobTicket = task.DailyTicket.DailyJobTicket;
//        //    if (!ganttTask.Owners.Any(f => f.OwnerId == jobTicket.CrewId))
//        //    {
//        //        var owner = new PMProjectGantTaskOwner
//        //        {
//        //            GTCo = ganttTask.GTCo,
//        //            GanttId = ganttTask.GanttId,
//        //            TaskId = ganttTask.TaskId,
//        //            SeqId = ganttTask.Owners.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
//        //            OwnerId = jobTicket.CrewId,
//        //            OwnerDesc = jobTicket.Crew?.Label(),
//        //            OwnerTypeId = 1,
//        //        };
//        //        ganttTask.Owners.Add(owner);
//        //        if (dbSaveVerbose) db.SaveChanges();
//        //    }
//        //}
//        #endregion

//        #region BidBoreLine
//        public static PMProjectGanttTask Init(BidBoreLine boreLine, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (boreLine == null)
//                return null;
//            if (db == null)
//                return null;
//            var result = new PMProjectGanttTask();
//            result.GTCo = boreLine.BDCo;

//            if (boreLine.GanttId == null)
//            {
//                result.GanttId = db.PMProjectGanttTasks.Where(f => f.GTCo == boreLine.BDCo).DefaultIfEmpty().Max(f => f == null ? 0 : f.GanttId) + 1;
//            }
//            else
//            {
//                result.GanttId = boreLine.GanttId ?? 0;
//            }
//            result.TaskId = db.PMProjectGanttTasks.Where(f => f.GTCo == boreLine.BDCo && f.GanttId == result.GanttId).DefaultIfEmpty().Max(f => f == null ? 0 : f.TaskId) + 1;
//            result.StartDate = boreLine.Bid.StartDate;
//            result.TaskSource = (int)DB.PMGanttSourceTypeEnum.BidPhase;
//            result.TaskType = "project";
//            result.Text = boreLine.Description;

//            return result;
//        }

//        public static PMProjectGanttTask Init(BidBoreLinePass boreLinePass, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (boreLinePass == null)
//                return null;
//            if (db == null)
//                return null;
//            var result = new PMProjectGanttTask
//            {
//                GTCo = boreLinePass.BDCo,
//                GanttId = (int)boreLinePass.BoreLine.ProjectGanttTask.GanttId
//            };
//            result.TaskId = db.PMProjectGanttTasks.Where(f => f.GTCo == boreLinePass.BDCo && f.GanttId == result.GanttId).DefaultIfEmpty().Max(f => f == null ? 0 : f.TaskId) + 1;
//            result.StartDate = boreLinePass.BoreLine.Bid.StartDate;
//            result.TaskSource = (int)DB.PMGanttSourceTypeEnum.BidPhase;
//            result.TaskType = "task";
//            result.Text = boreLinePass.Description;
//            result.ParentTaskId = boreLinePass.BoreLine.ProjectGanttTask.TaskId;

//            return result;
//        }
//        #endregion


//        private static void ClearLinkTask(PMProjectGanttTask task, VPContext db, bool dbSaveVerbose = false)
//        {
//            var links = task.TaskLinkTargets.ToList();
//            foreach (var link in links)
//            {
//                task.TaskLinkTargets.Remove(link);
//                if (dbSaveVerbose) db.SaveChanges();
//                if (db.PMProjectTaskLinks.Any(f => f.KeyID == link.KeyID))
//                {
//                    db.PMProjectTaskLinks.Remove(link);
//                    if (dbSaveVerbose) db.SaveChanges();
//                }
//            }
//        }
        
//        public static void Update(PMProjectGanttTask ganttTask, DailyJobTicket task)
//        {
            
//            ganttTask.ParentTaskId = task.Job.ProjectGanttTask.TaskId;
//            ganttTask.StartDate = task.WorkDate;
//            ganttTask.TaskSource = (int)DB.PMGanttSourceTypeEnum.Ticket;
//            ganttTask.TaskType = "project";
//            ganttTask.Text = string.Format("Daily {0}", task.TicketId.ToString());
//            ganttTask.Duration = 0;
//            ganttTask.Progress = 1;
//            ganttTask.RenderType = "split";
//        }

//        public static void BuildTasks(Job job, VPContext db, bool dbSaveVerbose = false)
//        {
//            if (job == null)
//                return;
//            if (db == null)
//                return;

//            if (job.SubJobs.Any())
//            {
//                BuildCrews(job, db, dbSaveVerbose);
//                AssignCrews(job, db, dbSaveVerbose);
//                foreach (var subJob in job.SubJobs)
//                {
//                    BuildTasks(subJob, db, dbSaveVerbose);
//                }
//                LinkJobs(job, db, dbSaveVerbose);
//                return;
//            }

//            //DailyJobTask priorTask = null;
//            JobPhaseTrack priorPass = null;

//            //Build Job Phass parent Task
//            foreach (var phaseProgress in job.JobPhaseTracks.OrderBy(o => o.SortId))
//            {
//                CreateUpdate(phaseProgress, db, dbSaveVerbose);

//                if (priorPass != null)
//                {
//                    if (!phaseProgress.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == priorPass.ProjectGanttTask.TaskId))
//                    {
//                        phaseProgress.ProjectGanttTask.TaskLinkTargets.Add(new PMProjectTaskLink
//                        {
//                            SourceId = priorPass.ProjectGanttTask.TaskId,
//                            LinkId = NextLinkId(phaseProgress.ProjectGanttTask.GTCo, phaseProgress.ProjectGanttTask.GanttId, db),
//                            LinkType = 0
//                        });
//                    }
//                }
//                //Build Job Actual progress and tie to parent.
//                DateTime? dailyWorkDate = null;
//                var dailyTasks = job.DailyJobTasks.Where(f => f.JobPhaseCost.CostTypeId == 1 && f.Value != 0 && f.ParentBudgetCodeId == (phaseProgress.ActualBudgetCodeId ?? phaseProgress.PlannedBudgetCodeId)).ToList();
//                foreach (var dailyTask in dailyTasks)
//                {
//                    if (dailyWorkDate == null || dailyWorkDate != dailyTask.WorkDate)
//                        priorTask = null;

//                    CreateUpdate(dailyTask, phaseProgress, db, dbSaveVerbose);
//                    dailyTask.ProjectGanttTask.ParentTaskId = phaseProgress.ProjectGanttTask.TaskId;

//                    if (priorTask != null)
//                    {
//                        if (!dailyTask.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId == priorTask.ProjectGanttTask.TaskId))
//                        {
//                            dailyTask.ProjectGanttTask.TaskLinkTargets.Add(new PMProjectTaskLink
//                            {
//                                SourceId = priorTask.ProjectGanttTask.TaskId,
//                                LinkId = NextLinkId(dailyTask.ProjectGanttTask.GTCo, dailyTask.ProjectGanttTask.GanttId, db),
//                                LinkType = 0
//                            });
//                        }
//                    }
//                    priorTask = dailyTask;
//                    dailyWorkDate = dailyTask.WorkDate;
//                }                
//                priorPass = phaseProgress;
//            }
//        }

//        public static void BuildTasks(BidBoreLine bidBoreLine, VPContext db, bool dbSaveVerbose = false)
//        {
//            BidBoreLinePass priorPhase = null;
//            foreach (var phase in bidBoreLine.Passes.Where(f => f.GroundDensityId == 0).OrderBy(o => o.PhaseMaster.SortId).ThenBy(o => o.PassId).ToList())
//            {
//                if (phase.ProjectGanttTask == null)
//                {
//                    phase.ProjectGanttTask = Init(phase, db, dbSaveVerbose);                    
//                }
//                phase.ProjectGanttTask.Duration = phase.ProductionCalTypeId == 0 ? phase.BoreLine.Footage / phase.ProductionRate : phase.ProductionDays;
//                db.SaveChanges();
//                if (priorPhase != null)
//                {
//                    if (!phase.ProjectGanttTask.TaskLinkTargets.Any(f => f.SourceId != priorPhase.ProjectGanttTask.TaskId))
//                    {
//                        phase.ProjectGanttTask.TaskLinkTargets.Add(new PMProjectTaskLink
//                        {
//                            SourceId = priorPhase.ProjectGanttTask.TaskId,
//                            LinkId = NextLinkId(phase.ProjectGanttTask.GTCo, phase.ProjectGanttTask.GanttId, db),
//                            LinkType = 0
//                        });
//                    }
//                }
//                priorPhase = phase;
//            }
//        }

//        public static int NextTaskId(byte co, int ganttId, VPContext db)
//        {
//            var nextId = PMProjectGanttTask.GetNextGanttTaskId(ganttId);
//            return nextId;
//        }

//        public static int NextGanttId(byte co, VPContext db)
//        {
//            var nextId = PMProjectGanttTask.GetNextGanttId();
//            return nextId;
//        }

//        public static int NextLinkId(byte co, int ganttId, VPContext db)
//        {
//            var nextId = db.GetNextId("PMProjectTaskLinks", "LinkId", ganttId.ToString());
//            if (nextId == -1)
//            {
//                var dbMax = db.PMProjectTaskLinks.Where(f => f.GTCo == co && f.GanttId == ganttId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId);
//                var localMax = db.PMProjectTaskLinks.Local.Where(f => f.GTCo == co && f.GanttId == ganttId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId);
//                nextId = dbMax > localMax ? dbMax : localMax;
//                nextId++;
//                db.SetNextId("PMProjectTaskLinks", "LinkId", nextId, ganttId.ToString());
//            }
//            return nextId;
//        }

//    }


//}
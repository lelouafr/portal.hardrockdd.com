using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DHTMLX.Gantt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portal.Models.Views.PM.Project.Schedule
{
    public class GanttListViewModel
    {
        public GanttListViewModel()
        {
            Links = new List<GanttLink>();
            Resources = new List<GanttResource>();
            Tasks = new List<JobGanttEvent>();
        }

        public GanttListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));
            Links = new List<GanttLink>();
            Tasks = new List<JobGanttEvent>();
            var projectResults = GetProject(job);
            Tasks.AddRange(projectResults.Tasks);
            Links.AddRange(projectResults.Links);
            SetResources();
        }

        public GanttListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            Links = new List<GanttLink>();
            Tasks = new List<JobGanttEvent>();
            var projectTasks = new List<Task>();
            foreach (var job in company.JCCompanyParm.Jobs)
            {
                Task threadTask = new Task(() =>
                {
                    using var threadDB = new VPContext();
                    var threadJob = threadDB.Jobs.FirstOrDefault(f => f.JCCo == job.JCCo && f.JobId == job.JobId);
                    
                    var projectResults = GetProject(threadJob);
                    lock (Tasks)
                        Tasks.AddRange(projectResults.Tasks);
                    lock (Links)
                        Links.AddRange(projectResults.Links);
                });
                projectTasks.Add(threadTask);
                if (projectTasks.Count >= 10)
                {
                    projectTasks.ForEach(f => f.Start());
                    Task.WaitAll(projectTasks.ToArray());
                    projectTasks = new List<Task>();
                }
            }
            projectTasks.ForEach(f => f.Start());
            Task.WaitAll(projectTasks.ToArray());
            foreach (var task in Tasks)
            {
                if (task.parent != null)
                {
                    task.open = false;
                }
            }
            SetResources();
        }

        public GanttListViewModel(List<DB.Infrastructure.ViewPointDB.Data.Job> jobs)
        {
            if (jobs == null) throw new System.ArgumentNullException(nameof(jobs));
            Links = new List<GanttLink>();
            Tasks = new List<JobGanttEvent>();
            var projectTasks = new List<Task>();
            foreach (var job in jobs)
            {
                Task threadTask = new Task(() =>
                {
                    using var threadDB = new VPContext();
                    var threadJob = threadDB.Jobs.FirstOrDefault(f => f.JCCo == job.JCCo && f.JobId == job.JobId);

                    var projectResults = GetProject(threadJob);
                    lock (Tasks)
                        Tasks.AddRange(projectResults.Tasks);
                    lock (Links)
                        Links.AddRange(projectResults.Links);
                });
                projectTasks.Add(threadTask);
                if (projectTasks.Count >= 10)
                {
                    projectTasks.ForEach(f => f.Start());
                    Task.WaitAll(projectTasks.ToArray());
                    projectTasks = new List<Task>();
                }
            }
            projectTasks.ForEach(f => f.Start());
            Task.WaitAll(projectTasks.ToArray());            
            foreach (var task in Tasks)
            {
                if (task.parent != null)
                {
                    task.open = false;
                }
            }
            SetResources();
        }

        private void SetResources()
        {
            Resources = new List<GanttResource>();
            var taskOwners = Tasks.Where(f => f.owner != null).SelectMany(s => s.owner).ToList();
            Resources.Add(new GanttResource
            {
                id = "1",
                text = "Unassigned"
            });

            var resources = taskOwners.GroupBy(g => new { g.resource_id, g.text }).Select(s => new GanttResource
            {
                id = s.Key.resource_id,
                text = s.Key.text
            }).ToList();
            Resources.AddRange(resources);
        }

        private GanttListViewModel GetProject(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            var tasks = new List<JobGanttEvent>();
            var links = new List<GanttLink>();
            var result = new GanttListViewModel();
            if (job.ProjectGanttTask != null)
            {
                var task = new JobGanttEvent(job);
                task.open = false;
                tasks.Add(task);

                var subTasksResults = GetSubTasks(job.ProjectGanttTask, job, task);
                tasks.AddRange(subTasksResults.Tasks);
                links.AddRange(subTasksResults.Links);

                var subJobTasks = GetSubJobs(job, task);
                tasks.AddRange(subJobTasks.Tasks);
                links.AddRange(subJobTasks.Links);
            }

            result.Tasks = tasks;
            result.Links = links;
            return result;
        }

        private GanttListViewModel GetSubJobs(DB.Infrastructure.ViewPointDB.Data.Job job, JobGanttEvent parentTask)
        {
            var subList = new List<JobGanttEvent>();
            var links = new List<GanttLink>();
            var subJobs = job.SubJobs.ToList();
            var subTasks = new List<Task>();
            var result = new GanttListViewModel();

            foreach (var subJob in subJobs)
            {
                Task threadTask = new Task(() =>
                {
                    using var threadDB = new VPContext();
                    var threadJob = threadDB.Jobs.FirstOrDefault(f => f.JCCo == subJob.JCCo && f.JobId == subJob.JobId);
                    var task = new JobGanttEvent(threadJob);
                    task.parent = parentTask.id;
                    task.open = false;


                    lock (subList)
                        subList.Add(task);

                    if (threadJob.ProjectGanttTask != null)
                    {
                        if (threadJob.ProjectGanttTask.SubTasks.Any())
                        {
                            var subTaskResults = GetSubTasks(threadJob.ProjectGanttTask, threadJob, task);
                            lock (subList)
                                subList.AddRange(subTaskResults.Tasks);

                            lock (links)
                                links.AddRange(subTaskResults.Links);

                        }
                        var taskLinks = GetLinks(threadJob.ProjectGanttTask);
                        lock (links)
                            links.AddRange(taskLinks);
                    }
                    
                    threadJob = null;
                });
                subTasks.Add(threadTask);
                if (subTasks.Count >= 10)
                {
                    subTasks.ForEach(f => f.Start());
                    Task.WaitAll(subTasks.ToArray());
                    subTasks = new List<Task>();
                }
            }
            subTasks.ForEach(f => f.Start());
            Task.WaitAll(subTasks.ToArray());



            var crewGroup = subJobs.GroupBy(g => g.CrewId).Select(s => new { Jobs = s.ToList() }).ToList();
            foreach (var crew in crewGroup)
            {
                DB.Infrastructure.ViewPointDB.Data.Job priorJob = null;
                foreach (var crewJob in crew.Jobs)
                {
                    if (!crewJob.DailyFieldTickets.Any())
                    {
                        if (priorJob != null)
                        {
                            var linkId = this.Links.DefaultIfEmpty().Max(max => max == null ? 0 : max.id) +1;
                            if (linkId < 10000)
                            {
                                linkId += 10000;
                            }
                            var link = new GanttLink();
                            link.id = linkId;
                            link.source = priorJob.ProjectGanttTask.KeyID;
                            link.target = crewJob.ProjectGanttTask.KeyID;
                            link.type = 0;
                            link.lag = 0;

                            Links.Add(link);
                        }
                    }
                    priorJob = crewJob;
                }
            }
            result.Tasks = subList;
            result.Links = links;

            return result;
        }

        private GanttListViewModel GetSubTasks(DB.Infrastructure.ViewPointDB.Data.PMProjectGanttTask task, DB.Infrastructure.ViewPointDB.Data.Job job, JobGanttEvent parentTask, bool isParentSplit = false)
        {
            var subList = new List<JobGanttEvent>();
            var links = new List<GanttLink>();
            var result = new GanttListViewModel();

            if (task == null)
            {
                return result;
            }
            foreach (var subTask in task.SubTasks)
            {
                if (subTask.KeyID == 43722 || subTask.KeyID == 43723)
                {
                    var test = 0;
                }
                var subTaskItem = new JobGanttEvent(subTask, job);
                subTaskItem.open = false;               
                subTaskItem.parent = parentTask.id;
                subTaskItem.type = "task";
                lock (subList)
                    subList.Add(subTaskItem);
                lock (links)
                    links.AddRange(GetLinks(subTask));
                
                if (subTask.SubTasks.Any())
                {
                    subTaskItem.type = "project";
                    var subResults = GetSubTasks(subTask, job, subTaskItem, false);
                    lock (subList)
                        subList.AddRange(subResults.Tasks);

                    lock (links)
                        links.AddRange(subResults.Links);
                }
            }

            result.Tasks = subList;
            result.Links = links;

            return result;
        }

        private List<GanttLink> GetLinks(PMProjectGanttTask task)
        {
            var result = new List<GanttLink>();
            foreach (var link in task.TaskLinkSources)
            {
                if (!result.Any(f => f.id == link.KeyID))
                {
                    if (link.Target != null && link.Source != null)
                    {
                        lock (result)
                            result.Add(new GanttLink(link));
                    }
                }
            }

            return result;
        }

        public List<JobGanttEvent> Tasks { get; set; }

        public List<GanttLink> Links { get; set; }


        public List<GanttResource> Resources { get; set; }
    }


    public class JobGanttEvent : GanttTask
    {
        public JobGanttEvent()
        {

        }

        public JobGanttEvent(DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            //IsCurrentProject = parentJob.JobId == job.ParentJobId;

            JobStatus = int.TryParse(job.StatusId, out int jobStatusIdOut) ? jobStatusIdOut : 0;
            Co = job.JCCo;
            JobId = job.JobId;

            switch ((DB.JCJobStatusEnum)JobStatus)
            {
                case DB.JCJobStatusEnum.Open:
                case DB.JCJobStatusEnum.Scheduled:
                case DB.JCJobStatusEnum.OnHold:
                    @readonly = false;
                    editable = true;
                    break;
                case DB.JCJobStatusEnum.InProgress:
                case DB.JCJobStatusEnum.PulledPipe:
                case DB.JCJobStatusEnum.Completed:
                case DB.JCJobStatusEnum.Canceled:
                case DB.JCJobStatusEnum.Invoiced:
                    @readonly = true;
                    editable = false;
                    break;
                default:
                    @readonly = false;
                    break;
            }
            @readonly = false;
            editable = true;
            text += string.Format(" ({0})", (DB.JCJobStatusEnum)JobStatus);
            text = JobId;
            JobStatusString = ((DB.JCJobStatusEnum)JobStatus).ToString();
            JobDescription = job.Description;
        }


        public JobGanttEvent(PMProjectGanttTask task, DB.Infrastructure.ViewPointDB.Data.Job job) : base(task)
        {

            Co = job.JCCo;
            JobId = job.JobId;

        }

        public byte Co { get; set; }

        public string JobId { get; set; }
        
        public string JobDescription { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        public bool IsCurrentProject { get; set; }

        public int JobStatus { get; set; }
        
        public string JobStatusString { get; set; }


    }
}
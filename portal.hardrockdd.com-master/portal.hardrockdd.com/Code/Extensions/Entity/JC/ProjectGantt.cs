using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PMProjectGanttTask
    {

        #region Job
        public PMProjectGanttTask Init(Job job)
        {
            if (job == null)
                return null;
            var result = new PMProjectGanttTask
            {
                GTCo = job.JCCo,
                GanttId = GetNextGanttId(),
                TaskId = GetNextGanttTaskId(GanttId)
            };

            return result;
        }

        public void Update(Job job)
        {
            if (job == null)
                return;

            StartDate = job.CalculatedStartDate();
            TaskSource = (int)DB.PMGanttSourceTypeEnum.Job;
            if (job.SubJobs.Any())
            {
                TaskSource = (int)DB.PMGanttSourceTypeEnum.Project;
            }
            TaskType = "project";
            Text = job.Description;
            //ganttTask.ParentTaskId = jobPhase.ProjectGanttTask.TaskId;
        }

        //public void CreateUpdate(Job job)
        //{
        //    if (job == null)
        //        return;

        //    if (job.ProjectGanttTask == null)
        //        job.ProjectGanttTask = job.ini;

        //    job.ProjectGanttTask.Update(job);
        //    job.ProjectGanttTask.UpdateOwners(job);
        //    job.ProjectGanttTask.ClearLinkTask();
        //}

        public void UpdateOwners(Job job)
        {
            if (job == null)
                return;

            List<Crew> crewList = new List<Crew>();
            var subJobs = job.SubJobs.Where(f => f.JobId != f.ParentJobId).ToList();
            if (subJobs.Any(f => f.JobId != f.ParentJobId))
            {
                var subJobsCrews = subJobs.Where(f => f.Crew != null).ToList();
                if (subJobsCrews.Any())
                {
                    crewList = subJobsCrews.Select(s => s.Crew).Distinct().ToList();
                }
            }
            else
            {
                var subJobsCrews = job.DailyFieldTickets.Where(f => f.Crew != null).ToList();
                if (subJobsCrews.Any())
                {
                    crewList = subJobsCrews.Select(s => s.Crew).Distinct().ToList();
                }
            }

            foreach (var crew in crewList)
            {
                if (!Owners.Any(f => f.OwnerId == crew.CrewId))
                {
                    var owner = new PMProjectGantTaskOwner
                    {
                        GTCo = GTCo,
                        GanttId = GanttId,
                        TaskId = TaskId,
                        SeqId = Owners.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                        OwnerId = crew?.CrewId,
                        OwnerDesc = crew?.Label(),
                        OwnerTypeId = 1,
                    };
                    Owners.Add(owner);
                }
            }

            foreach (var owner in Owners.ToList())
            {
                if (!crewList.Any(f => f.CrewId == owner.OwnerId))
                {
                    Owners.Remove(owner);
                }
            }
        }
        #endregion


        

        private void ClearLinkTask()
        {
            var db = VPEntities.GetDbContextFromEntity(this);

            var links = TaskLinkTargets.ToList();
            foreach (var link in links)
            {
                TaskLinkTargets.Remove(link);
                if (db.PMProjectTaskLinks.Any(f => f.KeyID == link.KeyID))
                {
                    db.PMProjectTaskLinks.Remove(link);
                }
            }
        }

        public static int GetNextGanttId()
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("ganttId", typeof(int));

            var result = db.udNextGanttId(outParm);

            return (int)outParm.Value;
        }

        public static int GetNextGanttTaskId(int ganttId)
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("taskId", typeof(int));

            var result = db.udNextGanttTaskId(ganttId, outParm);

            return (int)outParm.Value;
        }

        public static int GetNextGanttLinkkId(int ganttId)
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("linkId", typeof(int));

            var result = db.udNextGanttLinkId(ganttId, outParm);

            return (int)outParm.Value;
        }
    }
}
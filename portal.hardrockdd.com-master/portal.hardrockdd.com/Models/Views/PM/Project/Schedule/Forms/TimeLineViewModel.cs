using portal.Models.Views.DHTMLX;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Models.Views.PM.Project.Schedule
{
    public class TimeLineListViewModel
    {
        public TimeLineListViewModel()
        {
            
        }

        public TimeLineListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            Co = job.JCCo;
            JobId = job.JobId;
            Events = job.SubJobs.Select(s => new JobTimeLineEvent(job, s)).ToList();

            var minDate = Events.Min(min => min.startdate).AddDays(-20);
            var maxDate = Events.Min(min => min.enddate).AddDays(90);
            var relatedJobs = job.Crews.Where(f => f.Crew != null)
                                        .SelectMany(s => s.Crew.Jobs)
                                       .Where(f => f.ParentJobId != job.JobId && 
                                                   f.JobTypeId == "1" &&
                                                   (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2" || f.StatusId == "3" || f.StatusId == "4" || f.StatusId == "5")
                                                   ).ToList();
            var relatedEvents = relatedJobs.Where(f => f.CalculatedStartDate() >= minDate && f.CalculatedEndDate() <= maxDate).Select(s => new JobTimeLineEvent(job, s)).ToList();
            Events.AddRange(relatedEvents);
            Elements = job.Crews.Distinct().Select(s => (TimeLineElement)s).ToList();
            Elements.Add(new TimeLineElement() { 
                key = "null",
                label = "Unassigned"
            });


        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public string JobId { get; set; }

        [Key]
        [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [HiddenInput]
        public int PackageId { get; set; }

        public List<JobTimeLineEvent> Events { get; set; }

        public List<TimeLineElement> Elements { get; set; }
    }


    public class JobTimeLineEvent : TimeLineEvent
    {
        public JobTimeLineEvent()
        {

        }

        public JobTimeLineEvent(DB.Infrastructure.ViewPointDB.Data.Job parentJob, DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            IsCurrentProject = parentJob.JobId == job.ParentJobId;

            JobStatus = int.TryParse(job.StatusId, out int jobStatusIdOut) ? jobStatusIdOut : 0;
            Co = job.JCCo;
            JobId = job.JobId;

            switch ((DB.JCJobStatusEnum)JobStatus)
            {
                case DB.JCJobStatusEnum.Open:
                case DB.JCJobStatusEnum.Scheduled:
                case DB.JCJobStatusEnum.OnHold:
                    @readonly = false;
                    break;
                case DB.JCJobStatusEnum.InProgress:
                case DB.JCJobStatusEnum.PulledPipe:
                case DB.JCJobStatusEnum.Completed:
                case DB.JCJobStatusEnum.Canceled:
                case DB.JCJobStatusEnum.Invoiced:
                    @readonly = true;
                    break;
                default:
                    @readonly = false;
                    break;
            }

            text += string.Format(" ({0})", (DB.JCJobStatusEnum)JobStatus);
        }

        public byte Co { get; set; }

        public string JobId { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        public bool IsCurrentProject { get; set; }

        public int JobStatus { get; set; }


    }
}
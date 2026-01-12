using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.DHTMLX.Gantt
{
    public class GanttTask
    {
        public GanttTask()
        {

        }

        public GanttTask(DB.Infrastructure.ViewPointDB.Data.PMProjectGanttTask task)
        {
            id = task.KeyID;
            text = task.Text;
            startdate = task.StartDate ?? DateTime.Now;
            start_date = startdate.ToString("dd-MM-yyyy");
            type = task.TaskType;
            parent = task.ParentTask?.KeyID;
            progress = task.Progress ?? 0;
            duration = task.Duration ?? 0;
            render = task.RenderType;
            owner = task.Owners.Select(s => new GanttTaskOwner(s)).ToList();
            open = true;
            @readonly = false;
            editable = true;

            if ((DB.PMGanttSourceTypeEnum?)task.TaskSource == DB.PMGanttSourceTypeEnum.Daily)
            {
                @readonly = true;
                editable = false;
            }

            constraint_date = task.ConstraintDate?.ToString("dd-MM-yyyy");
            constraint_type = ((DB.PMGAnttConstraintTypeEnum?)task.ConstriantType)?.ToString();
            sourcetype = ((DB.PMGanttSourceTypeEnum?)task.TaskSource)?.ToString();
        }

        public GanttTask(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job?.ProjectGanttTask == null)
            {
                return;
            }
            var task = job.ProjectGanttTask;

            id = task.KeyID;
            text = task.Text;
            startdate = task.StartDate ?? DateTime.Now;
            start_date = startdate.ToString("dd-MM-yyyy");
            type = task.TaskType;
            parent = task.ParentTask?.KeyID;
            progress = task.Progress ?? 0;
            duration = task.Duration ?? 0;
            render = task.RenderType;
            if (task.Owners.Any())
            {
                owner = task.Owners.Select(s => new GanttTaskOwner(s)).ToList();
            }
            open = true;
            @readonly = false;
            editable = true;

            constraint_date = task.ConstraintDate?.ToString("dd-MM-yyyy");
            constraint_type = ((DB.PMGAnttConstraintTypeEnum?)task.ConstriantType)?.ToString();
            sourcetype = ((DB.PMGanttSourceTypeEnum?)task.TaskSource)?.ToString();
        }

        public string text { get; set; }

        public DateTime startdate { get; set; }

        public string start_date { get; set; }

        public decimal duration { get; set; }

        public long id { get; set; }

        public string type { get; set; }

        public long? parent { get; set; }

        public decimal progress { get; set; }

        public bool open { get; set; }

        public bool @readonly { get; set; }

        public bool editable { get; set; }

        public string render { get; set; }
        
        public string constraint_type { get; set; }

        public string constraint_date { get; set; }

        public string sourcetype { get; set; }

        public List<GanttTaskOwner> owner { get; set; }
    }

    public class GanttLink
    {
        public GanttLink()
        {

        }
        public GanttLink(DB.Infrastructure.ViewPointDB.Data.PMProjectTaskLink link)
        {
            id = link.KeyID;
            source = link.Source.KeyID;
            target = link.Target.KeyID;
            type = link.LinkType;
            lag = link.LagDuration;
            @readonly = false;
            editable = true;
        }
        public long id { get; set; }

        public long source { get; set; }

        public long target { get; set; }

        public int? type { get; set; }

        public decimal? lag { get; set; }

        public bool @readonly { get; set; }

        public bool editable { get; set; }
    }

    public class GanttTaskOwner
    {
        public GanttTaskOwner(DB.Infrastructure.ViewPointDB.Data.PMProjectGantTaskOwner owner)
        {
            resource_id = owner.OwnerId;
            text = owner.OwnerDesc;
        }
        public string resource_id { get; set; }

        //public int value { get; set; }

        //public DateTime startdate { get; set; }

        //public string start_date { get; set; }

        //public DateTime enddate { get; set; }

        //public string end_date { get; set; }

        public string text { get; set; }

    }

    public class GanttResource
    {        
        public string id { get; set; }

        public string text { get; set; }

        public string parent { get; set; }

    }
}
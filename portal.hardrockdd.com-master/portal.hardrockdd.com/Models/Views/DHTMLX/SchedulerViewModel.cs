using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.DHTMLX
{

    public class TimeLineEvent
    {
        public TimeLineEvent()
        {

        }

        public TimeLineEvent(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            id = job.KeyID;
            text = job.JobId;
            startdate = job.CalculatedStartDate(true);
            enddate = job.CalculatedEndDate(true);
            start_date = startdate.ToString("yyyy-MM-dd HH:mm");
            end_date = enddate.ToString("yyyy-MM-dd HH:mm");
            section_id = job.CrewId ?? "null";
            @readonly = false;
        }

        public long id { get; set; }

        public string text { get; set; }

        public string start_date { get; set; }

        public string end_date { get; set; }

        public DateTime startdate { get; set; }

        public DateTime enddate { get; set; }

        public string section_id { get; set; }

        public bool @readonly { get; set; }

    }

    public class TimeLineElement
    {
        public string key { get; set; }

        public string label { get; set; }

        public virtual List<TimeLineElement> children { get; set; }


        public static explicit operator TimeLineElement(DB.Infrastructure.ViewPointDB.Data.Crew crew)
        {
            return new TimeLineElement
            {
                key = crew.CrewId,
                label = crew.Description,
            };
        }
        public static explicit operator TimeLineElement(DB.Infrastructure.ViewPointDB.Data.PMProjectCrew crew)
        {
            return new TimeLineElement
            {
                key = crew.CrewId,
                label = crew.Crew?.Description,
            };
        }
    }
}
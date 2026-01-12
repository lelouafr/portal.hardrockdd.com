using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Crew
	{
		private VPContext _db;

		public VPContext db
		{
			set
			{
				_db = value;
			}
			get
			{
				_db ??= VPContext.GetDbContextFromEntity(this);
				_db ??= this.PRCompany.db;
				return _db;
			}
		}

		private string _DisplayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_DisplayName))
                {
                    _DisplayName = string.Format("{0}: {1}", CrewId, Description);
                }
                return _DisplayName;
            }
        }

        public bool Status 
        { 
            get
            {
                return CrewStatus.ToLower() == "active";
            }
            set
            {
                if (Status != value)
                {
                    CrewStatus = value ? "ACTIVE" : "INACTIVE";
                }
            }
        }


		public string JobId
		{
			get
			{
				return tJobId;
			}
			set
			{
				if (value != tJobId)
				{
					UpdateJobInfo(value);
				}
			}
		}


		public int? CrewLeaderId
		{
			get
			{
				return tCrewLeaderId;
			}
			set
			{
				if (value != tCrewLeaderId)
				{
					UpdateCrewLeader(value);
				}
			}
		}

		private void UpdateCrewLeader(int? newVal)
		{

			if (CrewLeader == null && tCrewLeaderId != null)
			{
				var obj = db.Employees.FirstOrDefault(f => f.EmployeeId == tCrewLeaderId);

				tCrewLeaderId = obj.EmployeeId;
				CrewLeader = obj;
			}

			if (tCrewLeaderId != newVal)
			{
				var obj = db.Employees.FirstOrDefault(f => f.EmployeeId == newVal);
				if (obj != null)
				{
					tCrewLeaderId = obj.EmployeeId;
					CrewLeader = obj;
				}
				else
				{
					tCrewLeaderId = null;
				}

			}
		}

		private void UpdateJobInfo(string newJobId)
		{
			if (DefaultJob == null && tJobId != null)
			{
				var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
				tJobId = job.JobId;
				JCCo = job.JCCo;
				DefaultJob = job;
			}

			if (tJobId != newJobId)
			{
				var job = db.Jobs.FirstOrDefault(f => f.JobId == newJobId);
				if (job != null)
				{
					tJobId = job.JobId;
					JCCo = job.JCCo;
					DefaultJob = job;

				}
				else
				{
					tJobId = null;
					JCCo = null;
					DefaultJob = null;
				}
			}
		}
	}

	public static class CrewExtension
    {
        public static string Label(this Crew crew)
        {
            if (crew == null)
                return "";

            var label = crew.Description;
            label = label.Replace("-", "");
            label = label.Replace("Crew", "");
            label = label.Replace("Trucking", "");
            label = label.Replace("WIRELINE", "");
            label = label.Trim();

            return label;
        }

    }
}
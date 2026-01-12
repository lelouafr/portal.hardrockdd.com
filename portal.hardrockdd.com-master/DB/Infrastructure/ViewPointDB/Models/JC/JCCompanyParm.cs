using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class JCCompanyParm
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
                if (_db == null)
                {
                    _db ??= this.HQCompanyParm.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }
        
        public Job TemplateJob
        {
            get
            {
                return db.Jobs.FirstOrDefault(f => f.JobId == this.JobId);
            }
        }

        public JCContract TemplateContract
        {
            get
            {
                return db.JCContracts.FirstOrDefault(f => f.ContractId == this.ContractId);
            }
        }

        public string NextProjectId(ProjectDivision division, Bid bid)
        {
            if (division == null)
                throw new System.ArgumentNullException(nameof(division));
            if (bid == null)
                throw new System.ArgumentNullException(nameof(bid));

            var startDate = bid.StartDate ?? DateTime.Now;
            if (startDate < DateTime.Now)
            {
                startDate = DateTime.Now;
            }
            var yearStr = startDate.Year.ToString(VPContext.AppCultureInfo).Substring(2, 2);
            var divisionStr = division.JobStartId.ToString().Substring(0, 1);
            var jobStr = string.Format(VPContext.AppCultureInfo, "{0}-{1}", yearStr, divisionStr);
            var LastId = db.Jobs.ToList()
                                 .Where(f => f.JobId.StartsWith(jobStr, StringComparison.Ordinal))
                                 .GroupBy(g => g.JobId.Substring(3, 4))
                                 .Max(max => max.Key);

            //throw new Exception();

            var jobIdint = int.TryParse(LastId, out int jobIdintOut) ? jobIdintOut : division.JobStartId ?? 1000;
            jobIdint++;
            jobStr = string.Format(VPContext.AppCultureInfo, "{0}-{1}-", yearStr, jobIdint.ToString(VPContext.AppCultureInfo).PadLeft(4, '0'));
            var jobList = new List<Job>();
            bid.ActivePackages.ToList().ForEach(p =>
            {
                if (p.Project != null)
                    jobList.Add(p.Project);
                p.ActiveBoreLines.ForEach(b =>
                {
                    if (b.Job != null)
                        jobList.Add(b.Job);
                });
            });
            jobList.AddRange(db.Jobs.Where(f => f.JobId == jobStr).ToList());

            while (jobList.Any(f => f.JobId == jobStr))
            {
                jobIdint++;
                jobStr = string.Format(VPContext.AppCultureInfo, "{0}-{1}-", yearStr, jobIdint.ToString(VPContext.AppCultureInfo).PadLeft(4, '0'));
            }

            return jobStr;
        }

        //public Job AddProject(Bid bid)
        //{

        //}

    }
}

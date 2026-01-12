//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.PM.Project.Form;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class JobRepository
//    {
//        public static List<Job> GetJobs(byte JCCo, bool openOnly = true)
//        {
//            ObjectCache systemCache = MemoryCache.Default;
//            CacheItemPolicy policy = new CacheItemPolicy
//            {
//                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(15)
//            };

//            var memKey = "JobList_" + openOnly.ToString();
//            if (!(MemoryCache.Default[memKey] is List<Job> jobList))
//            {
//                using var db = new VPContext();
//                jobList = db.Jobs
//                        .Where(f => f.JCCo == JCCo && f.JobStatus == (openOnly ? 1 : f.JobStatus))
//                        .ToList();
//                systemCache.Set(memKey, jobList, policy);
//            }

//            return jobList;
//        }

//        public static List<SelectListItem> GetSelectList(List<Job> List, string selected = "")
//        {
//            var result = List.Select(s => new SelectListItem
//            {
//                Value = s.JobId,
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
//                Selected = s.JobId == selected ? true : false
//            }).ToList();

//            return result;
//        }

//        public static List<Job> GetJobs(byte JCCo, string jobType, bool openOnly = true)
//        {
//            if (!(MemoryCache.Default["GetJobs_" + jobType] is List<Job> cache))
//            {
//                using var db = new VPContext();
//                var yearStr = DateTime.Now.Year.ToString(AppCultureInfo.CInfo()).Substring(2, 2);
//                var yearint = int.Parse(yearStr, AppCultureInfo.CInfo());
//                var qry = db.Jobs
//                            .Where(f =>
//                                        f.JobStatus == (openOnly ? 1 : f.JobStatus))
//                            .ToList()
//                            .Where(f => (f.Description + (int.Parse(f.JobId.Substring(0, 2), AppCultureInfo.CInfo()) <= yearint ? " Jobs" : "")).ToLower(AppCultureInfo.CInfo()).Contains(jobType.ToLower(AppCultureInfo.CInfo())))
//                            .ToList()//f.JCCo == JCCo &&
//                            ;

//                cache = qry;
//                ObjectCache systemCache = MemoryCache.Default;
//                CacheItemPolicy policy = new CacheItemPolicy
//                {
//                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
//                };
//                systemCache.Set("GetJobs_" + jobType, cache, policy);
//            }

//            return cache;
//        }

//    }
//}

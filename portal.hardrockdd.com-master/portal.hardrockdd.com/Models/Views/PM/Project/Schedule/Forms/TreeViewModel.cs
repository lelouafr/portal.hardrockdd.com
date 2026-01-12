using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Schedule
{
    public class TreeListViewModel
    {
        public TreeListViewModel()
        {
            List = new List<TreeViewModel>();
        }

        public TreeListViewModel(BidPackage package)
        {
            if (package == null) throw new System.ArgumentNullException(nameof(package));

            BDCo = package.BDCo;
            JobId = package.JobId;
            PackageId = package.PackageId;
            BidId = package.BidId;
            var unassignedCrewList = new TreeViewModel()
            {
                id = "-1",
                value = "Unassigned",
                items = package.ActiveBoreLines.Where(f => f.CrewId == null).Select(c => new TreeViewModel
                {
                    id = c.BoreId.ToString(),
                    value = c.Description,

                }).ToList()
            };
            var crewList = package.Crews.Where(f => f.CrewId != null).Select(s => new TreeViewModel
            {
                id = s.CrewId,
                value = s.Crew?.Description,
                items = package.ActiveBoreLines.Where(f => f.CrewId == s.CrewId).Select(c => new TreeViewModel
                {
                    id = c.BoreId.ToString(),
                    value = c.Description,

                }).ToList()
            }).ToList();

            crewList.Add(unassignedCrewList);

            List = crewList;
        }

        public TreeListViewModel(DB.Infrastructure.ViewPointDB.Data.Job project)
        {
            if (project == null) throw new System.ArgumentNullException(nameof(project));

            JCCo = project.JCCo;
            JobId = project.JobId;
            var unassignedCrewList = new TreeViewModel() { 
                id= "-1",
                value = "Unassigned",
                items = project.SubJobs.Where(f => f.CrewId == null).Select(c => new TreeViewModel
                {
                    id = c.JobId,
                    value = c.Description,
                    //order = c.

                }).ToList()
            };
            var crewList = project.Crews.Where(f => f.CrewId != null).Select(s => new TreeViewModel { 
                id = s.CrewId,
                value = s.Crew?.Description,
                items = project.SubJobs.Where(f => f.CrewId == s.CrewId).Select(c => new TreeViewModel { 
                    id = c.JobId,
                    value = c.Description,
                    //order = c.
                    
                }).ToList()
            }).ToList();

            crewList.Add(unassignedCrewList);

            List = crewList;
        }

        public byte JCCo { get; set; }

        public string JobId { get; set; }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }

        public List<TreeViewModel> List { get; }
    }

    public class TreeViewModel
    {
        public TreeViewModel()
        {

        }


        [HiddenInput]
        public string id { get; set; }

        public int order { get; set; }

        public string value { get; set; }


        public byte Co { get; set; }

        public string JobId { get; set; }

        public int BidId { get; set; }

        public int PackageId { get; set; }
        
        public int SeqId { get; set; }

        public virtual List<TreeViewModel> items { get; set; }


    }
}
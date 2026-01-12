using DB.Infrastructure.ViewPointDB.Data;
using DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class LocateAssignmentListViewModel
    {
        public LocateAssignmentListViewModel()
        {
            List = new List<LocateAssignmentViewModel>
            {
                new LocateAssignmentViewModel()
            };
        }

        public LocateAssignmentListViewModel(PMLocate locate)
        {
            if (locate == null)
                return;
            LocateId = locate.LocateId;

            var packages = locate.Bid?.ActivePackages;
            var assignments = locate.Assignments.ToList();
            if (locate.Bid == null)
            {
                Tree = new List<LocateAssignmentViewModel>();
                List = new List<LocateAssignmentViewModel>();
            }
            else
            {
                Tree = new List<LocateAssignmentViewModel>
                {
                    new LocateAssignmentViewModel(locate, locate.Bid, assignments)
                };

                List = packages.Select(s => new LocateAssignmentViewModel(locate, s, assignments)).ToList();
            }
        }


        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        public List<LocateAssignmentViewModel>? List { get; }

        public List<LocateAssignmentViewModel>? Tree { get; }

    }

    public class LocateAssignmentViewModel
    {
        public LocateAssignmentViewModel()
        {

        }

        public LocateAssignmentViewModel(PMLocate locate, DB.Infrastructure.ViewPointDB.Data.Bid? bid, List<PMLocateAssignment> assignments)
        {
            if (locate == null || assignments == null || bid == null)
                return;

            LocateId = locate.LocateId;
            SeqId = 0;
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            Bid = bid.Description;

            JCCo = bid.JCCo;

            Description = bid?.Description;

            children = bid.ActivePackages.Select(s => new LocateAssignmentViewModel(locate, s, assignments)).ToList();

            ChildCnt = children.Sum(s => s.ChildCnt);
            AssignedCnt = children.Sum(s => s.AssignedCnt);
            Footage = children.Sum(s => s.Footage);

            Included = children.Count == children.Sum(f => f.Included ? 1 : 0);
        }

        public LocateAssignmentViewModel(PMLocate locate, BidPackage? package, List<PMLocateAssignment> assignments)
        {
            if (locate == null || assignments == null || package == null)
                return;

            LocateId = locate.LocateId;
            SeqId = 0;
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            Package = package.Description;

            JCCo = package.JCCo;
            JobId = package.JobId;
            var job = package.Project;
            Description = package.Description;
            if (job != null)
            {
                Description = string.Format("{0}: {1}", job.JobId, job.Description);
            }

            var list = package.ActiveBoreLines.LeftOuterJoin(assignments,
                                           bore => bore.BoreId,
                                           asm => asm.BoreId,
                                           (bore, asm) => new { bore, asm }).ToList();

            children = list.Select(s => new LocateAssignmentViewModel(locate, s.asm, s.bore)).ToList();

            ChildCnt = children.Sum(s => s.ChildCnt);
            AssignedCnt = children.Sum(s => s.AssignedCnt);
            Footage = children.Sum(s => s.Footage);

            Included = children.Count == children.Sum(f => f.Included ? 1 : 0);
        }

        public LocateAssignmentViewModel(PMLocate locate, PMLocateAssignment asm, BidBoreLine? bore)
        {
            if (locate == null || bore == null)
                return;

            LocateId = locate.LocateId;
            SeqId = asm?.SeqId ?? 0;
            BDCo = bore.BDCo;

            BidId = bore.BidId;
            Bid = bore.Bid.Description;

            PackageId = bore.PackageId;
            Package = bore.Package.Description;

            BoreId = bore.BoreId;
            JCCo = bore.JCCo;
            JobId = bore.JobId;
            var job = bore.Job;
            Description = bore.Description;
            if (job != null)
            {
                Description = string.Format("{0}: {1}", job.JobId, job.Description);
            }

            Included = asm != null;
            ChildCnt = 1;
            AssignedCnt = Included ? 1 : 0;

            Footage = ((decimal?)bore?.Job?.Footage ?? bore.Footage) ?? 0;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public byte? BDCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int? BidId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public string Bid { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int? PackageId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Package")]
        public string Package { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int? BoreId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "#")]
        public byte? JCCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string Description { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Included")]
        public bool Included { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Count")]
        public int ChildCnt { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Assigned Cnt")]
        public int AssignedCnt { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Footage")]
        public decimal Footage { get; set; }

        public List<LocateAssignmentViewModel> children { get; }

        internal LocateAssignmentViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var locate = db.PMLocates.FirstOrDefault(f => f.LocateId == this.LocateId);
            if (locate == null)
            {
                return this;
            }
            if (this.PackageId == null)
            {
                var bid = db.Bids.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId);

                var model = new LocateAssignmentViewModel(locate, bid, locate.Assignments.ToList());
                model.children.ForEach(e => e.Included = Included);
                model.children.ForEach(e => e.ProcessUpdate(db, modelState));
                model.Included = Included;
                model = new LocateAssignmentViewModel(locate, bid, locate.Assignments.ToList());
                return model;
            }
            else if (this.BoreId == null)
            {
                var package = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

                var model = new LocateAssignmentViewModel(locate, package, locate.Assignments.ToList());
                model.children.ForEach(e => e.Included = Included);
                model.children.ForEach(e => e.ProcessUpdate(db, modelState));
                model.Included = Included;
                model = new LocateAssignmentViewModel(locate, package, locate.Assignments.ToList());
                return model;
            }
            var bore = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId && f.BoreId == BoreId);
            var asm = locate.Assignments.FirstOrDefault(f => f.LocateId == this.LocateId && f.SeqId == this.SeqId);
            if (asm == null && Included)
                asm = locate.AddAssignment();

            if (asm != null)
            {
                if (Included)
                {
                    asm.BDCo = this.BDCo;
                    asm.BidId = this.BidId;
                    asm.PackageId = this.PackageId;
                    asm.BoreId = this.BoreId;
                    asm.JCCo = this.JCCo;
                    asm.JobId = this.JobId;
                }
                else
                {
                    locate.Assignments.Remove(asm);
                    //db.PMLocateAssignments.Remove(updObj);
                    asm = null;
                }

                try
                {
                    db.BulkSaveChanges();
                    return new LocateAssignmentViewModel(locate, asm, bore);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
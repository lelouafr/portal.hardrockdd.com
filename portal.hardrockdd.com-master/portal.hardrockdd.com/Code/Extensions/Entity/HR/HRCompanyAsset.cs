using portal.Repository.VP.AP.CreditCard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace portal.Code.Data.VP
{
    public partial class HRCompanyAsset
    {

        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = this.HRCompanyParm.db;
                    _db ??= VPEntities.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }

        public string BaseTableName { get { return "bHRCA"; } }



        public DB.HRAssestStatusEnum Status { get => (DB.HRAssestStatusEnum)(tStatusId ?? 0); set => tStatusId = (byte)value; }

        public int? AssignedId { get => this.tAssignedId; set => UpdateAssignedEmployee(value); }

        public void UpdateAssignedEmployee(int? value)
        {
            //var assigned = this.Assignments.FirstOrDefault(f => f.HRRef == this.AssignedId);
            //var emp = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == this.AssignedId);
            var originalAssignedId = this.tAssignedId;


            if (Assigned == null && tAssignedId != null)
            {
                var emp = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == this.tAssignedId);
                if (emp != null)
                {
                    tAssignedId = emp.HRRef;
                    Assigned = emp;
                }
            }

            if (tAssignedId != value)
            {
                var emp = db.HRResources.FirstOrDefault(f => f.HRCo == this.HRCo && f.HRRef == value);
                if (emp != null)
                {
                    tAssignedId = emp.HRRef;
                    Assigned = emp;
                }
                else
                {
                    tAssignedId = null;
                    Assigned = null;
                }

                if (Assigned != null)
                {
                    this.Assignments.Where(f => f.DateIn == null).ToList().ForEach(e => {
                        e.DateIn = DateTime.Now.AddDays(-1).Date;
                        e.MemoIn = string.Format("Asset reassigned to {0}", Assigned.FullName(false));
                    });
                    db.BulkSaveChanges();
                    this.AddAssignment();
                }
                else
                {
                    this.Assignments.Where(f => f.HRRef == originalAssignedId && f.DateIn == null).ToList().ForEach(e => {
                        e.DateIn = DateTime.Now.AddDays(-1).Date;
                        e.MemoIn = string.Format("Asset returned");
                    });
                }
            }
        }

        public HRAssetAssignment AddAssignment()
        {
            var assignment = this.Assignments.FirstOrDefault(f => f.HRRef == this.AssignedId);
            if (assignment == null)
            {
                assignment = new HRAssetAssignment()
                {
                    HRCo = this.HRCo,
                    HRRef = this.Assigned.HRRef,
                    AssetId = this.AssetId,
                    DateOut = DateTime.Now.Date,
                };
                this.Assignments.Add(assignment);

            }
            return assignment;
        }
    }

    //public static class HRCompanyAssetExtension
    //{

    //    public static HRResource CurrentAssignedEmployee(this HRCompanyAsset asset)
    //    {
    //        if (asset == null) 
    //            return null;

    //        var assignedTo = asset.Assignments.FirstOrDefault(f => f.DateIn == null & f.DateOut != null);
    //        if (assignedTo == null)
    //            assignedTo = asset.Assignments.OrderByDescending(o => o.DateOut).FirstOrDefault(f => f.DateIn != null & f.DateOut != null);


    //        return assignedTo.HREmployee;
    //    }

    //}
}
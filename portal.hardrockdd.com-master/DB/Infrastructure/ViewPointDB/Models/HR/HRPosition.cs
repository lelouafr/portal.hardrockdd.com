using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HRPosition
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
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.HRCompany?.db;

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "bHRPC"; } }

        public HRPositionTask AddHireTask()
        {
            var task = new HRPositionTask()
            {
                HRCo = this.HRCo,
                PositionCodeId = this.PositionCodeId,
                SeqId = this.DefaultTasks.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

                Position = this,

            };

            this.DefaultTasks.Add(task);
            db.BulkSaveChanges();

            return task;
        }


    }
}
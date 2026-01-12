using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WAAppliedPosition
    {
        public static string BaseTableName { get { return "budWAAP"; } }

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
                return _db;
            }
        }


        public string? PositionCodeId
        {
            get
            {
                return tPositionCodeId;
            }
            set
            {
                if (value != tPositionCodeId)
                    UpdatePositionCode(value);
            }
        }

        private void UpdatePositionCode(string? newVal)
        {
            if (HRPositionCode == null && tPositionCodeId != null)
            {
                var obj = db.HRPositions.FirstOrDefault(f => f.PositionCodeId == tPositionCodeId);
                if (obj != null)
                {
                    tPositionCodeId = obj.PositionCodeId;
                    HRCo = obj.HRCo;
                    HRPositionCode = obj;
                }
            }

            if (tPositionCodeId != newVal)
            {
                var obj = db.HRPositions.FirstOrDefault(f => f.PositionCodeId == newVal);
                if (obj != null)
                {
                    tPositionCodeId = obj.PositionCodeId;
                    HRCo = obj.HRCo;
                    HRPositionCode = obj;
                }
                else
                {
                    HRCo = null;
                    tPositionCodeId = null;
                    HRPositionCode = null;
                }
            }
        }
    }
}
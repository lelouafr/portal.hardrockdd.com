using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WPMapSet
    {
        public VPContext _db;

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

        public WPMapCoord AddCoord()
        {
            var cord = new WPMapCoord()
            {
                db = db,
                MapSetId = this.MapSetId,
                CoordId = this.Coords.DefaultIfEmpty().Max(max => max == null ? 0 : max.CoordId) + 1,

            };

            this.Coords.Add(cord);

            return cord;
        }

        public static string BaseTableName { get { return "budWPMM"; } }
    }
}

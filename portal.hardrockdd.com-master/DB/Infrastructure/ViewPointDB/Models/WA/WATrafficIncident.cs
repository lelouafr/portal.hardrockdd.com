using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WATrafficIncident
    {
        public static string BaseTableName { get { return "budWAIH"; } }

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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public WAIncidentTypeEnum IncidentType 
        { 
            get
            {
                return (WAIncidentTypeEnum)IncidentTypeId;
            }
            set
            {
                IncidentTypeId = (byte)value;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQOffice
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
                return _db;
            }
        }

        public static string BaseTableName { get { return "budHQOM"; } }
    }
    public static class HQOfficeExt
    {
        public static HQOffice AddOffice(this VPContext db)
        {
            var office = new HQOffice()
            {
                //Locate = this,
                db = db,
                OfficeId = db.GetNextId(HQOffice.BaseTableName),
            };
            db.HQOffices.Add(office);
            return office;
        }

    }
}

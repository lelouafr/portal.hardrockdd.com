using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class ForumLine
    {
        public static string BaseTableName { get { return "budWPFL"; } }

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
                    _db = this.Forum.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }
    }
}
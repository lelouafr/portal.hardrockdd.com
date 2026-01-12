using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class ForumLine
    {
        public string BaseTableName { get { return "budWPFL"; } }

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
                    _db = this.Forum.db;
                    _db ??= VPEntities.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }
    }
}
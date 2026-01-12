using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Forum
    {
        public VPEntities _db;

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
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        
        public static string BaseTableName { get { return "budWPFH"; } }

        public ForumLine AddLine()
        {
            var line = new ForumLine()
            {
                Co = this.Co,
                ForumId = this.ForumId,
                LineId = this.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                UniqueAttchID = Guid.NewGuid(),
                ParentForumId = this.ForumId,

                db = this.db,
            };

            this.Lines.Add(line);
            try
            {
                db.BulkSaveChanges();
            }
            catch (Exception)
            {
            }
            

            return line;
        }

        //public static AddForum(byte co)
        //{

        //}
    }

    public interface IForum
    {
        public Forum AddForum();

        public Forum GetForum();

    }
}
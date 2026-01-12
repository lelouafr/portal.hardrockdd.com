using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class CreditImageLink
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
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Transaction.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budCMIT"; } }


        public HQAttachmentFile _file;

        public HQAttachmentFile File()
        {
            if (_file == null)
            {
                _file = db.HQAttachmentFiles.FirstOrDefault(f => f.HQCo == this.Image.CCCo && f.AttachmentId == this.Image.AttachmentId);
            }

            return _file;
        }
    }
}
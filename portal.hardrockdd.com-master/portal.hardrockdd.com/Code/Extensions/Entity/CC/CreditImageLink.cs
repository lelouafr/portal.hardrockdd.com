using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class CreditImageLink
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
                        _db = this.Transaction.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

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
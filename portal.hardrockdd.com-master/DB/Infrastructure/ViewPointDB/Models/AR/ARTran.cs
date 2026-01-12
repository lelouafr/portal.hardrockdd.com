using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class ARTran
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
                    _db ??= this.Company.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "bARTH"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                    return HQAttachment;

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = this.ARCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                    HQCompanyParm = this.Company,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return HQAttachment;
            }
        }
    }
}
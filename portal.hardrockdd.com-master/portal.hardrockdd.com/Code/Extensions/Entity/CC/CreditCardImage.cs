using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{

    public partial class CreditCardImage
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
        public string BaseTableName { get { return "buCMIB"; } }

        public HQAttachment Attachment
        {
            get
            {
                var HQAttachment = db.HQAttachments.FirstOrDefault(f => f.HQCo == this.CCCo && f.UniqueAttchID == this.UniqueAttchID);
                var compParm = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == this.CCCo);
                if (HQAttachment == null && UniqueAttchID != null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.CCCo,
                        UniqueAttchID = (Guid)UniqueAttchID,
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = compParm,
                    };

                    db.HQAttachments.Add(HQAttachment);
                }
                else if (HQAttachment == null && UniqueAttchID == null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.CCCo,
                        UniqueAttchID = Guid.NewGuid(),
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = compParm,
                    };
                    UniqueAttchID = HQAttachment.UniqueAttchID;
                    db.HQAttachments.Add(HQAttachment);
                }

                return HQAttachment;
            }
        }

    }
}
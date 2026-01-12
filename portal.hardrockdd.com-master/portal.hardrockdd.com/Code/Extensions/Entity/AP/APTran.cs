using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class APTran
    {
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
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = VPEntities.GetDbContextFromEntity(this.APCompanyParm);

                    if (_db == null)
                        _db = VPEntities.GetDbContextFromEntity(this.APCompanyParm.HQCompanyParm);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public string BaseTableName { get { return "APTH"; } }


        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                    return HQAttachment;

                if (HQAttachment == null && UniqueAttchID != null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.APCo,
                        UniqueAttchID = (Guid)UniqueAttchID,
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = this.APCompanyParm.HQCompanyParm,
                    };

                    db.HQAttachments.Add(HQAttachment);
                    db.BulkSaveChanges();
                }
                else if (HQAttachment == null && UniqueAttchID == null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.APCo,
                        UniqueAttchID = Guid.NewGuid(),
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = this.APCompanyParm.HQCompanyParm,
                    };
                    UniqueAttchID = HQAttachment.UniqueAttchID;
                    db.HQAttachments.Add(HQAttachment);
                    db.BulkSaveChanges();
                }

                return HQAttachment;
            }
        }

        public APBatchHeader ToBatch(Batch batch)
        {
            var batchSeq = new APBatchHeader
            {
                APCo = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                APTransId = this.APTransId,
                BatchSeq = batch.APBatches.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq) + 1,
                BatchTransType = "C",
                VendorGroupId = VendorGroupId,
                VendorId = VendorId,
                Description = Description,
                APRef = APRef,
                InvDate = InvDate,
                DueDate = DueDate,
                InvTotal = InvTotal,
                PayMethod = PayMethod,
                CMCo = CMCo,
                CMAcct = CMAcct,
                PrePaidYN = PrePaidYN,
                PrePaidProcYN = PrePaidProcYN,
                PayOverrideYN = PayOverrideYN,
                V1099YN = V1099YN,
                SeparatePayYN = SeparatePayYN,
                ChkRev = ChkRev,
                PaidYN = "N",
                CCTransId = CCTransId,
                MerchantId = MerchantId,
                UniqueAttchID = UniqueAttchID,


                OldVendorGroup = VendorGroupId,
                OldVendor = VendorId,
                OldDesc = Description,
                OldAPRef = APRef,
                OldInvDate = InvDate,
                OldDueDate = DueDate,
                OldInvTotal = InvTotal,
                OldPayMethod = PayMethod,
                OldCMCo = CMCo,
                OldCMAcct = CMAcct,
                OldPrePaidYN = PrePaidYN,
                OldPrePaidProcYN = PrePaidProcYN,
                OldPayOverrideYN = PayOverrideYN,
                Old1099YN = V1099YN,
                OldSeparatePayYN = SeparatePayYN,

                
                

                Batch = batch,
                Company = batch.Company,
            };

            return batchSeq;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PMLocateSequence: IAttachment
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

        public static string BaseTableName { get { return "budPMLS"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                {
                    return HQAttachment;

                }
                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == (this.HQCo ?? 1));
                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = comp.HQCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.LocateId,
                    TableName = BaseTableName,
                    HQCompanyParm = comp,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return Attachment;
            }
        }

        public int? StatusId
        {
            get => tStatusId;
            set => UpdateStatus(value);
        }

        public PMLocateStatusEnum Status
        {
            get => (PMLocateStatusEnum)(StatusId ?? 0);
            set => StatusId = (int)value;
        }

        private void UpdateStatus(int? value)
        {
            if (value == tStatusId)
                return;

            value ??= 0;
            var newStatus = (PMLocateStatusEnum)value;
            if (Status == PMLocateStatusEnum.Active && newStatus == PMLocateStatusEnum.New)
            {
                var priorSeq = this.Locate.Sequences.FirstOrDefault(f => f.SeqId == this.SeqId - 1);
                if (priorSeq.Status != PMLocateStatusEnum.Active)
                    priorSeq.tStatusId = (int)PMLocateStatusEnum.Active;
            }
            else if (Status == PMLocateStatusEnum.Closed && newStatus == PMLocateStatusEnum.Active)
            {
                var nextSeq = this.Locate.Sequences.FirstOrDefault(f => f.SeqId == this.SeqId + 1);
                if (nextSeq != null)
                {
                    if (nextSeq.Status != PMLocateStatusEnum.New)
                        nextSeq.tStatusId = (int)PMLocateStatusEnum.New;
                }
            }
            else if (Status == PMLocateStatusEnum.Closed) 
            {

            }
            tStatusId = value;
            if (Status == DB.PMLocateStatusEnum.Active)
            {
                var sequences = Locate.Sequences.ToList();
                foreach (var seq in sequences)
                {
                    if (seq.SeqId < SeqId)
                        seq.Status = DB.PMLocateStatusEnum.Closed;
                }
            }

        }
        public string GetSharePointRootFolderPath()
        {
            throw new NotImplementedException();
        }

        public SPList GetSharePointList()
        {
            throw new NotImplementedException();
        }
    }
}

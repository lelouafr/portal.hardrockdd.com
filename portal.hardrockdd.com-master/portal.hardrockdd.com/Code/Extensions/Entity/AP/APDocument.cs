using System;
using System.Linq;

namespace portal.Code.Data.VP
{
    public partial class APDocument: IForum, IWorkFlow
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


                }
                return _db;
            }
        }

        public string BaseTableName { get { return "budAPDH"; } }

        public DB.APDocumentStatusEnum Status
        {
            get
            {
                return (DB.APDocumentStatusEnum)StatusId;
            }
            set
            {
                StatusId = (byte)value;
            }
        }

        public byte StatusId
        {
            get
            {
                return tStatusId ?? 0;
            }
            set
            {
                if (value != tStatusId)
                {
                    UpdateStatus(value);
                }
            }
        }

        private APDocumentSeq _APDocumentSeq;
        
        public APDocumentSeq APDocumentSeq 
        { 
            get
            {
                if (_APDocumentSeq == null)
                {
                    _APDocumentSeq = DocumentSeqs.FirstOrDefault();
                }
                return _APDocumentSeq;
            }
            set 
            {
                _APDocumentSeq = value;
            } 
        }

        public void UpdateStatus(byte newValue)
        {

            if (WorkFlow == null)
                AddWorkFlow();
            tStatusId = newValue;
            var statusComments = string.Empty;
            var status = (DB.APDocumentStatusEnum)newValue;
            switch (status)
            {
                case DB.APDocumentStatusEnum.New:
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.Duplicate:
                case DB.APDocumentStatusEnum.Canceled:
                    WorkFlow.CreateSequance(newValue);
                    AddWorkFlowAssignments();
                    break;
                case DB.APDocumentStatusEnum.Error:
                    WorkFlow.CreateSequance(newValue);
                    AddWorkFlowAssignments();
                    statusComments = "DB Error";
                    break;
                case DB.APDocumentStatusEnum.RequestedInfo:
                    WorkFlow.CreateSequance(newValue);
                    AddWorkFlowAssignments();
                    break;
                case DB.APDocumentStatusEnum.Processed:
                    WorkFlow.CreateSequance(newValue);
                    AddWorkFlowAssignments();
                    AddToBatch();
                    break;
                case DB.APDocumentStatusEnum.Reviewed:
                case DB.APDocumentStatusEnum.LinesAdded:
                case DB.APDocumentStatusEnum.All:
                    break;
                default:
                    break;
            }

            GenerateStatusLog(statusComments);
        }

        public WorkFlow GetWorkFlow()
        {
            var workFlow = WorkFlow;
            if (workFlow == null)
            {
                workFlow = AddWorkFlow();
            }

            return workFlow;
        }

        public WorkFlow AddWorkFlow()
        {
            var workFlow = WorkFlow;
            if (workFlow == null)
            {
                workFlow = new WorkFlow
                {
                    WFCo = APCo,
                    WorkFlowId = db.GetNextId(WorkFlow.BaseTableName,1),
                    TableName = BaseTableName,
                    Id = DocId,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = APCompanyParm.HQCompanyParm,
                    db = this.db,
                };
                db.WorkFlows.Add(workFlow);
                WorkFlow = workFlow;
                WorkFlowId = workFlow.WorkFlowId;
                workFlow.AddSequance(this.StatusId);
                this.AddWorkFlowAssignments();
                db.BulkSaveChanges();
            }

            return workFlow;
        }
        
        public void AddWorkFlowAssignments(bool reset = false)
        {
            if (WorkFlow == null)
            {
                AddWorkFlow();
            }
            if (reset)
                WorkFlow.CurrentSequance().AssignedUsers.Clear();

            switch (Status)
            {
                case DB.APDocumentStatusEnum.New:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.Error:
                    break;
                case DB.APDocumentStatusEnum.Duplicate:
                case DB.APDocumentStatusEnum.Processed:
                case DB.APDocumentStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.APDocumentStatusEnum.RequestedInfo:
                    if (! string.IsNullOrEmpty(RequestedUserList))
                    {
                        var userList = RequestedUserList.Split('|');
                        foreach (var userId in userList)
                        {
                            WorkFlow.AddUser(userId);
                        }
                    }
                    break;
                case DB.APDocumentStatusEnum.Reviewed:
                case DB.APDocumentStatusEnum.LinesAdded:
                default:
                    break;
            }

            if (APCo == 1)
            {
                WorkFlow.AddByEmail("trudy.moore@hardrockdd.com");
            }
            else if (APCo == 10)
            {
                WorkFlow.AddByEmail("brandi@raymondconstruction.net");
            }
            WorkFlow.AddByEmail("bobby.hoover@hardrockdd.com");
            WorkFlow.AddByEmail("tj.wheeler@hardrockdd.com");
        }

        public void GenerateStatusLog()
        {
            var log = new APDocumentStatusLog
            {
                APCo = APCo,
                DocId = DocId,
                LineNum = StatusLogs.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                Status = tStatusId,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            StatusLogs.Add(log);
        }

        public void GenerateStatusLog(string comments)
        {
            var log = new APDocumentStatusLog
            {
                APCo = APCo,
                DocId = DocId,
                LineNum = StatusLogs.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                Status = tStatusId,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId(),
                Comments = comments
            };

            StatusLogs.Add(log);
        }

        public void AddToBatch()
        {
            var mth = DocumentSeqs.FirstOrDefault().Mth;
            var batch = Batch.FindCreate(APCompanyParm.HQCompanyParm, "APHB", "AP Entry", mth);
            batch.InUseBy = "WebPortalUser";
            foreach (var seq in DocumentSeqs)
            {
                var batchSeq = new APBatchHeader
                {
                    APCo = batch.Co,
                    Mth = batch.Mth,
                    BatchId = batch.BatchId,
                    BatchSeq = batch.APBatches.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq) + 1,
                    BatchTransType = "A",
                    VendorGroupId = (byte)seq.VendorGroupId,
                    VendorId = (int)seq.VendorId,
                    Description = seq.Description.Length >= 29 ? seq.Description.Substring(0,29) : seq.Description,
                    APRef = seq.APRef,
                    InvDate = (DateTime)seq.InvDate,
                    DueDate = (DateTime)seq.DueDate,
                    InvTotal = seq.InvTotal ?? 0,
                    PayMethod = seq.Vendor.PayMethod,
                    CMCo = APCompanyParm.CMCo,
                    CMAcct = seq.Vendor.CMAcct ?? APCompanyParm.CMAcct,
                    PrePaidYN = "N",
                    PrePaidProcYN = "N",
                    PayOverrideYN = "N",
                    V1099YN = seq.Vendor.V1099YN,
                    V1099Type = seq.Vendor.V1099YN == "Y" ? seq.Vendor.V1099Type : null,
                    V1099Box = seq.Vendor.V1099YN == "Y" ? seq.Vendor.V1099Box : null,
                    SeparatePayYN = "N",
                    ChkRev = "N",
                    PaidYN = "N",
                    Batch = batch,
                    Company = APCompanyParm.HQCompanyParm,
                };

                if (!db.CMAccounts.Any(f => f.CMCo == seq.CMCo && f.CMAcct == seq.CMAcct))
                {
                    seq.CMAcct = null;
                }
                seq.Lines.ToList().ForEach(line => batchSeq.AddLine(line));

                batchSeq.Attachment.AddFile(this.DocumentName, this.DocData);

                seq.Batch = batch;
                seq.BatchId = batchSeq.BatchId;
                seq.BatchSeq = batchSeq.BatchSeq;
                batch.APBatches.Add(batchSeq);
            }
        }
        
        public APDocumentSeq AddSequance()
        {
            var emp = StaticFunctions.GetCurrentEmployee();
            var employee = db.Employees.FirstOrDefault(f => f.PRCo == emp.PRCo && f.EmployeeId == emp.EmployeeId);
            var seq = new APDocumentSeq()
            {
                APCo = APCo,
                DocId = DocId,
                SeqId = 1,
                Mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DivisionId = employee.DivisionId,
                Division = employee.Division,

                Document = this
            };

            DocumentSeqs.Add(seq);
            return seq;
        }

        public Forum GetForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = AddForum();
            }
            return forum;
        }

        public Forum AddForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = new Forum()
                {
                    Co = this.APCo,
                    ForumId = db.GetNextId(Forum.BaseTableName),
                    RelKeyID = this.KeyID,
                    TableName = this.BaseTableName,

                    db = this.db
                };

                this.Forum = forum;
                db.BulkSaveChanges();
            }

            return forum;
        }

        //public static int NextDocId(APCompanyParm apComp)
        //{
        //    var db = VPEntities.GetDbContextFromEntity(apComp);
        //    var docId = db.GetNextId("budAPDH", 1);

        //    return docId;
        //}
    }
}
using System;
using System.Linq;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class APDocument: IForum, IWorkFlow
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
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = VPContext.GetDbContextFromEntity(this.APCompanyParm);


                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budAPDH"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                    return HQAttachment;

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = this.APCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                    HQCompanyParm = this.APCompanyParm.HQCompanyParm,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return HQAttachment;
            }
        }

        public APDocumentStatusEnum Status
        {
            get
            {
                return (APDocumentStatusEnum)StatusId;
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

        //private APDocumentSeq _APDocumentSeq;
        
        //public APDocumentSeq APDocumentSeq 
        //{ 
        //    get
        //    {
        //        if (_APDocumentSeq == null)
        //        {
        //            _APDocumentSeq = DocumentSeqs.FirstOrDefault();
        //        }
        //        return _APDocumentSeq;
        //    }
        //    set 
        //    {
        //        _APDocumentSeq = value;
        //    } 
        //}

        public void UpdateStatus(byte newValue)
        {

            if (WorkFlow == null)
                AddWorkFlow();
            tStatusId = newValue;
            var statusComments = string.Empty;
            var status = (APDocumentStatusEnum)newValue;
            switch (status)
            {
                case APDocumentStatusEnum.New:
                case APDocumentStatusEnum.Filed:
                case APDocumentStatusEnum.Duplicate:
                case APDocumentStatusEnum.Canceled:
                    WorkFlow.CreateSequence(newValue);
                    AddWorkFlowAssignments();
                    break;
                case APDocumentStatusEnum.Error:
                    WorkFlow.CreateSequence(newValue);
                    AddWorkFlowAssignments();
                    statusComments = "DB Error";
                    break;
                case APDocumentStatusEnum.RequestedInfo:
                    WorkFlow.CreateSequence(newValue);
                    AddWorkFlowAssignments();
                    break;
                case APDocumentStatusEnum.Processed:
                    WorkFlow.CreateSequence(newValue);
                    AddWorkFlowAssignments();
                    AddToBatch();
                    break;
                case APDocumentStatusEnum.Reviewed:
                case APDocumentStatusEnum.LinesAdded:
                case APDocumentStatusEnum.All:
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
                workFlow.AddSequence(this.StatusId);
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
                WorkFlow.CurrentSequence().AssignedUsers.Clear();

            switch (Status)
            {
                case APDocumentStatusEnum.New:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case APDocumentStatusEnum.Filed:
                case APDocumentStatusEnum.Error:
                    break;
                case APDocumentStatusEnum.Duplicate:
                case APDocumentStatusEnum.Processed:
                case APDocumentStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case APDocumentStatusEnum.RequestedInfo:
                    if (! string.IsNullOrEmpty(RequestedUserList))
                    {
                        var userList = RequestedUserList.Split('|');
                        foreach (var userId in userList)
                        {
                            WorkFlow.AddUser(userId);
                        }
                    }
                    break;
                case APDocumentStatusEnum.Reviewed:
                case APDocumentStatusEnum.LinesAdded:
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
                CreatedBy = db.CurrentUserId
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
                CreatedBy = db.CurrentUserId,
                Comments = comments
            };

            StatusLogs.Add(log);
        }

        public void AddToBatch()
        {
            var mth = (DateTime)Mth;

            var batch = Batch.FindCreate(APCompanyParm.HQCompanyParm, "APHB", "AP Entry", mth);
            batch.InUseBy = "WebPortalUser";

            var batchHeaders = db.APBatchHeaders
                .Where(f => f.BatchId == batch.BatchId && f.Mth == batch.Mth && f.APCo == batch.Co)
                .ToList();
            var seqId = batchHeaders.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq) + 1;

            var batchSeq = new APBatchHeader
            {
                APCo = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchSeq = seqId,
                BatchTransType = "A",
                VendorGroupId = (byte)VendorGroupId,
                VendorId = (int)VendorId,
                Description = Description.Length >= 29 ? Description.Substring(0,29) : Description,
                APRef = APRef,
                InvDate = (DateTime)InvDate,
                DueDate = (DateTime)DueDate,
                InvTotal = InvTotal ?? 0,
                PayMethod = Vendor.PayMethod,
                CMCo = APCompanyParm.CMCo,
                CMAcct = Vendor.CMAcct ?? APCompanyParm.CMAcct,
                PrePaidYN = "N",
                PrePaidProcYN = "N",
                PayOverrideYN = "N",
                V1099YN = Vendor.V1099YN,
                V1099Type = Vendor.V1099YN == "Y" ? Vendor.V1099Type : null,
                V1099Box = Vendor.V1099YN == "Y" ? Vendor.V1099Box : null,
                SeparatePayYN = "N",
                ChkRev = "N",
                PaidYN = "N",
                Batch = batch,
                Company = APCompanyParm.HQCompanyParm,
                //UniqueAttchID = file.UniqueAttchID,
            };
            foreach (var file in this.Attachment.GetRootFolder().Files)
            {
                batchSeq.Attachment.GetRootFolder().CopyFile(file);
            }
            
            if (!db.CMAccounts.Any(f => f.CMCo == CMCo && f.CMAcct == CMAcct))
            {
                CMAcct = null;
            }
            Lines.ToList().ForEach(line => batchSeq.AddLine(line));

            Batch = batch;
            BatchId = batchSeq.BatchId;
            BatchSeq = batchSeq.BatchSeq;
            batch.APBatches.Add(batchSeq);
            
        }
        
        public void MoveDocumentToAttachment()
        {
            if (this.DocData != null)
            {
                this.Attachment.GetRootFolder().AddFile(this.DocumentName, this.DocData);
                this.DocData = null;
            }
        }

        //public APDocumentSeq AddSequence()
        //{
        //    var emp = db.GetCurrentEmployee();
        //    var seq = new APDocumentSeq()
        //    {
        //        Document = this,
        //        APCo = APCo,
        //        DocId = DocId,
        //        SeqId = 1,
        //        Mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
        //        DivisionId = emp.DivisionId,
        //        Division = emp.Division,

        //    };

        //    DocumentSeqs.Add(seq);
        //    return seq;
        //}

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
                    TableName = BaseTableName,

                    db = this.db
                };

                this.Forum = forum;
                db.BulkSaveChanges();
            }

            return forum;
        }

        //public static int NextDocId(APCompanyParm apComp)
        //{
        //    var db = VPContext.GetDbContextFromEntity(apComp);
        //    var docId = db.GetNextId("budAPDH", 1);

        //    return docId;
        //}



        public DateTime? InvDate
        {
            get
            {
                return tInvDate;
            }
            set
            {
                if (tInvDate != value)
                {
                    tInvDate = value;
                    if (Vendor != null && tInvDate != null)
                    {
                        DueDate = tInvDate.Value.AddDays(Vendor.PayTerm?.DaysTillDue ?? 0);
                    }
                }
            }
        }

        public string APRef
        {
            get
            {
                if (tAPRef == null)
                    tAPRef ??= string.Empty;
                if (tAPRef.Length > 15)
                    tAPRef = tAPRef.Substring(0, 14);

                return tAPRef;
            }
            set
            {
                if (tAPRef != value)
                {
                    value ??= string.Empty;
                    value = value.Trim();
                    if (value.Length > 15)
                        value = value.Substring(0, 14);

                    tAPRef = value;
                }
            }
        }

        public string Description
        {
            get
            {
                if (tDescription == null)
                    tDescription ??= string.Empty;
                if (tDescription.Length > 30)
                    tDescription = tDescription.Substring(0, 29);

                return tDescription;
            }
            set
            {
                if (tDescription != value)
                {
                    value ??= string.Empty;
                    if (value.Length > 30)
                        value = value.Substring(0, 29);

                    tDescription = value;
                }
            }
        }

        public int? VendorId
        {
            get
            {
                return tVendorId;
            }
            set
            {
                if (tVendorId != value)
                {
                    UpdateVendor(value);
                }
            }
        }

        public void UpdateVendor(int? value)
        {
            if (Vendor == null && tVendorId != null)
            {
                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == tVendorId);
                if (vendor != null)
                {
                    VendorGroupId = vendor.VendorGroupId;
                    tVendorId = vendor.VendorId;

                    Vendor = vendor;
                }
            }

            if (tVendorId != value)
            {
                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == value);
                if (vendor != null)
                {
                    VendorGroupId = vendor.VendorGroupId;
                    tVendorId = vendor.VendorId;

                    Vendor = vendor;
                }
                else
                {
                    VendorGroupId = null;
                    tVendorId = null;
                    Vendor = null;
                }
                if (PurchaseOrder != null)
                {
                    if (PurchaseOrder?.VendorId != tVendorId)
                    {
                        PO = null;
                    }
                }
            }
        }

        public string PO
        {
            get
            {
                return tPO;
            }
            set
            {
                if (tPO != value)
                {
                    UpdatePurchaseOrder(value);
                }
            }
        }

        public void UpdatePurchaseOrder(string value)
        {
            if (PurchaseOrder == null && tPO != null)
            {
                var purchaseOrder = db.PurchaseOrders.FirstOrDefault(f => f.PO == tPO);
                if (purchaseOrder != null)
                {
                    POCo = purchaseOrder.POCo;
                    tPO = purchaseOrder.PO;
                    PurchaseOrder = purchaseOrder;
                }
                else
                {
                    POCo = null;
                    tPO = null;
                    PurchaseOrder = null;
                }
            }
            if (tPO != value)
            {
                var purchaseOrder = db.PurchaseOrders.FirstOrDefault(f => f.PO == value);
                if (purchaseOrder != null)
                {
                    POCo = purchaseOrder.POCo;
                    tPO = purchaseOrder.PO;
                    PurchaseOrder = purchaseOrder;

                    InvTotal = purchaseOrder.Items.Sum(sum => (sum.OrigCost + sum.OrigTax));
                    Description = purchaseOrder.Description;

                    VendorGroupId = purchaseOrder.VendorGroupId;
                    tVendorId = purchaseOrder.VendorId;
                    Vendor = purchaseOrder.Vendor;
                }
                else
                {
                    POCo = null;
                    tPO = null;
                    PurchaseOrder = null;

                }

                //model.Amount = updObj.InvTotal;
                //model.Description = updObj.Description;
                //model.VendorId = updObj.VendorId;

            }

        }

        public APDocumentLine AddLine(PurchaseOrderItem poItem)
        {

            var division = db.CompanyDivisions.OrderBy(o => o.DivisionId).FirstOrDefault(f => f.HQCompany.GLCo == poItem.GLCo);
            var line = new APDocumentLine
            {
                Document = this,
                PurchaseOrder = poItem.PurchaseOrder,
                POItem = poItem,

                APCo = APCo,
                DocId = DocId,
                SeqId = 1,
                LineId = (short)(Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1),
                LineTypeId = (byte)APLineTypeEnum.PO,
                UM = poItem.UM,
                Description = poItem.Description,
                MatlGroup = poItem.MatlGroupId,
                GLCo = poItem.GLCo,
                VendorGroupId = poItem.PurchaseOrder.VendorGroupId,
                UnitCost = poItem.OrigUnitCost,
                Units = poItem.OrigUnits,
                GrossAmt = poItem.OrigCost,
                TaxBasis = poItem.OrigCost,
                TaxGroupId = poItem.TaxGroup,
                TaxCodeId = poItem.TaxCodeId,
                TaxTypeId = poItem.TaxTypeId,
                TaxAmt = poItem.OrigTax,
                PayType = 1,
                MiscAmt = 0,
            };

            line.POCo = POCo;
            line.PO = PO;
            line.POItemId = poItem.POItemId;
            line.POItemTypeId = poItem.ItemTypeId;

            //var poItemLine = db.vPOItemLines.FirstOrDefault(f => f.POITKeyID == poItem.KeyID);
            //if (poItemLine != null)
            //{
            //    line.POItemLine = poItemLine.POItemLine;
            //}


            switch ((POItemTypeEnum)poItem.ItemTypeId)
            {
                case POItemTypeEnum.Job:
                    line.JCCo = poItem.JCCo;
                    line.JobId = poItem.JobId;
                    line.PhaseGroupId = poItem.PhaseGroupId;
                    line.PhaseId = poItem.PhaseId;
                    line.JCCType = poItem.JCCType;
                    line.Job = poItem.Job;
                    line.JobPhase = poItem.JobPhase;
                    line.JobPhaseCost = poItem.JobPhase.AddCostType((byte)poItem.JCCType);

                    line.DivisionId = poItem.Job.Division.WPDivision.DivisionId;
                    line.GLCo = poItem.Job.JCCompanyParm.GLCo;

                    break;
                case POItemTypeEnum.Expense:
                    line.GLCo = poItem.GLCo;
                    line.DivisionId = division.DivisionId;
                    break;
                case POItemTypeEnum.Equipment:

                    line.EMCo = poItem.EMCo;
                    line.EquipmentId = poItem.EquipmentId;
                    line.CostCodeId = poItem.CostCodeId;
                    line.EMCType = poItem.EMCType;
                    line.EMGroupId = poItem.EMGroupId;
                    line.DivisionId = poItem.Equipment.DivisionId;
                    line.GLCo = poItem.Equipment.EMCompanyParm.GLCo;

                    break;
                default:
                    break;
            }

            line.GLAcct = poItem.GLAcct;

            Lines.Add(line);

            return line;
        }

        public APDocumentLine AddLine()
        {
            var line = new APDocumentLine
            {
                Document = this,
                APCo = APCo,
                DocId = DocId,
                SeqId = 1,
                LineId = (short)(Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1),
                LineTypeId = (byte)APLineTypeEnum.Job,

                GLCo = APCompanyParm.GLCo,
                VendorGroupId = VendorGroupId,
                UM = "LS",
                Description = Description,
                MatlGroup = APCompanyParm.HQCompanyParm.MatlGroupId,
                PayType = 1,
                MiscAmt = 0,

                GrossAmt = InvTotal - Lines.DefaultIfEmpty().Sum(sum => sum == null ? 0 : sum.GrossAmt),
                tDivisionId = DivisionId,
                Division = Division,
            };
            if (PO != null)
            {
                line.POCo = POCo;
                line.PO = PO;
                line.LineTypeId = (byte)APLineTypeEnum.PO;
            }
            Lines.Add(line);
            return line;
        }
    }
}
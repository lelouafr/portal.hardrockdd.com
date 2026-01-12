using System;
using System.Linq;


namespace portal.Code.Data.VP
{
    public partial class CreditTransaction: IForum, IWorkFlow
    {
        public string BaseTableName { get { return "budCMTH"; } }

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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        private DateTime? _TransDateShort;
        public DateTime TransDateShort 
        { 
            get
            {
                if(_TransDateShort == null)
                {
                    _TransDateShort = new DateTime(TransDate.Year, TransDate.Month,TransDate.Day,TransDate.Hour,TransDate.Minute,0);
                }
                return (DateTime)_TransDateShort;
            }
        }


        public DB.CMTranStatusEnum Status
        {
            get
            {
                return (DB.CMTranStatusEnum)StatusId;
            }
            set
            {
                StatusId = (byte)value;
            }
        }

        public int StatusId
        {
            get
            {
                return this.TransStatusId ?? 0;
            }
            set
            {
                if (value != TransStatusId)
                {
                    TransStatusId = value;
                    this.WorkFlow.AddSequance(value);
                    this.AddWorkFlowAssignments(true);
                }
            }
        }

        public APBatchHeader AddToBatch(Batch batch = null)
        {
            if (batch == null)
            { 
                var comp = StaticFunctions.GetCurrentCompany();
                var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
                var mth = this.Mth;
                batch = Batch.FindCreate(company, "APHB", "AP Entry", mth);
                batch.InUseBy = "WebPortalUser";
            }
            APVendor dftVendor = null;
            var parms = batch.Company.APCompanyParm;
            switch (Source)
            {
                case "Wex":
                    dftVendor = this.Merchant.Vendor;
                    if (dftVendor == null)
                        dftVendor = db.APVendors.FirstOrDefault(f => f.VendorGroupId == batch.Company.VendorGroupId && f.VendorId == 834);
                    break;
                case "Zion":
                    dftVendor = this.Merchant.Vendor;
                    dftVendor = db.APVendors.FirstOrDefault(f => f.VendorGroupId == batch.Company.VendorGroupId && f.VendorId == parms.udDftMerchantVendorId);
                    break;
                default:
                    break;
            }
            if (dftVendor == null)
             dftVendor = db.APVendors.FirstOrDefault(f => f.VendorGroupId == batch.Company.VendorGroupId && f.VendorId == parms.udDftMerchantVendorId);

            var existingAPTran = db.APTrans.FirstOrDefault(f => f.APCo == batch.Co && f.CCTransId == this.TransId);
            if (existingAPTran == null)
            {

                var batchSeq = new APBatchHeader
                {
                    APCo = batch.Co,
                    Mth = batch.Mth,
                    BatchId = batch.BatchId,
                    BatchSeq = batch.APBatches.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq) + 1,
                    BatchTransType = "A",
                    VendorGroupId = Merchant.Vendor.VendorGroupId,
                    VendorId = Merchant.Vendor.VendorId,
                    Description = NewDescription ?? OrigDescription,
                    APRef = APRef,
                    InvDate = TransDate,
                    DueDate = TransDate,
                    InvTotal = TransAmt,
                    PayMethod = Merchant.Vendor.PayMethod,
                    CMCo = parms.CMCo,
                    CMAcct = dftVendor.CMAcct,
                    PrePaidYN = "N",
                    PrePaidProcYN = "N",
                    PayOverrideYN = "N",
                    V1099YN = Merchant.Vendor.V1099YN,
                    SeparatePayYN = "N",
                    ChkRev = "N",
                    PaidYN = "N",
                    CCTransId = TransId,
                    MerchantId = MerchantId,
                    UniqueAttchID = UniqueAttchID ?? Guid.NewGuid(),

                    Batch = batch,
                    Company = batch.Company,
                };
                if (batchSeq.Description?.Length > 30)
                {
                    batchSeq.Description = batchSeq.Description.Substring(0, 29);
                }
                if (batchSeq.V1099YN == "Y")
                {
                    batchSeq.V1099Type = Merchant.Vendor.V1099Type;
                    batchSeq.V1099Box = Merchant.Vendor.V1099Box;
                }

                Coding.ToList().ForEach(line => batchSeq.AddLine(line));
                LinkedImages.ToList().ForEach(link => {
                    var createDuplicate = link.Image.Transactions.Count > 1;
                    batchSeq.Attachment.AddFile(link.File(), createDuplicate);
                });

                batch.APBatches.Add(batchSeq);
                return batchSeq;
            }
            else
            {
                var batchSeq = existingAPTran.ToBatch(batch);
                Coding.ToList().ForEach(line => batchSeq.AddLine(line));
                LinkedImages.ToList().ForEach(link => {
                    var createDuplicate = link.Image.Transactions.Count > 1;
                    batchSeq.Attachment.AddFile(link.File(), createDuplicate);
                });
                batchSeq.InvTotal = batchSeq.Lines.Sum(sum => sum.GrossAmt);
                batch.APBatches.Add(batchSeq);
                return batchSeq;
            }
        }

        public void LinkImageToTrans(CreditCardImage image)
        {
            var link = image.Transactions.FirstOrDefault(f => f.TransId == this.TransId);
            if (link == null)
            {
                link = new CreditImageLink()
                {
                    CCCo = image.CCCo,
                    EmployeeId = image.EmployeeId,
                    Mth = image.Mth,
                    ImageId = image.ImageId,
                    LinkId = image.Transactions.DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId) + 1,
                    TransId = this.TransId,
                    TaggedDate = DateTime.Now
                };
                
                image.Transactions.Add(link);
                this.PictureStatusId = (int)DB.CMPictureStatusEnum.Attached;
                this.Logs.Add(Repository.VP.AP.CreditCard.CreditCardTransactionLogRepository.Init(this, DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction picture was added {0}", DB.CMPictureStatusEnum.Attached)));
            }

        }
        
        public void FixTransActionErrors()
        {
            if (this.APRef == null)
                this.APRef = this.UniqueTransId.Remove(0, 8);

            if (this.CodedStatusId == (int)DB.CMTransCodeStatusEnum.AutoCoded && this.TransStatusId == (int)DB.CMTranStatusEnum.Open)
                this.AutoCode();



        }

        public CreditTransactionLine AddCreditTransactionLine(CCWexImport importLine)
        {
            var line = Lines.FirstOrDefault(f => f.UniqueTransId == importLine.CalcLineUniqueTransId);//f.ProductCode == importLine.Product && 
            if (line == null)
            {
                line = new CreditTransactionLine()
                {
                    CCCo = CCCo,
                    TransId = TransId,
                    SeqId = Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    LineAmount = importLine.NetCost,
                    OrigDescription = importLine.ProductDescription,
                    ProductCode = importLine.Product,
                    Quantity = importLine.Units ?? 1,
                    UnitCost = importLine.UnitCost ?? 0,
                    UM = importLine.UnitofMeasure,
                    UniqueTransId = importLine.CalcLineUniqueTransId,

                    Transaction = this,
                };
                if (line.Quantity == 0)
                    line.Quantity = 1;
                if (line.UnitCost * line.Quantity != line.LineAmount)
                {
                    line.UnitCost = line.LineAmount / line.Quantity;
                }
                Lines.Add(line);
            }
            return line;

        }

        public CreditTransactionCode AddTransactionCode(CCWexImport importLine)
        {
            var line = Coding.FirstOrDefault(f => f.UniqueTransId == importLine.CalcLineUniqueTransId);
            if (line == null)
            {
                var mtlCat = Import.HQCompanyParm.MatlGroup.AddMaterialCategory("WEX", "WEX Products");
                var mtl = mtlCat.AddMaterial(importLine.Product, importLine.ProductDescription, importLine.UnitofMeasure, "E");
                line = new CreditTransactionCode()
                {
                    CCCo = CCCo,
                    TransId = TransId,
                    SeqId = Coding.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    EMCo = importLine.EMCo,
                    Description = importLine.ProductDescription,
                    Units = (importLine.Units ?? 0) == 0 ? 1 : importLine.Units,
                    UnitCost = importLine.UnitCost ?? 0,
                    Cost = importLine.NetCost,
                    GrossAmt = importLine.NetCost,
                    UM = importLine.UnitofMeasure,
                    Material = mtl.MaterialId,
                    Transaction = this,
                    tLineTypeId = (byte)DB.CMCodeLineTypeEnum.Equipment,
                    tEquipmentId = importLine.EMEquipmentId,
                    Equipment = importLine.EMEquipment,
                    CostCodeId = mtl.EMCostCode,
                    EMCType = mtl.EMCostType,
                    UniqueTransId = importLine.CalcLineUniqueTransId
                };
                if (line.UnitCost * line.Units != line.Cost)
                {
                    line.UnitCost = line.Cost / line.Units;
                }
                line.UpdateGLAccountInfo(line.tGLAcct);
                AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded"));
                Coding.Add(line);
            }
            return line;

        }

        public CreditTransactionLine AddCreditTransactionLine(ZionImport importLine)
        {
            if (!string.IsNullOrEmpty(importLine.TransactionLineItemProductCode))
            {
                var line = Lines.FirstOrDefault(f => f.ProductCode == importLine.TransactionLineItemProductCode);
                if (line == null)
                {
                    line = new CreditTransactionLine()
                    {
                        CCCo = CCCo,
                        TransId = TransId,
                        SeqId = Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                        LineAmount = importLine.TransactionLineItemTotal,
                        OrigDescription = importLine.TransactionLineItemDescription,
                        ProductCode = importLine.TransactionLineItemProductCode,
                        Quantity = importLine.TransactionLineItemQuantity ?? 1,
                        UnitCost = importLine.TransactionLineItemUnitPrice ?? 0,
                        UM = importLine.TransactionLineItemUnitofMeasure,
                        UniqueTransId = importLine.TransactionReference,

                        Transaction = this,

                    };
                }
                return line;
            }
            return null;

        }

        public  CreditTransactionLog AddLog(DB.CMLogEnum logType, string description = "")
        {
           
            var result = new CreditTransactionLog
            {
                CCCo = CCCo,
                TransId = TransId,
                AuditId = Logs.DefaultIfEmpty().Max(f => f == null ? 0 : f.AuditId) + 1,
                AuditTypeId = (byte)logType,
                LogBy = StaticFunctions.GetUserId(),
                LogDate = DateTime.Now,
                Description = description
            };

            Logs.Add(result);
            return result;
        }

        public void AddDefaultTransactionCode()
        {
            if (!Coding.Any())
            {
                var defaultLineTypeId = this.DefaultLineTypeId();
                var defaultGL = this.DefaultGLAcct();
                var defaultPhaseId = this.DefaultJCPhaseId();
                var defaultJobCostTypeId = this.DefaultJCCType();
                var defaultEMCostCodeId = this.DefaultEMCostCodeId();
                var defaultEMCType = this.DefaultEMCType();
                var defaultJob = Employee.CurrentJob(TransDate.Date);

                var coding = new CreditTransactionCode
                {
                    CCCo = CCCo,
                    TransId = TransId,
                    SeqId = 1,
                    tLineTypeId = defaultLineTypeId,
                    JCCo = CCCo,
                    tJobId = null,
                    PhaseGroupId = CCCo,
                    tPhaseId = defaultPhaseId,
                    tJCCType = defaultJobCostTypeId,
                    EMCo = CCCo,
                    tEquipmentId = null,
                    EMGroupId = CCCo,
                    tCostCodeId = defaultEMCostCodeId,
                    tEMCType = defaultEMCType,
                    GLCo = CCCo,
                    tGLAcct = defaultGL,
                    GrossAmt = TransAmt,

                    Transaction = this,
                };
                Coding.Add(coding);
            }
        }

        public void AutoCode()
        {
            if (CodedStatusId == (int)DB.CMTransCodeStatusEnum.Empty || CodedStatusId == (int)DB.CMTransCodeStatusEnum.AutoCoded)
            {
                var defaultLineTypeId = this.DefaultLineTypeId();
                var defaultGL = this.DefaultGLAcct();
                var defaultPhaseId = this.DefaultJCPhaseId();
                var defaultJobCostTypeId = this.DefaultJCCType();
                var defaultEMCostCodeId = this.DefaultEMCostCodeId();
                var defaultEMCType = this.DefaultEMCType();
                var defaultJob = Employee.CurrentJob(TransDate.Date);

                AddDefaultTransactionCode();


                foreach (var coding in Coding)
                {
                    if (coding.tLineTypeId != defaultLineTypeId ||
                        coding.tPhaseId != defaultPhaseId ||
                        coding.tJCCType != defaultJobCostTypeId ||
                        coding.tCostCodeId != defaultEMCostCodeId ||
                        coding.tEMCType != defaultEMCType ||
                        coding.tGLAcct != defaultGL ||
                        CodedStatusId == 0)
                    {
                        if (defaultLineTypeId == null)
                        {
                            //coding.tCostCodeId = defaultEMCostCodeId;
                            //coding.tEMCType = defaultEMCType;
                            coding.tGLAcct = defaultGL;

                            if (defaultPhaseId != null && defaultJobCostTypeId != null && defaultJob != null)
                            {
                                coding.LineType = DB.CMCodeLineTypeEnum.Job;
                                coding.JobId = defaultJob.JobId;
                                coding.JCCo = defaultJob.JCCo;
                                coding.PhaseId = defaultPhaseId;
                                coding.JCCType = defaultJobCostTypeId;
                                coding.UpdateGLAccountInfo(coding.tGLAcct);
                                CodedStatusId = (int)DB.CMTransCodeStatusEnum.AutoCoded;
                                AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded as Job"));
                            }
                            else if (coding.tGLAcct != null)
                            {
                                coding.LineType = DB.CMCodeLineTypeEnum.Expense;
                                coding.UpdateLineType(coding.LineType);
                                CodedStatusId = (int)DB.CMTransCodeStatusEnum.AutoCoded;
                                AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded as Expense"));
                            }
                        }
                        else
                        {
                            coding.LineTypeId = defaultLineTypeId;
                            switch ((DB.CMCodeLineTypeEnum)defaultLineTypeId)
                            {
                                case DB.CMCodeLineTypeEnum.Job:
                                    coding.tPhaseId = defaultPhaseId;
                                    coding.tJCCType = defaultJobCostTypeId;
                                    CodedStatusId = (int)DB.CMTransCodeStatusEnum.AutoCoded;
                                    AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded"));
                                    break;
                                case DB.CMCodeLineTypeEnum.Equipment:
                                    coding.tCostCodeId = defaultEMCostCodeId;
                                    coding.tEMCType = defaultEMCType;
                                    CodedStatusId = (int)DB.CMTransCodeStatusEnum.AutoCoded;
                                    AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded"));
                                    break;
                                case DB.CMCodeLineTypeEnum.Expense:
                                    coding.tGLAcct = defaultGL;
                                    CodedStatusId = (int)DB.CMTransCodeStatusEnum.AutoCoded;
                                    AddLog(DB.CMLogEnum.AutoCoded, string.Format(AppCultureInfo.CInfo(), "Transaction was autocoded"));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static int GetNextTransId(int blockOutCnt)
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

            var result = db.udNextId("budCMTH", blockOutCnt, outParm);

            return (int)outParm.Value;

            //return db.GetNextId()
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
                    Co = this.CCCo,
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
        
        public WorkFlow GetWorkFlow()
        {
            var workFlow = WorkFlow;
            if (workFlow == null)
            {
                workFlow = AddWorkFlow();
                
            }
            if (workFlow.CurrentSequance() == null)
            {
                workFlow.AddSequance(this.StatusId);
                this.AddWorkFlowAssignments();
                db.BulkSaveChanges();
            }
            return workFlow;
        }

        public WorkFlow AddWorkFlow()
        {
            var workFlow = this.WorkFlow;
            if (WorkFlow == null)
            {
                var company = db.HQCompanyParms.First(f => f.HQCo == CCCo);
                workFlow = new WorkFlow
                {
                    WFCo = CCCo,
                    WorkFlowId = db.GetNextId(WorkFlow.BaseTableName, 1),
                    TableName = BaseTableName,
                    Id = (int)TransId,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,
                    
                    db = this.db,
                    Company = company,
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
                AddWorkFlow();
            if (reset)
            {
                var delList = WorkFlow.CurrentSequance().AssignedUsers.ToList();
                delList.ForEach(del => WorkFlow.CurrentSequance().AssignedUsers.Remove(del));
            }

            switch (Status)
            {
                case DB.CMTranStatusEnum.Open:
                    WorkFlow.AddEmployee(this.Employee);
                    WorkFlow.AddUsersByPosition("FIN-APMGR");

                    break;
                case DB.CMTranStatusEnum.Locked:
                    WorkFlow.CompleteSequance();
                    break;
                case DB.CMTranStatusEnum.APPosted:
                    WorkFlow.CompleteSequance();
                    break;
                case DB.CMTranStatusEnum.RequestedInfomation:
                    WorkFlow.AddEmployee(this.Employee);
                    WorkFlow.AddUsersByPosition("FIN-APMGR");
                    break;
                default:
                    break;
            }
        }
    }



    public static class CreditTransactionExtension
    {
        public static bool IsReceiptRequired(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if ((transaction.Merchant.Vendor.IsReceiptRequired ?? true) == false)
            {
                return false;
            }
            if ((transaction.Merchant.IsReceiptRequired ?? true) == false)
            {
                return false;
            }
            return true;
        }

        public static byte? DefaultLineTypeId(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.Merchant.DefaultLineTypeId != null)
            {
                return transaction.Merchant.DefaultLineTypeId;
            }
            else if (transaction.Merchant.Vendor.DefaultLineTypeId != null)
            {
                return transaction.Merchant.Vendor.DefaultLineTypeId;
            }
            else if (transaction.Merchant.Category.DefaultLineTypeId != null)
            {
                return transaction.Merchant.Category.DefaultLineTypeId;
            }
            else if (transaction.Merchant.Category.Group.DefaultLineTypeId != null)
            {
                return transaction.Merchant.Category.Group.DefaultLineTypeId;
            }
            return null;
        }

        public static string DefaultGLAcct(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            if (transaction.Merchant.DefaultGLAcct != null)
            {
                return transaction.Merchant.DefaultGLAcct;
            }
            else if (transaction.Merchant.Vendor.DefaultGLAcct != null)
            {
                return transaction.Merchant.Vendor.DefaultGLAcct;
            }
            else if (transaction.Merchant.Category.DefaultGLAcct != null)
            {
                return transaction.Merchant.Category.DefaultGLAcct;
            }
            else if (transaction.Merchant.Category.Group.DefaultGLAcct != null)
            {
                return transaction.Merchant.Category.Group.DefaultGLAcct;
            }
            return null;
        }

        public static string DefaultJCPhaseId(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.Merchant.DefaultJCPhaseId != null)
            {
                return transaction.Merchant.DefaultJCPhaseId;
            }
            else if (transaction.Merchant.Vendor.DefaultJCPhaseId != null)
            {
                return transaction.Merchant.Vendor.DefaultJCPhaseId;
            }
            else if (transaction.Merchant.Category.DefaultJCPhaseId != null)
            {
                return transaction.Merchant.Category.DefaultJCPhaseId;
            }
            else if (transaction.Merchant.Category.Group.DefaultJCPhaseId != null)
            {
                return transaction.Merchant.Category.Group.DefaultJCPhaseId;
            }
            return null;
        }

        public static byte? DefaultJCCType(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.Merchant.DefaultJCCType != null)
            {
                return transaction.Merchant.DefaultJCCType;
            }
            else if (transaction.Merchant.Vendor.DefaultJCCType != null)
            {
                return transaction.Merchant.Vendor.DefaultJCCType;
            }
            else if (transaction.Merchant.Category.DefaultJCCType != null)
            {
                return transaction.Merchant.Category.DefaultJCCType;
            }
            else if (transaction.Merchant.Category.Group.DefaultJCCType != null)
            {
                return transaction.Merchant.Category.Group.DefaultJCCType;
            }
            return null;
        }

        public static string DefaultEMCostCodeId(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.Merchant.DefaultEMCostCodeId != null)
            {
                return transaction.Merchant.DefaultEMCostCodeId;
            }
            else if (transaction.Merchant.Vendor.DefaultEMCostCodeId != null)
            {
                return transaction.Merchant.Vendor.DefaultEMCostCodeId;
            }
            else if (transaction.Merchant.Category.DefaultEMCostCodeId != null)
            {
                return transaction.Merchant.Category.DefaultEMCostCodeId;
            }
            else if (transaction.Merchant.Category.Group.DefaultEMCostCodeId != null)
            {
                return transaction.Merchant.Category.Group.DefaultEMCostCodeId;
            }
            return null;
        }

        public static byte? DefaultEMCType(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.Merchant.DefaultEMCType != null)
            {
                return transaction.Merchant.DefaultEMCType;
            }
            else if (transaction.Merchant.Vendor.DefaultEMCType != null)
            {
                return transaction.Merchant.Vendor.DefaultEMCType;
            }
            else if (transaction.Merchant.Category.DefaultEMCType != null)
            {
                return transaction.Merchant.Category.DefaultEMCType;
            }
            else if (transaction.Merchant.Category.Group.DefaultEMCType != null)
            {
                return transaction.Merchant.Category.Group.DefaultEMCType;
            }

            return null;
        }

        public static void AutoPictureStatus(this CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            if (transaction.PictureStatusId == (int)DB.CMPictureStatusEnum.Attached)
            {

            }
            else if (!transaction.IsReceiptRequired())
            {
                if (transaction.PictureStatusId != (int)DB.CMPictureStatusEnum.NotNeeded)
                {
                    transaction.PictureStatusId = (int)DB.CMPictureStatusEnum.NotNeeded;
                    transaction.AddLog(DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction was set to picture not needed {0}", DB.CMPictureStatusEnum.NotNeeded));
                }
            }
            else if (transaction.PictureStatusId == (int)DB.CMPictureStatusEnum.NotNeeded)
            {
                transaction.PictureStatusId = (int)DB.CMPictureStatusEnum.Empty;
                transaction.AddLog(DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction was set to picture needed"));
            }
        }

      

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PORequest
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.POCompanyParm != null)
                    db = VPEntities.GetDbContextFromEntity(this.POCompanyParm);

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public string BaseTableName { get { return "budPORH"; } }

        private HQAttachment _Attachment;

        public HQAttachment Attachment
        {
            get
            {
                if (_Attachment == null)
                {                    
                    _Attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == this.UniqueAttchID);

                    if (_Attachment == null)
                    {
                        _Attachment = HQAttachment.Init(this.POCo, this.BaseTableName, this.KeyID);
                        db.HQAttachments.Add(_Attachment);
                        this.UniqueAttchID = _Attachment.UniqueAttchID;
                        db.BulkSaveChanges();
                    }

                    _Attachment.BuildDefaultFolders();
                    db.BulkSaveChanges();
                }

                return _Attachment;
            }
        }

        public DB.PORequestStatusEnum Status
        {
            get
            {
                return (DB.PORequestStatusEnum)StatusId;
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

        public bool IsPONew { get; set; }

        public string StatusComments { get; set; }

        public void UpdateStatus(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();
            tStatusId = newValue;
            
            var status = (DB.PORequestStatusEnum)newValue;

            GenerateStatusLog(StatusComments);
            switch (status)
            {
                case DB.PORequestStatusEnum.Open:
                    SubmittedBy = null;
                    SubmittedOn = null;
                    ApprovedBy = null;
                    ApprovedOn = null;
                    if (WorkFlow.CurrentSequance()?.Status != newValue)
                    {
                        WorkFlow.CreateSequance(newValue);
                        GenerateWorkFlowAssignments();
                    }
                    break;
                case DB.PORequestStatusEnum.Submitted:
                    SubmittedBy = StaticFunctions.GetUserId();
                    SubmittedOn = DateTime.Now;
                    ApprovedBy = null;
                    ApprovedOn = null;
                    WorkFlow.CreateSequance(newValue);
                    GenerateWorkFlowAssignments();
                    GetPO();                    
                    EmailStatusUpdate();
                    break;
                case DB.PORequestStatusEnum.Approved:
                    GetPO();
                    if (PO != null)
                    {
                        ApprovedBy = StaticFunctions.GetUserId();
                        ApprovedOn = DateTime.Now;
                        WorkFlow.CreateSequance(newValue);
                        GenerateWorkFlowAssignments();
                        EmailStatusUpdate();
                    }
                    else
                    {
                        Status = DB.PORequestStatusEnum.SupApproved;
                    }
                    break;
                case DB.PORequestStatusEnum.SupApproved:
                    WorkFlow.CreateSequance(newValue);
                    GenerateWorkFlowAssignments();
                    break;
                case DB.PORequestStatusEnum.Rejected:
                    WorkFlow.CreateSequance(newValue);
                    GenerateWorkFlowAssignments();
                    EmailStatusUpdate();
                    break;
                case DB.PORequestStatusEnum.Processed:
                    GetPO(true);
                    if (SubmittedBy == null)
                    {
                        SubmittedBy = StaticFunctions.GetUserId();
                        SubmittedOn = DateTime.Now;
                    }
                    if (ApprovedBy == null)
                    {
                        ApprovedBy = StaticFunctions.GetUserId();
                        ApprovedOn = DateTime.Now;
                    }
                    ProcessedBy = StaticFunctions.GetUserId();
                    ProcessedOn = DateTime.Now;
                    WorkFlow.CreateSequance(newValue);
                    GenerateWorkFlowAssignments();
                    //Create
                    break;
                case DB.PORequestStatusEnum.Canceled:
                    SubmittedBy = null;
                    SubmittedOn = null;
                    ApprovedBy = null;
                    ApprovedOn = null;
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.PORequestStatusEnum.Reviewed:
                    break;
                case DB.PORequestStatusEnum.Change:
                    break;
                default:
                    break;
            }

        }

        public void GetPO(bool noCheck = false)
        {
            if (PO == null)
            {
                if (noCheck)
                {
                    using var db = new VPEntities();
                    var poParm = new System.Data.Entity.Core.Objects.ObjectParameter("po", typeof(string));
                    var poErr = db.bspPOHDNextPO(POCo, poParm);
                    if (poErr == -1)
                    {
                        PO = (string)poParm.Value;
                        EmailStatusPOUpdate();
                        IsPONew = true;
                    }
                }
                else
                {
                    var emp = StaticFunctions.GetCurrentHREmployee();
                    var employee = db.HRResources.FirstOrDefault(f => f.HRCo == emp.HRCo && f.HRRef == emp.HRRef);
                    var currentUser = HttpContext.Current.User;
                    if (employee.Position.POLimit >= POAmount ||
                            currentUser.IsInRole("POProcess") ||
                            Vendor.udVendorType == "XXXSUBCONTRACTOR")
                    {
                        using var db = new VPEntities();
                        var poParm = new System.Data.Entity.Core.Objects.ObjectParameter("po", typeof(string));
                        var poErr = db.bspPOHDNextPO(POCo, poParm);
                        if (poErr == -1)
                        {
                            PO = (string)poParm.Value;
                            EmailStatusPOUpdate();
                            IsPONew = true;
                        }
                    }
                }
            }


        }

        private decimal POAmount
        {
            get
            {
                return Lines.Sum(s => s.Cost) ?? 0;
            }
        }

        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = POCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(POCo),
                    TableName = "budPORH",
                    Id = RequestId,
                    CreatedBy = StaticFunctions.GetUserId(),
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = POCompanyParm.HQCompanyParm,
                };
                POCompanyParm.HQCompanyParm.WorkFlows.Add(workflow);
                WorkFlow = workflow;
                WorkFlowId = workflow.WorkFlowId;
            }
        }

        public void GenerateWorkFlowAssignments()
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var employee = CreatedUser.PREmployee;
            switch (Status)
            {
                case DB.PORequestStatusEnum.Open:
                    WorkFlow.AddUser(this.CreatedUser);
                    break;
                case DB.PORequestStatusEnum.Submitted:
                    WorkFlow.AddEmployee(employee.Supervisor);
                    WorkFlow.AddEmployee(employee.Supervisor?.Supervisor);
                    WorkFlow.AddByEmail("TJ.Wheeler@hardrockdd.com");
                    break;
                case DB.PORequestStatusEnum.SupApproved:
                    var emp = StaticFunctions.GetCurrentEmployee();
                    var currentEmployee = db.Employees.FirstOrDefault(f => f.PRCo == emp.PRCo && f.EmployeeId == emp.EmployeeId);
                    WorkFlow.AddEmployee(currentEmployee.Supervisor);
                    WorkFlow.AddByEmail("TJ.Wheeler@hardrockdd.com");
                    break;
                case DB.PORequestStatusEnum.Approved:
                    WorkFlow.AddByEmail("trudy.moore@hardrockdd.com");
                    break;
                case DB.PORequestStatusEnum.Rejected:
                    WorkFlow.AddUser(this.CreatedUser);
                    WorkFlow.AddUser(this.ApprovedUser);
                    break;
                case DB.PORequestStatusEnum.Processed:
                    break;
                case DB.PORequestStatusEnum.Canceled:
                    break;
                default:
                    break;
            }
        }
              
        public Employee GetNextApprover(Employee currentApprover)
        {
            if (currentApprover.HREmployee.Position.POLimit >= POAmount)
            {
                return currentApprover;
            }
            else
            {
                return GetNextApprover(currentApprover.Supervisor);
            }
        }

        public void GenerateStatusLog(string comments)
        {
            var log = new PORequestStatusLog
            {
                POCo = POCo,
                RequestId = RequestId,
                LineNum = StatusLogs.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                Status = (short)(tStatusId ?? 0),
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId(),
                Comments = comments,
                PORequest = this,

            };

            StatusLogs.Add(log);
        }

        public void EmailStatusUpdate()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new Models.Views.Purchase.Request.PORequstFormViewModel(this);
            var viewPath = "../PO/Request/Email/EmailStatusChange"; 
            var subject = "";
            //using var db = new VPEntities();

            subject = string.Format(AppCultureInfo.CInfo(), "PO Request {0} {1} By {2} {3}", PO, Status, OrderUser.FirstName, OrderUser.LastName);
            //viewPath = "../SM/Request/Email/EmailSubmit";
            

            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                    {
                        Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    {
                        msg.To.Add(new System.Net.Mail.MailAddress(workFlow.AssignedUser.Email));
                    }
                    if (CreatedUser.Email != StaticFunctions.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
                    }
                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void EmailStatusPOUpdate()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new Models.Views.Purchase.Request.PORequstFormViewModel(this);
            var viewPath = "../PO/Request/Email/EmailPOUpdate";
            var subject = "";
            using var db = new VPEntities();

            subject = string.Format(AppCultureInfo.CInfo(), "PO Request Approved, PO Number is {0}", PO);
            //viewPath = "../SM/Request/Email/EmailSubmit";


            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                    {
                        Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };
                    msg.To.Add(new System.Net.Mail.MailAddress(CreatedUser.Email));
                    //foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    //{
                    //    var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                    //    msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
                    //}
                    //if (CreatedUser.Email != StaticFunctions.GetCurrentUser().Email)
                    //{
                    //    msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
                    //}
                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public PORequestLine AddLine()
        {
            var result = new PORequestLine
            {
                Request = this,
                LineId = this.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                POCo = this.POCo,
                RequestId = this.RequestId,
                TransTypeId = "A",
                ItemTypeId = (byte)DB.POItemTypeEnum.Job,
                ReqDate = this.OrderedDate,
                JobId = this.JobId,
                UM = "LS",
                Description = this.Description
            };

            Lines.Add(result);

            return result;
        }

        public PORequestLine AddLine(PORequestLine line)
        {
            var result = new PORequestLine
            {
                Request = line.Request,
                POCo = line.POCo,
                LineId = line.Request.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                RequestId = line.RequestId,
                TransTypeId = line.TransTypeId,
                ItemTypeId = line.ItemTypeId,
                ReqDate = line.ReqDate,
                JobId = line.JobId,
                PhaseGroupId = line.PhaseGroupId ?? line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId,
                PhaseId = line.PhaseId,
                JCCType = line.JCCType,

                EquipmentId = line.EquipmentId,
                CostCodeId = line.CostCodeId,
                EMCType = line.EMCType
            };

            Lines.Add(result);

            return result;
        }

        public Batch AddToBatch(Batch batch)
        {
            Status = DB.PORequestStatusEnum.Processed;
            var mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
            if (batch == null)
                batch = Batch.FindCreate(POCompanyParm.HQCompanyParm, "POHB", "PO Entry", mth);

            batch.InUseBy = "WebPortalUser";

            var batchSeq = new POBatchHeader
            {
                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchSeq = batch.POBatchs.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq) + 1,
                BatchTransType = "A",
                PO = PO,
                VendorGroupId = VendorGroup ?? batch.Co,
                VendorId = VendorId,
                Description = Description,
                OrderDate = OrderedDate,
                OrderedBy = OrderedBy.ToString(AppCultureInfo.CInfo()),
                ExpDate = OrderedDate,
                Status = 0,
                JCCo = JCCo,
                JobId = JobId,
                PayTerms = Vendor.PayTerms,
                UniqueAttchID = UniqueAttchID
            };

            if (ApprovedUser != null)
                batchSeq.PortalApprover = ApprovedUser.PREmployee.EmployeeId;
            else if (ApprovedBy != null)
                batchSeq.PortalApprover = db.WebUsers.FirstOrDefault(f => f.Id == ApprovedBy).PREmployee.EmployeeId;

            batch.POBatchs.Add(batchSeq);

            Attachment.TableName = batchSeq.BaseTableName;           
            Attachment.Files.ToList().ForEach(e => {
                e.TableName = Attachment.TableName;
                e.FormName = "POEntry";
            });
            Lines.Where(f => f.GLAcct != null).ToList().ForEach(line => batchSeq.AddItem(line));

            BatchId = batch.BatchId;
            BatchSeq = batchSeq.BatchSeq;

            return batch;
        }
    }
    
    public partial class PORequestLine
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.Request != null)
                    db = this.Request.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public int? ItemTypeId
        {
            get
            {
                return tItemTypeId;
            }
            set
            {
                if (value != tItemTypeId)
                {
                    UpdateItemType((DB.POItemTypeEnum)(value ?? 0));
                }
            }
        }

        public DB.POItemTypeEnum ItemType
        {
            get
            {
                return (DB.POItemTypeEnum)(this.tItemTypeId ?? 0);
            }
            set
            {
                ItemTypeId = (int)value;
            }
        }

        public string JobId
        {
            get
            {
                return tJobId;
            }
            set
            {
                if (value != tJobId)
                {
                    UpdateJobInfo(value);
                }
            }
        }

        public string PhaseId
        {
            get
            {
                return tPhaseId;
            }
            set
            {
                if (value != tPhaseId)
                {
                    UpdateJobPhaseInfo(value);
                }
            }
        }

        public byte? JCCType
        {
            get
            {
                return tJCCType;
            }
            set
            {
                if (tJCCType != value)
                {
                    UpdateJobCostTypeInfo(value);
                }
            }

        }

        public string EquipmentId
        {
            get
            {
                return tEquipmentId;
            }
            set
            {
                if (value != tEquipmentId)
                {
                    UpdateEquipmentInfo(value);
                }
            }
        }

        public string CostCodeId
        {
            get
            {
                return tCostCodeId;
            }
            set
            {
                if (value != tCostCodeId)
                {
                    UpdateEquipmentCostCodeInfo(value);
                }
            }
        }

        public byte? EMCType
        {
            get
            {
                return tEMCType;
            }
            set
            {
                if (tEMCType != value)
                {
                    UpdateEquipmentCostTypeInfo(value);
                }
            }

        }

        public string GLAcct
        {
            get
            {
                return tGLAcct;
            }
            set
            {
                if (value != tGLAcct)
                {
                    UpdateGLAccountInfo(value);
                }
            }
        }

        public string TaxCodeId
        {
            get
            {
                return tTaxCodeId;
            }
            set
            {
                if (tTaxCodeId != value)
                {
                    UpdateTaxCode(value);
                }
            }
        }

        private void UpdateTaxCode(string value)
        {
            if (!string.IsNullOrEmpty(tTaxCodeId) && TaxCode == null )
            {
                var taxCode = db.TaxCodes.FirstOrDefault(f => f.TaxCodeId == TaxCodeId && f.TaxGroupId == Request.POCompanyParm.HQCompanyParm.TaxGroupId);
                if (taxCode != null)
                {
                    tTaxCodeId = taxCode.TaxCodeId;
                    TaxGroupId = taxCode.TaxGroupId;
                    TaxCode = taxCode;
                }
                else
                {
                    tTaxCodeId = null;
                    TaxGroupId = null;
                    TaxCode = null;
                }
            }
            if (value != tTaxCodeId)
            {
                var taxCode = db.TaxCodes.FirstOrDefault(f => f.TaxCodeId == value && f.TaxGroupId == Request.POCompanyParm.HQCompanyParm.TaxGroupId);
                if (taxCode != null)
                {
                    tTaxCodeId = taxCode.TaxCodeId;
                    TaxGroupId = taxCode.TaxGroupId;
                    TaxCode = taxCode;
                }
                else
                {
                    tTaxCodeId = null;
                    TaxGroupId = null;
                    TaxCode = null;
                }
            }
        }

        private void UpdateJobInfo(string newValue)
        {
            
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                Job = job;
            }

            if (tJobId != newValue)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == newValue);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                    CrewId = job.CrewId;
                    Job = job;
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    PhaseGroupId = null;
                    Job = null;
                }

                PhaseId = null;
                JCCType = null;
                GLCo = null;
                tGLAcct = null;
            }
        }

        private void UpdateJobPhaseInfo(string newValue)
        {
            if (Job == null || string.IsNullOrEmpty(newValue))
            {
                tPhaseId = null;
                JobPhase = null;
                return;
            }

            if (JobPhase == null && tPhaseId != null)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                PhaseGroupId = phase.PhaseGroupId;
                JCCo = phase.JCCo;
                JobPhase = phase;
            }

            if (tPhaseId != newValue)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == newValue);
                if (phase == null)
                    phase = Job.AddMasterPhase(newValue);

                if (phase != null)
                {
                    JCCo = phase.JCCo;
                    PhaseGroupId = phase.PhaseGroupId;
                    tPhaseId = phase.PhaseId;
                    JobPhase = phase;

                    if (phase.JobPhaseCosts.Count == 0)
                        phase = Job.AddMasterPhase(PhaseId, true);
                }
                else
                {
                    JCCo = null;
                    PhaseGroupId = null;
                    tPhaseId = null;
                    JobPhase = null;
                }
                JCCType = null;
                GLCo = null;
                tGLAcct = null;
            }
        }

        private void UpdateJobCostTypeInfo(byte? newValue)
        {
            if (JobPhase == null || newValue == null)
            {
                JobPhaseCost = null;
                tJCCType = null;
                return;
            }

            if (JobPhaseCost == null && tJCCType != null)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == tJCCType);
                JobPhaseCost = phaseCost;
            }

            if (tJCCType != newValue)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == newValue);
                if (phaseCost == null) //Phase Cost is missing from Job Phase Cost List *Add It*
                    phaseCost = JobPhase.AddCostType((byte)newValue);

                if (phaseCost != null)
                {
                    tJCCType = phaseCost.CostTypeId;
                    JobPhaseCost = phaseCost;
                    UpdateGLAccountInfo(GLAcct);
                }
            }
        }
        
        private void UpdateEquipmentInfo(string newValue)
        {
            if (Equipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                EMGroupId = equipment.EMCompanyParm.EMGroupId;
                tEquipmentId = equipment.EquipmentId;
                EMCo = equipment.EMCo;
                Equipment = equipment;
            }

            if (tEquipmentId != newValue)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == newValue);
                if (equipment != null)
                {
                    EMGroupId = equipment.EMCompanyParm.EMGroupId;
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    Equipment = equipment;
                }
                else
                {
                    tEquipmentId = null;
                    tEMCType = null;
                    EMGroupId = null;
                    EMCo = null;
                    Equipment = null;
                }
                tCostCodeId = null;
                tEMCType = null;
                GLCo = null;
                tGLAcct = null;
            }
        }

        private void UpdateEquipmentCostCodeInfo(string newValue)
        {
            if (Equipment == null || string.IsNullOrEmpty(newValue))
            {
                tCostCodeId = null;
                tEMCType = null;
                return;
            }

            if (tCostCodeId != newValue)
            {
                EMGroupId = Equipment.EMCompanyParm.EMGroupId;
                tCostCodeId = newValue;
                tEMCType = null;
                GLCo = null;
                tGLAcct = null;
            }
        }

        private void UpdateEquipmentCostTypeInfo(byte? newValue)
        {
            if (Equipment == null || newValue == null)
            {
                tEMCType = null;
                return;
            }

            if (tEMCType != newValue)
            {
                EMGroupId = Equipment.EMCompanyParm.EMGroupId;
                tEMCType = newValue;

                UpdateGLAccountInfo(GLAcct);
            }
        }
        
        private void UpdateItemType(DB.POItemTypeEnum newValue)
        {
            switch (newValue)
            {
                case DB.POItemTypeEnum.Expense:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    JCCType = null;
                    JCCo = null;
                    Job = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;

                    break;
                case DB.POItemTypeEnum.Equipment:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    JCCType = null;

                    Job = null;
                    break;
                case DB.POItemTypeEnum.Job:
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;
                    break;
                default:
                    break;
            }
            tGLAcct = null;
            GLCo = Request.POCompanyParm.HQCompanyParm.GLCo;

            tItemTypeId = (byte)newValue;
        }

        private void UpdateGLAccountInfo(string newValue)
        {
            switch (ItemType)
            {
                case DB.POItemTypeEnum.Job:
                    GetJCGLAccount(db);
                    break;
                case DB.POItemTypeEnum.Expense:
                    GLCo = Request.POCompanyParm.HQCompanyParm.GLCo;
                    tGLAcct = newValue;
                    break;
                case DB.POItemTypeEnum.Equipment:
                    GetEMGLAccount(db);
                    break;
                default:
                    break;
            }
        }

        private void GetEMGLAccount(VPEntities db)
        {
            var glParm = new System.Data.Entity.Core.Objects.ObjectParameter("gltransacct", typeof(string));
            var glORParm = new System.Data.Entity.Core.Objects.ObjectParameter("GLOverride", typeof(string));
            var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
            var glErr = db.bspEMGlacctDflt(
                EMCo, 
                Request.POCompanyParm.HQCompanyParm.EMGroupId, 
                EMCType, 
                CostCodeId, 
                EquipmentId, 
                glParm, 
                glORParm, 
                msgParm);
            if (glErr == -1)
            {
                GLCo = Equipment.EMCompanyParm.GLCo;
                tGLAcct = (string)glParm.Value;
            }
            else
            {
                GLCo = null;
                tGLAcct = null;
            }
        }

        private void GetJCGLAccount(VPEntities db)
        {
            var glParm = new System.Data.Entity.Core.Objects.ObjectParameter("glacct", typeof(string));
            var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
            var glErr = db.bspJCCAGlacctDflt(
                JCCo, 
                JobId, 
                PhaseGroupId, 
                PhaseId, 
                JCCType, 
                "N", 
                glParm, 
                msgParm);
            if (glErr == -1)
            {
                if (glParm.Value is string)
                {
                    GLCo = Job.JCCompanyParm.GLCo;
                    tGLAcct = (string)glParm.Value;
                }
            }
            else
            {
                GLCo = null;
                tGLAcct = null;
            }
        }
    }

    public partial class POBatchHeader
    {
        public string BaseTableName { get { return "POHB"; } }

        public POBatchItem AddItem(PORequestLine line)
        {
            var item = new POBatchItem
            {
                Co = Co,
                Mth = Mth,
                BatchId = BatchId,
                BatchSeq = BatchSeq,
                POItem = (short)line.LineId,
                BatchTransType = BatchTransType,
                ItemType = (byte)line.ItemTypeId,
                MatlGroup = Co,
                Description = line.Description,
                UM = line.UM,
                RecvYN = "N",
                PostToCo = Co,

                GLCo = Co,
                GLAcct = line.GLAcct,
                OrigUnits = line.Units ?? 0,
                OrigCost = line.Cost ?? 0,
                OrigUnitCost = line.UnitCost ?? 0,
                CrewId = line.CrewId ?? line.Request.OrderUser.CrewId,
                TaxGroup = line.TaxGroupId,
                TaxCode = line.TaxCodeId,
                TaxType = line.TaxTypeId,
                TaxRate = line.TaxRate ?? 0,
                OrigTax = line.TaxAmount ?? 0
            };

            if ((DB.POItemTypeEnum)item.ItemType == DB.POItemTypeEnum.Job)
            {
                item.JCCo = line.JCCo;
                item.JobId = line.JobId;
                item.PhaseGroupId = line.PhaseGroupId ?? line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                item.Phase = line.PhaseId;
                item.JCCType = line.JCCType;
                item.Job = line.Job;
                item.GLCo = line.Job.JCCompanyParm.GLCo;
                item.PostToCo = line.Job.JCCo;

                item.JobPhase = line.JobPhase;
                item.JobCostType = line.JobPhaseCost.CostType;
            }
            else if ((DB.POItemTypeEnum)item.ItemType == DB.POItemTypeEnum.Equipment)
            {
                item.EMCo = line.EMCo;
                item.EquipmentId = line.EquipmentId;
                item.EMGroup = line.EMGroupId;
                item.CostCode = line.CostCodeId;
                item.EMCType = line.EMCType;
                item.Equipment = line.Equipment;
                item.PostToCo = line.Equipment.EMCo;
                item.GLCo = line.Equipment.EMCompanyParm.GLCo;
            }


            if (item.UM != "LS")
            {
                item.OrigECM = "E";
            }

            this.Items.Add(item);

            return item;
        }
    }
}
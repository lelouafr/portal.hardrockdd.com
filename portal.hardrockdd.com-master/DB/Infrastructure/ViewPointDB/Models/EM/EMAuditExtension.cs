using System;
using System.Linq;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public static class EMAuditExtension
    {
        public static bool IsAuditLate(this EMAudit audit, DailyTicket ticket)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));
            if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));

            if ((audit.Status == EMAuditStatusEnum.New ||
                 audit.Status == EMAuditStatusEnum.Rejected ||
                 audit.Status == EMAuditStatusEnum.Started))
            {
                using var db = new VPContext();
                var createdDate = (DateTime)audit.CreatedOn; ;
                var cal = db.Calendars.FirstOrDefault(f => f.Date == createdDate.Date);
                var week = db.Calendars.Where(f => f.Week == cal.Week).ToList();
                var testDate = week.FirstOrDefault(f => f.Weekday == 5);
                if (testDate.Date <= DateTime.Now.Date && ((DateTime)ticket.WorkDate).Date >= testDate.Date)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAuditLate(this EMAudit audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));

            if ((audit.Status == EMAuditStatusEnum.New ||
                 audit.Status == EMAuditStatusEnum.Rejected ||
                 audit.Status == EMAuditStatusEnum.Started))
            {
                using var db = new VPContext();
                var createdDate = (DateTime)audit.CreatedOn;
                var cal = db.Calendars.FirstOrDefault(f => f.Date == createdDate.Date);
                var week = db.Calendars.Where(f => f.Week == cal.Week).ToList();
                var testDate = week.FirstOrDefault(f => f.Weekday == 5);
                if (testDate.Date <= DateTime.Now.Date)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCurrentWeek(this EMAudit audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));

            using var db = new VPContext();
            var createdDate = (DateTime)audit.CreatedOn;
            var currentDate = DateTime.Now.Date;
            var cal = db.Calendars.FirstOrDefault(f => f.Date == createdDate.Date);
            var calCur = db.Calendars.FirstOrDefault(f => f.Date == currentDate);

            return cal.Week == calCur.Week;

        }

    }

    public partial class EMAudit
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
                        _db = this.EMParameter.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        public EMAuditFormEnum AuditForm
        {
            get
            {
                return (EMAuditFormEnum)AuditFormId;
            }
            set
            {
                if ((int)value != AuditFormId)
                {
                    AuditFormId = (int)(value);
                }
                AuditFormId = (int)(value);
            }
        }

        public EMAuditStatusEnum Status
        {
            get
            {
                return (EMAuditStatusEnum)StatusId;
            }
            set
            {
                if ((byte)value != StatusId || WorkFlow == null)
                {
                    if (WorkFlow == null)
                        GenerateWorkFlow();

                    StatusId = (byte)(value);
                    WorkFlow.CreateSequence(StatusId);
                    UpdateStatus();
                    GenerateStatusLog();
                    return;
                }
                StatusId = (byte)(value);
            }
        }

        public EMAuditTypeEnum AuditType
        {
            get
            {
                return (EMAuditTypeEnum)AuditTypeId;
            }
            set
            {
                if ((byte)value != AuditTypeId)
                {
                    AuditTypeId = (byte)(value);

                    //do something after change
                }
                AuditTypeId = (byte)(value);
            }
        }

        private void GenerateWorkFlow()
        {
            var workflow = new WorkFlow
            {
                WFCo = EMCo,
                WorkFlowId = WorkFlow.GetNextWorkFlowId(EMCo),
                TableName = "budEMAH",
                Id = AuditId,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                Active = true,

                Company = EMParameter.HQCompanyParm,
            };

            //EMParameter.HQCompanyParm.WorkFlows.Add(workflow);
            db.WorkFlows.Add(workflow);
            WorkFlow = workflow;
        }

        public void UpdateStatus()
        {

            switch (Status)
            {
                case EMAuditStatusEnum.New:
                case EMAuditStatusEnum.Started:
                    GenerateWorkFlowAssignments();
                    break;
                case EMAuditStatusEnum.Submitted:
                    this.CompletedOn = DateTime.Now;
                    UpdateRelatedEquipmentLines();
                    GenerateWorkFlowAssignments();
                    break;
                case EMAuditStatusEnum.Approved:
                    //if (EMParameter.AuditAutoProcess == "Y" || AuditForm == EMAuditFormEnum.Inventory)
                    //{
                    //    Status = EMAuditStatusEnum.Processed;
                    //    return;
                    //}
                    //EMParameter.AuditApprovers.ToList().ForEach(e => WorkFlow.AddEmployee(e.Employee));
                    UpdateRelatedEquipmentLines();
                    GenerateEquipmentLog();
                    Status = EMAuditStatusEnum.Processed;
                    WorkFlow.CurrentSequence().Comments = "Auto Processed";
                    WorkFlow.CompleteWorkFlow();
                    break;
                case EMAuditStatusEnum.Rejected:
                    GenerateWorkFlowAssignments();
                    break;
                case EMAuditStatusEnum.Completed:
                    break;
                case EMAuditStatusEnum.Processed:
                    UpdateRelatedEquipmentLines();
                    GenerateEquipmentLog();
                    WorkFlow.CompleteWorkFlow();
                    
                    break;
                case EMAuditStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }

        }

        public void GenerateWorkFlowAssignments()
        {
           // var userList = new List<string>();

            switch (Status)
            {
                case EMAuditStatusEnum.New:
                case EMAuditStatusEnum.Started:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddUser(AssignedUser);
                    break;
                case EMAuditStatusEnum.Submitted:
                    if (EMParameter.AuditAppover == "Y")
                        EMParameter.AuditApprovers.ToList().ForEach(e => WorkFlow.AddEmployee(e.Employee));
                    else
                        WorkFlow.AddEmployee(AssignedUser.PREmployee.Supervisor);
                    break;
                case EMAuditStatusEnum.Approved:

                    break;
                case EMAuditStatusEnum.Rejected:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddUser(AssignedUser);
                    break;
                case EMAuditStatusEnum.Completed:
                    break;
                case EMAuditStatusEnum.Processed:  

                    break;
                case EMAuditStatusEnum.Canceled:

                    break;
                default:
                    break;
            }

        }

        private void UpdateRelatedEquipmentLines()
        {
            var newLines = Lines.Where(f => f.ActionId == (int)EMAuditLineActionEnum.Add).ToList();

            foreach (var line in newLines)
            {
                var updateList = line.Equipment.ActiveAuditLines.Where(f => f.AuditId != this.AuditId);

                foreach (var cur in updateList)
                {
                    cur.Completed = true;
                    cur.ActionId = (int)EMAuditLineActionEnum.Transfer;
                    cur.OdoDate = line.OdoDate;
                    cur.OdoReading = line.OdoReading;
                    cur.HourDate = line.HourDate;
                    cur.HourReading = line.HourReading;
                    switch (AuditType)
                    {
                        case EMAuditTypeEnum.CrewAudit:
                            cur.ToCrewId = line.ToCrewId;
                            cur.Equipment.AssignmentType = (int)EMAssignmentTypeEnum.Crew;
                            cur.Comments = string.Format("Equipment was auto transfered by {0} {1} to Crew {2}.  User added to their Eqp Audit.",
                                                            AssignedUser.FirstName,
                                                            AssignedUser.LastName,
                                                            ParmCrewId);
                            break;
                        case EMAuditTypeEnum.EmployeeAudit:
                            cur.ToLocationId = line.ToLocationId;

                            cur.Comments = string.Format("Equipment was auto transfered by {0} {1} to Employee {2}.  User added to their Eqp Audit.",
                                                            AssignedUser.FirstName,
                                                            AssignedUser.LastName,
                                                            ParmEmployeeId);
                            break;
                        case EMAuditTypeEnum.LocationAudit:
                            cur.ToEmployeeId = line.ToEmployeeId;
                            cur.Equipment.AssignmentType = (int)EMAssignmentTypeEnum.Employee;
                            cur.Comments = string.Format("Equipment was auto transfered by {0} {1} to Location {2}.  User added to their Eqp Audit.",
                                                            AssignedUser.FirstName,
                                                            AssignedUser.LastName,
                                                            ParmLocationId);
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private void GenerateEquipmentLog()
        {
            foreach (var line in Lines)
            {

                var seqId = line.Equipment.Logs.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1;
                var log = new EquipmentLog();
                log.EMCo = line.EMCo;
                log.EquipmentId = line.EquipmentId;
                log.LogDate = DateTime.Now;
                if (AuditFormId == (int)EMAuditFormEnum.Inventory)
                {
                    log.LogTypeId = 1;
                }
                log.SeqId = seqId;
                log.AuditId = AuditId;
                log.AuditSeqId = line.SeqId;
                log.LoggedBy = line.Audit.AssignedTo;

                switch (AuditForm)
                {
                    case EMAuditFormEnum.Meter:
                        log.NewHourReading = line.HourReading;
                        log.NewOdoReading = line.OdoReading;
                        log.OldHourReading = line.Equipment.HourReading;
                        log.OldOdoReading = line.Equipment.OdoReading;
                        log.MeterDate = line.HourDate ?? line.OdoDate;

                        //Update Equipment current Readings if greater than zero
                        if ((line.HourReading ?? 0) > 0)
                        {
                            line.Equipment.HourReading = line.HourReading ?? 0;
                            line.Equipment.HourDate = line.HourDate;
                        }
                        if ((line.OdoReading ?? 0) > 0)
                        {
                            line.Equipment.OdoReading = line.OdoReading ?? 0;
                            line.Equipment.OdoDate = line.OdoDate;
                        }
                        break;
                    case EMAuditFormEnum.Inventory:
                        break;
                    default:
                        break;
                }
                switch (AuditType)
                {
                    case EMAuditTypeEnum.CrewAudit:
                        var toCrew = db.Crews.FirstOrDefault(f => f.CrewId == line.ToCrewId);

                        if (line.ActionId == (int)EMAuditLineActionEnum.Transfer)
                        {
                            if (line.ToCrewId != null )
                            {
                                log.CrewConfirmed = true;
                                log.FromCrewId = line.Equipment.AssignedCrewId;
                                log.ToCrewId = line.ToCrewId;
                                
                                log.ToLocationId = toCrew?.udLocationId;
                                if (line.Equipment.LocationId != "Field")
                                {
                                    log.FromLocationId = line.Equipment.LocationId;
                                    log.ToLocationId = "Field";
                                }
                            }
                            else if (line.ToCrewId == null )
                            {
                                log.CrewConfirmed = false;
                                log.FromCrewId = line.Equipment.AssignedCrewId;
                                log.ToCrewId = null;
                                log.FromLocationId = line.Equipment.LocationId;
                                log.ToLocationId = line.Equipment.LocationId;
                            }
                        }
                        else if (line.ActionId == (int)EMAuditLineActionEnum.Add)
                        {
                            log.CrewConfirmed = line.Completed;
                            log.FromCrewId = line.Equipment.AssignedCrewId;
                            log.ToCrewId = ParmCrewId;
                            log.FromLocationId = line.Equipment.LocationId;
                            log.ToLocationId = "Field";
                        }
                        else if (line.ActionId == (int)EMAuditLineActionEnum.Update)
                        {
                            log.CrewConfirmed = line.Completed;
                            log.FromCrewId = line.Equipment.AssignedCrewId;
                            log.ToCrewId = ParmCrewId;
                            log.FromLocationId = line.Equipment.LocationId;
                            log.ToLocationId = "Field";
                        }
                        line.Equipment.AssignmentType = line.Equipment.AssignmentType ?? (int)EMAssignmentTypeEnum.Crew;
                        line.Equipment.AssignedCrewId = log.ToCrewId;
                        line.Equipment.LocationId = log.ToLocationId;
                        break;
                    case EMAuditTypeEnum.EmployeeAudit:
                        if (line.ActionId == (int)EMAuditLineActionEnum.Transfer)
                        {
                            if (line.ToEmployeeId != null)
                            {
                                log.EmployeeConfirmed = true;
                                log.FromEmployeeId = line.Equipment.OperatorId;
                                log.ToEmployeeId = line.ToEmployeeId;
                                log.FromLocationId = line.Equipment.LocationId;
                                log.ToLocationId = "Field";
                            }
                            else if (line.ToEmployeeId == null)
                            {
                                log.EmployeeConfirmed = false;
                                log.FromEmployeeId = line.Equipment.OperatorId;
                                log.ToEmployeeId = null;
                                log.FromLocationId = line.Equipment.LocationId;
                                log.ToLocationId = line.Equipment.LocationId;
                            }

                        }
                        else if (line.ActionId == (int)EMAuditLineActionEnum.Add || line.ActionId == (int)EMAuditLineActionEnum.Update)
                        {
                            log.EmployeeConfirmed = line.Completed;
                            log.FromEmployeeId = line.Equipment.OperatorId;
                            log.ToEmployeeId = ParmEmployeeId;
                            log.FromLocationId = line.Equipment.LocationId;
                            log.ToLocationId = "Field";
                        }

                        line.Equipment.AssignmentType = line.Equipment.AssignmentType ?? (int)EMAssignmentTypeEnum.Employee;
                        line.Equipment.OperatorId = log.ToEmployeeId;
                        line.Equipment.LocationId = log.ToLocationId;
                        //line.Equipment.AssignedCrewId = log.ToCrewId;
                        break;
                    case EMAuditTypeEnum.LocationAudit:
                        if (line.Action == EMAuditLineActionEnum.Transfer)
                        {
                            if (line.ToLocationId != null)
                            {
                                log.LocationConfirmed = true;
                                log.FromLocationId = line.Equipment.LocationId;
                                log.ToLocationId = line.ToLocationId;
                            }
                            else if (line.ToLocationId == null)
                            {
                                log.LocationConfirmed = false;
                                log.FromLocationId = line.Equipment.LocationId;
                                log.ToLocationId = null;
                            }
                        }
                        else if (line.ActionId == (int)EMAuditLineActionEnum.Add || line.ActionId == (int)EMAuditLineActionEnum.Update)
                        {
                            log.LocationConfirmed = line.Completed;
                            log.FromLocationId = line.Equipment.LocationId;
                            log.ToLocationId = ParmLocationId;
                        }
                        line.Equipment.LocationId = log.ToLocationId;
                        break;
                    default:
                        break;
                }

                line.Equipment.Logs.Add(log);
            }
        }

        private void GenerateVPBatch()
        {
            if (AuditForm == EMAuditFormEnum.Meter)
            {
                var batchLines = Lines.Where(f => (f.Action == EMAuditLineActionEnum.Add || f.Action == EMAuditLineActionEnum.Update) &&
                                                   f.Completed == true &&
                                                   f.MeterType != EMMeterTypeEnum.None)
                                      .ToList();

                if (batchLines.Any())
                {
                    var company = this.EMParameter.HQCompanyParm;

                    var mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
                    var batch = Batch.FindCreate(company, "EMBF", "EMMeter", mth);
                    //batch = this.EMParameter.Company.Batches.FirstOrDefault(f => f.EMCo == batch.EMCo && f.Mth == batch.Mth && f.BatchId == batch.BatchId);

                    var batchSeqId = batch.EMUsageBatches.DefaultIfEmpty().Max(f => f == null ? 0 : f.BatchSeq) + 1;

                    this.Batch = batch;
                    foreach (var line in batchLines)
                    {
                        var existingLines = line.Equipment.EMBatchLines.ToList();
                        if (existingLines.Any())
                        {
                            existingLines.ForEach(e => e.Equipment.EMBatchLines.Remove(e));
                        }

                        var parms = line.Equipment.EMCompanyParm;
                        var batchSeq = new EMBatchUsage
                        {
                            Co = batch.Co,
                            Mth = batch.Mth,
                            BatchId = batch.BatchId,
                            BatchSeq = batchSeqId,
                            Description = "Equipment Meter Posting",
                            Source = batch.Source,
                            ActualDate = ((line.HourDate ?? line.OdoDate) ?? DateTime.Now.Date),
                            BatchTransType = "A",
                            EMTransType = "Equip",
                            EMGroup = line.EMCo,
                            EquipmentId = line.EquipmentId,
                            INStkUnitCost = 0,
                            UnitPrice = 0,
                            RevRate = 0,
                            GLCo = parms.GLCo,
                            OffsetGLCo = parms.GLCo,
                            PreviousHourMeter = line.Equipment.HourReading,
                            PreviousOdometer = line.Equipment.OdoReading,
                            PreviousTotalHourMeter = line.Equipment.HourReading,
                            PreviousTotalOdometer = line.Equipment.OdoReading,
                            MeterReadDate = line.HourReading != null ? line.HourDate : line.OdoDate,
                            CurrentHourMeter = line.HourReading ?? line.Equipment.HourReading,
                            CurrentOdometer = line.OdoReading ?? line.Equipment.OdoReading,
                            CurrentTotalHourMeter = line.HourReading ?? line.Equipment.HourReading,
                            CurrentTotalOdometer = line.OdoReading ?? line.Equipment.OdoReading,
                            AutoUsage = "N",
                            TicketId = line.AuditId,
                            TicketSeqId = line.SeqId,

                            Equipment = line.Equipment,
                            Batch = batch,
                        };
                        batchSeq.MeterMiles = batchSeq.CurrentTotalOdometer - batchSeq.PreviousTotalOdometer;
                        batchSeq.MeterHrs = batchSeq.CurrentTotalHourMeter - batchSeq.PreviousTotalHourMeter;

                        line.BatchId = batch.BatchId;
                        line.BatchSeq = batchSeq.BatchSeq;

                        batch.EMUsageBatches.Add(batchSeq);
                        line.Equipment.EMBatchLines.Add(batchSeq);
                        
                        batchSeqId++;
                    }
                }
            }
        }

        private void GenerateStatusLog(string comments = "")
        {
            var log = new EMAuditStatusLog
            {
                EMCo = EMCo,
                AuditId = AuditId,
                LineNum = StatusLogs.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineNum) + 1,
                Status = StatusId,
                CreatedOn = DateTime.Now,
                CreatedBy = db.CurrentUserId,
                Comments = comments ?? ""
            };

            StatusLogs.Add(log);
        }
    }
}
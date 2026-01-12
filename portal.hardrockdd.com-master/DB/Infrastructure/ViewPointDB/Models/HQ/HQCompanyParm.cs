using System;
using System.Collections.Generic;
using System.Linq;
 
namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQCompanyParm
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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "HQBC"; } }

        private PRCompanyParm _PRCompanyParm;

        public PRCompanyParm PRCompanyParm
        {
            get
            {
                if (_PRCompanyParm == null)
                {
                    _PRCompanyParm = this.dbPRCompanyParm;
                    if (_PRCompanyParm.PRCo != PRCo)
                    {
                        
                        _PRCompanyParm = PRCompanyParmLink;
                    }
                }

                return _PRCompanyParm;
            }
            set
            {
                _PRCompanyParm = value;
            }
        }

        public DailyTicket AddDailyTicket(DateTime workDate, DTFormEnum formType)
        {
            var curUser = db.GetCurrentUser();
            var dailyTicket = new DailyTicket
            {
                DTCo = (byte)HQCo,
                TicketId = DailyTicket.GetNextTicketId(),
                WorkDate = workDate,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                ParentTicketId = null,

                CreatedUser = curUser,
                HQCompanyParm = this,
                db = this.db,
                Status = DailyTicketStatusEnum.Draft,
                FormType = formType
            };
            db.DailyTickets.Add(dailyTicket);

            return dailyTicket;
        }

        public Batch AddBatch(string tableName, string source, DateTime mth)
        {
            var vpUserName = db.GetCurrentEmployee().UserProfile.FirstOrDefault()?.VPUserName;
            var batch = db.Batches.FirstOrDefault(f => f.Co == this.HQCo &&
                                                       f.CreatedBy == vpUserName &&
                                                       f.Status == (int)BatchStatusEnum.Open &&
                                                       f.Mth == mth &&
                                                       f.Source == source &&
                                                       f.TableName == tableName);

            if (batch == null)
            {
                var batchId = GetNextBatchId(mth);
                batch = new Batch
                {
                    Co = HQCo,
                    BatchId = batchId,
                    TableName = tableName,
                    Source = source,
                    Mth = mth,
                    CreatedBy = vpUserName,
                    DateCreated = DateTime.Now,
                    Status = 0,
                    Rstrict = "N",
                    Adjust = "N",
                    Company = this,
                    db = db,
                };
            }

            return batch;
        }

        public Batch AddBatch(string tableName, string source, DateTime mth, DateTime prEndDate)
        {
            var vpUserName = db.GetCurrentEmployee().UserProfile.FirstOrDefault()?.VPUserName;
            var batch = db.Batches.FirstOrDefault(f => f.Co == this.HQCo &&
                                                       f.CreatedBy == vpUserName &&
                                                       f.Status == (int)BatchStatusEnum.Open &&
                                                       f.Mth == mth &&
                                                       f.PREndDate == prEndDate &&
                                                       f.Source == source &&
                                                       f.TableName == tableName);

            if (batch == null)
            {
                var batchId = GetNextBatchId(mth);
                batch = new Batch
                {
                    Co = HQCo,
                    BatchId = batchId,
                    TableName = tableName,
                    Source = source,
                    Mth = mth,
                    CreatedBy = vpUserName,
                    DateCreated = DateTime.Now,
                    Status = 0,
                    Rstrict = "N",
                    Adjust = "N",
                    Company = this,
                    db = db,
                };
            }

            return batch;
        }

        private int GetNextBatchId(DateTime mth)
        {
            var transTable = this.BatchTableControls.FirstOrDefault(f => f.TableName == BaseTableName && f.Mth == mth);
            if (transTable == null)
            {
                transTable = new BatchTableControl()
                {
                    Co = this.HQCo,
                    TableName = BaseTableName,
                    Mth = mth,
                    LastTrans = 0,
                    Company = this,
                };

                this.BatchTableControls.Add(transTable);
            }
            
            transTable.LastTrans++;
            return transTable.LastTrans;
        }

        public Bid AddBid()
        {
            var employee = db.GetCurrentEmployee();
            var division = employee.Division?.PMDivisions?.FirstOrDefault();

            var bid = new Bid
            {
                db = db,
                CreatedUser = db.GetCurrentUser(),
                Company = this,
                Division = division,

                BDCo = 1,
                BidId = db.GetNextId(Bid.BaseTableName),
                BidDate = DateTime.Now.Date,
                tStatusId = (int)DB.BidStatusEnum.Draft,
                BidType = (int)DB.BidTypeEnum.Quote,
                DelayDeMobePrice = 18500,
                DeMobePrice = 0,
                MobePrice = 0,
                JCCo = division?.PMCo,
                tDivisionId = division?.DivisionId,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,

                ProjectMangerId = division?.WPDivision?.ManagerId,

            };

            bid.AddForum();
            bid.AddWorkFlow();
            bid.WorkFlow.CreateSequence(bid.StatusId);
            bid.AddWorkFlowAssignments();
            bid.ImportScopeTemplate();
            this.Bids.Add(bid);
            db.Bids.Add(bid);
            db.BulkSaveChanges();
            return bid;
        }
    }


    //public static class HQCompanyParmExt
    //{

    //    public static List<Job> ActiveProjects(this HQCompanyParm company)
    //    {
    //        if (company == null)
    //            return new List<Job>();
    //        return company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7" && (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).ToList();
    //    }
        
    //    public static List<Job> Projects(this HQCompanyParm company)
    //    {
    //        if (company == null)
    //            return new List<Job>();
    //        return company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7").ToList();
    //    }

    //    public static List<Job> ActiveJobs(this HQCompanyParm company)
    //    {
    //        if (company == null)
    //            return new List<Job>();
    //        return company.JCCompanyParm.Jobs.Where(f => (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).ToList();
    //    }
    //}
}
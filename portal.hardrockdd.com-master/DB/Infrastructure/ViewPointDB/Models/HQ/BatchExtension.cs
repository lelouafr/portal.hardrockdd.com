using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Batch
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
                _db ??= this.Company.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static Batch FindCreate(HQCompanyParm company, string tableName, string source, DateTime mth)
        {
            var curEmp = company.db.GetCurrentEmployee();
            var employee = company.db.Employees.Include("UserProfile").FirstOrDefault(f => f.PRCo == curEmp.PRCo && f.EmployeeId == curEmp.EmployeeId);
            var userProf = employee.UserProfile.FirstOrDefault();

            var db = company.db;
            //var batches = company.Batches.ToList();
            var batch = db.Batches.FirstOrDefault(f => f.Co == company.HQCo &&
                                                       f.Mth == mth &&
                                                       f.CreatedBy.Trim() == userProf.VPUserName.Trim() &&
                                                       f.TableName.Trim() == tableName.Trim() &&
                                                       f.Source.Trim() == source.Trim() &&
                                                       f.Status == 0
                                                       );

            //Check Local entities
            if (batch == null)
            {
                batch = db.Batches.Local.FirstOrDefault(f => f.Co == company.HQCo &&
                                                             f.Mth == mth &&
                                                             f.CreatedBy.Trim() == userProf.VPUserName.Trim() &&
                                                             f.TableName.Trim() == tableName.Trim() &&
                                                             f.Source.Trim() == source.Trim() &&
                                                             f.Status == 0
                                                        );
            }
            if (batch == null)
            {
                //throw new Exception("");
                if (userProf == null)
                {
                    return null;
                }
                //throw new Exception("");
                var batchTables = company.BatchTableControls.Where(f => f.TableName.Trim() == "bHQBC").ToList();
                var batchTable = batchTables.FirstOrDefault(f => f.Mth.Date == mth.Date);
                if (batchTable == null)
                {
                    batchTable = new BatchTableControl() { 
                        TableName = "bHQBC",
                        Co = company.HQCo,
                        Mth = mth.Date,
                        LastTrans = 0
                    };
                    company.BatchTableControls.Add(batchTable);
                }
                batchTable.LastTrans++;
                batch = new Batch
                {
                    Co = company.HQCo,
                    BatchId = batchTable.LastTrans,
                    Mth = mth,
                    CreatedBy = userProf.VPUserName,
                    DateCreated = DateTime.Now,
                    Status = 0,
                    Rstrict = "N",
                    Adjust = "N",
                    TableName = tableName,
                    Source = source,
                    InUseBy = "WebPortalUser",
                    Company = company,

                    db = db,
                };
                //company.Batches.Add(batch);
                db.Batches.Add(batch);
            }

            return batch;
        }

        public static Batch FindCreate(string tableName, string source, byte co, DateTime mth)
        {
            using var db = new VPContext();

            var curEmp = db.GetCurrentEmployee();
            var employee = db.Employees.FirstOrDefault(f => f.PRCo == curEmp.PRCo && f.EmployeeId == curEmp.EmployeeId);
            var userProf = employee.UserProfile.FirstOrDefault();

            var batch = db.Batches.FirstOrDefault(f => f.Co == co &&
                                                        f.Mth == mth &&
                                                        f.CreatedBy == userProf.VPUserName &&
                                                        f.TableName == tableName &&
                                                        f.Source == source &&
                                                        f.Status == 0
                                                        );
            if (batch == null)
            {
                batch = Create(tableName, source, co, mth);

            }

            return batch;
        }

        public static Batch Create(string tableName, string source, byte co, DateTime mth)
        {
            using var db = new VPContext();

            var curEmp = db.GetCurrentEmployee();
            var employee = db.Employees.FirstOrDefault(f => f.PRCo == curEmp.PRCo && f.EmployeeId == curEmp.EmployeeId);
            var userProf = employee.UserProfile.FirstOrDefault();
            var batchTable = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == mth && f.Co == co);
            batchTable.LastTrans++;

            var batch = new Batch
            {
                Co = co,
                BatchId = batchTable.LastTrans,
                Mth = mth,
                CreatedBy = userProf.VPUserName,
                DateCreated = DateTime.Now,
                Status = 0,
                Rstrict = "N",
                Adjust = "N",
                TableName = tableName,
                Source = source,
                InUseBy = "WebPortalUser",
            };
            db.Batches.Add(batch);
            db.SaveChanges();
            db.Detach(batch);

            batch = db.Batches.FirstOrDefault(f => f.Co == batch.Co && f.Mth == batch.Mth && f.BatchId == batch.BatchId);
            return batch;
        }

        public BatchStatusEnum StatusEnum
        {
            get
            {
                return (BatchStatusEnum)(Status);
            }
            set
            {
                Status = (byte)value;
            }
        }

        public string ValidateBatch()
        {
            var db = DbContextExtension.GetDbContextFromEntity(this);
            if (db == null)
                db = new VPContext();

            var batch = db.Batches.FirstOrDefault(f => f.Co == Co && f.Mth == Mth && f.BatchId == BatchId);
            batch.InUseBy = "WebPortalUser";
            db.SaveChanges();

            if (StatusEnum == BatchStatusEnum.Open || StatusEnum == BatchStatusEnum.ValidatedErros || StatusEnum == BatchStatusEnum.ValidationWarnings)
            {
                return this.Source.Trim() switch
                {
                    "AP Entry" => this.ValidateAPEntryBatch(),
                    "PR Entry" => this.ValidatePREntryBatch(),
                    "EMMeter" => this.ValidateEMMeter(),
                    "PO Entry" => this.ValidatePOEntryBatch(),
                    _ => "Batch Source Not Mapped",
                };
            }
            else if (StatusEnum == BatchStatusEnum.ValidationOK)
            {
                return "";
            }
            else//if (StatusEnum != BatchStatusEnum.Open)
            {
                return "Batch is not open";
            }
        }

        public string PostBatch()
        {
            var db = DbContextExtension.GetDbContextFromEntity(this);
            if (db == null)
                db = new VPContext();

            var batch = db.Batches.FirstOrDefault(f => f.Co == Co && f.Mth == Mth && f.BatchId == BatchId);
            batch.InUseBy = "WebPortalUser";
            db.SaveChanges();

            if (batch.StatusEnum == BatchStatusEnum.ValidationOK)
            {

                return this.Source.Trim() switch
                {
                    "AP Entry" => this.PostAPEntryBatch(),
                    "PR Entry" => this.PostPREntryBatch(),
                    "EMMeter" => this.PostEMMeter(),
                    "PO Entry" => this.PostPOEntryBatch(),
                    _ => "Batch Source Not Mapped",
                };
            }
            else
            {
                return "Batch is not Validated";
            }
        }

        public string ClearBatch()
        {
            if (StatusEnum == BatchStatusEnum.Open)
            {
                switch (this.Source.Trim())
                {
                    case "AP Entry":
                        return this.ClearAPEntryBatch();
                    default:
                        break;
                }

                return "Batch Source Not Mapped";
            }
            else//if (StatusEnum != BatchStatusEnum.Open)
            {
                return "Batch is not open";
            }
        }

        public string ErrorMessage { get; set; }

        public List<Batch> ProcessedBatches { get; set; }
    }

    
}
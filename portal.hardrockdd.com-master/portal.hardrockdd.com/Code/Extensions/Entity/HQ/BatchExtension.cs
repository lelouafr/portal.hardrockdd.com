using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Batch
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
                        _db = this.Company.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public static Batch FindCreate(Code.Data.VP.HQCompanyParm company, string tableName, string source, DateTime mth)
        {
            var curEmp = StaticFunctions.GetCurrentEmployee();
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
            using var db = new VPEntities();

            var curEmp = StaticFunctions.GetCurrentEmployee();
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
            using var db = new VPEntities();

            var curEmp = StaticFunctions.GetCurrentEmployee();
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

        public DB.BatchStatusEnum StatusEnum
        {
            get
            {
                return (DB.BatchStatusEnum)(Status);
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
                db = new VPEntities();

            var batch = db.Batches.FirstOrDefault(f => f.Co == Co && f.Mth == Mth && f.BatchId == BatchId);
            batch.InUseBy = "WebPortalUser";
            db.SaveChanges();

            if (StatusEnum == DB.BatchStatusEnum.Open || StatusEnum == DB.BatchStatusEnum.ValidatedErros || StatusEnum == DB.BatchStatusEnum.ValidationWarnings)
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
            else if (StatusEnum == DB.BatchStatusEnum.ValidationOK)
            {
                return "";
            }
            else//if (StatusEnum != DB.BatchStatusEnum.Open)
            {
                return "Batch is not open";
            }
        }

        public string PostBatch()
        {
            var db = DbContextExtension.GetDbContextFromEntity(this);
            if (db == null)
                db = new VPEntities();

            var batch = db.Batches.FirstOrDefault(f => f.Co == Co && f.Mth == Mth && f.BatchId == BatchId);
            batch.InUseBy = "WebPortalUser";
            db.SaveChanges();

            if (batch.StatusEnum == DB.BatchStatusEnum.ValidationOK)
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
            if (StatusEnum == DB.BatchStatusEnum.Open)
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
            else//if (StatusEnum != DB.BatchStatusEnum.Open)
            {
                return "Batch is not open";
            }
        }

        #region AP
        private string ValidateAPEntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspAPHBVal(this.Co, this.Mth, this.BatchId, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private string ClearAPEntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspAPBatchClear(this.Co, this.Mth, this.BatchId, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private string PostAPEntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspAPHBPost(this.Co, this.Mth, this.BatchId, DateTime.Now, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;

                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
        #endregion

        #region PO
        private string ValidatePOEntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPOHBVal(this.Co, this.Mth, this.BatchId, this.Source, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private string PostPOEntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPOHBPost(this.Co, this.Mth, this.BatchId, DateTime.Now, this.Source, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
        #endregion

        #region EM
        private string ValidateEMMeter()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspEMVal_Meters_Main(this.Co, this.Mth, this.BatchId, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (tmpBatch.BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private string PostEMMeter()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspEMPost_Meters_Main(this.Co, this.Mth, this.BatchId, DateTime.Now, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
        #endregion

        #region PR
        private string ValidatePREntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPRTBVal(this.Co, this.Mth, this.BatchId, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
        
        private string PostPREntryBatch()
        {
            try
            {
                using var tmpDb = new VPEntities();
                tmpDb.Database.CommandTimeout = 600;
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPRTBPost(this.Co, this.Mth, this.BatchId, DateTime.Now, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                }

                return this.ErrorMessage;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private Batch ProcessPROvertime()
        {

            using var db = DbContextExtension.GetDbContextFromEntity(this);
            /** OT Entries**/
            var batch = Repository.VP.HQ.BatchRepository.FindCreatePR(this.TableName, this.Source, this.PRGroup ?? Co, (DateTime)this.PREndDate, db);
            batch.InUseBy = InUseBy;
            db.SaveChanges();

            var addedYN = new ObjectParameter("PRTBAddedYN", typeof(string));
            var msgParm = new ObjectParameter("msg", typeof(string));
            var error = db.bspPRAutoOT(batch.Co, batch.Mth, batch.BatchId, null, null, addedYN, msgParm);
            if (BatchErrors.Any())
            {
                batch.ErrorMessage = (string)msgParm.Value;
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.ValidateBatch();
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.PostBatch();
            }
            return batch;
        }

        private Batch ProcessPRSalary()
        {
            using var db = DbContextExtension.GetDbContextFromEntity(this);
            
            var batch = Repository.VP.HQ.BatchRepository.FindCreatePR(this.TableName, this.Source, this.PRGroup ?? Co, (DateTime)this.PREndDate, db);
            var addedYN = new ObjectParameter("PRTBAddedYN", typeof(string));
            var msgParm = new ObjectParameter("msg", typeof(string));
            var error = db.bspPRSalaryDistrib(batch.Co, batch.Mth, batch.BatchId, null, null, addedYN, msgParm);
            if (BatchErrors.Any())
            {
                batch.ErrorMessage = (string)msgParm.Value;
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.ValidateBatch();
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.PostBatch();
            }
            return batch;
        }

        #endregion

        public string ErrorMessage { get; set; }

        public List<Batch> ProcessedBatches { get; set; }
    }

    
}
using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Repository.VP.HQ
{
    public partial class BatchRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public BatchRepository()
        {

        }

        public static Batch Init()
        {
            using var empRepo = new EmployeeRepository();
            var emp = empRepo.GetEmployee(StaticFunctions.GetUserId());
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new Batch
            {
                CreatedBy = emp.UserProfile.FirstOrDefault()?.VPUserName,
                DateCreated = DateTime.Now,
                Status = 0,
                Rstrict = "N",
                Adjust = "N",
                Co = comp.HQCo,
            };
            return model;
        }

        public static Batch Init(string tableName, string source)
        {
            using var empRepo = new EmployeeRepository();
            var emp = empRepo.GetEmployee(StaticFunctions.GetUserId());
            var comp = StaticFunctions.GetCurrentCompany();
            
            var model = new Batch
            {
                CreatedBy = emp.UserProfile.FirstOrDefault()?.VPUserName,
                DateCreated = DateTime.Now,
                Status = 0,
                Rstrict = "N",
                Adjust = "N",
                Co = comp.HQCo,
                TableName = tableName,
                Source = source,
            };
            return model;
        }
        
        public Models.Views.Payroll.PayrollBatchViewModel ProcessUpdate(Models.Views.Payroll.PayrollBatchViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.Batches.FirstOrDefault(f => f.Co == model.Co && f.Mth == model.Mth && f.BatchId == model.BatchId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Status = model.Status;
                if (model.PREndDate >= new DateTime(1900, 1, 1))
                {
                    updObj.PREndDate = model.PREndDate;
                }
                updObj.Notes = model.Notes;

                db.SaveChanges(modelState);
            }
            return new Models.Views.Payroll.PayrollBatchViewModel(updObj);
        }

        public Batch FindCreatePR(string tableName, string source, int prGroup, DateTime prEndDate)
        {
            var payPerdiod = db.PayPeriods.FirstOrDefault(f => f.PRGroup == prGroup && f.PREndDate == prEndDate);
            var batch = Init(tableName, source);
            batch.PRGroup = payPerdiod.PRGroup;
            batch.PREndDate = prEndDate;
            batch.Mth = payPerdiod.BeginMth;
            var result = db.Batches.FirstOrDefault(f => f.Co == batch.Co &&
                                                        f.Mth == batch.Mth &&
                                                        f.CreatedBy == batch.CreatedBy &&
                                                        f.TableName == batch.TableName &&
                                                        f.Status == batch.Status &&
                                                        f.PREndDate == prEndDate
                                                        );
            if (result == null)
            {
                batch = Create(batch);
            }
            else
            {
                batch = result;
            }

            return batch;
        }


        public Batch FindCreate(string tableName, string source, DateTime mth)
        {
            var batch = Init(tableName, source);
            batch.Mth = mth;
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == batch.Co);
            batch.Company = company;
            var result = db.Batches.FirstOrDefault(f => f.Co == batch.Co &&
                                                        f.Mth == batch.Mth &&
                                                        f.CreatedBy == batch.CreatedBy &&
                                                        f.TableName == batch.TableName &&
                                                        f.Status == batch.Status
                                                        );
            if (result == null)
            {
                batch = Create(batch);
            }
            else
            {
                batch = result;
            }

            return batch;
        }

        public static Batch FindCreate(string tableName, string source, DateTime mth, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            
            var batch = Init(tableName, source);
            batch.Mth = mth;

            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == batch.Co);
            batch.Company = company;

            var result = db.Batches.FirstOrDefault(f => f.Co == batch.Co &&
                                                        f.Mth == batch.Mth &&
                                                        f.CreatedBy == batch.CreatedBy &&
                                                        f.TableName == batch.TableName &&
                                                        f.Status == batch.Status &&
                                                        f.Source == batch.Source
                                                        );
            if (result == null)
            {
                var table = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == batch.Mth && f.Co == batch.Co);
                table.LastTrans++;
                batch.BatchId = table.LastTrans;
                db.Batches.Add(batch);
            }
            else
            {
                batch = result;
            }

            return batch;
        }


        public static Batch FindCreatePR(string tableName, string source, int prGroup, DateTime prEndDate, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var payPerdiod = db.PayPeriods.FirstOrDefault(f => f.PRGroup == prGroup && f.PREndDate == prEndDate);
            var batch = Init(tableName, source);
            batch.PRGroup = payPerdiod.PRGroup;
            batch.PREndDate = prEndDate;
            batch.Mth = payPerdiod.BeginMth;
            var result = db.Batches.FirstOrDefault(f => f.Co == batch.Co &&
                                                        f.Mth == batch.Mth &&
                                                        f.CreatedBy == batch.CreatedBy &&
                                                        f.TableName == batch.TableName &&
                                                        f.Status == batch.Status &&
                                                        f.Source == batch.Source &&
                                                        f.PREndDate == prEndDate
                                                        );
            if (result == null)
            {
                var table = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == batch.Mth && f.Co == batch.Co);
                table.LastTrans++;
                batch.BatchId = table.LastTrans;
                db.Batches.Add(batch);
            }
            else
            {
                batch = result;
            }

            return batch;
        }


        public static Batch CreatePR(string tableName, string source, int prGroup, DateTime prEndDate, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var payPerdiod = db.PayPeriods.FirstOrDefault(f => f.PRGroup == prGroup && f.PREndDate == prEndDate);
            var batch = Init(tableName, source);
            batch.PRGroup = payPerdiod.PRGroup;
            batch.PREndDate = prEndDate;
            batch.Mth = payPerdiod.BeginMth;

            var table = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == batch.Mth && f.Co == batch.Co);
            table.LastTrans++;
            batch.BatchId = table.LastTrans;
            db.Batches.Add(batch);

            return batch;
        }

        //public static Batch FindCreate(string tableName, string source, DateTime mth, VPContext db)
        //{
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));
        //    var batch = Init(tableName, source);
        //    batch.Mth = mth;
        //    var result = db.Batches.FirstOrDefault(f => f.Co == batch.Co &&
        //                                                f.Mth == batch.Mth &&
        //                                                f.CreatedBy == batch.CreatedBy &&
        //                                                f.TableName == batch.TableName &&
        //                                                f.Status == batch.Status
        //                                                );
        //    if (result == null)
        //    {
        //        var table = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == batch.Mth && f.Co == batch.Co);
        //        table.LastTrans++;
        //        batch.BatchId = table.LastTrans;
        //        db.Batches.Add(batch);
        //        db.SaveChanges();
        //        db.Detach(batch);
        //        return FindCreate(tableName, source, mth, db);
        //    }
        //    else
        //    {
        //        return result;
        //    }


        //}


        public static void AddDefaultBatchActions(Batch batch, Controller controller, VPContext db)
        {
            if (!batch.Actions.Any())
            {
                var action = new BatchAction()
                {
                    Co = batch.Co,
                    Mth = batch.Mth,
                    BatchId = batch.BatchId,
                    SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    Title = "Validate",
                    ActionUrl = controller.Url.Action("ValidateBatch", "Batch"),
                    ActionType = "post",
                    IsActive = true,
                    //Status = batch.Status,
                    SubBatchId = batch.BatchId,
                    SubBatchMth = batch.Mth,
                };
                batch.Actions.Add(action);

                action = new BatchAction()
                {
                    Co = batch.Co,
                    Mth = batch.Mth,
                    BatchId = batch.BatchId,
                    SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                    Title = "Post",
                    ActionUrl = controller.Url.Action("PostBatch", "Batch"),
                    ActionType = "post",
                    IsActive = true,
                    //Status = batch.Status,
                    SubBatchId = batch.BatchId,
                    SubBatchMth = batch.Mth,
                };
                batch.Actions.Add(action);

                if (batch.Source.Trim() == "PR Entry")
                {
                    action = new BatchAction()
                    {
                        Co = batch.Co,
                        Mth = batch.Mth,
                        BatchId = batch.BatchId,
                        SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                        Title = "Calculate OT",
                        ActionUrl = controller.Url.Action("PRCreateOTBatch", "Batch"),
                        ActionType = "get",
                        IsActive = true,
                        //Status = batch.Status,
                    };
                    batch.Actions.Add(action);

                    action = new BatchAction()
                    {
                        Co = batch.Co,
                        Mth = batch.Mth,
                        BatchId = batch.BatchId,
                        SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                        Title = "Salary Distribution",
                        ActionUrl = controller.Url.Action("PRCreateSalaryDistroBatch", "Batch"),
                        ActionType = "get",
                        IsActive = true,
                        //Status = batch.Status,
                    };
                    batch.Actions.Add(action);

                }
            }
        }

        public Batch GetBatch(byte Co, DateTime Mth, int BatchId)
        {
            var qry = db.Batches
                        .FirstOrDefault(f => f.Co == Co && f.Mth == Mth && f.BatchId == BatchId);

            return qry;
        }
        

        public Batch Create(Batch model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var table = db.BatchTableControls.FirstOrDefault(f => f.TableName == "bHQBC" && f.Mth == model.Mth && f.Co == model.Co);
            table.LastTrans++;
            model.BatchId = table.LastTrans;
            db.Batches.Add(model);
            db.SaveChanges(modelState);
            db.Detach(model);
            return GetBatch(model.Co, model.Mth, model.BatchId);
        }
  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BatchRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }
}

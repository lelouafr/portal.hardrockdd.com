//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.PR
//{
//    public partial class BatchHeaderRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public BatchHeaderRepository()
//        {

//        }
        
//        public static POBatchHeader Init()
//        {
//            var model = new POBatchHeader
//            {

//            };

//            return model;
//        }
//        public static POBatchHeader Init(Batch batch, PORequest request)
//        {
//            if (batch == null)
//            {
//                throw new ArgumentNullException(nameof(batch));
//            }
//            if (request == null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            var model = new POBatchHeader
//            {
//                Co = batch.Co,
//                Mth = batch.Mth,
//                BatchId = batch.BatchId,
//                BatchTransType = "A",
//                PO = request.PO,
//                VendorGroup = request.VendorGroup ?? batch.Co,
//                VendorId = request.VendorId,
//                Description = request.Description,
//                OrderDate = request.OrderedDate,
//                OrderedBy = request.OrderedBy.ToString(AppCultureInfo.CInfo()),
//                ExpDate = request.OrderedDate,
//                Status = 0,
//                JCCo = request.Co,
//                JobId = request.JobId,
//                PayTerms = request.Vendor.PayTerms,                
//                UniqueAttchID = request.UniqueAttchID
//            };

//            if (request.ApprovedUser != null)
//            {
//                model.PortalApprover = request.ApprovedUser.Employee.FirstOrDefault().HRRef;
//            }
//            else
//            {
//                using var db = new VPContext();
//                var user = db.WebUsers.FirstOrDefault(f => f.Id == request.ApprovedBy);
//                model.PortalApprover = user.Employee.FirstOrDefault().HRRef;
//            }
//            return model;
//        }
//        public static int NextId(Batch model)
//        {
//            using var db = new VPContext();
//            return db.POBatchHeaders
//                            .Where(f => f.Co == model.Co && f.BatchId == model.BatchId)
//                            .DefaultIfEmpty()
//                            .Max(f => f == null ? 0 : f.BatchSeq) + 1;
//        }

//        public POBatchHeader ProcessUpdate(POBatchHeader model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            var updObj = db.POBatchHeaders
//                            .FirstOrDefault(f => f.Co == model.Co && f.Mth == model.Mth && f.BatchId == model.BatchId && f.BatchSeq == model.BatchSeq);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Co = model.Co;
//                if (model.Mth >= new DateTime(1900, 1, 1))
//                {
//                    updObj.Mth = model.Mth;
//                }
//                updObj.BatchId = model.BatchId;
//                updObj.BatchSeq = model.BatchSeq;
//                updObj.BatchTransType = model.BatchTransType;
//                updObj.PO = model.PO;
//                updObj.VendorGroup = model.VendorGroup;
//                updObj.VendorId = model.VendorId;
//                updObj.Description = model.Description;
//                if (model.OrderDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.OrderDate = model.OrderDate ?? updObj.OrderDate;
//                }
//                updObj.OrderedBy = model.OrderedBy;
//                if (model.ExpDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.ExpDate = model.ExpDate ?? updObj.ExpDate;
//                }
//                updObj.Status = model.Status;
//                updObj.JCCo = model.JCCo;
//                updObj.JobId = model.JobId;
//                updObj.INCo = model.INCo;
//                updObj.Loc = model.Loc;
//                updObj.ShipLoc = model.ShipLoc;
//                updObj.Address = model.Address;
//                updObj.City = model.City;
//                updObj.State = model.State;
//                updObj.Zip = model.Zip;
//                updObj.ShipIns = model.ShipIns;
//                updObj.HoldCode = model.HoldCode;
//                updObj.PayTerms = model.PayTerms;
//                updObj.CompGroup = model.CompGroup;
//                updObj.Notes = model.Notes;
//                updObj.OldVendorGroup = model.OldVendorGroup;
//                updObj.OldVendor = model.OldVendor;
//                updObj.OldDesc = model.OldDesc;
//                if (model.OldOrderDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.OldOrderDate = model.OldOrderDate ?? updObj.OldOrderDate;
//                }
//                updObj.OldOrderedBy = model.OldOrderedBy;
//                if (model.OldExpDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.OldExpDate = model.OldExpDate ?? updObj.OldExpDate;
//                }
//                updObj.OldStatus = model.OldStatus;
//                updObj.OldJCCo = model.OldJCCo;
//                updObj.OldJob = model.OldJob;
//                updObj.OldINCo = model.OldINCo;
//                updObj.OldLoc = model.OldLoc;
//                updObj.OldShipLoc = model.OldShipLoc;
//                updObj.OldAddress = model.OldAddress;
//                updObj.OldCity = model.OldCity;
//                updObj.OldState = model.OldState;
//                updObj.OldZip = model.OldZip;
//                updObj.OldShipIns = model.OldShipIns;
//                updObj.OldHoldCode = model.OldHoldCode;
//                updObj.OldPayTerms = model.OldPayTerms;
//                updObj.OldCompGroup = model.OldCompGroup;
//                updObj.UniqueAttchID = model.UniqueAttchID;
//                updObj.Attention = model.Attention;
//                updObj.OldAttention = model.OldAttention;
//                updObj.PayAddressSeq = model.PayAddressSeq;
//                updObj.OldPayAddressSeq = model.OldPayAddressSeq;
//                updObj.POAddressSeq = model.POAddressSeq;
//                updObj.OldPOAddressSeq = model.OldPOAddressSeq;
//                updObj.Address2 = model.Address2;
//                updObj.Address2 = model.Address2;
//                updObj.Address2 = model.Address2;
//                updObj.OldAddress2 = model.OldAddress2;
//                updObj.KeyID = model.KeyID;
//                updObj.Country = model.Country;
//                updObj.Country = model.Country;
//                updObj.Country = model.Country;
//                updObj.OldCountry = model.OldCountry;
//                updObj.OrderedBy = model.OrderedBy;

//                db.SaveChanges(modelState);
//            }
//            return updObj;
//        }
        
//        public POBatchHeader Create(POBatchHeader model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            model.BatchSeq = db.POBatchHeaders
//                            .Where(f => f.Co == model.Co && f.Mth == model.Mth && f.BatchId == model.BatchId)
//                            .DefaultIfEmpty()
//                            .Max(f => f == null ? 0 : f.BatchSeq) + 1;

//            db.POBatchHeaders.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }

//        public bool Delete(POBatchHeader model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            db.POBatchHeaders.Remove(model);
//            return db.SaveChanges(modelState) == 0 ? false : true;
//        }


//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~BatchHeaderRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}
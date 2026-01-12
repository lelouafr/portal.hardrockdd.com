using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public partial class BatchHeaderRepository 
    {
        

       

        public static POBatchHeader Init(Batch batch, PORequest request)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var model = new POBatchHeader
            {
                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchTransType = "A",
                PO = request.PO,
                VendorGroupId = request.VendorGroup ?? batch.Co,
                VendorId = request.VendorId,
                Description = request.Description,
                OrderDate = request.OrderedDate,
                OrderedBy = request.OrderedBy.ToString(AppCultureInfo.CInfo()),
                ExpDate = request.OrderedDate,
                Status = 0,
                JCCo = request.JCCo,
                JobId = request.JobId,
                PayTerms = request.Vendor.PayTerms,
                UniqueAttchID = request.UniqueAttchID
            };

            if (request.ApprovedUser != null)
            {
                model.PortalApprover = request.ApprovedUser.Employee.FirstOrDefault().HRRef;
            }
            else
            {
                using var db = new VPContext();
                var user = db.WebUsers.FirstOrDefault(f => f.Id == request.ApprovedBy);
                model.PortalApprover = user.Employee.FirstOrDefault().HRRef;
            }
            return model;
        }
        
        public static int NextId(Batch model)
        {
            using var db = new VPContext();
            return db.POBatchHeaders
                            .Where(f => f.Co == model.Co && f.BatchId == model.BatchId)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.BatchSeq) + 1;
        }

       
    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.Batch.Payment
{
    public static class APPaymentRepository
    {
        public static APPaymentBatchSequence Init(DB.Infrastructure.ViewPointDB.Data.Batch batch, APTran transaction)
        {
            //using var db = new VPContext();
            if (batch == null) throw new ArgumentNullException(nameof(batch));
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            //var APBatches
            var model = new APPaymentBatchSequence
            {
                APCo = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchSeqId = batch.APBatches.DefaultIfEmpty().Max(f => f == null ? 0 : f.BatchSeq) + 1,                
                CMCo = transaction.CMCo,
                CMAcct = (short)transaction.CMAcct,
                PayMethod = "C",
                ChkType = "M",
                CMRef = ((int)transaction.CCTransId).ToString(AppCultureInfo.CInfo()),
                CMRefSeq = 1,
                PaidDate = transaction.CCTransaction.PostDate,
                VendorGroup = transaction.VendorGroupId,
                VendorId = transaction.VendorId,
                Name = transaction.CCTransaction.Merchant.Name,
                Address = transaction.CCTransaction.Merchant.Address,
                City = transaction.CCTransaction.Merchant.City,
                Zip =  transaction.CCTransaction.Merchant.Zip,
                State = transaction.CCTransaction.Merchant.State,
                Country = transaction.CCTransaction.Merchant.CountryCode,
                UniqueAttchID = transaction.UniqueAttchID ?? Guid.NewGuid(),
                Amount = transaction.Amount ?? 0,
                VoidYN ="N",
                Overflow = "N",
                SeparatePayYN = "N",
                PayOverrideYN = "N",
                CCTransId = transaction.CCTransId,

            };

            if (model.Name?.Length > 60)
            {
                model.Name = model.Name.Substring(0, 59);
            }
            return model;
        }
    }
}
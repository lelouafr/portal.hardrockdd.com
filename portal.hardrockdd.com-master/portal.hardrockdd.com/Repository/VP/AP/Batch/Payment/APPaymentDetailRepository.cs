using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.Batch.Payment
{
    public static class APPaymentDetailRepository
    {
        public static APPaymentBatchSequenceTransaction Init(DB.Infrastructure.ViewPointDB.Data.APPaymentBatchSequence sequence, APTran transaction)
        {
            //using var db = new VPContext();
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            //var APBatches
            var model = new APPaymentBatchSequenceTransaction
            {
                APCo = sequence.APCo,
                Mth = sequence.Mth,
                BatchId = sequence.BatchId,
                BatchSeqId = sequence.BatchSeqId,
                ExpMth = transaction.Mth,
                APTrans = transaction.APTransId,
                APRef = transaction.APRef,
                Description = transaction.Description,
                InvDate = transaction.InvDate,
                Gross = transaction.InvTotal,
                Retainage = 0,
                PrevPaid = 0,
                PrevDisc = 0,
                Balance = 0,
                DiscTaken = 0,
                CCTransId = transaction.CCTransId,
            };
            return model;
        }
    }
}
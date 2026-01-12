using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.CreditCard
{
    public static class CreditCardTransactionLogRepository
    {
        public static CreditTransactionLog Init(CreditTransaction transaction, DB.CMLogEnum logType, string description = "")
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            var result = new CreditTransactionLog
            {
                CCCo = transaction.CCCo,
                TransId = transaction.TransId,
                AuditId = transaction.Logs.DefaultIfEmpty().Max(f => f == null ? 0 : f.AuditId) + 1,
                AuditTypeId = (byte)logType,
                LogBy = StaticFunctions.GetUserId(),
                LogDate = DateTime.Now,
                Description = description
            };
            return result;
        }

    }
}
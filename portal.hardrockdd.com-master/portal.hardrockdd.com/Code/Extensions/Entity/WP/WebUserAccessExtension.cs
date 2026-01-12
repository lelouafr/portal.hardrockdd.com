using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Extensions.Entity
{
    public static class WebUserAccessExtension
    {
        public static bool CachedAccessList(this WebUserAccess transaction)
        {
            //if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            //if ((transaction.Merchant.Vendor.IsReceiptRequired ?? true) == false)
            //{
            //    return false;
            //}
            //if ((transaction.Merchant.IsReceiptRequired ?? true) == false)
            //{
            //    return false;
            //}
            return true;
        }
    }
}
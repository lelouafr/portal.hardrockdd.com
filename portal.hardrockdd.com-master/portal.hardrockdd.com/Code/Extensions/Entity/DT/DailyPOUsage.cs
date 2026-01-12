using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyPOUsage
    {
        public void UpdateFromModel(Models.Views.DailyTicket.DailyPOUsageViewModel model)
        {
            if (model == null)
                return;

            var vendorChange = VendorId != model.VendorId;
            VendorId = model.VendorId;
            PO = model.PO;
            Qty = model.Quantity;

            if (vendorChange)
                PO = null;
        }
    }
}
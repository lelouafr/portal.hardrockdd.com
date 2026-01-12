using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DTPayrollHour
    {
        public DB.PayrollEntryStatusEnum Status
        {
            get
            {
                return (DB.PayrollEntryStatusEnum)(this.StatusId ?? 0);
            }
            set
            {
                if (StatusId != (int)value)
                {
                    StatusId = (int)value;
                }
            }
        }
    }
}
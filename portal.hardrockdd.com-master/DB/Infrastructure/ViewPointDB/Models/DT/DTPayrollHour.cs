using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DTPayrollHour
    {
        public PayrollEntryStatusEnum Status
        {
            get
            {
                return (PayrollEntryStatusEnum)(this.StatusId ?? 0);
            }
            set
            {
                if (StatusId != (int)value)
                {
                    StatusId = (int)value;
                }
            }
        }

        public decimal PayHours
        {
            get => (HoursAdj ?? Hours) ?? 0;
        }
    }
}
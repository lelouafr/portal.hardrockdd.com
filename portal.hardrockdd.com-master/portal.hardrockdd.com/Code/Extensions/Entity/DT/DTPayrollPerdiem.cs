using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DTPayrollPerdiem
    {
        public DB.EntryTypeEnum? EntryType
        {
            get
            {
                return (DB.EntryTypeEnum)(this.EntryTypeId ?? 0);
            }
            set
            {
                EntryTypeId = (int)value;
            }
        }

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
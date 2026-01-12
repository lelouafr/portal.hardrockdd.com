using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DTPayrollPerdiem
    {
        public EntryTypeEnum? EntryType
        {
            get
            {
                return (EntryTypeEnum)(this.EntryTypeId ?? 0);
            }
            set
            {
                EntryTypeId = (int)value;
            }
        }

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

		public decimal PayPerDiem
		{
			get => (PerdiemIdAdj ?? PerdiemId) ?? 0;
		}

        public decimal PayAmt
        {
            get => PayPerDiem == 0 ? 0M : PayPerDiem == 2 ? 25M : (Employee.PerDiemRate ?? 0);
		}
	}
}
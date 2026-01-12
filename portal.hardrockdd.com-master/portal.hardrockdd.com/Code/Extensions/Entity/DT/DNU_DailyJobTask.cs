using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DNU_DailyJobTask
    {
        public DateTime? WorkDate
        {
            get
            {
                return tWorkDate;
            }
            set
            {
                if (value != tWorkDate)
                {
                    tWorkDate = value;
                }
            }
        }

        public decimal PayrollValue
        {
            get
            {
                return (ValueAdj ?? Value) ?? 0;
            }
        }

    }
}
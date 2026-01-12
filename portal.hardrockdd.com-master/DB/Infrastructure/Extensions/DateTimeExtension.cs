using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{

    public static class DateTimeExtension
    {
        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        //public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        //{
        //    var delta = dt.Ticks % d.Ticks;
        //    bool roundUp = delta > d.Ticks / 2;
        //    var offset = roundUp ? d.Ticks : 0;

        //    return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        //}

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }

        public static DateTime WeekEndDate(this DateTime currentDate, DayOfWeek dow)
        {
            DateTime firstDayInWeek = currentDate.WeekStartDate(dow);
            var lastDayInWeek = firstDayInWeek.AddDays(6);
            return lastDayInWeek;
        }

        public static DateTime WeekStartDate(this DateTime currentDate, DayOfWeek dow)
        {
            DateTime firstDayInWeek = currentDate.Date;

            while (firstDayInWeek.DayOfWeek != dow)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }
            return firstDayInWeek;
        }
    }
}

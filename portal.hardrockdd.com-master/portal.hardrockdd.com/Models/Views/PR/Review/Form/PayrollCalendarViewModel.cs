using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.Payroll.Review
{
    //public class PayrollCalendarListViewModel
    //{
    //    public PayrollCalendarListViewModel()
    //    {
    //        List = new List<PayrollCalendarViewModel>();
    //    }

    //    public List<PayrollCalendarViewModel> List { get; }
    //}
    public class PayrollCalendarViewModel
    {
        public PayrollCalendarViewModel()
        {

        }
        public PayrollCalendarViewModel(DB.Infrastructure.ViewPointDB.Data.Calendar calendar)
        {
            if (calendar == null)
            {
                throw new System.ArgumentNullException(nameof(calendar));
            }

            Date = calendar.Date;
            WeekDayName = calendar.Date.DayOfWeek.ToString();
            WeekId = (int)calendar.Week;
        }

        [Key]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime Date { get; set; }

        [Display(Name = "WeekId")]
        [UIHint("IntegerBox")]
        public int WeekId { get; set; }

        [Display(Name = "WeekDay")]
        [UIHint("TextBox")]

        public string WeekDayName { get; set; }


    }
}
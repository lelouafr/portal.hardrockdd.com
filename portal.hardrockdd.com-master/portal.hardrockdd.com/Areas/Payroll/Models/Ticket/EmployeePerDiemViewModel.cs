using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Employee
{
    public class PerDiemListViewModel
    {
        public PerDiemListViewModel()
        {

        }

        public PerDiemListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                List = new List<PerDiemViewModel>();
                Week = new List<WeekPerDiemViewModel>();
                return;
			}

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            //Used for Table View
            List = ticket.DailyEmployeePerdiems.Select(s => new PerDiemViewModel(s)).ToList();
			Week = new List<WeekPerDiemViewModel>
			{
				new WeekPerDiemViewModel(ticket)
			};

		}

		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public List<PerDiemViewModel> List { get; }

        public List<WeekPerDiemViewModel> Week { get; }
	}

    public class WeekPerDiemViewModel
    {
        public WeekPerDiemViewModel()
        {
            
        }
        public WeekPerDiemViewModel(DailyTicket ticket)
        {
			if (ticket == null)
				return;

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;
			EmployeeId = ticket.DailyEmployeeTicket.EmployeeId;
			WeekId = ticket.Calendar.Week ?? 0;

			Sunday = DB.PerdiemEnum.No;
			Monday = DB.PerdiemEnum.No;
			Tuesday = DB.PerdiemEnum.No;
			Wednesday = DB.PerdiemEnum.No;
			Thursday = DB.PerdiemEnum.No;
			Friday = DB.PerdiemEnum.No;
			Saturday = DB.PerdiemEnum.No;

			foreach (var item in ticket.DailyEmployeePerdiems)
			{
				var date = (DateTime)item.WorkDate;
				var day = date.DayOfWeek;
				switch (day)
				{
					case DayOfWeek.Sunday:
						Sunday = item.PerDiem;
						break;
					case DayOfWeek.Monday:
						Monday = item.PerDiem;
						break;
					case DayOfWeek.Tuesday:
						Tuesday = item.PerDiem;
						break;
					case DayOfWeek.Wednesday:
						Wednesday = item.PerDiem;
						break;
					case DayOfWeek.Thursday:
						Thursday = item.PerDiem;
						break;
					case DayOfWeek.Friday:
						Friday = item.PerDiem;
						break;
					case DayOfWeek.Saturday:
						Saturday = item.PerDiem;
						break;
					default:
						break;
				}
			}
		}

        public WeekPerDiemViewModel(List<PerDiemViewModel> perDiems)
        {
            if (perDiems == null || perDiems.Count == 0)
                return;

            var firstItem = perDiems.FirstOrDefault();

			DTCo = firstItem.DTCo;
			TicketId = firstItem.TicketId;
			EmployeeId= firstItem.EmployeeId;
			WeekId = firstItem.WeekId ;

			Sunday = DB.PerdiemEnum.No;
			Monday = DB.PerdiemEnum.No;
			Tuesday = DB.PerdiemEnum.No;
			Wednesday = DB.PerdiemEnum.No;
			Thursday = DB.PerdiemEnum.No;
			Friday = DB.PerdiemEnum.No;
			Saturday = DB.PerdiemEnum.No;

			foreach (var item in perDiems)
            {
                var day = item.WorkDate.DayOfWeek;
                switch (day)
                {
                    case DayOfWeek.Sunday:
						Sunday = item.PerDiem;
                        break;
                    case DayOfWeek.Monday:
						Monday = item.PerDiem;
						break;
                    case DayOfWeek.Tuesday:
						Tuesday = item.PerDiem;
						break;
                    case DayOfWeek.Wednesday:
						Wednesday = item.PerDiem;
						break;
                    case DayOfWeek.Thursday:
						Thursday = item.PerDiem;
						break;
                    case DayOfWeek.Friday:
						Friday = item.PerDiem;
						break;
                    case DayOfWeek.Saturday:
						Saturday = item.PerDiem;
						break;
                    default:
                        break;
                }
            }
		}

		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		[HiddenInput]
		public int WeekId { get; set; }

		public int? EmployeeId { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Saturday")]
		public DB.PerdiemEnum Saturday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Sunday")]
		public DB.PerdiemEnum Sunday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Monday")]
		public DB.PerdiemEnum Monday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Tuesday")]
		public DB.PerdiemEnum Tuesday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Wednesday")]
		public DB.PerdiemEnum Wednesday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Thursday")]
		public DB.PerdiemEnum Thursday { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Friday")]
		public DB.PerdiemEnum Friday { get; set; }

		internal WeekPerDiemViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
		{
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId);
			var empTicket = db.DailyEmployeeTickets.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId);
			var calendar = db.Calendars.Where(f => f.Week == this.WeekId).ToList();

			foreach (var day in calendar)
			{
				var updObj = ticket.DailyEmployeePerdiems.FirstOrDefault(f => f.tWorkDate == day.Date);
				updObj ??= empTicket.AddPerdiem(day.Date);

				updObj.PerDiem = day.Date.DayOfWeek switch
				{
					DayOfWeek.Sunday => Sunday,
					DayOfWeek.Monday => Monday,
					DayOfWeek.Tuesday => Tuesday,
					DayOfWeek.Wednesday => Wednesday,
					DayOfWeek.Thursday => Thursday,
					DayOfWeek.Friday => Friday,
					DayOfWeek.Saturday => Saturday,
					_ => DB.PerdiemEnum.No,
				};
			}
			try
			{
				db.BulkSaveChanges();
				return new WeekPerDiemViewModel(empTicket.DailyTicket);
			}
			catch (Exception ex)
			{
				modelState.AddModelError("", ex.Message);
				return this;
			}
		}
	}

    public class PerDiemViewModel
    {
        public PerDiemViewModel()
        {

        }

        public PerDiemViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem entry)
        {
            if (entry == null)
                return;

            DTCo = entry.DTCo;
            TicketId = entry.TicketId;
            LineNum = entry.LineNum;
            WeekId = entry.DailyTicket.Calendar.Week ?? 0;

            WorkDate = (DateTime)entry.WorkDate;
            EmployeeId = entry.EmployeeId;
            PerDiem = entry.PerDiem;
		}

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

		[Key]
		[Required]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		[Key]
        [Required]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int WeekId { get; set; }

		[Required]
        [HiddenInput]
        public DateTime WorkDate { get; set; }

        public int? EmployeeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "PerDiem")]
        [Field(LabelSize = 0, TextSize = 12)]
        public DB.PerdiemEnum PerDiem { get; set; }

        internal PerDiemViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeePerdiems.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/
                //updObj.EmployeeId = EmployeeId;
                updObj.PerDiem = PerDiem;
                try
                {
                    db.BulkSaveChanges();
                    return new PerDiemViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }


}
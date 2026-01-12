using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket
{
	public class StatusLogListViewModel
	{
		public StatusLogListViewModel()
		{

		}

		public StatusLogListViewModel(DailyTicket ticket)
		{
			if (ticket == null)
			{
				List = new List<StatusLogViewModel>();
				return;
			}

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;
			List = ticket.DailyTicketStatusLogs
							   .Where(f => !(f.DailyTicket.CreatedBy == f.CreatedBy && f.Status == (int)DB.DailyTicketStatusEnum.Reviewed))
							   .GroupBy(g => new { g.DTCo, g.TicketId, g.CreatedBy, g.CreatedUser, g.Status, g.CreatedOn.Date })
							   .ToList()
							   .Select(s => new StatusLogViewModel
							   {
								   DTCo = s.Key.DTCo,
								   TicketId = s.Key.TicketId,
								   CreatedUser = s.Key.CreatedUser?.FullName(),
								   Status = (DB.DailyTicketStatusEnum)s.Key.Status,
								   CreatedOn = s.Min(min => min.CreatedOn),
								   Comments = s.Max(max => max.Comments)
							   }
							   )
							   .OrderBy(o => o.CreatedOn)
							   .ToList();
		}

		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public List<StatusLogViewModel> List { get; }
	}

	public class StatusLogViewModel
	{
		public StatusLogViewModel()
		{

		}


		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		[Key]
		[Display(Name = "Line Num")]
		public int LineNum { get; set; }

		[UIHint("TextBox")]
		[Display(Name = "Comments")]
		public string Comments { get; set; }

		[UIHint("DateBox")]
		[Display(Name = "Log Date")]
		public DateTime? CreatedOn { get; set; }

		[UIHint("TextBox")]
		[Display(Name = "Created User")]
		public string CreatedUser { get; set; }

		[UIHint("EnumBox")]
		[Display(Name = "Status")]
		public DB.DailyTicketStatusEnum Status { get; set; }

	}
}
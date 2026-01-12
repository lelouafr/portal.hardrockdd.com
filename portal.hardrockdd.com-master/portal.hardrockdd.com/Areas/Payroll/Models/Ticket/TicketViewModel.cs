using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace portal.Areas.Payroll.Models.Ticket
{
	public class TicketListViewModel
	{
        public TicketListViewModel()
        {
			TimeSelection = DB.TimeSelectionEnum.LastMonth;

		}

        public TicketListViewModel(DB.TimeSelectionEnum timeSelection)
        {
            TimeSelection = timeSelection;
        }

		public TicketListViewModel(List<vDailyTicket> tickets, DB.TimeSelectionEnum timeSelection)
		{
			TimeSelection = timeSelection;
			List = tickets.Select(s => new TicketViewModel(s)).ToList();
		}

		public TicketListViewModel(VPContext db, DB.TimeSelectionEnum timeSelection)
		{
			if (db == null)
			{
				TimeSelection = DB.TimeSelectionEnum.LastMonth;
				List = new List<TicketViewModel>();

				return;
			}

			TimeSelection = timeSelection;
			var filterDate = FilterDate();

			List = db.vDailyTickets
				.Where(f => f.WorkDate >= filterDate)
				.ToList()
				.Select(s => new TicketViewModel(s))
				.ToList();
		}

		public TicketListViewModel(VPContext db, DB.TimeSelectionEnum timeSelection, List<DB.DailyTicketStatusEnum> status)
		{
			if (db == null)
			{
				TimeSelection = DB.TimeSelectionEnum.LastMonth;
				List = new List<TicketViewModel>();

				return;
			}
			TimeSelection = timeSelection;
			var statusList = status.Select(s => (int)s).ToList();
			var filterDate = FilterDate();

			List = db.vDailyTickets
				.Where(f => f.WorkDate >= filterDate &&
							 statusList.Contains((int)f.Status))
				.ToList()
				.Select(s => new TicketViewModel(s))
				.ToList();
		}


		public TicketListViewModel(WebUser user, DB.TimeSelectionEnum timeSelection)
		{
			if (user == null)
			{
				TimeSelection = DB.TimeSelectionEnum.LastMonth;
				List = new List<TicketViewModel>();

				return;
			}

			var filterDate = FilterDate();

			TimeSelection = timeSelection;
			List = user.db.vDailyTickets
				.Where(f => f.WorkDate >= filterDate &&
							f.CreatedBy == user.Id)
				.Select(s => new TicketViewModel(s))
				.ToList();
		}

		private DateTime FilterDate()
		{
			var filterDate = TimeSelection switch
			{
				DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
				DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
				DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
				DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
				DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
				_ => DateTime.Now.AddYears(-99),
			};

			return filterDate;
		}

		public List<TicketViewModel> List { get; }

		[UIHint("EnumBox")]
		[Display(Name = "Time Range")]
		public DB.TimeSelectionEnum TimeSelection { get; set; }
	}

	public class TicketViewModel
	{
        public TicketViewModel()
        {
            
        }

        public TicketViewModel(vDailyTicket vTicket)
        {
			if (vTicket == null)
				return;

			DTCo = vTicket.DTCo;
			TicketId = vTicket.TicketId;
			WorkDate = (DateTime)vTicket.WorkDate;
			Status = (DB.DailyTicketStatusEnum)vTicket.Status;
			StatusClass = StaticFunctions.StatusClass(Status);
			Description = vTicket.Description;
			Comments = vTicket.Comments;
			FormName = vTicket.FormDescription;
			CreatedBy = vTicket.CreatedBy;
			CreatedUser = string.Format("{0} {1}", vTicket.FirstName, vTicket.LastName);
			HasAttachment = vTicket.HasAttachments != 0;

			SearchString = vTicket.SearchString;
		}


        [Key]
		[Required]
		[HiddenInput]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Company #")]
		public int DTCo { get; set; }

		[Key]
		[Required]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Ticket #")]
		public int TicketId { get; set; }

		public string WorkDateString => WorkDate.ToString("MM/dd/yyyy");

		[Display(Name = "Work Date")]
		[UIHint("DateBox")]
		public DateTime WorkDate { get; set; }

		[UIHint("EnumBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Status")]
		public DB.DailyTicketStatusEnum Status { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Status")]
		public string StatusString { get { return Status.ToString(); } }

        public string StatusClass { get; set; }

        [UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Comments")]
		public string Comments { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Form Name")]
		public string FormName { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Created User Id")]
		public string CreatedBy { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Created User")]
		public string CreatedUser { get; set; }

		[Display(Name = "")]
		public bool HasAttachment { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Search String")]
		public string SearchString { get; set; }
	}
}
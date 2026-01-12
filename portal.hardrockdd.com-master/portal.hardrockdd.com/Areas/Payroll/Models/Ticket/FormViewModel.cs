using DB;
using portal.Areas.Payroll.Models.Ticket.Crew;
using System;
using System.ComponentModel.DataAnnotations;

namespace portal.Areas.Payroll.Models.Ticket
{
	public class FormViewModel
	{
        public FormViewModel()
        {
            
        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
			if (ticket == null)
				return;

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;
			UniqueAttchID = ticket.Attachment.UniqueAttchID;
			WorkFlowUsers = new portal.Models.Views.WF.WorkFlowUserListViewModel(ticket.WorkFlow);
			FormType = ticket.FormType;

			switch (ticket.FormType)
			{
				case DB.DTFormEnum.JobFieldTicket:
					JobForm = new Job.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.ProjectManager:
					break;
				case DB.DTFormEnum.TruckingTicket:
					TruckForm = new Truck.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.EmployeeDetailTicket:
					break;
				case DB.DTFormEnum.EmployeeTicket:
					EmployeeForm = new Employee.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.ShopTicket:
					ShopForm = new Shop.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.SubContractorTicket:
					break;
				case DB.DTFormEnum.CrewTicket:
					CrewForm = new Crew.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.TimeOff:
					TimeOffForm = new TimeOff.FormViewModel(ticket);
					break;
				case DB.DTFormEnum.PayrollEntriesTicket:
					break;
				case DB.DTFormEnum.HolidayTicket:
					HolidayForm = new Holiday.FormViewModel(ticket);
					break;
				default:
					break;
			}
		}

		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

        public DTFormEnum FormType { get; set; }

        public Guid UniqueAttchID { get; set; }

		public portal.Models.Views.WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

		public Crew.FormViewModel CrewForm { get; set; }

		public Employee.FormViewModel EmployeeForm { get; set; }

		public General.FormViewModel GeneralForm { get; set; }

		public Holiday.FormViewModel HolidayForm { get; set; }

		public Job.FormViewModel JobForm { get; set; }

		public Shop.FormViewModel ShopForm { get; set; }

		public TimeOff.FormViewModel TimeOffForm { get; set; }

		public Truck.FormViewModel TruckForm { get; set; }

	}
}
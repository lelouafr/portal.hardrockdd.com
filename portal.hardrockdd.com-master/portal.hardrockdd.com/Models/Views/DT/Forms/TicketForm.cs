using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web;

namespace portal.Models.Views.DailyTicket.Form
{
    public class TicketForm
    {
        public TicketForm()
        {


        }

        public TicketForm(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket, bool dynamicLoad = false)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            CanUnSubmit = false;
            CanUnPostTemp = false;
            var userId = ticket.db.CurrentUserId;
            var userEmplId = ticket.db.GetCurrentEmployee().EmployeeId;
            var payrollAccess = HttpContext.Current.User.FormAccess("PayrollReview", "Index");
            switch (ticket.Status)
            {
                case DB.DailyTicketStatusEnum.Deleted:
                    Access = DB.SessionAccess.View;
                    if (ticket.db.CurrentUserId == ticket.CreatedBy ||
                        payrollAccess >= DB.AccessLevelEnum.Write)
                    {
                        CanUnDelete = true;
                    }
                    break;
                case DB.DailyTicketStatusEnum.Rejected:
                    Access = ticket.db.CurrentUserId == ticket.CreatedBy ? DB.SessionAccess.Edit : DB.SessionAccess.View;
                    if (ticket.CreatedBy == ticket.db.CurrentUserId || payrollAccess >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSave = true;
                        CanSubmit = true;
                        CanCancel = true;
                    }
                    else if (ticket.DailyJobTicket?.Job?.Division?.WPDivision?.ManagerId == userEmplId ||
                             ticket.DailyJobTicket?.Job?.Division?.WPDivision?.DivisionManger?.Supervisor?.EmployeeId == userEmplId)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSave = true;
                        CanSubmit = true;
                        CanCancel = true;
                    }
                    else if(ticket.CreatedUser.Employee.FirstOrDefault()?.Supervisor?.WebId == userId ||
                            ticket.CreatedUser.Employee.FirstOrDefault()?.Supervisor?.Supervisor?.WebId == userId ||
                            ticket.db.CurrentUserId == ticket.CreatedBy)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSave = true;
                        CanSubmit = true;
                        CanCancel = true;
                    }


                    break;
                case DB.DailyTicketStatusEnum.Draft:
                    Access = ticket.db.CurrentUserId == ticket.CreatedBy ? DB.SessionAccess.Edit : DB.SessionAccess.View;
                    if (ticket.db.CurrentUserId == ticket.CreatedBy ||
                        payrollAccess >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSave = true;
                        CanSubmit = true;                        
                        CanCancel = true;
                    }
                    break;
                case DB.DailyTicketStatusEnum.UnPosted:
                    if (payrollAccess >= DB.AccessLevelEnum.Write)
                    { 
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;
                        CanApprove = true;
                        CanCancel = true;
                    }
                    break;
                case DB.DailyTicketStatusEnum.Submitted:
                    CanUnSubmit = ticket.db.CurrentUserId == ticket.CreatedBy;
                    if (payrollAccess >= DB.AccessLevelEnum.Write)
                        CanCancel = true;
                    Access = DB.SessionAccess.View;
                    
                    if (ticket.CreatedBy == ticket.db.CurrentUserId || payrollAccess >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanDecline = true;
                    }
                    else if (ticket.DailyJobTicket?.Job?.Division?.WPDivision?.ManagerId == userEmplId ||
                             ticket.DailyJobTicket?.Job?.Division?.WPDivision?.DivisionManger?.Supervisor?.EmployeeId == userEmplId)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanDecline = true;
                    }
                    else if (ticket.CreatedUser.Employee.FirstOrDefault()?.Supervisor?.WebId == userId ||
                            ticket.CreatedUser.Employee.FirstOrDefault()?.Supervisor?.Supervisor?.WebId == userId ||
                            ticket.db.CurrentUserId == ticket.CreatedBy)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanDecline = true;
                    }
                    break;
                case DB.DailyTicketStatusEnum.Approved:
                    Access = DB.SessionAccess.View;
                    if (payrollAccess >= DB.AccessLevelEnum.Write)
                    {
                        CanCancel = true;
                        CanDecline = true;
                        Access = DB.SessionAccess.Edit;
                    }
                    if (payrollAccess == DB.AccessLevelEnum.Full)
                    {
                        CanUnPostTemp = true;
                        Access = DB.SessionAccess.Edit;
                    }
                    break;
                case DB.DailyTicketStatusEnum.Processed:
                    if (payrollAccess >= DB.AccessLevelEnum.Write)
                        CanUnPost = true;
                    Access = DB.SessionAccess.View;
                    break;
                case DB.DailyTicketStatusEnum.Canceled:
                    //if (HttpContext.Current.User.IsFullControl())
                    //    CanCancel = true;
                    break;
                default:
                    Access = DB.SessionAccess.View;
                    break;
            }
            //if (HttpContext.Current.User.IsFullControl())
            //    Access = DB.SessionAccess.Edit;


            if (dynamicLoad)
            {
                switch (ticket.FormId)
                {
                    case 1:
                        DynaimicTicket = new JobTicketFormViewModel(ticket);
                        break;
                    case 3:
                        DynaimicTicket = new TruckTicketFormViewModel(ticket);
                        break;
                    case 5:
                        DynaimicTicket = new EmployeeTicketFormViewModel(ticket);
                        break;
                    case 6:
                        DynaimicTicket = new ShopTicketFormViewModel(ticket);
                        break;
                    case 8:
                        DynaimicTicket = new CrewTicketFormViewModel(ticket);
                        break;
                    case 9:
                        Access = DB.SessionAccess.View;
                        DynaimicTicket = new EmployeeTimeOffTicketFormViewModel(ticket);
                        break;
                    case 11:
                        DynaimicTicket = new HolidayTicketFormViewModel(ticket);
                        break;
                    default:
                        break;
                }

            }

            Form = new DailyFormViewModel(ticket.Form);
        }
        public DB.SessionAccess Access { get; set; }

        public bool CanUnSubmit { get; set; }

        public bool CanSubmit { get; set; }

        public bool CanCancel { get; set; }

        public bool CanSave { get; set; }

        public bool CanApprove { get; set; }

        public bool CanDecline { get; set; }

        public bool CanUnDelete { get; set; }

        public bool CanUnPost { get; set; }

        public bool CanReject { get; set; }

        public bool CanUnPostTemp { get; set; }

        public dynamic DynaimicTicket { get; set; }

        public DailyFormViewModel Form { get; set; }
    }


    public class TicketFormDynamic
    {
        public TicketFormDynamic()
        {


        }

        public TicketFormDynamic(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            CanUnSubmit = false;
            switch ((DB.DailyTicketStatusEnum)ticket.Status)
            {
                case DB.DailyTicketStatusEnum.Rejected:
                case DB.DailyTicketStatusEnum.Deleted:
                    Access = ticket.db.CurrentUserId == ticket.CreatedBy ? DB.SessionAccess.Edit : DB.SessionAccess.View;
                    break;
                case DB.DailyTicketStatusEnum.Draft:
                    Access = DB.SessionAccess.Edit;
                    break;
                case DB.DailyTicketStatusEnum.Submitted:
                    CanUnSubmit = ticket.db.CurrentUserId == ticket.CreatedBy;
                    Access = DB.SessionAccess.View;
                    break;
                case DB.DailyTicketStatusEnum.Approved:
                case DB.DailyTicketStatusEnum.Processed:
                case DB.DailyTicketStatusEnum.Canceled:
                default:
                    Access = DB.SessionAccess.View;
                    break;
            }
            //if (HttpContext.Current.User.IsInRole("Admin"))
            //    Access = DB.SessionAccess.Edit;

            switch (ticket.FormId)
            {
                case 1:
                    DynaimicTicket = new JobTicketFormViewModel(ticket);
                    break;
                case 3:
                    DynaimicTicket = new TruckTicketFormViewModel(ticket);
                    break;
                case 5:
                    DynaimicTicket = new EmployeeTicketFormViewModel(ticket);
                    break;
                case 6:
                    DynaimicTicket = new ShopTicketFormViewModel(ticket);
                    break;
                case 8:
                    DynaimicTicket = new CrewTicketFormViewModel(ticket);
                    break;
                case 9:
                    DynaimicTicket = new EmployeeTimeOffTicketFormViewModel(ticket);
                    break;
                case 11:
                    DynaimicTicket = new HolidayTicketFormViewModel(ticket);
                    break;
                default:
                    break;
            }
        }
        public DB.SessionAccess Access { get; set; }

        public bool CanUnSubmit { get; set; }

        public dynamic DynaimicTicket { get; set; }
    }
}
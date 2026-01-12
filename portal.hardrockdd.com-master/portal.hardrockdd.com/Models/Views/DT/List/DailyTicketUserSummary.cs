using DB.Infrastructure.ViewPointDB.Data;
using portal.Models;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketsUserSummary
    {
        

        public DailyTicketsUserSummary()
        {
            NewTicket = new DailyTicketCreateViewModel();
            Tickets = new List<DailyTicketSummary>();
        }

        public DailyTicketsUserSummary(List<DB.Infrastructure.ViewPointDB.Data.HQCompanyParm> companies)
        {
            if (companies == null) throw new System.ArgumentNullException(nameof(companies));

            using var db = new VPContext();
            Tickets = new List<DailyTicketSummary>();
            var webUsers = db.WebUsers.ToList();
            var dtCodes = companies.GroupBy(g => g.DTCo).Select(s => s.Key).ToList();

            foreach (var dtco in dtCodes)
            {
                //var tickets = db.DailyTickets.Where(f => f.Co == company.HQCo).ToList();
                var vtickets = db.vDailyTickets.Where(f => f.DTCo == dtco).OrderByDescending(o => o.WorkDate).ToList();
                var list = vtickets.Select(s => new DailyTicketSummary(s, webUsers)).ToList();
                Tickets.AddRange(list);
            }
            NewTicket = new DailyTicketCreateViewModel();
        }
        public DailyTicketsUserSummary(WebUser user)
        {
            using var db = new VPContext();
            Tickets = new List<DailyTicketSummary>();
            var webUsers = db.WebUsers.ToList();

            var vtickets = db.vDailyTickets.Where(f => f.CreatedBy == user.Id).OrderByDescending(o => o.WorkDate).ToList();
            var list = vtickets.Select(s => new DailyTicketSummary(s, webUsers)).ToList();
            Tickets.AddRange(list);

            NewTicket = new DailyTicketCreateViewModel();
        }

        public DailyTicketsUserSummary(List<DB.Infrastructure.ViewPointDB.Data.DailyTicket> dailyTickets, List<vDailyTicket> vdailyTickets)
        {
            NewTicket = new DailyTicketCreateViewModel();
            Tickets = dailyTickets.Select(s => new DailyTicketSummary(vdailyTickets.FirstOrDefault(f => f.TicketId == s.TicketId), s)).ToList();
        }

        //public TableViewModel TableInfo { get; set; }

        public List<DailyTicketSummary> Tickets { get; }

        public DailyTicketCreateViewModel NewTicket { get; set; }
    }

    public class DailyTicketSummary : TicketForm
    {

        public DailyTicketSummary()
        {

        }

        public DailyTicketSummary(DB.Infrastructure.ViewPointDB.Data.vDailyTicket vticket, DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket == null ? throw new System.ArgumentNullException(nameof(ticket)) : ticket)
        {
            if (vticket == null) throw new System.ArgumentNullException(nameof(vticket));
            if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));


            DTCo = vticket.DTCo;
            TicketId = vticket.TicketId;
            WorkDate = (DateTime)vticket.WorkDate;
            Status = (DB.DailyTicketStatusEnum)(vticket.Status??0);
            Description = vticket.Description;
            Comments = vticket.Comments;
            FormName = vticket.FormDescription;
            CreatedBy = vticket.CreatedBy;
            HasAttachment = vticket.HasAttachments == 0 ? false : true;

            CreatedUser = new WebUserViewModel(ticket.CreatedUser);
        }

        public DailyTicketSummary(DB.Infrastructure.ViewPointDB.Data.vDailyTicket vticket, List<WebUser> users)// : base(ticket)
        {
            if (vticket == null) throw new System.ArgumentNullException(nameof(vticket));
            if (users == null) throw new System.ArgumentNullException(nameof(users));

            var user = users.FirstOrDefault(f => f.Id == vticket.CreatedBy);
            DTCo = vticket.DTCo;
            TicketId = vticket.TicketId;
            WorkDate = (DateTime)vticket.WorkDate;
            Status = (DB.DailyTicketStatusEnum)vticket.Status;
            Description = vticket.Description;
            Comments = vticket.Comments;
            FormName = vticket.FormDescription;
            CreatedUser = new WebUserViewModel(user);
            CreatedBy = vticket.CreatedBy;
            HasAttachment = vticket.HasAttachments == 0 ? false : true;

        }

        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }


        [Key]
        [Required]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Company #")]
        public int DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Ticket #")]
        public int TicketId { get; set; }

        public string WorkDateString { get { return WorkDate.ToLongDateString(); } }

        [ReadOnly(true)]
        [Display(Name = "Work Date")]
        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "", FormGroupRow = 0, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkDate { get; set; }

        [ReadOnly(true)]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.DailyTicketStatusEnum Status { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public string StatusString { get { return Status.ToString(); } }

        [HiddenInput]
        public bool AllowEdit
        {
            get
            {
                return Status switch
                {
                    DB.DailyTicketStatusEnum.Draft => true,
                    DB.DailyTicketStatusEnum.Rejected => true,
                    DB.DailyTicketStatusEnum.Submitted => false,
                    DB.DailyTicketStatusEnum.Approved => false,
                    DB.DailyTicketStatusEnum.Processed => false,
                    DB.DailyTicketStatusEnum.Canceled => true,
                    DB.DailyTicketStatusEnum.Deleted => true,
                    _ => false,
                };
            }
        }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Form Name")]
        public string FormName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }
        public bool HasAttachment { get; set; }
    }


}

//namespace portal.Repository.Views.DailyTicket
//{
//    public class DailyTicketSummaryRepository //: IDisposable
//    {
//        //private VPContext db = new VPContext();


        

//        //public DailyTicketsUserSummary TicketSummary(string userId, int pageSize, int pageNum, string searchString, out int totalRecords, out int recordsFiltered)
//        //{
//        //    var ticketList = db.vDailyTickets.ToList();
//        //    var userList = db.WebUsers.AsEnumerable().Select(s => new WebUserViewModel(s)).ToList();
//        //    if (!string.IsNullOrEmpty(userId))
//        //    {
//        //        ticketList = ticketList.Where(f => f.CreatedBy == userId).ToList();
//        //        totalRecords = ticketList.Count;
//        //    }
//        //    else
//        //    {
//        //        totalRecords = ticketList.Count;
//        //    }

//        //    if (!string.IsNullOrEmpty(searchString))
//        //    {
//        //        ticketList = ticketList.Where(f => string.Concat(f.Description , " ",
//        //                                                         f.TicketId, " ",
//        //                                                         f.FirstName, " ",
//        //                                                         f.LastName, " ",
//        //                                                         f.FormDescription, " ",
//        //                                                         f.StatusDescription, " ",
//        //                                                         f.WorkDate.Value.ToShortDateString())
//        //                                        .ToLower()
//        //                                        .Contains(searchString.ToLower()))
//        //                                        .ToList();
//        //    }

//        //    recordsFiltered = ticketList.Count;
//        //    pageNum = pageNum == 0 ? 1 : pageNum;
//        //    ticketList = ticketList.OrderByDescending(o => o.WorkDate)
//        //                     .Skip(pageSize * (pageNum - 1))
//        //                     .Take(pageSize)
//        //                     .ToList();

//        //    var results = ticketList.Select(s => new DailyTicketSummary
//        //    {
//        //        Co = s.Co,
//        //        TicketId = s.TicketId,
//        //        WorkDate = (DateTime)s.WorkDate,
//        //        Status = (DB.DailyTicketStatusEnum)s.Status,
//        //        Description = s.Description,
//        //        Comments = s.Comments,
//        //        FormName = s.FormDescription,
//        //        CreatedBy = s.CreatedBy,

//        //    }).ToList();
//        //    foreach (var item in results)
//        //    {
//        //        item.CreatedUser = userList.FirstOrDefault(f => f.Id == item.CreatedBy);
//        //    }
//        //    var tableInfo = new TableViewModel
//        //    {
//        //        CurrentPage = pageNum,
//        //        FilteredRecordCount = recordsFiltered,
//        //        RecordCount = totalRecords,
//        //        SearchString = searchString,
//        //        PageSize = pageSize
//        //    };
//        //    double pageCount = (double)(recordsFiltered / Convert.ToDecimal(pageSize));
//        //    tableInfo.PageCount = (int)Math.Ceiling(pageCount);


//        //    var result = new DailyTicketsUserSummary()
//        //    {
//        //        TableInfo = tableInfo,                
//        //    };
//        //    result.Tickets.AddRange(results);
//        //    return result;
//        //}


//        //public void Dispose()
//        //{
//        //    Dispose(true);
//        //    GC.SuppressFinalize(this);
//        //}

//        //~DailyTicketSummaryRepository()
//        //{
//        //    // Finalizer calls Dispose(false)
//        //    Dispose(false);
//        //}

//        //protected virtual void Dispose(bool disposing)
//        //{
//        //    if (disposing)
//        //    {
//        //        if (db != null)
//        //        {
//        //            db.Dispose();
//        //            db = null;
//        //        }
//        //    }
//        //}

//    }
//}
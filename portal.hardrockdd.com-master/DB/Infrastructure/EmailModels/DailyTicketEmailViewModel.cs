using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.EmailModels
{
    public class DailyTicketEmailViewModel
    {

        public DailyTicketEmailViewModel()
        {

        }

        public DailyTicketEmailViewModel(DailyTicket ticket)
        {
            if (ticket == null)
                return;

            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            Comments = ticket.WorkFlow.CurrentSequence().Comments;
        }

        public byte DTCo { get; set; }

        public int TicketId { get; set; }

        public string Comments { get; set; }
    }
}

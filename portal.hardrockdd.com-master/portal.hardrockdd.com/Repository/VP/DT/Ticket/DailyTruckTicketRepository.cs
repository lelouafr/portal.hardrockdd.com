//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.DT
//{
//    public partial class DailyTruckTicketRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public DailyTruckTicketRepository()
//        {

//        }

//        public static DailyTruckTicket Init(DailyTicket ticket, VPContext db)
//        {
//            if (ticket == null)
//                throw new ArgumentNullException(nameof(ticket));
//            if (db == null)
//                throw new ArgumentNullException(nameof(db));

//            var crewLeader = StaticFunctions.GetCurrentEmployee();
//            var crew = db.Crews.FirstOrDefault(f => f.PRCo == crewLeader.PRCo && f.CrewLeaderID == crewLeader.EmployeeId);

//            var result = new DailyTruckTicket
//            {
//                DTCo = ticket.DTCo,
//                TicketId = ticket.TicketId,
//                tWorkDate = ticket.WorkDate,
//                tCrewId = crew?.CrewId,
//                DailyTicket = ticket,
//            };

//            return result;
//        }

//        public static void GenerateEmployeeList(Crew crew, DailyTicket ticket)
//        {
//            if (ticket == null)
//            {
//                throw new System.ArgumentNullException(nameof(ticket));
//            }
//            if (crew == null)
//            {
//                throw new System.ArgumentNullException(nameof(crew));
//            }
//            if (crew.CrewLeader != null)
//            {
//                foreach (var emp in ticket.DailyEmployeeEntries.ToList())
//                {
//                    ticket.DailyEmployeeEntries.Remove(emp);
//                }
//                foreach (var emp in ticket.DailyEmployeePerdiems.ToList())
//                {
//                    ticket.DailyEmployeePerdiems.Remove(emp);
//                }
//                foreach (var emp in crew.CrewLeader.DirectReports.Where(f => f.ActiveYN == "Y").ToList())
//                {
//                    var newEmp = DailyEmployeeEntryRepository.Init(ticket, emp);
//                    ticket.DailyEmployeeEntries.Add(newEmp);
//                }
//                ticket.DailyEmployeeEntries.Add(DailyEmployeeEntryRepository.Init(ticket, crew.CrewLeader));
//                foreach (var emp in ticket.DailyEmployeeEntries)
//                {
//                    var perdiem = DailyEmployeePerdiemRepository.Init(ticket, emp);
//                    ticket.DailyEmployeePerdiems.Add(perdiem);
//                    emp.PerdiemLineNum = perdiem.LineNum;
//                }
//            }
//        }

        
//        public Models.Views.DailyTicket.DailyTruckTicketViewModel ProcessUpdate(Models.Views.DailyTicket.DailyTruckTicketViewModel model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var updObj = db.DailyTruckTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
//            if (updObj != null)
//            {
//                model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)updObj.WorkDate : model.WorkDate;
//                var dateChanged = updObj.WorkDate != model.WorkDate;

//                /****Write the changes to object****/
//                updObj.CrewId = model.CrewId;
//                updObj.Comments = model.Comments;

//                if (dateChanged)
//                {
//                    updObj.WorkDate = model.WorkDate;
//                    updObj.DailyTicket.WorkDate = model.WorkDate;
//                    updObj.DailyTicket.DailyEmployeeEntries.ToList().ForEach(e => e.WorkDate = updObj.WorkDate);
//                    updObj.DailyTicket.DailyEmployeePerdiems.ToList().ForEach(e => e.WorkDate = updObj.WorkDate);
//                }
//                db.SaveChanges(modelState);
//            }
//            return new Models.Views.DailyTicket.DailyTruckTicketViewModel(updObj);
//        }

       
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~DailyTruckTicketRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}
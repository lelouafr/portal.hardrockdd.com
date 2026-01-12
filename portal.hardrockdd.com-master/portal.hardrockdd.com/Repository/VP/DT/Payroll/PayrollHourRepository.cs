//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.DT
//{
//    public partial class PayrollHourRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public PayrollHourRepository()
//        {

//        }
        
//        public Models.Views.Payroll.PayrollHourReviewViewModel ProcessUpdate(Models.Views.Payroll.PayrollHourReviewViewModel model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            var updObjs = db.DTPayrollHours
//                        .Where(f => f.PRCo == model.PRCo && 
//                                    f.EmployeeId == model.EmployeeId && 
//                                    f.WorkDate == model.WorkDate && 
//                                    f.JobId == model.JobId && 
//                                    f.EquipmentId == model.EquipmentId)
//                        .ToList();
//            var totalHours = updObjs.Sum(sum => sum.Hours);
//            foreach (var updObj in updObjs)
//            {
//                var calHour = totalHours != 0 ? (updObj.Hours / totalHours) * model.Hour : model.Hour;

//                /****Write the changes to object****/
//                updObj.HoursAdj = calHour;

//                //if (updTicket)
//                {
//                    var ticket = db.DailyTickets.FirstOrDefault(w => w.DTCo == updObj.DTCo && w.TicketId == updObj.TicketId);
//                    foreach (var hour in ticket.DailyEmployeeEntries.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
//                    {
//                        hour.ValueAdj = updObj.HoursAdj;
//                    }
//                    foreach (var hour in ticket.DailyJobEmployees.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
//                    {
//                        if (totalHours != 0)
//                        {
//                            hour.HoursAdj = (hour.Hours / totalHours) * model.Hour;
//                        }
//                        else
//                        {
//                            hour.HoursAdj = model.Hour;
//                        }
//                    }
//                }
//            }
//            db.SaveChanges(modelState);
//            return new Models.Views.Payroll.PayrollHourReviewViewModel(updObjs);
//        }
        
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~PayrollHourRepository()
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
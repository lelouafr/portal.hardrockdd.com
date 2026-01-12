//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.DT
//{
//    public partial class PayrollPerdiemRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public PayrollPerdiemRepository()
//        {

//        }
        
//        //public static DTPayrollPerdiem Init(DailyJobTask task, DailyJobEmployee employee)
//        //{
//        //    if (employee == null)
//        //        throw new ArgumentNullException(nameof(employee));

//        //    if (task == null)
//        //        throw new ArgumentNullException(nameof(task));

//        //    var model = new DTPayrollPerdiem
//        //    {
//        //        PRCo = employee.PREmployee.PRCo,
//        //        EmployeeId = employee.PREmployee.EmployeeId,
//        //        Employee = employee.PREmployee,
//        //        EarnCodeId = 20,

//        //        WorkDate = (DateTime)employee.WorkDate,
//        //        DTCo = task.DTCo,
//        //        TicketId = task.TicketId,
//        //        TicketLineNum = employee.LineNum,
//        //        DailyTicket = employee.DailyTicket,

//        //        Job = task.Job,
//        //        JCCo = task.Job.JCCo,
//        //        JobId = task.Job.JobId,
//        //        PhaseGroupId = task.PhaseGroupId ?? task.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
//        //        PhaseId = task.PhaseId,

//        //        PerdiemId = (int?)employee.PerdiemAdj ?? employee.Perdiem,
//        //        ModifiedOn = employee.DailyTicket.ModifiedOn,
//        //        ModifiedBy = employee.DailyTicket.ModifiedBy,
//        //        Status = DB.PayrollEntryStatusEnum.Accepted
//        //    };
//        //    return model;
//        //}

//        //public static DTPayrollPerdiem Init(DailyJobEmployee employee)
//        //{
//        //    if (employee == null)
//        //        throw new ArgumentNullException(nameof(employee));

//        //    var model = new DTPayrollPerdiem
//        //    {
//        //        PRCo = employee.PREmployee.PRCo,
//        //        EmployeeId = (int)employee.EmployeeId,
//        //        Employee = employee.PREmployee,

//        //        WorkDate = (DateTime)employee.WorkDate,
//        //        DTCo = employee.DTCo,
//        //        TicketId = employee.TicketId,
//        //        TicketLineNum = employee.LineNum,
//        //        DailyTicket = employee.DailyTicket,

//        //        EarnCodeId = 20,

//        //        Job = employee.DailyTicket.DailyJobTicket.Job,
//        //        JCCo = employee.DailyTicket.DailyJobTicket.Job.JCCo,
//        //        JobId = employee.DailyTicket.DailyJobTicket.Job.JobId,
//        //        PhaseGroupId = employee.DailyTicket.DailyJobTicket.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
//        //        PhaseId = "   100-  -",

//        //        PerdiemId = (int?)employee.PerdiemAdj ?? employee.Perdiem,
//        //        ModifiedOn = employee.DailyTicket.ModifiedOn,
//        //        ModifiedBy = employee.DailyTicket.ModifiedBy,
//        //        Status = (int)DB.PayrollEntryStatusEnum.Accepted
//        //    };
//        //    return model;
//        //}

//        //public static DTPayrollPerdiem Init(DailyEmployeePerdiem perdeim, DailyEmployeeEntry entry)
//        //{
//        //    if (perdeim == null)
//        //        throw new ArgumentNullException(nameof(perdeim));

//        //    if (entry == null)
//        //        throw new ArgumentNullException(nameof(entry));

//        //    var model = new DTPayrollPerdiem
//        //    {
//        //        PRCo = perdeim.PREmployee.PRCo,
//        //        //model.PRCo = perdeim.PREmployee.PRCo;
//        //        EmployeeId = (int)perdeim.PREmployee.EmployeeId,
//        //        Employee = entry.PREmployee,

//        //        WorkDate = (DateTime)perdeim.WorkDate,
//        //        DTCo = perdeim.DTCo,
//        //        TicketId = perdeim.TicketId,
//        //        TicketLineNum = perdeim.LineNum,
//        //        EarnCodeId = 20,

//        //        PerdiemId = perdeim.PerDiemIdAdj ?? perdeim.PerDiemId,
//        //        ModifiedOn = entry.DailyTicket.ModifiedOn,
//        //        ModifiedBy = entry.DailyTicket.ModifiedBy,
//        //        Status = (int)DB.PayrollEntryStatusEnum.Accepted
//        //    };

//        //    if (entry.Job != null)
//        //    {

//        //        if (entry.Job != null ? entry.JobId.Contains("99-95") : false)
//        //        {
//        //            model.Job = null;
//        //            model.JCCo = null;
//        //            model.JobId = null;
//        //            model.PhaseId = null;
//        //            model.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
//        //        }
//        //        else
//        //        {
//        //            model.Job = entry.Job;
//        //            model.JCCo = entry.Job.JCCo;
//        //            model.JobId = entry.Job.JobId;
//        //            model.PhaseGroupId = entry.PhaseGroupId ?? entry.Job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
//        //            model.PhaseId = entry.PhaseId;
//        //            model.EntryTypeId = (int)DB.EntryTypeEnum.Job;
//        //        }
//        //    }
//        //    if (entry.Equipment != null)
//        //    {
//        //        model.Equipment = entry.Equipment;
//        //        model.EMCo = entry.Equipment.EMCo;
//        //        model.EquipmentId = entry.Equipment.EquipmentId;
//        //        model.EntryTypeId = (int)DB.EntryTypeEnum.Equipment;
//        //    }

                        
//        //    //Correct for missing phase id
//        //    if (entry.DailyTicket.FormId == (int)DB.DTFormEnum.CrewTicket && model.JobId == null)
//        //    {
//        //        model.JobId = entry.DailyTicket.DailyShopTicket.JobId;
//        //        if (model.PhaseId == null)
//        //            model.PhaseId = "   100-  -";
//        //    }

//        //    return model;
//        //}

//        //public static DTPayrollPerdiem Init(DailyEmployeePerdiem perdeim)
//        //{
//        //    if (perdeim == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(perdeim));
//        //    }
//        //    var model = new DTPayrollPerdiem
//        //    {
//        //        PRCo = perdeim.PREmployee.PRCo,
//        //        EmployeeId = (int)perdeim.EmployeeId,
//        //        WorkDate = (DateTime)perdeim.WorkDate,

//        //        DailyTicket = perdeim.DailyTicket,
//        //        DTCo = perdeim.DTCo,
//        //        TicketId = perdeim.TicketId,
//        //        TicketLineNum = perdeim.LineNum,
//        //        EarnCodeId = 20,

//        //        //model.PhaseGroupId = perdeim.;
//        //        PerdiemId = perdeim.PerDiemIdAdj ?? perdeim.PerDiemId,
//        //        ModifiedOn = perdeim.DailyTicket.ModifiedOn,
//        //        ModifiedBy = perdeim.DailyTicket.ModifiedBy,
//        //        Status = DB.PayrollEntryStatusEnum.Accepted
//        //    };

//        //    return model;
//        //}

//        public Models.Views.Payroll.PayrollPerdiemReviewViewModel ProcessUpdate(Models.Views.Payroll.PayrollPerdiemReviewViewModel model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//                throw new ArgumentNullException(nameof(model));

//            var updObjs = db.DTPayrollPerdiems
//                        .Where(f => f.PRCo == model.PRCo && 
//                                    f.EmployeeId == model.EmployeeId && 
//                                    f.WorkDate == model.WorkDate && 
//                                    f.JobId == model.JobId && 
//                                    f.EquipmentId == model.EquipmentId)
//                        .ToList();

//            foreach (var updObj in updObjs)
//            {
//                var updTicket = updObj.PerdiemIdAdj != (int)model.PerdiemId;
//                /****Write the changes to object****/
//                updObj.PerdiemIdAdj = (int)model.PerdiemId;

//                if (updTicket)
//                {
//                    var ticket = db.DailyTickets.FirstOrDefault(w => w.DTCo == updObj.DTCo && w.TicketId == updObj.TicketId);
//                    foreach (var hour in ticket.DailyEmployeePerdiems.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
//                    {
//                        hour.PerDiemIdAdj = updObj.PerdiemIdAdj;
//                    }
//                    foreach (var hour in ticket.DailyJobEmployees.Where(w => w.LineNum == updObj.TicketLineNum).ToList())
//                    {
//                        hour.PerdiemAdj = updObj.PerdiemIdAdj;
//                    }
//                }
//            }
//            db.SaveChanges(modelState);
//            return new Models.Views.Payroll.PayrollPerdiemReviewViewModel(updObjs);
//        }
                
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~PayrollPerdiemRepository()
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyJobTicket
    {
        private VPContext db
        {
            get
            {

                var db = VPContext.GetDbContextFromEntity(this);

                if (db == null && this.DailyTicket != null)
                    db = this.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public string JobId
        {
            get
            {
                return tJobId;
            }
            set
            {
                if (value != tJobId)
                {
                    UpdateJobInfo(value);
                }
            }
        }

        public string CrewId
        {
            get
            {
                return tCrewId;
            }
            set
            {
                if (value != tCrewId)
                {
                    UpdateCrewInfo(value);
                }
            }
        }

        public string RigId
        {
            get
            {
                return tRigId;
            }
            set
            {
                if (value != tRigId)
                {
                    UpdateRigInfo(value);
                }
            }
        }

        public DateTime? WorkDate
        {
            get
            {
                return tWorkDate;
            }
            set
            {
                if (value != tWorkDate)
                {
                    DailyTicket.WorkDate = value;
                    tWorkDate = value;
                    if (value != null)
                        DTJobPhases.ToList().ForEach(e => e.WorkDate = (DateTime)value);
                }
            }
        }

        public List<DTJobPhaseCost> PayrollHourTasks()
        {
            return this.DTJobPhases.SelectMany(s => s.DTCostLines).Where(f => f.JobPhaseCost.UM == "HRS" && f.JobPhaseCost.CostType.JBCostTypeCategory == "L").ToList();

            //return DailyTicket.DailyJobTasks.Where(f => f.JobPhaseCost.UM == "HRS" && f.JobPhaseCost.CostType.JBCostTypeCategory == "L").ToList();
        }

        public void GenerateEmployeeList()
        {
            if (DailyTicket == null)
                return;

            //Remove current employees from ticket
            DailyTicket.DailyJobEmployees.ToList().ForEach(emp => DailyTicket.DailyJobEmployees.Remove(emp));

            if (Crew == null)
                return;

            if (Crew.CrewLeader != null)
            {
                var employeeList = Crew.CrewLeader.DirectReports.Where(f => f.ActiveYN == "Y").ToList();
                if (employeeList.Count == 0)
                {
                    var oldTicket = db.DailyJobTickets.Where(f => f.PRCo == Crew.PRCo && f.tCrewId == Crew.CrewId && f.TicketId != TicketId).OrderByDescending(o => o.tWorkDate).FirstOrDefault();
                    if (oldTicket != null)
                    {
                        var lastTicketEmployeeList = oldTicket.DailyTicket.DailyJobEmployees.Select(s => s.PREmployee).Where(f => f.EmployeeId != Crew.CrewLeader.EmployeeId).ToList();
                        employeeList.AddRange(lastTicketEmployeeList);
                    }
                }

                employeeList.Add(Crew.CrewLeader);
                employeeList.ForEach(emp => AddEmployee(emp));
            }
        }

        private void UpdateJobInfo(string newJobId)
        {
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                Job = job;
            }

            if (tJobId != newJobId)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == newJobId);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    Job = job;

                    DTJobPhases.ToList().ForEach(e => {
                        e.JCCo = job.JCCo;
                        e.JobId = job.JobId;
                        e.Job = job;
                    });
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    Job = null;
                }
            }
        }

        private void UpdateCrewInfo(string newCrewId)
        {

            if (Crew == null && tCrewId != null)
            {
                var crew = db.Crews.FirstOrDefault(f => f.CrewId == tCrewId);
                tCrewId = crew.CrewId;
                PRCo = crew.PRCo;
                Crew = crew;
            }

            if (tCrewId != newCrewId)
            {
                var crew = db.Crews.FirstOrDefault(f => f.CrewId == newCrewId);
                if (crew != null)
                {
                    tCrewId = crew.CrewId;
                    PRCo = crew.PRCo;
                    Crew = crew;
                }
                else
                {
                    tCrewId = null;
                    PRCo = null;
                    Crew = null;
                }
                GenerateEmployeeList();
            }
        }

        private void UpdateRigInfo(string newRigId)
        {

            if (Rig == null && tRigId != null)
            {
                var rig = db.Equipments.FirstOrDefault(f => f.EquipmentId == tRigId);
                tRigId = rig.EquipmentId;
                EMCo = rig.EMCo;
                Rig = rig;
            }

            if (tRigId != newRigId)
            {
                var rig = db.Equipments.FirstOrDefault(f => f.EquipmentId == newRigId);
                if (rig != null)
                {
                    tRigId = rig.EquipmentId;
                    EMCo = rig.EMCo;
                    Rig = rig;
                }
                else
                {
                    tRigId = null;
                    EMCo = null;
                    Rig = null;
                }
            }
        }

        public void UpdateEmployeesReportTo()
        {
            if (Crew?.CrewLeader == null)
                return;

            var crewLeader = Crew?.CrewLeader;

            var employeeList = DailyTicket.DailyJobEmployees
                .Select(s => s.PREmployee)
                .Distinct()
                .Where(f => f.EmployeeId != crewLeader.EmployeeId)
                .ToList();
            foreach (var emp in employeeList)
            {
                if (emp.HREmployee.Position.AutoAssignOrg == "Y")
                {
                    if (Job?.Division?.WPDivision != null)
                        if (Job?.Division?.WPDivision?.DivisionId != emp.DivisionId)
                        {
                            emp.Division = null;
                            emp.DivisionId = Job.Division.WPDivision.DivisionId;
                            emp.OfficeId = Job.Division.WPDivision.OfficeId;
                            emp.db.SaveChanges();
                        }

                    if (emp.ReportsToId != crewLeader.EmployeeId)
                    {
                        emp.Supervisor = null;
                        emp.ReportsToId = crewLeader.EmployeeId;
                        emp.db.SaveChanges();
                    }
                }
            }

            if (Job?.Division?.WPDivision != null)
            {
                if (crewLeader.DivisionId != Job.Division.WPDivision.DivisionId)
                {
                    crewLeader.Division = null;
                    crewLeader.DivisionId = Job.Division.WPDivision.DivisionId;
                    crewLeader.OfficeId = Job.Division.WPDivision.OfficeId;
                    crewLeader.db.SaveChanges();
                }

                if (crewLeader.ReportsToId != Job.Division.WPDivision.ManagerId)
                {
                    crewLeader.Supervisor = null;
                    crewLeader.ReportsToId = Job.Division.WPDivision.ManagerId;
                    crewLeader.db.SaveChanges();
                }
            }

        }

        public DTJobPhase AddJobPhase(string phaseId = "", int? taksId = null)
        {
            if (Job == null)
            {
                return null;
            }
            var dailyJobPhase = DTJobPhases.FirstOrDefault(f => f.PhaseId == phaseId && f.Hours == 0);
            if (dailyJobPhase == null)
            {
                dailyJobPhase = new DTJobPhase()
                {
                    DTCo = this.DTCo,
                    TicketId = this.TicketId,
                    TaskId = taksId ?? DTJobPhases.DefaultIfEmpty().Max(max => max == null ? 0 : max.TaskId) + 1,
                    JCCo = (byte)this.JCCo,
                    tJobId = this.JobId,
                    tWorkDate = (DateTime)this.WorkDate,
                    PassId = 0,

                    Job = this.Job,
                    JobTicket = this,
                };

                if (!string.IsNullOrEmpty(phaseId))
                    dailyJobPhase.PhaseId = phaseId;

                DTJobPhases.Add(dailyJobPhase);
                Job.DTJobPhases.Add(dailyJobPhase);
            }
            else
            {
                dailyJobPhase.JobPhase.AddMasterPhaseCosts();
                dailyJobPhase.UpdateProgressPhase();
                dailyJobPhase.UpdateProgressBudgetCode();
            }

            if (dailyJobPhase?.JobPhase?.JobPhaseCosts != null)
            {
                var costTypes = new int[] { 1, 2, 3, 13, 31, 32, 34, 35 };//2, 
				var costType = dailyJobPhase.JobPhase.JobPhaseCosts.Where(f => f.ActiveYN == "Y" && costTypes.Contains(f.CostTypeId)).ToList();
                foreach (var cost in costType)
                {
                    if (cost.UM != "EA" && cost.UM != "DYS")
                        dailyJobPhase.AddCostLine(cost.CostTypeId);

                }
            }

            return dailyJobPhase;
        }

        public void DeleteJobPhase(DTJobPhase dtJobPhase)
        {
            foreach (var cost in dtJobPhase.DTCostLines.ToList())
            {
                dtJobPhase.DTCostLines.Remove(cost);
            }
            dtJobPhase.UpdateProgressPhase();
            dtJobPhase.UpdateJobActualProgress();
            DTJobPhases.Remove(dtJobPhase);
        }

        public DailyPOUsage AddPOUsage()
        {
            var result = new DailyPOUsage
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineId = DailyTicket.POUsages.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                JobId = JobId,
                JCCo = JCCo,

                WorkDate = WorkDate,
                Qty = 0,
                Hours = 0,
            };
            DailyTicket.POUsages.Add(result);

            return result;
        }

        public DailyJobEmployee AddEmployee()
        {
            var result = new DailyJobEmployee
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyTicket.DailyJobEmployees.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = WorkDate,
                Hours = null,
                Perdiem = (int)PerdiemEnum.No,
                DailyTicket = this.DailyTicket,
            };
            DailyTicket.DailyJobEmployees.Add(result);

            return result;
        }

        public DailyJobEmployee AddEmployee(Employee employee)
        {
            var result = new DailyJobEmployee
            {
                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyTicket.DailyJobEmployees.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                tWorkDate = WorkDate,
                Hours = null,
                Perdiem = (int)PerdiemEnum.No,

                DailyTicket = this.DailyTicket,
            };

            if (employee != null)
            {
                result.PRCo = employee.PRCo;
                result.tEmployeeId = employee.EmployeeId;
                result.PREmployee = employee;
            }

            DailyTicket.DailyJobEmployees.Add(result);

            return result;
        }

        //public void UpdateFromModel(Models.Views.DailyTicket.DailyJobTicketViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)WorkDate : model.WorkDate;

        //    WorkDate = model.WorkDate;
        //    JobId = model.JobId;
        //    RigId = model.RigId;
        //    CrewId = model.CrewId;

        //    GroundCondition = model.GroundCondition;
        //    WeatherCondition = model.WeatherCondition;
        //    RainOut = model.RainOut;
        //    MudMotor = model.MudMotor;
        //    Comments = model.Comments;

        //}

        public void CalculateRigUtilization()
        {
            var rig = this.Rig;
            var dtTasks = db.DTJobPhases.Where(f => f.tWorkDate == WorkDate &&
                                                    f.JobTicket.tRigId == RigId &&
                                                    f.ProgressPhaseId != null &&
                                                    (f.JobTicket.DailyTicket.tStatusId == (int)DailyTicketStatusEnum.Submitted ||
                                                    f.JobTicket.DailyTicket.tStatusId == (int)DailyTicketStatusEnum.Approved ||
                                                    f.JobTicket.DailyTicket.tStatusId == (int)DailyTicketStatusEnum.Processed))
                                        .ToList();// && f.PhaseGroupId == PhaseGroupId && f.ProgressPhaseId == JCPhaseId
            var dtUsages = db.DailyEquipmentUsages.Where(f => f.ActualDate == WorkDate && f.EquipmentId == RigId).ToList();
            dtUsages.ForEach(eqp => {
                eqp.RevTimeUnits = 0;
                eqp.RevWorkUnits = 0;
                eqp.RevDollars = 0;
            });

            var totalHours = dtTasks.Sum(s => s.Hours);
            var totalUsage = 0m;
            var acumHours = 0m;
            foreach (var dtTask in dtTasks)
            {
                var hours = dtTask.Hours;
                acumHours += hours;
                if (hours > 0 && totalHours > 0)
                {
                    var unitCal = hours / totalHours;
                    var usage = dtTask.GetEquipmentUsage();

                    unitCal = Math.Round(unitCal, 3);
                    usage.RevWorkUnits += unitCal;
                    usage.RevTimeUnits += hours;
                    //usage.RevWorkUnits ??= 0;
                    //usage.RevWorkUnits += unitCal;
                    usage.RevDollars = usage.RevRate * usage.RevWorkUnits;
                    totalUsage += unitCal;

                    //db.BulkSaveChanges();
                }
            }
            if (totalUsage != 1m && dtTasks.Any())
            {
                var dif = 1 - totalUsage;
                var firstUsage = dtTasks.FirstOrDefault().GetEquipmentUsage();

                if (firstUsage != null)
                {
                    firstUsage.RevWorkUnits += dif;
                    firstUsage.RevDollars = firstUsage.RevRate * firstUsage.RevWorkUnits;
                }
            }
        }
    }
}
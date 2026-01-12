using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyTruckTicket
    {
        private VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.DailyTicket != null)
                    db = this.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
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
                }
            }
        }

        private Crew _crew;
        public Crew Crew
        {
            get
            {
                if (_crew == null)
                {
                    var db = VPEntities.GetDbContextFromEntity(this);
                    if (db != null)
                    {
                        _crew = db.Crews.FirstOrDefault(f => f.PRCo == PRCo && f.CrewId == CrewId);
                    }
                }
                return _crew;
            }
            set
            {
                _crew = value;
                PRCo = _crew?.PRCo;
                tCrewId = _crew?.CrewId;
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

        public void UpdateCrewInfo(string newCrewId)
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
                    GenerateEmployeeList();
                }
                else
                {
                    tCrewId = null;
                    PRCo = null;
                    Crew = null;
                }
            }
        }

        public void GenerateEmployeeList()
        {

            if (DailyTicket == null)
                return;


            foreach (var emp in DailyTicket.DailyEmployeeEntries.ToList())
            {
                DailyTicket.DailyEmployeeEntries.Remove(emp);
            }

            foreach (var emp in DailyTicket.DailyEmployeePerdiems.ToList())
            {
                DailyTicket.DailyEmployeePerdiems.Remove(emp);
            }

            if (Crew == null)
                return;

            var lastTicket = db.DailyTruckTickets.Where(f =>
                                                            f.TicketId != TicketId &&
                                                            f.tCrewId == CrewId &&
                                                            f.DailyTicket.CreatedBy == DailyTicket.CreatedBy &&
                                                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Draft &&
                                                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                            f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Rejected)
                                                .OrderByDescending(f => f.tWorkDate)
                                                .FirstOrDefault();

            if (lastTicket == null)
                lastTicket = db.DailyTruckTickets.Where(f =>
                                                        f.TicketId != TicketId &&
                                                        f.tCrewId == CrewId &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Draft &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Rejected)
                                            .OrderByDescending(f => f.tWorkDate)
                                            .FirstOrDefault();

            if (lastTicket != null)
            {
                var employeeList = lastTicket.DailyTicket.DailyEmployeeEntries.GroupBy(g => g.PREmployee).Select(s => s.Key).Where(f => f.ActiveYN == "Y").ToList();
                foreach (var emp in employeeList)
                {
                    var entry = DailyTicket.DailyTruckTicket.AddHoursEntry(emp);
                    var perdiem = DailyTicket.DailyTruckTicket.AddPerdiem(emp);
                    entry.PerdiemLineNum = perdiem.LineNum;
                }
            }

        }

        public void UpdateEmployeesReportTo()
        {
            var CrewLeader = Crew?.CrewLeader;
            if (CrewLeader != null)
            {
                var employeeList = DailyTicket.DailyEmployeeEntries.Select(s => s.PREmployee).Distinct().Where(f => f.EmployeeId != CrewLeader.EmployeeId).ToList();

                foreach (var emp in employeeList)
                {
                    if (emp.HREmployee.Position.AutoAssignOrg == "Y" && emp.ReportsToId != CrewLeader.EmployeeId)
                        emp.ReportsToId = CrewLeader.EmployeeId;
                }
            }
        }


        public DailyEmployeeEntry AddHoursEntry()
        {
            var entry = DailyTicket.AddHoursEntry();
            return entry;
        }

        public DailyEmployeeEntry AddHoursEntry(Employee employee)
        {
            var entry = DailyTicket.AddHoursEntry(employee);
            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem()
        {
            return DailyTicket.AddPerdiem();
        }

        public DailyEmployeePerdiem AddPerdiem(Employee employee)
        {
            return DailyTicket.AddPerdiem(employee);
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
        {
            if (model == null)
                return;

            model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)WorkDate : model.WorkDate;

            /****Write the changes to object****/
            CrewId = model.CrewId;
            Comments = model.Comments;
            WorkDate = model.WorkDate;
        }

    }
}
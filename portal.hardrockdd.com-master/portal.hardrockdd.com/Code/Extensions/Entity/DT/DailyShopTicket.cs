using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{

    public partial class DailyShopTicket
    {
        private VPEntities _db;
        public VPEntities db
        {
            get
            {
                _db ??= this.DailyTicket?.db;
                _db ??= VPEntities.GetDbContextFromEntity(this);

                return _db;
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

        public void UpdateJobInfo(string newJobId)
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
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    Job = null;
                }

                DailyTicket.DailyEmployeeEntries.ToList().ForEach(e => {
                    e.JCCo = JCCo;
                    e.JobId = tJobId;
                });
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
                    if (!string.IsNullOrEmpty(crew.JobId))
                    {
                        JCCo = crew.JCCo;
                        tJobId = crew.JobId;
                    }
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

            if (DailyTicket.FormType == DB.DTFormEnum.ShopTicket)
            {

                var lastTicket = db.DailyShopTickets.Where(f =>
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
                    lastTicket = db.DailyShopTickets.Where(f =>
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
                        var entry = DailyTicket.DailyShopTicket.AddHoursEntry(emp);
                        var perdiem = DailyTicket.DailyShopTicket.AddPerdiem(emp);
                        entry.PerdiemLineNum = perdiem.LineNum;
                    }
                }
            }
            else
            {
                if (Crew?.CrewLeader != null)
                {
                    var employeeList = Crew.CrewLeader.DirectReports.Where(f => f.ActiveYN == "Y").ToList();
                    foreach (var emp in employeeList)
                    {

                        var entry = DailyTicket.DailyShopTicket.AddHoursEntry(emp);
                        var perdiem = DailyTicket.DailyShopTicket.AddPerdiem(emp);
                        entry.PerdiemLineNum = perdiem.LineNum;
                    }

                    var crewEntry = DailyTicket.DailyShopTicket.AddHoursEntry(Crew?.CrewLeader);
                    var crewPerdiem = DailyTicket.DailyShopTicket.AddPerdiem(Crew?.CrewLeader);
                    crewEntry.PerdiemLineNum = crewPerdiem.LineNum;

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

        public void UpdateFromModel(Models.Views.DailyTicket.DailyCrewTicketViewModel model)
        {
            if (model == null)
                return;

            model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)WorkDate : model.WorkDate;

            /****Write the changes to object****/
            JobId = model.JobId;
            CrewId = model.CrewId;
            Comments = model.Comments;
            WorkDate = model.WorkDate;
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        {
            if (model == null)
                return;

            model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)WorkDate : model.WorkDate;

            /****Write the changes to object****/
            JobId = model.JobId;
            CrewId = model.CrewId;
            Comments = model.Comments;
            WorkDate = model.WorkDate;
        }

        public DailyEmployeeEntry AddHoursEntry()
        {
            var entry = DailyTicket.AddHoursEntry();

            if (Job != null)
            {
                entry.JobId = JobId;
                entry.JCCo = JCCo;
                entry.Job = Job;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Job;
            }

            return entry;
        }

        public DailyEmployeeEntry AddHoursEntry(Employee employee)
        {
            var entry = DailyTicket.AddHoursEntry(employee);

            if (Job != null)
            {
                entry.JobId = JobId;
                entry.JCCo = JCCo;
                entry.Job = Job;
                entry.EntryTypeId = (int)DB.EntryTypeEnum.Job;
            }

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
    }
}
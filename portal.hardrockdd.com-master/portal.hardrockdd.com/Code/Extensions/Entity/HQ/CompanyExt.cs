using System;
using System.Collections.Generic;
using System.Linq;
 
namespace portal.Code.Data.VP
{
    public partial class HQCompanyParm
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        private PRCompanyParm _PRCompanyParm;

        public PRCompanyParm PRCompanyParm
        {
            get
            {
                if (_PRCompanyParm == null)
                {
                    _PRCompanyParm = this.dbPRCompanyParm;
                    if (_PRCompanyParm.PRCo != PRCo)
                    {
                        
                        _PRCompanyParm = PRCompanyParmLink;
                    }
                }

                return _PRCompanyParm;
            }
            set
            {
                _PRCompanyParm = value;
            }
        }

        public DailyTicket AddDailyTicket(DateTime workDate, DB.DTFormEnum formType)
        {
            var userId = StaticFunctions.GetUserId();

            var curUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var dailyTicket = new DailyTicket
            {
                DTCo = (byte)HQCo,
                TicketId = DailyTicket.GetNextTicketId(),
                WorkDate = workDate,
                CreatedBy = StaticFunctions.GetUserId(),
                CreatedOn = DateTime.Now,
                ParentTicketId = null,

                CreatedUser = curUser,
                HQCompanyParm = this,
                db = this.db,
            };

            dailyTicket.Status = DB.DailyTicketStatusEnum.Draft;
            dailyTicket.FormType = formType;
            db.DailyTickets.Add(dailyTicket);

            return dailyTicket;
        }
    }


    public static class HQCompanyParmExt
    {

        public static List<Job> ActiveProjects(this HQCompanyParm company)
        {
            if (company == null)
                return new List<Job>();
            return company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7" && (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).ToList();
        }
        
        public static List<Job> Projects(this HQCompanyParm company)
        {
            if (company == null)
                return new List<Job>();
            return company.JCCompanyParm.Jobs.Where(f => f.JobTypeId == "7").ToList();
        }

        public static List<Job> ActiveJobs(this HQCompanyParm company)
        {
            if (company == null)
                return new List<Job>();
            return company.JCCompanyParm.Jobs.Where(f => (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).ToList();
        }
    }
}
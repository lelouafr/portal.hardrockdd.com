using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WebUser
    {

        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {                
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budWPUM"; } }

        private Employee _PREmployee;
        
        public Employee PREmployee
        {
            get
            {
                if (_PREmployee == null)
                {
                    var HREmployee = this.Employee.FirstOrDefault();
                    if (HREmployee != null)
                    {
                        _PREmployee = HREmployee.PREmployee;
                    }
                    if (_PREmployee == null)
                    {
                        _PREmployee = new Employee
                        {
                            FirstName = this.FirstName,
                            LastName = this.LastName,
                            Email = this.Email,
                            ActiveYN = "N"
                        };
                    }
                }
                return _PREmployee;
            }
        }
        private HRResource _HREmployee;

        public HRResource HREmployee
        {
            get
            {
                if (_HREmployee == null)
                {
                    var HREmployee = this.Employee.FirstOrDefault();
                    if (HREmployee != null)
                    {
                        _HREmployee = HREmployee;
                    }
                    if (_HREmployee == null)
                    {
                        _HREmployee = new HRResource
                        {
                            FirstName = this.FirstName,
                            LastName = this.LastName,
                            Email = this.Email,
                            ActiveYN = "N"
                        };
                    }
                }
                return _HREmployee;
            }
        }

        public List<EMAudit> ActiveEquipmentAudits()
        {
            var audits = AssignedEMAudits.Where(f => (f.AuditTypeId == (byte)EMAuditTypeEnum.CrewAudit || f.AuditFormId == (int)EMAuditFormEnum.Meter) &&
                                                     f.StatusId == (byte)EMAuditStatusEnum.New ||
                                                     f.StatusId == (byte)EMAuditStatusEnum.Rejected ||
                                                     f.StatusId == (byte)EMAuditStatusEnum.Started).ToList();
            
            return audits;
        }

        public string FullName()
        {
            
            if (HREmployee == null)
                return string.Format("{0} {1}", this.FirstName, this.LastName);

            return HREmployee.FullName(false);

        }

        public bool Active 
        { 
            get
            {
                if (PREmployee == null)
                {
                    return false;   
                }
                else
                {
                    return PREmployee.ActiveYN == "Y" ;
                }
            }
        }
    }
}
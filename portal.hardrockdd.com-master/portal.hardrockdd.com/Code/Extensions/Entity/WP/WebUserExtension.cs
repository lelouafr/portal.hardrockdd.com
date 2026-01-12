using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class WebUser
    {

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
            var audits = AssignedEMAudits.Where(f => (f.AuditTypeId == (byte)DB.EMAuditTypeEnum.CrewAudit || f.AuditFormId == (int)DB.EMAuditFormEnum.Meter) &&
                                                     f.StatusId == (byte)DB.EMAuditStatusEnum.New ||
                                                     f.StatusId == (byte)DB.EMAuditStatusEnum.Rejected ||
                                                     f.StatusId == (byte)DB.EMAuditStatusEnum.Started).ToList();
            
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
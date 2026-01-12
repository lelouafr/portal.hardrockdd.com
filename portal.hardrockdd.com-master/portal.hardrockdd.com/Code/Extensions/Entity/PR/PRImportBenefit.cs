using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PRImportBenefit
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
                        _db = this.Import.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public void SetEmployee()
        {
            if (this.PREmployee != null)
                return;
            IsError = false;
            var itemEmp = Import.PREmployees.FirstOrDefault(f => f.PaycomNumber == EECode);
            if (itemEmp != null)
            {
                PRCo = itemEmp.PRCo;
                PREmployeeId = itemEmp.EmployeeId;
                PREmployee = itemEmp;
            }
            if (PREmployee == null)
                IsError = true;
        }

        public void SetBenefitCode()
        {
            if (this.HRBenefitCode != null)
                return;
            IsError = false;
            var codeStr = PlanName.Substring(PlanName.Length - 6);
            codeStr = codeStr.Replace("(", string.Empty);
            codeStr = codeStr.Replace(")", string.Empty);

            var code = Import.HRBenefitCodes.FirstOrDefault(f => f.BenefitCodeId == codeStr);
            if (code == null)
            {
                var desciption = PlanName.Substring(0, PlanName.Length - 6);
                if (desciption.Length > 30)
                {
                    desciption = desciption.Substring(30);
                }
                var hrCompany = Import.HQCompanyParm.HRCompanyParm;
                code = new HRBenefitCode()
                {
                    HRCo = hrCompany.HRCo,
                    BenefitCodeId = codeStr,
                    Description = desciption,
                    PlanName = desciption,
                    PlanNumber = codeStr,
                    ACAHealthCareYN = "N",
                    ACAMinEssenCoverageYN = "N",
                    ACASelfInsuredYN = "N",
                    UpdatePRYN = "N",
                    
                    HRCompany = hrCompany,
                    db = db,
                };

                Import.HRBenefitCodes.Add(code);
                hrCompany.HRBenefitCodes.Add(code);
            }
            if (code != null)
            {
                HRCo = code.HRCo;
                HRBenefitCodeId = code.BenefitCodeId;
                HRBenefitCode = code;
            }
            if (HRBenefitCode == null)
                IsError = true;

        }


    }

}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class EmployeeRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public EmployeeRepository()
        {

        }

        public static Employee Init()
        {
            var model = new Employee
            {

            };

            return model;
        }

        public List<Employee> GetEmployees(byte PRCo)
        {
            var qry = db.Employees
                        .Where(f => f.PRCo == PRCo && f.ActiveYN == "Y" && f.EmployeeId <= 200000)
                        .ToList();
            return qry;
        }

        public Employee GetEmployee(byte PRCo, int EmployeeId)
        {
            var qry = db.Employees
                        .FirstOrDefault(f => f.PRCo == PRCo && f.EmployeeId == EmployeeId);

            return qry;
        }

        public List<SelectListItem> GetSelectList(byte PRCo, string selected = "")
        {
            return db.Employees.Where(f => f.PRCo == PRCo)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.EmployeeId.ToString(AppCultureInfo.CInfo()),
                                    Text = String.Format(AppCultureInfo.CInfo(), "{0} {1}", s.FirstName, s.LastName),
                                    Selected = s.EmployeeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<Employee> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.EmployeeId.ToString(AppCultureInfo.CInfo()),
                Text = String.Format(AppCultureInfo.CInfo(), "{0} {1}", s.FirstName, s.LastName),
                Selected = s.EmployeeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).OrderBy(o => o.Text).ToList();

            return result;
        }

        public Employee ProcessUpdate(Employee model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetEmployee(model.PRCo, model.EmployeeId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.LastName = model.LastName;
                updObj.FirstName = model.FirstName;
                updObj.MidName = model.MidName;
                updObj.SortName = model.SortName;
                updObj.Address = model.Address;
                updObj.City = model.City;
                updObj.State = model.State;
                updObj.Zip = model.Zip;
                updObj.Address2 = model.Address2;
                updObj.Phone = model.Phone;
                updObj.SSN = model.SSN;
                updObj.RaceId = model.RaceId;
                updObj.Sex = model.Sex;
                if (model.BirthDate >= new DateTime(1900, 1, 1))
                {
                    updObj.BirthDate = model.BirthDate ?? updObj.BirthDate;
                }
                if (model.HireDate >= new DateTime(1900, 1, 1))
                {
                    updObj.HireDate = model.HireDate ?? updObj.HireDate;
                }
                if (model.TermDate >= new DateTime(1900, 1, 1))
                {
                    updObj.TermDate = model.TermDate ?? updObj.TermDate;
                }
                updObj.PRGroupId = model.PRGroupId;
                updObj.PRDept = model.PRDept;
                updObj.Craft = model.Craft;
                updObj.Class = model.Class;
                updObj.InsCode = model.InsCode;
                updObj.TaxState = model.TaxState;
                updObj.UnempState = model.UnempState;
                updObj.InsState = model.InsState;
                updObj.LocalCode = model.LocalCode;
                updObj.GLCo = model.GLCo;
                updObj.UseState = model.UseState;
                updObj.UseIns = model.UseIns;
                updObj.JCCo = model.JCCo;
                updObj.JobId = model.JobId;
                updObj.CrewId = model.CrewId;
                if (model.LastUpdated >= new DateTime(1900, 1, 1))
                {
                    updObj.LastUpdated = model.LastUpdated ?? updObj.LastUpdated;
                }
                updObj.EarnCodeId = model.EarnCodeId;
                updObj.HrlyRate = model.HrlyRate;
                updObj.SalaryAmt = model.SalaryAmt;
                updObj.OTOpt = model.OTOpt;
                updObj.OTSched = model.OTSched;
                updObj.JCFixedRate = model.JCFixedRate;
                updObj.EMFixedRate = model.EMFixedRate;
                updObj.YTDSUI = model.YTDSUI;
                updObj.OccupCat = model.OccupCat;
                updObj.CatStatus = model.CatStatus;
                updObj.DirDeposit = model.DirDeposit;
                updObj.RoutingId = model.RoutingId;
                updObj.BankAcct = model.BankAcct;
                updObj.AcctType = model.AcctType;
                updObj.ActiveYN = model.ActiveYN;
                updObj.PensionYN = model.PensionYN;
                updObj.PostToAll = model.PostToAll;
                updObj.CertYN = model.CertYN;
                updObj.ChkSort = model.ChkSort;
                updObj.AuditYN = model.AuditYN;
                updObj.Notes = model.Notes;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.Email = model.Email;
                updObj.DefaultPaySeq = model.DefaultPaySeq;
                updObj.DDPaySeq = model.DDPaySeq;
                updObj.Suffix = model.Suffix;
                updObj.TradeSeq = model.TradeSeq;
                updObj.CSLimit = model.CSLimit;
                updObj.CSGarnGroup = model.CSGarnGroup;
                updObj.CSAllocMethod = model.CSAllocMethod;
                updObj.Shift = model.Shift;
                updObj.NonResAlienYN = model.NonResAlienYN;
                updObj.KeyID = model.KeyID;
                updObj.Country = model.Country;
                updObj.HDAmt = model.HDAmt;
                updObj.F1Amt = model.F1Amt;
                updObj.LCFStock = model.LCFStock;
                updObj.LCPStock = model.LCPStock;
                updObj.NAICS = model.NAICS;
                updObj.AUEFTYN = model.AUEFTYN;
                updObj.AUAccountNumber = model.AUAccountNumber;
                updObj.AUBSB = model.AUBSB;
                updObj.AUReference = model.AUReference;
                updObj.EMCo = model.EMCo;
                updObj.EquipmentId = model.EquipmentId;
                updObj.EMGroup = model.EMGroup;
                updObj.PayMethodDelivery = model.PayMethodDelivery;
                updObj.CPPQPPExempt = model.CPPQPPExempt;
                updObj.EIExempt = model.EIExempt;
                updObj.PPIPExempt = model.PPIPExempt;
                updObj.TimesheetRevGroup = model.TimesheetRevGroup;
                updObj.UpdatePRAEYN = model.UpdatePRAEYN;
                updObj.WOTaxState = model.WOTaxState;
                updObj.WOLocalCode = model.WOLocalCode;
                updObj.UseLocal = model.UseLocal;
                updObj.UseUnempState = model.UseUnempState;
                updObj.UseInsState = model.UseInsState;
                if (model.NewHireActStartDate >= new DateTime(1900, 1, 1))
                {
                    updObj.NewHireActStartDate = model.NewHireActStartDate ?? updObj.NewHireActStartDate;
                }
                if (model.NewHireActEndDate >= new DateTime(1900, 1, 1))
                {
                    updObj.NewHireActEndDate = model.NewHireActEndDate ?? updObj.NewHireActEndDate;
                }
                updObj.CellPhone = model.CellPhone;
                if (model.RecentRehireDate >= new DateTime(1900, 1, 1))
                {
                    updObj.RecentRehireDate = model.RecentRehireDate ?? updObj.RecentRehireDate;
                }
                if (model.RecentSeparationDate >= new DateTime(1900, 1, 1))
                {
                    updObj.RecentSeparationDate = model.RecentSeparationDate ?? updObj.RecentSeparationDate;
                }
                updObj.SeparationReason = model.SeparationReason;
                updObj.SeparationReasonExplanation = model.SeparationReasonExplanation;
                updObj.SMFixedRate = model.SMFixedRate;
                updObj.ArrearsActiveYN = model.ArrearsActiveYN;
                updObj.WeeklyHours = model.WeeklyHours;
                updObj.ETPPostedYN = model.ETPPostedYN;
                updObj.FMCSAPHMSA = model.FMCSAPHMSA;
                updObj.CAProgramAccount = model.CAProgramAccount;
                updObj.ADPnumber = model.ADPnumber;
                updObj.PaycomNumber = model.PaycomNumber;
                updObj.PerDiemRate = model.PerDiemRate;
                updObj.ZionsCC = model.ZionsCC;
                updObj.WEXID = model.WEXID;
                updObj.AdditionalSourceDedns = model.AdditionalSourceDedns;
                updObj.AuthorizedSourceDedns = model.AuthorizedSourceDedns;
                updObj.ExemptHealthContribution = model.ExemptHealthContribution;
                updObj.AlwaysCalcQPIP = model.AlwaysCalcQPIP;
                updObj.PrintInFrench = model.PrintInFrench;
                updObj.QCFileNumber = model.QCFileNumber;
                updObj.PAYGIncomeType = model.PAYGIncomeType;
                updObj.EmailW2YN = model.EmailW2YN;
                updObj.Email1095CYN = model.Email1095CYN;
                updObj.EmailT4YN = model.EmailT4YN;
                updObj.EmailPAYGSumYN = model.EmailPAYGSumYN;
                updObj.ReportsToId = model.ReportsToId;
                updObj.Nickname = model.Nickname;

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public Employee Create(Employee model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            model.EmployeeId = db.Employees
                            .Where(f => f.PRCo == model.PRCo && f.EmployeeId >= 900000)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.PRCo) + 1;

            db.Employees.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(Employee model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Employees.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(Employee model)
        {
            var qry = from f in db.Employees
                      where f.PRCo == model.PRCo && f.EmployeeId == model.EmployeeId
                      select f;

            if (qry.Any())
                return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EmployeeRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }
}

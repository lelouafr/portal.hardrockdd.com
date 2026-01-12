//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class ContractRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public ContractRepository()
//        {

//        }

//        public static JCContract Init()
//        {
//            var model = new JCContract
//            {

//            };

//            return model;
//        }

//        public List<JCContract> GetContracts(byte JCCo)
//        {
//            var qry = db.JCContracts
//                        .Where(f => f.JCCo == JCCo)
//                        .ToList();

//            return qry;
//        }
        
//        public JCContract GetContract(byte JCCo, string ContractId)
//        {
//            var qry = db.JCContracts.FirstOrDefault(f => f.JCCo == JCCo && f.ContractId == ContractId);

//            return qry;
//        }
        
//        public List<SelectListItem> GetSelectList(byte JCCo, string selected = "")
//        {
//            return db.JCContracts.Where(f => f.JCCo == JCCo)
//                                .Select(s => new SelectListItem
//                                {
//                                    Value = s.ContractId.ToString(AppCultureInfo.CInfo()),
//                                    Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
//                                    Selected = s.ContractId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//                                }).ToList();

//        }

//        public static List<SelectListItem> GetSelectList(List<JCContract> List, string selected = "")
//        {

//            var result = List.Select(s => new SelectListItem
//            {
//                Value = s.ContractId.ToString(AppCultureInfo.CInfo()),
//                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
//                Selected = s.ContractId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
//            }).ToList();

//            return result;
//        }

//        public JCContract ProcessUpdate(JCContract model, ModelStateDictionary modelState)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var updObj = GetContract(model.JCCo, model.ContractId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//                updObj.DepartmentId = model.DepartmentId;
//                updObj.ContractStatus = model.ContractStatus;
//                updObj.OriginalDays = model.OriginalDays;
//                updObj.CurrentDays = model.CurrentDays;
//                if (model.StartMonth >= new DateTime(1900, 1, 1))
//                {
//                    updObj.StartMonth = model.StartMonth;
//                }
//                if (model.MonthClosed >= new DateTime(1900, 1, 1))
//                {
//                    updObj.MonthClosed = model.MonthClosed ?? updObj.MonthClosed;
//                }
//                if (model.ProjCloseDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.ProjCloseDate = model.ProjCloseDate ?? updObj.ProjCloseDate;
//                }
//                if (model.ActualCloseDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.ActualCloseDate = model.ActualCloseDate ?? updObj.ActualCloseDate;
//                }
//                updObj.CustGroupId = model.CustGroupId;
//                updObj.CustomerId = model.CustomerId;
//                updObj.PayTerms = model.PayTerms;
//                updObj.TaxInterface = model.TaxInterface;
//                updObj.TaxGroup = model.TaxGroup;
//                updObj.TaxCode = model.TaxCode;
//                updObj.RetainagePCT = model.RetainagePCT;
//                updObj.DefaultBillType = model.DefaultBillType;
//                updObj.OrigContractAmt = model.OrigContractAmt;
//                updObj.ContractAmt = model.ContractAmt;
//                updObj.BilledAmt = model.BilledAmt;
//                updObj.ReceivedAmt = model.ReceivedAmt;
//                updObj.CurrentRetainAmt = model.CurrentRetainAmt;
//                if (model.InBatchMth >= new DateTime(1900, 1, 1))
//                {
//                    updObj.InBatchMth = model.InBatchMth ?? updObj.InBatchMth;
//                }
//                updObj.InUseBatchId = model.InUseBatchId;
//                updObj.Notes = model.Notes;
//                updObj.SIRegion = model.SIRegion;
//                updObj.SIMetric = model.SIMetric;
//                updObj.ProcessGroup = model.ProcessGroup;
//                updObj.BillAddress = model.BillAddress;
//                updObj.BillAddress2 = model.BillAddress2;
//                updObj.BillCity = model.BillCity;
//                updObj.BillState = model.BillState;
//                updObj.BillZip = model.BillZip;
//                updObj.BillNotes = model.BillNotes;
//                updObj.BillOnCompletionYN = model.BillOnCompletionYN;
//                updObj.CustomerReference = model.CustomerReference;
//                updObj.CompleteYN = model.CompleteYN;
//                updObj.RoundOpt = model.RoundOpt;
//                updObj.ReportRetgItemYN = model.ReportRetgItemYN;
//                updObj.ProgressFormat = model.ProgressFormat;
//                updObj.TMFormat = model.TMFormat;
//                updObj.BillGroup = model.BillGroup;
//                updObj.BillDayOfMth = model.BillDayOfMth;
//                updObj.ArchitectName = model.ArchitectName;
//                updObj.ArchitectProject = model.ArchitectProject;
//                updObj.ContractForDesc = model.ContractForDesc;
//                if (model.StartDate >= new DateTime(1900, 1, 1))
//                {
//                    updObj.StartDate = model.StartDate ?? updObj.StartDate;
//                }
//                updObj.JBTemplate = model.JBTemplate;
//                updObj.JBFlatBillingAmt = model.JBFlatBillingAmt;
//                updObj.JBLimitOpt = model.JBLimitOpt;
//                updObj.UniqueAttchID = model.UniqueAttchID;
//                updObj.RecType = model.RecType;
//                updObj.OverProjNotes = model.OverProjNotes;
//                updObj.ClosePurgeFlag = model.ClosePurgeFlag;
//                updObj.MiscDistCode = model.MiscDistCode;
//                updObj.SecurityGroup = model.SecurityGroup;
//                updObj.UpdateJCCI = model.UpdateJCCI;
//                updObj.KeyID = model.KeyID;
//                updObj.BillCountry = model.BillCountry;
//                updObj.PotentialProject = model.PotentialProject;
//                updObj.MaxRetgOpt = model.MaxRetgOpt;
//                updObj.MaxRetgPct = model.MaxRetgPct;
//                updObj.MaxRetgAmt = model.MaxRetgAmt;
//                updObj.InclACOinMaxYN = model.InclACOinMaxYN;
//                updObj.MaxRetgDistStyle = model.MaxRetgDistStyle;
//                updObj.Territory = model.Territory;
//                updObj.SalesTerritory = model.SalesTerritory;
//                updObj.BoreCount = model.BoreCount;

//                db.SaveChanges(modelState);
//            }
//            return updObj;
//        }
        
//        public JCContract Create(JCContract model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }

//            db.JCContracts.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }

//        public bool Delete(JCContract model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            db.JCContracts.Remove(model);
//            return db.SaveChanges(modelState) == 0 ? false : true;
//        }

//        public bool Exists(JCContract model)
//        {
//            var qry = from f in db.JCContracts
//                      where f.JCCo == model.JCCo && f.ContractId == model.ContractId
//                      select f;

//            if (qry.Any())
//                return true;

//            return false;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~ContractRepository()
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

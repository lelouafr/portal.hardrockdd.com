using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class EarnCodeRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public EarnCodeRepository()
        {

        }

        public static EarnCode Init()
        {
            var model = new EarnCode
            {

            };

            return model;
        }

        public List<EarnCode> GetEarnCodes(byte PRCo)
        {
            var qry = db.EarnCodes
                        .Where(f => f.PRCo == PRCo)
                        .ToList();

            return qry;
        }

        public EarnCode GetEarnCode(byte PRCo, short EarnCodeId)
        {
            var qry = db.EarnCodes
                        .FirstOrDefault(f => f.PRCo == PRCo && f.EarnCodeId == EarnCodeId);

            return qry;
        }

        public List<SelectListItem> GetSelectList(byte PRCo, string selected = "")
        {
            return db.EarnCodes.Where(f => f.PRCo == PRCo)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.EarnCodeId.ToString(AppCultureInfo.CInfo()),
                                    Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EarnCodeId, s.Description),
                                    Selected = s.EarnCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<EarnCode> List, string selected = "")
        {

            var result = List.Select(s => new SelectListItem
            {
                Value = s.EarnCodeId.ToString(AppCultureInfo.CInfo()),
                Text = s.PortalLabel,
                Selected = s.EarnCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();
            return result;
        }

        public EarnCode ProcessUpdate(EarnCode model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetEarnCode(model.PRCo, model.EarnCodeId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.Method = model.Method;
                updObj.Factor = model.Factor;
                updObj.SubjToAddOns = model.SubjToAddOns;
                updObj.CertRpt = model.CertRpt;
                updObj.TrueEarns = model.TrueEarns;
                updObj.OTCalcs = model.OTCalcs;
                updObj.EarnType = model.EarnType;
                updObj.JCCostType = model.JCCostType;
                updObj.Notes = model.Notes;
                updObj.StandardLimit = model.StandardLimit;
                updObj.UniqueAttchID = model.UniqueAttchID;
                updObj.SubjToAutoEarns = model.SubjToAutoEarns;
                updObj.LimitType = model.LimitType;
                updObj.AutoAP = model.AutoAP;
                updObj.VendorGroup = model.VendorGroup;
                updObj.VendorId = model.VendorId;
                updObj.TransByEmployee = model.TransByEmployee;
                updObj.PayType = model.PayType;
                updObj.Frequency = model.Frequency;
                updObj.GLCo = model.GLCo;
                updObj.GLAcct = model.GLAcct;
                updObj.IncldLiabDist = model.IncldLiabDist;
                updObj.IncldSalaryDist = model.IncldSalaryDist;
                updObj.KeyID = model.KeyID;
                updObj.Routine = model.Routine;
                updObj.IncldRemoteTC = model.IncldRemoteTC;
                updObj.ATOCategory = model.ATOCategory;
                updObj.InsurableEarningsYN = model.InsurableEarningsYN;
                updObj.InsurableHoursYN = model.InsurableHoursYN;
                updObj.ROESeparationYN = model.ROESeparationYN;
                updObj.ROECategory = model.ROECategory;
                updObj.OtherMonies = model.OtherMonies;
                updObj.ROESpecialYN = model.ROESpecialYN;
                updObj.ROEType = model.ROEType;
                updObj.ROEPeriod = model.ROEPeriod;
                updObj.PaycomCode = model.PaycomCode;
                updObj.PaycomImport = model.PaycomImport;
                updObj.VacationPayType = model.VacationPayType;
                updObj.ACATrackHrsYN = model.ACATrackHrsYN;
                updObj.FringeBenefitType = model.FringeBenefitType;
                updObj.Status = model.Status;
                updObj.PortalIsHidden = model.PortalIsHidden;
                updObj.PortalLabel = model.PortalLabel;

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public EarnCode Create(EarnCode model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            db.EarnCodes.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(EarnCode model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.EarnCodes.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(EarnCode model)
        {
            var qry = from f in db.EarnCodes
                      where f.PRCo == model.PRCo && f.EarnCodeId == model.EarnCodeId
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

        ~EarnCodeRepository()
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

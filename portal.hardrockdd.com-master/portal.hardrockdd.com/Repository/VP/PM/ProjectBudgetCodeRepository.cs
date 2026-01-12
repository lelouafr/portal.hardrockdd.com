using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PM
{
    public partial class ProjectBudgetCodeRepository : IDisposable
    {
        private VPContext db = new VPContext();        

        public ProjectBudgetCodeRepository()
        {

        }
        
        public static ProjectBudgetCode Init()
        {
            var model = new ProjectBudgetCode
            {

            };

            return model;
        }

        public List<ProjectBudgetCode> GetProjectBudgetCodes(byte PMCo)
        {
            var qry = db.ProjectBudgetCodes
                        .Where(f => f.PMCo == PMCo)
                        .ToList();

            return qry;
        }

        public ProjectBudgetCode GetProjectBudgetCode(byte PMCo, string BudgetCodeId)
        {
            var qry = db.ProjectBudgetCodes
                        .Where(f => f.PMCo == PMCo && f.BudgetCodeId == BudgetCodeId)
                        .FirstOrDefault();

            return qry;
        }

        public List<SelectListItem> GetSelectList(byte PMCo, string selected = "")
        {
            return GetProjectBudgetCodes(PMCo)
                                .Where(f => f.PMCo == PMCo)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                                    Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.BudgetCodeId, s.Description),
                                    Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<ProjectBudgetCode> List, string selected = "")
        {

            var result = List.Select(s => new SelectListItem
            {
                Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.BudgetCodeId, s.Description),
                Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();
            return result;
        }

        public ProjectBudgetCode ProcessUpdate(ProjectBudgetCode model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetProjectBudgetCode(model.PMCo, model.BudgetCodeId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.Active = model.Active;
                updObj.CostLevel = model.CostLevel;
                updObj.PhaseGroupId = model.PhaseGroupId;
                updObj.PhaseId = model.PhaseId;
                updObj.CostTypeId = model.CostTypeId;
                updObj.UM = model.UM;
                updObj.UnitCost = model.UnitCost;
                updObj.HrsPerUnit = model.HrsPerUnit;
                updObj.Percentage = model.Percentage;
                updObj.Notes = model.Notes;
                updObj.Basis = model.Basis;
                updObj.TimeUM = model.TimeUM;
                updObj.Rate = model.Rate;
                updObj.ExcludeFromLookups = model.ExcludeFromLookups;
                updObj.Radius = model.Radius;
                updObj.Hardness = model.Hardness;
                updObj.RockOnly = model.RockOnly;
                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public static ProjectBudgetCode Init(byte Co, string Prefix, string Description)
        {
            var model = new ProjectBudgetCode
            {
                PMCo = Co,
                PhaseGroupId = Co,
                BudgetCodeId = Prefix,
                Description = Description,
                CostLevel = "D",
                Basis = "U",
                ExcludeFromLookups = "N",
                RockOnly = "N",
                UnitCost = 0,
                UM = "EA",
                Active = "Y",

            };

            switch (Prefix)
            {
                case "BC":
                    model.CostTypeId = 34;
                    break;
                case "MT":
                    model.CostTypeId = 2;
                    break;
                case "EQ":
                    model.CostTypeId = 4;
                    break;
                case "RE":
                    model.CostTypeId = 5;
                    break;
                case "TM":
                    model.CostTypeId = 8;
                    break;
                default:
                    break;
            }
            return model;
        }

        public ProjectBudgetCode FindCreate(byte Co, string Prefix, string Description)
        {
            var result = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == Co && f.Description == Description && f.BudgetCodeId.Substring(0, 2) == Prefix);

            if (result == null)
            {
                result = Init(Co, Prefix, Description);
                result = Create(result);
            }
            return result;
        }
        
        public static ProjectBudgetCode FindCreate(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, byte Co, string Prefix, string Description)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            var result = company.ProjectBudgets.FirstOrDefault(f => f.PMCo == Co && f.Description == Description && f.BudgetCodeId.Substring(0, 2) == Prefix);

            if (result == null)
            {
                result = Init(Co, Prefix, Description);
                result.BudgetCodeId = NextId(company, result);
                company.ProjectBudgets.Add(result);
            }
            return result;
        }
    
        public static string NextId(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, ProjectBudgetCode model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            var maxID = company.ProjectBudgets
                               .Where(f => f.PMCo == company.HQCo && f.BudgetCodeId.Substring(0, model.BudgetCodeId.Length + 1) == model.BudgetCodeId + "-")
                               .DefaultIfEmpty()
                               .Max(f => f == null ? "" : f.BudgetCodeId);

            var dash = maxID.IndexOf("-", StringComparison.Ordinal) + 1;
            var right = maxID.Substring(dash, maxID.Length - dash);
            var id = int.TryParse(right, out int idtmp) ? idtmp : 0;

            id++;
            var newcode = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", model.BudgetCodeId, id.ToString(AppCultureInfo.CInfo()).PadLeft(4, '0'));

            return newcode;
        }

        public ProjectBudgetCode Create(ProjectBudgetCode model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            if (!model.BudgetCodeId.Contains("-"))
            {
                //var maxID = db.ProjectBudgetCodes
                //               .Where(f => f.PMCo == model.PMCo && f.BudgetCodeId.Substring(0, model.BudgetCodeId.Length + 1) == model.BudgetCodeId + "-")
                //               .DefaultIfEmpty()
                //               .Max(f => f == null ? "" : f.BudgetCodeId);

                //var dash = maxID.IndexOf("-", StringComparison.Ordinal) + 1;
                //var right = maxID.Substring(dash, maxID.Length - dash);
                //var id = int.TryParse(right, out int idtmp) ? idtmp : 0;

                //id++;
                //var newcode = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", model.BudgetCodeId, id.ToString(AppCultureInfo.CInfo()).PadLeft(4, '0'));
                var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == model.PMCo);
                model.BudgetCodeId = NextId(company, model);
            }

            db.ProjectBudgetCodes.Add(model);
            db.SaveChanges(modelState);            

            return model;
        }
               
        public bool Delete(ProjectBudgetCode model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.ProjectBudgetCodes.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(ProjectBudgetCode model)
        {
            var qry = from f in db.ProjectBudgetCodes
                      where f.PMCo == model.PMCo && f.BudgetCodeId == model.BudgetCodeId
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

        ~ProjectBudgetCodeRepository()
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

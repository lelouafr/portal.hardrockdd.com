using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace portal.Repository.VP.PM
{
    public partial class ProjectDivisionRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public ProjectDivisionRepository()
        {

        }
        
        public static ProjectDivision Init()
        {
            var model = new ProjectDivision
            {
                
            };

            return model;
        }
        
        public List<ProjectDivision> GetProjectDivisions(byte PMCo)
        {
            var qry = db.ProjectDivisions
                        .Where(f => f.PMCo == PMCo)
                        .ToList();

            return qry;
        }
        
        public ProjectDivision GetProjectDivision(byte PMCo, int DivisionId)
        {
            var qry = db.ProjectDivisions
                        .FirstOrDefault(f => f.PMCo == PMCo && f.DivisionId == DivisionId);

            return qry;
        }
        
        public List<SelectListItem> GetSelectList(byte PMCo, string selected = "")
        {
            return db.ProjectDivisions.Where(f => f.PMCo == PMCo)
                                .Select(s => new SelectListItem
                                {
                                    Value = s.DivisionId.ToString(AppCultureInfo.CInfo()),
                                    Text = s.Description,
                                    Selected = s.DivisionId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                                }).ToList();

        }

        public static List<SelectListItem> GetSelectList(List<ProjectDivision> List, string selected = "")
        {
            var result = List.Select(s => new SelectListItem
            {
                Value = s.DivisionId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = s.DivisionId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.IsActive == true ? "Active" : "Disabled",
                    Disabled = s.IsActive != true
                },
            }).ToList();
            return result;
        }

        public ProjectDivision ProcessUpdate(ProjectDivision model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetProjectDivision(model.PMCo, model.DivisionId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.DistributionGroupId = model.DistributionGroupId;

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public ProjectDivision Create(ProjectDivision model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            model.DivisionId = db.ProjectDivisions
                            .Where(f => f.PMCo == model.PMCo)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.DivisionId) + 1;
            db.ProjectDivisions.Add(model);
            db.SaveChanges(modelState);
            return model;
        }

        public bool Delete(ProjectDivision model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.ProjectDivisions.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public bool Exists(ProjectDivision model)
        {
            var qry = from f in db.ProjectDivisions
                      where f.PMCo == model.PMCo && f.DivisionId == model.DivisionId
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

        ~ProjectDivisionRepository()
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

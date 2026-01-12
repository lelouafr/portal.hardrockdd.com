//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.VP.WP;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.WP
//{
//    public partial class WebRoleRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public static WebRoleViewModel EntityToModel(WebRole t)
//        {
//            if (t == null)
//            {
//                throw new System.ArgumentNullException(nameof(t));
//            }
//            var result = new WebRoleViewModel
//            {
//                Id = t.Id,
//                Name = t.Name,
//            };
//            return result;
//        }

//        public List<WebRoleViewModel> GetRoles()
//        {
//            var qry = db.WebRoles
//               .AsEnumerable()
//               .Select(s => EntityToModel(s));

//            return qry.ToList();
//        }

//        public WebRoleViewModel GetRole(string roleId)
//        {
//            var qry = db.WebRoles
//               .Where(f => f.Id == roleId)
//               .AsEnumerable()
//               .Select(s => EntityToModel(s));

//            return qry.FirstOrDefault();
//        }

//        public WebRoleViewModel Update(WebRoleViewModel model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var entity = db.WebRoles.FirstOrDefault(p => p.Id == model.Id);
//            try
//            {
//                if (entity != null)
//                {
//                    entity = model.ToEntity(entity);

//                    db.WebRoles.Attach(entity);
//                    db.Entry(entity).State = EntityState.Modified;
//                    db.SaveChanges();
//                }
//            }
//            catch (Exception ex)
//            {
//                if (modelState != null)
//                {
//                    modelState.AddModelError("DB Save Error", ex.GetBaseException().ToString());
//                }
//                else
//                {
//                    throw new ArgumentException(ex.GetBaseException().ToString());
//                }
//            }
//            return model;
//        }

//        public WebRoleViewModel Create(WebRoleViewModel model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            try
//            {
//                var entity = model.ToEntity();

//                db.WebRoles.Add(entity);
//                db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                if (modelState != null)
//                {
//                    modelState.AddModelError("", ex.GetBaseException().ToString());
//                }
//                else
//                {
//                    throw new System.ArgumentException(ex.GetBaseException().ToString());
//                }
//            }

//            return model;
//        }

//        public WebRoleViewModel Delete(WebRoleViewModel model, ModelStateDictionary modelState = null)
//        {
//            try
//            {
//                var entity = db.WebRoles
//                               .Where(p => p.Id == model.Id)
//                               .FirstOrDefault();
//                if (entity != null)
//                {
//                    db.WebRoles.Attach(entity);
//                    db.WebRoles.Remove(entity);
//                    db.Entry(entity).State = EntityState.Deleted;
//                    db.SaveChanges();
//                }
//                else
//                {
//                    if (modelState != null)
//                    {
//                        modelState.AddModelError("Role didn't exists", "Role didn't exists");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                if (modelState != null)
//                {
//                    modelState.AddModelError("", ex.GetBaseException().ToString());
//                }
//                else
//                {
//                    throw new System.ArgumentException(ex.GetBaseException().ToString());
//                }
//            }

//            return model;
//        }

//        public bool Exists(string roleId)
//        {
//            var qry = from obj in db.WebRoles
//                      where
//                         obj.Id == roleId
//                      select obj;

//            if (qry.Any())
//                return true;

//            return false;
//        }


//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~WebRoleRepository()
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
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.VP.WP;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.ModelBinding;

//namespace portal.Repository.VP.WP
//{
//    public class WebUserRoleRepository : IDisposable
//    {
//        private VPContext db = new VPContext();
//        private WebUserRepository usrRepo = new WebUserRepository();
//        private WebRoleRepository rolRepo = new WebRoleRepository();

//        public static WebUserRoleViewModel EntityToModel(WebUserRole t, string includeObjects)
//        {
//            if (t == null)
//            {
//                throw new System.ArgumentNullException(nameof(t));
//            }
//            includeObjects ??= "";
//            var result = new WebUserRoleViewModel
//            {
//                UserId = t.UserId,
//                RoleId = t.RoleId,
//            };
//            if (includeObjects.Contains("Role"))
//            {
//                result.Role = t.Role != null ? WebRoleRepository.EntityToModel(t.Role) : null;
//            }

//            if (includeObjects.Contains("User"))
//            {
//                var subIncludeString = StaticFunctions.GetChildObjectString(includeObjects, "User");
//                result.User = t.User != null ? WebUserRepository.EntityToModel(t.User, subIncludeString) : null;
//            }
//            return result;
//        }

//        public bool Create(WebUserRoleModel model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            try
//            {
//                var entity = model.ToEntity();

//                db.WebUserRoles.Add(entity);
//                db.SaveChanges();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                if (modelState != null)
//                {
//                    modelState.AddModelError("DB Save Error", ex.GetBaseException().ToString());
//                }
//                else
//                {
//                    throw new InvalidOperationException(ex.GetBaseException().ToString());
//                }
//                return false;
//            }
//        }

//        public bool Delete(WebUserRoleModel model, ModelStateDictionary modelState = null)
//        {
//            try
//            {
//                var entity = db.WebUserRoles
//                               .Where(f => f.UserId == model.UserId && f.RoleId == model.RoleId)
//                               .FirstOrDefault();
//                if (entity != null)
//                {
//                    db.WebUserRoles.Attach(entity);
//                    db.WebUserRoles.Remove(entity);
//                    db.Entry(entity).State = EntityState.Deleted;
//                    db.SaveChanges();
//                    return true;
//                }
//                else
//                {
//                    if (modelState != null)
//                    {
//                        modelState.AddModelError("", "User didn't exists");
//                    }
//                    return false;
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
//                return false;
//            }
//        }

//        public List<WebRoleViewModel> GetAssignedRoles(string userId)
//        {
//            var qry = db.WebRoles.Where(f => db.WebUserRoles.Where(r => r.RoleId == f.Id && r.UserId == userId).Any());

//            var results = qry.AsEnumerable()
//                             .Select(t => WebRoleRepository.EntityToModel(t))
//                             .ToList();
//            return results;
//        }

//        public List<WebRoleViewModel> GetUnassignedRoles(string userId)
//        {
//            var qry = db.WebRoles.Where(f => !db.WebUserRoles.Where(r => r.RoleId == f.Id && r.UserId == userId).Any());

//            var results = qry.AsEnumerable()
//                             .Select(t => WebRoleRepository.EntityToModel(t))
//                             .ToList();
//            return results;

//        }

//        public List<WebUserViewModel> GetAssignedUsers(string roleId)
//        {
//            var qry = db.WebUsers.Where(f => db.WebUserRoles.Where(r => r.RoleId == roleId && r.UserId == f.Id).Any());

//            var results = qry.AsEnumerable()
//                             .OrderBy(o => o.LastName)
//                             .Select(t => WebUserRepository.EntityToModel(t, ""))
//                             .ToList();
//            return results;
//        }

//        public List<WebUserViewModel> GetUnassignedUsers(string roleId)
//        {
//            var qry = db.WebUsers.Where(f => !db.WebUserRoles.Where(r => r.RoleId == roleId && r.UserId == f.Id).Any());

//            var results = qry.OrderBy(o => o.LastName)
//                             .AsEnumerable()
//                             .Select(t => WebUserRepository.EntityToModel(t, ""))
//                             .ToList();
//            return results;

//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~WebUserRoleRepository()
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
//                if (usrRepo != null)
//                {
//                    usrRepo.Dispose();
//                    usrRepo = null;
//                }
//                if (rolRepo != null)
//                {
//                    rolRepo.Dispose();
//                    rolRepo = null;
//                }
//            }
//        }
//    }
//}
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.VP.WP;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.WP
//{
//    public class WebUserRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public static WebUserViewModel EntityToModel(WebUser t, string includeObjects)
//        {
//            if (t == null)
//            {
//                throw new System.ArgumentNullException(nameof(t));
//            }
//            includeObjects ??= "";
//            var result = new WebUserViewModel
//            {
//                Id = t.Id,
//                Email = t.Email,
//                EmailConfirmed = t.EmailConfirmed,
//                PasswordHash = t.PasswordHash,
//                SecurityStamp = t.SecurityStamp,
//                PhoneNumber = t.PhoneNumber,
//                PhoneNumberConfirmed = t.PhoneNumberConfirmed,
//                TwoFactorEnabled = t.TwoFactorEnabled,
//                LockoutEndDateUtc = t.LockoutEndDateUtc,
//                LockoutEnabled = t.LockoutEnabled,
//                AccessFailedCount = t.AccessFailedCount,
//                UserName = t.UserName,
//                FirstName = t.FirstName,
//                LastName = t.LastName,
//            };

//            if (includeObjects.Contains("Roles"))
//            {
//                result.Roles.AddRange(t.Roles.Select(s => WebUserRoleRepository.EntityToModel(s, StaticFunctions.GetChildObjectString(includeObjects, "Roles"))).ToList());
//            }
//            return result;
//        }

//        public List<WebUserViewModel> GetUsers(string includeObjects = "")
//        {
//            var qry = db.WebUsers
//                        .AsEnumerable()
//                        .Select(s => EntityToModel(s, includeObjects));

//            return qry.ToList();
//        }

//        public WebUserViewModel GetUser(string userId, string includeObjects = "")
//        {
//            var qry = db.WebUsers
//                        .Where(user => user.Id == userId)
//                        .AsEnumerable()
//                        .Select(s => EntityToModel(s, includeObjects));

//            return qry.FirstOrDefault();
//        }

//        public List<SelectListItem> GetSelectList(string selected = "")
//        {
//            return GetUsers().OrderBy(o => o.LastName)
//                             .Select(s => new SelectListItem
//                             {
//                                 Value = s.Id,
//                                 Text = s.FullName,
//                                 Selected = s.Id == selected ? true : false
//                             }).ToList();

//        }

//        public WebUserModel GetUserbyEmail(string Email, string includeObjects = "")
//        {
//            var qry = db.WebUsers
//                        .Where(user => user.Email.ToLower() == Email.ToLower())
//                        .AsEnumerable()
//                        .Select(s => EntityToModel(s, includeObjects));


//            return qry.FirstOrDefault();
//        }

//        public WebUserModel Update(WebUserModel model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            var entity = db.WebUsers.FirstOrDefault(p => p.Id == model.Id);

//            try
//            {
//                if (entity != null)
//                {
//                    entity = model.ToEntity(entity);

//                    db.WebUsers.Attach(entity);
//                    db.Entry(entity).State = EntityState.Modified;
//                    db.SaveChanges();
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

//        public WebUserModel Delete(WebUserModel model, ModelStateDictionary modelState = null)
//        {
//            var allowDelete = true;
//            //if (db.DailyJobTickets.Where(f => f.DailyTicket.CreatedBy == model.Id ||
//            //                                    f.DailyTicket.ApprovedBy == model.Id ||
//            //                                    f.DailyTicket.SubmittedBy == model.Id).Any())
//            //{
//            //    modelState.AddModelError("", "Record cannot be delete user has transactions");
//            //    allowDelete = false;
//            //}

//            //if (allowDelete && db.EmployeeEntries.Where(f => f.CreatedBy == model.Id ||
//            //                                               f.ApprovedBy == model.Id ||
//            //                                               f.SubmittedBy == model.Id ||
//            //                                               f.ProcessedBy == model.Id ||
//            //                                               f.PayrollBy == model.Id).Any())
//            //{
//            //    modelState.AddModelError("", "Record cannot be delete user has transactions");
//            //    allowDelete = false;
//            //}

//            try
//            {
//                if (allowDelete)
//                {
//                    var entity = db.WebUsers
//                                   .Where(f => f.Id == model.Id)
//                                   .FirstOrDefault();
//                    if (entity != null)
//                    {
//                        db.WebUsers.Attach(entity);
//                        db.WebUsers.Remove(entity);
//                        db.Entry(entity).State = EntityState.Deleted;
//                        db.SaveChanges();
//                    }
//                    else
//                    {
//                        if (modelState != null)
//                        {
//                            modelState.AddModelError("", "Record didn't exists");
//                        }
//                    }
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
//                    throw new InvalidOperationException(ex.GetBaseException().ToString());
//                }
//            }

//            return model;
//        }

//        public bool Exists(WebUserModel model)
//        {
//            var qry = from prp in db.WebUsers
//                      where
//                         (prp.Id == model.Id)
//                      select prp;

//            if (qry.Any())
//                return true;

//            return false;
//        }


//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~WebUserRepository()
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
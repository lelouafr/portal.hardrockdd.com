using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.WP
{
    public partial class WebUserRepository : IDisposable
    {
        private VPContext db = new VPContext();

        public List<WebUser> GetUsers()
        {
            var qry = db.WebUsers.ToList();

            return qry;
        }

        public WebUser GetUser(string userId)
        {
            var qry = db.WebUsers
                        .FirstOrDefault(user => user.Id == userId);

            return qry;
        }

        public List<SelectListItem> GetSelectList(string selected = "")
        {
            return db.WebUsers.OrderBy(o => o.LastName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.Id,
                                 Text = string.Format(AppCultureInfo.CInfo(),"{0} {1}", s.FirstName, s.LastName),
                                 Selected = s.Id == selected ? true : false
                             }).ToList();

        }

        public bool Delete(WebUser model, ModelStateDictionary modelState = null)
        {
            var allowDelete = true;
            //if (db.DailyJobTickets.Where(f => f.DailyTicket.CreatedBy == model.Id ||
            //                                    f.DailyTicket.ApprovedBy == model.Id ||
            //                                    f.DailyTicket.SubmittedBy == model.Id).Any())
            //{
            //    modelState.AddModelError("", "Record cannot be delete user has transactions");
            //    allowDelete = false;
            //}

            //if (allowDelete && db.EmployeeEntries.Where(f => f.CreatedBy == model.Id ||
            //                                               f.ApprovedBy == model.Id ||
            //                                               f.SubmittedBy == model.Id ||
            //                                               f.ProcessedBy == model.Id ||
            //                                               f.PayrollBy == model.Id).Any())
            //{
            //    modelState.AddModelError("", "Record cannot be delete user has transactions");
            //    allowDelete = false;
            //}

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (allowDelete)
            {
                db.WebUsers.Remove(model);
                return db.SaveChanges(modelState) == 0 ? false : true;
            }
            else
                return allowDelete;

        }

        public bool Exists(WebUser model)
        {
            var qry = from prp in db.WebUsers
                      where
                         (prp.Id == model.Id)
                      select prp;

            if (qry.Any())
                return true;

            return false;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebUserRepository()
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
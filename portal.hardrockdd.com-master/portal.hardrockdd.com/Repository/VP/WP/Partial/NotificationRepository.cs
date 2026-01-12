using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.WP
{
    public partial class NotificationRepository : IDisposable
    {
        
        public List<Notification> GetNotifications(string UserId)
        {
            var result = db.Notifications
                        .Where(f => f.AssignedTo == UserId && f.IsDeleted == (int)DB.YesNoEnum.No)
                        .OrderByDescending(o => o.CreatedOn)
                        .ToList();
            

            return result;
        }
        
        public List<Notification> GetUnReadNotifications(string UserId)
        {
            var result = db.Notifications
                        .Where(f => f.AssignedTo == UserId && f.IsRead == (int)DB.YesNoEnum.No)
                        .ToList();

            return result;
        }

        public static Notification Init(Models.Views.DailyTicket.DailyTicketRejectViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var notification = new Notification
            {
                AssignedTo = model.CreatedUser.Id,
                Title = string.Format(AppCultureInfo.CInfo(), "{0} Reject ({1})", model.FormName, model.Description),
                Note = model.Comments,
                CreatedBy = StaticFunctions.GetUserId(),
                CreatedOn = DateTime.Now,
                NotificationLevel = (int)DB.NotificationLevelEnum.Danger,
                IsRead = (int)DB.YesNoEnum.No,
                IsEmailed = (int)DB.YesNoEnum.No,
                IsDeleted = (int)DB.YesNoEnum.No,
                Url = model.Url
            };

            return notification;
        }
        
        public static Notification Init(string Title, string CurrentUserId, string AssignedUserId, DB.NotificationLevelEnum level, string Url = "")
        {
            var notification = new Notification
            {
                AssignedTo = AssignedUserId,
                Title = Title,
                CreatedBy = CurrentUserId,
                CreatedOn = DateTime.Now,
                NotificationLevel = (int)level,
                IsRead = (int)DB.YesNoEnum.No,
                IsEmailed = (int)DB.YesNoEnum.No,
                IsDeleted = (int)DB.YesNoEnum.No,
                Url = Url
            };

            return notification;
        }
    }
}
using System;

namespace portal.Models.Views.Notification
{

    public class NotificationViewModel 
    {
        public NotificationViewModel()
        {

        }
        public NotificationViewModel(DB.Infrastructure.ViewPointDB.Data.Notification note)
        {
            if (note == null)
            {
                throw new System.ArgumentNullException(nameof(note));
            }
            #region mapping
            Id = note.Id;
            Title = note.Title;
            Notes = note.Note;
            CreatedOn = note.CreatedOn;
            CreatedBy = note.CreatedBy;
            AssignedTo = note.AssignedTo;
            IsRead = (DB.YesNoEnum)note.IsRead;
            IsEmailed = (DB.YesNoEnum)note.IsEmailed;
            IsDeleted = (DB.YesNoEnum)note.IsDeleted;
            NotificationLevel = (DB.NotificationLevelEnum)note.NotificationLevel;
            WorkflowId = note.WorkflowId;
            TaskId = note.TaskId;
            Url = note.Url;
            ObjectTable = note.ObjectTable;
            KeyValue1 = note.KeyValue1;
            KeyValue2 = note.KeyValue2;
            KeyValue3 = note.KeyValue3;
            Controller = note.Controller;
            Action = note.Action;
            RoutedValues = note.RoutedValues;
        #endregion
        }

        public string NotificationLevelClass
        {
            get
            {
                return NotificationLevel switch
                {
                    DB.NotificationLevelEnum.Info => "fas fa-info-circle text-info",
                    DB.NotificationLevelEnum.Warning => "fa fa-exclamation-circle text-warning",
                    DB.NotificationLevelEnum.Danger => "fa fa-exclamation-circle text-danger",
                    _ => "",
                };
            }
        }

        public string NotificationMediaLeft { get; set; }

        public string TimeSpanString
        {
            get
            {
                TimeSpan span = (DateTime.Now - (DateTime)this.CreatedOn);

                var result = "";

                result += span.Days > 0 ? string.Format(AppCultureInfo.CInfo(), "{0}d", span.Days) : string.Empty;
                result += !string.IsNullOrEmpty(result) ? " and " : string.Empty;
                result += span.Hours > 0 ? string.Format(AppCultureInfo.CInfo(), "{0}h", span.Hours) : string.Empty;
                result += !string.IsNullOrEmpty(result) ? " and " : string.Empty;
                result += span.Minutes > 0 ? string.Format(AppCultureInfo.CInfo(), "{0}m", span.Minutes) : string.Empty;
                result += !string.IsNullOrEmpty(result) ? " ago" : string.Empty;

                return result;
            }
        }

        public Web.WebUserViewModel CreatedUser { get; set; }
            
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Notes { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string AssignedTo { get; set; }
        
        public DB.YesNoEnum IsRead { get; set; }
        
        public DB.YesNoEnum IsEmailed { get; set; }
        
        public DB.YesNoEnum IsDeleted { get; set; }
        
        public DB.NotificationLevelEnum NotificationLevel { get; set; }
        
        public int? WorkflowId { get; set; }
        
        public int? TaskId { get; set; }
        
        public string Url { get; set; }
        
        public string ObjectTable { get; set; }
        
        public string KeyValue1 { get; set; }
        
        public string KeyValue2 { get; set; }
        
        public string KeyValue3 { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string RoutedValues { get; set; }

    }
}
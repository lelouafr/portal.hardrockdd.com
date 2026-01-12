using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Web
{

    public class ErrorLogListViewModel
    {
        public ErrorLogListViewModel()
        {
            List = new List<ErrorLogViewModel>();
        }

        public ErrorLogListViewModel( DB.ErrorLogStatusEnum status)
        {
            Status = status;
            List = new List<ErrorLogViewModel>();
        }

        public ErrorLogListViewModel(VPContext db, DB.ErrorLogStatusEnum status)
        {
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            Status = status;
            List = db.ErrorLogs.Where(f => f.StatusCode == (int)status).AsEnumerable().Select(s => new ErrorLogViewModel(s)).ToList();
        }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Source")]
        public DB.ErrorLogStatusEnum Status { get; set; }

        public List<ErrorLogViewModel> List { get; }
    }
    
    public class ErrorLogViewModel
    {
        public ErrorLogViewModel()
        {

        }

        public ErrorLogViewModel(ErrorLog error)
        {
            if (error == null)
            {
                throw new System.ArgumentNullException(nameof(error));
            }
            ErrorId = error.ErrorId;
            UserId = error.UserId;
            LogDate = (DateTime)(error.LogDate ?? DateTime.Now);
            Controller = error.Controller;
            Action = error.Action;
            Parameters = error.Parameters;
            ExceptionMessage = error.ExceptionMessage;
            ErrorMessage = error.ErrorMessage;
            StackTrace = error.StackTrace;
            Source = error.Source;
            UrlRef = error.UrlReferrer;
            StatusCode = (DB.ErrorLogStatusEnum)(error.StatusCode ?? 0);
            Fixed = StatusCode == DB.ErrorLogStatusEnum.Fixed;
            User = new WebUserViewModel(UserId);
        }

        [Key]
        [HiddenInput]
        public int ErrorId { get; set; }
        
        [HiddenInput]
        public string UserId { get; set; }


        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "User")]
        public WebUserViewModel User { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Log Date")]
        public DateTime LogDate { get; set; }

        public string LogDateString { get { return LogDate.ToString("MM/dd/yyyy", AppCultureInfo.CInfo()); } }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Action")]
        public string Action { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Parameters")]
        public string Parameters { get; set; }

        [AllowHtml]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Exception Message")]
        public string ExceptionMessage { get; set; }

        [AllowHtml]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }

        [AllowHtml]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Stack Trace")]
        public string StackTrace { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Source")]
        public string Source { get; set; }
        
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "UrlRef")]
        public string UrlRef { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Status")]
        public DB.ErrorLogStatusEnum StatusCode { get; set; }


        [UIHint("SwitchBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Fixed")]
        public bool Fixed { get; set; }



    }
}

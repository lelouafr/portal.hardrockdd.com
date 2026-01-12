using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace portal.Models.Views.Web
{

    public class UserAccessLogListViewModel
    {
        public UserAccessLogListViewModel()
        {

        }
        public UserAccessLogListViewModel(DB.Infrastructure.ViewPointDB.Data.WebUserAccess userAccess)
        {
            if (userAccess == null)
            {
                throw new System.ArgumentNullException(nameof(userAccess));
            }
            UserId = userAccess.UserId;
            ControllerActionId = userAccess.ControllerActionId;
            var emp = userAccess.User.Employee.FirstOrDefault();
            UserName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", emp.FirstName, emp.LastName);
            List = userAccess.Logs.Select(s => new UserAccessLogViewModel(s)).ToList().OrderByDescending(o => o.LogDate).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int ControllerActionId { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Controller")]
        public string UserId { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }


        public List<UserAccessLogViewModel> List { get; }
    }

    public class UserAccessLogViewModel
    {
        public UserAccessLogViewModel()
        {

        }

        public UserAccessLogViewModel(DB.Infrastructure.ViewPointDB.Data.WebUserLog entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            UserId = entry.UserId;
            ControllerActionId = entry.ControllerActionId;
            AccessLevel = (DB.AccessLevelEnum?)entry.AccessLevel;
            LogDate = entry.LogDate;
            Parameters = entry.Parameters;
            var emp = entry.UserAccess.User.Employee.FirstOrDefault();
            UserName = string.Format(AppCultureInfo.CInfo(), "{0} {1}", emp.FirstName, emp.LastName);
        }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "ControllerActionId")]
        public int ControllerActionId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Access Level")]
        public DB.AccessLevelEnum? AccessLevel { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Log Date Time")]
        public DateTime LogDate { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Log Date Time")]
        public string LogDateString { get { return string.Format(AppCultureInfo.CInfo(), "{0} {1}", LogDate.ToShortDateString(), LogDate.ToShortTimeString()); } }

        [UIHint("TextAreaBox")]
        [Display(Name = "Parameters")]
        public string Parameters { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

    }
}
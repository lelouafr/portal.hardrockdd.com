using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace portal.Models.Views.Web
{
    public class UserAccessAddViewModel
    {
        public UserAccessAddViewModel()
        {

        }
        public UserAccessAddViewModel(int controllerActionId)
        {
            ControllerActionId = controllerActionId;
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int ControllerActionId { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        [Display(Name = "UserId")]
        public string UserId { get; set; }
    }

    public class UserAccessListViewModel
    {
        public UserAccessListViewModel()
        {

        }
        public UserAccessListViewModel(DB.Infrastructure.ViewPointDB.Data.WebController controller)
        {
            if (controller == null)
            {
                throw new System.ArgumentNullException(nameof(controller));
            }
            Id = controller.Id;
            Controller = controller.ControllerName;
            Action = controller.ActionName;
            Path = controller.Path;

            //var db = DB.Infrastructure.ViewPointDB.Data.VPContext.GetDbContextFromEntity(controller);
            //var emps = db.HRResources.Where(f => f.HRRef <= 200000 && f.ActiveYN == "Y").ToList();
            //var emp = entry.User.Employee.FirstOrDefault();
            List = controller.Users.Select(s => new UserAccessViewModel(s)).ToList().OrderBy(o => o.UserName).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Action")]
        public string Action { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Path")]
        public string Path { get; set; }


        public List<UserAccessViewModel> List { get; }
    }

    public class UserAccessViewModel
    {
        public UserAccessViewModel()
        {

        }

        public UserAccessViewModel(DB.Infrastructure.ViewPointDB.Data.WebUserAccess entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            UserId = entry.UserId;
            ControllerActionId = entry.ControllerActionId;
            AccessLevel = (DB.AccessLevelEnum?)entry.AccessLevel;
            LastAccessed = (entry.LastAccessed ?? DateTime.MinValue);
            //var emp = emps.FirstOrDefault(e => e.WebId == entry.UserId);
            var emp = entry.User.Employee.FirstOrDefault();
            if (emp != null)
            {
                UserName = string.Concat(emp.FirstName, " ", emp.LastName);
            }
            else
            {
                UserName = entry.User.UserName;
            }

            LastAccessString = LastAccessed.ToShortDateString();
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
        [Display(Name = "Last Accessed")]
        public DateTime LastAccessed { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Display(Name = "LastAccess")]
        public string LastAccessString { get; set; }

    }
}
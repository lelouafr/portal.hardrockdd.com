using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.Web
{
    public class ControllerViewModel
    {
        public ControllerViewModel()
        {

        }

        public ControllerViewModel(WebController controller)
        {
            if (controller == null) throw new System.ArgumentNullException(nameof(controller));
            Id = controller.Id;
            Controller = controller.ControllerName;
            Action = controller.ActionName;
            Path = controller.Path;

            Users = new UserAccessListViewModel(controller);
        }


        public ControllerViewModel(int controllerId)
        {
            using var db = new VPContext();
            var controller = db.WebControllers
                .Include("Users")
                .Include("Users.User")
                .Include("Users.User.Employee")
                .FirstOrDefault(f => f.Id == controllerId);
            Id = controller.Id;
            Controller = controller.ControllerName;
            Action = controller.ActionName;
            Path = controller.Path;

            Users = new UserAccessListViewModel(controller);
            AddUser = new UserAccessAddViewModel(controller.Id);

            CurrentUser = Users.List.FirstOrDefault(f => f.UserId == StaticFunctions.GetUserId());
        }

        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        public UserAccessListViewModel Users { get; set; }

        public UserAccessAddViewModel AddUser { get; set; }

        public UserAccessViewModel CurrentUser { get; }

    }

}
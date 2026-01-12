
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace portal.Models.Views.Web
{
    public class WebUserListViewModel
    {
        public WebUserListViewModel()
        {

        }

        public WebUserListViewModel(DB.Infrastructure.ViewPointDB.Data.VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            List = db.WebUsers
                        .OrderBy(o => o.FirstName)
                        .ThenBy(o => o.LastName)
                        .Include("Employee")
                        .Include("Employee.Position")
                        .ToList()
                        .Select(s => new WebUserViewModel(s))
                        .Where(w => w.Status == DB.WebUserStatusEnum.Active)
                        .ToList();
        }

        public List<WebUserViewModel> List { get; }
    }
    public class WebUserViewModel
    {
        public WebUserViewModel()
        {

        }

        public WebUserViewModel(string userId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var webUser = db.WebUsers
                   .Include("Employee")
                   .FirstOrDefault(f => f.Id == userId);
            if (webUser != null)
            {               
                var emp = webUser.Employee.FirstOrDefault();

                Id = webUser.Id;
                Email = webUser.Email;
                EmailConfirmed = webUser.EmailConfirmed;
                PasswordHash = webUser.PasswordHash;
                SecurityStamp = webUser.SecurityStamp;
                PhoneNumber = webUser.PhoneNumber;
                PhoneNumberConfirmed = webUser.PhoneNumberConfirmed;
                TwoFactorEnabled = webUser.TwoFactorEnabled;
                LockoutEndDateUtc = webUser.LockoutEndDateUtc;
                LockoutEnabled = webUser.LockoutEnabled;
                AccessFailedCount = webUser.AccessFailedCount;
                UserName = webUser.UserName;
                FirstName = webUser.FirstName;
                LastName = webUser.LastName;
                Discriminator = webUser.Discriminator;
                FullName = string.Format(AppCultureInfo.CInfo(), format: "{0} {1}", FirstName, LastName);
                JobTitle = "N/A";
                Status = DB.WebUserStatusEnum.Disabled;
                if (emp != null)
                {
                    JobTitle = emp.Position?.Description ?? " N/A";
                    Status = emp.ActiveYN == "N" && emp.HRRef != 100798 ? DB.WebUserStatusEnum.Disabled : DB.WebUserStatusEnum.Active;
                }
            }
        }

        public WebUserViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }

            
            var hrEmp = employee.Resource.FirstOrDefault();

            var resource = employee.Resource.FirstOrDefault();
            var email = (resource.CompanyEmail ?? resource.Email)?.ToLower(AppCultureInfo.CInfo());

            var webUser = employee.db.WebUsers.ToList().FirstOrDefault(f => f.Email?.ToLower(AppCultureInfo.CInfo()) == email);
            if (webUser == null)
            {
                webUser = employee.db.WebUsers.ToList().FirstOrDefault(f => f.Email?.ToLower(AppCultureInfo.CInfo()) == employee.Email.ToLower(AppCultureInfo.CInfo()));
            }

            if (webUser != null)
            {

                Id = webUser.Id;
                Email = webUser.Email;
                EmailConfirmed = webUser.EmailConfirmed;
                PasswordHash = webUser.PasswordHash;
                SecurityStamp = webUser.SecurityStamp;
                PhoneNumber = webUser.PhoneNumber;
                PhoneNumberConfirmed = webUser.PhoneNumberConfirmed;
                TwoFactorEnabled = webUser.TwoFactorEnabled;
                LockoutEndDateUtc = webUser.LockoutEndDateUtc;
                LockoutEnabled = webUser.LockoutEnabled;
                AccessFailedCount = webUser.AccessFailedCount;
                UserName = webUser.UserName;
                FirstName = webUser.FirstName;
                LastName = webUser.LastName;
                Discriminator = webUser.Discriminator;
                FullName = string.Format(AppCultureInfo.CInfo(), format: "{0} {1}", FirstName, LastName);
            }
            JobTitle = hrEmp?.Position?.Description ?? " N/A";
            Status = DB.WebUserStatusEnum.Disabled;
            if (hrEmp != null)
            {
                Status = hrEmp.ActiveYN == "N" && hrEmp.HRRef != 100798 ? DB.WebUserStatusEnum.Disabled : DB.WebUserStatusEnum.Active;
            }
        }


        public WebUserViewModel(DB.Infrastructure.ViewPointDB.Data.WebUser webUser)
        {
            if (webUser == null)
            {
                return;
            }
            var emp = webUser.Employee.FirstOrDefault();

            Id = webUser.Id;
            Email = webUser.Email;
            EmailConfirmed = webUser.EmailConfirmed;
            PasswordHash = webUser.PasswordHash;
            SecurityStamp = webUser.SecurityStamp;
            PhoneNumber = webUser.PhoneNumber;
            PhoneNumberConfirmed = webUser.PhoneNumberConfirmed;
            TwoFactorEnabled = webUser.TwoFactorEnabled;
            LockoutEndDateUtc = webUser.LockoutEndDateUtc;
            LockoutEnabled = webUser.LockoutEnabled;
            AccessFailedCount = webUser.AccessFailedCount;
            UserName = webUser.UserName;
            FirstName = webUser.FirstName;
            LastName = webUser.LastName;
            Discriminator = webUser.Discriminator;
            FullName = string.Format(AppCultureInfo.CInfo(), format: "{0} {1}", FirstName, LastName);
            JobTitle = "N/A";
            Status = DB.WebUserStatusEnum.Disabled;

            if (emp != null)
            {
                JobTitle = emp.Position?.Description ?? " N/A";
                Status = emp.ActiveYN == "N" && emp.HRRef != 100798 ? DB.WebUserStatusEnum.Disabled : DB.WebUserStatusEnum.Active;
            }
        }


        [Display(Name = "Status")]
        public DB.WebUserStatusEnum Status { get; set; }

        [Display(Name = "User")]
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool? PhoneNumberConfirmed { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool? LockoutEnabled { get; set; }

        public int? AccessFailedCount { get; set; }

        public string UserName { get; set; }

        public string Discriminator { get; set; }

        public string JobTitle { get; set; }
        
    }
}
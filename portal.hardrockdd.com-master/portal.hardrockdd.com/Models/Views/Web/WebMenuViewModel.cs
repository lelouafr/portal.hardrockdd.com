using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace portal.Models.Views.Web
{
    public class WebMenuListViewModel
    {
        public WebMenuListViewModel()
        {
            var userId = StaticFunctions.GetUserId();
            var memKey = "Menu_" + userId;
            if (!(MemoryCache.Default[memKey] is List<WebMenuViewModel> cache))
            {
                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
                };

                using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
                cache = db.WebMenus.Where(f => f.ParentMenuId == null)
                    .AsEnumerable()
                    .Select(s => new WebMenuViewModel(s, userId))
                    .ToList();
                systemCache.Set(memKey, cache, policy);
            }

            List = cache;

        }
        public List<WebMenuViewModel> List { get; }
    }
    
    public class WebMenuViewModel
    {
        public WebMenuViewModel()
        {


        }

        public WebMenuViewModel(WebMenu menuItem,string userId)
        {
            if (menuItem == null) throw new System.ArgumentNullException(nameof(menuItem));

            MenuId = menuItem.MenuId;
            Description = menuItem.Description;
            ParentId = menuItem.ParentMenuId;
            ControllerId = menuItem.ControllerActionId;
            IconClass = menuItem.IconClass;
            SortId = menuItem.SortId;
            Parameters = menuItem.Parameters;
            ControllerName = menuItem.Action?.ControllerName;
            ActionName = menuItem.Action?.ActionName;
            HRef = menuItem.Href;
            SubMenus = menuItem.SubMenus.Select(s => new WebMenuViewModel(s, userId)).ToList();
            HasAccess = true;
            if (menuItem.Action != null)
            {
                if (!menuItem.Action.Users.Any(f => f.UserId == userId && f.AccessLevel != 0))
                {
                    HasAccess = false;
                }
            }

            MenuCount = 0;
            AccessCount(SubMenus);
        }
        public bool HasAccess { get; set; }

        public long MenuCount
        {
            get; set;
        }

        public void AccessCount(List<WebMenuViewModel> subMenus)
        {
            if (subMenus == null) throw new System.ArgumentNullException(nameof(subMenus));

            if (HasAccess)
            {
                //MenuCount++;
            }
            foreach (var item in subMenus)
            {
                
                if (item.SubMenus.Count > 0)
                {
                    AccessCount(item.SubMenus);
                }
                else if (item.HasAccess)
                {
                    MenuCount++;
                }
            }
        }

        [Key]
        [Display(Name = "Id")]
        public int MenuId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Controller Id")]
        public int? ControllerId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "IconClass")]
        public string IconClass { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "HRef")]
        public string HRef { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "SortId")]
        public int SortId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Parameters")]
        public string Parameters { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Parameters")]
        public string ControllerName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Parameters")]
        public string ActionName { get; set; }

        public List<WebMenuViewModel> SubMenus { get; }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace portal
{
    public class LogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new System.ArgumentNullException(nameof(filterContext));
            }
            if (filterContext.Result is ViewResultBase view)
            {
                using var db = new VPContext();
                var form = FindCreateForm(filterContext, db);

                if (form != null)
                {
                    var user = FindCreateUserAccess(filterContext, form);
                    if (user != null)
                    {
                        var token = LogUser(filterContext, user);
                        view.ViewBag.ControllerActionToken = token;
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception )
                    {
                    }
                }
                if (form == null)
                {
                    return;
                }
                var userId = StaticFunctions.GetUserId();
                var memKey = "MainForm_" + userId;
                //var memKey = "MainForm_ViewBag.ViewOnly" + userId;
                ObjectCache systemCache = MemoryCache.Default;
                if (filterContext.Result is PartialViewResult)
                {
                    ///** Use Main Form access **/
                    //var mainForm = systemCache.Get(memKey) as WebController;
                    //if (mainForm != null)
                    //{
                    //    var model = new ControllerViewModel(mainForm.Id);
                    //    //view.ViewBag.ControllerViewModel = model;
                    //    var parentController = db.WebControllers.FirstOrDefault(f => f.Id == mainForm.Id);
                    //    var formLink = parentController.SubForms.FirstOrDefault(f => f.SubFormId == form.Id);
                    //    if (formLink == null)
                    //    {
                    //        formLink = new WebControllerSub();
                    //        formLink.FormId = parentController.Id;
                    //        formLink.SubFormId = form.Id;
                    //        formLink.InheritAccessFromParent = 1;
                    //        parentController.SubForms.Add(formLink);
                    //    }
                    //    db.SaveChanges();
                    //}
                }
                else
                {
                    var model = new ControllerViewModel(form.Id);
                    view.ViewBag.ControllerViewModel = model;
                    view.ViewBag.ControllerActionId = form.Id;
                    if (model?.CurrentUser != null)
                    {
                        //view.ViewBag.ViewOnly = model.CurrentUser.AccessLevel == DB.AccessLevelEnum.Full ? false : view.ViewBag.ViewOnly;
                    }
                    CacheItemPolicy policy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(20)
                    };
                    systemCache.Set(memKey, form, policy);

                    //memKey = "MainForm_ViewBag.ViewOnly_" + userId;
                    //systemCache.Set(memKey, view.ViewBag.ViewOnly, policy);

                }
            }
        }

        private static WebController FindCreateForm(ActionExecutedContext filterContext, VPContext db)
        {
            try
            {
                var connected = !db.WebUsers.Any();
            }
            catch (Exception)
            {
                return null;
            }
            if (filterContext.Result is ViewResultBase view)
            {
                string actionName = filterContext.RouteData.Values["action"].ToString();
                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string url = filterContext.HttpContext.Request.ApplicationPath;
                url = view.ViewName;

                if (controllerName == "Error")
                {
                    return null;
                }
                if (String.IsNullOrEmpty(url))
                {
                    url = string.Concat("../", controllerName, "/", actionName);
                }
                else
                {
                    if (!url.Contains("../"))
                    {
                        url = string.Concat("../", controllerName, "/", url);
                    }
                }
                
                var form = db.WebControllers.FirstOrDefault(f => f.ControllerName == controllerName && f.ActionName == actionName);
                if (form == null)
                {
                    var lineId = db.WebControllers.DefaultIfEmpty().Max(f => f == null ? 0 : f.Id) + 1;
                    form = new WebController
                    {
                        Id = lineId,
                        ControllerName = controllerName,
                        ActionName = actionName,
                        Path = url
                    };
                    db.WebControllers.Add(form);
                }
                return form;
            }

            return null;
        }

        private static WebUserAccess FindCreateUserAccess(ActionExecutedContext filterContext, WebController form)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }
            var userId = StaticFunctions.GetUserId();
            if (userId != null)
            {
                var user = form.Users.FirstOrDefault(f => f.UserId == userId);

                if (user == null && userId != null)
                {
                    user = new WebUserAccess
                    {
                        ControllerActionId = form.Id,
                        UserId = userId,
                        AccessLevel = (byte)DB.AccessLevelEnum.Denied
                    };
                    form.Users.Add(user);
                }
                user.LastAccessed = DateTime.Now;

                return user;
            }

            return null;
        }

        //[ValidateInput(false)]
        private static Guid LogUser(ActionExecutedContext filterContext, WebUserAccess user)
        {
            if (user != null)
            {
                var parameters = "";
                //var partialview = filterContext.Result as PartialViewResult;

                foreach (var key in filterContext.RouteData.Values.Keys.Where(f => f != "controller" && f != "action").ToList())
                {
                    if (!string.IsNullOrEmpty(parameters))
                    {
                        parameters += ", ";
                    }

                    parameters += string.Format(AppCultureInfo.CInfo(), "{0} = {1}", key, filterContext.RouteData.Values[key]);
                }
                if (filterContext.HttpContext.Request.QueryString != null)
                {
                    //parameters += filterContext.HttpContext.Request.QueryString;
                    foreach (var item in filterContext.HttpContext.Request.QueryString.ToDictionary())
                    {
                        if (item.Key != "__RequestVerificationToken")
                        {
                            if (!string.IsNullOrEmpty(parameters))
                            {
                                parameters += ", ";
                            }
                            parameters += string.Format(AppCultureInfo.CInfo(), "{0} = {1}", item.Key, item.Value);
                        }
                    }
                }
                if (filterContext.HttpContext.Request.Form != null)
                {
                    foreach (var item in filterContext.HttpContext.Request.Form.ToDictionary())
                    {
                        if (item.Key != "__RequestVerificationToken")
                        {
                            if (!string.IsNullOrEmpty(parameters))
                            {
                                parameters += ", ";
                            }
                            parameters += string.Format(AppCultureInfo.CInfo(), "{0} = {1}", item.Key, item.Value);
                        }
                    }
                    //string values = string.Join(",", filterContext.HttpContext.Request.Form.AllKeys.Select(key => filterContext.HttpContext.Request.Form[key]));
                    //parameters += values;
                }

                var log = new WebUserLog
                {
                    UserId = user.UserId,
                    ControllerActionId = user.ControllerActionId,
                    LogDate = (DateTime)user.LastAccessed,
                    AccessLevel = user.AccessLevel,
                    Parameters = parameters,
                    IPAddress = filterContext.HttpContext.Request.UserHostAddress,
                    SessionGuid = Guid.NewGuid(),
                };

                user.Logs.Add(log);

                return (Guid)log.SessionGuid;
            }

            return Guid.NewGuid();
        }
    }
}
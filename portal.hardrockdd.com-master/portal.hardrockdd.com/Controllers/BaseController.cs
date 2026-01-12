using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using portal.Repository.VP.WP; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Mvc;

namespace portal.Controllers
{
    [FilterIP]
    [Log]
    [ControllerAuthorize]
    [ControllerAccess]
    public class BaseController : Controller
    {
        internal readonly ErrorLogRepository errorRepo = new ErrorLogRepository();

        public JsonResult Json(object data)
        {
            return new Code.Services.JsonNetResult(data);
        }

        public JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            return new Code.Services.JsonNetResult(data, behavior);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new System.ArgumentNullException(nameof(filterContext));
            }

            //If the exeption is already handled we do nothing

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            if (controllerName == "Error" || (controllerName == "Account" && actionName == "Login"))
            {
                return;
            }

            var parameters = "";
            var viewTmp = filterContext.Result as ViewResult;
            var partialviewTmp = filterContext.Result as PartialViewResult;

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

            var errorModel = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var statusCode = 0;// ((HttpException)filterContext.Exception).GetHttpCode();
            var errorMsg = filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.Message : filterContext.Exception.Message;
            if (errorMsg.Contains("anti-forgery"))
            {
                statusCode = 1;
            }
            var model = new ErrorLog
            {
                UserId = StaticFunctions.GetUserId(),
                LogDate = DateTime.Now,
                Controller = controllerName,
                Action = actionName,
                Parameters = parameters,
                ExceptionMessage = errorMsg,
                StackTrace = filterContext.Exception.StackTrace,
                Source = filterContext.Exception.Source,
                StatusCode = statusCode,
                IP6Address = filterContext.HttpContext.Request.UserHostAddress,
                UrlReferrer = filterContext.HttpContext.Request?.UrlReferrer?.AbsolutePath
            };

            var url = filterContext.HttpContext.Request.UrlReferrer;

            model.Parameters = model.Parameters.Length > 4000 ? model.Parameters.Substring(0, 3999) : model.Parameters;

            if (!actionName.Contains("Combo"))
            {
                try
                {
                    errorRepo.Create(model);
                }
                catch (Exception)
                {

                    filterContext.Result = RedirectToAction("NoDBError", "Error");
                    return;
                }
            }
            else
            {
                try
                {
                    var rand = new Random();                    
                    System.Threading.Thread.Sleep(rand.Next(1000, 2000));
                    errorRepo.Create(model);
                   
                }
                catch (Exception)
                {

                    filterContext.Result = RedirectToAction("NoDBError", "Error");
                    return;
                }
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Headers.Add("errorId", string.Format("{0}", model.ErrorId));
            if (filterContext.HttpContext.Request.IsAjaxRequest() ||
                (filterContext.HttpContext.PreviousHandler != null &&
                filterContext.HttpContext.PreviousHandler is MvcHandler))
            {
                var result = RedirectToAction("IndexPartial", "Error", new { Area = "", ErrorId = model.ErrorId });
                var partialview = filterContext.Result as PartialViewResult;

                filterContext.Result = result;
            }
            else
            {
                filterContext.Result = RedirectToAction("Index", "Error", new { Area = "",ErrorId = model.ErrorId });
            }
        }

        protected ErrorLog LogError(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new System.ArgumentNullException(nameof(filterContext));
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var parameters = "";
            foreach (var key in filterContext.RouteData.Values.Keys.Where(f => f != "controller" && f != "action").ToList())
            {
                if (!string.IsNullOrEmpty(parameters))
                {
                    parameters += ", ";
                }

                parameters += string.Format(AppCultureInfo.CInfo(), "{0} = {1}", key, filterContext.RouteData.Values[key]);
            }

            var errorModel = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var statusCode = 0;// ((HttpException)filterContext.Exception).GetHttpCode();

            var model = new ErrorLog
            {
                UserId = StaticFunctions.GetUserId(),
                LogDate = DateTime.Now,
                Controller = controllerName,
                Action = actionName,
                Parameters = parameters,
                ExceptionMessage = filterContext.Exception.Message,
                StackTrace = filterContext.Exception.StackTrace,
                Source = filterContext.Exception.Source,
                StatusCode = statusCode
            };

            model = errorRepo.Create(model);

            return model;
        }

        protected virtual bool TryValidateModelRecursive(object model, Dictionary<object, bool> ValidatedModel = null)
        {

            if (model == null)
            {
                return TryValidateModel(model);
            }
            if (ValidatedModel == null)
            {
                ValidatedModel = new Dictionary<object, bool>();
            }
            Dictionary<string, ModelState> errors = new Dictionary<string, System.Web.Mvc.ModelState>(ModelState);

            foreach (var member in model.GetType().GetProperties())
            {
                var isList = member.PropertyType.IsGenericType && member.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
                var submodelTmp = member.GetValue(model, null);
                if (isList)
                {
                    var subList = member.GetValue(model, null) as IList;
                    if (subList == null) continue;
                    foreach (var submodel in subList)
                    {
                        if (submodel.GetType().Namespace.StartsWith("System", StringComparison.Ordinal)) continue;
                        //ModelState.Clear();
                        TryValidateModelRecursive(submodel, ValidatedModel);
                        ValidatedModel[submodel] = true;
                    }
                }
                else
                {
                    if (member.PropertyType.Namespace.StartsWith("System", StringComparison.Ordinal)) continue;
                    var submodel = member.GetValue(model, null);
                    if (submodel == null) continue;

                    if (!member.PropertyType.IsPublic) continue;

                    if (ValidatedModel.ContainsKey(submodel)) continue; //prevent infinite loop by circle-reference
                    ModelState.Clear();
                    TryValidateModelRecursive(submodel, ValidatedModel);
                    ValidatedModel[submodel] = true;
                }
                foreach (var error in ModelState)
                {
                    if (!errors.Keys.Any(a => a == error.Key))
                    {
                        errors.Add(member.Name + "." + error.Key, error.Value);
                    }

                }
            }
            ModelState.Clear();
            TryValidateModel(model);
            if (model.GetType().GetMethod("Validate") != null)
            {
                object[] parametersArray = new object[] { ModelState };
                var val = model.GetType().GetMethod("Validate");
                val.Invoke(model, parametersArray);
            }
            foreach (var key in errors.Keys)
            {
                if (!ModelState.ContainsKey(key))
                {
                    ModelState.Add(key, errors[key]);
                }
            }
            return ModelState.IsValid;
        }

        public ActionResult UserIdOverride(WebUserOverrideViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var curentUserId = User.Identity.GetUserId();
            var memKey = "UserIdOverRide_" + curentUserId;
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };

            systemCache.Set(memKey, model.UserId, policy);

            if ((MemoryCache.Default[memKey] is string OverrideUserId))
            {
                return Json(new { success = ModelState.IsValidJson() });
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        public ActionResult CompanyChange(WebCompanyOverrideViewModel model)
        {
            if (model == null) 
                throw new ArgumentNullException(nameof(model));

            StaticFunctions.SetCurrentCompany(model.CompanyId);
			//StaticFunctions.SetCurrentDivision(model.DivisionId);
			
			return Json(new { success = ModelState.IsValidJson() });
        }
    }
}
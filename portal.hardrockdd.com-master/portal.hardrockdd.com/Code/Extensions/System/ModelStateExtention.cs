using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal
{
    public static class ModelStateExtention
    {
        public static string IsValidJson(this ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            return modelState.IsValid.ToString(AppCultureInfo.CInfo()).ToLower(AppCultureInfo.CInfo());
        }

        public static List<ModelErrors> ModelErrors(this ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            var results = from x in modelState.Keys
                          where modelState[x].Errors.Count > 0
                          select new ModelErrors
                          {
                              key = x,
                              errors = modelState[x].Errors.Select(y => y.ErrorMessage).ToArray()
                          };

            return results.ToList();
        }
    }
    public class ModelErrors
    {
        public string key { get; set; }

        public string[] errors { get; set; }
    }
}
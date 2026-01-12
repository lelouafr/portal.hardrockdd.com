using DB.Infrastructure.ViewPointDB.Data;
using System;

namespace portal.Repository.VP.WP
{
    public partial class ErrorLogRepository : IDisposable
    {

        public static ErrorLog Init(string userId = "",
                                        string controller = "",
                                        string action = "",
                                        string parameters = "",
                                        string exceptionMessage = "",
                                        string errorMessage = "",
                                        string stackTrace = "",
                                        string source = "",
                                        int statusCode = 0)
        {
            var model = new ErrorLog
            {
                UserId = userId,
                LogDate = DateTime.Now,
                Controller = controller,
                Action = action,
                Parameters = parameters,
                ExceptionMessage = exceptionMessage,
                ErrorMessage = errorMessage,
                StackTrace = stackTrace,
                Source = source,
                StatusCode = statusCode
            };

            return model;
        }


    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class EmployeeRepository : IDisposable
    {
        public List<Employee> GetEmployees(byte PRCo, bool activeOnly = true)
        {
            if (!(MemoryCache.Default["GetEmployees"] is List<Employee> cache))
            {
                var qry = db.Employees
                        .Where(f => f.PRCo == PRCo && f.ActiveYN == (activeOnly ? "Y" : f.ActiveYN))
                        .ToList();

                cache = qry;
                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
                };
                systemCache.Set("GetEmployees", cache, policy);
            }
            return cache;
        }

        public static string FullName(Employee employee)
        {
            if (employee == null)
            {
                return "";
            }
            return string.Format(AppCultureInfo.CInfo(), "{0} {1}", employee.FirstName, employee.LastName);
        }

        public Employee GetEmployee(string userId)
        {
           
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var result = db.Employees.FirstOrDefault(f => f.Email.ToLower() == user.Email.ToLower());
            if (result == null)
            {
                result = db.Employees.FirstOrDefault(f => f.Resource.FirstOrDefault().CompanyEmail.ToLower() == user.Email.ToLower());
            }
            return result;
        }

        public Models.Views.Employee.EmployeeViewModel ProcessUpdate(Models.Views.Employee.EmployeeViewModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetEmployee(model.PRCo, model.EmployeeId);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.LastName = model.LastName;
                updObj.FirstName = model.FirstName;
                updObj.MidName = model.MidName;
                updObj.Phone = model.Phone;

                db.SaveChanges(modelState);
            }
            return new Models.Views.Employee.EmployeeViewModel(updObj);
        }

        public Employee Create(Models.Views.Employee.EmployeeViewModel model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            /****Write the changes to object****/
            var updObj = Init();

            updObj.PRCo = model.PRCo;
            updObj.EmployeeId = model.EmployeeId;
            updObj.LastName = model.LastName;
            updObj.FirstName = model.FirstName;
            updObj.MidName = model.MidName;
            updObj.Phone = model.Phone;

            return Create(updObj, modelState);
        }
    }
}

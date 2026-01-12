using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views
{
    public class AuditBaseViewModel
    {
        public AuditBaseViewModel()
        {

        }
        public AuditBaseViewModel(dynamic entity)
        {
            if (entity is object dbEntity)
            {
                string tableName = dbEntity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() is TableAttribute tableAttr ? tableAttr.Name : dbEntity.GetType().Name;
                if (tableName.IndexOf('_') > 0)
                {
                    tableName = tableName.Substring(0, tableName.IndexOf('_'));
                }

                tableName = tableName.Substring(0, tableName.Length > 30 ? 30 : tableName.Length);

                var keyNames = dbEntity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).ToList();
                var keyString = "";

                foreach (var key in keyNames)
                {
                    if (!string.IsNullOrEmpty(keyString))
                    {
                        keyString += " ";
                    }
                    var prop = dbEntity.GetType().GetProperty(key.Name);
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        keyString += string.Format(AppCultureInfo.CInfo(), "[{0}] = \"{1:MM/dd/yy}\"", key.Name, prop.GetValue(dbEntity, null));
                    }
                    else
                    {
                        keyString += string.Format(AppCultureInfo.CInfo(), "[{0}] = \"{1}\"", key.Name, prop.GetValue(dbEntity, null));
                    }
                }
                keyString += "";
                if (keyString == null)
                {
                    keyString = "";
                }

                EntityName = tableName;
                EntityKeyString = keyString;
            }
        }

        [HiddenInput]
        public string EntityName { get; set; }

        [HiddenInput]
        public string EntityKeyString { get; set; }
    }
}
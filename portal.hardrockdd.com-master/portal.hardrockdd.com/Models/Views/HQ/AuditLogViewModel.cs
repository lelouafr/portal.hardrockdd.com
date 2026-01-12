using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Web
{

    public class AuditLogListViewModel
    {
        public AuditLogListViewModel()
        {
            List = new List<AuditLogViewModel>();
        }


        public AuditLogListViewModel(VPContext db, string entityName, string keyString)
        {
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(entityName))
            {
                List = new List<AuditLogViewModel>();
            }
            else 
            {
                keyString = keyString.Replace(" ", "");
                keyString = keyString.Replace("]", string.Empty);
                keyString = keyString.Replace("[", string.Empty);
                keyString = keyString.Replace("\"", "");

                var results = db.HQMA_SearchResults(entityName, keyString).ToList();

                //var qry = db.vAudits.Where(f => (f.TableName == entityName || f.EntityName == entityName) && f.EntityKey.Contains(keyString));
                //var results = qry.ToList();
                var qry = new List<HQMA_SearchResults_Result>();
                switch (entityName)
                {
                    case "Employee":
                        entityName = "HRResource";
                        keyString = keyString.Replace("EmployeeId", "HRRef");
                        keyString = keyString.Replace("PRCo", "HRCo");

                        qry = db.HQMA_SearchResults(entityName, keyString).ToList();
                        results.AddRange(qry);

                        break;
                    case "HRResource":
                        entityName = "Employee";
                        keyString = keyString.Replace("HRRef", "EmployeeId");
                        keyString = keyString.Replace("HRCo", "PRCo");

                        qry = db.HQMA_SearchResults(entityName, keyString).ToList();
                        results.AddRange(qry);
                        break;
                    default:
                        break;
                }
                List = results.AsEnumerable().Select(s => new AuditLogViewModel(s)).ToList();
            }
            EntityName = entityName;
            EntityKeyString = keyString;

        }

        public string EntityName { get; set; }

        public string EntityKeyString { get; set; }

        public List<AuditLogViewModel> List { get; }
    }
    public class AuditLogViewModel
    {
        public AuditLogViewModel()
        {

        }

        public AuditLogViewModel(vAudit log)
        {
            if (log == null)
            {
                throw new System.ArgumentNullException(nameof(log));
            }
            TableName = log.TableName;
            KeyString = log.EntityKey;
            RecType = log.RecType;
            FieldName = log.FieldName;
            OldValue = log.OldValue;
            NewValue = log.NewValue;
            LogDate = string.Format(AppCultureInfo.CInfo(), "{0} {1}", log.DateTime.ToShortDateString(), log.DateTime.ToShortTimeString());
            UserName = log.UserName;

        }
        public AuditLogViewModel(HQMA_SearchResults_Result log)
        {
            if (log == null)
            {
                throw new System.ArgumentNullException(nameof(log));
            }
            TableName = log.TableName;
            KeyString = log.EntityKey;
            RecType = log.RecType;
            FieldName = log.FieldName;
            OldValue = log.OldValue;
            NewValue = log.NewValue;
            LogDate = string.Format(AppCultureInfo.CInfo(), "{0} {1}", ((DateTime)log.DateTime).ToShortDateString(), ((DateTime)log.DateTime).ToShortTimeString());
            UserName = log.UserName;

        }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }
        
        public string UserId { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "TableName")]
        public string TableName { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "KeyString")]
        public string KeyString { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "RecType")]
        public string RecType { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Field")]
        public string FieldName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Old Value")]
        public string OldValue { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "New Value")]
        public string NewValue { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "LogDate")]
        public string LogDate { get; set; }


        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }



    }
}

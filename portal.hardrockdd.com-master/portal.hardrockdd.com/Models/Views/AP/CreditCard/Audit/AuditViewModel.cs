using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Audit
{
    public class AuditListViewModel
    {
        public AuditListViewModel()
        {

        }

        public AuditListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            CCCo = (byte)company.PRCo;
            Mth = mth;
            //var trans = company.db.CreditTransactions.Where(f => f.CCCo == CCCo && f.Mth == mth).ToList();
            //var logs = trans.SelectMany(s => s.Logs).ToList();
			List = company.db.CreditTransactionLogs.Where(f => f.CCCo == CCCo && f.Transaction.Mth == mth).ToList().Select(s => new AuditViewModel(s)).ToList();
        }

        [Key]

        public byte CCCo { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        public List<AuditViewModel> List { get; }
    }

    public class AuditViewModel
    {
        public AuditViewModel()
        {

        }
        public AuditViewModel(CreditTransactionLog log)
        {
            if (log == null) throw new System.ArgumentNullException(nameof(log));
            CCCo = log.CCCo;
            TransId = log.TransId;
            AuditId = log.AuditId;
            AuditTypeId = (DB.CMLogEnum)log.AuditTypeId;
            LogDate = log.LogDate ?? DateTime.MinValue;
            LogBy = string.Format(AppCultureInfo.CInfo() ,"{0} {1}", log.LogUser.FirstName, log.LogUser.LastName);
            Description = log.Description;
        }

        [Key]
        public byte CCCo { get; set; }
        [Key]
        public long TransId { get; set; }
        [Key]
        public int AuditId { get; set; }

        public DB.CMLogEnum AuditTypeId { get; set; }

        public DateTime LogDate { get; set; }

        public string LogBy { get; set; }

        public string Description { get; set; }

    }

}
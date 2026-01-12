using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.Payment
{
    public class CSVZionCheckListModel
    {
        public CSVZionCheckListModel()
        {

            List = new List<CSVZionCheckModel>();
        }
        public CSVZionCheckListModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, int cMAcct)
        {
            if (batch == null)
            {
                List = new List<CSVZionCheckModel>();
                return;
            }

            List = batch.APPayments.Where(f => f.CMAcct == cMAcct && f.ChkType == "C").Select(s => new CSVZionCheckModel(s)).ToList();
        }
        public List<CSVZionCheckModel> List { get; }

    }

    public class CSVZionCheckModel
    {
        public CSVZionCheckModel()
        {

        }


        public CSVZionCheckModel(DB.Infrastructure.ViewPointDB.Data.APPayment payment)
        {
            if (payment == null)
                return;

            IsVoid = payment.VoidYN == "Y" ? "Y" : null;
            Account = payment.CMAccount.BankAcct;
            CheckNumber = payment.CMRef.Trim();
            CheckDate = payment.PaidDate.ToString("MM/dd/yy");
            Amount = payment.Amount.ToString();
            Vendor = payment.Name;
        }

        public string IsVoid { get; set; }
        public string Account { get; set; }

        public string CheckNumber { get; set; }
        //[Format("dd-MM-yy")]
        public string CheckDate { get; set; }
        public string Amount { get; set; }
        public string Vendor { get; set; }
    }
}
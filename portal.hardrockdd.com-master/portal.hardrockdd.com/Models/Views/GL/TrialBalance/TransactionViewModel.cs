using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.GL.TrialBalance
{

    public class TransactionListViewModel
    {
        public TransactionListViewModel()
        {

        }

        public TransactionListViewModel(int subsidiaryId, int year, int period, string accountNumber, VPContext db)
        {
            SubsidiaryId = subsidiaryId;
            Year = year;
            List = new List<TransactionViewModel>();
            List = db.udTransactionsDetails(subsidiaryId, year, period, accountNumber)
                                          .ToList()
                                          .Select(s => new TransactionViewModel(s))
                                          .ToList();


            //List = db.vTransactionsDetails.Where(f => f.SubsidiaryId == subsidiaryId && f.FinYear == year && f.FinPeriod == period && f.AccountNumber == accountNumber)
            //                              .ToList()
            //                              .Select(s => new TransactionViewModel(s))
            //                              .ToList();
        }

        //public TransactionListViewModel(TransactionListModel APIResults)
        //{
        //    if (APIResults.Items.Count != 0)
        //    {
        //        SubsidiaryId = APIResults.Items.FirstOrDefault().SubsidiaryId;
        //        Year = (int)APIResults.Items.FirstOrDefault().Year;
        //    }
        //    List = APIResults.Items.Select(s => new TransactionViewModel(s)).ToList();
        //}
        [Key]
        public int SubsidiaryId { get; set; }

        [Key]
        public int Year { get; set; }

        public List<TransactionViewModel> List { get; set; }
    }
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {

        }


        public TransactionViewModel(udTransactionsDetails_Result details)
        {
            SubsidiaryId = details.SubsidiaryId;
            Year = (int)details.FinYear;
            Month = (int)details.FinMonth;
            GLTrans = details.GLTrans;
            AccountId = details.AccountId;
            AccountName = details.AccountName;
            AccountNumber = details.AccountNumber;
            ActDate = details.ActDate;
            SourceCo = details.SourceCo;
            GLRef = details.GLRef;
            Jrnl = details.Jrnl;
            Source = details.Source;
            Description = details.Description;
            Amount = details.Amount;
        }

        public byte SubsidiaryId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int GLTrans { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public long AccountNumber { get; set; }
        //public System.DateTime DatePosted { get; set; }
        public System.DateTime ActDate { get; set; }
        public string Jrnl { get; set; }
        public byte? SourceCo { get; set; }
        public string GLRef { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public long TBKeyId { get; set; }
    }
}
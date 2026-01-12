using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.GL.TrialBalance
{
    public class ActTrialBalanceListViewModel
    {
        public ActTrialBalanceListViewModel()
        {

        }

        public ActTrialBalanceListViewModel(int subsidiaryId, int year, VPContext db)
        {
            SubsidiaryId = subsidiaryId;
            Year = year;

            List = db.udTrialBalance(subsidiaryId, year).ToList().Select(s => new ActTrialBalanceViewModel(s)).ToList();
            //List = db.vTrialBalances.Where(f => f.SubsidiaryId == subsidiaryId && f.FinYear == year)
            //            .ToList()
            //            .Select(s => new TrialBalanceViewModel(s))
            //            .ToList();
        }

        //public TrialBalanceListViewModel(TrialBalanceListModel APIResults)
        //{
        //    if (APIResults.Items.Count != 0)
        //    {
        //        SubsidiaryId = APIResults.Items.FirstOrDefault().SubsidiaryId;
        //        Year = (int)APIResults.Items.FirstOrDefault().Year;
        //    }
        //    List = APIResults.Items.Select(s => new TrialBalanceViewModel(s)).ToList();
        //}
        [Key]
        public int SubsidiaryId { get; set; }

        [Key]
        public int Year { get; set; }

        public List<ActTrialBalanceViewModel> List { get; set; }
    }
    public class ActTrialBalanceViewModel
    {
        public ActTrialBalanceViewModel()
        {

        }

        public ActTrialBalanceViewModel(udTrialBalance_Result trialBalance)
        {
            SubsidiaryId = trialBalance.SubsidiaryId;
            Year = trialBalance.FinYear;
            Month = trialBalance.FinPeriod;
            GLAcct = trialBalance.GLAcct;
            AccountId = trialBalance.AccountId;
            AccountNumber = trialBalance.AccountNumber.ToString();
            AccountName = trialBalance.AccountName;
            Amount = (double?)trialBalance.Amount;
        }

        //public TrialBalanceViewModel(TrialBalanceModel APITrialbalance)
        //{
        //    SubsidiaryId = APITrialbalance.SubsidiaryId;
        //    Year = APITrialbalance.Year;
        //    Month = APITrialbalance.Month;
        //    AccountId = APITrialbalance.AccountId;
        //    AccountNumber = APITrialbalance.AccountNumber;
        //    AccountName = APITrialbalance.AccountName;
        //    Amount = APITrialbalance.Amount;
        //}

        [Key]
        public int? SubsidiaryId { get; set; }

        [Key]
        public int? Year { get; set; }

        [Key]
        public int? Month { get; set; }
        
        public string AccountId { get; set; }        

        [Key]
        public string GLAcct { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public double? Amount { get; set; }
    }
}
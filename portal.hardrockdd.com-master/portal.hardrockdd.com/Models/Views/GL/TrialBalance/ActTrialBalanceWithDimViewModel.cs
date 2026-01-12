using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.GL.TrialBalance
{
    public class ActTrialBalanceWithDimListViewModel
    {
        public ActTrialBalanceWithDimListViewModel()
        {

        }

        public ActTrialBalanceWithDimListViewModel(int subsidiaryId, int year, VPContext db)
        {
            SubsidiaryId = subsidiaryId;
            Year = year;

            List = db.udTrialBalanceWithDim(subsidiaryId, year).ToList().Select(s => new ActTrialBalanceWithDimViewModel(s)).ToList();
            //var tbList = db.vTrialBalanceWithDims.Where(f => f.SubsidiaryId == subsidiaryId && f.FinYear == year).ToList();
            //List = db.vTrialBalanceWithDims.Where(f => f.SubsidiaryId == subsidiaryId && f.FinYear == year)
            //            .AsEnumerable()
            //            .Select(s => new TrialBalanceWithDimViewModel(s))
            //            .ToList();
        }

        //public TrialBalanceWithDimListViewModel(TrialBalanceWithDimListModel APIResults)
        //{
        //    if (APIResults.Items.Count != 0)
        //    {
        //        SubsidiaryId = APIResults.Items.FirstOrDefault().SubsidiaryId;
        //        Year = (int)APIResults.Items.FirstOrDefault().Year;
        //    }
        //    List = APIResults.Items.Select(s => new TrialBalanceWithDimViewModel(s)).ToList();
        //}
        [Key]
        public int SubsidiaryId { get; set; }

        [Key]
        public int Year { get; set; }

        public List<ActTrialBalanceWithDimViewModel> List { get; set; }
    }
    public class ActTrialBalanceWithDimViewModel
    {
        public ActTrialBalanceWithDimViewModel()
        {

        }

        public ActTrialBalanceWithDimViewModel(udTrialBalanceWithDim_Result trialBalance)
        {
            SubsidiaryId = trialBalance.SubsidiaryId;
            Year = trialBalance.FinYear;
            Month = trialBalance.FinPeriod;
            GLAcct = trialBalance.GLAcct;
            AccountId = trialBalance.AccountId;
            AccountNumber = trialBalance.AccountNumber.ToString();
            AccountName = trialBalance.AccountName;
            JobId = trialBalance.JobId;
            JobName = trialBalance.JobName;
            CustomerId = trialBalance.CustomerId;
            CustomerName = trialBalance.CustomerName;
            //Class = trialBalance.ClassId;
            //ClassName = trialBalance.ClassName;
            //Company = trialBalance.EntityId;
            //CompanyName = trialBalance.EntityName;
            Amount = (double?)trialBalance.Amount;
        }
        //public TrialBalanceWithDimViewModel(TrialBalanceWithDimModel APITrialbalance)
        //{
        //    SubsidiaryId = APITrialbalance.SubsidiaryId;
        //    Year = APITrialbalance.Year;
        //    Month = APITrialbalance.Month;
        //    AccountId = APITrialbalance.AccountId;
        //    AccountNumber = APITrialbalance.AccountNumber;
        //    AccountName = APITrialbalance.AccountName;
        //    Department = APITrialbalance.Department;
        //    DepartmentName = APITrialbalance.DepartmentName;
        //    Location = APITrialbalance.Location;
        //    LocationName = APITrialbalance.LocationName;
        //    Class = APITrialbalance.Class;
        //    ClassName = APITrialbalance.ClassName;
        //    Company = APITrialbalance.Company;
        //    CompanyName = APITrialbalance.CompanyName;
        //    Amount = APITrialbalance.Amount;
        //}

        [Key]
        public int? SubsidiaryId { get; set; }

        [Key]
        public int? Year { get; set; }

        [Key]
        public int? Month { get; set; }

        [Key]
        public string GLAcct { get; set; }

        public string AccountId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string? JobId { get; set; }

        public string JobName { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        //public int? Class { get; set; }

        //public string ClassName { get; set; }

        //public int? Company { get; set; }

        //public string CompanyName { get; set; }

        public double? Amount { get; set; }
    }
}
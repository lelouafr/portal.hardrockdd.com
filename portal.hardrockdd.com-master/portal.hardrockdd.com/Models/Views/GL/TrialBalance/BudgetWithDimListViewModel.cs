using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.GL.TrialBalance
{
    public class BudgetWithDimListViewModel
    {
        public BudgetWithDimListViewModel()
        {

        }
        public BudgetWithDimListViewModel(int subsidiaryId, int year, VPContext db)
        {
            SubsidiaryId = subsidiaryId;
            Year = year;
            //List = db.vBudgetBalanceWithDims
            //        .Where(f => f.SubsidiaryId == subsidiaryId && f.FinYear == year)
            //        .AsEnumerable()
            //        .Select(s => new BudgetWithDimViewModel(s))
            //        .ToList();

            List = new List<BudgetWithDimViewModel>();
        }

        //public BudgetWithDimListViewModel(BudgetWithDimListModel APIResults)
        //{
        //    if (APIResults.Items.Count != 0)
        //    {
        //        SubsidiaryId = APIResults.Items.FirstOrDefault().SubsidiaryId;
        //        Year = (int)APIResults.Items.FirstOrDefault().Year;
        //    }
        //    List = APIResults.Items.Select(s => new BudgetWithDimViewModel(s)).ToList();
        //}
        [Key]
        public int SubsidiaryId { get; set; }

        [Key]
        public int Year { get; set; }

        public List<BudgetWithDimViewModel> List { get; set; }
    }

    public class BudgetWithDimViewModel
    {
        public BudgetWithDimViewModel()
        {

        }
        //public BudgetWithDimViewModel(vBudgetBalanceWithDim vBudgetBalanceWithDim)
        //{
        //    SubsidiaryId = vBudgetBalanceWithDim.SubsidiaryId;
        //    Year = vBudgetBalanceWithDim.FinYear;
        //    Month = vBudgetBalanceWithDim.FinPeriod;
        //    AccountId = vBudgetBalanceWithDim.AccountId;
        //    AccountNumber = vBudgetBalanceWithDim.AccountNumber.ToString();
        //    AccountName = vBudgetBalanceWithDim.AccountName;
        //    Department = vBudgetBalanceWithDim.DepartmentId;
        //    DepartmentName = vBudgetBalanceWithDim.DepartmentName;
        //    Location = vBudgetBalanceWithDim.LocationId;
        //    LocationName = vBudgetBalanceWithDim.LocationName;
        //    Class = vBudgetBalanceWithDim.ClassId;
        //    ClassName = vBudgetBalanceWithDim.ClassName;
        //    Company = vBudgetBalanceWithDim.EntityId;
        //    CompanyName = vBudgetBalanceWithDim.EntityName;
        //    Budget = (double?)vBudgetBalanceWithDim.Amount;
        //}


        //public BudgetWithDimViewModel(BudgetWithDimModel APITrialbalance)
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
        //    Budget = APITrialbalance.Budget;
        //}

        [Key]
        public int? SubsidiaryId { get; set; }

        [Key]
        public int? Year { get; set; }

        [Key]
        public int? Month { get; set; }

        [Key]
        public int? AccountId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public int? Department { get; set; }

        public string DepartmentName { get; set; }

        public int? Location { get; set; }

        public string LocationName { get; set; }

        public int? Class { get; set; }

        public string ClassName { get; set; }

        public int? Company { get; set; }

        public string CompanyName { get; set; }

        public double? Budget { get; set; }
    }
}
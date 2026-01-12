using AutoMapper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Import
{
    public class WexImportLinesViewModel
    {
        public WexImportLinesViewModel()
        {
            List = new System.Collections.Generic.List<Models.Views.AP.CreditCard.Import.WexImportViewModel>();

        }

        public WexImportLinesViewModel(DB.Infrastructure.ViewPointDB.Data.CreditImport import)
        {

            Co = import.Co;
            ImportId = import.ImportId;
            List = import.WexLines.Select(s => new WexImportViewModel(s)).ToList();
        }

        [Key]
        public byte Co { get; set; }
        [Key]
        public int ImportId { get; set; }

        public List<WexImportViewModel> List { get; }
    }
        

    public class WexImportViewModel
    {

        public WexImportViewModel()
        {

        }

        public WexImportViewModel(DB.Infrastructure.ViewPointDB.Data.CCWexImport importLine)
        {
            CMCo = importLine.CMCo;
            ImportId = importLine.ImportId;
            ImportLineId = importLine.ImportLineId;
            TransactionDate = importLine.TransactionDate;
            TransactionTime = importLine.TransactionTime;
            PostDate = importLine.PostDate;
            AccountNumber = importLine.AccountNumber;
            AccountName = importLine.AccountName;
            CardNumber = importLine.CardNumber;
            EmbossLine2 = importLine.EmbossLine2;
            CustomVehicleAssetId = importLine.CustomVehicleAssetId;
            Units = importLine.Units;
            UnitofMeasure = importLine.UnitofMeasure;
            UnitCost = importLine.UnitCost;
            TotalFuelCost = importLine.TotalFuelCost;
            ServiceCost = importLine.ServiceCost;
            OtherCost = importLine.OtherCost;
            TotalNonFuelCost = importLine.TotalNonFuelCost;
            GrossCost = importLine.GrossCost;
            ExemptTax = importLine.ExemptTax;
            Discount = importLine.Discount;
            NetCost = importLine.NetCost;
            ReportedTax = importLine.ReportedTax;
            Product = importLine.Product;
            ProductDescription = importLine.ProductDescription;
            TransactionDescription = importLine.TransactionDescription;
            MerchantBrand = importLine.MerchantBrand;
            MerchantName = importLine.MerchantName;
            MerchantAddress = importLine.MerchantAddress;
            MerchantCity = importLine.MerchantCity;
            MerchantStateProvince = importLine.MerchantStateProvince;
            MerchantPostalCode = importLine.MerchantPostalCode;
            MerchantSiteId = importLine.MerchantSiteId;
            CurrentOdometer = importLine.CurrentOdometer;
            AdjustedOdometer = importLine.AdjustedOdometer;
            PreviousOdometer = importLine.PreviousOdometer;
            DistanceDriven = importLine.DistanceDriven;
            FuelEconomy = importLine.FuelEconomy;
            CostPerDistance = importLine.CostPerDistance;
            VehicleDescription = importLine.VehicleDescription;
            VIN = importLine.VIN;
            TankCapacity = importLine.TankCapacity;
            InServiceDate = importLine.InServiceDate;
            StartOdometer = importLine.StartOdometer;
            DriverLastName = importLine.DriverLastName;
            DriverFirstName = importLine.DriverFirstName;
            DriverMiddleName = importLine.DriverMiddleName;
            DriverDepartment = importLine.DriverDepartment;
            PRCo = importLine.PRCo;
            PREmployeeId = importLine.PREmployeeId;
            EmployeeID = importLine.EmployeeID;
            TransactionTicketNumber = importLine.TransactionTicketNumber;
            CurrencyExchangeRate = importLine.CurrencyExchangeRate;
            RebateCode = importLine.RebateCode;
            DriverPromptId = importLine.DriverPromptId;
            VehiclePromptId = importLine.VehiclePromptId;
            Department = importLine.Department;
            TankCapacity2 = importLine.TankCapacity2;
            EMCo = importLine.EMCo;
            EMEquipmentId = importLine.EMEquipmentId;
            IsError = importLine.IsError;
            TransactionDescription = importLine.TransactionDescription;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Co")]
        public byte CMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "ImportId")]
        public int ImportId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int ImportLineId { get; set; }

        [UIHint("DateBox")]
        [Name("Transaction Date")]
        public DateTime? TransactionDate { get; set; }

        [UIHint("DateBox")]
        [Name("Transaction Time")]
        public DateTime? TransactionTime { get; set; }

        [UIHint("DateBox")]
        [Name("Post Date")]
        public DateTime? PostDate { get; set; }

        [UIHint("TextBox")]
        [Name("Account Number")]
        public long? AccountNumber { get; set; }

        [UIHint("TextBox")]
        [Name("Account Name")]
        public string AccountName { get; set; }

        [UIHint("TextBox")]
        [Name("Card Number")]
        public string CardNumber { get; set; }

        [UIHint("TextBox")]
        [Name("Emboss Line 2")]
        public string EmbossLine2 { get; set; }

        [UIHint("TextBox")]
        [Name("Custom Vehicle/Asset ID")]
        public string CustomVehicleAssetId { get; set; }

        [UIHint("TextBox")]
        [Name("Units")]
        public decimal? Units { get; set; }

        [UIHint("TextBox")]
        [Name("Unit of Measure")]
        public string UnitofMeasure { get; set; }

        [UIHint("TextBox")]
        [Name("Unit Cost")]
        public decimal? UnitCost { get; set; }

        [UIHint("TextBox")]
        [Name("Total Fuel Cost")]
        public decimal? TotalFuelCost { get; set; }

        [UIHint("TextBox")]
        [Name("Service Cost")]
        public decimal? ServiceCost { get; set; }

        [UIHint("TextBox")]
        [Name("Other Cost")]
        public decimal? OtherCost { get; set; }

        [UIHint("TextBox")]
        [Name("Total Non-Fuel Cost")]
        public decimal? TotalNonFuelCost { get; set; }

        [UIHint("TextBox")]
        [Name("Gross Cost")]
        public decimal? GrossCost { get; set; }

        [UIHint("TextBox")]
        [Name("Exempt Tax")]
        public decimal? ExemptTax { get; set; }

        [UIHint("TextBox")]
        [Name("Discount")]
        public decimal? Discount { get; set; }

        [UIHint("TextBox")]
        [Name("Net Cost")]
        public decimal? NetCost { get; set; }

        [UIHint("TextBox")]
        [Name("Reported Tax")]
        public decimal? ReportedTax { get; set; }

        [UIHint("TextBox")]
        [Name("Product")]
        public string Product { get; set; }

        [UIHint("TextBox")]
        [Name("Product Description")]
        public string ProductDescription { get; set; }

        [UIHint("TextBox")]
        [Name("Transaction Description")]
        public string TransactionDescription { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant (Brand)")]
        public string MerchantBrand { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant Name")]
        public string MerchantName { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant Address")]
        public string MerchantAddress { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant City")]
        public string MerchantCity { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant State / Province")]
        public string MerchantStateProvince { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant Postal Code")]
        public string MerchantPostalCode { get; set; }

        [UIHint("TextBox")]
        [Name("Merchant Site ID")]
        public string MerchantSiteId { get; set; }

        [UIHint("TextBox")]
        [Name("Current Odometer")]
        public decimal? CurrentOdometer { get; set; }

        [UIHint("TextBox")]
        [Name("Adjusted Odometer")]
        public decimal? AdjustedOdometer { get; set; }

        [UIHint("TextBox")]
        [Name("Previous Odometer")]
        public decimal? PreviousOdometer { get; set; }

        [UIHint("TextBox")]
        [Name("Distance Driven")]
        public decimal? DistanceDriven { get; set; }

        [UIHint("TextBox")]
        [Name("Fuel Economy")]
        public decimal? FuelEconomy { get; set; }

        [UIHint("TextBox")]
        [Name("Cost Per Distance")]
        public decimal? CostPerDistance { get; set; }

        [UIHint("TextBox")]
        [Name("Vehicle Description")]
        public string VehicleDescription { get; set; }

        [UIHint("TextBox")]
        [Name("VIN")]
        public string VIN { get; set; }

        [UIHint("TextBox")]
        [Name("Tank Capacity")]
        public decimal? TankCapacity { get; set; }

        [UIHint("DateBox")]
        [Name("In Service Date")]
        public DateTime? InServiceDate { get; set; }

        [UIHint("TextBox")]
        [Name("Start Odometer")]
        public decimal? StartOdometer { get; set; }

        [UIHint("TextBox")]
        [Name("Driver Last Name")]
        public string DriverLastName { get; set; }

        [UIHint("TextBox")]
        [Name("Driver First Name")]
        public string DriverFirstName { get; set; }

        [UIHint("TextBox")]
        [Name("Driver Middle Name")]
        public string DriverMiddleName { get; set; }

        [UIHint("TextBox")]
        [Name("Driver Department")]
        public string DriverDepartment { get; set; }

        [UIHint("TextBox")]
        [Name("Employee ID")]
        public int? EmployeeID { get; set; }

        [UIHint("TextBox")]
        [Name("Transaction Ticket Number")]
        public string TransactionTicketNumber { get; set; }

        [UIHint("TextBox")]
        [Name("Currency Exchange Rate")]
        public string CurrencyExchangeRate { get; set; }

        [UIHint("TextBox")]
        [Name("Rebate Code")]
        public string RebateCode { get; set; }

        [UIHint("TextBox")]
        [Name("Driver Prompt ID")]
        public int? DriverPromptId { get; set; }

        [UIHint("TextBox")]
        [Name("Vehicle Prompt ID")]
        public string VehiclePromptId { get; set; }

        [UIHint("TextBox")]
        [Name("Department")]
        public string Department { get; set; }

        [UIHint("TextBox")]
        [Name("Tank Capacity 2")]
        public string TankCapacity2 { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "PRCo")]
        public byte? PRCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Employee Id")]
        public int? PREmployeeId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "EMCo")]
        public byte? EMCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Equipment Id")]
        public string EMEquipmentId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Error")]
        public bool? IsError { get; set; }
    }
}
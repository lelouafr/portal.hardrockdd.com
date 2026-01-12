using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public class WexCSV
    {
        [Name("Transaction Date")]
        public DateTime? TransactionDate { get; set; }
        [Name("Transaction Time")]
        public DateTime? TransactionTime { get; set; }
        [Name("Post Date")]
        public DateTime? PostDate { get; set; }
        [Name("Account Number")]
        public long? AccountNumber { get; set; }
        [Name("Account Name")]
        public string AccountName { get; set; }
        [Name("Card Number")]
        public string CardNumber { get; set; }
        [Name("Emboss Line 2")]
        public string EmbossLine2 { get; set; }
        [Name("Custom Vehicle/Asset ID")]
        public string CustomVehicleAssetId { get; set; }
        [Name("Units")]
        public decimal? Units { get; set; }
        [Name("Unit of Measure")]
        public string UnitofMeasure { get; set; }
        [Name("Unit Cost")]
        public decimal? UnitCost { get; set; }
        [Name("Total Fuel Cost")]
        public decimal? TotalFuelCost { get; set; }
        [Name("Service Cost")]
        public decimal? ServiceCost { get; set; }

        [Name("Other Cost")]
        public decimal? OtherCost { get; set; }
        [Name("Total Non-Fuel Cost")]
        public decimal? TotalNonFuelCost { get; set; }
        [Name("Gross Cost")]
        public decimal? GrossCost { get; set; }
        [Name("Exempt Tax")]
        public decimal? ExemptTax { get; set; }
        [Name("Discount")]
        public decimal? Discount { get; set; }
        [Name("Net Cost")]
        public decimal? NetCost { get; set; }
        [Name("Reported Tax")]
        public decimal? ReportedTax { get; set; }
        [Name("Transaction Fee Type 1")]
        public string TransactionFeeType1 { get; set; }
        [Name("Transaction Fee Amount 1")]
        public decimal? TransactionFeeAmount1 { get; set; }
        [Name("Transaction Fee Type 2")]
        public string TransactionFeeType2 { get; set; }
        [Name("Transaction Fee Amount 2")]
        public decimal? TransactionFeeAmount2 { get; set; }
        [Name("Transaction Fee Type 3")]
        public string TransactionFeeType3 { get; set; }
        [Name("Transaction Fee Amount 3")]
        public decimal? TransactionFeeAmount3 { get; set; }
        [Name("Transaction Fee Type 4")]
        public string TransactionFeeType4 { get; set; }
        [Name("Transaction Fee Amount 4")]
        public decimal? TransactionFeeAmount4 { get; set; }
        [Name("Transaction Fee Type 5")]
        public string TransactionFeeType5 { get; set; }
        [Name("Transaction Fee Amount 5")]
        public decimal? TransactionFeeAmount5 { get; set; }
        [Name("Product")]
        public string Product { get; set; }
        [Name("Product Description")]
        public string ProductDescription { get; set; }
        [Name("Transaction Description")]
        public string TransactionDescription { get; set; }
        [Name("Merchant (Brand)")]
        public string MerchantBrand { get; set; }
        [Name("Merchant Name")]
        public string MerchantName { get; set; }
        [Name("Merchant Address")]
        public string MerchantAddress { get; set; }
        [Name("Merchant City")]
        public string MerchantCity { get; set; }
        [Name("Merchant State / Province")]
        public string MerchantStateProvince { get; set; }
        [Name("Merchant Postal Code")]
        public string MerchantPostalCode { get; set; }
        [Name("Merchant Site ID")]
        public string MerchantSiteId { get; set; }
        [Name("Current Odometer")]
        public decimal? CurrentOdometer { get; set; }
        [Name("Adjusted Odometer")]
        public decimal? AdjustedOdometer { get; set; }
        [Name("Previous Odometer")]
        public decimal? PreviousOdometer { get; set; }
        [Name("Distance Driven")]
        public decimal? DistanceDriven { get; set; }
        [Name("Fuel Economy")]
        public decimal? FuelEconomy { get; set; }
        [Name("Cost Per Distance")]
        public decimal? CostPerDistance { get; set; }
        [Name("Vehicle Description")]
        public string VehicleDescription { get; set; }
        [Name("VIN")]
        public string VIN { get; set; }
        [Name("Tank Capacity")]
        public decimal? TankCapacity { get; set; }
        [Name("In Service Date")]
        public DateTime? InServiceDate { get; set; }
        [Name("Start Odometer")]
        public decimal? StartOdometer { get; set; }
        [Name("Driver Last Name")]
        public string DriverLastName { get; set; }
        [Name("Driver First Name")]
        public string DriverFirstName { get; set; }
        [Name("Driver Middle Name")]
        public string DriverMiddleName { get; set; }
        [Name("Driver Department")]
        public string DriverDepartment { get; set; }
        [Name("Employee ID")]
        public int? EmployeeID { get; set; }
        [Name("Transaction Ticket Number")]
        public string TransactionTicketNumber { get; set; }
        [Name("Currency Exchange Rate")]
        public string CurrencyExchangeRate { get; set; }
        [Name("Rebate Code")]
        public string RebateCode { get; set; }
        [Name("Driver Prompt ID")]
        public int? DriverPromptId { get; set; }
        [Name("Vehicle Prompt ID")]
        public string VehiclePromptId { get; set; }
        [Name("Department")]
        public string Department { get; set; }
        [Name("Tank Capacity 2")]
        public string TankCapacity2 { get; set; }

    }
}
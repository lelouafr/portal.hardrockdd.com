using AutoMapper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public class ZionCSV
    {

        /************/
        [Name("Company - Name")]
        public string CompanyName { get; set; }

        [Name("Statement Period - Start Date")]
        public DateTime? StatementStartDate { get; set; }

        [Name("Statement Period - End Date")]
        public DateTime? StatementEndDate { get; set; }

        [Name("Employee - CO")]
        public byte? CardEmployeePRCo { get; set; }

        [Name("Employee - ID")]
        public int? CardEmployeeId { get; set; }
        
        [Name("Employee - First Name")]
        public string EmployeeFirstName { get; set; }

        [Name("Employee - Last Name")]
        public string EmployeeLastName { get; set; }

        [Name("Employee - Email Address")]
        public string EmployeeEmail { get; set; }

        [Name("Cardholder - First Name")]
        public string CardHolderFirstName { get; set; }

        [Name("Cardholder - Last Name")]
        public string CardHolderLastName { get; set; }

        [Name("Transaction - Transaction Reference")]
        public string TransactionReference { get; set; }

        [Name("Transaction - Line Number")]
        public int? TransLineNumber { get; set; }

        [Name("Transaction - Sequence Reference")]
        public int? TransSequenceReference { get; set; }

        [Name("Transaction - Sequence Number")]
        public string TransSequenceNumber { get; set; }

        [Name("Transaction - Description")]
        public string TransactionDescription { get; set; }

        [Name("Transaction - Secondary Description")]
        public string TransactionSecondDescription { get; set; }

        [Name("Transaction - Posting Date")]
        public DateTime? TransPostDate { get; set; }

        [Name("Transaction - Transaction Date")]
        public DateTime? TransDate { get; set; }

        [Name("Transaction - Transaction Type")]
        public string TransactionType { get; set; }

        [Name("Transaction - Billing Currency Code")]
        public string TransactionBillingCurrencyCode { get; set; }

        [Name("Transaction - Billing Amount")]
        public decimal? TransBillingAmount { get; set; }

        [Name("Transaction - Line Amount")]
        public decimal? TransLineAmount { get; set; }

        [Name("Transaction - Line Tax Amount")]
        public decimal? TransLineTaxAmount { get; set; }

        [Name("Transaction - Transaction Status ID")]
        public int? TransStatusId { get; set; }

        [Name("Transaction - Transaction Status Description")]
        public string TransactionStatusDescription { get; set; }

        [Name("Supplier - ID")]
        public string MerchantId { get; set; }

        [Name("Supplier - Name")]
        public string MerchantName { get; set; }

        [Name("Supplier - Address")]
        public string MerchantAddress { get; set; }

        [Name("Supplier - City")]
        public string MerchantCity { get; set; }

        [Name("Supplier - State")]
        public string MerchantState { get; set; }

        [Name("Supplier - Zip Code")]
        public string MerchantZip { get; set; }

        [Name("Supplier - Country Code")]
        public string MerchantCountry { get; set; }

        [Name("Employee - Vendor ID")]
        public string EmployeeVendorId { get; set; }

        [Name("Transaction - Receipt Flag")]
        public string TransactionReceiptFlag { get; set; }

        [Name("Transaction - Receipt Image Name")]
        public string TransactionReceiptImageName { get; set; }

        [Name("Transaction - Receipt Image Reference ID")]
        public string TransactionReceiptImageReferenceId { get; set; }

        /**************************/

        [Name("Supplier - Merchant Category Code")]
        public int? MerchantCategoryCode { get; set; }

        [Name("Supplier - Merchant Category Code Description")]
        public string MerchantCategoryCodeDescription { get; set; }

        [Name("Supplier - Merchant Category Group")]
        public string MerchantCategoryGroup { get; set; }

        [Name("Supplier - Merchant Category Group Description")]
        public string MerchantCategoryGroupDescription { get; set; }

        [Name("Transaction Line Item - Bill Number")]
        public string TransactionLineItemBillNumber { get; set; }

        [Name("Transaction Line Item - Description")]
        public string TransactionLineItemDescription { get; set; }

        [Name("Transaction Line Item - Last Item")]
        public string TransactionLineItemLastItem { get; set; }

        [Name("Transaction Line Item - Line Item Total")]
        public decimal? TransactionLineItemTotal { get; set; }

        [Name("Transaction Line Item - Message ID (Detail)")]
        public string TransactionLineItemMessageIdDetail { get; set; }

        [Name("Transaction Line Item - Message ID (Summary)")]
        public string TransactionLineItemMessageIdSummary { get; set; }

        [Name("Transaction Line Item - Order Date")]
        public string TransactionLineItemOrderDate { get; set; }

        [Name("Transaction Line Item - Product Code")]
        public string TransactionLineItemProductCode { get; set; }

        [Name("Transaction Line Item - Purchase ID")]
        public string TransactionLineItemPurchaseId { get; set; }

        [Name("Transaction Line Item - Purchase Type")]
        public string TransactionLineItemPurchaseType { get; set; }

        [Name("Transaction Line Item - Quantity")]
        public decimal? TransactionLineItemQuantity { get; set; }

        [Name("Transaction Line Item - Service Identifier (Detail)")]
        public string TransactionLineItemServiceIdentifierDetail { get; set; }

        [Name("Transaction Line Item - Service Identifier (Summary)")]
        public string TransactionLineItemServiceIdentifierSummary { get; set; }

        [Name("Transaction Line Item - Source Country Code")]
        public string TransactionLineItemSourceCountryCode { get; set; }

        [Name("Transaction Line Item - Source Postal Code")]
        public string TransactionLineItemSourcePostalCode { get; set; }

        [Name("Transaction Line Item - Tax Amount")]
        public string TransactionLineItemTaxAmount { get; set; }

        [Name("Transaction Line Item - Tax Rate")]
        public string TransactionLineItemTaxRate { get; set; }

        [Name("Transaction Line Item - Unit of Measure")]
        public string TransactionLineItemUnitofMeasure { get; set; }

        [Name("Transaction Line Item - Unit Price")]
        public decimal? TransactionLineItemUnitPrice { get; set; }

        [Name("Transaction Line Coding - Job Number")]
        public string TransactionLineCodingJobNumber { get; set; }

        [Name("Transaction Line Coding - Task/Phase Code")]
        public string TransactionLineCodingTaskPhaseCode { get; set; }

        [Name("Transaction Line Coding - Job Cost Type")]
        public byte? TransactionLineCodingJobCostType { get; set; }

        [Name("Transaction Line Coding - Equipment Number")]
        public string TransactionLineCodingEquipmentNumber { get; set; }

        [Name("Transaction Line Coding - Equipment Cost Code")]
        public string TransactionLineCodingEquipmentCostCode { get; set; }

        [Name("Transaction Line Coding - Equipment Cost Type")]
        public byte? TransactionLineCodingEquipmentCostType { get; set; }

        [Name("Transaction Line Coding - GL Number")]
        public string TransactionLineCodingGLNumber { get; set; }

        //public portal.Code.Data.VP.Employee PREmployee { get; set; }


    }
    //public sealed class ZionCSVViewModelMap : ClassMap<ZionCSVViewModel>
    //{
    //    public ZionCSVViewModelMap()
    //    {
    //        AutoMap(CultureInfo.InvariantCulture);
    //        Map(m => m.PREmployee).Ignore();
    //    }
    //}
    //public class ZionCSVViewModelExt : ZionCSVViewModel
    //{
    //    public ZionCSVViewModelExt(): base()
    //    {

    //    }
    //    public ZionCSVViewModelExt(ZionCSVViewModel item)
    //    {
    //        CompanyName = item.CompanyName;
    //        StatementStartDate = item.StatementStartDate;
    //        StatementEndDate = item.StatementEndDate;
    //        CardEmployeeId = item.CardEmployeeId;
    //        EmployeeFirstName = item.EmployeeFirstName;
    //        EmployeeLastName = item.EmployeeLastName;
    //        EmployeeEmail = item.EmployeeEmail;
    //        CardHolderFirstName = item.CardHolderFirstName;
    //        CardHolderLastName = item.CardHolderLastName;
    //        TransactionReference = item.TransactionReference;
    //        TransLineNumber = item.TransLineNumber;
    //        TransSequenceReference = item.TransSequenceReference;
    //        TransSequenceNumber = item.TransSequenceNumber;
    //        TransactionSecondDescription = item.TransactionSecondDescription;
    //        TransPostDate = item.TransPostDate;
    //        TransDate = item.TransDate;
    //        TransactionType = item.TransactionType;
    //        TransactionBillingCurrencyCode = item.TransactionBillingCurrencyCode;
    //        TransBillingAmount = item.TransBillingAmount;
    //        TransLineAmount = item.TransLineAmount;
    //        TransLineTaxAmount = item.TransLineTaxAmount;
    //        TransStatusId = item.TransStatusId;
    //        TransactionStatusDescription = item.TransactionStatusDescription;
    //        MerchantId = item.MerchantId;
    //        MerchantName = item.MerchantName;
    //        MerchantAddress = item.MerchantAddress;
    //        MerchantCity = item.MerchantCity;
    //        MerchantState = item.MerchantState;
    //        MerchantZip = item.MerchantZip;
    //        MerchantCountry = item.MerchantCountry;
    //        EmployeeVendorId = item.EmployeeVendorId;
    //        TransactionReceiptFlag = item.TransactionReceiptFlag;
    //        TransactionReceiptImageName = item.TransactionReceiptImageName;
    //        TransactionReceiptImageReferenceId = item.TransactionReceiptImageReferenceId;
    //        MerchantCategoryCode = item.MerchantCategoryCode;
    //        MerchantCategoryCodeDescription = item.MerchantCategoryCodeDescription;
    //        MerchantCategoryGroup = item.MerchantCategoryGroup;
    //        MerchantCategoryGroupDescription = item.MerchantCategoryGroupDescription;
    //        TransactionLineItemBillNumber = item.TransactionLineItemBillNumber;
    //        TransactionLineItemDescription = item.TransactionLineItemDescription;
    //        TransactionLineItemLastItem = item.TransactionLineItemLastItem;
    //        TransactionLineItemTotal = item.TransactionLineItemTotal;
    //        TransactionLineItemMessageIdDetail = item.TransactionLineItemMessageIdDetail;
    //        TransactionLineItemMessageIdSummary = item.TransactionLineItemMessageIdSummary;
    //        TransactionLineItemOrderDate = item.TransactionLineItemOrderDate;
    //        TransactionLineItemProductCode = item.TransactionLineItemProductCode;
    //        TransactionLineItemPurchaseId = item.TransactionLineItemPurchaseId;
    //        TransactionLineItemPurchaseType = item.TransactionLineItemPurchaseType;
    //        TransactionLineItemQuantity = item.TransactionLineItemQuantity;
    //        TransactionLineItemServiceIdentifierDetail = item.TransactionLineItemServiceIdentifierDetail;
    //        TransactionLineItemServiceIdentifierSummary = item.TransactionLineItemServiceIdentifierSummary;
    //        TransactionLineItemSourceCountryCode = item.TransactionLineItemSourceCountryCode;
    //        TransactionLineItemSourcePostalCode = item.TransactionLineItemSourcePostalCode;
    //        TransactionLineItemTaxAmount = item.TransactionLineItemTaxAmount;
    //        TransactionLineItemTaxRate = item.TransactionLineItemTaxRate;
    //        TransactionLineItemUnitofMeasure = item.TransactionLineItemUnitofMeasure;
    //        TransactionLineItemUnitPrice = item.TransactionLineItemUnitPrice;
    //        TransactionLineCodingJobNumber = item.TransactionLineCodingJobNumber;
    //        TransactionLineCodingTaskPhaseCode = item.TransactionLineCodingTaskPhaseCode;
    //        TransactionLineCodingJobCostType = item.TransactionLineCodingJobCostType;
    //        TransactionLineCodingEquipmentNumber = item.TransactionLineCodingEquipmentNumber;
    //        TransactionLineCodingEquipmentCostCode = item.TransactionLineCodingEquipmentCostCode;
    //        TransactionLineCodingEquipmentCostType = item.TransactionLineCodingEquipmentCostType;
    //        TransactionLineCodingGLNumber = item.TransactionLineCodingGLNumber;
    //    }
    //    public portal.Code.Data.VP.Employee PREmployee { get; set; }
    //}
}
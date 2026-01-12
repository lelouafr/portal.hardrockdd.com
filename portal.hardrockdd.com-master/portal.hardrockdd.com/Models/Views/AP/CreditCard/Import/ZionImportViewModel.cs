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
    public class ZionImportLinesViewModel
    {
        public ZionImportLinesViewModel()
        {

        }

        public ZionImportLinesViewModel(DB.Infrastructure.ViewPointDB.Data.CreditImport import)
        {

            Co = import.Co;
            ImportId = import.ImportId;
            List = import.ZionLines.Select(s => new ZionImportViewModel(s)).ToList();
        }

        [Key]
        public byte Co { get; set; }
        [Key]
        public int ImportId { get; set; }

        public List<ZionImportViewModel> List { get; set; }
    }
        

    public class ZionImportViewModel
    {

        public ZionImportViewModel()
        {

        }

        public ZionImportViewModel(DB.Infrastructure.ViewPointDB.Data.ZionImport importLine)
        {
            CCCo = importLine.CCCo;
            ImportId = importLine.ImportId;
            ImportLineId = importLine.ImportLineId;
            CompanyName = importLine.CompanyName;
            StatementStartDate = importLine.StatementStartDate;
            StatementEndDate = importLine.StatementEndDate;
            PRCo = importLine.PRCo;
            PREmployeeId = importLine.PREmployeeId;
            CardEmployeeId = importLine.CardEmployeeId;
            EmployeeFirstName = importLine.EmployeeFirstName;
            EmployeeLastName = importLine.EmployeeLastName;
            EmployeeEmail = importLine.EmployeeEmail;
            CardHolderFirstName = importLine.CardHolderFirstName;
            CardHolderLastName = importLine.CardHolderLastName;
            TransactionReference = importLine.TransactionReference;
            TransLineNumber = importLine.TransLineNumber;
            TransSequenceReference = importLine.TransSequenceReference;
            TransSequenceNumber = importLine.TransSequenceNumber;
            TransactionSecondDescription = importLine.TransactionSecondDescription;
            TransPostDate = importLine.TransPostDate;
            TransDate = importLine.TransDate;
            TransactionType = importLine.TransactionType;
            TransactionBillingCurrencyCode = importLine.TransactionBillingCurrencyCode;
            TransBillingAmount = importLine.TransBillingAmount;
            TransLineAmount = importLine.TransLineAmount;
            TransLineTaxAmount = importLine.TransLineTaxAmount;
            TransStatusId = importLine.TransStatusId;
            TransactionStatusDescription = importLine.TransactionStatusDescription;
            MerchantId = importLine.MerchantId;
            MerchantName = importLine.MerchantName;
            MerchantAddress = importLine.MerchantAddress;
            MerchantCity = importLine.MerchantCity;
            MerchantState = importLine.MerchantState;
            MerchantZip = importLine.MerchantZip;
            MerchantCountry = importLine.MerchantCountry;
            EmployeeVendorId = importLine.EmployeeVendorId;
            //TransactionReceiptFlag = importLine.TransactionReceiptFlag;
            //TransactionReceiptImageName = importLine.TransactionReceiptImageName;
            //TransactionReceiptImageReferenceId = importLine.TransactionReceiptImageReferenceId;
            MerchantCategoryCode = importLine.MerchantCategoryCode;
            MerchantCategoryCodeDescription = importLine.MerchantCategoryCodeDescription;
            MerchantCategoryGroup = importLine.MerchantCategoryGroup;
            MerchantCategoryGroupDescription = importLine.MerchantCategoryGroupDescription;
            TransactionLineItemBillNumber = importLine.TransactionLineItemBillNumber;
            TransactionLineItemDescription = importLine.TransactionLineItemDescription;
            //TransactionLineItemLastItem = importLine.TransactionLineItemLastItem;
            TransactionLineItemTotal = importLine.TransactionLineItemTotal;
            //TransactionLineItemMessageIdDetail = importLine.TransactionLineItemMessageIdDetail;
            //TransactionLineItemMessageIdSummary = importLine.TransactionLineItemMessageIdSummary;
            //TransactionLineItemOrderDate = importLine.TransactionLineItemOrderDate;
            //TransactionLineItemProductCode = importLine.TransactionLineItemProductCode;
            //TransactionLineItemPurchaseId = importLine.TransactionLineItemPurchaseId;
            TransactionLineItemPurchaseType = importLine.TransactionLineItemPurchaseType;
            TransactionLineItemQuantity = importLine.TransactionLineItemQuantity;
            //TransactionLineItemServiceIdentifierDetail = importLine.TransactionLineItemServiceIdentifierDetail;
            //TransactionLineItemServiceIdentifierSummary = importLine.TransactionLineItemServiceIdentifierSummary;
            //TransactionLineItemSourceCountryCode = importLine.TransactionLineItemSourceCountryCode;
            //TransactionLineItemSourcePostalCode = importLine.TransactionLineItemSourcePostalCode;
            TransactionLineItemTaxAmount = importLine.TransactionLineItemTaxAmount;
            TransactionLineItemTaxRate = importLine.TransactionLineItemTaxRate;
            TransactionLineItemUnitofMeasure = importLine.TransactionLineItemUnitofMeasure;
            TransactionLineItemUnitPrice = importLine.TransactionLineItemUnitPrice;
            //TransactionLineCodingJobNumber = importLine.TransactionLineCodingJobNumber;
            //TransactionLineCodingTaskPhaseCode = importLine.TransactionLineCodingTaskPhaseCode;
            //TransactionLineCodingJobCostType = importLine.TransactionLineCodingJobCostType;
            //TransactionLineCodingEquipmentNumber = importLine.TransactionLineCodingEquipmentNumber;
            //TransactionLineCodingEquipmentCostCode = importLine.TransactionLineCodingEquipmentCostCode;
            //TransactionLineCodingEquipmentCostType = importLine.TransactionLineCodingEquipmentCostType;
            //TransactionLineCodingGLNumber = importLine.TransactionLineCodingGLNumber;
            IsError = importLine.IsError;
            TransactionDescription = importLine.TransactionDescription;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Co")]
        public byte CCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "ImportId")]
        public int ImportId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int ImportLineId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Start Date")]
        public Nullable<System.DateTime> StatementStartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "End Date")]
        public Nullable<System.DateTime> StatementEndDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Card Id")]
        public Nullable<int> CardEmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "First Name")]
        public string EmployeeFirstName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Last Name")]
        public string EmployeeLastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Email")]
        public string EmployeeEmail { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "First Name")]
        public string CardHolderFirstName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Last Name")]
        public string CardHolderLastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Reference")]
        public string TransactionReference { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Line #")]
        public Nullable<int> TransLineNumber { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Seq Ref")]
        public Nullable<int> TransSequenceReference { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Sequence #")]
        public string TransSequenceNumber { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "2nd Description")]
        public string TransactionSecondDescription { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Post Date")]
        public Nullable<System.DateTime> TransPostDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Trans Date")]
        public Nullable<System.DateTime> TransDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Type")]
        public string TransactionType { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Currency Code")]
        public string TransactionBillingCurrencyCode { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public Nullable<decimal> TransBillingAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Line $")]
        public Nullable<decimal> TransLineAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Line Tax")]
        public Nullable<decimal> TransLineTaxAmount { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Status Id")]
        public Nullable<int> TransStatusId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string TransactionStatusDescription { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant #")]
        public string MerchantId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Name")]
        public string MerchantName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Address")]
        public string MerchantAddress { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant City")]
        public string MerchantCity { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant State")]
        public string MerchantState { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Zip")]
        public string MerchantZip { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Country")]
        public string MerchantCountry { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Emp Vend Id")]
        public string EmployeeVendorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Cat #")]
        public Nullable<int> MerchantCategoryCode { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Cat")]
        public string MerchantCategoryCodeDescription { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Group #")]
        public string MerchantCategoryGroup { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant Group")]
        public string MerchantCategoryGroupDescription { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Line Bill #")]
        public string TransactionLineItemBillNumber { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Line Desc")]
        public string TransactionLineItemDescription { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Line Item $")]
        public Nullable<decimal> TransactionLineItemTotal { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Line Product")]
        public string TransactionLineItemProductCode { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Line Pruchase Type")]
        public string TransactionLineItemPurchaseType { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Line Qty")]
        public Nullable<decimal> TransactionLineItemQuantity { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Line Tax $")]
        public string TransactionLineItemTaxAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Line Tax Rate")]
        public string TransactionLineItemTaxRate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "LINE UM")]
        public string TransactionLineItemUnitofMeasure { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Line Unit $")]
        public Nullable<decimal> TransactionLineItemUnitPrice { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "PRCo")]
        public Nullable<byte> PRCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Employee Id")]
        public Nullable<int> PREmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Error")]
        public Nullable<bool> IsError { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string TransactionDescription { get; set; }

    }
}
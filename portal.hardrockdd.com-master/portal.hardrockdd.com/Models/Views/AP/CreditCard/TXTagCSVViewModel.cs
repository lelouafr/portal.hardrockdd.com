using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard
{
    public class TXTagCSVViewModel
    {
        [Name("TRANSACTION_NUMBER")]
        public long TransactionNumber { get; set; }
        [Name("LOCATION")]
        public string Location { get; set; }
        [Name("TAG_NUMBER/PLATE_NUMBER")]
        public string TagNumber { get; set; }
        [Name("TRANSACTION_TYPE")]
        public string TransType { get; set; }
        [Name("AMOUNT")]
        public string AmountStr { get; set; }
        [Name("DATE/TIME")]
        public string TransDateStr { get; set; }

    }
}
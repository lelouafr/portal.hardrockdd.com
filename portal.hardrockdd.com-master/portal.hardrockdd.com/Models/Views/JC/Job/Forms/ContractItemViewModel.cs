using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job.Forms
{
    public class ContractItemListViewModel
    {
        public ContractItemListViewModel()
        {
            List = new List<ContractItemViewModel>();
        }

        public ContractItemListViewModel(DB.Infrastructure.ViewPointDB.Data.JCContract contract)
        {
            if (contract == null) return;

            Co = contract.JCCo;
            ContractId = contract.ContractId;
            List = contract.ContractItems.Select(s => new ContractItemViewModel(s)).ToList();
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        public string ContractId { get; set; }

        public List<ContractItemViewModel> List { get; }
    }

    public class ContractItemViewModel
    {
        public ContractItemViewModel()
        {

        }

        public ContractItemViewModel(DB.Infrastructure.ViewPointDB.Data.ContractItem contractItem)
        {
            if (contractItem == null) 
                return;

            Co = contractItem.JCCo;
            ContractId = contractItem.ContractId;
            ItemId = contractItem.Item;
            //ContractItemId = contractItem.ContractItemId;
            //StartDate = contractItem.StartDate;

            
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }

        [Key]
        public string ItemId { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/JobTypeContractItemCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "ContractItem")]
        public string ItemCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/SIRegionCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Item Code")]
        public string SIRegion { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/SICodeCombo", ComboForeignKeys = "SIRegion")]
        [Display(Name = "Item Code")]
        public string SICode { get; set; }

        [UIHint("PercentageBox")]
        [Display(Name = "Retainage %")]
        public decimal RetainPCT { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/JobTypeContractItemCombo", ComboForeignKeys = "Co")]
        [Display(Name = "UM")]
        public string UM { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal OrigContractAmt { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Units")]
        public decimal OrigContractUnits { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Price")]
        public decimal OrigUnitPrice { get; set; }
    }
}
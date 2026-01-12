using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Project.Models.Contract
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

            JCCo = contractItem.JCCo;
            ContractId = contractItem.ContractId;
            ItemId = contractItem.Item;

            Description = contractItem.Description;
            ItemCodeId = contractItem.ItemCodeId;
            SIRegion = contractItem.SIRegion;

            RetainPCT = contractItem.RetainPCT;
            UM = contractItem.UM;
            OrigContractAmt = contractItem.OrigContractAmt;
            OrigContractUnits = contractItem.OrigContractUnits;
            OrigUnitPrice = contractItem.OrigUnitPrice;
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }

        [Key]
        public string ItemId { get; set; }


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
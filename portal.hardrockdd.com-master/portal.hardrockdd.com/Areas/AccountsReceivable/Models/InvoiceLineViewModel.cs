using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.AccountsReceivable.Models
{

    public class InvoiceLineListViewModel
    {
        public InvoiceLineListViewModel()
        {

        }
        public InvoiceLineListViewModel(DB.Infrastructure.ViewPointDB.Data.ARTran invoice)
        {
            if (invoice == null)
            {
                throw new System.ArgumentNullException(nameof(invoice));
            }
            Co = invoice.ARCo;
            Mth = invoice.Mth.ToShortDateString();
            ARTransId = invoice.ARTransId;

            List = invoice.Lines.Select(s => new InvoiceLineViewModel(s)).ToList();
        }


        public InvoiceLineListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            Co = job.JCCo;
            //ARTransId = invoice.ARTransId;

            List = job.ARInvoiceLines.Select(s => new InvoiceLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [Display(Name = "Request Id")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [Display(Name = "Trans Id")]
        public int ARTransId { get; set; }

        public List<InvoiceLineViewModel> List { get; }
    }

    public class InvoiceLineViewModel
    {
        public InvoiceLineViewModel()
        {

        }

        public InvoiceLineViewModel(ARTransLine entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            ARCo = entry.ARCo;
            Mth = entry.Mth.ToShortDateString();
            MthDate = entry.Mth;
            ARTransId = entry.ARTransId;
            LineId = entry.ARLineId;

            CustGroupId = entry.ARTran.CustGroup;
            CustomerName = entry.ARTran.Customer.Name;
            CustomerId = entry.ARTran.CustomerId;
            ARRef = entry.ARTran.CustRef;

            LineTypeId = entry.LineType;
            Description = entry.Description;

            ContractId = entry.ContractId;
            ItemId = entry.ItemId;
            ItemDescription = entry.ContractItem?.Description;

            JCCo = entry.JCCo;
            JobId = entry.JobId;
            PhaseGroupId = entry.PhaseGroup;
            PhaseId = entry.PhaseId;
            JobCostTypeId = entry.JCCType;

            EMCo = entry.EMCo;
            EquipmentId = entry.EquipmentId;
            EMGroupId = entry.EMGroupId;
            CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;

            GLCo = entry.GLCo;
            GLAcct = entry.GLAcct;

            Units = entry.ContractUnits;
            UM = entry.UM;
            UnitPrice = entry.UnitPrice ?? (entry.ContractUnits != 0 ? (Amount = entry.Amount / entry.ContractUnits) : 0);
            Amount = entry.Amount;

            Comments = entry.Notes;
        }

        public byte? PhaseGroupId { get; set; }
        public byte? EMGroupId { get; set; }
        public byte? CustGroupId { get; set; }
        public byte? JCCo { get; set; }
        public byte? GLCo { get; set; }
        public byte? EMCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte ARCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Mth")]
        public string Mth { get; set; }

        [Required]
        [Display(Name = "Mth")]
        public DateTime MthDate { get; set; }

        [Key]
        [Required]
        [Display(Name = "Trans Id")]
        public int ARTransId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Line")]
        public int LineId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Reference")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ARRef { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Line Type")]
        [Field(Placeholder = "Line Type")]
        public string LineTypeId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JobCostTypeId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/CustomerCombo/POJobCombo", ComboForeignKeys = "CustGroupId")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Customer Name")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string CustomerName { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostTypeCombo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/ContractCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/ContractItemsCombo", ComboForeignKeys = "JCCo, ContractId")]
        [Display(Name = "Item")]
        public string ItemId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Item Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ItemDescription { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 4, TextSize = 4)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 4, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=ARCo")]
        [Display(Name = "UM")]
        public string UM { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Price")]
        [Field(LabelSize = 4, TextSize = 8)]
        public decimal? UnitPrice { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 4, TextSize = 8)]
        public decimal? Amount { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }
}
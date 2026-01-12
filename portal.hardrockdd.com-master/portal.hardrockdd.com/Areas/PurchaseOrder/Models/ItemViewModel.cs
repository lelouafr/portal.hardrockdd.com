using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Areas.PurchaseOrder.Models
{

    public class ItemListViewModel
    {
        public ItemListViewModel()
        {

        }
        public ItemListViewModel(DB.Infrastructure.ViewPointDB.Data.PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
                return;

            POCo = purchaseOrder.POCo;
            PO = purchaseOrder.PO;

            List = purchaseOrder.Items.Select(s => new ItemViewModel(s)).ToList();
        }

        public ItemListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            POCo = job.JCCo;
            JobId = job.JobId;

            List = job.POItems.Select(s => new ItemViewModel(s)).ToList();
        }

        [Key]
        [Display(Name = "Co")]
        public byte POCo { get; set; }

        [Key]
        [Display(Name = "Request Id")]
        public string PO { get; set; }

        public string JobId { get; set; }

        public List<ItemViewModel> List { get; }
    }

    public class ItemViewModel
    {
        public ItemViewModel()
        {

        }

        public ItemViewModel(DB.Infrastructure.ViewPointDB.Data.PurchaseOrderItem entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            POCo = entry.POCo;
            PO = entry.PO;
            POItem = entry.POItemId;
            Status = (DB.POStatusEnum)entry.PurchaseOrder.Status;
            ItemTypeId = (DB.POItemTypeEnum)entry.ItemTypeId;
            CalcType = entry.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = entry.Description;

            JCCo = entry.JCCo ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.JCCo;
            JobId = entry.JobId;
            PhaseGroupId = entry.PhaseGroupId ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.PhaseGroupId;
            PhaseId = entry.PhaseId;
            JobCostTypeId = entry.JCCType;

            PRCo = entry.PurchaseOrder.POCompanyParm.HQCompanyParm.PRCo;
            CrewId = entry.CrewId;

            VendorGroupId = entry.PurchaseOrder.VendorGroupId;
            VendorName = entry.PurchaseOrder.Vendor?.Name;
            VendorId = entry.PurchaseOrder.VendorId;

            EMCo = entry.EMCo ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.EMCo;
            EquipmentId = entry.EquipmentId;
            EMGroupId = entry.EMGroupId ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.EMGroupId;
            CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;

            GLCo = entry.GLCo;// ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.GLCo;
            GLAcct = entry.GLAcct;

            Units = entry.OrigUnits;
            UM = entry.UM;
            UnitCost = entry.OrigUnitCost;
            Cost = entry.OrigCost;

            TaxGroupId = entry.TaxGroup ?? entry.PurchaseOrder.POCompanyParm.HQCompanyParm.TaxGroupId;
            TaxTypeId = (DB.TaxTypeEnum)(entry.TaxTypeId ?? 0);
            TaxCodeId = entry.TaxCodeId;
            TaxRate = entry.TaxRate;
            TaxAmount = entry.OrigTax;
            if (TaxTypeId != DB.TaxTypeEnum.None && TaxAmount == 0)
            {
                TaxAmount = Cost * TaxRate;
            }


            Comments = entry.Notes;
        }

        public byte? GLCo { get; set; }
        public byte? JCCo { get; set; }
        public byte? PRCo { get; set; }
        public byte? EMCo { get; set; }
        public byte? PhaseGroupId { get; set; }
        public byte? VendorGroupId { get; set; }
        public byte? EMGroupId { get; set; }



        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Key]
        [Required]
        [Display(Name = "Item")]
        public int POItem { get; set; }

        [Required]
        [Display(Name = "Status")]
        public DB.POStatusEnum Status { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Type")]
        public DB.POItemTypeEnum ItemTypeId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

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
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

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
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 2, TextSize = 2)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 2, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=POCo")]
        [Display(Name = "UM")]
        public string UM { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        public decimal? UnitCost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Cost { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Tax Type")]
        public DB.TaxTypeEnum TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Tax Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? TaxAmount { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }
}
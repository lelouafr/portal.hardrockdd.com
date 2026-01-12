using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{
    public class PORequestLineListViewModel
    {
        public PORequestLineListViewModel()
        {

        }
        public PORequestLineListViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            POCo = request.POCo;
            RequestId = request.RequestId;

            List = request.Lines.Select(s => new PORequestLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        public List<PORequestLineViewModel> List { get; }
    }

    public class PORequestLineViewModel
    {
        public PORequestLineViewModel()
        {

        }

        public PORequestLineViewModel(DB.Infrastructure.ViewPointDB.Data.PORequestLine entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            POCo = entry.POCo;
            RequestId = entry.RequestId;
            LineId = entry.LineId;
            Status = (DB.PORequestStatusEnum)entry.Request.Status;
            ItemTypeId = (DB.POItemTypeEnum)entry.ItemTypeId;
            CalcType = entry.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = entry.Description;

            JCCo = entry.JCCo ?? 0;
            JobId = entry.JobId;
            CrewId = entry.CrewId;
            PhaseGroupId = entry.PhaseGroupId ?? entry.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
            PhaseId = entry.PhaseId;
            JobCostTypeId = entry.JCCType;

            VendorGroupId = entry.Request.VendorGroup ?? entry.Request.Vendor?.VendorGroupId;
            VendorId = entry.Request.VendorId;

            EMGroupId = entry.EMGroupId ?? entry.Equipment?.EMCompanyParm?.HQCompanyParm?.EMGroupId;
            EMCo = entry.EMCo ?? 0;
            EquipmentId = entry.EquipmentId;
            CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;
            
            switch (ItemTypeId)
            {
                case DB.POItemTypeEnum.Job:
                    if (entry.Job != null)
                    {
                        ClassDescription = string.Format(AppCultureInfo.CInfo(), "Job:{0}", entry.Job.JobId);
                    }
                    break;
                case DB.POItemTypeEnum.Expense:
                    if (entry.GLAcct != null)
                    {
                        ClassDescription = string.Format(AppCultureInfo.CInfo(), "Exp:{0}", entry.GLAcct);
                    }
                    break;
                case DB.POItemTypeEnum.Equipment:
                    if (entry.Equipment != null)
                    {
                        ClassDescription = string.Format(AppCultureInfo.CInfo(), "Eqp:{0}", entry.Equipment.EquipmentId);
                    }
                    break;
                default:
                    ClassDescription = "";
                    break;
            }

            GLCo = entry.GLCo ?? entry.Request.Company.GLCo;
            GLAcct = entry.GLAcct;

            Units = entry.Units;
            UM = entry.UM;
            UnitCost = entry.UnitCost;
            Cost = entry.Cost;

            TaxGroupId = entry.Request.POCompanyParm.HQCompanyParm.TaxGroupId;
            TaxTypeId = (DB.TaxTypeEnum)(entry.TaxTypeId ?? 0);
            TaxCodeId = entry.TaxCodeId;
            TaxRate = entry.TaxRate;
            TaxAmount = entry.TaxAmount;
            if (TaxTypeId != DB.TaxTypeEnum.None && TaxAmount == 0)
            {
                TaxAmount = Cost * TaxRate;
            }
            

            Comments = entry.Comments;
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            if (ItemTypeId == DB.POItemTypeEnum.Job)
            {
                if (JobId == null)
                {
                    modelState.AddModelError("JobId", "Job Is required");
                    ok &= false;
                }
                if (PhaseId == null)
                {
                    modelState.AddModelError("PhaseId", "Phase Is required");
                    ok &= false;
                }
                if (JobCostTypeId == null)
                {
                    modelState.AddModelError("JobCostTypeId", "Cost Type Is required");
                    ok &= false;
                }
            }
            if (ItemTypeId == DB.POItemTypeEnum.Equipment)
            {
                if (EquipmentId == null)
                {
                    modelState.AddModelError("EquipmentId", "Equipment Is required");
                    ok &= false;
                }
                if (CostCodeId == null)
                {
                    modelState.AddModelError("CostCodeId", "Cost Code Is required");
                    ok &= false;
                }
                if (CostTypeId == null)
                {
                    modelState.AddModelError("CostTypeId", "Cost Type Is required");
                    ok &= false;
                }
            }
            if (CalcType == DB.POCalcTypeEnum.Units)
            {
                if (Units == null)
                {
                    modelState.AddModelError("Units", "Units Is required");
                    ok &= false;
                }
                if (UnitCost == null)
                {
                    modelState.AddModelError("UnitCost", "Unit Cost Is required");
                    ok &= false;
                }
            }
            if (CalcType == DB.POCalcTypeEnum.LumpSum)
            {
                if (Cost == null)
                {
                    modelState.AddModelError("Cost", "Cost Is required");
                    ok &= false;
                }
            }
            if (TaxTypeId != DB.TaxTypeEnum.None)
            {
                if (TaxCodeId == null)
                {
                    modelState.AddModelError("TaxCodeId", "Tax Code Is required");
                    ok &= false;
                }
            }
            if (ok == false)
            {
                modelState.AddModelError("", string.Format("Line # {0} has errors", LineId));

            }
            return ok;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        public int RequestId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        public int LineId { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.PORequestStatusEnum Status { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Type")]
        public DB.POItemTypeEnum ItemTypeId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ClassDescription { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/POCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/ARCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }


        public byte? PhaseGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/VendorCombo", ComboForeignKeys = "PhaseGroupId, VendorId", SearchUrl = "/PhaseMaster/Search", SearchForeignKeys = "PhaseGroupId, VendorId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/POCombo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JobCostTypeId { get; set; }

        public byte? VendorGroupId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        public byte? EMGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EquipmentCostType/POCombo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        public byte? GLCo { get; set; }

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
        [Field(LabelSize = 2, TextSize = 4, Placeholder = "Tax Type")]
        public DB.TaxTypeEnum? TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 0, TextSize = 2, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 2, TextSize = 4)]
        public decimal? TaxAmount { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }

    
}
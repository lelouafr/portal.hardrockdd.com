using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{
    public class DocumentLineListViewModel
    {
        public DocumentLineListViewModel()
        {

        }

        public DocumentLineListViewModel(APDocument document)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            using var db = new VPContext();
            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            PO = document.PO;
            #endregion

            List = document.Lines.Select(s => new DocumentLineViewModel(s)).ToList();            
        }



        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "DocId")]
        public int DocId { get; set; }

        public string PO { get; set; }

        public List<DocumentLineViewModel> List { get; }
    }

    public class DocumentLineViewModel
    {
        public DocumentLineViewModel()
        {

        }

        public DocumentLineViewModel(DB.Infrastructure.ViewPointDB.Data.APDocumentLine documentLine)
        {
            if (documentLine == null)
            {
                throw new ArgumentNullException(nameof(documentLine));
            }

            #region mapping
            APCo = documentLine.APCo;
            DocId = documentLine.DocId;
            SeqId = documentLine.SeqId;
            LineId = documentLine.LineId;

            DivisionId = documentLine.DivisionId;
            LineTypeId = (DB.APLineTypeEnum)documentLine.LineTypeId;

            VendorGroupId = documentLine.VendorGroupId ?? documentLine.Document.APCompanyParm.HQCompanyParm.VendorGroupId;
            VendorId = documentLine.Document.VendorId;

            POCo = documentLine.POCo ?? documentLine.APCo;
            PO = documentLine.PO;
            POItemId = documentLine.POItemId;
            POItemTypeId = documentLine.POItemType;
            CalcType = documentLine.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = documentLine.Description;

            JCCo = documentLine.JCCo ?? documentLine.Document.APCompanyParm.HQCompanyParm.JCCo;
            JobId = documentLine.JobId;
            PhaseGroupId = documentLine.PhaseGroupId ?? documentLine.Document.APCompanyParm.HQCompanyParm.PhaseGroupId;
            PhaseId = documentLine.PhaseId;
            JobCostTypeId = documentLine.JCCType;

            //VendorId = entry.PurchaseOrder.VendorId;

            EMGroupId = documentLine.EMGroupId ?? documentLine.Document.APCompanyParm.HQCompanyParm.EMGroupId;
            EMCo = documentLine.EMCo ?? documentLine.Document.APCompanyParm.HQCompanyParm.EMCo;
            EquipmentId = documentLine.EquipmentId;
            CostCodeId = documentLine.CostCodeId;
            CostTypeId = documentLine.EMCType;

            GLCo = documentLine.GLCo ?? documentLine.Document.APCompanyParm.HQCompanyParm.GLCo;
            GLAcct = documentLine.GLAcct;

            Units = documentLine.Units;
            UM = documentLine.UM;
            UnitCost = documentLine.UnitCost;
            GrossAmt = documentLine.GrossAmt;

            TaxGroupId = documentLine.Document.APCompanyParm.HQCompanyParm.TaxGroupId;
            TaxTypeId = (DB.TaxTypeEnum)(documentLine.TaxTypeId ?? 0);
            TaxCodeId = documentLine.TaxCodeId;
            if (documentLine.GrossAmt != 0 )
            {
                TaxRate = documentLine.TaxAmt / documentLine.GrossAmt;
            }
            TaxAmount = documentLine.TaxAmt;

            #endregion
        }

        [HiddenInput]
        public byte? POCo { get; set; }
        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? JCCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }
        [HiddenInput]
        public byte? VendorGroupId { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Doc Id")]
        public int DocId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int SeqId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "#")]
        public int LineId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/WPCombo/WPDivisionCombo", ComboForeignKeys = "")]
        [Display(Name = "Division")]
        public int? DivisionId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(LabelSize = 3, TextSize = 9, Placeholder = "Type")]
        public DB.APLineTypeEnum LineTypeId { get; set; }


        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 3, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo, VendorId")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 1, TextSize = 5, ComboUrl = "/POCombo/POItemCombo", ComboForeignKeys = "POCo, PO")]
        [Display(Name = "Item")]
        public short? POItemId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(Placeholder = "Type")]
        public DB.POItemTypeEnum? POItemTypeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 3, TextSize = 9)]
        public string Description { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        [Field(LabelSize = 3, TextSize = 9, Placeholder = "Amount Type")]
        public DB.POCalcTypeEnum CalcType { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/JCCombo/APCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JobCostTypeId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo", SearchUrl = "/EMCombo/Search", SearchForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/EMCostCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Cost Code")]
        public string CostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EquipmentCostType/Combo", ComboForeignKeys = "EMGroupId, CostCodeId")]
        [Display(Name = "Cost Type")]
        public byte? CostTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "GL Acct")]
        public string GLAcct { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 3, TextSize = 6)]
        [Display(Name = "Units")]
        public decimal? Units { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 6, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo='1'")]
        [Display(Name = "UM")]
        public string UM { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Unit Cost")]
        [Field(LabelSize = 3, TextSize = 6)]
        public decimal? UnitCost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? GrossAmt { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Tax Type")]
        [Field(LabelSize = 3, TextSize = 6, Placeholder = "Tax Type")]
        public DB.TaxTypeEnum? TaxTypeId { get; set; }

        public byte? TaxGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 6, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Code")]
        public string TaxCodeId { get; set; }

        [UIHint("IntegerBox")]
        [Field(LabelSize = 0, TextSize = 3, ComboUrl = "/HQCombo/TaxCombo", ComboForeignKeys = "HQCo=TaxGroupId")]
        [Display(Name = "Tax Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F4}")]
        public decimal? TaxRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 3, TextSize = 6)]
        public decimal? TaxAmount { get; set; }


        internal DocumentLineViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            //Model update
            if (CalcType == DB.POCalcTypeEnum.LumpSum)
            {
                UM = "LS";
                Units = 0;
                UnitCost = 0;
            }
            else
            {
                if (UM == "LS")
                    UM = null;
                //Convert the current Amount to unit and unit cost
                Units ??= 1;
                UnitCost ??= GrossAmt ?? 0;
                GrossAmt = Units * UnitCost;
            }

            var updObj = db.APDocumentLines.FirstOrDefault(f => f.APCo == this.APCo && f.DocId == this.DocId && f.LineId == this.LineId);

            if (updObj != null)
            {
                if (updObj.LineType != LineTypeId)
                    updObj.LineType = LineTypeId;
                else
                {
                    switch (updObj.LineType)
                    {
                        case DB.APLineTypeEnum.Job:
                            updObj.JobId = JobId;
                            updObj.PhaseId = PhaseId;
                            updObj.JCCType = JobCostTypeId;
                            break;
                        case DB.APLineTypeEnum.Expense:
                            updObj.GLAcct = GLAcct;
                            break;
                        case DB.APLineTypeEnum.Equipment:
                            updObj.EquipmentId = EquipmentId;
                            updObj.CostCodeId = CostCodeId;
                            updObj.EMCType = CostTypeId;
                            break;
                        case DB.APLineTypeEnum.PO:
                            updObj.PO = PO;
                            updObj.POItemId = POItemId;
                            break;
                        default:
                            break;
                    }
                    updObj.DivisionId = DivisionId;
                    updObj.UM = UM;
                    updObj.Units = Units;
                    updObj.UnitCost = UnitCost;
                    updObj.GrossAmt = GrossAmt;
                    updObj.TaxAmt = TaxAmount;
                    updObj.TaxType = TaxTypeId;
                    updObj.TaxCodeId = TaxCodeId;
                    updObj.PayType = 1;
                    updObj.MiscAmt = 0;
                    updObj.MatlGroup = APCo;
                    updObj.VendorGroupId = updObj.VendorGroupId;
                    updObj.Description = Description;
                }

                try
                {
                    db.BulkSaveChanges();
                    return new DocumentLineViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Vendor.Forms
{
    public class VendorCodingViewModel
    {
        public VendorCodingViewModel()
        {

        }

        public VendorCodingViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null) throw new System.ArgumentNullException(nameof(vendor));
            VendGroupId = vendor.VendorGroupId;
            VendorId = vendor.VendorId;

            EMGroupId = 1;
            PhaseGroupId = 1;
            EMCo = 1;
            GLCo = 1;

            IsReoccurring = vendor.IsReoccurring ?? false;
            IsReceiptRequired = vendor.IsReceiptRequired ?? false;
            DefaultGLAcct = vendor.DefaultGLAcct;
            DefaultJCPhaseId = vendor.DefaultJCPhaseId;
            DefaultJCCType = vendor.DefaultJCCType;
            DefaultEMCostCodeId = vendor.DefaultEMCostCodeId;
            DefaultEMCType = vendor.DefaultEMCType;
        }

        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }



        [HiddenInput]
        [Key]
        public byte VendGroupId { get; set; }

        [HiddenInput]
        [Key]
        public int VendorId { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReoccurring { get; set; }

        [UIHint("SwitchBox")]
        public bool? IsReceiptRequired { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Default GL Acct")]
        public string DefaultGLAcct { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Phase")]
        public string DefaultJCPhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMasterCostType/APCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Job Cost Type")]
        public byte? DefaultJCCType { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/EMCostCodeAPCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Code")]
        public string DefaultEMCostCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EquipmentCostType/APCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "Equip Cost Type")]
        public byte? DefaultEMCType { get; set; }

    }
}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Forms
{
    public class EquipmentOwnershipViewModel : AuditBaseViewModel
    {
        public EquipmentOwnershipViewModel()
        {

        }
        
        public EquipmentOwnershipViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment) : base(equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            VendorGroupId = equipment.VendorGroup;
            OwnershipStatusId = equipment.OwnershipStatus;

            PurchasedFrom = equipment.PurchasedFrom;
            PurchasePrice = equipment.PurchasePrice;
            PurchDate = equipment.PurchDate;

            LeasedFrom = equipment.LeasedFrom;
            LeaseStartDate = equipment.LeaseStartDate;
            LeaseEndDate = equipment.LeaseEndDate;
            LeasePayment = equipment.LeasePayment;


        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [HiddenInput]
        public byte? VendorGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/EMCombo/OwnershipCombo", ComboForeignKeys = "")]
        [Display(Name = "Equipment")]
        public string OwnershipStatusId { get; set; }

        [UIHint("Textbox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Dealer")]
        public string PurchasedFrom { get; set; }

        [UIHint("CurrencyBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime? PurchDate { get; set; }


        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? LeasedFrom { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? LeaseStartDate { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? LeaseEndDate { get; set; }

        [UIHint("CurrencyBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Price")]
        public decimal LeasePayment { get; set; }

        //public decimal LeaseResidualValue { get; set; }


    }
}
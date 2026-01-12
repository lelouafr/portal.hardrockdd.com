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
    public class EquipmentLicInfoViewModel
    {
        public EquipmentLicInfoViewModel()
        {

        }

        public EquipmentLicInfoViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            LicensePlateNo = equipment.LicensePlateNo;
            LicensePlateState = equipment.LicensePlateState;
            LicensePlateExpDate = equipment.LicensePlateExpDate;
            InspectionExpDate = equipment.InspectionExpiration;
            RegGVWR = equipment.RegisteredGVWR;
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Plate #")]
        public string LicensePlateNo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "")]
        [Display(Name = "State")]
        public string LicensePlateState { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Field(LabelSize = 4, TextSize = 8)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Lic Exp Date")]
        public DateTime? LicensePlateExpDate { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Field(LabelSize = 4, TextSize = 8)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Insp Exp Date")]
        public DateTime? InspectionExpDate { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Reg. GVWR")]
        public decimal? RegGVWR { get; set; }


    }
}
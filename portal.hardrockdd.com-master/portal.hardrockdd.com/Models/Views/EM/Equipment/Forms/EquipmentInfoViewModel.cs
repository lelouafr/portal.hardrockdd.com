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
    public class EquipmentInfoViewModel : AuditBaseViewModel
    {
        public EquipmentInfoViewModel()
        {

        }

        public EquipmentInfoViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment) : base(equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            Description = equipment.Description;
            StatusId = equipment.Status;
            CategoryId = equipment.CategoryId;
            Manufacturer = equipment.Manufacturer;
            EquipModel = equipment.Model;
            Type = equipment.Type;
            ModelYr = equipment.ModelYr;
            VINNumber = equipment.VINNumber;
            Description = equipment.Description;
            TXTagId = equipment.TXTagId;

            HasAttachment = StaticFunctions.HasAttachments(equipment.UniqueAttchID);

            Category = equipment.Category?.Description;
            Status = StaticFunctions.GetComboValues("EMEquipStatus").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;
            Type = StaticFunctions.GetComboValues("EMEquipType").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;

        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
        public string StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string Status { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/TypeCombo", ComboForeignKeys = "")]
        [Display(Name = "Type")]
        public string TypeId { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/CategoryCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Model")]
        public string EquipModel { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Year")]
        public string ModelYr { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "VIN/Serial")]
        public string VINNumber { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Log Date")]
        public DateTime? LastUsedDate { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "TX Tag")]
        public string TXTagId { get; set; }

        [HiddenInput]
        public bool HasAttachment { get; set; }


    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Forms
{
    public class EquipmentSpecViewModel
    {
        public EquipmentSpecViewModel()
        {

        }

        public EquipmentSpecViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;

            GrossVehicleWeight = equipment.GrossVehicleWeight;
            TareWeight = equipment.TareWeight;
            Height = equipment.Height;
            Wheelbase = equipment.Wheelbase;
            NoAxles = equipment.NoAxles;
            Width = equipment.Width;
            OverallLength = equipment.OverallLength;
            HorsePower = equipment.HorsePower;
            TireSize = equipment.TireSize;

        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }


        [UIHint("LongBox")]
        [Display(Name = "Rated Weight")]
        public decimal GrossVehicleWeight { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Tare Weight")]
        public decimal TareWeight { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Height")]
        public string Height { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Wheelbase")]
        public string Wheelbase { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "# of Axles")]
        public byte NoAxles { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Width")]
        public string Width { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Overall Length")]
        public string OverallLength { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Horse Power")]
        public string HorsePower { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Tire Size")]
        public string TireSize { get; set; }


    }
}
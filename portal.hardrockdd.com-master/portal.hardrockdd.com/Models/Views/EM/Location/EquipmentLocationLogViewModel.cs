//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Equipment
//{
//    public class EquipmentLocationLogListViewModel
//    {
//        public EquipmentLocationLogListViewModel()
//        {

//        }

//        public EquipmentLocationLogListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
//        {
//            Co = equipment.EMCo;
//            EquipmentId = equipment.EquipmentId;
//            List = equipment.LocationLogs.Select(s => new EquipmentLocationLogViewModel(s)).ToList();
//        }

//        [Key]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        public string EquipmentId { get; set; }


//        public List<EquipmentLocationLogViewModel> List { get; set; }
//    }
//    public class EquipmentLocationLogViewModel
//    {

//        public EquipmentLocationLogViewModel()
//        {

//        }

//        public EquipmentLocationLogViewModel(EMLocationLog loc)
//        {
//            Co = loc.EMCo;
//            Mth = loc.Month;
//            TransId = loc.Trans;
//            FromLocationId = loc.FromLocationId;
//            ToLocationId = loc.ToLocationId;
//            DateIn = loc.DateIn;
//            DateOut = loc.DateOut;
//            Memo = loc.Memo;
//        }

//        [Key]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        public DateTime Mth { get; set; }

//        [Key]
//        [HiddenInput]
//        public int TransId { get; set; }

//        [UIHint("DropdownBox")]
//        [Display(Name = "From Location")]
//        [Field(ComboUrl = "/EMCombo/Location", ComboForeignKeys = "Co")]
//        public string FromLocationId { get; set; }

//        [UIHint("DropdownBox")]
//        [Display(Name = "To Location")]
//        [Field(ComboUrl = "/EMCombo/Location", ComboForeignKeys = "Co")]
//        public string ToLocationId { get; set; }

//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Date In")]
//        public DateTime? DateIn { get; set; }
        
//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Date Out")]
//        public DateTime? DateOut { get; set; }

//        [UIHint("Textbox")]
//        [Display(Name = "Memo")]
//        public string Memo { get; set; }
//    }
//}
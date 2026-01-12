using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment
{
    public class EquipmentLogListViewModel
    {
        public EquipmentLogListViewModel()
        {
            List = new List<EquipmentLogViewModel>();
        }

        public EquipmentLogListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }
            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
           
            List = equipment.Logs.Select(s => new EquipmentLogViewModel(s)).ToList();

        }

       

        public byte EMCo { get; set; }

        public string EquipmentId { get; set; }


        public List<EquipmentLogViewModel> List { get;  }
    }

    public class EquipmentLogViewModel
    {
        public EquipmentLogViewModel()
        {

        }
        
        public EquipmentLogViewModel(DB.Infrastructure.ViewPointDB.Data.EquipmentLog equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            SeqId = equipment.SeqId;
            LogTypeId = (DB.EMLogTypeEnum)equipment.LogTypeId;
            LogDate = equipment.LogDate;
            LoggedBy = equipment.LoggedBy;
            Notes = equipment.Notes;
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public string EquipmentId { get; set; }

        [Key]
        [HiddenInput]
        public int SeqId { get; set; }

        [Display(Name = "Type")]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public DB.EMLogTypeEnum LogTypeId { get; set; }

        [Display(Name = "LogDate")]
        [UIHint("Datebox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public DateTime LogDate { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Logged By Id")]
        public string LoggedBy { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
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
    public class EquipmentAssignmentLogListViewModel
    {
        public EquipmentAssignmentLogListViewModel()
        {
            List = new List<EquipmentAssignmentLogViewModel>();
        }

        public EquipmentAssignmentLogListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }
            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
           
            List = equipment.Logs.Where(f => (f.FromLocationId ?? "") != (f.ToLocationId ?? "") ||
                                             (f.FromCrewId ?? "") != (f.ToCrewId ?? "") ||
                                             (f.FromEmployeeId ?? 0) != (f.ToEmployeeId ?? 0) 
                                             )
                                 .Select(s => new EquipmentAssignmentLogViewModel(s))
                                 .ToList();

        }

       

        public byte EMCo { get; set; }

        public string EquipmentId { get; set; }


        public List<EquipmentAssignmentLogViewModel> List { get;  }
    }

    public class EquipmentAssignmentLogViewModel
    {
        public EquipmentAssignmentLogViewModel()
        {

        }
        
        public EquipmentAssignmentLogViewModel(DB.Infrastructure.ViewPointDB.Data.EquipmentLog log)
        {
            if (log == null)
            {
                throw new System.ArgumentNullException(nameof(log));
            }

            EMCo = log.EMCo;
            EquipmentId = log.EquipmentId;
            SeqId = log.SeqId;
            LogTypeId = (DB.EMLogTypeEnum)log.LogTypeId;
            LogDate = log.LogDate;
            LoggedBy = log.LoggedUser.PREmployee.FullName(false);

            if ((log.FromLocationId ?? "") != (log.ToLocationId ?? ""))
            {
                AssignmentType = DB.EMLogAssignmentType.Location;
                var locTo = log.db.EMLocations.FirstOrDefault(f => f.LocationId == log.ToLocationId);
                var locFrom = log.db.EMLocations.FirstOrDefault(f => f.LocationId == log.FromLocationId);

                ToId = locTo?.Description;
                FromId = locFrom?.Description;
            }
            else if ((log.FromCrewId ?? "") != (log.ToCrewId ?? ""))
            {
                AssignmentType = DB.EMLogAssignmentType.Crew;
                ToId = log.ToCrew?.Description;
                FromId = log.FromCrew?.Description;
            }
            else if ((log.FromEmployeeId ?? 0) != (log.ToEmployeeId ?? 0))
            {
                AssignmentType = DB.EMLogAssignmentType.Employee;
                ToId = log.ToEmployee.FullName(false);
                FromId = log.FromEmployee.FullName(false);
            }

            //FromEmployeeId = log.FromEmployee.FullName(false);
            //ToEmployeeId = log.ToEmployee.FullName(false);

            //FromCrewId = log.FromCrew?.Description;
            //ToCrewId = log.ToCrew?.Description;

            Notes = log.Notes;
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
        public DateTime LogDate { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "From Employee")]
        public string FromEmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "To Employee")]
        public string ToEmployeeId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "From Crew")]
        public string FromCrewId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "To Crew")]
        public string ToCrewId { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "From")]
        public string FromId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "To")]
        public string ToId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "AssignmentType")]
        public DB.EMLogAssignmentType AssignmentType { get; set; }
    }
}
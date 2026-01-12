using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment
{
    public class EquipmentListViewModel
    {
        public EquipmentListViewModel()
        {

        }

        public EquipmentListViewModel(List<SMRequest> list)
        {
            List = list.SelectMany(s => s.Lines)
                    .Where(s => s.Status != DB.SMRequestLineStatusEnum.Completed &&
                                s.Status != DB.SMRequestLineStatusEnum.Canceled &&
                                s.Status != DB.SMRequestLineStatusEnum.WorkOrderCompleted &&
                                s.Status != DB.SMRequestLineStatusEnum.WorkOrderCanceled &&
                                s.Equipment != null)
                    .GroupBy(g => new { g.Equipment })
                    .Select(s => new EquipmentViewModel()
                    {
                        EMCo = s.Key.Equipment.EMCo,
                        EquipmentId = s.Key.Equipment.EquipmentId,
                        EquipmentName = s.Key.Equipment.Description,
                        IsEquipmentDisabled = s.Max(max => max.IsEquipmentDisabled),
                        LastRequestDate = s.Max(max => max.Request.RequestDate),
                        FirstRequestDate = s.Min(min => min.Request.RequestDate),
                        RequestCount = s.Count()
                    }).ToList();
        }

        public EquipmentListViewModel(List<SMRequestLine> list)
        {
            List = list
                    .Where(s => s.StatusId == 0)
                    .GroupBy(g => new { g.Equipment })
                    .Select(s => new EquipmentViewModel()
                    {
                        EMCo = s.Key.Equipment.EMCo,
                        EquipmentId = s.Key.Equipment.EquipmentId,
                        EquipmentName = s.Key.Equipment.Description,
                        IsEquipmentDisabled = s.Max(max => max.IsEquipmentDisabled),
                        LastRequestDate = s.Max(max => max.Request.RequestDate),
                        FirstRequestDate = s.Min(min => min.Request.RequestDate),
                        RequestCount = s.Count()
                    }).ToList();
        }

        public List<EquipmentViewModel> List { get; }
    }

    public class EquipmentViewModel
    {
        public EquipmentViewModel()
        {

        }

        [Key]
        [HiddenInput]
        public byte? EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Equipment")]
        [UIHint("TextBox")]
        public string EquipmentName { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime LastRequestDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime FirstRequestDate { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Active")]
        public bool? IsEquipmentDisabled { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "# of Request")]
        public int RequestCount { get; set; }
    }
}
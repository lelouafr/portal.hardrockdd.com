using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditFormViewModel
    {
        public EquipmentAuditFormViewModel()
        {

        }

        public EquipmentAuditFormViewModel(EMAudit audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;

            Action = new EquipmentAuditActionViewModel(audit);
            Audit = new EquipmentAuditViewModel(audit);
            switch (Audit.AuditForm)
            {
                case DB.EMAuditFormEnum.Meter:
                    MeterLines = new EquipmentAuditMeterLineListViewModel(audit);
                    InventoryLines = new EquipmentAuditInventoryLineListViewModel();
                    break;
                case DB.EMAuditFormEnum.Inventory:
                    MeterLines = new EquipmentAuditMeterLineListViewModel();
                    InventoryLines = new EquipmentAuditInventoryLineListViewModel(audit);
                    break;
                default:
                    break;
            }
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }


        public EquipmentAuditActionViewModel Action { get; set; }
        
        public EquipmentAuditViewModel Audit { get; set; }

        public EquipmentAuditMeterLineListViewModel MeterLines { get; set; }
        
        public EquipmentAuditInventoryLineListViewModel InventoryLines { get; set; }
    }
}
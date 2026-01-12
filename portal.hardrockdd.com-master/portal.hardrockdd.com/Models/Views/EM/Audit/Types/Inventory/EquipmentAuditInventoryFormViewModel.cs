using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditInventoryFormViewModel
    {
        public EquipmentAuditInventoryFormViewModel()
        {

        }

        public EquipmentAuditInventoryFormViewModel(EMAudit audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;

            Action = new EquipmentAuditActionViewModel(audit);
            Audit = new EquipmentAuditViewModel(audit);
            Lines = new EquipmentAuditInventoryLineListViewModel(audit);
            StatusLogs = new EquipmentStatusLogListViewModel(audit);

            WorkFlowUsers = new WF.WorkFlowUserListViewModel(audit.WorkFlow.CurrentSequence());
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }


        public EquipmentAuditActionViewModel Action { get; set; }
        
        public EquipmentAuditViewModel Audit { get; set; }

        public EquipmentAuditInventoryLineListViewModel Lines { get; set; }


        public EquipmentStatusLogListViewModel StatusLogs { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }
    }
}
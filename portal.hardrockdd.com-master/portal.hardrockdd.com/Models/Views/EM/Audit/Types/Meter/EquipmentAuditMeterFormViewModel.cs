using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditMeterFormViewModel
    {
        public EquipmentAuditMeterFormViewModel()
        {

        }

        public EquipmentAuditMeterFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null) throw new System.ArgumentNullException(nameof(ticket));
            var company = ticket.HQCompanyParm;
            //var audit = ticket.CreatedUser.ActiveEquipmentAudits().FirstOrDefault();

            foreach (var audit in ticket.CreatedUser.ActiveEquipmentAudits())
            {
                if (audit != null && audit.IsAuditLate(ticket))
                {
                    EMCo = audit.EMCo;
                    AuditId = audit.AuditId;

                    Action = new EquipmentAuditActionViewModel(audit);
                    Audit = new EquipmentAuditViewModel(audit);
                    Lines = new EquipmentAuditMeterLineListViewModel(audit);
                    StatusLogs = new EquipmentStatusLogListViewModel(audit);
                    WorkFlowUsers = new WF.WorkFlowUserListViewModel(audit.WorkFlow.CurrentSequence());
                }

            }
        }

        public EquipmentAuditMeterFormViewModel(EMAudit audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;

            Action = new EquipmentAuditActionViewModel(audit);
            Audit = new EquipmentAuditViewModel(audit);
            Lines = new EquipmentAuditMeterLineListViewModel(audit);
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
        
        public EquipmentAuditMeterLineListViewModel Lines { get; set; }

        public EquipmentStatusLogListViewModel StatusLogs { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }
    }
}
using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditActionViewModel
    {
        public EquipmentAuditActionViewModel()
        {


        }

        public EquipmentAuditActionViewModel(EMAudit entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            EMCo = entity.EMCo;
            AuditId = entity.AuditId;
            CanSubmit = false;
            CanUnSubmit = false;
            CanCancel = false;
            CanApprove = false;
            CanReject = false;
            CanProcess = false;

            var userId = StaticFunctions.GetUserId();
            if (entity.WorkFlow == null)
            {
                entity.Status = DB.EMAuditStatusEnum.New;
            }
            var inWorkFlow = entity.WorkFlow.IsUserInWorkFlow(userId);
            Access = DB.SessionAccess.View;
            switch (entity.Status)
            {
                case DB.EMAuditStatusEnum.New:
                case DB.EMAuditStatusEnum.Started:
                case DB.EMAuditStatusEnum.Rejected:
                    if (inWorkFlow ||
                        HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;
                        if (HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                        {
                            CanCancel = true;
                        }
                    }
                    break;
                case DB.EMAuditStatusEnum.Submitted:
                    if (inWorkFlow ||
                        HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanReject = true;
                        if (HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                        {
                            CanCancel = true;
                        }
                    }
                    if (entity.AssignedTo == userId)
                    {
                        CanUnSubmit = true;
                    }
                    break;
                case DB.EMAuditStatusEnum.Approved:
                    if (inWorkFlow ||
                        HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanProcess = true;
                        CanReject = true;
                        if (HttpContext.Current.User.HasFullAccess("EMAudit", "Index"))
                        {
                            CanCancel = true;
                        }
                    }
                    if (entity.AssignedTo == userId)
                    {
                        CanUnSubmit = true;
                    }
                    break;
                case DB.EMAuditStatusEnum.Processed:
                case DB.EMAuditStatusEnum.Completed:
                    if (entity.AssignedTo == userId && entity.IsCurrentWeek())
                    {
                        CanUnSubmit = true;
                    }
                    break;
                case DB.EMAuditStatusEnum.Canceled:
                default:
                    break;
            }

            //CanReject = true;

            //if (HttpContext.Current.User.IsInRole("Admin"))
            //    Access = DB.SessionAccess.Edit;

        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "EMCo")]
        public byte EMCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int AuditId { get; set; }

        public DB.SessionAccess Access { get; set; }

        public bool CanSubmit { get; set; }

        public bool CanUnSubmit { get; set; }

        public bool CanReject { get; set; }

        public bool CanCancel { get; set; }

        public bool CanApprove { get; set; }

        public bool CanProcess { get; set; }
    }


}
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            PRCo = entity.PRCo;
            RequestId = entity.RequestId;
            CanUnSubmit = false;
            CanUnDelete = false;
            CanCancel = false;
            CanApprove = false;
            CanReject = false;
            CanSave = false;

            var userId = StaticFunctions.GetUserId();
            var inWorkFlow = entity.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == entity.Status && w.Active == "Y");
            Access = DB.SessionAccess.View;
            switch ((DB.LeaveRequestStatusEnum)entity.Status)
            {
                case DB.LeaveRequestStatusEnum.Canceled:
                    if (inWorkFlow)
                    {
                        CanUnDelete = true;
                    }
                    break;
                case DB.LeaveRequestStatusEnum.Rejected:
                case DB.LeaveRequestStatusEnum.Open:
                    if (HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR"))
                    {
                        CanCancel = true;
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;
                        CanCancel = true;
                        CanSave = true;
                    }
                    Access = StaticFunctions.GetUserId() == entity.CreatedBy ? DB.SessionAccess.Edit : Access;
                    if (inWorkFlow)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;                        
                        CanCancel = true;
                        CanSave = true;
                    }
                    break;
                case DB.LeaveRequestStatusEnum.Submitted:
                    CanUnSubmit = StaticFunctions.GetUserId() == entity.CreatedBy;
                    if (HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR"))
                    {
                        CanCancel = true;
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;
                        CanCancel = true;
                        CanSave = true;
                    }
                    if (inWorkFlow || HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanReject = true;
                    }
                    break;
                case DB.LeaveRequestStatusEnum.Approved:
                    if (HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR"))
                    {
                        CanCancel = true;
                        Access = DB.SessionAccess.Edit;
                        CanReject = true;
                    }
                    if (inWorkFlow || HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR"))
                    {
                        CanCancel = true;
                        CanReject = true;
                    }
                    break;
                case DB.LeaveRequestStatusEnum.Processed:
                default:
                    break;
            }
            //if (HttpContext.Current.User.IsInRole("Admin"))
            //    Access = DB.SessionAccess.Edit;
           
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        public DB.SessionAccess Access { get; set; }

        public bool CanSave { get; set; }

        public bool CanUnSubmit { get; set; }

        public bool CanUnDelete { get; set; }

        public bool CanSubmit { get; set; }

        public bool CanCancel { get; set; }

        public bool CanApprove { get; set; }

        public bool CanReject { get; set; }

        public bool CanProcess { get; set; }
    }


}
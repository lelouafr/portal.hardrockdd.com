using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            RequestId = entity.RequestId;
            CanUnSubmit = false;
            CanUnDelete = false;
            CanCancel = false;
            CanApprove = false;
            CanReject = false;
            CanSave = false;
            CanCreatePO = false;
            IsPONew = entity.IsPONew;

            var userId = StaticFunctions.GetUserId();
            var inWorkFlow = entity.WorkFlow.IsUserInWorkFlow(userId);
            Access = DB.SessionAccess.View;
            switch ((DB.PORequestStatusEnum)entity.Status)
            {
                case DB.PORequestStatusEnum.Canceled:
                    if (inWorkFlow)
                    {
                        CanUnDelete = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Rejected:
                case DB.PORequestStatusEnum.Open:
                    if (HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                        CanCancel = true;
                    Access = StaticFunctions.GetUserId() == entity.CreatedBy ? DB.SessionAccess.Edit : Access;
                    if (inWorkFlow)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanSubmit = true;                        
                        CanCancel = true;
                        CanSave = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Submitted:
                    CanUnSubmit = StaticFunctions.GetUserId() == entity.CreatedBy;
                    if (HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                        CanCancel = true;
                    if (inWorkFlow || 
                        HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanReject = true;
                    }
                    break;
                case DB.PORequestStatusEnum.SupApproved:
                    CanUnSubmit = StaticFunctions.GetUserId() == entity.CreatedBy;
                    if (HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                        CanCancel = true;
                    if (inWorkFlow || HttpContext.Current.User.IsInRole("Admin") ||
                                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                                        HttpContext.Current.User.IsInPosition("CFO"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanReject = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Approved:
                    if (HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                        Access = DB.SessionAccess.Edit;

                    if (inWorkFlow || HttpContext.Current.User.IsInRole("Admin") ||
                        HttpContext.Current.User.IsInPosition("FIN-APMGR") ||
                        HttpContext.Current.User.IsInPosition("FIN-AP") ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO"))
                    {
                        if (entity.PO == null)
                            CanCreatePO = true;
                        CanCancel = true;
                        CanReject = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Processed:
                default:
                    break;
            }
            //if (HttpContext.Current.User.IsInRole("Admin"))
            //    Access = DB.SessionAccess.Edit;
           
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte POCo { get; set; }

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
        
        public bool CanCreatePO { get; set; }

        public bool IsPONew { get; set; }
    }


}
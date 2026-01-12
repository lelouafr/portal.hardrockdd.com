using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Order
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(DB.Infrastructure.ViewPointDB.Data.PurchaseOrder entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            PO = entity.PO;
            CanUnSubmit = false;
            CanUnDelete = false;
            CanCancel = false;
            CanApprove = false;
            CanReject = false;
            CanSave = false;

            //var userId = StaticFunctions.GetUserId();
            //var inWorkFlow = entity.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == entity.Status && w.Active == "Y");
            Access = DB.SessionAccess.View;
            /*
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
                    if (HttpContext.Current.User.IsInRole("Admin"))
                        CanCancel = true;
                    if (inWorkFlow)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanApprove = true;
                        CanReject = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Approved:
                    if (inWorkFlow)
                    {
                        CanCancel = true;
                        CanReject = true;
                    }
                    break;
                case DB.PORequestStatusEnum.Processed:
                default:
                    break;
            }
            */
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
        [Display(Name = "PO")]
        public string PO { get; set; }

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
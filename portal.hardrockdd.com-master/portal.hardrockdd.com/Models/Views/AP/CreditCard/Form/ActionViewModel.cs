using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(CreditTransaction entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            CCCo = entity.CCCo;
            TransId = entity.TransId;
            CanForward = false;

            var inWorkFlow = entity.GetWorkFlow().CurrentSequence().AssignedUsers.Any(f => f.AssignedTo == entity.db.CurrentUserId);

            Access = DB.SessionAccess.View;
            switch (entity.Status)
            {
                case DB.CMTranStatusEnum.Locked:
                    break;
                case DB.CMTranStatusEnum.APPosted:
                    break;
                case DB.CMTranStatusEnum.Open:
                case DB.CMTranStatusEnum.RequestedInfomation:
                    if (inWorkFlow ||
                        HttpContext.Current.User.IsInPosition("FIN-CTRL") ||
                        HttpContext.Current.User.IsInPosition("CFO") ||
                        HttpContext.Current.User.IsInPosition("IT-DIR"))
                    {
                        CanForward = true;
                    }
                    Access = DB.SessionAccess.Edit;
                    break;
                default:
                    break;
            }

        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte CCCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public long TransId { get; set; }


        public DB.SessionAccess Access { get; set; }

        public bool CanForward { get; set; }
    }


}
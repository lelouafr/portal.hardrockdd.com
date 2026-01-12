using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class RequestInfoViewModel
    {


        public RequestInfoViewModel()
        {

        }

        public RequestInfoViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction transaction)
        {
            if (transaction == null)
                return;

            CCCo = transaction.CCCo;
            TransId = transaction.TransId;

            SendTo = transaction.db.CurrentUserId;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte CCCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Trans Id")]
        public long TransId { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 3, TextSize = 9)]
        public string Comments { get; set; }

        [Required]
        [UIHint("TagBox")]
        [Display(Name = "SendTo")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        public string SendTo { get; set; }


        internal RequestInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState, ControllerContext controllerContext)
        {
            var updObj = db.CreditTransactions.FirstOrDefault(f => f.CCCo == this.CCCo && f.TransId == this.TransId);

            if (updObj != null)
            {
                var userList = this.SendTo.Split('|');

                //updObj.RequestedUserList = this.SendTo;
                updObj.Status = DB.CMTranStatusEnum.RequestedInfomation;

                if (updObj.Forum == null)
                    updObj.AddForum();

                var line = updObj.Forum.AddLine();

                line.Comment = this.Comments;
                line.HtmlComment = this.Comments;

                foreach (var userid in userList)
                {
                    var user = db.WebUsers.FirstOrDefault(f => f.Id == userid);
                    line.Comment += string.Format(AppCultureInfo.CInfo(), "{2} Sent To: {0} {1}", user.FirstName, user.LastName, System.Environment.NewLine);
                    line.HtmlComment += string.Format(AppCultureInfo.CInfo(), @"<br> <small>Sent To: {0} {1}</small>", user.FirstName, user.LastName);
                                        
                    updObj.WorkFlow.AddUser(user);
                }
                this.EmailRequestInfo(updObj, controllerContext);
                try
                {
                    db.BulkSaveChanges();
                    return new RequestInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

        private void EmailRequestInfo(CreditTransaction trans, ControllerContext controllerContext)
        {
            var model = new TransactionFormViewModel(trans);
            try
            {
                using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                {
                    Body = Code.EmailHelper.RenderViewToString(controllerContext, "../AP/CreditCard/Email/EmailRequestedInfo", model, false),
                    IsBodyHtml = true,
                    Subject = string.Format(AppCultureInfo.CInfo(), "Requested Credit Card Transaction Info on {0} for {1}", trans.TransDate, trans.Merchant.Name),
                };
                foreach (var workflow in trans.WorkFlow.CurrentSequence().AssignedUsers)
                {
                    msg.To.Add(new System.Net.Mail.MailAddress(workflow.AssignedUser.Email));
                }
                Code.EmailHelper.Send(msg);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
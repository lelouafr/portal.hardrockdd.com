using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace portal.Models.Views.AP.Document
{
    public class DocumentPendingInfoViewModel
    {
        public DocumentPendingInfoViewModel()
        {

        }

        public DocumentPendingInfoViewModel(APDocument document)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            APCo = document.APCo;
            DocId = document.DocId;

            SendTo = StaticFunctions.GetUserId();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Request Id")]
        public int DocId { get; set; }

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

        internal DocumentPendingInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState, ControllerContext controllerContext)
        {
            var updObj = db.APDocuments.FirstOrDefault(f => f.APCo == this.APCo && f.DocId == this.DocId);

            if (updObj != null)
            {
                var userList = this.SendTo.Split('|');

                updObj.RequestedUserList = this.SendTo;
                updObj.Status = DB.APDocumentStatusEnum.RequestedInfo;

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

                    //Users already added in status update!
                    //updObj.WorkFlow.AddUser(user);
                }

                this.EmailRequestInfo(updObj, controllerContext);
                try
                {
                    db.BulkSaveChanges();
                    return new DocumentPendingInfoViewModel(updObj);
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


        private void EmailRequestInfo(APDocument document, ControllerContext controllerContext)
        {
            var model = new DocumentFormViewModel(document);
            try
            {
                using MailMessage msg = new MailMessage()
                {
                    Body = Code.EmailHelper.RenderViewToString(controllerContext, "../AP/Document/Email/EmailInvoiceRequestedInfo", model, false),
                    IsBodyHtml = true,
                    Subject = string.Format(AppCultureInfo.CInfo(), "Requested Invoice Info {0}", model.Document.APEntry.APReference),
                };
                foreach (var workflow in document.WorkFlow.CurrentSequence().AssignedUsers)
                {
                    msg.To.Add(new MailAddress(workflow.AssignedUser.Email));
                }
                Code.EmailHelper.Send(msg);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
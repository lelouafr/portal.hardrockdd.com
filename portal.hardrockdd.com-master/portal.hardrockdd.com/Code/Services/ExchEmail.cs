using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Exchange.WebServices.Data;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.AP;

namespace portal
{
    public static class ExchEmail
    {
        public static void OpenEmail()
        {
            return;
            ExchangeService service = new ExchangeService()
            {
                Credentials = new WebCredentials("APDocument@hardrockdd.com", "HR@pD0c4ment2022$")
            };
            using var db = new VPContext();
            //service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            service.AutodiscoverUrl("APDocument@hardrockdd.com", RedirectionUrlValidationCallback);
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);
            ItemView view = new ItemView(int.MaxValue);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);
            var apComp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).APCompanyParm;
            foreach (EmailMessage item in findResults.Items)
            {
                item.Load();
                if (item.HasAttachments)
                {
                    foreach (var i in item.Attachments)
                    {
                        try
                        {
                            if (i.IsInline == false)
                            {
                                var fileAttachment = (FileAttachment)i;
                                apComp.AddDocument(item, fileAttachment);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                }
                //set mail as read 
                item.IsRead = true;

                //item.Delete(DeleteMode.MoveToDeletedItems);
                item.Update(ConflictResolutionMode.AutoResolve);

            }
        }
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }
    }
}
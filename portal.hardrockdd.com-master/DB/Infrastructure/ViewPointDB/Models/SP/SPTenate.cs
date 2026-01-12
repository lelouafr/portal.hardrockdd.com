using DB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    [System.ComponentModel.DataAnnotations.Schema.Table("budSPTM", Schema = "dbo")]
    public partial class SPTenate
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budSPTM"; } }

        public Uri SiteUri { get => new Uri(Url); set => Url = value.OriginalString; }

        public string UserName
        {
            get
            {
                if (!string.IsNullOrEmpty(tUserName))
                    return AESEncryption.Decrypt(tUserName);
                return "";
            }
            set
            {
                if (value != tUserName)
                {
                    if (string.IsNullOrEmpty(value) && tUserName == null)
                        tUserName = value;
                    else if (!string.IsNullOrEmpty(value))
                        tUserName = AESEncryption.Encrypt(value);
                }
            }
        }

        public string Password
        {
            get
            {
                if (!string.IsNullOrEmpty(tPassword))
                    return AESEncryption.Decrypt(tPassword);
                return "";
            }
            set
            {
                if (value != tPassword)
                {
                    if (string.IsNullOrEmpty(value) && tPassword == null)
                        tPassword = value;
                    else if (!string.IsNullOrEmpty(value))
                        tPassword = AESEncryption.Encrypt(value);
                }
            }
        }
        private System.Security.SecureString _SecurePassword;
        public System.Security.SecureString SecurePassword
        {
            get
            {
                if (_SecurePassword == null)
                {
                    _SecurePassword = new System.Security.SecureString();
                    Password.ToCharArray().ToList().ForEach(e => _SecurePassword.AppendChar(e));
                }
                return _SecurePassword;

            }
        }

        private Microsoft.SharePoint.Client.SharePointOnlineCredentials _Credentials;
        public Microsoft.SharePoint.Client.SharePointOnlineCredentials Credentials
        {
            get 
            {
                if (_Credentials == null)
                    _Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(UserName, SecurePassword);

                return _Credentials;
            }
        }

        public SPSite GetSite(string siteName)
        {
            var site = this.Sites.FirstOrDefault(f => f.Name == siteName);
            if (site == null)
            {
                var siteId = db.GetNextId(SPSite.BaseTableName);
                site = new SPSite()
                {
                    db = this.db,
                    Tenate = this,
                    TenateId = this.TenateId,
                    SiteId = siteId,
                    Name = siteName
                };

                var ctx = new Microsoft.SharePoint.Client.ClientContext(site.Uri) { Credentials = this.Credentials };
                ctx.Load(ctx.Web);
                try
                {
                    ctx.ExecuteQuery();
                    var sharePointSite = ctx.Web;
                    site.SiteGuidId = ctx.Web.Id;

                    this.Sites.Add(site);

                }
                catch (Exception)
                {
                    return null;
                }
            }
            return site;
        }
    }
}

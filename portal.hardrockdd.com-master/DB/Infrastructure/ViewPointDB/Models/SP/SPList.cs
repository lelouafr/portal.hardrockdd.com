using DB.Infrastructure.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    [System.ComponentModel.DataAnnotations.Schema.Table("budSPLM", Schema = "dbo")]
    public partial class SPList
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
                _db ??= this.Site.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budSPLM"; } }

        //public Folder GetFolder(string folderPath)
        //{
        //    var web = this.Site.SharePointClient.Context.Web;
        //    var folder = web.GetFolder(this.Name, folderPath);
        //    return folder;

        //}

        private Microsoft.SharePoint.Client.List _sharePointList;
        public Microsoft.SharePoint.Client.List GetSharePointList()
        {
            if (_sharePointList == null)
            {
                var ctx = this.Site.Context;
                var web = ctx.Web;
                if (!web.IsPropertyAvailable("Lists"))
                {
                    web.Context.Load(web, f => f.Lists);
                    ctx.ExecuteQuery();
                }
                _sharePointList = web.Lists.FirstOrDefault(l => l.Title == this.Name);
            }

            return _sharePointList;
        }

    }
}

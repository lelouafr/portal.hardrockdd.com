using DB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    [System.ComponentModel.DataAnnotations.Schema.Table("budSPSM", Schema = "dbo")]
    public partial class SPSite
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
                _db ??= this.Tenate.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budSPSM"; } }


        public string Url { get => string.Concat(Tenate.Url, "/sites/", Name); }

        public Uri Uri { get => new Uri(Url); }

        private Microsoft.SharePoint.Client.ClientContext _Context;
        public Microsoft.SharePoint.Client.ClientContext Context
        {
            get
            {
                if (_Context == null)
                {
                    var ctx = new Microsoft.SharePoint.Client.ClientContext(Uri) { Credentials = this.Tenate.Credentials };
                    ctx.Load(ctx.Web, w => w.Url, w => w.Url, w => w.RootFolder.ServerRelativeUrl);
                    ctx.ExecuteQuery();
                    _Context = ctx;
                }

                return _Context;
            }
        }


        public SPList AddList(Microsoft.SharePoint.Client.List sharePointList)
        {
            if (sharePointList == null)
                return null;

            var list = this.Lists.FirstOrDefault(f => f.ListGuidId == sharePointList.Id);
            if (list == null)
                list = db.SPLists.FirstOrDefault(f => f.ListGuidId == sharePointList.Id);

            if (list == null)
            {
                list = new SPList()
                {
                    db = this.db,
                    Site = this,
                    ListId = db.GetNextId(BaseTableName),
                    Name = sharePointList.Title,
                    ListGuidId = sharePointList.Id,
                    SiteId = this.SiteId,
                };
                this.Lists.Add(list);
            }
            else
            {
                if (list.Name != sharePointList.Title)
                    list.Name = sharePointList.Title;

                if (list.SiteId  != this.SiteId)
                    list.SiteId = this.SiteId;
            }
            return list;
        }

        private Microsoft.SharePoint.Client.List _SharePointList;
        public SPList GetList(string listName, bool autoCreate = false)
        {
            var list = this.Lists.FirstOrDefault(f => f.Name == listName);
            if (list == null)
            {
                var sharePointList = Context.Web.Lists.GetByTitle(listName);
                Context.Load(sharePointList, f => f);
                try
                {
                    Context.ExecuteQuery();
                    list = AddList(sharePointList);
                }
                catch (Exception)
                {
                    if (autoCreate)
                    {
                        var creationInfo = new Microsoft.SharePoint.Client.ListCreationInformation();
                        creationInfo.Title = listName ;
                        creationInfo.TemplateType = (int)Microsoft.SharePoint.Client.ListTemplateType.DocumentLibrary;
                        sharePointList = Context.Web.Lists.Add(creationInfo);
                        sharePointList.Description = listName;
                        sharePointList.ListExperienceOptions = Microsoft.SharePoint.Client.ListExperience.NewExperience;
                        sharePointList.Update();

                        sharePointList = Context.Web.Lists.GetByTitle(listName);
                        Context.Load(sharePointList, f => f);
                        Context.Load(sharePointList, f => f.RootFolder);
                        Context.ExecuteQuery();

                        var navigation = Context.Web.Navigation;
                        var topNavigationCollection = navigation.TopNavigationBar;
                        Context.Load(sharePointList, f => f.DefaultViewUrl);
                        Context.ExecuteQuery();

                        var navItem = new Microsoft.SharePoint.Client.NavigationNodeCreationInformation();
                        navItem.Title = listName;
                        navItem.Url = sharePointList.RootFolder.ServerRelativeUrl;
                        navItem.AsLastNode = true;

                        navigation.QuickLaunch.Add(navItem);


                        Context.Load(navigation);
                        Context.ExecuteQuery();

                        topNavigationCollection.Add(navItem);
                        Context.Load(topNavigationCollection);
                        Context.ExecuteQuery();

                        return this.GetList(listName, false);
                    }

                    return null;
                }
            }

            return list;
        }

        public void SyncListFromSharePoint()
        {
            Context.Load(Context.Web, f => f.Lists.Where(l => l.BaseTemplate == (int)Microsoft.SharePoint.Client.ListTemplateType.DocumentLibrary));
            Context.ExecuteQuery();
            
            var lists = Context.Web.Lists.ToList();
            lists.ForEach(e => this.AddList(e));
        }
    }
}

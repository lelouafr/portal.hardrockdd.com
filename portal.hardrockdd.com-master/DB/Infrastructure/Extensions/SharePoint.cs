using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.Extensions
{
    public static class FolderExtensions
    {


        public static void MoveFilesTo(this Folder folder, string folderUrl)
        {
            var ctx = (ClientContext)folder.Context;

            if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl") ||
                !folder.IsPropertyAvailable("Folders") ||
                !folder.IsPropertyAvailable("Files") ||
                !folder.IsPropertyAvailable("ServerRelativeUrl"))
            {
                if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                    ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                if (!folder.IsPropertyAvailable("Folders"))
                    ctx.Load(folder, f => f.Folders);
                if (!folder.IsPropertyAvailable("Files"))
                    ctx.Load(folder, f => f.Files);
                if (!folder.IsPropertyAvailable("ServerRelativeUrl"))
                    ctx.Load(folder, f => f.ServerRelativeUrl);
                ctx.ExecuteQuery();
            }
            if (!folderUrl.Contains(ctx.Web.ServerRelativeUrl))
                folderUrl = ctx.Web.ServerRelativeUrl + folderUrl;

            //Ensure target folder exists
            ctx.Web.EnsureFolder(folderUrl.Replace(ctx.Web.ServerRelativeUrl, string.Empty));
            foreach (var file in folder.Files)
            {
                var targetFileUrl = file.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                targetFileUrl = targetFileUrl.Replace("//", "/");
                file.MoveTo(targetFileUrl, MoveOperations.Overwrite);
            }
            ctx.ExecuteQuery();

            foreach (var subFolder in folder.Folders)
            {
                var targetFolderUrl = subFolder.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                targetFolderUrl = targetFolderUrl.Replace("//", "/");
                
                subFolder.MoveFilesTo(targetFolderUrl);
            }
        }

    }

    static class WebExtensions
    {
        /// <summary>
        /// Ensures whether the folder exists   
        /// </summary>
        /// <param name="web"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public static Folder EnsureFolder(this Web web, string folderUrl)
        {
            return EnsureFolderInternal(web.RootFolder, folderUrl);
        }


        private static Folder EnsureFolderInternal(Folder parentFolder, string folderUrl)
        {
            var ctx = parentFolder.Context;
            var folderNames = folderUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var folderName = folderNames[0].Trim();

            if (!parentFolder.IsPropertyAvailable("Folders"))
            {
                ctx.Load(parentFolder, f => f.Folders);
                ctx.ExecuteQuery();
            }

            var folder = parentFolder.Folders.FirstOrDefault(f => f.Name.ToLower() == folderName.ToLower());
            if (folder == null)
            {
                var prams = new Microsoft.SharePoint.Client.ListItemUpdateParameters();
                parentFolder.AddSubFolder(folderName, prams);
                parentFolder.Context.ExecuteQuery();
                if (!parentFolder.IsPropertyAvailable("Folders"))
                {
                    ctx.Load(parentFolder, f => f.Folders);
                    ctx.ExecuteQuery();
                }
                folder = parentFolder.Folders.FirstOrDefault(f => f.Name.ToLower() == folderName.ToLower());
            }

            if (folderNames.Length > 1)
            {
                var subFolderUrl = string.Join("/", folderNames, 1, folderNames.Length - 1);
                return EnsureFolderInternal(folder, subFolderUrl);
            }
            return folder;
        }
    }
}

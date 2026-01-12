using DB.Infrastructure.Extensions;
using DB.Infrastructure.Services;
using DB.Infrastructure.VPAttachmentDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQAttachment
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
                _db ??= HQCompanyParm?.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public string UncPath
        {
            get
            {
                var path = string.Format("{0}{1}", db.AttachmentShare, tUncPath);
                return path;
            }

            set => tUncPath = value;// UpdateUncPath(value);
        }

        //public void UpdateUncPath(string value)
        //{
        //    if (tUncPath != value)
        //    {
        //        //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
        //        {
        //            var oldPath = string.Format("{0}{1}", db.AttachmentShare, tUncPath);
        //            var newPath = string.Format("{0}{1}", db.AttachmentShare, value);

        //            if (!string.IsNullOrEmpty(tUncPath))
        //            {
        //                DirectoryExt.Merge(oldPath, newPath);
        //            }
        //            else if (!string.IsNullOrEmpty(newPath))
        //            {
        //                tUncPath = value;
        //                System.IO.Directory.CreateDirectory(newPath);
        //                this.GetRootFolder().MoveToShare();
        //            }
        //            else
        //            {
        //                //Todo: move files from share to db?
        //            }
        //        }
        //        tUncPath = value;
        //    }
        //}
        public string SharePointRootFolder
        {
            get => tSharePointRootFolder;
            set => UpdateSharePointRootFolder(value);
        }

        public void UpdateSharePointRootFolder(string value)
        {
            if (value == null)
                return;
            var root = GetRootFolder();
            value = value.Replace("\\", "/");
            tSharePointRootFolder ??= root.SharePointFolderPath;
            if (tSharePointRootFolder != value)
            {
                if (this.SharePointList == null)
                {
                    var list = db.SPLists.FirstOrDefault(f => f.ListId == this.SPListId);
                    if (list != null)
                        SharePointList = list;
                }
                if (string.IsNullOrEmpty(tSharePointRootFolder))
                {
                    tSharePointRootFolder = value;
                }
                else
                {
                    switch (root.StorageLocation)
                    {
                        case HQAttachmentStorageEnum.DB:
                        case HQAttachmentStorageEnum.FileSystem:
                        case HQAttachmentStorageEnum.DBAndSharePoint:
                            tSharePointRootFolder = value;
                            MoveSharePointRootFolder(value);
                            break;
                        case HQAttachmentStorageEnum.SharePoint:
                            MoveSharePointRootFolder(value);
                            tSharePointRootFolder = value;
                            break;
                        default:
                            break;
                    }
                }

                root.SharePointFolderPath = value;
            }
        }

        private void MoveSharePointRootFolder(string newFolderpath)
        {
            var root = GetRootFolder();
            var folder = root.GetFolderSharePoint();
            if (folder != null)
            {
                var spList = SharePointList.GetSharePointList();
                var ctx = (Microsoft.SharePoint.Client.ClientContext)spList.Context;
                if (!spList.IsPropertyAvailable("RootFolder"))
                    ctx.Load(spList, w => w.RootFolder);
                if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                    ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                ctx.ExecuteQuery();

                var webUrl = ctx.Web.ServerRelativeUrl;
                var listUrl = spList.RootFolder.ServerRelativeUrl;
                listUrl = listUrl.Replace(webUrl, string.Empty);


                var folderUrl = listUrl + "/" + newFolderpath;
                folderUrl = folderUrl.Replace("\\", "/");
                folder.MoveFilesTo(folderUrl);
                
                
                folder.DeleteObject();
                folder.Context.ExecuteQuery();
            }
        }

        public HQAttachmentFolder GetRootFolder()
        {
            var root = Folders.FirstOrDefault(f => f.tDescription == "root");
            if (root == null)
            {           
                this.BuildDefaultFolders();
                root = Folders.FirstOrDefault(f => f.tDescription == "root");                
            }
            if (this.Files.Any(f => f.FolderId == null))
                this.AssignFilesToRoot();
            return root;
        }

        public void AssignFilesToRoot()
        {
            if (Files.Any(f => f.FolderId == null || f.Folder == null))
            {
                var root = Folders.FirstOrDefault(f => f.tDescription == "root");
                if (root == null)
                    return;
                this.Files.Where(f => f.FolderId == null || f.Folder == null).ToList().ForEach(e => {
                    e.Folder = root;
                    e.FolderId = root.FolderId;
                    root.Files.Add(e);
                });
            }
        }

        public void BuildDefaultFolders()
        {
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == this.TableName).ToList();

            var root = this.Folders.FirstOrDefault(f => f.tDescription == "root");
            if (root == null)
                root = AddFolder("root", null);

            foreach (var attachmentType in attachmentTypes)
            {
                var folder = this.Folders.FirstOrDefault(f => f.AttachmentTypeId == attachmentType.AttachmentTypeID);
                if (folder == null)
                    folder = this.Folders.FirstOrDefault(f => f.tDescription == attachmentType.Description);
                if (folder == null)
                    folder = root.AddFolder(attachmentType.Description);

                folder.AttachmentTypeId = attachmentType.AttachmentTypeID;
            }
            AssignFilesToRoot();
        }

        public HQAttachmentFolder AddFolder(string name, HQAttachmentFolder parentFolder = null)
        {
            var folder = this.Folders.FirstOrDefault(f => f.ParentId == parentFolder?.FolderId && f.tDescription == name);

            if (folder == null)
            {
                folder = new HQAttachmentFolder()
                {
                    Attachment = this,
                    db = this.db,

                    HQCo = HQCo,
                    UniqueAttchID = UniqueAttchID,
                    FolderId = Folders.DefaultIfEmpty().Max(max => max == null ? -1 : max.FolderId) + 1,
                    tDescription = name,
                    tParentId = parentFolder?.FolderId,
                };
                this.Folders.Add(folder);
                if (parentFolder != null)
                    parentFolder.SubFolders.Add(folder);
            }
            if (parentFolder != null && !parentFolder.SubFolders.Any(f => f.FolderId == folder.FolderId))
                parentFolder.SubFolders.Add(folder);

            if (!string.IsNullOrEmpty(this.SharePointRootFolder))
            {
                folder.SharePointFolderPath = this.SharePointRootFolder;
                folder.SharePointListId = this.SPListId;
                folder.StorageLocation = HQAttachmentStorageEnum.SharePoint;
            }

            //if (!string.IsNullOrEmpty(tUncPath))
            //{
            //    folder.FolderPath = folder.GetFolderPath();

            //    //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            //    {
            //        var uncPath = string.Format(@"{0}{1}", UncPath, folder.FolderPath);
            //        System.IO.Directory.CreateDirectory(uncPath);
            //    };
            //}

            return folder;
        }

        public static HQAttachment FindCreate(byte hqco, Guid? uniqueAttchID, string tableName, long keyID, VPContext db)
        {
            if (uniqueAttchID == null)
            {
                uniqueAttchID = Guid.NewGuid();
                db.Database.ExecuteSqlCommand(string.Format("UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyID));
            }
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            if (attachment == null)
            {
                attachment = new HQAttachment()
                {
                    HQCo = hqco,
                    UniqueAttchID = (Guid)uniqueAttchID,
                    TableName = tableName,
                    TableKeyId = keyID,
                };
                db.HQAttachments.Add(attachment);
                db.BulkSaveChanges();
            }

            return attachment;
        }

        public static HQAttachment Init(byte co, string tableName, long keyID)
        {
            var uniqueAttchID = Guid.NewGuid();
            //using var db = new VPContext();

            //db.Database.ExecuteSqlCommand(string.Format("UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyID));

            var attachment = new HQAttachment() { 
                HQCo = co,
                UniqueAttchID = uniqueAttchID,
                TableKeyId = keyID,
                TableName = tableName,
            };

            attachment.BuildDefaultFolders();

            return attachment;
        }

        public HQAttachmentFolder AddFiles(HttpFileCollectionBase files, int folderId, string folderPath)
        {
            var folder = GetRootFolder();
            foreach (string requestFileName in files)
            {
                HttpPostedFileBase fileUpload = files[requestFileName];
                var fileName = fileUpload.FileName;
                var dstFolder = folderPath.Replace(fileName, "");
                if (dstFolder == "undefined")
                    dstFolder = "";

                folder = BuildFolderTree(folderId, dstFolder);
                db.BulkSaveChanges();

                var file = folder.Files.FirstOrDefault(f => f.OrigFileName == fileName);
                if (file == null)
                    file = folder.AddFile(fileUpload);
                else
                    file.SaveData(fileUpload);

                db.BulkSaveChanges();
            }

            return folder;
        }

        public HQAttachmentFolder BuildFolderTree(long? folderId = 0, string folderPath = "")
        {
            var folder = Folders.FirstOrDefault(f => f.FolderId == folderId);
            if (folder == null)
            {
                BuildDefaultFolders();
                folder = GetRootFolder();
            }
            if (!string.IsNullOrEmpty(folderPath))
            {
                var folderTree = folderPath.Split('/');
                foreach (var treeFolder in folderTree)
                {
                    if (!string.IsNullOrEmpty(treeFolder))
                    {
                        var subFolder = folder.AddFolder(treeFolder);

                        folder = subFolder;
                    }
                }
            }

            return folder;
        }

        public void SyncFromSource()
        {
            var root = GetRootFolder();
            root.SyncFromSource();
        }
    }

    public interface IAttachment
    {
        public HQAttachment Attachment { get; }

        public string GetSharePointRootFolderPath();

        public SPList GetSharePointList();

    }

    //public string GetFullUncPath(string value)
    //{
    //    var path = string.Format("{0}{1}", db.AttachmentShare, value);
    //    return path;
    //}

    //public void SyncFromUncPath()
    //{
    //    if (string.IsNullOrEmpty(tUncPath))
    //        return;
    //    var root = GetRootFolder();

    //    //Folders.ToList().ForEach(e => e.IsInShare = false);
    //    //Files.ToList().ForEach(e => e.IsInShare = false);

    //    //AddUncFolderAndFiles(root);
    //    //root.MoveToShare();

    //    //var folders = this.Folders.ToList();
    //    //var files = this.Files.ToList();

    //    //var removeFolders = Folders.Where(f => f.IsInShare == false && f.IsPhysicalPath == true && f.FolderId != root.FolderId).ToList();
    //    //var removeFiles = Files.Where(f => f.IsInShare == false && f.IsDBAttachment == false).ToList();
    //    //removeFiles.ForEach(e => e.Delete());
    //    //removeFolders.ForEach(e => e.Delete());
    //}

    //private void AddUncFolderAndFiles(HQAttachmentFolder folder)
    //{
    //    //var folderUncPath = UncPath;
    //    //if (folder.ParentId != null)
    //    //    folderUncPath = string.Format(@"{0}{1}", UncPath, folder.FolderPath);

    //    //var folderList = new List<string>();
    //    //var fileList = new List<string>();
    //    ////if (Directory.Exists(folderUncPath))
    //    ////{
    //    ////    folderList = Directory.GetDirectories(folderUncPath).ToList();
    //    ////    fileList = Directory.GetFiles(folderUncPath).ToList();
    //    ////}
    //    ////using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
    //    //{
    //    //    if (Directory.Exists(folderUncPath))
    //    //    {
    //    //        folderList = Directory.GetDirectories(folderUncPath).ToList();
    //    //        fileList = Directory.GetFiles(folderUncPath).ToList();
    //    //    }
    //    //};

    //    //foreach (var subFolderPath in folderList)
    //    //{
    //    //    var subFolder = folder.AddFolder(subFolderPath);
    //    //    AddUncFolderAndFiles(subFolder);
    //    //}

    //    //foreach (var fileName in fileList)
    //    //{
    //    //    folder.AddFile(fileName);
    //    //}
    //}
}
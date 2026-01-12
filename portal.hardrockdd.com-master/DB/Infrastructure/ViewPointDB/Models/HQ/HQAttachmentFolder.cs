using DB.Infrastructure.Extensions;
using DB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQAttachmentFolder
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
                _db ??= Attachment.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        [DefaultValue(false)]
        public bool IsInShare { get; set; }

        public HQAttachmentStorageEnum StorageLocation
        {
            get => (HQAttachmentStorageEnum)(tStorageTypeId ?? 0);
            set => UpdateStorageLocation(value);
        }

        public void UpdateStorageLocation(HQAttachmentStorageEnum value)
        {
            if (value != StorageLocation)
            {
                switch (StorageLocation)
                {
                    case HQAttachmentStorageEnum.DB:
                        MoveFromDB(value);
                        break;
                    case HQAttachmentStorageEnum.FileSystem:
                        MoveFromFileSystem(value);
                        break;
                    case HQAttachmentStorageEnum.SharePoint:
                        MoveFromSharePoint(value);
                        break;
                    case HQAttachmentStorageEnum.DBAndSharePoint:
                        break;
                    default:
                        break;
                }
                tStorageTypeId = (int)value;
            }
        }

        public long? ParentId { get => tParentId; set => UpdateParentFolder(value); }

        public string Description
        {
            get
            {
                return tDescription;
            }
            set
            {
                if (tDescription != value)
                {
                    var origFolderPath = FolderPath;
                    tDescription = value;

                    switch (StorageLocation)
                    {
                        case HQAttachmentStorageEnum.DB:
                            break;
                        case HQAttachmentStorageEnum.FileSystem:
                            MoveFolderFileSystem();
                            break;
                        case HQAttachmentStorageEnum.SharePoint:
                            MoveFolderSharePoint();
                            break;
                        case HQAttachmentStorageEnum.DBAndSharePoint:
                            MoveFolderSharePoint();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #region Move Storage Location
        private void MoveFromDB(HQAttachmentStorageEnum value)
        {
            switch (value)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    this.CreateFolderFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    _ = CreateFolderSharePoint();
                    break;
                default:
                    break;
            }
            Files.ToList().ForEach(e => e.StorageLocation = value);
            SubFolders.ToList().ForEach(e => e.StorageLocation = value);
        }

        private void MoveFromFileSystem(HQAttachmentStorageEnum value)
        {
            switch (value)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    _ = CreateFolderSharePoint();
                    break;
                default:
                    break;
            }
            Files.ToList().ForEach(e => e.StorageLocation = value);
            SubFolders.ToList().ForEach(e => e.StorageLocation = value);
        }

        private void MoveFromSharePoint(HQAttachmentStorageEnum value)
        {
            switch (value)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    CreateFolderFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    break;
                default:
                    break;
            }
            Files.ToList().ForEach(e => e.StorageLocation = value);
            SubFolders.ToList().ForEach(e => e.StorageLocation = value);
        }
        #endregion

        #region Update Folder Location
        public void UpdateParentFolder(long? value)
        {
            if (tParentId != value)
            {
                switch (StorageLocation)
                {
                    case HQAttachmentStorageEnum.DB:
                        MoveFolderDB(value);
                        break;
                    case HQAttachmentStorageEnum.FileSystem:
                        MoveFolderDB(value);
                        MoveFolderFileSystem();
                        break;
                    case HQAttachmentStorageEnum.SharePoint:
                        MoveFolderDB(value);
                        MoveFolderSharePoint();
                        break;
                    case HQAttachmentStorageEnum.DBAndSharePoint:
                        MoveFolderDB(value);
                        MoveFolderSharePoint();
                        break;
                    default:
                        break;
                }
            }
        }


        private void MoveFolderDB(long? value)
        {
            var folder = this.Attachment.Folders.FirstOrDefault(f => f.FolderId == value);
            if (folder != null)
            {
                this.Parent = folder;
                this.tParentId = folder.FolderId;
            }
        }

        private void MoveFolderFileSystem()
        {
            var newPath = this.GetFolderPath();

            var uncOldPath = string.Format(@"{0}{1}", this.Attachment.UncPath, this.FolderPath);
            var uncNewPath = string.Format(@"{0}{1}", this.Attachment.UncPath, newPath);

            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                System.IO.File.Move(uncOldPath, uncNewPath);
            };

            this.FolderPath = newPath;

        }

        private void MoveFolderSharePoint()
        {

            var folderPath = this.Parent.SharePointFolderPath + "\\" + Description;
            if(folderPath != SharePointFolderPath)
            {
                folderPath = this.Attachment.SharePointList.Name + "/" + folderPath;
                var folder =  GetFolderSharePoint();

                //var url = string.Concat(Attachment.SharePointList.Site.Url, folderPath);
                
                folderPath = folderPath.Replace("\\", "/");
                folder.MoveFilesTo(folderPath);
                folder.DeleteObject();
                folder.Context.ExecuteQuery();

                FolderPath = folderPath;
            }
        }
        #endregion

        #region Add Folder
        public HQAttachmentFolder AddFolder(string folderName)
        {
            folderName = System.IO.Path.GetFileName(folderName);
            var folder = SubFolders.FirstOrDefault(f => f.tDescription == folderName);
            if (folder == null)
            {
                //folder = this.Attachment.AddFolder(folderName, this);

                folder = new HQAttachmentFolder()
                {
                    Attachment = this.Attachment,
                    db = this.db,
                    Parent = this,

                    HQCo = HQCo,
                    UniqueAttchID = UniqueAttchID,
                    FolderId = this.Attachment.Folders.DefaultIfEmpty().Max(max => max == null ? -1 : max.FolderId) + 1,
                    tDescription = folderName,
                    tParentId = this.FolderId,
                    IsPhysicalPath = false,

                };
                this.SubFolders.Add(folder);
                this.Attachment.Folders.Add(folder);
                //folder.SubFolders.Add(folder);
            }

            switch (folder.Parent.StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    folder.CreateFolderFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    folder.CreateFolderSharePoint();
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    folder.CreateFolderSharePoint();
                    break;
                default:
                    break;
            }
            //if (this.Attachment.SharePointList != null)
            //{
            //    folder.StorageLocation = HQAttachmentStorageEnum.SharePoint;
            //    folder.SharePointFolderPath = String.Concat(this.SharePointFolderPath, @"\", folderName);
            //    folder.SharePointListId = this.Attachment.SPListId;
            //    _ = folder.CreateFolderSharePoint();
            //}

            //if (!string.IsNullOrEmpty(this.Attachment.tUncPath))
            //{
            //    folder.StorageLocation = HQAttachmentStorageEnum.FileSystem;
            //    folder.FolderPath = folderPath;
            //    folder.IsPhysicalPath = true;
            //    folder.IsInShare = true;
                
            //}

            return folder;
        }

        private HQAttachmentFolder AddFolder(Microsoft.SharePoint.Client.Folder folder)
        {
            var folderName = System.IO.Path.GetFileName(folder.Name);
            var subFolder = SubFolders.FirstOrDefault(f => f.tDescription == folderName);
            if (subFolder == null)
            {
                //folder = this.Attachment.AddFolder(folderName, this);

                subFolder = new HQAttachmentFolder()
                {
                    Attachment = this.Attachment,
                    db = this.db,
                    Parent = this,

                    HQCo = HQCo,
                    UniqueAttchID = UniqueAttchID,
                    FolderId = this.Attachment.Folders.DefaultIfEmpty().Max(max => max == null ? -1 : max.FolderId) + 1,
                    tDescription = folderName,
                    tParentId = this.FolderId,
                    IsPhysicalPath = false,
                    tStorageTypeId = (int)HQAttachmentStorageEnum.SharePoint,
                    SharePointFolderPath = String.Concat(this.SharePointFolderPath, @"\", folderName),
                    SharePointListId = this.Attachment.SPListId,
                    FolderPath = GetFolderPath(),
                    _SharePointFolder = folder
                };
                this.SubFolders.Add(subFolder);
                this.Attachment.Folders.Add(subFolder);
                //folder.SubFolders.Add(folder);
            }
            else
            {
                if (subFolder.SharePointFolderPath != string.Concat(this.SharePointFolderPath, @"\", folderName))
                    subFolder.SharePointFolderPath = String.Concat(this.SharePointFolderPath, @"\", folderName);
            }

            //if (this.Attachment.SharePointList != null)
            //{
            //    subFolder.StorageLocation = HQAttachmentStorageEnum.SharePoint;
            //    subFolder.SharePointFolderPath = String.Concat(this.SharePointFolderPath, @"\", folderName);
            //    subFolder.SharePointListId = this.Attachment.SPListId;
            //    _ = this.AddSharePointFolder();
            //}

            return subFolder;
        }
        #endregion

        #region Add File
        public HQAttachmentFile AddFile()
        {
            var keyField = string.Format("KeyID={0}", this.Attachment.TableKeyId);


            var emp = db.GetCurrentEmployee();
            var usrProf = db.UserProfiles.FirstOrDefault(f => f.Employee == emp.EmployeeId);
            var attachmentId = db.GetNextAttachmentId();
            var newObj = new HQAttachmentFile()
            {
                Attachment = this.Attachment,
                db = this.db,
                Folder = this,

                FolderId = this.FolderId,
                HQCo = this.HQCo,
                AttachmentId = attachmentId,
                FormName = this.Attachment.TableName,
                KeyField = keyField,
                AddedBy = usrProf == null ? emp.Email : usrProf.VPUserName,
                AddDate = DateTime.Now,
                DocName = "Database",
                TableName = this.Attachment.TableName,
                UniqueAttchID = this.Attachment.UniqueAttchID,
                DocAttchYN = "Y",
                CurrentState = "A",
                IsEmail = "N",
                IsDBAttachment = true,
                tStorageTypeId = this.tStorageTypeId,
                SharePointFolderPath = this.SharePointFolderPath,
                SharePointListId = this.SharePointListId,
                SharePointSiteId = this.SharePointSiteId,
                //SharePointSiteId = this.Attachment.SharePointList?.Site.SiteId,
                //SharePointListId = this.Attachment.SharePointList?.ListId,
            };

            return newObj;
        }

        public HQAttachmentFile AddFile(string fileName)
        {
            var badChar = new List<string>() { "\"", ",", "*", "#", ":", "<", ">", "?", "\\", "|" };
            badChar.ForEach(c => { fileName = fileName.Replace(c, ""); });
            fileName = System.IO.Path.GetFileName(fileName);

            var file = this.Files.FirstOrDefault(f => f.OrigFileName == fileName);
            if (file == null)
            {
                file = AddFile();
                file.Description = fileName;
                file.OrigFileName = fileName;
                Files.Add(file);
            }
            file.IsInShare = true;
            return file;
        }

        public HQAttachmentFile AddFile(string fileName, byte[] data)
        {
            var badChar = new List<string>() { "\"", ",", "*", "#", ":", "<", ">", "?", "\\", "|" };
            badChar.ForEach(c => { fileName = fileName.Replace(c, ""); });

            var file = this.Files.FirstOrDefault(f => f.OrigFileName == fileName);
            if (fileName.Contains("image."))
                file = null;
            if (file == null)
            {
                file = AddFile();
                file.Description = fileName;
                if (file.Description.Contains("image."))
                {
                    file.Description = string.Format("{0}-{1}", file.AttachmentId, file.Description);
                }
                file.OrigFileName = file.Description;
                Files.Add(file);
            }
            file.SaveData(data);
            return file;
        }

        public HQAttachmentFile AddFile(HttpPostedFileBase uploadFile)
        {
            var fileName = uploadFile.FileName;
            var badChar = new List<string>() { "\"", ",", "*", "#", ":", "<", ">", "?", "\\", "|" };
            badChar.ForEach(c => { fileName = fileName.Replace(c, ""); });

            var file = this.Files.FirstOrDefault(f => f.Description == fileName);
            if (fileName.Contains("image."))
                file = null;

            if (file == null)
            {
                file = AddFile();
                file.AttachmentTypeID = this.AttachmentTypeId;
                file.Description = fileName;
                if (file.Description.Contains("image."))
                {
                    file.Description = string.Format("{0}-{1}", file.AttachmentId, file.Description);
                }
                file.OrigFileName = file.Description;

                if (!string.IsNullOrEmpty(this.Attachment.tUncPath))
                    file.IsDBAttachment = false;

                Files.Add(file);
                this.Files.Add(file);
            }
            else
            {
                if (file.FolderId != this.FolderId)
                {
                    file.FolderId = this.FolderId;
                }
            }

            if (file.OrigFileName.Contains("_THUMBNAIL"))
            {
                var originalFileName = file.OrigFileName.Replace("_THUMBNAIL", "");
                originalFileName = System.IO.Path.GetFileNameWithoutExtension(originalFileName);
                var origFile = this.Files.FirstOrDefault(f => f.OrigFileName.StartsWith(originalFileName) && f.OrigFileName != file.OrigFileName);
                if (origFile != null)
                    origFile.ThumbnailAttachmentID = file.AttachmentId;
                file.IsDBAttachment = true;
                file.tStorageTypeId = (int)HQAttachmentStorageEnum.DB;
            }

            file.SaveData(uploadFile);
            return file;
        }

        public HQAttachmentFile CopyFile(HQAttachmentFile originalFile, bool createDuplicate = true)
        {
            if (originalFile == null)
                return null;

            if (createDuplicate)
            {
                var file = AddFile();
                file.Description = originalFile.Description;
                file.OrigFileName = file.Description;

                file.SaveData(originalFile.GetData());
                Files.Add(file);
                return file;
            }
            else
            {
                originalFile.UniqueAttchID = this.UniqueAttchID;
                return originalFile;
            }
        }
        #endregion

        #region Sync
        public void SyncFromSource()
        {
            switch (StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    SyncFromFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    SyncFromSharePoint();
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    SyncFromSharePoint();
                    break;
                default:
                    break;
            }
        }

        public void SyncFromSharePoint()
        {
            this.SubFolders.ToList().ForEach(e => e.IsInShare = false);
            this.Files.ToList().ForEach(e => e.IsInShare = false);

            var folder = GetFolderSharePoint();
            if (folder != null)
            {
                try
                {
                    var context = folder.Context;

                    if (!folder.IsPropertyAvailable("Folders") ||
                        !folder.IsPropertyAvailable("Files"))
                    {
                        if (!folder.IsPropertyAvailable("Folders"))
                            context.Load(folder, f => f.Folders);
                        if (!folder.IsPropertyAvailable("Files"))
                            context.Load(folder, f => f.Files);
                        context.ExecuteQuery();
                    }

                    var subFolders = folder.Folders.ToList();
                    var subFiles = folder.Files.ToList();
                    foreach (var subSPFolder in subFolders)
                    {
                        var subFolder = AddFolder(subSPFolder);
                        subFolder.IsInShare = true;
                        subFolder.SyncFromSharePoint();
                    }

                    foreach (var spFile in subFiles)
                    {
                        var file = this.AddFile(spFile.Name);
                        file.IsInShare = true;
                    }
                }
                catch (Exception)
                {

                    return;
                }
            }

            var folderDelList = this.SubFolders.Where(f => f.IsInShare == false).ToList();
            var fileDelList = this.Files.Where(f => !f.Files.Any() && f.IsInShare == false).ToList();

            folderDelList.ForEach(e => e.Delete());
            fileDelList.ForEach(e => e.Delete());
        }

        public void SyncFromFileSystem()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Delete Storage
        public void Delete()
        {
            this.SubFolders.ToList().ForEach(s => s.Delete());
            this.Files.ToList().ForEach(s => s.Delete());

            switch (StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    DeleteFolderFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    DeleteFolderSharePoint();
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    DeleteFolderSharePoint();
                    break;
                default:
                    break;
            }

            this.SubFolders.Remove(this);
            db.HQAttachmentFolders.Remove(this);
        }

        private void DeleteFolderSharePoint()
        {
            var folder = GetFolderSharePoint();
            if (folder != null)
            {
                folder.DeleteObject();
                folder.Context.ExecuteQuery();
            }
        }

        private void DeleteFolderFileSystem()
        {
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                var uncPath = string.Format(@"{0}{1}", this.Attachment.UncPath, this.FolderPath);
                System.IO.Directory.Delete(uncPath, true);
            }
        }
        #endregion

        public void DownloadFolder(ZipArchive zip)
        {
            foreach (var file in Files)
            {
                zip.AddDBAttachment(file);
            }
            foreach (var folder in SubFolders)
            {
                zip.AddDBFolder(folder);
            }
        }
        
        public string GetFolderPath()
        {
            var parent = this.Parent;
            var folders = new List<string>();

            while (parent != null)
            {
                folders.Add(Description);
                parent = parent.Parent;
            }
            folders.Reverse();

            return string.Join(@"\", folders);

        }

        public string GetUncFolderPath()
        {
            if (string.IsNullOrEmpty(this.Attachment.tUncPath))
                return null;


            var parent = this.Parent;
            var folders = new List<string>();

            while (parent != null)
            {
                folders.Add(Description);
                parent = parent.Parent;
            }
            folders.Reverse();
            var path = string.Join(@"\", folders);

            return string.Format(@"{0}{1}", this.Attachment.UncPath, path);
        }

        public string SharePointRelativeUrl()
        {
            var list = this.Attachment.SharePointList;
            var site = list.Site;
            var spList = list.GetSharePointList();
            var ctx = (Microsoft.SharePoint.Client.ClientContext)spList.Context;
            if (!spList.IsPropertyAvailable("RootFolder"))
                ctx.Load(spList, w => w.RootFolder);
            if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
            ctx.ExecuteQuery();

            var webUrl = ctx.Web.ServerRelativeUrl;

            var listUrl = spList.RootFolder.ServerRelativeUrl;
            listUrl = listUrl.Replace(webUrl, string.Empty);


            var url = string.Concat(site.Url, listUrl, "/", SharePointFolderPath);

            //ctx.Web.EnsureFolder(folderUrl.Replace(ctx.Web.ServerRelativeUrl, string.Empty));
            url = url.Replace("\\", "/");
            return url;
        }

        private Microsoft.SharePoint.Client.ClientContext _SharePointContext;
        public Microsoft.SharePoint.Client.ClientContext SharePointContext 
        { 
            get
            {
                if (_SharePointContext == null)
                {
                    var parentFolder = Parent ?? this.Attachment.GetRootFolder();
                    if (this.FolderId != parentFolder.FolderId)
                        _SharePointContext ??= parentFolder?.SharePointContext;
                }
                _SharePointContext ??= this.Attachment.SharePointList.Site.Context;
                              
                return _SharePointContext;
            }
            set
            {
                _SharePointContext = value;
            }
        }


        private Microsoft.SharePoint.Client.Folder _SharePointFolder;        
        public Microsoft.SharePoint.Client.Folder GetFolderSharePoint()
        {
            if (_SharePointFolder != null)
            {
                if (_SharePointFolder?.Name != this.Description && this.Description != "root")
                {
                    _SharePointFolder = null;
                }
            }
            if (_SharePointFolder == null)
            {
                var context = this.SharePointContext;

                var url = new Uri(SharePointRelativeUrl());
                var folder = context.Web.GetFolderByServerRelativeUrl(url.LocalPath);
                context.Load(folder);
                try
                {
                    context.Load(folder, f => f.Folders, f => f.Files);
                    context.ExecuteQuery();
                    _SharePointFolder = folder;

                }
                catch (Exception)
                {
                    return null;
                }
            }
            return _SharePointFolder;
        }
        
        public Microsoft.SharePoint.Client.Folder CreateFolderSharePoint()
        {
            var parentFolder = this.Parent;
            if (parentFolder == null)
                SharePointFolderPath ??= this.Attachment.GetRootFolder().SharePointFolderPath;
            else
                SharePointFolderPath = parentFolder.SharePointFolderPath + @"/" + Description;

            if (parentFolder == null)
                parentFolder = this.Attachment.GetRootFolder();

            tStorageTypeId = (int)parentFolder.StorageLocation;
            SharePointListId = this.Attachment.SPListId;

            var folder = GetFolderSharePoint();
            
            if (folder == null)
            {
                var list = this.Attachment.SharePointList;
                var spList = list.GetSharePointList();
                
                var site = this.Attachment.SharePointList.Site;
                var ctx = site.Context;
                if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
                    ctx.Load(ctx.Web, w => w.ServerRelativeUrl);

                ctx.ExecuteQuery();
                var folderPath = SharePointFolderPath.Replace("\\", "/");
                var folderUrl = string.Concat(spList.RootFolder.ServerRelativeUrl, "/", folderPath);
                ctx.Web.EnsureFolder(folderUrl.Replace(ctx.Web.ServerRelativeUrl, string.Empty));

                folder = GetFolderSharePoint();
            }


            return folder;
        }

        public void CreateFolderFileSystem()
        {
            this.FolderPath = GetFolderPath();
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                var uncPath = string.Format(@"{0}{1}", this.Attachment.UncPath, this.FolderPath);
                System.IO.Directory.CreateDirectory(uncPath);

                //var folder = System.IO.Directory.GEtcu(uncPath);
            }
            tStorageTypeId = (int)HQAttachmentStorageEnum.FileSystem;
            //this.StorageLocation = HQAttachmentStorageEnum.FileSystem;
        }
    }
}
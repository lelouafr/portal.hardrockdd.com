using DB.Infrastructure.Services;
using DB.Infrastructure.VPAttachmentDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{

    public partial class HQAttachmentFile
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
                _db ??= Attachment?.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        
        public HQAttachmentStorageEnum StorageLocation 
        {
            get => (HQAttachmentStorageEnum)(tStorageTypeId ?? 0);            
            set => UpdateStorageLocation(value);
        }

        public void UpdateStorageLocation(HQAttachmentStorageEnum value)
        {
            if (value != StorageLocation)
            {
                var data = GetData();
                
                if (data != null)
                {
                    if (value != HQAttachmentStorageEnum.DBAndSharePoint)
                    {
                        DeleteData();
                    }
                    tStorageTypeId = (int)value;
                    SaveData(data);
                }
                tStorageTypeId = (int)value;
            }
        }


        public long? FolderId 
        { 
            get => tFolderId; 
            set => UpdateFolder(value); 
        }

        public void UpdateFolder(long? value)
        {
            if (tFolderId != value)
            {                
                
                switch (StorageLocation)
                {
                    case HQAttachmentStorageEnum.DB:
                        UpdateFolderDB(value);
                        break;
                    case HQAttachmentStorageEnum.FileSystem:
                        break;
                    case HQAttachmentStorageEnum.SharePoint:
                        UpdateFolderSharePoint(value);
                        break;
                    case HQAttachmentStorageEnum.DBAndSharePoint:
                        UpdateFolderSharePoint(value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateFolderDB(long? value)
        {
            if (tFolderId != value)
            {
                var folder = this.Attachment.Folders.FirstOrDefault(f => f.FolderId == value);

                if (folder != null)
                {
                    this.Folder = folder;
                    this.tFolderId = folder.FolderId;

                    if (folder.AttachmentTypeId != null)
                        this.AttachmentTypeID = folder.AttachmentTypeId;
                }
            }
        }
        private void UpdateFolderSharePoint(long? value)
        {
            var newFolder = this.Attachment.Folders.FirstOrDefault(f => f.FolderId == value);
            var oldFolder = this.Folder;
            if (newFolder != null)
            {
                var folderPath = newFolder.SharePointFolderPath;

                if (this.SharePointFolderPath != folderPath)
                {
                    var folder = oldFolder.GetFolderSharePoint();
                    var file = GetSharePointFile();
                    
                    var folderUrl = this.Attachment.SharePointList.Name + "\\" + folderPath;
                    var targetFileUrl = file.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                    
                    file.MoveTo(targetFileUrl, Microsoft.SharePoint.Client.MoveOperations.Overwrite);
                    file.Context.ExecuteQuery();

                    this.SharePointFolderPath = folderPath;

                    var list = this.Folder.Attachment.SharePointList;
                    var site = list.Site;
                    this.SharePointSiteId = site.SiteId;
                    this.SharePointListId = list.ListId;
                }

                this.Folder = newFolder;
                this.tFolderId = newFolder.FolderId;

                if (newFolder.AttachmentTypeId != null)
                    this.AttachmentTypeID = newFolder.AttachmentTypeId;
            }
        }

        private string _FolderPath;
        public string FolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_FolderPath))
                {
                    _FolderPath = "";
                    BuildFolderList(this.Folder);
                    ParentFolders.Reverse();
                    foreach (var folder in ParentFolders)
                    {
                        if (folder.tDescription != "root")
                        {
                            if (!string.IsNullOrEmpty(_FolderPath))
                                _FolderPath += "/";

                            _FolderPath += folder.tDescription;
                        }
                    }
                }
                return _FolderPath;
            }
        }

        public bool IsInShare { get; set; }
                
        #region Get Data
        public MemoryStream GetMemoryStream()
        {
            var ms =  new MemoryStream(GetData());
            return ms;
        }

        public byte[] GetData()
        {
            return StorageLocation switch
            {
                HQAttachmentStorageEnum.DB => GetDataFromDb(),
                HQAttachmentStorageEnum.FileSystem => GetDataFromFileSystem(),
                HQAttachmentStorageEnum.SharePoint => GetDataFromSharePoint(),
                HQAttachmentStorageEnum.DBAndSharePoint => GetDataFromSharePoint() ?? GetDataFromDb(),
                _ => GetDataFromDb(),
            };
        }

        private byte[] GetDataFromDb()
        {
            using var dbAttch = new VPAttachmentsContext();
            dbAttch.Database.CommandTimeout = 600;
            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);
            if (attachment != null)
            {
                return attachment.AttachmentData;
            }
            return null;
        }

        private byte[] GetDataFromFileSystem()
        {
            var filePath = string.Format(@"{0}{1}\{2}", this.Attachment.UncPath, Folder.FolderPath, this.OrigFileName);
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
        }

        private byte[] GetDataFromSharePoint()
        {
            var list = this.Attachment.SharePointList;
            if (list == null)
                return null;
            var site = list.Site;
            var tenate = site.Tenate;

            var context = Folder.SharePointContext;// new SharePointClient(site.Url, tenate.UserName, tenate.Password).Context;
            var url = new Uri(SharePointRelativeUrl());
            //var tmpFile = GetSharePointFile();

            //var file = context.Web.GetFileByServerRelativeUrl(url.LocalPath);
            try
            {
                //context.Load(file, file => file.ServerRelativeUrl);
                //context.ExecuteQuery();
                var ms = new System.IO.MemoryStream();
                var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, url.LocalPath);
                fileInfo.Stream.CopyTo(ms);
                var data = ms.ToArray();
                ms.Dispose();
                fileInfo.Dispose();
                return data;
            }
            catch (Exception)
            {

                return null;
            }

        }
        #endregion

        #region Save Data
        public void SaveData(byte[] data)
        {
            switch (StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    SaveToDB(data);
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    SaveToFileSystem(data);
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    SaveToSharePoint(data);
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    SaveToDB(data);
                    SaveToSharePoint(data);
                    break;
                default:
                    break;
            }
        }

        public void SaveData(HttpPostedFileBase fileUpload)
        {
            switch (StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    SaveToDB(fileUpload);
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    SaveToFileSystem(fileUpload);
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    SaveToSharePoint(fileUpload);
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    SaveToDB(fileUpload);
                    var dbData = GetDataFromDb();
                    SaveToSharePoint(dbData);
                    break;
                default:
                    break;
            }
        }

        private void SaveToDB(HttpPostedFileBase fileUpload)
        {

            using var binaryReader = new BinaryReader(fileUpload.InputStream);
            var data = binaryReader.ReadBytes(fileUpload.ContentLength);
            SaveToDB(data);
            binaryReader.Dispose();
        }

        private void SaveToDB(byte[] data)
        {
            if (data?.Length == 0 || data == null)
                return;

            using var dbAttch = new VPAttachmentsContext();
            dbAttch.Database.CommandTimeout = 600;
            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);

            if (attachment == null)
            {
                attachment = new Attachment
                {
                    AttachmentFileType = System.IO.Path.GetExtension(this.OrigFileName),
                    SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks),
                    AttachmentID = this.AttachmentId,
                    AttachmentData = data
                };
                dbAttch.Attachments.Add(attachment);
            }
            else
            {
                var a1 = (ReadOnlySpan<byte>)data;
                var a2 = (ReadOnlySpan<byte>)attachment.AttachmentData;

                if (!a1.SequenceEqual(a2))
                {
                    attachment.AttachmentFileType = System.IO.Path.GetExtension(this.OrigFileName);
                    attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                    attachment.AttachmentID = this.AttachmentId;
                    attachment.AttachmentData = data;
                }
            }
            this.IsDBAttachment = true;
            dbAttch.BulkSaveChanges();
        }

        private void SaveToFileSystem(HttpPostedFileBase fileUpload)
        {
            var filePath = string.Format(@"{0}{1}\{2}", this.Attachment.UncPath, Folder.FolderPath, this.OrigFileName);
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                fileUpload.SaveAs(filePath);
            }
        }

        private void SaveToFileSystem(byte[] data)
        {
            var filePath = string.Format(@"{0}{1}\{2}", this.Attachment.UncPath, Folder.FolderPath, this.OrigFileName);
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                using var stream = File.Create(filePath);
                stream.Write(data, 0, data.Length);
            }
        }

        private void SaveToSharePoint(HttpPostedFileBase fileUpload)
        {
            using var binaryReader = new BinaryReader(fileUpload.InputStream);
            var data = binaryReader.ReadBytes(fileUpload.ContentLength);
            SaveToSharePoint(data);
            binaryReader.Dispose();
        }

        private void SaveToSharePoint(byte[] data)
        {
            var folder = Folder.GetFolderSharePoint();
            if (folder == null)
                folder = Folder.CreateFolderSharePoint();

            if (folder != null)
            {
                var sharePointData = GetDataFromSharePoint() ;

                var a1 = (ReadOnlySpan<byte>)data;
                var a2 = (ReadOnlySpan<byte>)sharePointData;

                if (!a1.SequenceEqual(a2))
                {
                    var context = folder.Context;
                    context.Load(folder, folder => folder.Files);
                    context.ExecuteQuery();

                    var ms = new MemoryStream(data);
                    var newItem = new Microsoft.SharePoint.Client.FileCreationInformation
                    {
                        ContentStream = ms,
                        Overwrite = true,
                        Url = System.IO.Path.GetFileName(this.OrigFileName)
                    };
                    var newFile = folder.Files.Add(newItem);
                    context.Load(newFile);
                    context.ExecuteQuery();

                    ms.Dispose();
                }
                    
            }
        }
        #endregion

        #region Delete Data
        public void DeleteData(bool preserveTumbnail = false)
        {
            switch (StorageLocation)
            {
                case HQAttachmentStorageEnum.DB:
                    DeleteDataFromDB(preserveTumbnail);
                    break;
                case HQAttachmentStorageEnum.FileSystem:
                    DeleteDataFromFileSystem();
                    break;
                case HQAttachmentStorageEnum.SharePoint:
                    DeleteDataFromSharePoint();
                    break;
                case HQAttachmentStorageEnum.DBAndSharePoint:
                    DeleteDataFromDB(preserveTumbnail);
                    DeleteDataFromSharePoint();
                    break;
                default:
                    break;
            }
        }

        private void DeleteDataFromDB(bool preserveTumbnail = true)
        {
            using var dbAttch = new VPAttachmentsContext();
            dbAttch.Database.CommandTimeout = 600;
            if (!preserveTumbnail)
            {
                if (this.Thumbnail != null)
                {
                    this.Thumbnail.Files.ToList().ForEach(e => e.ThumbnailAttachmentID = null);
                    this.Thumbnail.DeleteData();
                    Attachment.Files.Remove(this.Thumbnail);
                }
            }

            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);
            if (attachment != null)
                dbAttch.Attachments.Remove(attachment);
            dbAttch.BulkSaveChanges();
        }

        private void DeleteDataFromFileSystem()
        {
            //using (new Steeltoe.Common.Net.WindowsNetworkFileShare(db.AttachmentShare, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass, db.AttachmentUserDomain)))
            {
                var uncPath = string.Format(@"{0}{1}\{2}", this.Attachment.UncPath, Folder.FolderPath, this.OrigFileName);
                try
                {
                    File.Delete(uncPath);
                }
                catch (Exception)
                {

                }
            };

        }

        private void DeleteDataFromSharePoint()
        {
            var file = GetSharePointFile();
            if (file != null)
            {
                file.DeleteObject();
                file.Context.ExecuteQuery();
            }
        }
        #endregion

        private List<HQAttachmentFolder> ParentFolders { get; set; }

        private void BuildFolderList(HQAttachmentFolder folder)
        {
            if (ParentFolders == null)
                ParentFolders = new List<HQAttachmentFolder>();

            if (folder != null)
            {
                ParentFolders.Add(folder);
                if (folder.Parent != null)
                {
                    BuildFolderList(folder.Parent);
                }

            }
        }

        public static int GetNextAttachmentId()
        {
            using var db = new VPContext();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("attachmentId", typeof(int));
            var result = db.udNextAttachmentId(outParm);

            return (int)outParm.Value;
        }

        public void Delete()
        {
            this.DeleteData();
            if (this.Folder != null)
                this.Folder.Files.Remove(this);
            db.HQAttachmentFiles.Remove(this);
        }

        private Microsoft.SharePoint.Client.File GetSharePointFile()
        {
            var folder = Folder.GetFolderSharePoint();
            if (folder != null)
            {

                var context = folder.Context;
                context.Load(folder, f => f.Files);
                context.ExecuteQuery();

                var file = folder.Files.FirstOrDefault(f => f.Name == this.OrigFileName);
                if (file != null)
                {
                    return file;
                }
            }
            return null;
        }
        public string SharePointRelativeUrl()
        {
            var list = this.Attachment.SharePointList;
            var site = list.Site;

            var folderUrl = Folder.SharePointRelativeUrl();
            var url = string.Concat(folderUrl, "/", this.OrigFileName);
            return url;
        }

        private Microsoft.SharePoint.Client.ClientContext _SharePointContext;
        public Microsoft.SharePoint.Client.ClientContext SharePointContext
        {
            get
            {
                _SharePointContext ??= Folder?.SharePointContext;
                _SharePointContext ??= this.Attachment.SharePointList.Site.Context;

                return _SharePointContext;
            }
            set
            {
                _SharePointContext = value;
            }
        }
    }
}

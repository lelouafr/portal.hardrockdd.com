using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class HQAttachment
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = HQCompanyParm.db;

                }
                return _db;
            }
        }
        public HQAttachmentFolder GetRootFolder()
        {
            var root = Folders.FirstOrDefault(f => f.Description == "root");
            if (root == null)
            {
                var db = VPEntities.GetDbContextFromEntity(this);
                
                this.BuildDefaultFolders();
                //db.BulkSaveChanges();
                root = Folders.FirstOrDefault(f => f.Description == "root");
            }

            return root;
        }

        public void AssignFilesToRoot()
        {
            if (Files.Any(f => f.FolderId == null))
            {
                var db = VPEntities.GetDbContextFromEntity(this);
                var root = GetRootFolder();
                this.Files.ToList().ForEach(e => {
                    e.FolderId ??= root.FolderId;
                    e.Folder = root;
                    root.Files.Add(e);
                });
                db.BulkSaveChanges();
            }
        }

        public void BuildDefaultFolders()
        {
            using var db = new VPEntities();
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == this.TableName).ToList();

            var folder = this.Folders.FirstOrDefault(f => f.Description == "root");
            if (folder == null)
            {
                folder = HQAttachmentFolder.Init(this);
                folder.Description = "root";
                folder.ParentId = null;
                this.Folders.Add(folder);
            }

            foreach (var attachmentType in attachmentTypes)
            {
                folder = this.Folders.FirstOrDefault(f => f.AttachmentTypeId == attachmentType.AttachmentTypeID);
                if (folder == null)
                {
                    folder = this.Folders.FirstOrDefault(f => f.Description == attachmentType.Description);
                    if (folder == null)
                    {

                        folder = HQAttachmentFolder.Init(this);
                        folder.Description = attachmentType.Description;
                        folder.AttachmentTypeId = attachmentType.AttachmentTypeID;
                        this.Folders.Add(folder);
                    }
                    else
                    {
                        folder.AttachmentTypeId = attachmentType.AttachmentTypeID;
                    }
                }
            }
        }

        public static HQAttachment FindCreate(byte hqco, Guid? uniqueAttchID, string tableName, long keyID, VPEntities db)
        {
            if (uniqueAttchID == null)
            {
                uniqueAttchID = Guid.NewGuid();
                db.Database.ExecuteSqlCommand(string.Format(AppCultureInfo.CInfo(), "UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyID));
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
            //using var db = new VPEntities();

            //db.Database.ExecuteSqlCommand(string.Format(AppCultureInfo.CInfo(), "UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyID));

            var attachment = new HQAttachment() { 
                HQCo = co,
                UniqueAttchID = uniqueAttchID,
                TableKeyId = keyID,
                TableName = tableName,
            };

            attachment.BuildDefaultFolders();

            return attachment;
        }


        public HQAttachmentFile AddFile(HQAttachmentFile originalFile, bool createDuplicate = true)
        {
            if (originalFile == null)
                return null;

            if (createDuplicate)
            {
                var file = HQAttachmentFile.Init(this);
                file.Description = originalFile.Description;
                file.OrigFileName = file.Description;

                file.SetData(originalFile.GetData());
                Files.Add(file);
                return file;
            }
            else
            {
                originalFile.UniqueAttchID = this.UniqueAttchID;
                return originalFile;
            }
        }

        public HQAttachmentFile AddFile(string fileName, byte[] data)
        {
            var file = HQAttachmentFile.Init(this);
            file.Description = fileName;
            if (file.Description.Contains("image."))
            {
                file.Description = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", file.AttachmentId, file.Description);
            }
            file.OrigFileName = file.Description;
            file.SetData(data);
            Files.Add(file);
            return file;
        }

        public HQAttachmentFile AddFile(HttpPostedFileBase uploadFile, HQAttachmentFolder folder, int? AttachmentTypeID = null)
        {
            var fileName = uploadFile.FileName;
            var file = this.Files.FirstOrDefault(f => f.Description == uploadFile.FileName);
            if (fileName.Contains("image."))
                file = null;

            if (file == null)
            {
                file = HQAttachmentFile.Init(this);
                file.AttachmentTypeID = AttachmentTypeID;
                file.Description = uploadFile.FileName;
                if (file.Description.Contains("image."))
                {
                    file.Description = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", file.AttachmentId, file.Description);
                }
                file.OrigFileName = file.Description;
                file.FolderId = folder.FolderId;
                file.Folder = folder;
                Files.Add(file);
                folder.Files.Add(file);
            }
            file.SetData(uploadFile);
            return file;
        }

        public HQAttachmentFile AddFile(HttpPostedFileBase uploadFile, int? AttachmentTypeID = null, long? folderId = 0)
        {
            var folder = Folders.FirstOrDefault(f => f.FolderId == folderId);
            if (folder == null)
            {
                BuildDefaultFolders();
                folder = Folders.FirstOrDefault(f => f.FolderId == folderId);
            }
            if (folder == null)
                folder = GetRootFolder();

            return AddFile(uploadFile, folder, AttachmentTypeID);

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
                        var subFolder = folder.SubFolders.FirstOrDefault(f => f.Description == treeFolder);
                        if (subFolder == null)
                        {
                            subFolder = HQAttachmentFolder.Init(folder.Attachment);
                            subFolder.ParentId = folder.FolderId;
                            subFolder.Description = treeFolder;
                            subFolder.Parent = folder;
                            folder.SubFolders.Add(subFolder);
                        }
                        folder = subFolder;
                    }
                }
            }

            return folder;
        }
    }

    public partial class HQAttachmentFolder
    {
        public static HQAttachmentFolder Init(HQAttachment attachment)
        {
            var newObj = new HQAttachmentFolder()
            {
                HQCo = attachment.HQCo,
                UniqueAttchID = attachment.UniqueAttchID,
                FolderId = attachment.Folders.DefaultIfEmpty().Max(max => max == null ? -1: max.FolderId) + 1,
                Description = "New Folder",
                ParentId = 0,
                Attachment = attachment
            };

            return newObj;
        }

        public void DeleteSubItems()
        {
            var db = VPEntities.GetDbContextFromEntity(this);
            foreach (var subFolder in this.SubFolders.ToList())
            {
                subFolder.DeleteSubItems();
                this.SubFolders.Remove(subFolder);
                db.HQAttachmentFolders.Remove(subFolder);
            }
            foreach (var file in this.Files.ToList())
            {
                file.DeleteData();
                this.Files.Remove(file);
                db.HQAttachmentFiles.Remove(file);
            }

        }

        public HQAttachmentFolder AddFolder(string folderName)
        {
            var folder = SubFolders.FirstOrDefault(f => f.Description == folderName);
            if (folder == null)
            {
                folder = HQAttachmentFolder.Init(folder.Attachment);
                folder.ParentId = folder.FolderId;
                folder.Description = folderName;
                folder.Parent = folder;
                folder.SubFolders.Add(folder);
            }
            return folder;
        }

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
    }

    public partial class HQAttachmentFile
    {

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
                        if (folder.Description != "root")
                        {
                            if (!string.IsNullOrEmpty(_FolderPath))
                                _FolderPath += "/";

                            _FolderPath += folder.Description;
                        }
                    }
                }
                return _FolderPath;
            }
        }

        public MemoryStream GetMemoryStream()
        {
            using var dbAttch = new VPAttachmentsEntities();
            dbAttch.Database.CommandTimeout = 600;
            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);
            if (attachment == null)
            {
                return null;
            }
            var filems = new MemoryStream(attachment.AttachmentData);
            return filems;
        }

        public byte[] GetData()
        {
            using var dbAttch = new VPAttachmentsEntities();
            dbAttch.Database.CommandTimeout = 600;
            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);

            return attachment.AttachmentData;
        }

        public void SetData(HttpPostedFileBase fileUpload)
        {
            //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            //{
                var attachmentId = this.AttachmentId;
                using var binaryReader = new BinaryReader(fileUpload.InputStream);
                var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);

                using var dbAttch = new VPAttachmentsEntities();
                dbAttch.Database.CommandTimeout = 600;
                var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == attachmentId);

                if (attachment == null)
                {
                    attachment = new Attachment
                    {
                        AttachmentFileType = System.IO.Path.GetExtension(fileUpload.FileName),
                        SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks),
                        AttachmentID = this.AttachmentId,
                        AttachmentData = fileData
                    };
                    dbAttch.Attachments.Add(attachment);
                }
                else
                {
                    attachment.AttachmentFileType = System.IO.Path.GetExtension(fileUpload.FileName);
                    attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                    attachment.AttachmentID = this.AttachmentId;
                    attachment.AttachmentData = fileData;
                }
                binaryReader.Dispose();
                dbAttch.BulkSaveChanges();
            //});
            //task.Start();
        }

        public void SetData(byte[] data)
        {
            //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            //{
                var attachmentId = this.AttachmentId;
                using var dbAttch = new VPAttachmentsEntities();
                dbAttch.Database.CommandTimeout = 600;
                var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == attachmentId);

                if (attachment == null)
                {
                    attachment = new Attachment
                    {
                        AttachmentFileType = System.IO.Path.GetExtension(OrigFileName),
                        SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks),
                        AttachmentID = this.AttachmentId,
                        AttachmentData = data
                    };
                    dbAttch.Attachments.Add(attachment);
                }
                else
                {
                    attachment.AttachmentFileType = System.IO.Path.GetExtension(OrigFileName);
                    attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                    attachment.AttachmentID = this.AttachmentId;
                    attachment.AttachmentData = data;
                }
                dbAttch.BulkSaveChanges();
                attachment = null;
            //});

            //task.Start();
        }

        public void DeleteData()
        {
            using var dbAttch = new VPAttachmentsEntities();
            dbAttch.Database.CommandTimeout = 600;
            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == this.AttachmentId);
            if (attachment != null)
            {
                dbAttch.Attachments.Remove(attachment);
            }
            dbAttch.BulkSaveChanges();
        }

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

        public static HQAttachmentFile Init(HQAttachment attachment)
        {
            
            var keyField = string.Format(AppCultureInfo.CInfo(), "KeyID={0}", attachment.TableKeyId);

            using var db = new VPEntities();
            
            var emp = StaticFunctions.GetCurrentEmployee();
            var usrProf = db.UserProfiles.FirstOrDefault(f => f.Employee == emp.EmployeeId);
            var attachmentId = GetNextAttachmentId();
            var newObj = new HQAttachmentFile()
            {
                HQCo = attachment.HQCo,
                AttachmentId = attachmentId,
                FormName = attachment.TableName,
                KeyField = keyField,
                AddedBy = usrProf == null ? emp.Email : usrProf.VPUserName,
                AddDate = DateTime.Now,
                DocName = "Database",
                TableName = attachment.TableName,
                UniqueAttchID = attachment.UniqueAttchID,
                DocAttchYN = "Y",
                CurrentState = "A",
                IsEmail = "N",
                Attachment = attachment
            };            

            return newObj;
        }

        public static int GetNextAttachmentId()
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("attachmentId", typeof(int));

            var result = db.udNextAttachmentId(outParm);

            return (int)outParm.Value;
        }
    }

}
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Attachment.Models
{
    public class ExplorerListViewModel
    {

        public ExplorerListViewModel()
        {

        }

        public ExplorerListViewModel(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType = DB.ExplorerViewTypeEnum.List)
        {

            UniqueAttchId = folder.UniqueAttchID;
            FolderId = folder.FolderId;
            ViewType = viewType;
            Preview = true;
            Items = new List<ExplorerViewModel>();
            if (folder.Parent != null)
            {
                var parentFolder = new ExplorerViewModel(folder.Parent);
                parentFolder.FontIcon = "fas fa-folder-upload folder";
                parentFolder.FileCnt = 0;
                Items.Add(parentFolder);
            }

            Items.AddRange(folder.SubFolders.OrderBy(o => o.Description).Select(s => new ExplorerViewModel(s)).ToList());
            Items.AddRange(folder.Files.Where(f => !f.Files.Any()).OrderBy(o => o.Description).Select(s => new ExplorerViewModel(s)).ToList());

            ParentFolders = new List<FolderViewModel>();
            BuildFolderList(folder, viewType);

            ParentFolders.Reverse();
        }

        public ExplorerListViewModel(HQAttachment attachment, long folderId = 0, DB.ExplorerViewTypeEnum viewType = DB.ExplorerViewTypeEnum.List)
        {
            UniqueAttchId = attachment.UniqueAttchID;
            FolderId = folderId;
            ViewType = viewType;
            Preview = true;
            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
            var files = attachment.Files.Where(f => (f.FolderId ?? 0) == folderId && !f.Files.Any()).ToList();

            Items = new List<ExplorerViewModel>();
            if (folder.Parent != null)
            {
                var parentFolder = new ExplorerViewModel(folder.Parent);
                Items.Add(parentFolder);
                parentFolder.FontIcon = "far fa-folder-upload";
            }

            Items.AddRange(folder.SubFolders.Select(s => new ExplorerViewModel(s)).ToList());
            Items.AddRange(files.Select(s => new ExplorerViewModel(s)).ToList());

            ParentFolders = new List<FolderViewModel>();
            BuildFolderList(folder, viewType);

            ParentFolders.Reverse();
        }

        private void BuildFolderList(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType)
        {
            if (folder != null)
            {
                ParentFolders.Add(new FolderViewModel(folder, viewType));
                if (folder.Parent != null)
                {
                    BuildFolderList(folder.Parent, viewType);
                }
            }
        }

        [Key]
        public Guid UniqueAttchId { get; set; }

        [Key]
        public long FolderId { get; set; }

        public DB.ExplorerViewTypeEnum ViewType { get; set; }

        public bool Preview { get; set; }

        public List<ExplorerViewModel> Items { get; }

        public List<FolderViewModel> ParentFolders { get; }
    }

    public class FolderViewModel
    {
        public FolderViewModel()
        {

        }

        public FolderViewModel(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType)
        {

            Title = folder.tDescription;
            FolderId = folder.FolderId;
            UniqueAttchId = folder.UniqueAttchID;
            HQCo = folder.HQCo;
            ViewType = viewType;
        }
        public byte HQCo { get; set; }

        public Guid UniqueAttchId { get; set; }

        public string Title { get; set; }

        public long FolderId { get; set; }

        public DB.ExplorerViewTypeEnum ViewType { get; set; }
    }

    public class ExplorereMoveViewMode
    {
        public ExplorereMoveViewMode()
        {

        }

        public ExplorerViewModel From { get; set; }

        public ExplorerViewModel To { get; set; }

        internal ExplorereMoveViewMode ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == From.UniqueAttchId);
            if (From.ObjectType == DB.ExplorerObjectTypeEnum.Folder)
            {
                var updObj = attachment.Folders.FirstOrDefault(f => f.FolderId == From.FolderId);
                if (updObj != null)
                {
                    updObj.ParentId = To.FolderId;
                    try
                    {
                        db.BulkSaveChanges();
                    }
                    catch (Exception ex)
                    {
                        modelState.AddModelError("", ex.Message);
                    }
                    return this;
                }
            }
            else if (From.ObjectType == DB.ExplorerObjectTypeEnum.File)
            {
                var updObj = attachment.Files.FirstOrDefault(f => f.AttachmentId == From.AttachmentId);
                if (updObj != null)
                {
                    updObj.FolderId = To.FolderId;
                    try
                    {
                        db.BulkSaveChanges();
                    }
                    catch (Exception ex)
                    {
                        modelState.AddModelError("", ex.Message);
                    }
                    return this;
                }
            }

            return this;
        }
    }

    public class ExplorerViewModel
    {
        public ExplorerViewModel()
        {

        }

        public ExplorerViewModel(HQAttachmentFolder folder)
        {
            Title = folder.tDescription;
            FolderId = folder.FolderId;
            UniqueAttchId = folder.UniqueAttchID;
            HQCo = folder.HQCo;
            FontIcon = "fas fa-folder folder";
            ObjectType = DB.ExplorerObjectTypeEnum.Folder;
            IsNew = false;
            IsThumbnail = false;

            FileCnt = FileCntAll(folder);

            OriginalName = folder.tDescription;
            Title = folder.tDescription.Trim();
            //Title = Title.Length > 25 ? string.Format("{0}...", Title.Substring(0, 24)) : Title;
        }

        private int FileCntAll(HQAttachmentFolder folder)
        {
            var cnt = 0;
            cnt = folder.Files.Where(f => !f.Files.Any()).Count();
            folder.SubFolders.ToList().ForEach(e => { cnt += FileCntAll(e); });
            return cnt;
        }

        public ExplorerViewModel(HQAttachmentFile file)
        {

            var mime = MimeMapping.GetMimeMapping(file.OrigFileName);

            Mime = mime;
            Title = file.Description;
            FolderId = file.FolderId ?? 0;
            UniqueAttchId = (Guid)file.UniqueAttchID;
            HQCo = file.HQCo;
            AttachmentId = file.AttachmentId;
            TumbnailId = file.ThumbnailAttachmentID;
            FontIcon = GetFont();
            ObjectType = DB.ExplorerObjectTypeEnum.File;
            OriginalName = file.OrigFileName;

            IsNew = false;
            IsThumbnail = false;

            Title = System.IO.Path.GetFileNameWithoutExtension(file.OrigFileName);
            //Title = Title.Length > 25 ? string.Format("{0}...", Title.Substring(0, 24)) : Title;
        }

        private string GetFont()
        {
            var font = "far fa-file";
            switch (Mime)
            {
                case "image/jpeg":
                case "image/jpg":
                case "image/bmp":
                case "image/gif":
                case "image/png":
                case "image/pjpeg":
                    font = "far fa-file-image";
                    break;
                case "application/pdf":
                    font = "far fa-file-pdf";
                    break;
                case "application/vnd.ms-project":
                    font = "far fa-file-chart-line";
                    break;
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    font = "far fa-file-powerpoint";
                    break;
                case "application/msword":
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    font = "far fa-file-word";
                    break;
                case "application/vnd.ms-excel":
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    font = "far fa-file-excel";
                    break;
                case "application/octet-stream":
                    var fileName = Title;
                    if (Title.Contains(".kmz") || Title.Contains(".kml"))
                    {
                        font = "far fa-map";
                    }
                    else
                    {
                    }
                    break;
                case "application/txt":
                case "htaccess":
                case "log":
                case "sql":
                case "php":
                case "js":
                case "json":
                case "css":
                case "html":

                default:
                    break;
            }

            return font;
        }

        [Key]
        public byte HQCo { get; set; }

        [Key]
        public Guid UniqueAttchId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 12)]
        public string Title { get; set; }

        [Key]
        public long FolderId { get; set; }

        [Key]
        public int AttachmentId { get; set; }

        public int? TumbnailId { get; set; }

        public int FileCnt { get; set; }

        [Key]
        public DB.ExplorerObjectTypeEnum ObjectType { get; set; }

        public string FontIcon { get; set; }

        [Key]
        public string Mime { get; set; }

        public bool IsNew { get; set; }

        public bool IsThumbnail { get; set; }

        public string OriginalName { get; set; }


        internal ExplorerViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {

            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == UniqueAttchId);
            if (ObjectType == DB.ExplorerObjectTypeEnum.Folder)
            {
                var updObj = attachment.Folders.FirstOrDefault(f => f.FolderId == FolderId);
                if (updObj != null)
                {
                    updObj.tDescription = Title;
                    try
                    {
                        db.BulkSaveChanges();
                        return new ExplorerViewModel(updObj);
                    }
                    catch (Exception ex)
                    {
                        modelState.AddModelError("", ex.Message);
                        return this;
                    }
                }
            }
            else if (ObjectType == DB.ExplorerObjectTypeEnum.File)
            {
                var updObj = attachment.Files.FirstOrDefault(f => f.AttachmentId == AttachmentId);
                if (updObj != null)
                {
                    var newName = System.IO.Path.GetFileNameWithoutExtension(Title);
                    newName = string.Format("{0}{1}", newName, System.IO.Path.GetExtension(OriginalName));
                    updObj.Description = newName;
                    try
                    {
                        db.BulkSaveChanges();
                        return new ExplorerViewModel(updObj);
                    }
                    catch (Exception ex)
                    {
                        modelState.AddModelError("", ex.Message);
                        return this;
                    }
                }
            }

            return this;
        }
    }
}
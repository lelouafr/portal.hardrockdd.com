using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HQ.Attachment
{
    public class FileExplorerListViewModel
    {

        public FileExplorerListViewModel()
        {

        }

        public FileExplorerListViewModel(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType = DB.ExplorerViewTypeEnum.List)
        {

            UniqueAttchId = folder.UniqueAttchID;
            FolderId = folder.FolderId;
            ViewType = viewType;

            Items = folder.SubFolders.Select(s => new FileExplorerViewModel(s)).ToList();
            Items.AddRange(folder.Files.Where(f => !f.Files.Any()).Select(s => new FileExplorerViewModel(s)).ToList());

            ParentFolders = new List<FileFolderViewModel>();
            BuildFolderList(folder, viewType);

            ParentFolders.Reverse();
        }

        //public FileExplorerListViewModel(Guid uniqueAttchId, long folderId)
        //{
        //    using var db = new VPContext();
        //    UniqueAttchId = uniqueAttchId;
        //    FolderId = folderId;
        //    var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchId && f.FolderId == folderId);
        //    var files = db.HQAttachmentFiles.Where(f => f.UniqueAttchID == uniqueAttchId && (f.FolderId ?? 0) == folderId).ToList();
        //    files.ForEach(e => e.FolderId ??= folderId);

        //    db.BulkSaveChanges();
        //    Items = folder.SubFolders.Select(s => new FileExplorerViewModel(s)).ToList();
        //    Items.AddRange(files.Select(s => new FileExplorerViewModel(s)).ToList());

        //    ParentFolders = new List<FileFolderViewModel>();
        //    BuildFolderList(folder);

        //    ParentFolders.Reverse();
        //}


        public FileExplorerListViewModel(HQAttachment attachment, long folderId = 0, DB.ExplorerViewTypeEnum viewType = DB.ExplorerViewTypeEnum.List)
        {
            UniqueAttchId = attachment.UniqueAttchID;
            FolderId = folderId;
            ViewType = viewType;
            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
            var files = attachment.Files.Where(f => (f.FolderId ?? 0) == folderId && !f.Files.Any()).ToList();

            Items = folder.SubFolders.Select(s => new FileExplorerViewModel(s)).ToList();
            Items.AddRange(files.Select(s => new FileExplorerViewModel(s)).ToList());

            ParentFolders = new List<FileFolderViewModel>();
            BuildFolderList(folder, viewType);

            ParentFolders.Reverse();
        }

        private void BuildFolderList(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType) {
            if (folder != null)
            {
                ParentFolders.Add(new FileFolderViewModel(folder, viewType));
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

        public List<FileExplorerViewModel> Items { get;  }

        public List<FileFolderViewModel> ParentFolders { get; }
    }

    public class FileFolderViewModel
    {
        public FileFolderViewModel()
        {

        }

        public FileFolderViewModel(HQAttachmentFolder folder, DB.ExplorerViewTypeEnum viewType)
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

    public class FileExplorerViewModel
    {
        public FileExplorerViewModel()
        {

        }

        public FileExplorerViewModel(HQAttachmentFolder folder)
        {
            Title = folder.tDescription;
            FolderId = folder.FolderId;
            UniqueAttchId = folder.UniqueAttchID;
            HQCo = folder.HQCo;
            FontIcon = "fas fa-folder folder";
            ObjectType = DB.ExplorerObjectTypeEnum.Folder;
            IsNew = false;
            IsThumbnail = false;
        }

        public FileExplorerViewModel(HQAttachmentFile file)
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

            IsNew = false;
            IsThumbnail = false;
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
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    font = "far fa-file-word";
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    font = "far fa-file-excel";
                    break;
                case "application/octet-stream":
                    var fileName = Title;
                    if (fileName.Contains(".kmz")  || fileName.Contains(".kml"))
                    {
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

    }
}
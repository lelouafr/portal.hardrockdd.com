using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Attachment.Models
{
    public class AttachmentViewModel
    {
        public AttachmentViewModel()
        {

        }

        public AttachmentViewModel(DB.Infrastructure.ViewPointDB.Data.HQAttachment attachment)
        {
            if (attachment == null)
                return;

            var root = attachment.GetRootFolder();
            Tree = new TreeViewModel(root);
        }

        [Key]
        [Display(Name = "Co")]
        public byte HQCo { get; set; }

        [Key]
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        public TreeViewModel Tree { get; set; }
    }

    public class TreeViewModel
    {
        public TreeViewModel()
        {

        }

        public TreeViewModel(HQAttachmentFolder folder)
        {
            if (folder == null)
                return;

            HQCo = folder.HQCo;
            UniqueAttchID = folder.UniqueAttchID;
            FolderId = folder.FolderId;
            ParentId = folder.ParentId ?? 0;
            Description = folder.tDescription;

            var folders = folder.SubFolders.Select(e => new TreeViewModel(e)).ToList();
            var files = folder.Files.Select(file => new TreeViewModel(file)).ToList();
            Tree = new List<TreeViewModel>();
            Tree.AddRange(folders);
            Tree.AddRange(files);
        }

        public TreeViewModel(HQAttachmentFile file)
        {
            if (file == null)
                return;

            HQCo = file.HQCo;
            UniqueAttchID = file.UniqueAttchID;
            AttachmentId = file.AttachmentId;
            FileName = file.OrigFileName;
            Description = file.Description;
            FolderId = file.FolderId ?? 0;
            ParentId = FolderId;
            Mime = MimeMapping.GetMimeMapping(file.OrigFileName);
        }


        [Key]
        [Display(Name = "Attachment Group")]
        public byte HQCo { get; set; }

        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        [Key]
        [Display(Name = "Attachment Number")]
        public long AttachmentId { get; set; }

        [Display(Name = "FolderId")]
        public long FolderId { get; set; }

        [Display(Name = "FolderId")]
        public long ParentId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Mime")]
        public string Mime { get; set; }
        public List<TreeViewModel> Tree { get;  }
    }
}
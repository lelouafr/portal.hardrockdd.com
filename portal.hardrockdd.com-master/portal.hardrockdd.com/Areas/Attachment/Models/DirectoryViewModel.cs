//using Steeltoe.Common.Net;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace portal.Areas.Attachment.Models
//{
//    public class DirectoryViewModel
//    {
//        public DirectoryViewModel()
//        {

//        }

//        public DirectoryViewModel(string uncPath)
//        {
//            Credential = new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass);
//            using var fileShare = new WindowsNetworkFileShare(uncPath, new System.Net.NetworkCredential(db.AttachmentUser, db.AttachmentPass));



//        }
//        public System.Net.NetworkCredential Credential { get; set; }

//        public string UncPath { get; set; }

//        public string FolderName { get; set; }

//        public FolderTreeViewModel Tree { get; set; }
//    }


//    public class FolderTreeViewModel
//    {
//        public FolderTreeViewModel()
//        {

//        }

//        public FolderTreeViewModel(string uncPath, string folder)
//        {
//            if (folder == null)
//                return;


//            var folders = folder.SubFolders.Select(e => new TreeViewModel(e)).ToList();
//            var files = folder.Files.Select(file => new TreeViewModel(file)).ToList();
//            Tree = new List<TreeViewModel>();
//            Tree.AddRange(folders);
//            Tree.AddRange(files);
//        }

//        public FolderTreeViewModel(HQAttachmentFile file)
//        {
//            if (file == null)
//                return;

//            HQCo = file.HQCo;
//            UniqueAttchID = file.UniqueAttchID;
//            AttachmentId = file.AttachmentId;
//            FileName = file.OrigFileName;
//            Description = file.Description;
//            FolderId = file.FolderId ?? 0;
//            ParentId = FolderId;
//            Mime = MimeMapping.GetMimeMapping(file.OrigFileName);
//        }


//        [Key]
//        [Display(Name = "Attachment Group")]
//        public byte HQCo { get; set; }

//        [Display(Name = "UniqueAttchID")]
//        public Guid? UniqueAttchID { get; set; }

//        [Key]
//        [Display(Name = "Attachment Number")]
//        public long AttachmentId { get; set; }

//        [Display(Name = "FolderId")]
//        public long FolderId { get; set; }

//        [Display(Name = "FolderId")]
//        public long ParentId { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
//        [Display(Name = "File Name")]
//        public string Description { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
//        [Display(Name = "File Name")]
//        public string FileName { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
//        [Display(Name = "File Mime")]
//        public string Mime { get; set; }
//        public List<TreeViewModel> Tree { get; }
//    }
//}
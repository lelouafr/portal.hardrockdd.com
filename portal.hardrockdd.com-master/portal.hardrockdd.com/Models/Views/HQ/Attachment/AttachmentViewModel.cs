using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Attachment
{
    public class AttachmentListViewModel
    {
        //public static List<AttachmentViewModel> FlattenTreeList(AttachmentTreeViewModel tree)
        //{
        //    if (tree == null)
        //        throw new ArgumentNullException(nameof(tree));
        //    var List = new List<AttachmentViewModel>();
        //    List.Add(tree.data);
        //    SubFlattenTreeList(tree.children, List);
        //    return List;
        //}

        //private static void SubFlattenTreeList(List<AttachmentTreeViewModel> tree, List<AttachmentViewModel> list)
        //{
        //    foreach (var item in tree)
        //    {
        //        list.Add(item.data);
        //        if (item.children != null)
        //        {
        //            SubFlattenTreeList(item.children, list);
        //        }
        //    }
        //}

        public AttachmentListViewModel()
        {

        }

        public AttachmentListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            UniqueAttchID = job.UniqueAttchID;
            //using var db = new VPContext();
            //var root = db.HQAttachmentFolders.FirstOrDefault(f => f.UniqueAttchID == job.UniqueAttchID && f.ParentId == null);
            //var files = db.HQAttachmentFiles.Where(f => f.UniqueAttchID == job.UniqueAttchID && f.UniqueAttchID != null).ToList();
            //if (root == null)
            //{
            //    root = new HQAttachmentFolder()
            //    {
            //        HQCo = job.JCCo,
            //        FolderId = 0,
            //        ParentId = null,
            //        Description = "root",
            //        //UniqueAttchID = (Guid)job.UniqueAttchID
            //    };
            //    db.HQAttachmentFolders.Add(root);
            //}
            //var tree = new AttachmentTreeViewModel(root, files);
            //var APFolder = new HQAttachmentFolder()
            //{
            //    HQCo = job.JCCo,
            //    FolderId = 90000,
            //    ParentId = 0,
            //    Description = "AP Invoices"
            //};
            //var APFolderId = 90001;
            //var APTree = new AttachmentTreeViewModel(APFolder, new List<HQAttachmentFile>());
            //tree.children.Add(APTree);
            //foreach (var vendor in job.APInvoiceLines.GroupBy(f => new { f.APTran.Vendor, f.APTran.VendorId }).Select(s => new { s.Key.Vendor, lines = s }).ToList())
            //{
            //    var vendFolder = new HQAttachmentFolder() { HQCo = vendor.Vendor.VendorGroupId, FolderId = APFolderId, ParentId = APFolder.FolderId, Description = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", vendor.Vendor.VendorId, vendor.Vendor.DisplayName) };
            //    var vendInv = vendor.lines.GroupBy(f => new { f.APTran, f.APTransId }).Select(s => new { s.Key.APTran, lines = s }).ToList();
            //    var vndTree = new AttachmentTreeViewModel(vendFolder, new List<HQAttachmentFile>(), true);
            //    APFolderId++;
            //    foreach (var invoice in vendInv)
            //    {
            //        var subFolder = new HQAttachmentFolder() { HQCo = invoice.APTran.APCo, FolderId = APFolderId, ParentId = vendFolder.FolderId, Description = invoice.APTran.APRef };
            //        files = db.HQAttachmentFiles.Where(f => f.HQCo == invoice.APTran.APCo && f.UniqueAttchID == invoice.APTran.UniqueAttchID && f.UniqueAttchID != null).ToList();
            //        var subTree = new AttachmentTreeViewModel(subFolder, files, true);
            //        vndTree.children.Add(subTree);
            //        APFolderId++;
            //    }
            //    APTree.children.Add(vndTree);
            //}
            
            //Tree = tree;
            //JsonTree = JsonConvert.SerializeObject(Tree, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        //public AttachmentListViewModel(byte hqco, string tableName, string state)
        //{
        //    using var db = new VPContext();
        //    #region mapping
        //    HQCo = hqco;
        //    TableName = tableName;
        //    #endregion
        //    var files = db.HQAttachmentFiles.Where(f => f.HQCo == hqco && f.CurrentState == state && f.TableName == tableName && f.UniqueAttchID == null).AsEnumerable();

        //    //List = files.Select(s => new AttachmentViewModel(s)).ToList();

        //    //var root = new HQAttachmentFolder()
        //    //{
        //    //    HQCo = hqco,
        //    //    FolderId = 0,
        //    //    ParentId = null,
        //    //    Description = "root",
        //    //    //UniqueAttchID = uniqueAttchID
        //    //};
        //    //Tree = new AttachmentTreeViewModel(root, files.ToList());

        //    //JsonTree = JsonConvert.SerializeObject(Tree, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        //}

        public AttachmentListViewModel(byte hqco, string tableName, long keyID, Guid? uniqueAttchID)
        {
            using var db = new VPContext();
            var attachment = HQAttachment.FindCreate(hqco, uniqueAttchID, tableName, keyID, db);
            uniqueAttchID = attachment.UniqueAttchID;

            #region mapping
            HQCo = hqco;
            UniqueAttchID = uniqueAttchID;
            KeyID = keyID;
            TableName = tableName;
            #endregion
            //var files = db.HQAttachmentFiles.Where(f => f.HQCo == hqco && f.UniqueAttchID == uniqueAttchID && f.UniqueAttchID != null).AsEnumerable();
            //var root = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == hqco && f.UniqueAttchID == uniqueAttchID && f.ParentId == null);
            //var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == tableName).ToList();
            //var attachmentFolders = db.HQAttachmentFolders.Where(f => f.UniqueAttchID == uniqueAttchID).ToList();
            ////attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;
            //if (uniqueAttchID != null)
            //{
            //    List = files.Select(s => new AttachmentViewModel(s)).ToList();
            //}
            //else
            //{
            //    List = new List<AttachmentViewModel>();
            //}
            //if (root == null)
            //{
            //    root = new HQAttachmentFolder()
            //    {
            //        HQCo = hqco,
            //        FolderId = 0,
            //        ParentId = null,
            //        Description = "root",
            //        UniqueAttchID = (Guid)uniqueAttchID
            //    };
            //    db.HQAttachmentFolders.Add(root);
            //}
            //var folderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;
            //if (attachmentTypes.Count > 0)
            //{
            //    foreach (var folder in attachmentTypes)
            //    {
            //        if (!root.SubFolders.Any(f => f.Description == folder.Description))
            //        {
            //            var subFolder = new HQAttachmentFolder();
            //            subFolder.HQCo = root.HQCo;
            //            subFolder.ParentId = root.FolderId;
            //            subFolder.Description = folder.Description;
            //            subFolder.FolderId = folderId;
            //            root.SubFolders.Add(subFolder);
            //            folderId++;
            //        }
            //    }
            //}
            //db.SaveChanges();

            //Tree = new AttachmentTreeViewModel(root, files.ToList());

            //JsonTree = JsonConvert.SerializeObject(Tree, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        //public AttachmentListViewModel(HQAttachment attachment)
        //{
        //    #region mapping
        //    HQCo = attachment.HQCo;
        //    UniqueAttchID = attachment.UniqueAttchID;
        //    KeyID = attachment.TableKeyId;
        //    TableName = attachment.TableName;
        //    #endregion
        //    var files = attachment.Files.AsEnumerable();
        //    var root = attachment.Folders.FirstOrDefault(f => f.ParentId == null);
        //    var attachmentTypes = attachment.db.AttachmentTypes.Where(f => f.TableId == attachment.TableName).ToList();
        //    var attachmentFolders = attachment.Folders.ToList();

        //    List = files.Select(s => new AttachmentViewModel(s)).ToList();

        //    if (root == null)
        //    {
        //        root = new HQAttachmentFolder()
        //        {
        //            HQCo = attachment.HQCo,
        //            FolderId = 0,
        //            ParentId = null,
        //            Description = "root",
        //            UniqueAttchID = attachment.UniqueAttchID
        //        };
        //        attachment.Folders.Add(root);
        //        attachment.db.BulkSaveChanges();
        //    }

        //    var folderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;
        //    if (attachmentTypes.Count > 0)
        //    {
        //        foreach (var folder in attachmentTypes)
        //        {
        //            if (!root.SubFolders.Any(f => f.Description == folder.Description))
        //            {
        //                var subFolder = new HQAttachmentFolder
        //                {
        //                    HQCo = root.HQCo,
        //                    ParentId = root.FolderId,
        //                    Description = folder.Description,
        //                    FolderId = folderId
        //                };
        //                root.SubFolders.Add(subFolder);
        //                folderId++;
        //                attachment.db.BulkSaveChanges();
        //            }
        //        }
        //    }

        //    Tree = new AttachmentTreeViewModel(root, files.ToList());

        //    JsonTree = JsonConvert.SerializeObject(Tree, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        //}



        [Key]
        [HiddenInput]
        //[Required]
        [Display(Name = "Co")]
        public byte HQCo { get; set; }

        [Key]
        [HiddenInput]
        //[Required]
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        [Key]
        //[Required]
        [UIHint("TextBox")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [Key]
        [HiddenInput]
        //[Required]
        [Display(Name = "Co")]
        public long? KeyID { get; set; }

        //public List<AttachmentViewModel> List { get; }


        //public AttachmentTreeViewModel Tree { get; }

        //public string JsonTree { get; set; }
    }
    /*
    public class AttachmentTreeViewModel
    {
        public AttachmentTreeViewModel()
        {

        }

        public AttachmentTreeViewModel(HQAttachmentFolder folder, List<DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile> attachments, bool virtualFolder = false)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }
            var files = attachments.Where(f => (f.FolderId ?? 0) == folder.FolderId || (virtualFolder && (f.FolderId ?? 0) == 0));
            //id = folder.FolderId.ToString();
            //parent = folder.ParentId == null ? "#" : folder.ParentId.ToString();
            //data = new HQAttachmentFolder() { 
            //    Co = folder.Co,
            //    FolderId = folder.FolderId,
            //    ParentId = folder.ParentId,
            //    UniqueAttchID = folder.UniqueAttchID,
            //    Description = folder.Description,                
            //};
            data = new AttachmentViewModel(folder);
            if (virtualFolder)
            {
                data.VirtualFolderId = folder.FolderId;
            }
            state = new NodeState();
            state.opened = false;
            if (folder.ParentId == null )//|| folder.Parent.ParentId == null)
            {
                state.opened = true;
            }
            icon = "fa fa-folder text-warning fa-lg";
            type = "folder";
            text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", files.Count(), folder.Description);
            text = folder.Description;
            children = folder.SubFolders.Select(s => new AttachmentTreeViewModel(s, attachments, virtualFolder)).ToList();
            children.AddRange(files.Select( s => new AttachmentTreeViewModel(folder, s, virtualFolder)).ToList());
        }


        public AttachmentTreeViewModel(HQAttachmentFolder folder, DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile file, bool virtualFolder = false)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var mime = MimeMapping.GetMimeMapping(file.OrigFileName);
            state = new NodeState();
            state.opened = false;
            data = new AttachmentViewModel(file);
            if (virtualFolder)
            {
                data.VirtualFolderId = folder.FolderId;
            }
            switch (mime)
            {

                default:
                    break;
            }
            icon = "fa fa-file fa-lg";
            type = "file";
            text = string.IsNullOrEmpty(file.Description) ? file.OrigFileName : file.Description;
            text = file.OrigFileName;
            a_attr = new nodeLink();
            if (HttpContext.Current != null)
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                if (urlHelper != null)
                {
                    a_attr.href = urlHelper.Action("Open", "Explorer", new { Area = "Attachment", attachmentId = file.AttachmentId });
                }
            }

        }

        // public Guid? UniqueAttchID { get; set; }

        //public string id { get; set; }

        public string icon { get; set; }
        
        public string type { get; set; }

        //public string parent { get; set; }

        public string text { get; set; }

        public NodeState state { get; set; }

        public nodeLink a_attr { get; set; }

        //public List<AttachmentTreeViewModel> Attachments { get; set; }

        public List<AttachmentTreeViewModel> children { get; }

        public AttachmentViewModel data { get; }
    }

    public class NodeState
    {
        public bool opened { get; set; }
    }
    public class nodeLink
    {
        public string href { get; set; }

        public string quantity {get; set;}
    }

    public class AttachmentViewModel
    {
        public AttachmentViewModel()
        {

        }

        public AttachmentViewModel(DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile Attachment)
        {
            if (Attachment == null)
            {
                throw new ArgumentNullException(nameof(Attachment));
            }

            #region mapping
            HQCo = Attachment.HQCo;
            AttachmentId = Attachment.AttachmentId;
            FileName = Attachment.OrigFileName;
            Description = Attachment.Description;
            FolderId = Attachment.FolderId ?? 0;
            ParentId = FolderId;
            UniqueAttchID = Attachment.UniqueAttchID;
            Mime = MimeMapping.GetMimeMapping(Attachment.OrigFileName);
            #endregion
        }

        public AttachmentViewModel(DB.Infrastructure.ViewPointDB.Data.HQAttachmentFolder folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            #region mapping
            HQCo = folder.HQCo;
            FolderId = folder.FolderId;
            ParentId = folder.ParentId ?? 0;
            Description = folder.Description;
            UniqueAttchID = folder.UniqueAttchID;
            //Mime = MimeMapping.GetMimeMapping(Attachment.OrigFileName);
            #endregion
        }

        [Key]
        //[Required]
        [HiddenInput]
        [Display(Name = "Attachment Group")]
        public byte HQCo { get; set; }

        [Key]
       // [Required]
        [HiddenInput]
        [Display(Name = "Attachment Number")]
        public long AttachmentId { get; set; }

        [HiddenInput]
        [Display(Name = "FolderId")]
        public long FolderId { get; set; }

        [HiddenInput]
        [Display(Name = "FolderId")]
        public long VirtualFolderId { get; set; }

        [HiddenInput]
        [Display(Name = "FolderId")]
        public long ParentId { get; set; }

        [HiddenInput]
        //[Required]
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        //[Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }


        //[Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Mime")]
        public string Mime { get; set; }
    }

    */
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Forums
{
    public class ForumLineListViewModel
    {
        public ForumLineListViewModel()
        {

        }

        public ForumLineListViewModel(DB.Infrastructure.ViewPointDB.Data.Forum forum)
        {
            if (forum == null)
            {
                List = new List<ForumLineViewModel>();
                Add = new ForumLineViewModel();
                return;
            }
            #region mapping
            Co = forum.Co;
            ForumId = forum.ForumId;
            TableName = forum.TableName;
            #endregion

            List = forum.Lines.OrderByDescending(o => o.KeyID).Select(s => new ForumLineViewModel(s)).ToList();

            Add = new ForumLineViewModel(forum);
        }


        //public ForumLineListViewModel(DB.Infrastructure.ViewPointDB.Data.Forum forum)
        //{
        //    if (forum == null)
        //    {
        //        List = new List<ForumLineViewModel>();

        //        return;
        //    }
        //    #region mapping
        //    Co = forum.Co;
        //    ForumId = forum.ForumId;
        //    TableName = forum.TableName;
        //    #endregion

        //    List = forum.Lines.OrderByDescending(o => o.KeyID).Select(s => new ForumLineViewModel(s)).ToList();

        //    Add = new ForumLineViewModel(forum);
        //}

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Forum Id")]
        public int ForumId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        public List<ForumLineViewModel> List { get; }
        
        public ForumLineViewModel Add { get; }
    }

    public class ForumLineViewModel
    {
        public ForumLineViewModel()
        {

        }
        public ForumLineViewModel(DB.Infrastructure.ViewPointDB.Data.Forum line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            #region mapping
            Co = line.Co;
            ForumId = line.ForumId;

            Attachments = new Attachment.AttachmentListViewModel(line.Co, "udWPFL", line.KeyID, line.UniqueAttchID);

            #endregion
        }
        public ForumLineViewModel(DB.Infrastructure.ViewPointDB.Data.ForumLine line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            #region mapping
            Co = line.Co;
            ForumId = line.ForumId;
            LineId = line.LineId;
            Comment = line.Comment;
            HtmlComment = line.HtmlComment;
            UniqueAttchID = line.UniqueAttchID;
            CreatedOn = (DateTime)line.CreatedOn;
            CreatedUser = new Web.WebUserViewModel(line.CreatedUser);
            Attachments = new Attachment.AttachmentListViewModel(line.Co, "udWPFL", line.KeyID, line.UniqueAttchID);

            SubList = line.Children.Select(s => new ForumLineViewModel(s)).ToList();
            #endregion
        }


        //public ForumLineViewModel(DB.Infrastructure.ViewPointDB.Data.Forum line)
        //{
        //    if (line == null)
        //    {
        //        throw new ArgumentNullException(nameof(line));
        //    }

        //    #region mapping
        //    Co = line.Co;
        //    ForumId = line.ForumId;

        //    Attachments = new Attachment.AttachmentListViewModel(line.Co, "udWPFL", line.KeyID, line.UniqueAttchID);

        //    #endregion
        //}
        //public ForumLineViewModel(DB.Infrastructure.ViewPointDB.Data.ForumLine line)
        //{
        //    if (line == null)
        //    {
        //        throw new ArgumentNullException(nameof(line));
        //    }

        //    #region mapping
        //    Co = line.Co;
        //    ForumId = line.ForumId;
        //    LineId = line.LineId;
        //    Comment = line.Comment;
        //    HtmlComment = line.HtmlComment;
        //    UniqueAttchID = line.UniqueAttchID;
        //    CreatedOn = (DateTime)line.CreatedOn;
        //    CreatedUser = new Web.WebUserViewModel(line.CreatedUser);
        //    Attachments = new Attachment.AttachmentListViewModel(line.Co, "udWPFL", line.KeyID, line.UniqueAttchID);

        //    SubList = line.Children.Select(s => new ForumLineViewModel(s)).ToList();
        //    #endregion
        //}

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "ForumId")]
        public int ForumId { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "LineId")]
        public int LineId { get; set; }

        [HiddenInput]
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        [UIHint("TextEditorAreaBox")]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "")]
        [AllowHtml]
        public string HtmlComment { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "")]
        [AllowHtml]
        public string Comment { get; set; }


        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Comment On")]
        public DateTime CreatedOn { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 7)]
        public Web.WebUserViewModel CreatedUser { get; set; }

        public List<ForumLineViewModel> SubList { get; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }
    }
}
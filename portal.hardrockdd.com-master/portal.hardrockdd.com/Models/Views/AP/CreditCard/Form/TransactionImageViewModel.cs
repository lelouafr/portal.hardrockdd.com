using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class ImageListViewModel
    {
        public ImageListViewModel()
        {

        }

        public ImageListViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            List = transaction.LinkedImages.Select(s => new ImageViewModel(s.Image, transaction)).ToList();
        }
        public List<ImageViewModel> List { get; }
    }

    
    public class ImageViewModel
    {
        public ImageViewModel()
        {

        }

        public ImageViewModel(DB.Infrastructure.ViewPointDB.Data.CreditCardImage creditCardImage, DB.Infrastructure.ViewPointDB.Data.CreditTransaction transaction)
        {
            if (creditCardImage == null) throw new System.ArgumentNullException(nameof(creditCardImage));
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            CCCo = creditCardImage.CCCo;
            EmployeeId = creditCardImage.EmployeeId;
            Mth = creditCardImage.Mth;
            ImageId = creditCardImage.ImageId;
            TransId = transaction.TransId;
            CreatedOn = creditCardImage.CreatedOn;
            CreatedBy = creditCardImage.CreatedBy;
            UniqueAttchID = creditCardImage.UniqueAttchID;
            AttachmentId = creditCardImage.AttachmentId;

            //if (creditCardImage.ThumbAttachmentId != null)
            //{
            //    var thumb = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == creditCardImage.ThumbAttachmentId);
            //    ThumbImage = thumb.AttachmentData;
            //}

            ThumbAttachmentId = creditCardImage.ThumbAttachmentId;
        }

        [Key]
        public byte CCCo { get; set; }
        
        [Key]
        public int EmployeeId { get; set; }
        
        [Key]
        public DateTime Mth { get; set; }
        
        [Key]
        public int ImageId { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public string CreatedBy { get; set; }
        
        public Guid? UniqueAttchID { get; set; }

        public int? AttachmentId { get; set; }
        
        public int? ThumbAttachmentId { get; set; }

        public long KeyID { get; set; }
        
        public long? TransId { get; set; }
        
        public string ReceiptImageRefId { get; set; }
        
        public string ImageName { get; set; }

        public string ThumbImageUri
        {
            get
            {
                if (ThumbImage == null)
                {
                    return null;
                }
                return "data:image/png;base64," + Convert.ToBase64String(ThumbImage);
            }
            set
            {
                var dataUri = value;
                if (dataUri != null)
                {
                    var encodedImage = dataUri.Split(',')[1];
                    var decodedImage = Convert.FromBase64String(encodedImage);
                    ThumbImage = decodedImage;
                }
                else
                {
                    ThumbImage = null;
                }
            }
        }
         
        [HiddenInput]
        public byte[] ThumbImage { get; set; }
    }
}
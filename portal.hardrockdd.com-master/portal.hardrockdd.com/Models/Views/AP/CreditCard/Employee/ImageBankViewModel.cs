using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Employee
{
    public class ImageBankListViewModel
    {
        public ImageBankListViewModel()
        {

        }

        public ImageBankListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DateTime mth)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            CCCo = employee.PRCo;
            Mth = mth;
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            List = employee.CreditCardImages.Where(f => f.Mth >= mth.AddMonths(-3).Date &&
                                                        f.Mth <= mth.AddMonths(3).Date)
                                            .Select(s => new ImageBankViewModel(s))
                                            .ToList();
        }

        public ImageBankListViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction transaction, bool linkedOnly = false, bool multipleMonths = false)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            CCCo = transaction.CCCo;
            Mth = transaction.Mth;
            PRCo = transaction.PRCo ?? transaction.Employee.PRCo;
            EmployeeId = (int)transaction.EmployeeId;
            TransId = transaction.TransId;
            if (linkedOnly)
            {
                List = transaction.LinkedImages.Select(s => new ImageBankViewModel(s.Image, transaction)).ToList();
            }
            else
            {
                if (multipleMonths)
                {
                    List = transaction.Employee.CreditCardImages.Where(f => f.Mth >= Mth.AddMonths(-3).Date &&
                                                                            f.Mth <= Mth.AddMonths(3).Date)
                                                                .Select(s => new ImageBankViewModel(s))
                                                                .ToList();
                }
                else
                {
                    List = transaction.Employee.CreditCardImages.Where(f => f.Mth == transaction.Mth).Select(s => new ImageBankViewModel(s, transaction)).ToList();
                }
            }
        }

        [Key]
        public byte PRCo { get; set; }
        [Key]
        public byte CCCo { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [Key]
        public long TransId { get; set; }

        public List<ImageBankViewModel> List { get; }
    }
    public class ImageBankViewModel
    {
        public ImageBankViewModel()
        {

        }

        public ImageBankViewModel(CreditCardImage image)
        {
            if (image == null) throw new System.ArgumentNullException(nameof(image));
            var attachment = image.Attachment;

            CCCo = image.CCCo;
            PRCo = image.Employee.PRCo;
            EmployeeId = image.EmployeeId;
            Mth = image.Mth;
            ImageId = image.ImageId;
            CreatedOn = image.CreatedOn;
            CreatedBy = image.CreatedBy;
            UniqueAttchID = image.UniqueAttchID;
            AttachmentId = image.AttachmentId;
            KeyID = image.KeyID;
            ReceiptImageRefId = image.ReceiptImageRefId;
            ImageName = image.ImageName;
            ThumbAttachmentId = image.ThumbAttachmentId;
            IsTagged = image.Transactions.Count > 0;
            MimeType = MimeMapping.GetMimeMapping(image.ImageName);
        }


        public ImageBankViewModel(CreditCardImage image, CreditTransaction trans)
        {
            if (image == null) throw new System.ArgumentNullException(nameof(image));

            var attachment = image.Attachment;
            CCCo = image.CCCo;
            PRCo = image.Employee.PRCo;
            EmployeeId = image.EmployeeId;
            Mth = image.Mth;
            ImageId = image.ImageId;
            CreatedOn = image.CreatedOn;
            CreatedBy = image.CreatedBy;
            UniqueAttchID = image.UniqueAttchID;
            AttachmentId = image.AttachmentId;
            KeyID = image.KeyID;
            TransId = trans.TransId;
            ReceiptImageRefId = image.ReceiptImageRefId;
            ImageName = image.ImageName;
            ThumbAttachmentId = image.ThumbAttachmentId;
            IsTagged = image.Transactions.Count > 0;
            IsTaggedCurrentTrans = image.Transactions.Where(f => f.TransId == trans.TransId).Any();
            MimeType = MimeMapping.GetMimeMapping(image.ImageName);
        }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        public byte PRCo { get; set; }

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

        public long KeyID { get; set; }

        public long? TransId { get; set; }

        public string ReceiptImageRefId { get; set; }

        public string ImageName { get; set; }
        
        public string MimeType { get; set; }

        public int? ThumbAttachmentId { get; set; }

        public bool IsTagged { get; set; }
        public bool IsTaggedCurrentTrans { get; set; }

        

    }
}
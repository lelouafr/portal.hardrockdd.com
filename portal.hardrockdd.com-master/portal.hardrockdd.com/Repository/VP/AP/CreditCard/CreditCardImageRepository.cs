using DB.Infrastructure.ViewPointDB.Data;
using DB.Infrastructure.VPAttachmentDB.Data;
using portal.Repository.VP.HQ;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.CreditCard
{
    public static class CreditCardImageRepository
    {

        public static HQAttachmentFile ProcessCreditCardReceipt(HttpPostedFileBase fileUpload, Employee employee, DateTime mth, long transId, VPContext db, VPAttachmentsContext dbAttch)
        {
            if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            if (mth == null) throw new System.ArgumentNullException(nameof(mth));
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            if (dbAttch == null) throw new System.ArgumentNullException(nameof(dbAttch));

            //var emp = StaticFunctions.GetCurrentEmployee();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var attachmentId = HQAttachmentFile.GetNextAttachmentId();
            var tableName = "budCMIB";

            var isThumbnail = fileUpload.FileName.ToLower().Contains("thumbnail");
            var filename = fileUpload.FileName; 
            if (filename.Contains("image") && !isThumbnail)
            {
                filename = string.Format(AppCultureInfo.CInfo(), "{0}-{1}", Guid.NewGuid().ToString().Replace("-","").Substring(0,5), filename);
            }
            CreditCardImage imageBank = null;
            var ext = Path.GetExtension(filename);
            var name = Path.GetFileNameWithoutExtension(filename);
            if (isThumbnail)
            {
                filename = name.Replace("_THUMBNAIL", "");
                imageBank = db.CreditCardImages.FirstOrDefault(f => f.CCCo == employee.PRCo && f.EmployeeId == employee.EmployeeId && f.Mth == mth && f.ImageName.StartsWith(filename));
            }
            else
            {
                imageBank = db.CreditCardImages.FirstOrDefault(f => f.CCCo == employee.PRCo && f.EmployeeId == employee.EmployeeId && f.Mth == mth && f.ImageName.StartsWith(filename));
                
                if (imageBank?.AttachmentId != null)
                {
                    imageBank = null;
                }
            }
            if (imageBank == null)
            {
                imageBank = new CreditCardImage();
                imageBank.CCCo = employee.PRCo;
                imageBank.Mth = mth;
                imageBank.EmployeeId = employee.EmployeeId;
                imageBank.ImageId = db.CreditCardImages
                                        .Where(f => f.CCCo == employee.PRCo &&
                                                    f.Mth == mth &&
                                                    f.EmployeeId == employee.EmployeeId)
                                        .DefaultIfEmpty()
                                        .Max(f => f == null ? 0 : f.ImageId) + 1;
                imageBank.UniqueAttchID ??= Guid.NewGuid();
                imageBank.CreatedBy = StaticFunctions.GetUserId();
                imageBank.CreatedOn = DateTime.Now;

                if (isThumbnail)
                {
                    imageBank.ThumbAttachmentId = attachmentId;
                }
                else
                {
                    imageBank.AttachmentId = attachmentId;
                }
                imageBank.ImageName = filename;
                
                db.CreditCardImages.Add(imageBank);
            }
            else
            {
                imageBank.UniqueAttchID ??= Guid.NewGuid();
                if (isThumbnail)
                {
                    imageBank.ThumbAttachmentId = attachmentId;
                }
                else
                {
                    imageBank.AttachmentId = attachmentId;
                }
            }
            if (transId != 0)
            {
                if (!imageBank.Transactions.Any(a => a.TransId == transId && a.ImageId == imageBank.ImageId))
                {
                    var imageLink = new CreditImageLink
                    {
                        CCCo = imageBank.CCCo,
                        EmployeeId = imageBank.EmployeeId,
                        Mth = imageBank.Mth,
                        ImageId = imageBank.ImageId,
                        LinkId = imageBank.Transactions.DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId) + 1,
                        TransId = transId,
                        TaggedDate = DateTime.Now
                    };
                    imageBank.Transactions.Add(imageLink);
                }

                var tran = db.CreditTransactions.FirstOrDefault(f => f.CCCo == imageBank.CCCo && f.TransId == transId);
            }
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == tableName).ToList();
            var attachmentFolders = db.HQAttachmentFolders.Where(f => f.UniqueAttchID == imageBank.UniqueAttchID).ToList();
            //var parentFolder = new 
            var parentFolder = attachmentFolders.FirstOrDefault(f => f.tDescription == "root");
            if (parentFolder == null)
            {
                parentFolder = new HQAttachmentFolder();
                parentFolder.HQCo = imageBank.CCCo;
                parentFolder.UniqueAttchID = (Guid)imageBank.UniqueAttchID;
                parentFolder.tDescription = "root";
                parentFolder.FolderId = 0;

                attachmentFolders.Add(parentFolder);
                db.HQAttachmentFolders.Add(parentFolder);
            }

            //var file = db.HQAttachmentFiles.FirstOrDefault(f => f.OrigFileName == fileUpload.FileName && f.FolderId == parentFolder.FolderId && f.UniqueAttchID == imageBank.UniqueAttchID);
            //if (file == null)
            //{
                var file = FileRepository.Init(tableName, (Guid)imageBank.UniqueAttchID, imageBank.KeyID);
                file.HQCo = imageBank.CCCo;
                file.AttachmentId = attachmentId;
                file.Description = filename;
                file.OrigFileName = filename;
                file.FolderId =  parentFolder.FolderId;
                file.AttachmentTypeID = attachmentTypes.FirstOrDefault(f => f.TableId == tableName & f.Description == parentFolder.tDescription)?.AttachmentTypeID;
                db.HQAttachmentFiles.Add(file);
                using var binaryReader = new BinaryReader(fileUpload.InputStream);
                var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);

                var attachment = new Attachment();
                attachment.AttachmentData = fileData;
                attachment.AttachmentFileType = System.IO.Path.GetExtension(filename);
                attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                attachment.AttachmentID = file.AttachmentId;

                dbAttch.Attachments.Add(attachment);
                binaryReader.Dispose();
            //}
            //else
            //{
            //    using var binaryReader = new BinaryReader(fileUpload.InputStream);
            //    var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);
            //    var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
            //    if (attachment.AttachmentData != fileData)
            //    {
            //        attachment.AttachmentData = fileData;
            //    }
            //}

            if (transId != 0)
            {
                var trans = db.CreditTransactions.FirstOrDefault(f => f.CCCo == imageBank.CCCo && f.TransId == transId);
                trans.PictureStatusId = (int)DB.CMPictureStatusEnum.Attached;

                trans.Logs.Add(CreditCardTransactionLogRepository.Init(trans, DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction picture was added {0}", DB.CMPictureStatusEnum.Attached)));
            }
            return file;
        }

        public static void TagCreditCardReceipt(CreditCardImage image, CreditTransaction tran)
        {
            if (image == null) throw new System.ArgumentNullException(nameof(image));
            if (tran == null) throw new System.ArgumentNullException(nameof(tran));
            if (tran.Mth != image.Mth)
            {
                var imageCopy = new CreditCardImage
                {
                    CCCo = image.CCCo,
                    Mth = tran.Mth,
                    EmployeeId = (int)tran.EmployeeId,
                    ImageId = tran.Employee.CreditCardImages
                                        .Where(f => f.Mth == tran.Mth)
                                        .DefaultIfEmpty()
                                        .Max(f => f == null ? 0 : f.ImageId) + 1,
                    AttachmentId = image.AttachmentId,
                    ThumbAttachmentId = image.ThumbAttachmentId,
                    CreatedBy = image.CreatedBy,
                    CreatedOn = image.CreatedOn,
                    ImageName = image.ImageName,
                    ReceiptImageRefId = image.ReceiptImageRefId,
                    UniqueAttchID = Guid.NewGuid()
                };
                tran.Employee.CreditCardImages.Add(imageCopy);
                image = imageCopy;
            }
            if (!tran.LinkedImages.Any(a => a.ImageId == image.ImageId))
            {
                var imageLink = new CreditImageLink();
                imageLink.CCCo = image.CCCo;
                imageLink.EmployeeId = image.EmployeeId;
                imageLink.Mth = image.Mth;
                imageLink.ImageId = image.ImageId;
                imageLink.LinkId = image.Transactions.DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId) + 1;
                imageLink.TransId = tran.TransId;
                imageLink.TaggedDate = DateTime.Now;
                tran.LinkedImages.Add(imageLink);
            }

            tran.PictureStatusId = (int)DB.CMPictureStatusEnum.Attached;
            tran.Logs.Add(CreditCardTransactionLogRepository.Init(tran, DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction picture was tagged {0}", DB.CMPictureStatusEnum.Attached)));


        }



        public static void UnTagCreditCardReceipt(CreditCardImage image, CreditTransaction tran)
        {
            if (image == null) throw new System.ArgumentNullException(nameof(image));
            if (tran == null) throw new System.ArgumentNullException(nameof(tran));

            var list = tran.LinkedImages.Where(f => f.ImageId == image.ImageId).ToList();

            foreach (var item in list)
            {
                image.Transactions.Remove(item);
                tran.LinkedImages.Remove(item);
            }

            if (!image.Transactions.Any(f => f.TransId == tran.TransId) || !tran.LinkedImages.Any())
            {
                tran.PictureStatusId = (int)DB.CMPictureStatusEnum.Empty;
            }
            tran.Logs.Add(CreditCardTransactionLogRepository.Init(tran, DB.CMLogEnum.PictureAdded, string.Format(AppCultureInfo.CInfo(), "Transaction picture was untagged {0}", DB.CMPictureStatusEnum.Empty)));

        }

    }
}
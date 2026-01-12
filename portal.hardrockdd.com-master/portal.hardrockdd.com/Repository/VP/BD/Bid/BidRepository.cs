//using DB.Infrastructure.ViewPointDB.Data;
//using DB.Infrastructure.VPAttachmentDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;

//namespace portal.Repository.VP.BD
//{
//    public static class BidRepository
//    {
//        //public static void RecalculateAllBores(Bid bid)
//        //{
//        //    //using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//        //    List<Task> tasks = new List<Task>();
//        //    var packages = bid.ActivePackages;
//        //    var packagesList = packages.Where(f => f.BoreLines.Any(b => b.RecalcNeeded == true)).ToList();
//        //    foreach (var package in packagesList)
//        //    {
//        //        tasks = new List<Task>();
//        //        Task subTask = new Task(() =>
//        //        {
//        //            var threadDb = new VPContext();
//        //            var threadpackage = threadDb.BidPackages.FirstOrDefault(f => f.BidId == package.BidId && f.PackageId == package.PackageId);
//        //            BidPackageProductionRateRepository.ApplyDefaultToPackage(threadDb, threadpackage.BDCo, threadpackage.BidId, threadpackage.PackageId, 0);
//        //            threadpackage.ApplyPackageCost();

//        //            threadDb.SaveChanges();
//        //            threadDb.Dispose();
//        //        });
//        //        tasks.Add(subTask);
//        //        tasks.ForEach(f => f.Start());
//        //        Task.WaitAll(tasks.ToArray());

//        //        tasks = new List<Task>();
//        //        var boreLines = package.ActiveBoreLines.Where(f => f.RecalcNeeded == true).ToList();
//        //        foreach (var bore in boreLines)
//        //        {
//        //            Task task = new Task(() =>
//        //            {
//        //                var threadDb = new VPContext();
//        //                var line = threadDb.BidBoreLines.FirstOrDefault(f => f.BidId == bore.BidId && f.BoreId == bore.BoreId);
//        //                line.UpdateMudBudget();
//        //                //threadDb.SaveChanges();
//        //                line.UpdateDirtToolingBudget();
//        //                //threadDb.SaveChanges();
//        //                line.UpdateLaborBudget();
//        //                //threadDb.SaveChanges();
//        //                line.UpdateEquipmentBudget();
//        //                //threadDb.SaveChanges();
//        //                line.UpdatePullHeadBudget((decimal)line.PipeSize);
//        //                //threadDb.SaveChanges();
//        //                BidBoreLineCostItemRepository.ApplyBidBudgetCodes(line);
                        
//        //                //threadDb.SaveChanges();
//        //                line.RecalculateCostUnits();
//        //                //threadDb.SaveChanges();


//        //                threadDb.SaveChanges();
//        //                threadDb.Dispose();
//        //            });
//        //            tasks.Add(task);
//        //            if (tasks.Count >= 5)
//        //            {
//        //                tasks.ForEach(f => f.Start());
//        //                Task.WaitAll(tasks.ToArray());
//        //                tasks = new List<Task>();
//        //            }
//        //        }
//        //        tasks.ForEach(f => f.Start());
//        //        Task.WaitAll(tasks.ToArray());
//        //        tasks = new List<Task>();
//        //    }
//        //}
        
//        //public static Models.Views.Bid.Forms.Header.BidInfoViewModel ProcessUpdate(Models.Views.Bid.Forms.Header.BidInfoViewModel model, VPContext db)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    var updObj = db.Bids.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId);
//        //    if (updObj != null)
//        //    {
//        //        var updateCustomer = updObj.CustomerId != model.CustomerId;
//        //        var updateContact = updObj.ContactId != model.ContactId;
//        //        var oldCustomerId = updObj.CustomerId;

//        //        /****Write the changes to object****/

//        //        updObj.Description = model.Description;
//        //        updObj.Comments = model.Comments;
//        //        updObj.ProjectMangerId = model.ProjectManagerId;
//        //        updObj.DivisionId = model.DivisionId;
//        //        updObj.IndustryId = model.IndustryId;
//        //        updObj.FirmId = model.FirmNumber;
//        //        updObj.Location = model.Location;
//        //        updObj.StateCodeId = model.StateCodeId;
//        //        updObj.BidType = (int)model.BidType;
//        //        updObj.StartDate = model.StartDate;
//        //        updObj.DueDate = model.DueDate;
//        //        updObj.DelayDeMobePrice = model.DelayDeMobePrice;
//        //        updObj.CustomerId = model.CustomerId;
//        //        updObj.ContactId = model.ContactId;

//        //    }
//        //    return new Models.Views.Bid.Forms.Header.BidInfoViewModel(updObj);
//        //}

//        //public static Bid CopyBid(Bid bid)
//        //{
//        //    using var db = new VPContext();

//        //    var newBid = db.Bids
//        //            .Include(x => x.BoreLines.Select(s => s.CostItems))
//        //            .Include(x => x.BoreLines.Select(s => s.Passes))
//        //            .Include(x => x.Packages.Select(s => s.CostItems))
//        //            .Include(x => x.Packages.Select(s => s.ProductionRates))
//        //            .Include(x => x.Packages.Select(s => s.Scopes))
//        //            //.Include("Packages, Packages.CostItems, Packages.ProductionRates, Packages.Scopes")
//        //            //.Include(x => x.Packages.Select(s => new { s.CostItems, s.ProductionRates, s.Scopes }))
//        //            //.Include(x => x.Forum)
//        //            .Include(x => x.Scopes)
//        //            .Include(x => x.Customers)
//        //            .Include(x => x.WorkFlow)
//        //            .AsNoTracking()
//        //            .FirstOrDefault(x => x.BidId == bid.BidId);
//        //    db.Entry(newBid).State = EntityState.Detached;
//        //    return newBid;
//        //}
     
//        //public static Bid ProcessUpdate(Bid model, VPContext db)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    var updObj = db.Bids.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId);
//        //    if (updObj != null)
//        //    {
//        //        //var addWorkFlow = true;
//        //        /****Write the changes to object****/
//        //        updObj.Status = model.Status;
//        //        //addWorkFlow = true;
//        //        //if (addWorkFlow)
//        //        //{
//        //        //    BidWorkFlowAssignmentRepository.GenerateWorkFlow(updObj, db);
//        //        //    foreach (var package in updObj.ActivePackages)
//        //        //    {
//        //        //        if (package.Status < updObj.Status && package.AwardStatus != DB.BidAwardStatusEnum.Awarded)
//        //        //        {
//        //        //            package.Status = updObj.Status;
//        //        //        }

//        //        //        foreach (var bore in package.ActiveBoreLines.Where(f => f.AwardStatus != DB.BidAwardStatusEnum.NotAwarded && package.AwardStatus != DB.BidAwardStatusEnum.Awarded))
//        //        //        {
//        //        //            bore.Status = package.Status;
//        //        //        }

//        //        //        if (updObj.Status == DB.BidStatusEnum.Awarded && package.AwardStatus == DB.BidAwardStatusEnum.Awarded)
//        //        //        {
//        //        //            package.CreateProject();
//        //        //        }
//        //        //    }
//        //        //}
//        //    }
//        //    return updObj;
//        //}

//        public static void AddPDFToAttachments(Bid bid, System.IO.MemoryStream workstream, string fileName, VPContext db, VPAttachmentsContext dbAttch)
//        {
//            if (workstream == null) throw new ArgumentNullException(nameof(workstream));
//            if (bid == null) throw new ArgumentNullException(nameof(bid));
//            if (fileName == null) throw new System.ArgumentNullException(nameof(fileName));
//            if (dbAttch == null) throw new System.ArgumentNullException(nameof(dbAttch));
//            if (db == null) throw new System.ArgumentNullException(nameof(db));

//            dbAttch.Database.CommandTimeout = 600;

//            if (bid.UniqueAttchID == null)
//            {
//                bid.UniqueAttchID = Guid.NewGuid();
//            }

//            var attachmentFolders = db.HQAttachmentFolders.Where(f => f.UniqueAttchID == bid.UniqueAttchID).ToList();
//            var parentFolder = attachmentFolders.FirstOrDefault(f => f.Description == "root");
//            if (parentFolder == null)
//            {
//                parentFolder = new HQAttachmentFolder();
//                parentFolder.HQCo = bid.BDCo;
//                parentFolder.UniqueAttchID = (Guid)bid.UniqueAttchID;
//                parentFolder.Description = "root";
//                parentFolder.FolderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;

//                attachmentFolders.Add(parentFolder);
//                db.HQAttachmentFolders.Add(parentFolder);
//            }

//            fileName = fileName.Replace("\"", "");
//            var dir = "Proposals";

//            var folder = parentFolder;
//            folder = attachmentFolders.FirstOrDefault(f => f.Description == dir);

//            if (folder == null)
//            {
//                folder = new HQAttachmentFolder();
//                folder.HQCo = bid.BDCo;
//                folder.UniqueAttchID = (Guid)bid.UniqueAttchID;
//                folder.Description = dir;
//                folder.FolderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;
//                if (parentFolder != null)
//                {
//                    folder.ParentId = parentFolder.FolderId;
//                }
//                attachmentFolders.Add(folder);
//                db.HQAttachmentFolders.Add(folder);
//            }
//            parentFolder = folder;
//            var attachId = HQAttachmentFile.GetNextAttachmentId();

//            var file = db.HQAttachmentFiles.FirstOrDefault(f => f.OrigFileName == fileName && f.FolderId == parentFolder.FolderId && f.UniqueAttchID == bid.UniqueAttchID);
//            if (file == null)
//            {
//                file = HQ.FileRepository.Init("udBDBH", (Guid)bid.UniqueAttchID, bid.KeyID);
//                file.HQCo = bid.BDCo;
//                file.AttachmentId = attachId;
//                //file.DocName = fileUpload.FileName;
//                file.Description = fileName;
//                file.OrigFileName = fileName;
//                file.FolderId = parentFolder.FolderId;
//                db.HQAttachmentFiles.Add(file);
//                var fileData = workstream.ToArray();

//                var attachment = new Attachment();
//                attachment.AttachmentData = fileData;
//                attachment.AttachmentFileType = System.IO.Path.GetExtension(fileName.Replace("\"", ""));
//                attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
//                attachment.AttachmentID = file.AttachmentId;

//                dbAttch.Attachments.Add(attachment);
//            }
//            else
//            {
//                var fileData = workstream.ToArray();
//                var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
//                if (attachment.AttachmentData != fileData)
//                {
//                    attachment.AttachmentData = fileData;
//                }
//            }
//        }
               
//    }
//}
using DB.Infrastructure.ViewPointDB.Data;
using DB.Infrastructure.VPAttachmentDB.Data;
using portal.Repository.VP.HQ;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.CreditCard
{
    public static class CreditTransactionRepository
    {
        public static HQAttachmentFile ProcessCreditCardReceipt(HttpPostedFileBase fileUpload, CreditTransaction trans, VPContext db, VPAttachmentsContext dbAttch)
        {
            if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));
            if (trans == null) throw new System.ArgumentNullException(nameof(trans));
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            if (dbAttch == null) throw new System.ArgumentNullException(nameof(dbAttch));

            trans.UniqueAttchID ??= Guid.NewGuid();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var attachId = HQAttachmentFile.GetNextAttachmentId();
            var tableName = "budCMTH";
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == tableName).ToList();
            var attachmentFolders = db.HQAttachmentFolders.Where(f => f.UniqueAttchID == trans.UniqueAttchID).ToList();
            //var parentFolder = new 
            var parentFolder = attachmentFolders.FirstOrDefault(f => f.tDescription == "root");
            if (parentFolder == null)
            {
                parentFolder = new HQAttachmentFolder();
                parentFolder.HQCo = trans.CCCo;
                parentFolder.UniqueAttchID = (Guid)trans.UniqueAttchID;
                parentFolder.tDescription = "root";
                parentFolder.FolderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;

                attachmentFolders.Add(parentFolder);
                db.HQAttachmentFolders.Add(parentFolder);
            }

            var file = db.HQAttachmentFiles.FirstOrDefault(f => f.OrigFileName == fileUpload.FileName && f.FolderId == parentFolder.FolderId && f.UniqueAttchID == trans.UniqueAttchID);
            if (file == null)
            {
                file = FileRepository.Init(tableName, (Guid)trans.UniqueAttchID, trans.KeyID);
                file.HQCo = trans.CCCo;
                file.AttachmentId = attachId;
                file.Description = fileUpload.FileName;
                file.OrigFileName = fileUpload.FileName;
                file.FolderId = parentFolder.FolderId;
                file.AttachmentTypeID = attachmentTypes.FirstOrDefault(f => f.TableId == tableName & f.Description == parentFolder.tDescription)?.AttachmentTypeID;
                db.HQAttachmentFiles.Add(file);
                using var binaryReader = new BinaryReader(fileUpload.InputStream);
                var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);

                var attachment = new Attachment();
                attachment.AttachmentData = fileData;
                attachment.AttachmentFileType = System.IO.Path.GetExtension(fileUpload.FileName);
                attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                attachment.AttachmentID = file.AttachmentId;

                dbAttch.Attachments.Add(attachment);
                binaryReader.Dispose();
            }
            else
            {
                using var binaryReader = new BinaryReader(fileUpload.InputStream);
                var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);
                var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
                if (attachment.AttachmentData != fileData)
                {
                    attachment.AttachmentData = fileData;
                }
            }
            return file;
        }

        //public static void ProcessAutoCoding(List<CreditTransaction> transactions)
        //{
        //    using var db = new VPContext();
        //    var mthList = transactions.GroupBy(g => new { g.CCCo, g.Mth }).Select(s => new { s.Key.CCCo, s.Key.Mth }).ToList();
        //    foreach (var grp in mthList)
        //    {
        //        var trans = db.CreditTransactions.Where(f => f.CCCo == grp.CCCo && f.Mth == grp.Mth).ToList();
        //        foreach (var tran in trans)
        //        {
        //            tran.AutoPictureStatus();
        //            tran.AutoCode();
        //        }
        //    }
        //    db.SaveChanges();
        //}

        //public static void ProcessAutoCoding(byte ccco, DateTime mth)
        //{
        //    using var db = new VPContext();
        //    var transactions = db.CreditTransactions.Where(f => f.CCCo == ccco && f.Mth == mth).ToList();
        //    foreach (var tran in transactions)
        //    {
        //        tran.AutoPictureStatus();
        //        tran.AutoCode();

        //    }
        //    db.SaveChanges();
        //}

        //public static List<CreditTransaction> ProcessCSVList(HttpPostedFileBase fileUpload, VPContext db)
        //{
        //    if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));

        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var errorRecords = new List<ZionCSVViewModel>();
        //    var import = new CreditImport
        //    {
        //        Co = emp.PRCo,
        //        ImportId = db.CreditImports.DefaultIfEmpty().Max(f => f == null ? 0 : f.ImportId) + 1,
        //        Source = "Zion",
        //        CreatedBy = StaticFunctions.GetUserId(),
        //        CreatedOn = DateTime.Now,
        //        FileName = fileUpload.FileName,
        //        LinesAdded = 0,
        //        LinesSkipped = 0,
        //        ErrorData = JsonConvert.SerializeObject(errorRecords)
        //    };
        //    var records = PrepareZionCSVFile(fileUpload, db, errorRecords, import);
        //    ImportMerchantV2(records, db);
        //    AddMissingJobPhaseOrCost(records, db);
        //    var trans = ImportTransaction(records, import, db);
        //    //ImportImageBank(records, trans, db);
        //    db.CreditImports.Add(import);

        //    return trans;
        //}
        
        //public static List<ZionCSVViewModelExt> PrepareZionCSVFile(HttpPostedFileBase fileUpload, VPContext db, List<ZionCSVViewModel> errorRecords, CreditImport import)
        //{

        //    if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));
        //    if (import == null) throw new System.ArgumentNullException(nameof(import));

        //    using var streamReader = new StreamReader(fileUpload.InputStream);
        //    using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        //    csv.Configuration.HeaderValidated = null;
        //    csv.Configuration.MissingFieldFound = null;
        //    //csv.Configuration.RegisterClassMap<ZionCSVViewModelMap>();
        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var empList = db.HRResources.Where(f => f.HRCo == emp.PRCo).ToList();
        //    var prEmpList = db.Employees.Where(f => f.PRCo == emp.PRCo).ToList();
        //    var assetList = db.HRCompanyAssets.Where(f => f.HRCo == emp.PRCo && f.AssetCategory.ToLower() == "credit" && f.Manufacturer.ToLower() == "zion").ToList();
        //    var accountList = db.GLAccounts.Where(f => f.GLCo == emp.PRCo).ToList();
        //    var jobList = db.Jobs.Where(f => f.JCCo == emp.PRCo).ToList();// JobRepository.GetJobs(emp.PRCo, false); 
        //    var equipmentList = db.Equipments.Where(f => f.EMCo == emp.PRCo).ToList();
        //    var phaseMasterList = db.PhaseMasters.Where(f => f.PhaseGroupId == emp.PRCo).ToList();


        //    var config = new MapperConfiguration(cfg => cfg.CreateMap<ZionCSVViewModel, ZionCSVViewModelExt>());
        //    var mapper = new Mapper(config);

            
        //    //var errorRecords = new List<ZionCSVViewModel>();
        //    var list = csv.GetRecords<ZionCSVViewModel>().ToList();
        //    var records = new List<ZionCSVViewModelExt>();
        //    var row = 0;
        //    foreach (var tran in list)
        //    {
        //        var item = mapper.Map<ZionCSVViewModel, ZionCSVViewModelExt>(tran);
        //        //var item = new ZionCSVViewModelExt(tran);
        //        if (item.TransactionType != "Payment")
        //        {
        //            //item.TransBillingAmount ??= 0;
        //            item.TransactionLineCodingJobNumber = JobLookup(jobList, item.TransactionLineCodingJobNumber);
        //            item.TransactionLineCodingTaskPhaseCode = PhaseLookup(phaseMasterList, item.TransactionLineCodingTaskPhaseCode);
        //            item.TransactionLineCodingEquipmentNumber = EquipmentLookup(equipmentList, item.TransactionLineCodingEquipmentNumber);
        //            item.TransactionLineCodingEquipmentCostCode = string.IsNullOrEmpty(item.TransactionLineCodingEquipmentCostCode) ? null : item.TransactionLineCodingEquipmentCostCode;
        //            item.TransactionLineCodingGLNumber = AccountLookup(accountList, item.TransactionLineCodingGLNumber);
        //            item.TransactionLineCodingGLNumber = GLLookUp(emp.PRCo, item, db);
        //            var itemEmp = empList.FirstOrDefault(f => f.CompanyEmail?.ToLower() == item.EmployeeEmail?.ToLower());
        //            if (itemEmp != null)
        //            {
        //                item.CardEmployeePRCo = itemEmp.PRCo;
        //                item.CardEmployeeId = itemEmp.HRRef;
        //                var prEmp = prEmpList.FirstOrDefault(f => f.EmployeeId == itemEmp.PREmp && f.PRCo == itemEmp.PRCo);
        //                item.PREmployee = prEmp;
        //            }
        //            else
        //            {
        //                var asset = assetList.FirstOrDefault(f => f.Identifier == item.CardEmployeeId.ToString());
        //                if (asset != null)
        //                {
        //                    item.CardEmployeeId = asset.Assigned ?? item.CardEmployeeId;
        //                    itemEmp = empList.FirstOrDefault(f => f.HRRef == item.CardEmployeeId);

        //                    item.CardEmployeePRCo = itemEmp?.PRCo;
        //                    //if (item.TransBillingAmount == 108.24m && item.CardEmployeeId == 100798)
        //                    //{
        //                    //    item.MerchantId = "37404693";
        //                    //    item.TransactionDescription = "Purchase Msft * E0300fkxky";
        //                    //    item.MerchantName = "Msft * E0300fkxky";
        //                    //}
        //                }
        //                else
        //                {
        //                    var prEmp = prEmpList.FirstOrDefault(f => f.ZionsCC == item.CardEmployeeId.ToString());
        //                    if (prEmp != null)
        //                    {
        //                        item.CardEmployeePRCo = prEmp.PRCo;
        //                        item.CardEmployeeId = prEmp.EmployeeId;
        //                    }
        //                    else
        //                    {
        //                        item.CardEmployeeId = null;
        //                    }

                            
        //                }
        //            }
        //            var empTest = prEmpList.FirstOrDefault(f => f.EmployeeId == item.CardEmployeeId);

        //            if (empTest == null)
        //            {
        //                errorRecords.Add(item);
        //            }
        //            else
        //            {
        //                records.Add(item);
        //            }
        //            var zion = new ZionImport {
        //                CCCo = import.Co,
        //                ImportId = import.ImportId,
        //                ImportLineId = row + 1,
        //                CompanyName = item.CompanyName,
        //                StatementStartDate = item.StatementStartDate,
        //                StatementEndDate = item.StatementEndDate,
        //                CardEmployeeId = item.CardEmployeeId,
        //                EmployeeFirstName = item.EmployeeFirstName,
        //                EmployeeLastName = item.EmployeeLastName,
        //                EmployeeEmail = item.EmployeeEmail,
        //                CardHolderFirstName = item.CardHolderFirstName,
        //                CardHolderLastName = item.CardHolderLastName,
        //                TransactionReference = item.TransactionReference,
        //                TransLineNumber = item.TransLineNumber,
        //                TransSequenceReference = item.TransSequenceReference,
        //                TransSequenceNumber = item.TransSequenceNumber,
        //                TransactionSecondDescription = item.TransactionSecondDescription,
        //                TransPostDate = item.TransPostDate,
        //                TransDate = item.TransDate,
        //                TransactionType = item.TransactionType,
        //                TransactionBillingCurrencyCode = item.TransactionBillingCurrencyCode,
        //                TransBillingAmount = item.TransBillingAmount,
        //                TransLineAmount = item.TransLineAmount,
        //                TransLineTaxAmount = item.TransLineTaxAmount,
        //                TransStatusId = item.TransStatusId,
        //                TransactionStatusDescription = item.TransactionStatusDescription,
        //                MerchantId = item.MerchantId,
        //                MerchantName = item.MerchantName,
        //                MerchantAddress = item.MerchantAddress,
        //                MerchantCity = item.MerchantCity,
        //                MerchantState = item.MerchantState,
        //                MerchantZip = item.MerchantZip,
        //                MerchantCountry = item.MerchantCountry,
        //                EmployeeVendorId = item.EmployeeVendorId,
        //                TransactionReceiptFlag = item.TransactionReceiptFlag,
        //                TransactionReceiptImageName = item.TransactionReceiptImageName,
        //                TransactionReceiptImageReferenceId = item.TransactionReceiptImageReferenceId,
        //                MerchantCategoryCode = item.MerchantCategoryCode,
        //                MerchantCategoryCodeDescription = item.MerchantCategoryCodeDescription,
        //                MerchantCategoryGroup = item.MerchantCategoryGroup,
        //                MerchantCategoryGroupDescription = item.MerchantCategoryGroupDescription,
        //                TransactionLineItemBillNumber = item.TransactionLineItemBillNumber,
        //                TransactionLineItemDescription = item.TransactionLineItemDescription,
        //                TransactionLineItemLastItem = item.TransactionLineItemLastItem,
        //                TransactionLineItemTotal = item.TransactionLineItemTotal,
        //                TransactionLineItemMessageIdDetail = item.TransactionLineItemMessageIdDetail,
        //                TransactionLineItemMessageIdSummary = item.TransactionLineItemMessageIdSummary,
        //                TransactionLineItemOrderDate = item.TransactionLineItemOrderDate,
        //                TransactionLineItemProductCode = item.TransactionLineItemProductCode,
        //                TransactionLineItemPurchaseId = item.TransactionLineItemPurchaseId,
        //                TransactionLineItemPurchaseType = item.TransactionLineItemPurchaseType,
        //                TransactionLineItemQuantity = item.TransactionLineItemQuantity,
        //                TransactionLineItemServiceIdentifierDetail = item.TransactionLineItemServiceIdentifierDetail,
        //                TransactionLineItemServiceIdentifierSummary = item.TransactionLineItemServiceIdentifierSummary,
        //                TransactionLineItemSourceCountryCode = item.TransactionLineItemSourceCountryCode,
        //                TransactionLineItemSourcePostalCode = item.TransactionLineItemSourcePostalCode,
        //                TransactionLineItemTaxAmount = item.TransactionLineItemTaxAmount,
        //                TransactionLineItemTaxRate = item.TransactionLineItemTaxRate,
        //                TransactionLineItemUnitofMeasure = item.TransactionLineItemUnitofMeasure,
        //                TransactionLineItemUnitPrice = item.TransactionLineItemUnitPrice,
        //                TransactionLineCodingJobNumber = item.TransactionLineCodingJobNumber,
        //                TransactionLineCodingTaskPhaseCode = item.TransactionLineCodingTaskPhaseCode,
        //                TransactionLineCodingJobCostType = item.TransactionLineCodingJobCostType,
        //                TransactionLineCodingEquipmentNumber = item.TransactionLineCodingEquipmentNumber,
        //                TransactionLineCodingEquipmentCostCode = item.TransactionLineCodingEquipmentCostCode,
        //                TransactionLineCodingEquipmentCostType = item.TransactionLineCodingEquipmentCostType,
        //                TransactionLineCodingGLNumber = item.TransactionLineCodingGLNumber,
        //            };
        //            import.ZionLines.Add(zion);
        //        }
        //        if (!string.IsNullOrEmpty(item.TransactionReceiptImageReferenceId))
        //        {
        //            //var img = DownloadImage(@"https://intellilink.spendmanagement.visa.com/ImageManagement/ImageLibrary/Image/" + item.TransactionReceiptImageReferenceId.Replace(".jpg", ""));
        //        }
        //        row++;
        //    }
        //    import.NumberofLines = records.Where(l => l.TransLineNumber != null).Count();

        //    empList = null;
        //    prEmpList = null;
        //    assetList = null;
        //    accountList = null;
        //    jobList = null;
        //    equipmentList = null;
        //    phaseMasterList = null;
        //    return records;
        //}

        //public static void AddMissingJobPhaseOrCost(List<ZionCSVViewModelExt> records, VPContext db)
        //{
        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var transJobList = records.Where(w => w.TransactionLineCodingJobNumber != null).GroupBy(g => g.TransactionLineCodingJobNumber).Select(s => s.Key).ToList();
        //    if (transJobList.Count == 0)
        //    {
        //        return;
        //    }
        //    var phaseMasterList = db.PhaseMasters.Where(f => f.PhaseGroupId == emp.PRCo).ToList();
        //    var phaseCostMasterList = db.PhaseMasterCosts.Where(f => f.PhaseGroupId == emp.PRCo).ToList();
        //    var jobList = JobRepository.GetJobs(emp.PRCo, false).Where(f => transJobList.Contains(f.JobId)).ToList();
        //    var jobPhaseList = JobPhaseRepository.GetJobPhases(emp.PRCo).Where(f => f.PhaseGroupId == emp.PRCo && transJobList.Contains(f.JobId)).ToList();
        //    var jobPhaseCostList = db.JobPhaseCosts.Where(f => f.PhaseGroupId == emp.PRCo && transJobList.Contains(f.JobId)).ToList();

        //    var newJobPhaseList = new List<JobPhase>();
        //    var newJobPhaseCostList = new List<JobPhaseCost>();
        //    foreach (var tran in records.GroupBy(g => new { g.TransactionLineCodingJobNumber, 
        //                                                    g.TransactionLineCodingTaskPhaseCode, 
        //                                                    g.TransactionLineCodingJobCostType })
        //                                .Select(s => new {
        //                                    s.Key.TransactionLineCodingJobNumber,
        //                                    s.Key.TransactionLineCodingTaskPhaseCode,
        //                                    s.Key.TransactionLineCodingJobCostType
        //                                }).ToList())
        //    {
        //        if (!string.IsNullOrEmpty(tran.TransactionLineCodingJobNumber) &&
        //            !string.IsNullOrEmpty(tran.TransactionLineCodingTaskPhaseCode) &&
        //             tran.TransactionLineCodingJobCostType != null)
        //        {
        //            var job = jobList.FirstOrDefault(f => f.JobId == tran.TransactionLineCodingJobNumber);
        //            var phase = jobPhaseList.FirstOrDefault(f => f.JobId == tran.TransactionLineCodingJobNumber && 
        //                                                         f.PhaseId.Trim() == tran.TransactionLineCodingTaskPhaseCode.Trim());
        //            var phaseMaster = phaseMasterList.FirstOrDefault(f => f.PhaseId.Trim() == tran.TransactionLineCodingTaskPhaseCode.Trim());
        //            if (phase == null)
        //            {
        //                phase = JobPhaseRepository.Init(phaseMaster, job);
        //                jobPhaseList.Add(phase);
        //                newJobPhaseList.Add(phase);
        //            }
        //            var cost = jobPhaseCostList.FirstOrDefault(f => f.PhaseId.Trim() == tran.TransactionLineCodingTaskPhaseCode.Trim() && f.CostTypeId == tran.TransactionLineCodingJobCostType);
        //            if (cost == null)
        //            {
        //                var phaseCostMaster = phaseCostMasterList.FirstOrDefault(f => f.PhaseId.Trim() == tran.TransactionLineCodingTaskPhaseCode.Trim() && f.CostTypeId == tran.TransactionLineCodingJobCostType);
        //                cost = JobPhaseCostRepository.Init(phaseCostMaster, job);
        //                jobPhaseCostList.Add(cost);
        //                newJobPhaseCostList.Add(cost);
        //            }
        //        }
        //    }
        //    db.JobPhases.AddRange(newJobPhaseList);
        //    db.JobPhaseCosts.AddRange(newJobPhaseCostList);
        //}

        //public static void ImportMerchant(List<ZionCSVViewModelExt> records, VPContext db)
        //{
        //    if (records == null) throw new System.ArgumentNullException(nameof(records));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));

        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var merchantGroupList = db.CreditMerchantGroups.Where(f => f.VendGroupId == emp.PRCo).ToList();
        //    var merchantCatList = db.CreditMerchantCategories.Where(f => f.VendGroupId == emp.PRCo).ToList();
        //    var merchantList = db.CreditMerchants.Where(f => f.VendGroupId == emp.PRCo).ToList();

        //    var groups = records.Where(w => w.MerchantCategoryGroup != null && w.MerchantCategoryGroup != "")
        //                        .GroupBy(g => new { g.MerchantCategoryGroup, g.MerchantCategoryGroupDescription })
        //                        .Select(s => new CreditMerchantGroup
        //                        {
        //                            VendGroupId = emp.PRCo,
        //                            CategoryGroup = s.Key.MerchantCategoryGroup,
        //                            Description = s.Key.MerchantCategoryGroupDescription,

        //                        }).ToList();
        //    var newMerchantGroupList = new List<CreditMerchantGroup>();
        //    var newMerchantCatList = new List<CreditMerchantCategory>();
        //    var newMerchantList = new List<CreditMerchant>();
        //    foreach (var group in groups)
        //    {
        //        var dbGroup = merchantGroupList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup);
        //        if (dbGroup == null)
        //            dbGroup = newMerchantGroupList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup);

        //        if (dbGroup == null)
        //            newMerchantGroupList.Add(group);

        //        var categories = records.Where(w => w.MerchantCategoryCode != null && w.MerchantCategoryGroup == group.CategoryGroup)
        //                                .GroupBy(cat => new { cat.MerchantCategoryCode, cat.MerchantCategoryCodeDescription })
        //                                .Select(cat => new CreditMerchantCategory
        //                                {
        //                                    VendGroupId = emp.PRCo,
        //                                    CategoryGroup = group.CategoryGroup,
        //                                    CategoryCodeId = cat.Key.MerchantCategoryCode ?? 0,
        //                                    Description = cat.Key.MerchantCategoryCodeDescription,
        //                                }).ToList();

        //        foreach (var cat in categories)
        //        {
        //            var dbCat = merchantCatList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup && f.CategoryCodeId == cat.CategoryCodeId);
        //            if (dbCat == null)
        //                dbCat = newMerchantCatList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup && f.CategoryCodeId == cat.CategoryCodeId);

        //            if (dbCat == null)
        //                newMerchantCatList.Add(cat);
        //            else
        //            {

        //            }
        //            var merchants = records.Where(w => w.MerchantId != null &&
        //                                               w.MerchantId != "" &&
        //                                               w.MerchantCategoryGroup == cat.CategoryGroup &&
        //                                               w.MerchantCategoryCode == cat.CategoryCodeId)
        //                            .GroupBy(merchant => new { merchant.MerchantId })
        //                            .Select(merchant => new CreditMerchant
        //                            {
        //                                VendGroupId = emp.PRCo,
        //                                MerchantId = merchant.Key.MerchantId,
        //                                CategoryGroup = cat.CategoryGroup,
        //                                CategoryCodeId = cat.CategoryCodeId,
        //                                Name = merchant.Max(max => max.MerchantName),
        //                                Address = merchant.Max(max => max.MerchantAddress),
        //                                City = merchant.Max(max => max.MerchantCity),
        //                                State = merchant.Max(max => max.MerchantState),
        //                                Zip = merchant.Max(max => max.MerchantZip),
        //                                CountryCode = merchant.Max(max => max.MerchantCountry),
        //                            }).ToList();
        //            foreach (var merchant in merchants)
        //            {
        //                var dbMerchant = merchantList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId &&
        //                                                                        f.MerchantId == merchant.MerchantId);
        //                if (dbMerchant == null)
        //                    dbMerchant = newMerchantList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId &&
        //                                                                        f.MerchantId == merchant.MerchantId);
        //                if (dbMerchant == null)
        //                    newMerchantList.Add(merchant);
        //                else
        //                {
        //                    //dbMerchant.Co = emp.HRCo;
        //                    //dbMerchant.MerchantId = merchant.MerchantId;
        //                    dbMerchant.CategoryGroup = merchant.CategoryGroup;
        //                    dbMerchant.CategoryCodeId = merchant.CategoryCodeId;
        //                    dbMerchant.Name = merchant.Name;
        //                    dbMerchant.Address = merchant.Address;
        //                    dbMerchant.City = merchant.City;
        //                    dbMerchant.State = merchant.State;
        //                    dbMerchant.Zip = merchant.Zip;
        //                    dbMerchant.CountryCode = merchant.CountryCode;
        //                }
        //            }
        //        }
        //    }
        //    db.CreditMerchantGroups.AddRange(newMerchantGroupList);
        //    db.CreditMerchantCategories.AddRange(newMerchantCatList);
        //    db.CreditMerchants.AddRange(newMerchantList);
        //}

        //public static List<CreditTransaction> ImportTransaction(List<ZionCSVViewModelExt> records, CreditImport import, VPContext db)
        //{
        //    if (records == null) throw new System.ArgumentNullException(nameof(records));
        //    if (import == null) throw new System.ArgumentNullException(nameof(import));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));

        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var minTransDate = records.Min(min => min.TransDate);
        //    var maxTransDate = records.Max(max => max.TransDate);
        //    var transList = db.CreditTransactions
        //        .Include("Coding")
        //        .Where(f => f.TransDate >= minTransDate && f.TransDate <= maxTransDate && f.CCCo == emp.PRCo)
        //        .ToList();
            
        //    var trans = records
        //        .GroupBy(g => new { g.TransactionReference })
        //        .Select(s => new CreditTransaction
        //        {
        //            CCCo = emp.PRCo,
        //            Mth = s.Max(max => max.StatementStartDate ?? DateTime.MinValue),
        //            UniqueTransId = s.Key.TransactionReference,
        //            APRef = s.Key.TransactionReference.Remove(0,8),
        //            MerchantId = s.Max(max => max.MerchantId),
        //            NewDescription = s.Max(max => max.TransactionDescription),
        //            TransDate = s.Max(max => max.TransDate ?? DateTime.MinValue),
        //            PostDate = s.Max(max => max.TransPostDate ?? DateTime.MinValue),
        //            PRCo = s.Max(max => max.CardEmployeePRCo) ?? emp.PRCo,
        //            EmployeeId = s.Max(max => max.CardEmployeeId) ?? 0,
        //            Employee = s.FirstOrDefault().PREmployee,
        //            EmployeeEmail = s.Max(max => max.EmployeeEmail),
        //            TransAmt = s.Max(max => max.TransBillingAmount) ?? 0,
        //            OrigDescription = s.Max(max => max.TransactionDescription),
        //            ImportId = import.ImportId,
        //            Lines = s.Where(f => !string.IsNullOrEmpty(f.TransactionLineItemProductCode))
        //                     .GroupBy(g => new { g.TransactionLineItemProductCode })
        //                     .Select(line => new CreditTransactionLine { 
        //                        CCCo = emp.PRCo,
        //                        LineAmount = line.Sum(sum => sum.TransactionLineItemTotal),
        //                        OrigDescription = line.Max(max => max.TransactionLineItemDescription),
        //                        ProductCode = line.Key.TransactionLineItemProductCode,
        //                        Quantity = line.Sum(sum => sum.TransactionLineItemQuantity) ?? 1,
        //                        UnitCost = line.Max(max => max.TransactionLineItemUnitPrice) ?? 0,
        //                        UM = line.Max(max => max.TransactionLineItemUnitofMeasure),
        //                        UniqueTransId = s.Key.TransactionReference



        //                     }).ToList(),
        //            Coding = s.Where(f => f.TransLineNumber != null)
        //                      .GroupBy(g => new {   g.TransactionLineCodingJobNumber,
        //                                            g.TransactionLineCodingTaskPhaseCode,
        //                                            g.TransactionLineCodingJobCostType,
        //                                            g.TransactionLineCodingEquipmentNumber,
        //                                            g.TransactionLineCodingEquipmentCostCode,
        //                                            g.TransactionLineCodingEquipmentCostType,
        //                                            g.TransactionLineCodingGLNumber,
        //                                            g.TransLineNumber })
        //                      .Select(coding => new CreditTransactionCode
        //                      {
        //                          CCCo = emp.PRCo,
        //                          SeqId = (int)coding.Key.TransLineNumber,
        //                          tLineTypeId = (!string.IsNullOrEmpty(coding.Key.TransactionLineCodingJobNumber) ? (byte?)DB.APLineTypeEnum.Job :
        //                                        !string.IsNullOrEmpty(coding.Key.TransactionLineCodingEquipmentNumber) ? (byte?)DB.APLineTypeEnum.Equipment :
        //                                        !string.IsNullOrEmpty(coding.Key.TransactionLineCodingGLNumber) ? (byte?)DB.APLineTypeEnum.Expense :
        //                                        null),
        //                          JCCo = emp.PRCo,
        //                          tJobId = coding.Key.TransactionLineCodingJobNumber,
        //                          PhaseGroupId = emp.PRCo,
        //                          tPhaseId = coding.Key.TransactionLineCodingTaskPhaseCode,
        //                          tJCCType = coding.Key.TransactionLineCodingJobCostType,
        //                          EMCo = emp.PRCo,
        //                          tEquipmentId = coding.Key.TransactionLineCodingEquipmentNumber,
        //                          EMGroupId = emp.PRCo,
        //                          tCostCodeId = coding.Key.TransactionLineCodingEquipmentCostCode,
        //                          tEMCType = coding.Key.TransactionLineCodingEquipmentCostType,
        //                          GLCo = emp.PRCo,
        //                          tGLAcct = coding.Key.TransactionLineCodingGLNumber,
        //                          GrossAmt = coding.Max(max => max.TransLineAmount) ?? 0,

        //                      }).ToList(),
        //        }).ToList();

        //    var newTransList = new List<CreditTransaction>();
        //    var newTransCodeList = new List<CreditTransactionCode>();
        //    var newTransLineList = new List<CreditTransactionLine>();
        //    var transId = db.CreditTransactions.DefaultIfEmpty().Max(f => f == null ? 0 : f.TransId) + 1;
        //    //trans = trans.Where(f => f.UniqueTransId == "H140020210310wvgvuumwm").ToList();
        //    foreach (var tran in trans)
        //    {
        //        //tran.CodedStatusId = (int)(tran.Coding.Any() ? DB.CMTransCodeStatusEnum.Coded : DB.CMTransCodeStatusEnum.New);
        //        tran.TransStatusId = 0;
        //        if (tran.Coding.Where(f => f.tGLAcct != null).Count() == tran.Coding.Count && tran.Coding.Any())
        //        {
        //            tran.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Coded;
        //        }
        //        else
        //        {
        //            tran.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Empty;
        //        }
        //        var dbtran = transList.FirstOrDefault(f => f.UniqueTransId == tran.UniqueTransId);
        //        //var imgId = 1;
        //        if (dbtran == null)
        //        {
        //            dbtran = newTransList.FirstOrDefault(f => f.UniqueTransId == tran.UniqueTransId);
        //        }
        //        if (dbtran == null)
        //        {
        //            tran.TransId = transId;
        //            foreach (var code in tran.Coding)
        //            {
        //                code.TransId = tran.TransId;
        //            }
        //            var seqId = 1;
        //            foreach (var line in tran.Lines)
        //            {
        //                line.TransId = tran.TransId;
        //                line.UniqueTransId = tran.UniqueTransId;
        //                line.SeqId = seqId;
        //                seqId++;
        //            }
        //            if (tran.Coding.Where(f => f.tGLAcct != null).Count() == tran.Coding.Count && tran.Coding.Any())
        //            {
        //                if (tran.CodedStatusId != (int)DB.CMTransCodeStatusEnum.AutoCoded)
        //                {
        //                    tran.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Coded;
        //                }
        //            }

        //            //if (tran.EmployeeId == 100798 )
        //            //{
        //            //    if (tran.TransAmt == 108.24m)
        //            //    {
        //            //        tran.MerchantId = "37404693";
        //            //        tran.OrigDescription = "Purchase Msft * E0300fkxky";
        //            //        //tran.ApprovalStatusId = (int)DB.CMApprovalStatusEnum.SupervisorApproved;
        //            //        tran.PictureStatusId = (int)DB.CMPictureStatusEnum.NotNeeded;
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //}
        //            tran.Logs.Add(CreditCardTransactionLogRepository.Init(tran, DB.CMLogEnum.Added));
        //            newTransList.Add(tran);
        //            transId++;
        //        }
        //        else
        //        {
        //            tran.TransId = dbtran.TransId;
        //            dbtran.TransStatusId ??= 0;

        //            if (dbtran.OrigDescription != tran.OrigDescription)
        //            {
        //                dbtran.NewDescription = tran.OrigDescription;
        //            }
        //            if (dbtran.TransAmt != tran.TransAmt)
        //            {
        //                dbtran.TransAmt = tran.TransAmt;
        //            }
        //            //Set seqId to the first seq id in the coding table.  03/22/2021 Glen Lewis
        //            var seqId = dbtran.Coding.DefaultIfEmpty().Min(f => f == null ? 1 : f.SeqId);

        //            foreach (var code in tran.Coding)
        //            {
        //                code.SeqId = seqId;
        //                seqId++;
        //            }

        //            seqId = dbtran.Coding.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1;
        //            foreach (var code in tran.Coding)
        //            {
        //                code.TransId = dbtran.TransId;
        //                var dbCode = dbtran.Coding.FirstOrDefault(f => f.SeqId == code.SeqId);
        //                if (dbCode == null)
        //                {
        //                    code.SeqId = seqId;
        //                    seqId++;
        //                    newTransCodeList.Add(code);
        //                    dbtran.Coding.Add(code);
        //                }
        //                else
        //                {
        //                    if (dbCode.tJobId != code.tJobId && dbCode.tJobId == null && code.tJobId != null)
        //                        dbCode.tJobId = code.tJobId;
                            
        //                    if (dbCode.tPhaseId != code.tPhaseId && dbCode.tPhaseId == null && code.tPhaseId != null)
        //                        dbCode.tPhaseId = code.tPhaseId;
                            
        //                    if (dbCode.tJCCType != code.tJCCType && dbCode.tJCCType == null && code.tJCCType != null)
        //                        dbCode.tJCCType = code.tJCCType;

        //                    if (dbCode.tEquipmentId != code.tEquipmentId && dbCode.tEquipmentId == null && code.tEquipmentId != null)
        //                        dbCode.tEquipmentId = code.tEquipmentId;

        //                    if (dbCode.tCostCodeId != code.tCostCodeId && dbCode.tCostCodeId == null && code.tCostCodeId != null)
        //                        dbCode.tCostCodeId = code.tCostCodeId;

        //                    if (dbCode.tEMCType != code.tEMCType && dbCode.tEMCType == null && code.tEMCType != null)
        //                        dbCode.tEMCType = code.tEMCType;

        //                    if (dbCode.tGLAcct != code.tGLAcct && dbCode.tGLAcct == null && code.tGLAcct != null)
        //                        dbCode.tGLAcct = code.tGLAcct;
        //                }

        //            }
        //            seqId = 1;
        //            foreach (var line in tran.Lines)
        //            {
        //                line.SeqId = seqId;
        //                seqId++;
        //            }

        //            seqId = dbtran.Lines.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1;
        //            foreach (var line in tran.Lines)
        //            {
        //                var dbLine = dbtran.Lines.FirstOrDefault(f => f.SeqId == line.SeqId);
        //                if (dbLine == null)
        //                    dbLine = newTransLineList.FirstOrDefault(f => f.SeqId == line.SeqId);
        //                if (dbLine == null)
        //                {
        //                    line.SeqId = seqId;
        //                    line.TransId = dbtran.TransId;
        //                    seqId++;
        //                    newTransLineList.Add(line);
        //                    dbtran.Lines.Add(line);
        //                }
        //                else 
        //                {
        //                    dbLine.LineAmount = line.LineAmount;
        //                    dbLine.OrigDescription = line.OrigDescription;
        //                    dbLine.ProductCode = line.ProductCode;
        //                    dbLine.Quantity = line.Quantity;
        //                    dbLine.UnitCost = line.UnitCost;
        //                    dbLine.UM = line.UM;
        //                }
        //            }
        //            if (dbtran.Coding.Where(f => f.tGLAcct != null).Count() == dbtran.Coding.Count && dbtran.Coding.Any())
        //            {
        //                if (dbtran.CodedStatusId == (int)DB.CMTransCodeStatusEnum.Empty)
        //                {
        //                    tran.Logs.Add(CreditCardTransactionLogRepository.Init(tran, DB.CMLogEnum.Coded));
        //                    dbtran.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Coded;
        //                }
        //            }
        //        }
        //    }
        //    db.CreditTransactions.AddRange(newTransList);
        //    db.CreditTransactionCodes.AddRange(newTransCodeList);
        //    db.budCMTL.AddRange(newTransLineList);
        //    import.LinesAdded = newTransList.Count;
        //    import.LinesSkipped = import.NumberofLines - import.LinesAdded;

        //    return trans;
        //}

        //public static string AccountLookup(List<GLAccount> accounts, string GLAccount)
        //{
        //    if (string.IsNullOrEmpty(GLAccount))
        //    {
        //        return null;
        //    }
        //    GLAccount = GLAccount.Trim();
        //    var dbAccount = accounts.FirstOrDefault(f => f.GLAcct.Trim() == GLAccount);
        //    if (dbAccount == null)
        //    {
        //        dbAccount = accounts.FirstOrDefault(f => f.GLAcct.Trim().Replace("-", "").Trim() == GLAccount);
        //        var dbGlaccount = accounts.FirstOrDefault().GLAcct.Trim().Replace("-", "");
        //    }
        //    if (dbAccount == null)
        //    {
        //        GLAccount = GLAccount.Replace("-", "");
        //        dbAccount = accounts.FirstOrDefault(f => f.GLAcct.Trim().Replace("-", "").Trim() == GLAccount);
        //    }
        //    if (dbAccount != null)
        //    {
        //        return dbAccount.GLAcct;
        //    }
        //    return null;
        //}

        //public static string JobLookup(List<Job> Jobs, string JobId)
        //{
        //    if (string.IsNullOrEmpty(JobId))
        //    {
        //        return null;
        //    }
        //    JobId = JobId.Trim();
        //    var dbJob = Jobs.FirstOrDefault(f => f.JobId.Trim() == JobId);
        //    if (dbJob == null)
        //    {
        //        dbJob = Jobs.FirstOrDefault(f => f.JobId.Trim().Replace("-", "") == JobId);
        //    }
        //    if (dbJob == null)
        //    {
        //        JobId = JobId.Replace("-", "");
        //        dbJob = Jobs.FirstOrDefault(f => f.JobId.Trim().Replace("-", "") == JobId);
        //    }
        //    if (dbJob != null)
        //    {
        //        return dbJob.JobId;
        //    }
        //    return null;
        //}

        //public static string PhaseLookup(List<PhaseMaster> Phases, string PhaseId)
        //{
        //    if (string.IsNullOrEmpty(PhaseId))
        //    {
        //        return null;
        //    }
        //    PhaseId = PhaseId.Trim();
        //    var dbPhase = Phases.FirstOrDefault(f => f.PhaseId.Trim() == PhaseId);
        //    if (dbPhase == null)
        //    {
        //        dbPhase = Phases.FirstOrDefault(f => f.PhaseId.Trim().Replace("-", "").Trim() == PhaseId);
        //    }
        //    if (dbPhase == null)
        //    {
        //        PhaseId = PhaseId.Replace("-", "");
        //        dbPhase = Phases.FirstOrDefault(f => f.PhaseId.Trim().Replace("-", "").Trim() == PhaseId);
        //    }
        //    if (dbPhase != null)
        //    {
        //        return dbPhase.PhaseId;
        //    }
        //    return null;
        //}

        //public static string EquipmentLookup(List<Equipment> Equipments, string EquipmentId)
        //{
        //    if (EquipmentId == null)
        //    {
        //        return null;
        //    }
        //    EquipmentId = EquipmentId.Trim();
        //    var dbEquipment = Equipments.FirstOrDefault(f => f.EquipmentId.Trim() == EquipmentId);
        //    if (dbEquipment == null)
        //    {
        //        dbEquipment = Equipments.FirstOrDefault(f => f.EquipmentId.Trim() == EquipmentId);
        //    }
        //    if (dbEquipment != null)
        //    {
        //        return dbEquipment.EquipmentId;
        //    }
        //    return null;
        //}

        //public static string GLLookUp(CreditTransactionCode code, VPContext db)
        //{
            
        //    if (code.tLineTypeId == (byte)DB.APLineTypeEnum.Job && code.tJobId != null && code.tPhaseId != null && code.tJCCType != null)
        //    {

        //        var glParm = new ObjectParameter("glacct", typeof(string));
        //        var msgParm = new ObjectParameter("msg", typeof(string));
        //        var glErr = db.bspJCCAGlacctDflt(code.JCCo, code.tJobId, code.PhaseGroupId, code.tPhaseId, code.tJCCType, "N", glParm, msgParm);
        //        if (glErr == -1)
        //        {
        //            return (string)glParm.Value;
        //        }
        //    }
        //    else if (code.tLineTypeId == (byte)DB.APLineTypeEnum.Equipment && code.tEquipmentId != null && code.tEMCType != null && code.tCostCodeId != null)
        //    {
        //        var glParm = new ObjectParameter("gltransacct", typeof(string));
        //        var glORParm = new ObjectParameter("GLOverride", typeof(string));
        //        var msgParm = new ObjectParameter("msg", typeof(string));
        //        var glErr = db.bspEMGlacctDflt(code.EMCo, code.EMGroupId, code.tEMCType, code.tCostCodeId, code.tEquipmentId, glParm, glORParm, msgParm);
        //        if (glErr == -1)
        //        {
        //            return (string)glParm.Value;
        //        }
        //    }
        //    return code.tGLAcct;
        //}
        
        //public static string GLLookUp(byte Co, ZionCSVViewModel trans, VPContext db)
        //{


        //    if (trans.TransactionLineCodingJobNumber != null && 
        //        trans.TransactionLineCodingTaskPhaseCode != null && 
        //        trans.TransactionLineCodingJobCostType != null)
        //    {
        //        return AccountRepository.GetDefaultJobGL(Co, trans.TransactionLineCodingJobNumber, trans.TransactionLineCodingTaskPhaseCode, (byte)trans.TransactionLineCodingJobCostType);
        //        //var glParm = new ObjectParameter("glacct", typeof(string));
        //        //var msgParm = new ObjectParameter("msg", typeof(string));
        //        //var glErr = db.bspJCCAGlacctDflt(Co, trans.TransactionLineCodingJobNumber, Co, trans.TransactionLineCodingTaskPhaseCode, trans.TransactionLineCodingJobCostType, "N", glParm, msgParm);
        //        //if (glErr == -1)
        //        //{
        //        //    return (string)glParm.Value;
        //        //}
        //    }
        //    else if (trans.TransactionLineCodingEquipmentNumber != null && 
        //             trans.TransactionLineCodingEquipmentCostCode != null && 
        //             trans.TransactionLineCodingEquipmentCostType != null)
        //    {
        //        return AccountRepository.GetDefaultEquipmentGL(Co, trans.TransactionLineCodingEquipmentNumber, trans.TransactionLineCodingEquipmentCostCode, (byte)trans.TransactionLineCodingEquipmentCostType);
        //        //var glParm = new ObjectParameter("gltransacct", typeof(string));
        //        //var glORParm = new ObjectParameter("GLOverride", typeof(string));
        //        //var msgParm = new ObjectParameter("msg", typeof(string));
        //        //var glErr = db.bspEMGlacctDflt(Co, Co, trans.TransactionLineCodingEquipmentCostType, trans.TransactionLineCodingEquipmentCostCode, trans.TransactionLineCodingEquipmentNumber, glParm, glORParm, msgParm);
        //        //if (glErr == -1)
        //        //{
        //        //    return (string)glParm.Value;
        //        //}
        //    }
        //    return trans.TransactionLineCodingGLNumber;
        //}

        //public static void ImportMerchantV2(List<ZionCSVViewModelExt> records, VPContext db)
        //{
        //    if (records == null) throw new System.ArgumentNullException(nameof(records));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));

        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var merchantGroupList = db.CreditMerchantGroups.Where(f => f.VendGroupId == emp.PRCo).ToList();
        //    var merchantCatList = db.CreditMerchantCategories.Where(f => f.VendGroupId == emp.PRCo).ToList();
        //    var merchantList = db.CreditMerchants.Where(f => f.VendGroupId == emp.PRCo).ToList();
        //    var vendorList = db.APVendors.Where(f => f.VendorGroupId == emp.PRCo && f.MerchantMatchString != null).ToList();
        //    var matchList = vendorList.Select(s => new { s.VendorGroupId, s.VendorId, matchList = s.MerchantMatchString.Split(';').ToList()})
        //                              .ToList();
        //    var apParms = db.APCompanyParms.FirstOrDefault(f => f.APCo == emp.PRCo);
        //    var dataSet = new List<dynamic>();
        //    foreach (var item in matchList)
        //    {
        //        foreach (var match in item.matchList)
        //        {
        //            dataSet.Add(new
        //            {
        //                VendorGroup = item.VendorGroupId,
        //                VendorId = item.VendorId,
        //                MatchString = match.Trim()
        //            });
        //        }
        //    }
        //    var APParms = db.APCompanyParms.FirstOrDefault(f => f.APCo == emp.PRCo);

        //    var groups = records.Where(w => !string.IsNullOrEmpty(w.MerchantId))
        //                        .GroupBy(g => new { g.MerchantCategoryGroup, g.MerchantCategoryGroupDescription })
        //                        .Select(s => new CreditMerchantGroup
        //                        {
        //                            VendGroupId = emp.PRCo,
        //                            CategoryGroup = s.Key.MerchantCategoryGroup,
        //                            Description = s.Key.MerchantCategoryGroupDescription,
        //                            Categories = s.GroupBy(cat => new { cat.MerchantCategoryCode, cat.MerchantCategoryCodeDescription })
        //                                            .Select(cat => new CreditMerchantCategory
        //                                            {
        //                                                VendGroupId = emp.PRCo,
        //                                                CategoryGroup = s.Key.MerchantCategoryGroup,
        //                                                CategoryCodeId = cat.Key.MerchantCategoryCode ?? 0,
        //                                                Description = cat.Key.MerchantCategoryCodeDescription,
        //                                                Merchants = cat.GroupBy(merchant => new { merchant.MerchantId })
        //                                                                .Select(merchant => new CreditMerchant
        //                                                                {
        //                                                                    VendGroupId = emp.PRCo,
        //                                                                    MerchantId = merchant.Key.MerchantId,
        //                                                                    CategoryGroup = s.Key.MerchantCategoryGroup,
        //                                                                    CategoryCodeId = cat.Key.MerchantCategoryCode,
        //                                                                    Name = merchant.Max(max => max.MerchantName),
        //                                                                    Address = merchant.Max(max => max.MerchantAddress),
        //                                                                    City = merchant.Max(max => max.MerchantCity),
        //                                                                    State = merchant.Max(max => max.MerchantState),
        //                                                                    Zip = merchant.Max(max => max.MerchantZip),
        //                                                                    VendorId =  APParms.udDftMerchantVendorId,
        //                                                                    CountryCode = merchant.Max(max => max.MerchantCountry),
        //                                                                    IsReceiptRequired = true,
        //                                                                    IsReoccurring = false
        //                                                                }).ToList()
        //                                            }).ToList()
        //                        }).ToList();
        //    var newMerchantGroupList = new List<CreditMerchantGroup>();
        //    var newMerchantCatList = new List<CreditMerchantCategory>();
        //    var newMerchantList = new List<CreditMerchant>();
        //    foreach (var group in groups)
        //    {
        //        var dbGroup = merchantGroupList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup);
        //        if (dbGroup == null)
        //            dbGroup = newMerchantGroupList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup);

        //        if (dbGroup == null)
        //            newMerchantGroupList.Add(group);
        //        else
        //        {
        //            foreach (var cat in group.Categories)
        //            {
        //                var dbCat = merchantCatList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup && f.CategoryCodeId == cat.CategoryCodeId);
        //                if (dbCat == null)
        //                    dbCat = newMerchantCatList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.CategoryGroup == group.CategoryGroup && f.CategoryCodeId == cat.CategoryCodeId);

        //                if (dbCat == null)
        //                    newMerchantCatList.Add(cat);
        //                else
        //                {
                        
        //                    foreach (var merchant in cat.Merchants)
        //                    {
        //                        if (merchant.VendorId == APParms.udDftMerchantVendorId)
        //                        {

        //                            var match = dataSet.FirstOrDefault(f => merchant.Name.ToLower(AppCultureInfo.CInfo()).Contains(f.MatchString.ToLower(AppCultureInfo.CInfo())));
        //                            if (match != null)
        //                            {
        //                                var vendor = vendorList.FirstOrDefault(f => f.VendorGroupId == match.VendorGroup && f.VendorId == match.VendorId);
        //                                merchant.VendorId = vendor.VendorId;
        //                                var address = vendor.AltAddresses.FirstOrDefault(f => f.MerchantId == merchant.MerchantId);
        //                                if (address == null && vendor.VendorId != apParms.udDftMerchantVendorId && vendor.AltAddresses.Count < 250)
        //                                {
        //                                    address = new VendorAltAddress
        //                                    {
        //                                        VendorGroupId = vendor.VendorGroupId,
        //                                        VendorId = vendor.VendorId,
        //                                        AddressSeq = (byte)(vendor.AltAddresses.DefaultIfEmpty().Max(f => f == null ? 0 : f.AddressSeq) + 1),
        //                                        Type = 1,
        //                                        Description = merchant.Name,
        //                                        Address = merchant.Address,
        //                                        City = merchant.City,
        //                                        State = merchant.State,
        //                                        Zip = merchant.Zip,
        //                                        Notes = "Auto Added by CC",
        //                                        MerchantId = merchant.MerchantId
        //                                    };
        //                                    vendor.AltAddresses.Add(address);
        //                                }
        //                            }
        //                        }
        //                        var dbMerchant = merchantList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.MerchantId == merchant.MerchantId);
        //                        if (dbMerchant == null)
        //                            dbMerchant = newMerchantList.FirstOrDefault(f => f.VendGroupId == group.VendGroupId && f.MerchantId == merchant.MerchantId);
        //                        if (dbMerchant == null)
        //                            newMerchantList.Add(merchant);
        //                        else
        //                        {
        //                            if (dbMerchant.VendorId == APParms.udDftMerchantVendorId && merchant.VendorId != dbMerchant.VendorId)
        //                            {
        //                                dbMerchant.VendorId = (int)merchant.VendorId;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    db.CreditMerchantGroups.AddRange(newMerchantGroupList);
        //    db.CreditMerchantCategories.AddRange(newMerchantCatList);
        //    db.CreditMerchants.AddRange(newMerchantList);
            
        //    merchantGroupList = null;
        //    merchantCatList = null;
        //    merchantList = null;
        //    vendorList = null;
        //    matchList = null;
        //    apParms = null;
        //    dataSet = null;
        //}

        //public static void ImportImageBank(List<ZionCSVViewModel> records, List<CreditTransaction> transactions, VPContext db)
        //{
        //    if (records == null) throw new System.ArgumentNullException(nameof(records));
        //    if (transactions == null) throw new System.ArgumentNullException(nameof(transactions));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));

        //    var emp = StaticFunctions.GetCurrentEmployee();
        //    var imageList = db.CreditCardImages.ToList();
        //    var imageLinkList = db.CreditImageLinks.ToList();
        //    var images = records.Where(f => !string.IsNullOrEmpty(f.TransactionReference) && 
        //                                    !string.IsNullOrEmpty(f.TransactionReceiptImageReferenceId))
        //                        .GroupBy(g => new { g.CardEmployeeId, g.TransactionReceiptImageReferenceId })
        //                        .Select(image => new CreditCardImage {
        //                            CCCo = emp.PRCo,
        //                            Mth = image.Max(max => max.StatementStartDate ?? DateTime.MinValue),
        //                            EmployeeId = (int)image.Key.CardEmployeeId,
        //                            ReceiptImageRefId = image.Key.TransactionReceiptImageReferenceId,
        //                            ImageName = image.Max(max => max.TransactionReceiptImageName),
        //                            CreatedBy = "System",
        //                            CreatedOn = image.Max(max => max.TransDate ?? DateTime.MinValue),
        //                            Transactions = image.GroupBy(g => g.TransactionReference)
        //                                                .Select(link => new CreditImageLink
        //                            {
        //                                CCCo = emp.PRCo,
        //                                Mth = image.Max(max => max.StatementStartDate ?? DateTime.MinValue),
        //                                EmployeeId = (int)image.Key.CardEmployeeId,     
        //                                UniqueTransId = link.Key,
        //                                TaggedDate = image.Max(max => max.StatementStartDate ?? DateTime.MinValue)
        //                            }).ToList()

        //                        });            

        //    var imageId = db.CreditCardImages.DefaultIfEmpty().Max(f => f == null ? 0 : f.ImageId) + 1;
        //    var newImageList = new List<CreditCardImage>();
        //    var newImageLinkList = new List<CreditImageLink>();

        //    foreach (var image in images)
        //    {
        //        var tranid = image.Transactions.FirstOrDefault().UniqueTransId;
        //        var tran = transactions.FirstOrDefault(f => f.UniqueTransId == tranid);
        //        if (image.Mth == DateTime.MinValue)
        //        {
        //            image.Mth = tran.Mth;
        //            image.CreatedOn = tran.PostDate;
        //        }
        //        var dbimage = imageList.FirstOrDefault(f => f.ReceiptImageRefId == image.ReceiptImageRefId);
        //        //var imgId = 1;
        //        if (dbimage == null)
        //        {
        //            dbimage = newImageList.FirstOrDefault(f => f.ReceiptImageRefId == image.ReceiptImageRefId);
        //        }
        //        if (dbimage == null)
        //        {
        //            image.ImageId = imageId;
        //            var linkId = 1;
        //            foreach (var link in image.Transactions)
        //            {
        //                tran = transactions.FirstOrDefault(f => f.UniqueTransId == link.UniqueTransId);
        //                link.TransId = tran.TransId;
        //                link.Mth = tran.Mth;
        //                link.ImageId = imageId;
        //                link.LinkId = linkId;
        //                link.TaggedDate = tran.PostDate;
        //                linkId++;
        //                transactions.FirstOrDefault(f => f.TransId == link.TransId).PictureStatusId = (int)DB.CMPictureStatusEnum.Attached;
        //            }
        //            newImageList.Add(image);
        //            imageId++;
        //        }
        //        else
        //        {
        //            var linkId = dbimage.Transactions.DefaultIfEmpty().Max(f => f == null ? 0 : f.LinkId) + 1;
        //            foreach (var link in image.Transactions)
        //            {
        //                tran = transactions.FirstOrDefault(f => f.UniqueTransId == link.UniqueTransId);
        //                link.ImageId = dbimage.ImageId;
        //                link.TransId = tran.TransId;
        //                link.Mth = tran.Mth;
        //                link.TaggedDate = tran.PostDate;
        //                var dbLink = imageLinkList.FirstOrDefault(f => f.CCCo == link.CCCo &&
        //                                                               f.EmployeeId == link.EmployeeId &&
        //                                                               f.Mth == link.Mth &&
        //                                                               f.ImageId == link.ImageId &&
        //                                                               f.TransId == link.TransId);
        //                if (dbLink == null)
        //                    dbLink = newImageLinkList.FirstOrDefault(f => f.CCCo == link.CCCo &&
        //                                                               f.EmployeeId == link.EmployeeId &&
        //                                                               f.Mth == link.Mth &&
        //                                                               f.ImageId == link.ImageId &&
        //                                                               f.TransId == link.TransId);
        //                if (dbLink == null)
        //                {
        //                    link.LinkId = linkId;
        //                    linkId++;
        //                    newImageLinkList.Add(link);
        //                    transactions.FirstOrDefault(f => f.TransId == link.TransId).PictureStatusId = (int)DB.CMPictureStatusEnum.Attached;
        //                }

        //            }
        //        }
        //    }
        //    db.CreditCardImages.AddRange(newImageList);
        //    db.CreditImageLinks.AddRange(newImageLinkList);
        //}
        
        //public static void AutoCodeTransaction(List<CreditTransaction> transactions, VPContext db)
        //{
        //    if (transactions == null) throw new System.ArgumentNullException(nameof(transactions));
        //    if (db == null) throw new System.ArgumentNullException(nameof(db));
        //    foreach (var tran in transactions)
        //    {
        //        var merchant = tran.Merchant;
        //        var vendor = tran.Merchant.Vendor;
        //        var category = tran.Merchant.Category;
        //        var group = tran.Merchant.Category.Group;

        //        var glAccount = ((merchant.DefaultGLAcct ?? vendor?.DefaultGLAcct) ?? category.DefaultGLAcct) ?? group.DefaultGLAcct;
        //        var phaseId = ((merchant.DefaultJCPhaseId ?? vendor?.DefaultJCPhaseId) ?? category.DefaultJCPhaseId) ?? group.DefaultJCPhaseId;
        //        var jobCostTypeId = ((merchant.DefaultJCCType ?? vendor?.DefaultJCCType) ?? category.DefaultJCCType) ?? group.DefaultJCCType;
        //        var costCodeId = ((merchant.DefaultEMCostCodeId ?? vendor?.DefaultEMCostCodeId) ?? category.DefaultEMCostCodeId) ?? group.DefaultEMCostCodeId;
        //        var eqpCostTypeId = ((merchant.DefaultEMCType ?? vendor?.DefaultEMCType) ?? category.DefaultEMCType) ?? group.DefaultEMCType;

        //        var minDate = tran.TransDate.AddDays(-2);
        //        var maxDate = tran.TransDate.AddDays(0);

        //        var dailyJobList = db.DTPayrollHours.Where(f => f.EmployeeId == tran.EmployeeId & f.WorkDate >= minDate && f.WorkDate <= maxDate)
        //                                       .GroupBy(g => new { g.WorkDate, g.Job })
        //                                       .Select(s => new { s.Key.WorkDate, s.Key.Job })
        //                                       .ToList();
        //        var dailyEqpList = db.DTPayrollHours.Where(f => f.EmployeeId == tran.EmployeeId & f.WorkDate >= minDate && f.WorkDate <= maxDate)
        //                                       .GroupBy(g => new { g.WorkDate, g.Equipment })
        //                                       .Select(s => new { s.Key.WorkDate, s.Key.Equipment })
        //                                       .ToList();
        //        var coding = tran.Coding.FirstOrDefault();
        //        if (dailyJobList.Any())
        //        {
        //            coding.tLineTypeId = (byte)DB.APLineTypeEnum.Job;
        //            coding.tJobId = dailyJobList.OrderByDescending(o => o.WorkDate).FirstOrDefault().Job.JobId;
        //            coding.tPhaseId = phaseId;
        //            coding.tJCCType = jobCostTypeId;
        //            //coding.
        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        public static void ProcessUpdate(Models.Views.AP.CreditCard.Form.InfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditTransactions.FirstOrDefault(f => f.CCCo == model.CCCo && f.TransId == model.TransId);
            if (updObj != null)
            {
                updObj.NewDescription = model.Description;
                updObj.APRef = model.APRef;
            }

        }

        public static void ProcessUpdate(Models.Views.AP.CreditCard.Form.MerchantInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            if (updObj != null)
            {
                updObj.VendorId = model.VendorId;
            }

        }

        public static void ProcessUpdate(Models.Views.AP.CreditCard.Form.TransactionApprovalViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditTransactions.FirstOrDefault(f => f.CCCo == model.CCCo && f.TransId == model.TransId);
            if (updObj != null)
            {
                updObj.ApprovalStatusId = (int)model.ApprovalStatusId;
                updObj.Logs.Add(CreditCardTransactionLogRepository.Init(updObj, DB.CMLogEnum.Approved, string.Format(AppCultureInfo.CInfo(), "Transaction was {0}", model.ApprovalStatusId)));
            }

        }
    }
}
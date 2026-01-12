using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class CreditImport
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public List<CreditTransaction> CreditTransactions { get; set; }

        public List<HRResource> HREmployees { get; set; }

        public List<Employee> PREmployees { get; set; }

        public List<Equipment> EMEquipments { get; set; }

        public List<CreditMerchantGroup> MerchantGroups { get; set; }

        public List<HRCompanyAsset> CompanyAssets { get; set; }

        public List<HRBenefitCode> HRBenefitCodes { get; set; }

        public static CreditImport CreateImport(HttpPostedFileBase fileUpload, VPEntities db)
        {
            if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var co = StaticFunctions.GetCurrentCompany().HQCo;
            var hqParm = db.HQCompanyParms                
                .Include("MatlGroup")
                .Include("MatlGroup.HQMaterialCategories")
                .Include("MatlGroup.HQMaterialCategories.Materials")
                .FirstOrDefault(f => f.HQCo == co);
            var import = new CreditImport
            {
                Co = hqParm.HQCo,
                ImportId = GetNextImportId(),
                CreatedBy = StaticFunctions.GetUserId(),
                CreatedOn = DateTime.Now,
                FileName = fileUpload.FileName,
                LinesAdded = 0,
                LinesSkipped = 0,
                db = db,
                HQCompanyParm = hqParm,
            };
            import.ImportCsvFile(fileUpload);
            if (import.Source != null)
            {
                db.CreditImports.Add(import);
            }

            return import;
        }

        private void ImportCsvFile(HttpPostedFileBase fileUpload)
        {
            if (fileUpload == null) throw new System.ArgumentNullException(nameof(fileUpload));

            using var streamReader = new StreamReader(fileUpload.InputStream);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Configuration.HeaderValidated = null;
            csv.Configuration.MissingFieldFound = null;
            csv.Read();
            var csvHeader = csv.ReadHeader();
            var headerRow = csv.Context.HeaderRecord.ToList();

            if (headerRow.Any(f => f == "Transaction - Transaction Reference"))
            {
                Source = "Zion";
                var list = csv.GetRecords<Models.Views.AP.CreditCard.ZionCSVViewModel>().ToList();
                list.ForEach(e => this.AddLine(e));
                ProcessZionLines();
            }
            else if (headerRow.Any(f => f == "VIN"))
            {
                Source = "Wex";
                var list = csv.GetRecords<Models.Views.AP.CreditCard.WexCSVViewModel>().ToList();
                list.ForEach(e => this.AddLine(e));
                ProcessWexLines();
            }
            else if (headerRow.Any(f => f == "TAG_NUMBER/PLATE_NUMBER"))
            {
                Source = "TXTag";
                var list = csv.GetRecords<Models.Views.AP.CreditCard.TXTagCSVViewModel>().ToList();
                list.ForEach(e => this.AddLine(e));
                ProcessTXTagLines();
            }
            else if (headerRow.Any(f => f == "Benefits Eligibility Profile"))
            {
                Source = "PRBenefit";
                var list = csv.GetRecords<Models.Views.PR.Import.BenefitsCSVViewModel>().ToList();
                list.ForEach(e => this.AddLine(e));
                ProcessPRBenefitLines();
            }
        }

        public ZionImport AddLine(Models.Views.AP.CreditCard.ZionCSVViewModel item)
        {
            if (item.TransactionType == "Payment")
                return null;
            
            var zion = new Code.Data.VP.ZionImport
            {
                CCCo = Co,
                ImportId = ImportId,
                ImportLineId = this.ZionLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.ImportLineId) + 1,                
                EmployeeEmail = item.EmployeeEmail?.ToLower(),
                CompanyName = item.CompanyName,
                StatementStartDate = item.StatementStartDate,
                StatementEndDate = item.StatementEndDate,
                CardEmployeeId = item.CardEmployeeId,
                EmployeeFirstName = item.EmployeeFirstName,
                EmployeeLastName = item.EmployeeLastName,
                CardHolderFirstName = item.CardHolderFirstName,
                CardHolderLastName = item.CardHolderLastName,
                TransactionReference = item.TransactionReference,
                TransLineNumber = item.TransLineNumber,
                TransSequenceReference = item.TransSequenceReference,
                TransSequenceNumber = item.TransSequenceNumber,
                TransactionSecondDescription = item.TransactionSecondDescription,
                TransPostDate = item.TransPostDate,
                TransDate = item.TransDate,
                TransactionType = item.TransactionType,
                TransactionBillingCurrencyCode = item.TransactionBillingCurrencyCode,
                TransBillingAmount = item.TransBillingAmount,
                TransLineAmount = item.TransLineAmount,
                TransLineTaxAmount = item.TransLineTaxAmount,
                TransStatusId = item.TransStatusId,
                TransactionStatusDescription = item.TransactionStatusDescription,
                MerchantId = item.MerchantId,
                MerchantName = item.MerchantName,
                MerchantAddress = item.MerchantAddress,
                MerchantCity = item.MerchantCity,
                MerchantState = item.MerchantState,
                MerchantZip = item.MerchantZip,
                MerchantCountry = item.MerchantCountry,
                EmployeeVendorId = item.EmployeeVendorId,
                TransactionReceiptFlag = item.TransactionReceiptFlag,
                TransactionReceiptImageName = item.TransactionReceiptImageName,
                TransactionReceiptImageReferenceId = item.TransactionReceiptImageReferenceId,
                MerchantCategoryCode = item.MerchantCategoryCode,
                MerchantCategoryCodeDescription = item.MerchantCategoryCodeDescription,
                MerchantCategoryGroup = item.MerchantCategoryGroup,
                MerchantCategoryGroupDescription = item.MerchantCategoryGroupDescription,
                TransactionLineItemBillNumber = item.TransactionLineItemBillNumber,
                TransactionLineItemDescription = item.TransactionLineItemDescription,
                TransactionLineItemLastItem = item.TransactionLineItemLastItem,
                TransactionLineItemTotal = item.TransactionLineItemTotal,
                TransactionLineItemMessageIdDetail = item.TransactionLineItemMessageIdDetail,
                TransactionLineItemMessageIdSummary = item.TransactionLineItemMessageIdSummary,
                TransactionLineItemOrderDate = item.TransactionLineItemOrderDate,
                TransactionLineItemProductCode = item.TransactionLineItemProductCode,
                TransactionLineItemPurchaseId = item.TransactionLineItemPurchaseId,
                TransactionLineItemPurchaseType = item.TransactionLineItemPurchaseType,
                TransactionLineItemQuantity = item.TransactionLineItemQuantity,
                TransactionLineItemServiceIdentifierDetail = item.TransactionLineItemServiceIdentifierDetail,
                TransactionLineItemServiceIdentifierSummary = item.TransactionLineItemServiceIdentifierSummary,
                TransactionLineItemSourceCountryCode = item.TransactionLineItemSourceCountryCode,
                TransactionLineItemSourcePostalCode = item.TransactionLineItemSourcePostalCode,
                TransactionLineItemTaxAmount = item.TransactionLineItemTaxAmount,
                TransactionLineItemTaxRate = item.TransactionLineItemTaxRate,
                TransactionLineItemUnitofMeasure = item.TransactionLineItemUnitofMeasure,
                TransactionLineItemUnitPrice = item.TransactionLineItemUnitPrice,
                TransactionLineCodingJobNumber = item.TransactionLineCodingJobNumber,
                TransactionLineCodingTaskPhaseCode = item.TransactionLineCodingTaskPhaseCode,
                TransactionLineCodingJobCostType = item.TransactionLineCodingJobCostType,
                TransactionLineCodingEquipmentNumber = item.TransactionLineCodingEquipmentNumber,
                TransactionLineCodingEquipmentCostCode = item.TransactionLineCodingEquipmentCostCode,
                TransactionLineCodingEquipmentCostType = item.TransactionLineCodingEquipmentCostType,
                TransactionLineCodingGLNumber = item.TransactionLineCodingGLNumber,
                TransactionDescription = item.TransactionDescription,


                IsError = false,
                db = db,
                Import = this,
            };
            this.ZionLines.Add(zion);
            return zion;
        }

        public CCWexImport AddLine(Models.Views.AP.CreditCard.WexCSVViewModel item)
        {
            var line = new Code.Data.VP.CCWexImport
            {
                CMCo = Co,
                ImportId = ImportId,
                ImportLineId = this.WexLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.ImportLineId) + 1,
                TransactionDate = item.TransactionDate,
                TransactionTime = item.TransactionTime,
                PostDate = item.PostDate,
                AccountNumber = item.AccountNumber,
                AccountName = item.AccountName,
                CardNumber = item.CardNumber,
                EmbossLine2 = item.EmbossLine2,
                CustomVehicleAssetId = item.CustomVehicleAssetId,
                Units = item.Units,
                UnitofMeasure = item.UnitofMeasure,
                UnitCost = item.UnitCost,
                TotalFuelCost = item.TotalFuelCost,
                ServiceCost = item.ServiceCost,
                OtherCost = item.OtherCost,
                TotalNonFuelCost = item.TotalNonFuelCost,
                GrossCost = item.GrossCost,
                ExemptTax = item.ExemptTax,
                Discount = item.Discount,
                NetCost = item.NetCost,
                ReportedTax = item.ReportedTax,
                TransactionFeeType1 = item.TransactionFeeType1,
                TransactionFeeAmount1 = item.TransactionFeeAmount1,
                TransactionFeeType2 = item.TransactionFeeType2,
                TransactionFeeAmount2 = item.TransactionFeeAmount2,
                TransactionFeeType3 = item.TransactionFeeType3,
                TransactionFeeAmount3 = item.TransactionFeeAmount3,
                TransactionFeeType4 = item.TransactionFeeType4,
                TransactionFeeAmount4 = item.TransactionFeeAmount4,
                TransactionFeeType5 = item.TransactionFeeType5,
                TransactionFeeAmount5 = item.TransactionFeeAmount5,
                Product = item.Product,
                ProductDescription = item.ProductDescription,
                TransactionDescription = item.TransactionDescription,
                MerchantBrand = item.MerchantBrand,
                MerchantName = item.MerchantName,
                MerchantAddress = item.MerchantAddress,
                MerchantCity = item.MerchantCity,
                MerchantStateProvince = item.MerchantStateProvince,
                MerchantPostalCode = item.MerchantPostalCode,
                MerchantSiteId = item.MerchantSiteId,
                CurrentOdometer = item.CurrentOdometer,
                AdjustedOdometer = item.AdjustedOdometer,
                PreviousOdometer = item.PreviousOdometer,
                DistanceDriven = item.DistanceDriven,
                FuelEconomy = item.FuelEconomy,
                CostPerDistance = item.CostPerDistance,
                VehicleDescription = item.VehicleDescription,
                VIN = item.VIN,
                TankCapacity = item.TankCapacity,
                InServiceDate = item.InServiceDate,
                StartOdometer = item.StartOdometer,
                DriverLastName = item.DriverLastName,
                DriverFirstName = item.DriverFirstName,
                DriverMiddleName = item.DriverMiddleName,
                DriverDepartment = item.DriverDepartment,
                EmployeeID = item.EmployeeID,
                TransactionTicketNumber = item.TransactionTicketNumber,
                CurrencyExchangeRate = item.CurrencyExchangeRate,
                RebateCode = item.RebateCode,
                DriverPromptId = item.DriverPromptId,
                VehiclePromptId = item.VehiclePromptId,
                Department = item.Department,
                TankCapacity2 = item.TankCapacity2,
                IsError = false,
                db = db,
                Import = this,
            };

            this.WexLines.Add(line);
            return line;
        }

        public ImportTexTag AddLine(Models.Views.AP.CreditCard.TXTagCSVViewModel item)
        {

            var line = new Code.Data.VP.ImportTexTag
            {
                CMCo = Co,
                ImportId = ImportId,
                ImportLineId = this.TXTagLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.ImportLineId) + 1,
                TransactionNumber = item.TransactionNumber,
                Location = item.Location.Trim(),
                TagNumber = item.TagNumber.Trim(),
                TransType = item.TransType.Trim(),
                AmountStr = item.AmountStr.Trim(),
                TransDateStr = item.TransDateStr,
                
                IsError = false,
                db = db,
                Import = this,
            };
            line.TransDate = line.TransDateTime;
            line.Amount = line.AmountConv();

            this.TXTagLines.Add(line);
            return line;
        }

        public PRImportBenefit AddLine(Models.Views.PR.Import.BenefitsCSVViewModel item)
        {

            var line = new PRImportBenefit
            {
                Co = Co,
                ImportId = ImportId,
                ImportLineId = this.PRImportBenefits.DefaultIfEmpty().Max(max => max == null ? 0 : max.ImportLineId) + 1,
               
                EECode = item.EECode,
                EEName = item.EEName,
                BenefitsEligibilityProfile = item.BenefitsEligibilityProfile,
                HomeAllocation = item.HomeAllocation,
                PlanYear = item.PlanYear,
                PlanName = item.PlanName,
                CoverageLevel = item.CoverageLevel,
                BenefitStatus = item.BenefitStatus,
                CoverageStartDate = item.CoverageStartDate,
                CoverageEndDate = item.CoverageEndDate,
                MonthlyPremium = decimal.TryParse(item.MonthlyPremium, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal outdec1) ? outdec1 : 0,
                EmployerCostPerPayPeriod = decimal.TryParse(item.EmployerCostPerPayPeriod, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal outdec2) ? outdec2 : 0,
                EmployeeCostPerPayPeriod = decimal.TryParse(item.EmployeeCostPerPayPeriod, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal outdec3) ? outdec3 : 0,
                DeductionCode = item.DeductionCode,
                TaxTreatment = item.TaxTreatment,
                CurrentPayrollDeduction = decimal.TryParse(item.CurrentPayrollDeduction, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal outdec4) ? outdec4 : 0,
                //PRCo = item.PRCo,
                //PREmployeeId = item.PREmployeeId,
                //HRCo = item.HRCo,
                //HRBenefitCodeId = item.HRBenefitCodeId,
                //BenefitGroupId = item.BenefitGroupId,
                //IsError = item.IsError,
                IsError = false,
                db = db,
                Import = this,
            };

            this.PRImportBenefits.Add(line);
            return line;
        }
        
        public static int GetNextImportId()
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

            var result = db.udNextId("budCMIP", 1, outParm);

            return (int)outParm.Value;
        }

        public void ProcessZionLines()
        {
            if (!this.ZionLines.Any())
                return;

            MerchantGroups = db.CreditMerchantGroups
                .Include("Categories")
                .Include("Categories.Merchants")
                .Include("Categories.Merchants.Vendor")
                //.Include(i => i.Categories.SelectMany(c => c.Merchants.Select(m => m.Vendor)))
                .ToList();
            HREmployees = db.HRResources
                .Where(f => f.HRRef <= 200000)
                .Include(pr => pr.PREmployee)
                .ToList();
            CompanyAssets = db.HRCompanyAssets.Where(f => f.HRCo == this.HQCompanyParm.HRCo && f.AssetCategory.ToLower() == "cc" && f.Manufacturer.ToLower() == "zion").ToList();

            this.ZionLines.ToList().ForEach(e => {
                if (e.TransactionDescription.Contains("Xsolla") || e.MerchantId == "37456630")
                {
                    e.TransactionDescription = "Purchase Msft * E0300fkxky";
                    e.MerchantId = "41369544";
                    e.SetMerchant();
                }
            });

            //Fill in missing email, merchantId and card id for the trans lines
            this.ZionLines.ToList().ForEach(e => {
                if (string.IsNullOrEmpty(e.EmployeeEmail))
                {
                    var tran = this.ZionLines.FirstOrDefault(f => f.TransactionReference == e.TransactionReference && !string.IsNullOrEmpty(f.EmployeeEmail));
                    if (tran != null)
                    {
                        e.EmployeeEmail = tran.EmployeeEmail;
                        e.CardEmployeeId = tran.CardEmployeeId;
                    }
                }
                if (e.CardEmployeeId == null)
                {
                    var tran = this.ZionLines.FirstOrDefault(f => f.TransactionReference == e.TransactionReference && f.CardEmployeeId != null);
                    if (tran != null)
                        e.CardEmployeeId = tran.CardEmployeeId;
                }
                if (string.IsNullOrEmpty(e.MerchantId))
                {
                    var tran = this.ZionLines.FirstOrDefault(f => f.TransactionReference == e.TransactionReference && !string.IsNullOrEmpty(e.MerchantId));
                    if (tran != null)
                        e.MerchantId = tran.MerchantId;
                }
            });


            this.ZionLines.ToList().ForEach(e => {
                e.SetEmployee();
                if (!string.IsNullOrEmpty(e.MerchantCategoryGroup) && e.MerchantCategoryCode != null && !string.IsNullOrEmpty(e.MerchantId))
                    e.GetMerchant();
            });

            ErrorLines = this.ZionLines.Where(f => f.IsError == true).Count();
            NumberofLines = this.ZionLines.Count;
        }

        public void ProcessWexLines()
        {
            if (!this.WexLines.Any())
                return;

            MerchantGroups = db.CreditMerchantGroups
               .Include("Categories")
               .Include("Categories.Merchants")
               .Include("Categories.Merchants.Vendor")
               .ToList();
            HREmployees = db.HRResources
                .Where(f => f.HRRef <= 200000)
                .Include(pr => pr.PREmployee)
                .ToList();
            CompanyAssets = db.HRCompanyAssets.Where(f => f.HRCo == this.HQCompanyParm.HRCo && f.AssetCategory.ToLower() == "cc" && f.Manufacturer.ToLower() == "wex").ToList();
            EMEquipments = db.Equipments.ToList();
            
            this.WexLines.ToList().ForEach(e => {
                e.IsError = false;
                e.SetEmployee();
                e.SetEquipment();
                e.SetMerchant();
                e.UniqueTransId = e.CalcUniqueTransId;
            });
            NumberofLines = this.WexLines.Count;
            ErrorLines = this.WexLines.Where(f => f.IsError == true).Count();
        }

        public void ProcessTXTagLines()
        {
            if (!this.TXTagLines.Any())
                return;

            EMEquipments = db.Equipments.ToList();

            this.TXTagLines.ToList().ForEach(e => {
                e.IsError = false;
                //e.SetEmployee();
                e.SetEquipment();
                //e.SetMerchant();
            });
            NumberofLines = this.TXTagLines.Count;
        }


        public void ProcessPRBenefitLines()
        {
            if (!this.PRImportBenefits.Any())
                return;

            PREmployees = db.Employees.ToList();
            HRBenefitCodes = db.HRBenefitCodes.ToList();

            this.PRImportBenefits.ToList().ForEach(e => {
                e.IsError = false;
                e.SetEmployee();
                e.SetBenefitCode();
                //e.SetMerchant();
            });

            NumberofLines = this.PRImportBenefits.Count;
        }

        public void ProcessTransactions()
        {
            MerchantGroups = db.CreditMerchantGroups
               .Include("Categories")
               .Include("Categories.Merchants")
               .Include("Categories.Merchants.Vendor")
               .ToList();

            if (Source == "Wex")
            {
                var minTransDate = WexLines.Min(min => min.TransactionDate).Value.AddDays(-1);
                var maxTransDate = WexLines.Max(max => max.TransactionDate).Value.AddDays(1);

                CreditTransactions = db.CreditTransactions                    
                    .Include(i => i.Coding)
                    .Include(i => i.Lines)                    
                    .Where(f => f.TransDate >= minTransDate && f.TransDate <= maxTransDate && f.CCCo == this.Co && f.Source == this.Source)
                    .ToList();


                var i = 0;
                this.WexLines.ToList().ForEach(e => {
                    if (e.IsError == false)
                    {
                        var trans = e.AddCreditTransaction();
                        if (trans != null)
                        {
                            var transline = trans.AddCreditTransactionLine(e);
                            var transcode = trans.AddTransactionCode(e);
                        }
                        trans.TransAmt = trans.Coding.Sum(sum => sum.GrossAmt ?? 0);
                    }
                    i++;
                });

                var newTrans = CreditTransactions.Where(f => f.TransId == 0).ToList();
                this.NumberofLines = WexLines.Count;
                this.LinesAdded = 0;
                if (newTrans.Any())
                {
                    var transId = CreditTransaction.GetNextTransId(newTrans.Count);
                    foreach (var item in newTrans)
                    {
                        item.TransId = transId;
                        item.Coding.ToList().ForEach(e => e.TransId = transId);
                        item.Lines.ToList().ForEach(e => e.TransId = transId);
                        item.APRef = item.TransId.ToString();
                        transId++;
                    }
                    this.LinesAdded = newTrans.Count;
                    db.CreditTransactions.AddRange(newTrans);

                }
                this.LinesSkipped = NumberofLines - LinesAdded;
            }
            else if (Source == "Zion")
            {
                var minTransDate = ZionLines.Min(min => min.TransDate);
                var maxTransDate = ZionLines.Max(max => max.TransDate);
                CompanyAssets = db.HRCompanyAssets.Where(f => f.AssetCategory == "CC").ToList();
                CreditTransactions = db.CreditTransactions
                    .Include(i => i.Coding)
                    .Include(i => i.Lines)
                    .Where(f => f.TransDate >= minTransDate && f.TransDate <= maxTransDate && f.CCCo == this.Co && f.Source == this.Source)
                    .ToList();


                var i = 0;
                this.ZionLines.ToList().ForEach(e => {
                    if (e.IsError == false)
                    {
                        var lines = this.ZionLines.Where(f => f.TransactionReference == e.TransactionReference).ToList();
                        var trans = e.AddCreditTransaction(lines);
                        if (trans != null)
                        {
                            
                            var transline = trans.AddCreditTransactionLine(e);
                            trans.AutoCode();
                            trans.AutoPictureStatus();
                            trans.TransAmt = trans.Coding.Sum(sum => sum.GrossAmt ?? 0);
                        }
                    }
                    i++;
                });

                var newTrans = CreditTransactions.Where(f => f.TransId == 0).ToList();
                var importTrans = CreditTransactions.Where(f => f.ImportId == this.ImportId).ToList();
                this.NumberofLines = ZionLines.Count;
                this.LinesAdded = importTrans.Count;
                if (newTrans.Any())
                {
                    var transId = CreditTransaction.GetNextTransId(newTrans.Count);
                    foreach (var item in newTrans)
                    {
                        item.TransId = transId;
                        item.Coding.ToList().ForEach(e => e.TransId = transId);
                        item.Lines.ToList().ForEach(e => e.TransId = transId);
                        transId++;
                    }
                    db.CreditTransactions.AddRange(newTrans);
                }
                //Run vendor match on new transactions
                var vendorList = db.APVendors.Where(f => !string.IsNullOrEmpty(f.MerchantMatchString)).ToList();
                foreach (var vendor in vendorList)
                {
                    vendor.RunMerchantMatch(importTrans);
                }
                this.LinesSkipped = NumberofLines - LinesAdded;
            }
            else if (Source == "PRBenefit")
            {
                var i = 0;
                this.PRImportBenefits.ToList().ForEach(e => {
                    if (e.IsError == false)
                    {
                        e.PREmployee.AddBenefit(e);
                    }
                    i++;
                });
            }
        }
    }
}
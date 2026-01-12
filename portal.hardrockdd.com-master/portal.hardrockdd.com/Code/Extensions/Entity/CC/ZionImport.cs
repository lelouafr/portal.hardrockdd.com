using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class ZionImport
    {
        public VPEntities _db;

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
                        _db = this.Import.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public void SetEmployee()
        {
            //if (this.PREmployee != null)
            //    return;
            IsError = false;
            var itemEmp = Import.HREmployees.FirstOrDefault(f => f.CompanyEmail?.ToLower() == EmployeeEmail);
            //if (itemEmp == null)
            //{
            //    itemEmp = Import.HREmployees.FirstOrDefault(f => f.AssignedAssets.Any(f => f.DateIn == null && f.Asset.AssetCategory == "CC" && f.Asset.Identifier == this.CardEmployeeId.ToString()));
            //}
            //if (CardEmployeeId == 325)
            //{
            //    var i = 0;
            //}
            if (itemEmp != null)
            {
                PRCo = itemEmp.PRCo;
                PREmployeeId = itemEmp.HRRef;
                PREmployee = itemEmp.PREmployee;
            }
            else if(CardEmployeeId != null)
            {
                var import = this.Import;
                var asset = Import.CompanyAssets.FirstOrDefault(f => f.Identifier == CardEmployeeId.ToString());
                if (asset != null && asset?.tAssignedId != null)
                {
                    itemEmp = Import.HREmployees.FirstOrDefault(f => f.HRRef == asset.tAssignedId);
                    if (itemEmp != null)
                    {
                        PRCo = itemEmp?.PRCo;
                        PREmployeeId = itemEmp?.HRRef;
                        PREmployee = itemEmp?.PREmployee;
                    }
                }
                else
                {
                    var prEmp = Import.HREmployees.FirstOrDefault(f => f.PREmployee?.ZionsCC == CardEmployeeId.ToString());
                    if (prEmp != null)
                    {
                        PRCo = prEmp.PRCo;
                        PREmployeeId = prEmp.HRRef;
                        PREmployee = prEmp.PREmployee;
                    }
                }
            }

            if (PREmployee == null)
                IsError = true;
        }

        public CreditMerchantGroup GetMerchantGroup()
        {
            var group = Import.MerchantGroups.FirstOrDefault(f => f.VendGroupId == (byte)Import.HQCompanyParm.VendorGroupId && f.CategoryGroup == this.MerchantCategoryGroup);
            if (group == null)
            {
                group = new CreditMerchantGroup()
                {
                    VendGroupId = (byte)Import.HQCompanyParm.VendorGroupId,
                    CategoryGroup = MerchantCategoryGroup,
                    Description = MerchantCategoryGroupDescription,
                };
                db.CreditMerchantGroups.Add(group);
                Import.MerchantGroups.Add(group);
            }

            return group;
        }

        public CreditMerchantCategory GetMerchantCategory()
        {
            var group = GetMerchantGroup();
            var category = group.Categories.FirstOrDefault(f => f.CategoryCodeId == MerchantCategoryCode);

            if (category == null)
            {
                category = new CreditMerchantCategory()
                {
                    VendGroupId = group.VendGroupId,
                    CategoryGroup = group.CategoryGroup,
                    CategoryCodeId = MerchantCategoryCode ?? 0,
                    Description = MerchantCategoryCodeDescription,
                    Group = group,
                };
                group.Categories.Add(category);
            }

            return category;
        }

        public CreditMerchant GetMerchant()
        {
            var category = GetMerchantCategory();
            var merchant = category.Merchants.FirstOrDefault(f => f.MerchantId == MerchantId);

            if (merchant == null)
            {
                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == Import.HQCompanyParm.APCompanyParm.udDftMerchantVendorId);
                merchant = new CreditMerchant()
                {
                    VendGroupId = category.VendGroupId,
                    CategoryGroup = category.CategoryGroup,
                    CategoryCodeId = category.CategoryCodeId,
                    MerchantId = MerchantId,
                    Name = MerchantName,
                    Address = MerchantAddress,
                    City = MerchantCity,
                    State = MerchantState,
                    Zip = MerchantZip,
                    VendorId = Import.HQCompanyParm.APCompanyParm.udDftMerchantVendorId,
                    CountryCode = MerchantCountry,
                    IsReceiptRequired = true,
                    IsReoccurring = false,
                    Category = category,
                    Vendor = vendor,
                };
                category.Merchants.Add(merchant);
            }

            return merchant;
        }

        public CreditMerchant SetMerchant()
        {
            var merchant = Import.MerchantGroups.SelectMany(group => group.Categories).SelectMany(cat => cat.Merchants).FirstOrDefault(f => f.MerchantId == MerchantId);
           

            if (merchant != null)
            {
                MerchantCategoryGroup = merchant.Category.Group.CategoryGroup;
                MerchantCategoryGroupDescription = merchant.Category.Group.Description;
                MerchantCategoryCode = merchant.Category.CategoryCodeId;
                MerchantCategoryCodeDescription = merchant.Category.Description;
                MerchantName = merchant.Name;
                MerchantAddress = merchant.Address;
                MerchantCity = merchant.City;
                MerchantZip = merchant.Zip;
                MerchantCountry = merchant.CountryCode;
                MerchantState = merchant.State;
            }

            return merchant;
        }

        public CreditTransaction AddCreditTransaction(List<ZionImport> lines)
        {
            if (!string.IsNullOrEmpty(TransactionReference) && StatementStartDate != null)
            {
                var transaction = Import.CreditTransactions.FirstOrDefault(f => f.CCCo == Import.HQCompanyParm.APCo && f.Mth == StatementStartDate && f.UniqueTransId == TransactionReference);

                if (transaction == null)
                {
                    transaction = new CreditTransaction()
                    {
                        CCCo = (byte)Import.HQCompanyParm.APCo,
                        Mth = StatementStartDate ?? DateTime.MinValue,
                        UniqueTransId = TransactionReference,
                        APRef = TransactionReference.Remove(0, 8),
                        MerchantId = MerchantId,
                        NewDescription = TransactionDescription,
                        TransDate = TransDate ?? DateTime.MinValue,
                        PostDate = TransPostDate ?? DateTime.MinValue,
                        PRCo = PRCo,
                        EmployeeId = (int)PREmployeeId,
                        Employee = PREmployee,
                        EmployeeEmail = EmployeeEmail,
                        TransAmt = lines.DefaultIfEmpty().Max(max => max == null ? 0 : TransBillingAmount) ?? 0,
                        OrigDescription = TransactionDescription,
                        ImportId = ImportId,

                        PictureStatusId = 0,
                        TransStatusId = 0,
                        CodedStatusId = (int)DB.CMTransCodeStatusEnum.Empty,
                        Source = Import.Source,                        
                        Import = this.Import,
                        db = db,
                        Merchant = Merchant,

                    };

                    Import.CreditTransactions.Add(transaction);
                }
                else
                {
                    if (transaction.EmployeeId != this.PREmployeeId)
                    {
                        transaction.PRCo = PRCo;
                        transaction.EmployeeId = (int)PREmployeeId;
                        transaction.Employee = PREmployee;
                    }
                    if (transaction.MerchantId != this.MerchantId)
                    {
                        transaction.MerchantId = MerchantId;
                        transaction.Merchant = Merchant;
                    }
                }

                return transaction;
            }
            return null;

        }
        public static int GetNextTransId()
        {
            using var db = new VPEntities();

            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("nextId", typeof(int));

            var result = db.udNextId("budCMTH", 1, outParm);

            return (int)outParm.Value;
        }
    }
}
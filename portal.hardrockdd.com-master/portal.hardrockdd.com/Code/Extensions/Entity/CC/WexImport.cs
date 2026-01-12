using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class CCWexImport
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
            if (this.PREmployee != null)
                return;

            var itemEmp = Import.HREmployees.FirstOrDefault(f => f.HRRef == EmployeeID);
            if (itemEmp != null)
            {
                PRCo = itemEmp.PRCo;
                PREmployeeId = itemEmp.HRRef;
                PREmployee = itemEmp.PREmployee;
            }
            else
            {
                //var asset = assetList.FirstOrDefault(f => f.Identifier == item.DriverPromptId.ToString());
                var asset = Import.CompanyAssets.FirstOrDefault(f => f.HRCo == this.Import.HQCompanyParm.HRCo && f.AssetCategory.ToLower() == "credit" && f.Manufacturer.ToLower() == "wex" && f.Identifier == DriverPromptId.ToString());
                if (asset != null)
                {
                    itemEmp = Import.HREmployees.FirstOrDefault(f => f.HRRef == asset.tAssignedId);

                    PRCo = itemEmp.PRCo;
                    PREmployeeId = itemEmp.HRRef;
                    PREmployee = itemEmp.PREmployee;
                }
            }

            if (PREmployee == null)
                IsError = true;
        }

        public void SetEquipment()
        {
            if (this.EMEquipment != null)
                return;

            var eqp = Import.EMEquipments.FirstOrDefault(f => f.EquipmentId.Trim() == CustomVehicleAssetId);
            if (eqp != null)
            {
                EMCo = eqp.EMCo;
                EMEquipmentId = eqp.EquipmentId;
                EMEquipment = eqp;
            }

            if (EMEquipment == null)
                IsError = true;
        }

        private string _UniqueTransId_OLD;
        public string UniqueTransId_OLD
        { 
            get
            {
                if (_UniqueTransId_OLD == null)
                {
                    //_UniqueTransId_OLD = string.Format("{0}_{1}_{2}", DriverPromptId, TransDateTime.ToShortDateString(), TransDateTime.ToShortTimeString());


                    var dt = new DateTime(TransDateTime.Year, TransDateTime.Month, TransDateTime.Day, TransDateTime.Hour, TransDateTime.Minute, 0);
                    _UniqueTransId_OLD = string.Format("{0}_{1}", DriverPromptId, dt.Ticks);
                }                
                return _UniqueTransId_OLD;
            }
        }

        private string _CalcUniqueTransId;
        public string CalcUniqueTransId
        {
            get
            {
                if (_CalcUniqueTransId == null)
                {
                    var dt = new DateTime(TransDateTime.Year, TransDateTime.Month, TransDateTime.Day, TransDateTime.Hour, TransDateTime.Minute, 0);
                    _CalcUniqueTransId = string.Format("{0}_{1}_{2}", DriverPromptId, this.MerchantId, dt.Ticks);
                }
                return _CalcUniqueTransId;
            }
        }
        private string _CalcLineUniqueTransId;
        public string CalcLineUniqueTransId
        {
            get
            {
                if (_CalcLineUniqueTransId == null)
                {
                    _CalcLineUniqueTransId = string.Format("{0}_{1}_{2}", CalcUniqueTransId, this.TransactionTicketNumber, this.Product);
                }
                return _CalcLineUniqueTransId;
            }
        }

        //private string _UniqueTransId_Old2;
        //public string UniqueTransId_Old2
        //{
        //    get
        //    {
        //        if (_UniqueTransId_Old2 == null)
        //        {
        //            _UniqueTransId_Old2 = string.Format("{0}_{1}", DriverPromptId, TransDateTime.Ticks);
        //        }
        //        return _UniqueTransId_Old2;
        //    }

        //}


        private DateTime? _TransDateTime;
        public DateTime TransDateTime 
        { 
            get
            {
                if (_TransDateTime == null )
                {
                    var dt = TransactionDate ?? DateTime.Now;
                    var t = TransactionTime ?? DateTime.MinValue;
                    TimeSpan time = new TimeSpan(0, t.Hour, t.Minute, t.Second, t.Millisecond);
                    _TransDateTime = dt.Add(time);
                }
                return (DateTime)_TransDateTime;
            }

        }

        private DateTime? _TransDateShort;
        public DateTime TransDateShort
        {
            get
            {
                if (_TransDateShort == null)
                {
                    _TransDateShort = new DateTime(TransDateTime.Year, TransDateTime.Month, TransDateTime.Day, TransDateTime.Hour, TransDateTime.Minute, TransDateTime.Second);
                    _TransDateShort = ((DateTime)_TransDateShort).RoundToNearest(TimeSpan.FromMinutes(1));
                }
                return (DateTime)_TransDateShort;
            }
        }

        public DateTime Mth 
        { 
            get
            {
                var dt = TransactionDate ?? DateTime.Now;
                return new DateTime(dt.Year, dt.Month, 1);
            }
        }

        public CreditMerchantGroup GetMerchantGroup()
        {
            var group = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == (byte)Import.HQCompanyParm.VendorGroupId && f.CategoryGroup == "AUTO_WEX");
            if (group == null)
                group = db.CreditMerchantGroups.Local.FirstOrDefault(f => f.VendGroupId == (byte)Import.HQCompanyParm.VendorGroupId && f.CategoryGroup == "AUTO_WEX");
            if (group == null)
            {
                group = new CreditMerchantGroup()
                {
                    VendGroupId = (byte)Import.HQCompanyParm.VendorGroupId,
                    CategoryGroup = "AUTO_WEX",
                    Description = "Auto/Vehicle WEX",
                };
                db.CreditMerchantGroups.Add(group);
                //Import.MerchantGroups.Add(group);
            }

            return group;
        }

        public CreditMerchantCategory GetMerchantCategory()
        {
            var group = GetMerchantGroup();
            var category = group.Categories.FirstOrDefault(f => f.CategoryCodeId == 5542);

            if (category == null)
            {
                category = new CreditMerchantCategory()
                {
                    VendGroupId = group.VendGroupId,
                    CategoryGroup = group.CategoryGroup,
                    CategoryCodeId = 5542,
                    Description = "Automated Fuel Dispensers",
                    Group = group,
                };
                group.Categories.Add(category);
            }

            return category;
        }

        public CreditMerchant SetMerchant()
        {
            var category = GetMerchantCategory();
            //var merchant = category.Merchants.FirstOrDefault(f => f.MerchantId == string.Format("WEX_{0}", MerchantSiteId));
            var merchant = category.Merchants.FirstOrDefault(f => f.Name == MerchantName && f.City == MerchantCity && f.State == MerchantStateProvince && f.Zip == MerchantPostalCode && f.Address == MerchantAddress);

            if (merchant == null)
            {
                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == 834);
                var merchants = category.Merchants.ToList();
                var merchantId = merchants.DefaultIfEmpty().Max(max => max == null ? 0 : (int.TryParse(max.MerchantId?.Replace("WEX_", ""), out int merchantIdOut) ? merchantIdOut : 0));
                merchantId++;

                merchant = new CreditMerchant()
                {
                    VendGroupId = category.VendGroupId,
                    CategoryGroup = category.CategoryGroup,
                    CategoryCodeId = category.CategoryCodeId,
                    MerchantId = string.Format("WEX_{0}", merchantId),
                    Name = MerchantName,
                    Address = MerchantAddress,
                    City = MerchantCity,
                    State = MerchantStateProvince,
                    Zip = MerchantPostalCode,
                    VendorId = vendor.VendorId,
                    //CountryCode = MerchantCountry,
                    IsReceiptRequired = false,
                    IsReoccurring = false,
                    Category = category,
                    Vendor = vendor,
                };
                category.Merchants.Add(merchant);
                //db.CreditMerchants.Add(merchant);
            }
            MerchantId = merchant.MerchantId;
            return merchant;
        }

        public CreditTransaction AddCreditTransaction()
        {
            if (TransactionDate != null && TransactionTime != null && DriverPromptId != null)
            {
                var transaction = Import.CreditTransactions.FirstOrDefault(f => f.CCCo == Import.HQCompanyParm.APCo && f.Mth == Mth && f.UniqueTransId == CalcUniqueTransId );
                if (transaction == null)
                {
                    //var merchant = SetMerchant();
                    transaction = new CreditTransaction()
                    {
                        CCCo = (byte)Import.HQCompanyParm.APCo,
                        Mth = Mth,
                        //TransId = GetNextTransId(),
                        UniqueTransId = CalcUniqueTransId,
                        MerchantId = MerchantId,
                        NewDescription = TransactionDescription,
                        TransDate = TransDateTime,
                        PostDate = PostDate ?? DateTime.MinValue,
                        PRCo = PRCo,
                        EmployeeId = (int)PREmployeeId,
                        Employee = PREmployee,
                        EmployeeEmail = PREmployee?.HREmployee?.CompanyEmail,
                        TransAmt = NetCost ?? 0,
                        OrigDescription = TransactionDescription,
                        ImportId = ImportId,

                        Source = Import.Source,
                        Import = this.Import,
                        db = db,
                        Merchant = Merchant,
                        PictureStatusId = (int)DB.CMPictureStatusEnum.NotNeeded,
                    };
                    //transaction.APRef = transaction.TransId.ToString();

                    //db.CreditTransactions.Add(transaction);
                    Import.CreditTransactions.Add(transaction);
                }

                return transaction;
            }
            return null;

        }

    }
}
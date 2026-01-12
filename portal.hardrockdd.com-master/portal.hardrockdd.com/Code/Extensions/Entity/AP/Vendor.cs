using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class APVendor
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
                _db ??= VPEntities.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public VendorAltAddress? AddAddress(CreditMerchant merchant)
        {
            if (merchant == null)
                return null;

            var address = AltAddresses.FirstOrDefault(f => f.MerchantId == merchant.MerchantId);

            if (address == null)
            {
                var addressSeq = AltAddresses.DefaultIfEmpty().Max(f => f == null ? (byte)0 : f.AddressSeq);
                if (addressSeq <= (byte.MaxValue - 1))
                {
                    addressSeq++;
                    address = new VendorAltAddress
                    {
                        VendorGroupId = VendorGroupId,
                        VendorId = VendorId,
                        AddressSeq = addressSeq,
                        Type = 1,
                        Description = merchant.Name,
                        Address = merchant.Address,
                        City = merchant.City,
                        State = merchant.State,
                        Zip = merchant.Zip,
                        Notes = "Auto Added by CC",
                        MerchantId = merchant.MerchantId
                    };
                    AltAddresses.Add(address);
                }
            }
            return address;
        }

        public void RunMerchantMatch()
        {
            if (string.IsNullOrEmpty(MerchantMatchString))
                return;

            var matchList = MerchantMatchString.Split(';');
            foreach (var str in matchList)
            {
                var merchantList = db.CreditMerchants.Where(f => f.VendGroupId == this.VendorGroupId && f.Name.Contains(str)).ToList();
                foreach (var item in merchantList)
                {
                    var isVendorIdChanged = item.VendorId != VendorId;
                    if (isVendorIdChanged)
                    {
                        item.VendorId = VendorId;
                        AddAddress(item);
                        if (isVendorIdChanged)
                        {
                            var picList = db.CreditTransactions.Where(f => f.Merchant.Vendor.VendorId == item.VendorId &&
                                                            (f.PictureStatusId == (int)DB.CMPictureStatusEnum.Empty ||
                                                             f.PictureStatusId == (int)DB.CMPictureStatusEnum.NotNeeded) &&
                                                             f.TransStatusId == (int)DB.CMTranStatusEnum.Open).ToList();

                            foreach (var trans in picList)
                            {
                                trans.AutoPictureStatus();
                            }
                            var codeList = db.CreditTransactions.Where(f => f.Merchant.VendorId == item.VendorId &&
                                                                       (f.CodedStatusId == (int)DB.CMTransCodeStatusEnum.AutoCoded ||
                                                                        f.PictureStatusId == (int)DB.CMTransCodeStatusEnum.Empty ||
                                                                        f.PictureStatusId == (int)DB.CMTransCodeStatusEnum.NeedReview) &&
                                                                        f.TransStatusId == (int)DB.CMTranStatusEnum.Open).ToList();

                            foreach (var trans in codeList)
                            {
                                trans.AutoCode();
                            }
                        }
                    }
                }
            }
        }
        
        public void RunMerchantMatch(List<CreditTransaction> transactions)
        {
            if (string.IsNullOrEmpty(MerchantMatchString))
                return;

            var matchList = MerchantMatchString.Split(';');
            foreach (var str in matchList)
            {
                var merchantList = db.CreditMerchants.Where(f => f.VendGroupId == this.VendorGroupId && 
                                                                 f.Name.Contains(str) &&
                                                                 f.Transactions.Any(trans => (trans.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open)
                                                            ).ToList();
                foreach (var item in merchantList)
                {
                    var isVendorIdChanged = (item.VendorId != VendorId) && (item.VendorId == null || item.Company.APCompanyParm.udDftMerchantVendorId == item.VendorId);
                    if (isVendorIdChanged)
                    {
                        item.VendorId = VendorId;
                        AddAddress(item);
                        if (isVendorIdChanged)
                        {
                            var picList = transactions.Where(f => f.Merchant.Vendor.VendorId == item.VendorId &&
                                                            ((f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.Empty ||
                                                             (f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.NotNeeded) &&
                                                            (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                            foreach (var trans in picList)
                            {
                                trans.AutoPictureStatus();
                            }
                            var codeList = transactions.Where(f => f.Merchant.VendorId == item.VendorId &&
                                                                   (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.Empty &&
                                                                   (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                            foreach (var trans in codeList)
                            {
                                trans.AutoCode();
                            }
                        }
                    }
                }
            }
        }
    }
}
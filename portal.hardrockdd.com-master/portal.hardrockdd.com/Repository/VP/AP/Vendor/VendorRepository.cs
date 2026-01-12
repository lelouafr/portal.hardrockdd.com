using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor;
using portal.Models.Views.AP.Vendor.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AP
{
    public static class VendorRepository 
    {
        public static VendorViewModel ProcessUpdate(VendorViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == model.VendGroupId && f.VendorId == model.VendorId);
            if (updObj != null)
            {
                var isReceiptRequiredChanged = updObj.IsReceiptRequired != model.IsReceiptRequired;
                var defaultCodingChanged = updObj.DefaultGLAcct != model.DefaultGLAcct ||
                                            updObj.DefaultJCPhaseId != model.DefaultJCPhaseId ||
                                            updObj.DefaultJCCType != model.DefaultJCCType ||
                                            updObj.DefaultEMCostCodeId != model.DefaultEMCostCodeId ||
                                            updObj.DefaultEMCType != model.DefaultEMCType;

                updObj.IsReoccurring = model.IsReoccurring;
                updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;

                if (isReceiptRequiredChanged)
                {
                    var list = db.CreditTransactions.Where(f => f.Merchant.Vendor.VendorId == updObj.VendorId &&
                                                    ((f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.Empty ||
                                                     (f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.NotNeeded) &&
                                                     (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                    foreach (var trans in list)
                    {
                        trans.AutoPictureStatus();
                    }
                }

                if (defaultCodingChanged)
                {
                    var list = db.CreditTransactions.Where(f => f.Merchant.VendorId == updObj.VendorId &&
                                                               ((f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.AutoCoded ||
                                                                (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.Empty ||
                                                                (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.NeedReview) &&
                                                                (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                    foreach (var trans in list)
                    {
                        trans.AutoCode();
                    }
                }
            }

            return new VendorViewModel(updObj);
        }
        //
        public static VendorMerchantViewModel ProcessUpdate(VendorMerchantViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == model.VendGroupId && f.VendorId == model.VendorId);
            if (updObj != null)
            {
                updObj.MerchantMatchString = model.MatchString;
            }

            return new VendorMerchantViewModel(updObj);
        }
        
        public static VendorInfoViewModel ProcessUpdate(VendorInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == model.VendGroupId && f.VendorId == model.VendorId);
            if (updObj != null)
            {
                updObj.DisplayName = model.VendorName;
            }

            return new VendorInfoViewModel(updObj);
        }

        public static VendorCodingViewModel ProcessUpdate(VendorCodingViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == model.VendGroupId && f.VendorId == model.VendorId);
            if (updObj != null)
            {

                var isReceiptRequiredChanged = updObj.IsReceiptRequired != model.IsReceiptRequired;
                var defaultCodingChanged = updObj.DefaultGLAcct != model.DefaultGLAcct ||
                                            updObj.DefaultJCPhaseId != model.DefaultJCPhaseId ||
                                            updObj.DefaultJCCType != model.DefaultJCCType ||
                                            updObj.DefaultEMCostCodeId != model.DefaultEMCostCodeId ||
                                            updObj.DefaultEMCType != model.DefaultEMCType;

                updObj.IsReoccurring = model.IsReoccurring;
                updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;

                if (isReceiptRequiredChanged)
                {
                    var list = db.CreditTransactions.Where(f => f.Merchant.Vendor.VendorId == updObj.VendorId &&
                                                    ((f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.Empty ||
                                                     (f.PictureStatusId ?? 0) == (int)DB.CMPictureStatusEnum.NotNeeded) &&
                                                     (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                    foreach (var trans in list)
                    {
                        trans.AutoPictureStatus();
                    }
                }

                if (defaultCodingChanged)
                {
                    var list = db.CreditTransactions.Where(f => f.Merchant.VendorId == updObj.VendorId &&
                                                               (f.CodedStatusId == (int)DB.CMTransCodeStatusEnum.AutoCoded ||
                                                                (f.PictureStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.Empty ||
                                                                (f.PictureStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.NeedReview) &&
                                                                (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                    foreach (var trans in list)
                    {
                        trans.AutoCode();
                    }
                }
            }

            return new VendorCodingViewModel(updObj);
        }
    }
}
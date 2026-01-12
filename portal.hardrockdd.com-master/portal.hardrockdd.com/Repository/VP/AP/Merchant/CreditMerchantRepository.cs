using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Models.Views.AP.Vendor.Merchant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.Merchant
{
    public static class CreditMerchantRepository
    {

        public static CreditMerchant FindCreate(CreditMerchant model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var merchant = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            if (merchant == null)
            {
                merchant = db.CreditMerchants.Local.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            }
            if (merchant == null)
            {
                db.CreditMerchants.Add(model);
                merchant = model;
            }
            return merchant;
        }

        public static MerchantViewModel ProcessUpdate(MerchantViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            if (updObj != null)
            {
                updObj.VendorId = model.VendorId;
                updObj.IsReoccurring = model.IsReoccurring;
                updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.IsAPRefRequired = model.IsAPRefRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;
            }

            return new MerchantViewModel(updObj);
        }

        public static MerchantInfoViewModel ProcessUpdate(MerchantInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            if (updObj != null)
            {
                updObj.VendorId = model.VendorId;
            }

            return new MerchantInfoViewModel(updObj);
        }

        public static MerchantCodingViewModel ProcessUpdate(MerchantCodingViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && f.MerchantId == model.MerchantId);
            if (updObj != null)
            {
                var isReceiptRequiredChanged = updObj.IsReceiptRequired != model.IsReceiptRequired;
                var defaultCodingChanged = updObj.DefaultGLAcct != model.DefaultGLAcct ||
                                            updObj.DefaultJCPhaseId != model.DefaultJCPhaseId ||
                                            updObj.DefaultJCCType != model.DefaultJCCType ||
                                            updObj.DefaultEMCostCodeId != model.DefaultEMCostCodeId ||
                                            updObj.DefaultEMCType != model.DefaultEMCType;
                var isAPRefRequiredChanged = updObj.IsAPRefRequired != model.IsAPRefRequired;

                updObj.IsReoccurring = model.IsReoccurring;
                updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.IsAPRefRequired = model.IsAPRefRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;


                if (isReceiptRequiredChanged)
                {
                    var list = db.CreditTransactions.Where(f => f.MerchantId == updObj.MerchantId &&
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
                    var list = db.CreditTransactions.Where(f => f.MerchantId == updObj.MerchantId &&
                                                               ((f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.AutoCoded ||
                                                                (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.Empty ||
                                                                (f.CodedStatusId ?? 0) == (int)DB.CMTransCodeStatusEnum.NeedReview) &&
                                                                (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();

                    foreach (var trans in list)
                    {
                        trans.AutoCode();
                    }
                }
                if (isAPRefRequiredChanged && updObj.IsReceiptRequired == true)
                {
                    var list = db.CreditTransactions.Where(f => f.MerchantId == updObj.MerchantId &&
                                                                (f.TransStatusId ?? 0) == (int)DB.CMTranStatusEnum.Open).ToList();
                    foreach (var trans in list)
                    {
                        if (updObj.IsReceiptRequired == true)
                        {
                            if (trans.UniqueTransId.Contains(trans.APRef))
                            {
                                //trans.APRef = null;
                            }
                        }
                    }
                }
            }

            return new MerchantCodingViewModel(updObj);
        }
    }
}
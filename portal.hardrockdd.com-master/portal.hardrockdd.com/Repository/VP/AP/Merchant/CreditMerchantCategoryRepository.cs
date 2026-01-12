using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Models.Views.AP.Vendor.Merchant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.Merchant
{
    public static class CreditMerchantCategoryRepository
    {

        public static MerchantCategoryViewModel ProcessUpdate(MerchantCategoryViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == model.VendGroupId && 
                                                                         f.CategoryGroup == model.CategoryGroup &&
                                                                         f.CategoryCodeId == model.CategoryCodeId);
            if (updObj != null)
            {
                //updObj.VendorId = model.VendorId;
                //updObj.IsReoccurring = model.IsReoccurring;
                //updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;
            }

            return new MerchantCategoryViewModel(updObj);
        }

        public static MerchantCategoryInfoViewModel ProcessUpdate(MerchantCategoryInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == model.VendGroupId &&
                                                                         f.CategoryGroup == model.CategoryGroup &&
                                                                         f.CategoryCodeId == model.CategoryCodeId);
            if (updObj != null)
            {
                //updObj.VendorId = model.VendorId;
            }

            return new MerchantCategoryInfoViewModel(updObj);
        }

        public static MerchantCategoryCodingViewModel ProcessUpdate(MerchantCategoryCodingViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == model.VendGroupId &&
                                                                         f.CategoryGroup == model.CategoryGroup &&
                                                                         f.CategoryCodeId == model.CategoryCodeId);
            if (updObj != null)
            {
                //updObj.IsReoccurring = model.IsReoccurring;
                //updObj.IsReceiptRequired = model.IsReceiptRequired;
                updObj.DefaultGLAcct = model.DefaultGLAcct;
                updObj.DefaultJCPhaseId = model.DefaultJCPhaseId;
                updObj.DefaultJCCType = model.DefaultJCCType;
                updObj.DefaultEMCostCodeId = model.DefaultEMCostCodeId;
                updObj.DefaultEMCType = model.DefaultEMCType;
            }

            return new MerchantCategoryCodingViewModel(updObj);
        }
    }
}
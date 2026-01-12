using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Models.Views.AP.Vendor.Merchant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.Merchant
{
    public static class CreditMerchantGroupRepository
    {

        public static MerchantGroupViewModel ProcessUpdate(MerchantGroupViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == model.VendGroupId &&
                                                                         f.CategoryGroup == model.CategoryGroup);
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

            return new MerchantGroupViewModel(updObj);
        }

        public static MerchantGroupInfoViewModel ProcessUpdate(MerchantGroupInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == model.VendGroupId &&
                                                                         f.CategoryGroup == model.CategoryGroup);
            if (updObj != null)
            {
                //updObj.VendorId = model.VendorId;
            }

            return new MerchantGroupInfoViewModel(updObj);
        }

        public static MerchantGroupCodingViewModel ProcessUpdate(MerchantGroupCodingViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == model.VendGroupId &&
                                                                         f.CategoryGroup == model.CategoryGroup );
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

            return new MerchantGroupCodingViewModel(updObj);
        }
    }
}
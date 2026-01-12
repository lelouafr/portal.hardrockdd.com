using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Repository.VP.AP
{
    public static class APDocumentSeqRepository
    {
        public static DocumentSeqViewModel ProcessUpdate(DocumentSeqViewModel model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.APDocuments.FirstOrDefault(f => f.APCo == model.APCo && f.DocId == model.DocId);
            if (updObj != null)
            {
                var invDateChanged = updObj.InvDate != model.InvoiceDate;
                var mthDate = DateTime.TryParse(model.Mth, out DateTime mthDateOut) ? mthDateOut : updObj.Mth;

                if (updObj.VendorId != model.VendorId)
                    updObj.VendorId = model.VendorId;
                else if (updObj.PO != model.PO)
                    updObj.PO = model.PO;
                else
                {
                    updObj.Description = model.Description;
                    updObj.DivisionId = model.DivisionId;
                    updObj.APRef = model.APReference;
                    updObj.Mth = mthDate;

                    updObj.Description = model.Description;
                    updObj.DueDate = model.DueDate;
                    updObj.InvDate = model.InvoiceDate;
                    updObj.InvTotal = model.Amount;
                }

                db.SaveChanges(modelState);
            }
            var result = new DocumentSeqViewModel(updObj);
            return result;
        }
    }
}
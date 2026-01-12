using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.AP.CreditCard;
using portal.Repository.VP.HQ;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AP
{
    public static class APBatchSeqRepository
    {
        public static APBatchHeader ProcessUpdate(Models.Views.HQ.Batch.AP.APBatchTransViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.APBatchHeaders.FirstOrDefault(f => f.APCo == model.Co && f.BatchId == model.BatchId && f.BatchSeq == model.BatchSeq);
            if (updObj != null)
            {
                var invDateChanged = updObj.InvDate != model.InvoiceDate;
                var addLines = false;
                if (invDateChanged && updObj.Vendor != null && model.InvoiceDate != null)
                {
                    model.DueDate = model.InvoiceDate.AddDays(updObj.Vendor.PayTerm?.DaysTillDue ?? 0);
                }
                if (model.Description?.Length >= 30 && !string.IsNullOrEmpty(model.Description))
                {
                    model.Description = model.Description.Substring(0, 29);
                }


                updObj.APRef = model.APReference?.Trim();
                var mthDate = DateTime.TryParse(model.Mth, out DateTime mthDateOut) ? mthDateOut : updObj.Mth;
                updObj.Mth = mthDate;
                updObj.Description = model.Description;
                updObj.InvDate = model.InvoiceDate;
                updObj.DueDate = model.DueDate;
                updObj.VendorId = model.VendorId;
                updObj.VendorGroupId = model.VendGroupId;
                updObj.InvTotal = model.Amount;
                updObj.CMAcct = model.CMAcct;
                updObj.CMCo = updObj.bAPCO.CMCo;
            }
            return updObj;
        }


        public static DB.Infrastructure.ViewPointDB.Data.Batch AddCCTransToBatch(byte co, DateTime mth, string source, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var parms = db.APCompanyParms.FirstOrDefault(f => f.APCo == co);
            var dftVendor = db.APVendors.FirstOrDefault(f => f.VendorGroupId == co && f.VendorId == parms.udDftMerchantVendorId);
            var postList = db.vCMTransactionCodes.Where(f => f.Mth == mth & f.Co == co && f.BatchId != 0 && f.Source == source).ToList();
            var transList = db.CreditTransactions
                              .Include("Merchant")
                              .Include("Merchant.Vendor")
                              .Include("Coding")
                              .Include("Coding.Transaction")
                              .Include("Coding.Transaction.Merchant")
                              .Include("Coding.Transaction.Merchant.Vendor")
                              .Include("Coding.Job")
                              .Include("Coding.JobPhase")
                              .Include("Coding.JobPhaseCost")
                              .Include("Coding.Equipment")
                              .Include("Coding.EquipmentCostCode")
                              .Where(f => f.CCCo == co && 
                                          f.Mth == mth && 
                                          f.Source == source &&
                                          f.APRef.Length != 0 && 
                                          f.APRef != null &&
                                          
                                          f.TransAmt == f.Coding.Sum(sum => sum.GrossAmt) &&
                                          !f.Coding.Any(c => c.GrossAmt == null || c.tGLAcct == null))
                              .ToList();
            var newList = new List<CreditTransaction>();
            foreach (var trans in transList)
            {
                foreach (var code in trans.Coding)
                {
                    if(!postList.Any(p => p.TransId == code.TransId && p.SeqId == code.SeqId && p.Co == code.CCCo))
                    {
                        if (!newList.Any(p => p.TransId == code.TransId && p.CCCo == code.CCCo))
                            newList.Add(trans);
                    }
                }
            }

            transList = newList;// transList.Where(f => !postList.Any(p => p.TransId == f.TransId && p.Co == f.CCCo)).ToList();
            if (!transList.Any())
            {
                return null;
            }
            //return null;
            var batch = DB.Infrastructure.ViewPointDB.Data.Batch.FindCreate(parms.HQCompanyParm, "APHB", "AP Entry", mth);            
            batch.InUseBy = "WebPortalUser";
            var transCnt = 0;
            var transTotalCnt = 0;
            db.BulkSaveChanges();

            foreach (var tran in transList)
            {
                APBatchHeader seq = null;
                APTran apTran = null;
                if (tran.Merchant.IsAPRefRequired == true)
                {
                    //Test to see if AP ref exisit already
                    apTran = db.APTrans.FirstOrDefault(f => f.APCo == tran.CCCo && f.VendorId == tran.Merchant.VendorId && f.APRef == tran.APRef);
                    var apExists = db.APTrans.Any(f => f.APCo == tran.CCCo && f.VendorId == tran.Merchant.VendorId && f.APRef == tran.APRef);
                    if (apTran != null)
                    {
                        
                        apTran.CCTransId = tran.TransId;
                        apTran.MerchantId = tran.MerchantId;
                        apTran.UniqueAttchID ??= Guid.NewGuid();
                        if (apTran.CMAcct != dftVendor.CMAcct)
                        {
                            apTran.CMAcct = dftVendor.CMAcct;
                        }

                        tran.LinkedImages.ToList().ForEach(link => {
                            var createDuplicate = link.Image.Transactions.Count > 1;
                            apTran.Attachment.GetRootFolder().CopyFile(link.File(), createDuplicate);
                        });

                        if (apTran.InvTotal == tran.TransAmt)
                        {
                            //Remove Exisiting CC Coding
                            foreach (var code in tran.Coding.ToList())
                            {
                                tran.Coding.Remove(code);
                            }

                            //Copy AP Trans coding to CC Coding
                            foreach (var item in apTran.Lines)
                            {
                                
                                var code = CreditCardTransactionCodingRepository.Init(item, tran);
                                tran.Coding.Add(code);

                                item.CCTransId = tran.TransId;
                                item.CCCodingSeqId = code.SeqId;

                                item.TransDate = tran.TransDate;
                                item.CCReference = tran.UniqueTransId;
                                item.MerchGroup = tran.Merchant.CategoryGroup;
                                item.EmpNumber = tran.EmployeeId;
                                item.Vendor = tran.Merchant.Name;
                            }
                        }
                        else
                        {
                            if (apTran.Lines.Any(f => f.GrossAmt == tran.TransAmt))
                            {
                                var item = apTran.Lines.FirstOrDefault(f => f.GrossAmt == tran.TransAmt);

                                item.CCTransId = tran.TransId;
                                item.CCCodingSeqId = 1;

                                item.TransDate = tran.TransDate;
                                item.CCReference = tran.UniqueTransId;
                                item.MerchGroup = tran.Merchant.CategoryGroup;
                                item.EmpNumber = tran.EmployeeId;
                                item.Vendor = tran.Merchant.Name;

                            }
                        }
                    }
                }
                //If AP trans is null continue to adding the AP Transactions to the Batch
                if (apTran == null)
                {
                    //var apTranLine = db.APTransLines.FirstOrDefault(f => f.APCo == tran.CCCo && f.CCTransId == tran.TransId && f.CCCodingSeqId == tran.SeqId);

                }
                if (seq == null && apTran == null)
                {
                    seq = tran.AddToBatch(batch);

                    if (batch == null)
                        batch = seq.Batch;
                }
                tran.TransStatusId = (int)DB.CMTranStatusEnum.APPosted;
                transCnt++;
                transTotalCnt++;
                if (transCnt >= 50)
                {
                    transCnt = 0;
                    db.BulkSaveChanges();
                }
            }

            return batch;
        }
    }
}
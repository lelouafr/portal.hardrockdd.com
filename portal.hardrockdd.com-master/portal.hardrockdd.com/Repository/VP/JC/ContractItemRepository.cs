//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class ContractItemRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public ContractItemRepository()
//        {

//        }
        
//        public static ContractItem Init()
//        {
//            var model = new ContractItem
//            {

//            };

//            return model;
//        }

//        public List<ContractItem> GetContractItems(byte JCCo, string ContractId)
//        {
//            var qry = db.ContractItems
//                        .Where(f => f.JCCo == JCCo && f.ContractId == ContractId)
//                        .ToList();

//            return qry;
//        }
        
//        public ContractItem GetContractItem(byte JCCo, string ContractId, string Item)
//        {
//            var qry = db.ContractItems
//                        .FirstOrDefault(f => f.JCCo == JCCo && f.ContractId == ContractId && f.Item == Item);

//            return qry;
//        }

//        public static List<SelectListItem> GetSelectList(List<ContractItem> List, string selected = "")
//        {
//            var result = List.Select(s => new SelectListItem
//            {
//                Value = s.Item,
//                Text = s.Description,
//                Selected = s.Item == selected ? true : false
//            }).ToList();

//            return result;
//        }

//        //public ContractItem ProcessUpdate(ContractItem model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    var updObj = GetContractItem(model.JCCo, model.ContractId, model.Item);
//        //    if (updObj != null)
//        //    {
//        //        /****Write the changes to object****/
//        //        updObj.JCCo = model.JCCo;
//        //        updObj.ContractId = model.ContractId;
//        //        updObj.Item = model.Item;
//        //        updObj.Description = model.Description;
//        //        updObj.Department = model.Department;
//        //        updObj.TaxGroup = model.TaxGroup;
//        //        updObj.TaxCode = model.TaxCode;
//        //        updObj.UM = model.UM;
//        //        updObj.UM = model.UM;
//        //        updObj.SIRegion = model.SIRegion;
//        //        updObj.ItemCodeId = model.ItemCodeId;
//        //        updObj.RetainPCT = model.RetainPCT;
//        //        updObj.OrigContractAmt = model.OrigContractAmt;
//        //        updObj.OrigContractUnits = model.OrigContractUnits;
//        //        updObj.OrigUnitPrice = model.OrigUnitPrice;
//        //        updObj.ContractAmt = model.ContractAmt;
//        //        updObj.ContractUnits = model.ContractUnits;
//        //        updObj.UnitPrice = model.UnitPrice;
//        //        updObj.BilledAmt = model.BilledAmt;
//        //        updObj.BilledUnits = model.BilledUnits;
//        //        updObj.ReceivedAmt = model.ReceivedAmt;
//        //        updObj.CurrentRetainAmt = model.CurrentRetainAmt;
//        //        updObj.BillType = model.BillType;
//        //        updObj.BillGroup = model.BillGroup;
//        //        updObj.BillDescription = model.BillDescription;
//        //        updObj.BillOriginalUnits = model.BillOriginalUnits;
//        //        updObj.BillOriginalAmt = model.BillOriginalAmt;
//        //        updObj.BillCurrentUnits = model.BillCurrentUnits;
//        //        updObj.BillCurrentAmt = model.BillCurrentAmt;
//        //        updObj.BillUnitPrice = model.BillUnitPrice;
//        //        updObj.Notes = model.Notes;
//        //        updObj.InitSubs = model.InitSubs;
//        //        updObj.UniqueAttchID = model.UniqueAttchID;
//        //        if (model.StartMonth >= new DateTime(1900, 1, 1))
//        //        {
//        //            updObj.StartMonth = model.StartMonth;
//        //        }
//        //        updObj.MarkUpRate = model.MarkUpRate;
//        //        updObj.ProjNotes = model.ProjNotes;
//        //        updObj.ProjPlug = model.ProjPlug;
//        //        updObj.KeyID = model.KeyID;
//        //        updObj.InitAsZero = model.InitAsZero;
//        //        updObj.ProjectAmt = model.ProjectAmt;

//        //        db.SaveChanges(modelState);
//        //    }
//        //    return updObj;
//        //}
        
//        public ContractItem Create(ContractItem model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            db.ContractItems.Add(model);
//            db.SaveChanges(modelState);
//            return model;
//        }

//        public bool Delete(ContractItem model, ModelStateDictionary modelState = null)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            db.ContractItems.Remove(model);
//            return db.SaveChanges(modelState) == 0 ? false : true;            
//        }

//        public bool Exists(ContractItem model)
//        {
//            var qry = from f in db.ContractItems
//                      where f.JCCo == model.JCCo && f.ContractId == model.ContractId && f.Item == model.Item
//                      select f;

//            if (qry.Any())
//                return true;

//            return false;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~ContractItemRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}

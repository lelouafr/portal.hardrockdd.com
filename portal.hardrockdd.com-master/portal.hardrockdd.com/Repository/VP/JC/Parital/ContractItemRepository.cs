//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//namespace portal.Repository.VP.JC
//{
//    public partial class ContractItemRepository : IDisposable
//    {

//        public static ContractItem Init(ContractItem contractItem, Contract contract)
//        {
//            if (contractItem == null)
//            {
//                throw new System.ArgumentNullException(nameof(contractItem));
//            }
//            if (contract == null)
//            {
//                throw new System.ArgumentNullException(nameof(contract));
//            }
//            var model = new ContractItem
//            {
//                JCCo = contract.JCCo,
//                ContractId = contract.ContractId,
//                Item = contractItem.Item,
//                Description = contractItem.Description,
//                Department = contractItem.Department,
//                TaxGroup = contractItem.TaxGroup,
//                TaxCode = contractItem.TaxCode,
//                UM = contractItem.UM,
//                SIRegion = contractItem.SIRegion,
//                ItemCodeId = contractItem.ItemCodeId,
//                RetainPCT = contractItem.RetainPCT,
//                OrigContractAmt = contractItem.OrigContractAmt,
//                OrigContractUnits = contractItem.OrigContractUnits,
//                OrigUnitPrice = contractItem.OrigUnitPrice,
//                ContractAmt = contractItem.ContractAmt,
//                ContractUnits = contractItem.ContractUnits,
//                UnitPrice = contractItem.UnitPrice,
//                BilledAmt = contractItem.BilledAmt,
//                BilledUnits = contractItem.BilledUnits,
//                ReceivedAmt = contractItem.ReceivedAmt,
//                CurrentRetainAmt = contractItem.CurrentRetainAmt,
//                BillType = contractItem.BillType,
//                BillGroup = contractItem.BillGroup,
//                BillDescription = contractItem.BillDescription,
//                BillOriginalUnits = contractItem.BillOriginalUnits,
//                BillOriginalAmt = contractItem.BillOriginalAmt,
//                BillCurrentUnits = contractItem.BillCurrentUnits,
//                BillCurrentAmt = contractItem.BillCurrentAmt,
//                BillUnitPrice = contractItem.BillUnitPrice,
//                Notes = contractItem.Notes,
//                InitSubs = contractItem.InitSubs,
//                StartMonth = contract.StartMonth,
//                MarkUpRate = contractItem.MarkUpRate,
//                ProjNotes = contractItem.ProjNotes,
//                ProjPlug = contractItem.ProjPlug,
//                InitAsZero = contractItem.InitAsZero,
//                ProjectAmt = contractItem.ProjectAmt
//            };
//            return model;
//        }

//        //public static List<ContractItem> CopyTemplate(Contract contract)
//        //{
//        //    if (contract == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(contract));
//        //    }
                        
//        //    var srcContract = contract.Company.JCCompanyParm.TemplateContract;

//        //    //var srcList = srcContract.ContractItems.ToList();
//        //    var list = srcContract.ContractItems
//        //                 .Where(f => !contract.ContractItems.Any(a => a.Item == f.Item))
//        //                 .ToList();
//        //    var newList = contract.ContractItems.ToList();
//        //    foreach (var item in list)
//        //    {
//        //        var listItem = Init(item, contract);
//        //        newList.Add(listItem);
//        //    }
//        //    return newList;
//        //}
//    }
//}

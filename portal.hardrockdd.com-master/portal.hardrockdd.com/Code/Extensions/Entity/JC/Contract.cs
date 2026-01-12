using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class JCContract
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
                        _db = this.JCCompanyParm.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public ContractItem AddContractItem(ContractItem contractItem)
        {
            if (contractItem == null)
                return null;

            var newItem = ContractItems.FirstOrDefault(f => f.Item == contractItem.Item);
            if (newItem == null)
            {
                newItem = new ContractItem
                {
                    JCCo = JCCo,
                    ContractId = ContractId,
                    Item = contractItem.Item,
                    Description = contractItem.Description,
                    Department = contractItem.Department,
                    TaxGroup = contractItem.TaxGroup,
                    TaxCode = contractItem.TaxCode,
                    UM = contractItem.UM,
                    SIRegion = contractItem.SIRegion,
                    ItemCodeId = contractItem.ItemCodeId,
                    RetainPCT = contractItem.RetainPCT,
                    OrigContractAmt = contractItem.OrigContractAmt,
                    OrigContractUnits = contractItem.OrigContractUnits,
                    OrigUnitPrice = contractItem.OrigUnitPrice,
                    ContractAmt = contractItem.ContractAmt,
                    ContractUnits = contractItem.ContractUnits,
                    UnitPrice = contractItem.UnitPrice,
                    BilledAmt = contractItem.BilledAmt,
                    BilledUnits = contractItem.BilledUnits,
                    ReceivedAmt = contractItem.ReceivedAmt,
                    CurrentRetainAmt = contractItem.CurrentRetainAmt,
                    BillType = contractItem.BillType,
                    BillGroup = contractItem.BillGroup,
                    BillDescription = contractItem.BillDescription,
                    BillOriginalUnits = contractItem.BillOriginalUnits,
                    BillOriginalAmt = contractItem.BillOriginalAmt,
                    BillCurrentUnits = contractItem.BillCurrentUnits,
                    BillCurrentAmt = contractItem.BillCurrentAmt,
                    BillUnitPrice = contractItem.BillUnitPrice,
                    Notes = contractItem.Notes,
                    InitSubs = contractItem.InitSubs,
                    StartMonth = StartMonth,
                    MarkUpRate = contractItem.MarkUpRate,
                    ProjNotes = contractItem.ProjNotes,
                    ProjPlug = contractItem.ProjPlug,
                    InitAsZero = contractItem.InitAsZero,
                    ProjectAmt = contractItem.ProjectAmt,

                    Contract = this,
                    
                };

                ContractItems.Add(newItem);
            }

            return newItem;
        }

        internal EXT_JCContract AddExtContract()
        {
            var ext = this.ExtContract;
            if (ext == null)
            {
                ext = new EXT_JCContract()
                {
                    JCCo = this.JCCo,
                    ContractId = this.ContractId,
                    Contract = this
                };

                this.ExtContract = ext;
                db.BulkSaveChanges();
            }
            return ext;
        }

        public void SyncTemplatedContractItems()
        {
            var srcContract = JCCompanyParm.TemplateContract;

            foreach (var item in srcContract.ContractItems.ToList())
            {
                AddContractItem(item);
            }
        }
    }
}
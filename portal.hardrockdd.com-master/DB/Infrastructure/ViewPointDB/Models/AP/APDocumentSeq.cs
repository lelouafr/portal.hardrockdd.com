//using System;
//using System.Linq;

//namespace DB.Infrastructure.ViewPointDB.Data
//{
//    public partial class APDocumentSeq
//    {
//        private VPContext _db;

//        public VPContext db
//        {
//            set
//            {
//                _db = value;
//            }
//            get
//            {
//                if (_db == null)
//                {
//                    _db = VPContext.GetDbContextFromEntity(this);

//                    if (_db == null)
//                        _db = this.Document.db;

//                    if (_db == null)
//                        throw new NullReferenceException("GetDbContextFromEntity is null");

//                }
//                return _db;
//            }
//        }

//        public DateTime? InvDate
//        {
//            get
//            {
//                return tInvDate;
//            }
//            set
//            {
//                if (tInvDate != value)
//                {
//                    tInvDate = value;
//                    if (Vendor != null && tInvDate != null)
//                    {
//                        DueDate = tInvDate.Value.AddDays(Vendor.PayTerm?.DaysTillDue ?? 0);
//                    }
//                }
//            }
//        }

//        public string APRef
//        {
//            get
//            {
//                if (tAPRef == null)
//                    tAPRef ??= string.Empty;
//                if (tAPRef.Length > 15)
//                    tAPRef = tAPRef.Substring(0, 14);

//                return tAPRef;
//            }
//            set
//            {
//                if (tAPRef != value)
//                {
//                    value ??= string.Empty;
//                    value = value.Trim();
//                    if (value.Length > 15)
//                        value = value.Substring(0, 14);

//                    tAPRef = value;
//                }
//            }
//        }

//        public string Description
//        {
//            get
//            {
//                if (tDescription == null)
//                    tDescription ??= string.Empty;
//                if (tDescription.Length > 30)
//                    tDescription = tDescription.Substring(0, 29);

//                return tDescription;
//            }
//            set
//            {
//                if (tDescription != value)
//                {
//                    value ??= string.Empty;
//                    if (value.Length > 30)
//                        value = value.Substring(0, 29);

//                    tDescription = value;
//                }
//            }
//        }

//        public int? VendorId
//        {
//            get
//            {
//                return tVendorId;
//            }
//            set
//            {
//                if (tVendorId != value)
//                {
//                    UpdateVendor(value);
//                }
//            }
//        }

//        public void UpdateVendor(int? value)
//        {
//            if (Vendor == null && tVendorId != null)
//            {
//                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == tVendorId);
//                if (vendor != null)
//                {
//                    VendorGroupId = vendor.VendorGroupId;
//                    tVendorId = vendor.VendorId;

//                    Vendor = vendor;
//                }
//            }

//            if (tVendorId != value)
//            {
//                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == value);
//                if (vendor != null)
//                {
//                    VendorGroupId = vendor.VendorGroupId;
//                    tVendorId = vendor.VendorId;

//                    Vendor = vendor;
//                }
//                else
//                {
//                    VendorGroupId = null;
//                    tVendorId = null;
//                    Vendor = null;
//                }
//                if (PurchaseOrder != null)
//                {
//                    if (PurchaseOrder?.VendorId != tVendorId)
//                    {
//                        PO = null;
//                    }
//                }
//            }
//        }

//        public string PO
//        {
//            get
//            {
//                return tPO;
//            }
//            set
//            {
//                if (tPO != value)
//                {
//                    UpdatePurchaseOrder(value);
//                }
//            }
//        }

//        public void UpdatePurchaseOrder(string value)
//        {
//            if (PurchaseOrder == null && tPO != null)
//            {
//                var purchaseOrder = db.PurchaseOrders.FirstOrDefault(f => f.PO == tPO);
//                if (purchaseOrder != null)
//                {
//                    POCo = purchaseOrder.POCo;
//                    tPO = purchaseOrder.PO;
//                    PurchaseOrder = purchaseOrder;
//                }
//                else
//                {
//                    POCo = null;
//                    tPO = null;
//                    PurchaseOrder = null;
//                }
//            }
//            if (tPO != value)
//            {
//                var purchaseOrder = db.PurchaseOrders.FirstOrDefault(f => f.PO == value);
//                if (purchaseOrder != null)
//                {
//                    POCo = purchaseOrder.POCo;
//                    tPO = purchaseOrder.PO;
//                    PurchaseOrder = purchaseOrder;

//                    InvTotal = purchaseOrder.Items.Sum(sum => (sum.OrigCost + sum.OrigTax));
//                    Description = purchaseOrder.Description;

//                    VendorGroupId = purchaseOrder.VendorGroupId;
//                    tVendorId = purchaseOrder.VendorId;
//                    Vendor = purchaseOrder.Vendor;
//                }
//                else
//                {
//                    POCo = null;
//                    tPO = null;
//                    PurchaseOrder = null;

//                }

//                //model.Amount = updObj.InvTotal;
//                //model.Description = updObj.Description;
//                //model.VendorId = updObj.VendorId;

//            }

//        }

//        public APDocumentLine AddLine(PurchaseOrderItem poItem)
//        {

//            var division = db.CompanyDivisions.OrderBy(o => o.DivisionId).FirstOrDefault(f => f.HQCompany.GLCo == poItem.GLCo);
//            var line = new APDocumentLine
//            {
//                Header = this,
//                PurchaseOrder = poItem.PurchaseOrder,
//                POItem = poItem,

//                APCo = APCo,
//                DocId = DocId,
//                SeqId = SeqId,
//                LineId = (short)(Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1),
//                LineTypeId = (byte)APLineTypeEnum.PO,
//                UM = poItem.UM,
//                Description = poItem.Description,
//                MatlGroup = poItem.MatlGroupId,
//                GLCo = poItem.GLCo,
//                VendorGroupId = poItem.PurchaseOrder.VendorGroupId,
//                UnitCost = poItem.OrigUnitCost,
//                Units = poItem.OrigUnits,
//                GrossAmt = poItem.OrigCost,
//                TaxBasis = poItem.OrigCost,
//                TaxGroupId = poItem.TaxGroup,
//                TaxCodeId = poItem.TaxCodeId,
//                TaxTypeId = poItem.TaxTypeId,
//                TaxAmt = poItem.OrigTax,
//                PayType = 1,
//                MiscAmt = 0,
//            };

//            line.POCo = POCo;
//            line.PO = PO;
//            line.POItemId = poItem.POItemId;
//            line.POItemTypeId = poItem.ItemTypeId;

//            //var poItemLine = db.vPOItemLines.FirstOrDefault(f => f.POITKeyID == poItem.KeyID);
//            //if (poItemLine != null)
//            //{
//            //    line.POItemLine = poItemLine.POItemLine;
//            //}


//            switch ((POItemTypeEnum)poItem.ItemTypeId)
//            {
//                case POItemTypeEnum.Job:
//                    line.JCCo = poItem.JCCo;
//                    line.JobId = poItem.JobId;
//                    line.PhaseGroupId = poItem.PhaseGroupId;
//                    line.PhaseId = poItem.PhaseId;
//                    line.JCCType = poItem.JCCType;
//                    line.Job = poItem.Job;
//                    line.JobPhase = poItem.JobPhase;
//                    line.JobPhaseCost = poItem.JobPhase.AddCostType((byte)poItem.JCCType);

//                    line.DivisionId = poItem.Job.Division.WPDivision.DivisionId;
//                    line.GLCo = poItem.Job.JCCompanyParm.GLCo;

//                    break;
//                case POItemTypeEnum.Expense:
//                    line.GLCo = poItem.GLCo;
//                    line.DivisionId = division.DivisionId;
//                    break;
//                case POItemTypeEnum.Equipment:

//                    line.EMCo = poItem.EMCo;
//                    line.EquipmentId = poItem.EquipmentId;
//                    line.CostCodeId = poItem.CostCodeId;
//                    line.EMCType = poItem.EMCType;
//                    line.EMGroupId = poItem.EMGroupId;
//                    line.DivisionId = poItem.Equipment.DivisionId;
//                    line.GLCo = poItem.Equipment.EMCompanyParm.GLCo;

//                    break;
//                default:
//                    break;
//            }

//            line.GLAcct = poItem.GLAcct;

//            Lines.Add(line);

//            return line;
//        }

//        public APDocumentLine AddLine()
//        {
//            var line = new APDocumentLine
//            {
//                Header = this,
//                APCo = APCo,
//                DocId = DocId,
//                SeqId = SeqId,
//                LineId = (short)(Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1),
//                LineTypeId = (byte)APLineTypeEnum.Job,

//                GLCo = Document.APCompanyParm.GLCo,
//                VendorGroupId = VendorGroupId,
//                UM = "LS",
//                Description = Description,
//                MatlGroup = Document.APCompanyParm.HQCompanyParm.MatlGroupId,
//                PayType = 1,
//                MiscAmt = 0,

//                GrossAmt = InvTotal - Lines.DefaultIfEmpty().Sum(sum => sum == null ? 0 : sum.GrossAmt),
//                tDivisionId = DivisionId,
//                Division = Division,
//            };
//            if (PO != null)
//            {
//                line.POCo = POCo;
//                line.PO = PO;
//                line.LineTypeId = (byte)APLineTypeEnum.PO;
//            }
//            Lines.Add(line);
//            return line;
//        }
//    }
//}
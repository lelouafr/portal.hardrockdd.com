using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.EmailModels
{
    public class PORequestUpdateEmailViewModel
    {
        public PORequestUpdateEmailViewModel()
        {

        }

        public PORequestUpdateEmailViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            RequestId = entity.RequestId;
            OrderedDate = entity.OrderedDate;
            PO = entity.PO;
            Status = (DB.PORequestStatusEnum)entity.Status;
            Description = entity.Description;

            VendorGroupId = entity.VendorGroup ?? entity.POCompanyParm.HQCompanyParm.VendorGroupId;
            VendorId = entity.VendorId;
            JobId = entity.JobId;
            PRCo = entity.OrderedByCo;
            EmployeeId = entity.OrderedBy;

            NewVendorName = entity.NewVendorName;
            NewVendorPhone = entity.NewVendorPhone;
            NewVendorAddress = entity.NewVendorAddress;

            if (entity.Vendor != null)
            {
                VendorName = entity.Vendor.Name;
            }

            POAmount = entity.Lines.Sum(sum => (sum.Cost ?? 0) + (sum.TaxAmount ?? 0));

            Lines = entity.Lines.Select(s => new PORequestLineEmailViewModel(s)).ToList();
        }

        public byte POCo { get; set; }

        public int RequestId { get; set; }

        public DB.PORequestStatusEnum Status { get; set; }

        public byte? PRCo { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime OrderedDate { get; set; }

        public string PO { get; set; }

        public string Description { get; set; }

        public byte? VendorGroupId { get; set; }

        public int? VendorId { get; set; }

        public string NewVendorName { get; set; }

        public string VendorName { get; set; }

        public string NewVendorPhone { get; set; }

        public string NewVendorAddress { get; set; }

        public string JobId { get; set; }

        public decimal? POAmount { get; set; }

        public List<PORequestLineEmailViewModel> Lines { get; set; }
    }

    public class PORequestLineEmailViewModel
    {
        public PORequestLineEmailViewModel()
        {

        }

        public PORequestLineEmailViewModel(DB.Infrastructure.ViewPointDB.Data.PORequestLine entry)
        {
            //POCo = entry.POCo;
            //RequestId = entry.RequestId;
            //LineId = entry.LineId;
            //Status = (DB.PORequestStatusEnum)entry.Request.Status;
            ItemTypeId = (DB.POItemTypeEnum)entry.ItemTypeId;
            //CalcType = entry.UM == "LS" ? DB.POCalcTypeEnum.LumpSum : DB.POCalcTypeEnum.Units;
            Description = entry.Description;

            //JCCo = entry.JCCo ?? 0;
            //JobId = entry.JobId;
            //CrewId = entry.CrewId;
            //PhaseGroupId = entry.PhaseGroupId ?? entry.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
            //PhaseId = entry.PhaseId;
            JobCostTypeId = entry.JCCType;

            //VendorGroupId = entry.Request.VendorGroup ?? entry.Request.Vendor?.VendorGroupId;
            //VendorId = entry.Request.VendorId;

            //EMGroupId = entry.EMGroupId ?? entry.Equipment?.EMCompanyParm?.HQCompanyParm?.EMGroupId;
            //EMCo = entry.EMCo ?? 0;
            //EquipmentId = entry.EquipmentId;
            //CostCodeId = entry.CostCodeId;
            CostTypeId = entry.EMCType;

            switch (ItemTypeId)
            {
                case DB.POItemTypeEnum.Job:
                    if (entry.Job != null)
                    {
                        ClassDescription = string.Format(DB.Infrastructure.ViewPointDB.Data.VPContext.AppCultureInfo, "Job:{0}", entry.Job.JobId);
                    }
                    break;
                case DB.POItemTypeEnum.Expense:
                    if (entry.GLAcct != null)
                    {
                        ClassDescription = string.Format(DB.Infrastructure.ViewPointDB.Data.VPContext.AppCultureInfo, "Exp:{0}", entry.GLAcct);
                    }
                    break;
                case DB.POItemTypeEnum.Equipment:
                    if (entry.Equipment != null)
                    {
                        ClassDescription = string.Format(DB.Infrastructure.ViewPointDB.Data.VPContext.AppCultureInfo, "Eqp:{0}", entry.Equipment.EquipmentId);
                    }
                    break;
                default:
                    ClassDescription = "";
                    break;
            }

            //GLCo = entry.GLCo ?? entry.Request.Company.GLCo;
            //GLAcct = entry.GLAcct;

            //Units = entry.Units;
            //UM = entry.UM;
            //UnitCost = entry.UnitCost;
            Cost = entry.Cost;

            //TaxGroupId = entry.Request.POCompanyParm.HQCompanyParm.TaxGroupId;
            //TaxTypeId = (DB.TaxTypeEnum)(entry.TaxTypeId ?? 0);
            //TaxCodeId = entry.TaxCodeId;
            //TaxRate = entry.TaxRate;
            TaxAmount = entry.TaxAmount;
            //if (TaxTypeId != DB.TaxTypeEnum.None && TaxAmount == 0)
            //{
            //    TaxAmount = Cost * TaxRate;
            //}


            //Comments = entry.Comments;
        }
        public DB.POItemTypeEnum ItemTypeId { get; set; }
        public string Description { get; set; }
        public string ClassDescription { get; set; }
        public decimal? Cost { get; set; }
        public decimal? TaxAmount { get; set; }
        public byte? JobCostTypeId { get; set; }
        public byte? CostTypeId { get; set; }
    }
}

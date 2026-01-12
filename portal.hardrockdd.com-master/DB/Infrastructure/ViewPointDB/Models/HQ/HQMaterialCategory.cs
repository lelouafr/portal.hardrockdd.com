using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQMaterialCategory
    {
        public VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.HQGroup.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public HQMaterial AddMaterial(string code, string description, string um, string materialType)
        {
            var materialId = string.Format("{0}-{1}", this.CategoryId, code);
            var mtl = this.Materials.FirstOrDefault(f => f.MaterialId == materialId);

            if (mtl == null)
            {
                mtl = new HQMaterial()
                {
                    MatlGroupId = this.MatlGroupId,
                    CategoryId = this.CategoryId,
                    MaterialId = materialId,
                    Description = description,
                    StdUM = um,
                    Cost = 0,
                    CostECM = "E",
                    Price = 0,
                    PriceECM = "E",
                    PayDiscType = "N",
                    PurchaseUM = um,
                    SalesUM = um,
                    Stocked = "N",
                    Taxable = "N",
                    Active = "Y",
                    Type = materialType,
                    PayDiscRate = 0,
                };
                if (materialType == "E")
                {
                    mtl.EMCostCode = "100";
                    mtl.EMCostType = 4;
                }
                this.Materials.Add(mtl);
            }

            return mtl;
           
        }
    }
}
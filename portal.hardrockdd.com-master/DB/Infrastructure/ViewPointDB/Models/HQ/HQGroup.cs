using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HQGroup
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
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public HQMaterialCategory AddMaterialCategory(string category, string description)
        {
            var cat = this.HQMaterialCategories.FirstOrDefault(f => f.CategoryId == category);
            if (cat == null)
            {
                cat = new HQMaterialCategory()
                {
                    MatlGroupId = this.GroupId,
                    CategoryId = category,
                    Description = description,
                };
                this.HQMaterialCategories.Add(cat);
            }

            return cat;
        }
    }
}
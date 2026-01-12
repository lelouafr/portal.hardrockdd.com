using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class EquipmentCategory
    {
        private IEnumerable<Equipment> _ActiveEquipments;
        public IEnumerable<Equipment> ActiveEquipments
        {
            get
            {
                if (_ActiveEquipments == null)
                {
                    _ActiveEquipments = this.Equipments.Where(f => f.Status == "A");
                }
                return _ActiveEquipments;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Equipment
    {
        private VPEntities _db;

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
                        _db = this.EMCompanyParm.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        private string _DisplayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_DisplayName))
                {
                    _DisplayName = string.Format("{0}: {1}", EquipmentId, Description);
                }
                return _DisplayName;
            }
        }

        public List<EMAuditLine> ActiveAuditLines
        {
            get
            {
                var lines = AuditLines.ToList().Where(f => f.Audit.Status == DB.EMAuditStatusEnum.New ||
                                                  f.Audit.Status == DB.EMAuditStatusEnum.Submitted ||
                                                  f.Audit.Status == DB.EMAuditStatusEnum.Started ||
                                                  f.Audit.Status == DB.EMAuditStatusEnum.Rejected).ToList();

                return lines;
            }
        }
    }

    public interface IEquipment
    {
        public string EquipmentId { get; set; }

        public void UpdateEquipment(string newValue);

        //public string CostCodeId { get; set; }

        //public void UpdateEquipmentCostCodeId(string newValue);

        //public byte? EMCType { get; set; }

        //public void UpdateEquipmentCostTypeInfo(byte? newValue);
    }
}
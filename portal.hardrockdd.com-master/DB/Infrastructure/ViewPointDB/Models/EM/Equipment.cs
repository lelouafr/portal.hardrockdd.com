using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Equipment
    {
        private VPContext _db;

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

        public EMMeterTypeEnum MeterType { get => (EMMeterTypeEnum)(byte.TryParse(MeterTypeId, out byte val) ? val : 0); set => MeterTypeId = ((byte)value).ToString(); }

        public List<EMAuditLine> ActiveAuditLines
        {
            get
            {
                var lines = AuditLines.ToList().Where(f => f.Audit.Status == EMAuditStatusEnum.New ||
                                                  f.Audit.Status == EMAuditStatusEnum.Submitted ||
                                                  f.Audit.Status == EMAuditStatusEnum.Started ||
                                                  f.Audit.Status == EMAuditStatusEnum.Rejected).ToList();

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
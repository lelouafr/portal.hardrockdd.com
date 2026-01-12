using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class EMAuditLine
    {

        public EMAuditLineActionEnum Action
        {
            get
            {
                return (EMAuditLineActionEnum)ActionId;
            }
            set
            {
                if ((byte)value != ActionId)
                {
                    ActionId = (byte)(value);
                }
                ActionId = (byte)(value);
            }
        }

        public EMMeterTypeEnum MeterType
        {
            get
            {
                if (int.TryParse(MeterTypeId, out int MeterTypeIdOut))
                {
                    return (EMMeterTypeEnum)MeterTypeIdOut;
                }
                MeterTypeId = "0";
                return EMMeterTypeEnum.None;
            }
            set
            {
                var valStr = ((byte)value).ToString(VPContext.AppCultureInfo);
                if (valStr != MeterTypeId)
                {
                    MeterTypeId = valStr;
                }
                MeterTypeId = valStr;
            }
        }
    }
}
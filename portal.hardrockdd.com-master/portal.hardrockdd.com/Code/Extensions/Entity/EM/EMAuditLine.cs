using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class EMAuditLine
    {

        public DB.EMAuditLineActionEnum Action
        {
            get
            {
                return (DB.EMAuditLineActionEnum)ActionId;
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

        public DB.EMMeterTypeEnum MeterType
        {
            get
            {
                if (int.TryParse(MeterTypeId, out int MeterTypeIdOut))
                {
                    return (DB.EMMeterTypeEnum)MeterTypeIdOut;
                }
                MeterTypeId = "0";
                return DB.EMMeterTypeEnum.None;
            }
            set
            {
                var valStr = ((byte)value).ToString(AppCultureInfo.CInfo());
                if (valStr != MeterTypeId)
                {
                    MeterTypeId = valStr;
                }
                MeterTypeId = valStr;
            }
        }
    }
}
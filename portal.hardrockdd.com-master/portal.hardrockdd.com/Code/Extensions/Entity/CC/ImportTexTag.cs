using System;using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class ImportTexTag
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
                        _db = this.Import.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        public void SetEquipment()
        {
            if (this.EMEquipment != null)
                return;
            IsError = false;

            var eqp = Import.EMEquipments.FirstOrDefault(f => f.TXTagId == TagNumber);
            if (eqp == null)
            { 
                eqp = Import.EMEquipments.FirstOrDefault(f => f.LicensePlateNo == TagNumber);
                if (eqp != null)
                {
                    eqp = eqp;
                }
            }
            if (eqp != null)
            {
                EMCo = eqp.EMCo;
                EMEquipmentId = eqp.EquipmentId;
                EMEquipment = eqp;
            }

            if (EMEquipment == null)
                IsError = true;
        }

        public string UniqueTransId 
        { 
            get
            {
                var result = string.Format("{0}", TransactionNumber);
                return result;
            }
            
        }

        public DateTime TransDateTime 
        { 
            get
            {
                var dateStringArray = TransDateStr.Split('/');

                var dt = DateTime.TryParse(dateStringArray[0], out DateTime dtOut)? dtOut : DateTime.Now;
                var t = TimeSpan.TryParse(dateStringArray[1], out TimeSpan tOut) ? tOut : new TimeSpan(0);
               
                return dt.Add(t);
            }

        }

        public decimal AmountConv()
        {
            return decimal.TryParse(this.AmountStr, System.Globalization.NumberStyles.Currency, AppCultureInfo.CInfo().NumberFormat, out decimal outDec) ? outDec : 0;
        }

        public DateTime Mth 
        { 
            get
            {
                return new DateTime(TransDateTime.Year, TransDateTime.Month, 1);
            }
        }


    }
}
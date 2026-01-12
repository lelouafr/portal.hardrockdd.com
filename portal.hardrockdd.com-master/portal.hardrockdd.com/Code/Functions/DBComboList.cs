using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace portal
{
    public static partial class StaticFunctions
    {
        public static List<ComboValue> GetComboValues(string ComboType)
        {

            var memKey = "GetComboValues_" + ComboType;
            if (!(MemoryCache.Default[memKey] is List<ComboValue> list))
            {
                using var db = new VPContext();
                list = db.ComboValues.Where(f => f.ComboType == ComboType).ToList();
                var custlist = db.ComboCustomValues.Where(f => f.ComboType == ComboType).AsEnumerable().Select(s => new ComboValue()
                {
                    ComboType = s.ComboType,
                    DatabaseValue = s.DatabaseValue,
                    Seq = s.Seq,
                    DisplayValue = s.DisplayValue
                }).ToList();
                list.AddRange(custlist);
                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
                };
                systemCache.Set(memKey, list, policy);
            }
            list.ForEach(f => f.DisplayValue = f.DisplayValue.Replace(string.Format(AppCultureInfo.CInfo(), "{0}-", f.DatabaseValue), string.Empty));
            return list;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace portal
{
    public static class NameValueCollentionExtension
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            if (col == null)
            {
                throw new System.ArgumentNullException(nameof(col));
            }
            var result = new Dictionary<string, string>();
            foreach (var item in col.AllKeys)
            {
                try
                {
                    result.Add(item, col[item]);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
            return result;
        }
    }
}
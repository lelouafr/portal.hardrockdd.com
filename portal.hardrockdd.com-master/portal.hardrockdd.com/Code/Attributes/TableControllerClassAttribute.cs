using System;

namespace portal
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TableControllerClassAttribute : Attribute
    {

        public TableControllerClassAttribute()
        {
            AllowAdd = false;
            AllowDelete = false;
            AllowUpdate = false;
            AllowValidate = false;
        }

        public string Controller { get; set; }

        public bool AllowAdd { get; set; }

        public bool AllowDelete { get; set; }

        public bool AllowUpdate { get; set; }

        public bool AllowValidate { get; set; }


    }
}
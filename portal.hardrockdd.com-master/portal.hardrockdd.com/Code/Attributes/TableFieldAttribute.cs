using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TableFieldAttribute : Attribute
    {
        public TableFieldAttribute()
        {
            Width = "10%";
            IconClass = "";
            Placeholder = "";
            EditorTemplate = "TextBox";
            InternalTableRow = 1;
        }

        //public OptionListEnum OptionListType { get; set; }

        //public SelectListTypeEnum SelectListType { get; set; }

        public int InternalTableRow { get; set; }

        public string Placeholder { get; set; }

        public string EditorTemplate { get; set; }

        public string Width { get; set; }

        public string IconClass { get; set; }

    }
}
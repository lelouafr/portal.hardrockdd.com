using System;

namespace portal
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldAttribute : Attribute
    {
        public FieldAttribute()
        {
            LabelSize = 2;
            TextSize = 4;
            FormGroupRow = 100;
            FormGroup = "None";
            IconClass = "";
            Placeholder = "";
            CacheUrl = true;
            UpdateRelatedPanes = false;
            //SelectListViewBag = "";
            InternalTableRow = 1;
        }

        //public OptionListEnum OptionListType { get; set; }

        //public Dictionary<string, string> OptionLists
        //{
        //    get
        //    {
        //        switch (OptionListType)
        //        {
        //            case OptionListEnum.MaleFemale:
        //                return OptionListModel.MaleFemaleOptions;
        //            case OptionListEnum.YesNo:
        //                return OptionListModel.YesNoOptions;
        //            default:
        //                break;
        //        }

        //        return new Dictionary<string, string>();

        //    }
        //}

        public string Placeholder { get; set; }

        public string FormGroup { get; set; }

        public int FormGroupRow { get; set; }

        public bool UpdateRelatedPanes { get; set; }

        public int InternalTableRow { get; set; }

        public int LabelSize { get; set; }

        public int TextSize { get; set; }

        public string IconClass { get; set; }

        public string InfoUrl { get; set; }

        public string InfoForeignKeys { get; set; }

        public string AddUrl { get; set; }

        public string AddForeignKeys { get; set; }

        public string SearchUrl { get; set; }

        public string SearchForeignKeys { get; set; }

        public string EditUrl { get; set; }

        public string EditForeignKeys { get; set; }

        public bool CacheUrl { get; set; }

        public string ComboUrl { get; set; }

        public string ComboForeignKeys { get; set; }



        public string ForeignKeys { get; set; }
    }
}
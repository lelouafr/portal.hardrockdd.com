using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PMLocate_Import
    {
        private DateTime? _StartDateDT;
        public DateTime? StartDateDT
        {
            get
            {
                if (_StartDateDT == null)
                {
                    if (DateTime.TryParse(this.StartDate, out DateTime outStartDate))
                        _StartDateDT = outStartDate;
                }
                return _StartDateDT;
            }
            set => _StartDateDT = value;
        }

        private DateTime? _EndDateDT;
        public DateTime? EndDateDT
        {
            get
            {
                if (_EndDateDT == null)
                {
                    if (DateTime.TryParse(this.EndDate, out DateTime outEndDate))
                        _EndDateDT = outEndDate;
                }
                return _EndDateDT;
            }
            set => _EndDateDT = value;
        }

        private DateTime? _OriginalDateDT;
        public DateTime? OriginalDateDT
        {
            get
            {
                if (_OriginalDateDT == null)
                {
                    if (DateTime.TryParse(this.OriginalDate, out DateTime outOriginalDate))
                        _OriginalDateDT = outOriginalDate;
                }
                return _OriginalDateDT;
            }
            set => _OriginalDateDT = value;
        }
    }

    public class PMLocate_ImportContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }
        private Dictionary<string, string> PropertyMappings2 { get; set; }

        public PMLocate_ImportContractResolver()
        {
            this.PropertyMappings = new Dictionary<string, string>
            {
                //{ "RefId", "TEXAS 811"},
                { "Description", "Description"},
                { "Owner", "Owner"},
                { "ProjectName", "Project Name"},
                { "Comments", "Comments"},
                { "General", "General"},
                { "OriginalDate", "Original Request Date"},
                { "StartDate", "Start Date"},
                { "EndDate", "Expiration"},
                { "GPS", "GPS Coordinates"},
                { "RequestName", "Requested By"},
            };
            this.PropertyMappings2 = new Dictionary<string, string>
            {
               // { "RefId", "811" },
                { "Description", "Description"},
                { "Owner", "Owner"},
                { "ProjectName", "Project Name"},
                { "Comments", "Comments"},
                { "General", "General"},
                { "OriginalDate", "Original Request Date"},
                { "StartDate", "Start Date"},
                { "EndDate", "Expiration"},
                { "GPS", "GPS Coordinates"},
                { "RequestName", "Requested By"},
            };

            //{ "RefId", "TEXAS 811"},
            //    { "RefId", "811" },
            //    { "Description", "Description"},
            //    { "Owner", "Owner"},
            //    { "ProjectName", "Project Name"},
            //    { "Comments", "Comments"},
            //    { "General", "General"},
            //    { "OriginalDate", "Original Request Date"},
            //    { "StartDate", "Start Date"},
            //    { "EndDate", "Expiration"},
            //    { "GPS", "GPS Coordinates"},
            //    { "Requested By", "RequestName"},
            
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            if(!resolved)
                resolved = this.PropertyMappings2.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }

    }

}

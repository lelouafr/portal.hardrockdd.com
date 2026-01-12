using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Crew
    {

        private string _DisplayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_DisplayName))
                {
                    _DisplayName = string.Format("{0}: {1}", CrewId, Description);
                }
                return _DisplayName;
            }
        }
    }

    public static class CrewExtension
    {
        public static string Label(this Crew crew)
        {
            if (crew == null)
                return "";

            var label = crew.Description;
            label = label.Replace("-", "");
            label = label.Replace("Crew", "");
            label = label.Replace("Trucking", "");
            label = label.Replace("WIRELINE", "");
            label = label.Trim();

            return label;
        }

    }
}
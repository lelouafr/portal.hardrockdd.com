using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WPExcelSheet
    {
        public VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }
        public static string BaseTableName { get { return "budWPEI"; } }

        public string GetJson()
        {
            var json = System.Text.Encoding.Default.GetString(JsonData);
            return json;
        }
        public DataSet GetDataSet()
        {
            var json = System.Text.Encoding.Default.GetString(JsonData);
            return JsonConvert.DeserializeObject<DataSet>(json);
        }

        public List<PMLocate_Import> PMLocateData()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PMLocate_ImportContractResolver()
            };
            var json = GetJson();

            while (json.Contains("  "))
            {
                json = json.Replace("  ", " ");
            }
            json = json.Replace("Column 0", "RefId");
            json = json.Replace("TEXAS 811", "RefId");
            json = json.Replace("Texas 811", "RefId");
            json = json.Replace("\"811\":", "\"RefId\":");

            json = json.Replace("Lone Star 811", "RefId2");
            json = json.Replace("EXPIRE DATE", "Expiration");
            json = json.Replace("GPS Coordinates","GPS Coordinates");
            if (!json.Contains("\"RefId\""))
                json = json.Replace("\" \":", "\"RefId\":");

            //json = json.Replace("Lone Star 811", "RefId2");
            //json = json.Replace("811", "RefId3");
            //dynamic parsedJson = JsonConvert.DeserializeObject(json);
            //return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            
            

            //JObject obj = JObject.Parse(json);
            
            var list = JsonConvert.DeserializeObject<List<PMLocate_Import>>(json, settings);
            list = list.Where(f => f.Description != null ||
                                    f.General != null ||
                                    f.ProjectName != null ||
                                    f.RequestName != null).ToList();
            return list;
        }

    }
}

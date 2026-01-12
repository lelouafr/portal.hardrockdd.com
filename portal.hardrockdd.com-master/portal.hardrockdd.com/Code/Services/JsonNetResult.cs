using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;

namespace portal.Code.Services
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {

        }

        public JsonNetResult(object data)
        {
            Data = data;
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public JsonNetResult(object data, JsonRequestBehavior behavior)
        {
            Data = data;
            this.JsonRequestBehavior = behavior;
        }

        //public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting.Indented };
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
                serializer.Serialize(writer, Data);
                writer.Flush();

                
            }
        }
    }
}
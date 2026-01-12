using System.Web.Http;

namespace portal.Controllers.API.Register
{
    //[APIAuthorizeDevice]
    public class ConnectTestController : ApiController
    {
        [HttpGet]
        public bool Get()
        {            
            return true;
        }
    }
}

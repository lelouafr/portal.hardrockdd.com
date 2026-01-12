using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,FIN-APMGR,FIN-CTRL,OP-GM,OP-DM,IT-DIR,HR-MGR,OP-EQMGR")]
    public class VideoController : BaseController
    {
        // GET: Video
        [HttpGet]
        public ActionResult Index()
        {
            //return View();
            return Redirect("https://video.ui.com/");
        }

        [HttpGet]
        // GET: Video
        [Route("Video/Monahans")]
        public ActionResult Monahans()
        {
            //return View("../Video/Monahans/Index");

            return Redirect("https://unifi.ui.com/device/ec7610a2-15e5-4105-9505-f8f470f64d6c/protect/");
        }
        [HttpGet]
        // GET: Video
        [Route("Video/Pearsall")]
        public ActionResult Pearsall()
        {
            //return View("../Video/Monahans/Index");

            return Redirect("https://unifi.ui.com/device/baffa644-72b9-4e2e-8dba-4bfef1f1f139/protect/");
        }

        [HttpGet]
        // GET: Video
        [Route("Video/SaShop")]
        public ActionResult SaShop()
        {
            //return View("../Video/SaShop/Index");
            return Redirect("https://unifi.ui.com/device/D021F98948F500000000065491D20000000006A0337800000000623C3AF2:1578703907/protect/");
        }
    }
}
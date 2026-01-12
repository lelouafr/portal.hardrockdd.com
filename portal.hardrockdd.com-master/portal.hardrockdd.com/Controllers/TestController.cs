using portal.Models.Views.Dashboard;
using System;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class TestController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult Forbidden()
        {
            return new HttpStatusCodeResult(403);
        }

        [HttpGet]
        public ActionResult Other()
        {
            throw new InvalidOperationException();
        }

    }
}
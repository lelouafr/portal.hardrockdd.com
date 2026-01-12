using DB.Infrastructure.ViewPointDB.Data;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.StyledXmlParser.Css.Media;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.Bid
{
    public class BidFormController : BaseController
    {
        // GET: Bid
        [HttpGet]
        [Route("Bid/{bidId}-{bdco}")]
        public ActionResult Index(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        public ActionResult BidPopupForm(byte bdco, int bidId)
        {
            return RedirectToAction("PopUp", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        public ActionResult Create()
        {
            return RedirectToAction("Create", "Bid", new { Area = "Project" });
        }

        [HttpGet]
        public ActionResult CreateQuickBid()
        {
            return RedirectToAction("Create", "Bid", new { Area = "Project" });
        }
    }
}
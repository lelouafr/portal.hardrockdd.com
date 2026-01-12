using System.Web.Mvc;

namespace portal.Controllers.View.Bid
{
    public class BidWizardController : BaseController
    {
        [HttpGet]
        [Route("Bid/Wizard/{bidId}-{bdco}")]
        public ActionResult Index(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        public ActionResult SalesWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }
        
        [HttpGet]
        public ActionResult EstimateWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        public ActionResult SalesReviewWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        public ActionResult FinalReviewWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        [Route("Bid/Wizard/Proposal/{bidId}-{bdco}")]
        public ActionResult ProposalWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

        [HttpGet]
        [Route("Bid/Wizard/Award/{bidId}-{bdco}")]
        public ActionResult AwardWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }


        [HttpGet]
        [Route("Bid/Wizard/ContractApproval/{bidId}-{bdco}")]
        public ActionResult ContractApprovalWizard(byte bdco, int bidId)
        {
            return RedirectToAction("Index", "Bid", new { Area = "Project", bdco, bidId });
        }

    }
}
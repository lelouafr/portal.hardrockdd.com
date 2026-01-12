using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Wizard.Base
{
    public class BidWizardFormViewModel
    {
        public BidWizardFormViewModel()
        {

        }

        public BidWizardFormViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid, Controller controller)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            StatusInt = bid.StatusId;
            BidTypeId = (DB.BidTypeEnum)(bid.BidType ?? 0);
            #endregion

            WizardSteps = BuildSteps(controller);
            WizardActions = BuildActions(controller);
            Attachments = new Attachment.AttachmentListViewModel(bid.BDCo, "udBDBH", bid.KeyID, bid.UniqueAttchID);
            Forum = new Forums.ForumLineListViewModel(bid.Forum);
            WorkFlowUsers = new WF.WorkFlowUserListViewModel(bid.WorkFlow.CurrentSequance());

        }


        public BidWizardFormViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            StatusInt = bid.StatusId;
            BidTypeId = (DB.BidTypeEnum)(bid.BidType ?? 0);
            #endregion

            WorkFlowUsers = new WF.WorkFlowUserListViewModel(bid.WorkFlow.CurrentSequance());
        }

        public List<WizardSteps> BuildSteps(Controller controller)
        {
            var baseSteps = new List<WizardSteps>();

            baseSteps.Add(new Base.WizardSteps { Title = "Project Info", AjaxUrl = controller.Url.Action("InfoPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
            baseSteps.Add(new Base.WizardSteps { Title = "Sub Projects", AjaxUrl = controller.Url.Action("PackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });

            if (BidTypeId != DB.BidTypeEnum.QuickBid)
            {
                switch ((DB.BidStatusEnum)StatusInt)
                {
                    case DB.BidStatusEnum.Draft:
                        break;
                    case DB.BidStatusEnum.Estimate:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.SalesReview:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.FinalReview:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.Proposal:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.PendingAward:
                        //baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Award Packages", AjaxUrl = controller.Url.Action("PackageAwardListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.ContractReview:
                        //baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Customer Assignment", AjaxUrl = controller.Url.Action("PackageAwardListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.ContractApproval:
                        //baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.Awarded:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.NotAwarded:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.Canceled:
                        break;
                    case DB.BidStatusEnum.Deleted:
                        break;
                    case DB.BidStatusEnum.Template:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "", bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "", bdco = BDCo, bidId = BidId }) });
                         
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch ((DB.BidStatusEnum)StatusInt)
                {
                    case DB.BidStatusEnum.Draft:
                    case DB.BidStatusEnum.Estimate:
                    case DB.BidStatusEnum.SalesReview:
                    case DB.BidStatusEnum.FinalReview:
                    case DB.BidStatusEnum.Proposal:
                    case DB.BidStatusEnum.PendingAward:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.ContractReview:
                        baseSteps.Add(new Base.WizardSteps { Title = "Customer Assignment", AjaxUrl = controller.Url.Action("PackageAwardListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.ContractApproval:
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.Awarded:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Scheduling", AjaxUrl = controller.Url.Action("PackageSchedulePanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.NotAwarded:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        //baseSteps.Add(new Base.WizardSteps { Title = "Cost Summary", AjaxUrl = controller.Url.Action("PackageSummaryListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Pricing/Summary", AjaxUrl = controller.Url.Action("PackagePricingListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Review Bid Proposal", AjaxUrl = controller.Url.Action("BidProposalPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Job Assignment", AjaxUrl = controller.Url.Action("PackageJobSetupListPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
                        break;
                    case DB.BidStatusEnum.Canceled:
                        break;
                    case DB.BidStatusEnum.Deleted:
                        break;
                    case DB.BidStatusEnum.Template:
                        baseSteps.Add(new Base.WizardSteps { Title = "Packages Setup", AjaxUrl = controller.Url.Action("PackageSetupListPanel", "BidForm", new { Area = "", bdco = BDCo, bidId = BidId }) });
                        baseSteps.Add(new Base.WizardSteps { Title = "Bore Details", AjaxUrl = controller.Url.Action("BoreSetupPackageListPanel", "BidForm", new { Area = "", bdco = BDCo, bidId = BidId }) });

                        break;
                    default:
                        break;
                }
            }
            baseSteps.Add(new Base.WizardSteps { Title = "Submit", AjaxUrl = controller.Url.Action("BidActionPanel", "BidForm", new { Area = "",bdco = BDCo, bidId = BidId }) });
            var stepId = 1;
            baseSteps.ForEach(e => e.StepId = stepId++);

            return baseSteps;
        }


        public List<WizardAction> BuildActions(Controller controller)
        {
            var results = new List<WizardAction>();

            if (BidTypeId != DB.BidTypeEnum.QuickBid)
            {
                switch ((DB.BidStatusEnum)StatusInt)
                {
                    case DB.BidStatusEnum.Draft:
                        results.Add(new WizardAction() { Title = "Submit your bid", GotoStatusId = DB.BidStatusEnum.Estimate, ButtonClass = "btn-primary", ActionRedirect="BidTracker" });
                        break;
                    case DB.BidStatusEnum.Estimate:
                        results.Add(new WizardAction() { Title = "Send Back to Draft", GotoStatusId = DB.BidStatusEnum.Draft, ButtonClass= "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send for Sales Review", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.SalesReview:
                        results.Add(new WizardAction() { Title = "Send Back to Draft", GotoStatusId = DB.BidStatusEnum.Draft, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Estimate", GotoStatusId = DB.BidStatusEnum.Estimate, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send for Final Review", GotoStatusId = DB.BidStatusEnum.FinalReview, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.FinalReview:
                        results.Add(new WizardAction() { Title = "Send Back to Draft", GotoStatusId = DB.BidStatusEnum.Draft, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Estimate", GotoStatusId = DB.BidStatusEnum.Estimate, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Sales Review", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send for Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.Proposal:
                        results.Add(new WizardAction() { Title = "Send Back to Draft", GotoStatusId = DB.BidStatusEnum.Draft, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Estimate", GotoStatusId = DB.BidStatusEnum.Estimate, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Sales Review", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send Back to Final Review", GotoStatusId = DB.BidStatusEnum.FinalReview, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Proposal Sent", GotoStatusId = DB.BidStatusEnum.PendingAward, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Award Bid", GotoStatusId = DB.BidStatusEnum.PendingAward, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.PendingAward:
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Send for Contract Review", GotoStatusId = DB.BidStatusEnum.ContractReview, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.ContractReview:
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Approve Contract", GotoStatusId = DB.BidStatusEnum.ContractApproval, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.ContractApproval:
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Create/Apply Jobs and Award", GotoStatusId = DB.BidStatusEnum.Awarded, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.Awarded:
                        results.Add(new WizardAction() { Title = "Send Back to Approve Contract", GotoStatusId = DB.BidStatusEnum.ContractApproval, ButtonClass = "btn-warning", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Create/Apply Updates to Project", GotoStatusId = DB.BidStatusEnum.Update, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        break;
                    case DB.BidStatusEnum.NotAwarded:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Canceled:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Deleted:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Template:
                        break;
                    default:
                        break;
                    }
            }
            else
            {
                switch ((DB.BidStatusEnum)StatusInt)
                {
                    case DB.BidStatusEnum.Draft:
                    case DB.BidStatusEnum.Estimate:
                    case DB.BidStatusEnum.SalesReview:
                    case DB.BidStatusEnum.FinalReview:
                    case DB.BidStatusEnum.Proposal:
                    case DB.BidStatusEnum.PendingAward:
                        results.Add(new WizardAction() { Title = "Send for Contract Review", GotoStatusId = DB.BidStatusEnum.ContractReview, ButtonClass = "btn-primary", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Close Bid", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.ContractReview:
                        results.Add(new WizardAction() { Title = "Send Back", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Approve Contract", GotoStatusId = DB.BidStatusEnum.ContractApproval, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.ContractApproval:
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "BidTracker" });
                        results.Add(new WizardAction() { Title = "Create/Apply Jobs and Award", GotoStatusId = DB.BidStatusEnum.Awarded, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Bid Not Awarded (Close Bid)", GotoStatusId = DB.BidStatusEnum.NotAwarded, ButtonClass = "btn-danger", ActionRedirect = "BidTracker" });
                        break;
                    case DB.BidStatusEnum.Awarded:
                        results.Add(new WizardAction() { Title = "Send Back to Approve Contract", GotoStatusId = DB.BidStatusEnum.ContractApproval, ButtonClass = "btn-warning", ActionRedirect = "Home" });
                        results.Add(new WizardAction() { Title = "Send Back to Proposal", GotoStatusId = DB.BidStatusEnum.Proposal, ButtonClass = "btn-warning", ActionRedirect = "Reload" });
                        results.Add(new WizardAction() { Title = "Create/Apply Updates to Project", GotoStatusId = DB.BidStatusEnum.Update, ButtonClass = "btn-primary", ActionRedirect = "Home" });
                        break;
                    case DB.BidStatusEnum.NotAwarded:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Canceled:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Deleted:
                        results.Add(new WizardAction() { Title = "Re-open Bid", GotoStatusId = DB.BidStatusEnum.SalesReview, ButtonClass = "btn-primary", ActionRedirect = "Reload" });
                        break;
                    case DB.BidStatusEnum.Template:
                        break;
                    default:
                        break;
                }
                //results.Add(new WizardAction() { Title = "Create Project", GotoStatusId = DB.BidStatusEnum.Awarded, ButtonClass = "btn-primary", ActionRedirect = "Home" });
            }
            return results;
        }
        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }


        public DB.BidTypeEnum BidTypeId { get; set; }

        public int StatusInt { get; set; }

        public List<WizardSteps> WizardSteps { get; set; }

        public List<WizardAction> WizardActions { get; set; }

        public WF.WorkFlowUserListViewModel WorkFlowUsers { get; set; }

        public Attachment.AttachmentListViewModel Attachments { get; set; }

        public Forums.ForumLineListViewModel Forum { get; set; }
    }


    public class WizardSteps
    {

        public int StepId { get; set; }

        public string  AjaxUrl { get; set; }

        public string Title { get; set; }
    }

    public class WizardAction
    {
        public int ActionId { get; set; }

        public string Title { get; set; }

        public string ButtonClass { get; set; }

        public string ActionUrl { get; set; }

        public DB.BidStatusEnum GotoStatusId { get; set; }

        public string ActionRedirect { get; set; }
    }
}
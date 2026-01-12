using DB.Infrastructure.ViewPointDB.Data;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.StyledXmlParser.Css.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Project.Controllers
{
    [RouteArea("Project")]
    public class BidController : portal.Controllers.BaseController
    {
        #region Bid Form
        [HttpGet]
        [Route("Bid/{bidId}-{bdco}")]
        public ActionResult Index(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids
                .Include("Forum")
                .Include("WorkFlow")
                .Include("WorkFlow.Sequences")
                .Include("WorkFlow.Sequences.AssignedUsers")
                .FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            if (bid.Forum == null)
            {
                bid.Forum = bid.AddForum();
            }
            bid.AddWorkFlowAssignments();
            db.BulkSaveChanges();
            var controller = (Controller)this;
            var model = new Models.Bid.WizardFormViewModel(bid, controller);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return View("Wizard/Index", model);
        }

        [HttpGet]
        public ActionResult PopUp(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids
                .Include("Forum")
                .Include("WorkFlow")
                .Include("WorkFlow.Sequences")
                .Include("WorkFlow.Sequences.AssignedUsers")
                .FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            if (bid.Forum == null)
            {
                bid.Forum = bid.AddForum();
                db.BulkSaveChanges();
            }
            var controller = (Controller)this;
            var model = new Models.Bid.WizardFormViewModel(bid, controller);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Wizard/Index", model);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            using var db = new VPContext();
            var newObj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1).AddBid();

            return RedirectToAction("Index", new { bdco = newObj.BDCo, bidId = newObj.BidId });
        }
        #endregion

        #region Tracker
        [Route("Bid/Tracker/")]
        [HttpGet]
        public ActionResult Tracker()
        {
            var results = new Models.Bid.TrackerListViewModel();
            return View("List/Tracker/Index", results);
        }

        [HttpGet]
        public ActionResult TrackerTable()
        {
            var results = new Models.Bid.TrackerListViewModel();
            return PartialView("List/Tracker/_Table", results);
        }

        [HttpGet]
        public ActionResult TrackerData()
        {
            using var db = new VPContext();
            var results = new Models.Bid.TrackerListViewModel(db);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region User Bids
        [Route("Bid/Assigned/")]
        [HttpGet]
        public ActionResult UserBids()
        {
            using var db = new VPContext();
            var results = new Models.Bid.TrackerListViewModel(db.GetCurrentUser());
            return View("List/User/Index", results);
        }

        [HttpGet]
        public ActionResult UserBidsPanel()
        {
            using var db = new VPContext();
            var results = new Models.Bid.TrackerListViewModel(db.GetCurrentUser());
            return PartialView("List/User/_Panel", results);
        }


        [HttpGet]
        public ActionResult UserBidsTable()
        {
            using var db = new VPContext();
            var results = new Models.Bid.TrackerListViewModel(db.GetCurrentUser());
            return PartialView("List/User/_Table", results);
        }

        //[HttpGet]
        //public ActionResult UserBidsData()
        //{
        //    using var db = new VPContext();
        //    var results = new Models.Bid.TrackerListViewModel(db.GetCurrentUser());

        //    JsonResult result = Json(new
        //    {
        //        data = results.List.ToArray()
        //    }, JsonRequestBehavior.AllowGet);
        //    result.MaxJsonLength = int.MaxValue;
        //    return result;
        //}
        #endregion

        #region Bid Actions
        [HttpGet]
        public ActionResult ActionPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.WizardFormViewModel(bid, this);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Action/_Panel", model);
        }

        [HttpGet]
        public ActionResult ActionForm(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.WizardFormViewModel(bid, this);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Action/_Form", model);
        }


        [HttpGet]
        public ActionResult UpdateStatus(byte bdco, int bidId, int gotoStatusId, string ActionRedirect)
        {
            using var db = new VPContext();

            var controller = (Controller)this;
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            if (hasAccess || User.IsInRole("Admin"))
            {
                var originalStatusId = bid.Status;
                try
                {
                    bid.StatusId = gotoStatusId;
                    db.SaveChanges(ModelState);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    db.Entry(bid).Reload();
                    ActionRedirect = "Reload";
                }
            }
            return ActionRedirect switch
            {
                "Reload" => RedirectToAction("Index", new { bdco, bidId }),
                "BidTracker" => RedirectToAction("Tracker"),
                "Home" => RedirectToAction("Index", "Home", new {  Area = "" }),
                _ => RedirectToAction("Index", new { bdco, bidId }),
            };
        }
        #endregion

        #region Bid Info
        [HttpGet]
        public ActionResult InfoPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.BidInfoViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Info/_Panel", model);
        }

        [HttpGet]
        public ActionResult InfoForm(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.BidInfoViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Info/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoUpdate(Models.Bid.BidInfoViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult InfoValidate(Models.Bid.BidInfoViewModel model)
        {
            using var db = new VPContext();
            var obj = db.Bids.Where(f => f.BDCo == model.BDCo && f.BidId == model.BidId).FirstOrDefault();
            if (obj != null)
            {
                model = new Models.Bid.BidInfoViewModel(obj);
            }

            ModelState.Clear();
            TryValidateModel(model);
            if (obj.Customers.Count == 0)
            {
                ModelState.AddModelError("", "No customer listed");
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Bid Customer
        [HttpGet]
        public ActionResult CustomerPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.CustomerListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Customer/_Panel", model);
        }

        [HttpGet]
        public ActionResult CustomerTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.CustomerListViewModel(bid);

            //var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Bid.Status == w.Status && w.Active == "Y") ? "False" : "True";

            return PartialView("Forms/Customer/_Table", model);
        }

        [HttpGet]
        public PartialViewResult CustomerAdd(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var newObj = bid.AddCustomer();
            db.SaveChanges(ModelState);

            var result = new Models.Bid.CustomerViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("Forms/Customer/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerUpdate(Models.Bid.CustomerViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerDelete(byte bdco, int bidId, int lineId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            if (bid != null)
            {
                var delObj = bid.Customers.FirstOrDefault(f => f.LineId == lineId);
                if (delObj != null)
                {
                    bid.Customers.Remove(delObj);
                    db.SaveChanges(ModelState);
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult CustomerValidate(Models.Bid.CustomerViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Package Setup
        #region Package List
        [HttpGet]
        public ActionResult PackageSetupListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Setup/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageSetupTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Setup/List/_Table", model);
        }


        [HttpGet]
        public PartialViewResult PackageSetupAdd(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.AddPackage();

            db.BulkSaveChanges();

            var result = new Models.Bid.Package.PackageViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            ViewBag.tableRow = "True";
            return PartialView("Forms/Package/Setup/List/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageSetupUpdate(Models.Bid.Package.PackageViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageSetupDelete(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            if (bid != null)
            {
                var delObj = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
                if (delObj != null)
                {
                    bid.Packages.Remove(delObj);
                    db.SaveChanges(ModelState);
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageSetupValidate(Models.Bid.Package.PackageViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();

            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
            var boreList = new Models.Bid.Bore.BoreLineListViewModel(package);
            TryValidateModelRecursive(boreList);
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError("Description", "Review Bore Lines for Errors");
            }
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Bore List Setup
        [HttpGet]
        public ActionResult PackageSetupBoreListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.BoreLineListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Setup/BoreList/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageSetupBoreListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.BoreLineListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Setup/BoreList/_Table", model);
        }

        [HttpGet]
        public PartialViewResult PackageSetupBoreListAdd(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var newObj = package.AddBoreLine();
            db.SaveChanges(ModelState);

            var result = new Models.Bid.Bore.BoreLineViewModel(newObj);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;
            ViewBag.tableRow = "True";

            return PartialView("Forms/Package/Setup/BoreList/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageSetupBoreListUpdate(Models.Bid.Bore.BoreLineViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageSetupBoreListDelete(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            if (bore != null)
            {
                bore.Status = DB.BidStatusEnum.Deleted;
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageSetupBoreListValidate(Models.Bid.Bore.BoreLineViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Package Defaults
        #region Package List
        [HttpGet]
        public ActionResult PackageDefaultListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            bid.SetupDefaultsForPackages();
            db.BulkSaveChanges();

            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/_PanelList", model);
        }

        [HttpGet]
        public ActionResult PackageDefaultPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.DefaultFormView(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/_Panel", model);
        }
        #endregion

        #region Package Day Rounding
        [HttpGet]
        public ActionResult PackageDayForm(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.RoundDayViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/DefaultSetup/DayRounding/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageDayUpdate(Models.Bid.Package.RoundDayViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageDayValidate(Models.Bid.Package.RoundDayViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Package Setup Production Default Rates
        [HttpGet]
        public ActionResult PackageProductionRateListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.ProductionRateListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/ProductionRate/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageProductionRateListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.ProductionRateListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/ProductionRate/_Table", model);
        }

        [HttpGet]
        public ActionResult PackageProductionRateListApplyDefault(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
            package.ApplyProductionDefaults(true);

            return RedirectToAction("PackageProductionRateListTable", new { bdco, bidId, packageId });
        }

        [HttpGet]
        public PartialViewResult PackageProductionRateAdd(byte bdco, int bidId, int packageId)
        {

            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageProductionRateUpdate(Models.Bid.Package.ProductionRateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageProductionRateDelete(byte bdco, int bidId, int boreId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public JsonResult PackageProductionRateValidate(Models.Bid.Package.ProductionRateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Package Setup Default CostItems
        [HttpGet]
        public ActionResult PackageCostItemListPanel(byte bdco, int bidId, int packageId, string budgetCategory)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.CostItemListViewModel(package, budgetCategory);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/DefaultSetup/CostItem/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageCostItemListTable(byte bdco, int bidId, int packageId, string budgetCategory)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.CostItemListViewModel(package, budgetCategory);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/DefaultSetup/CostItem/_Table", model);
        }

        [HttpGet]
        public ActionResult PackageCostItemListApplyDefault(byte bdco, int bidId, int packageId, string budgetCategory)
        {
            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
            package.ImportDefaultCostItems(budgetCategory);
            db.BulkSaveChanges();
            
            return RedirectToAction("PackageCostItemListTable", new { bdco, bidId, packageId, budgetCategory });
        }

        [HttpGet]
        public PartialViewResult PackageCostItemAdd(byte bdco, int bidId, int packageId, string budgetCategory)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var newObj = package.AddPackageCostItem();
            db.BulkSaveChanges();
            var model = new Models.Bid.Package.CostItemViewModel(newObj, budgetCategory);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/CostItem/_TableRow", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageCostItemUpdate(Models.Bid.Package.CostItemViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageCostItemDelete(byte bdco, int bidId, int packageId, int lineId)
        {
            using var db = new VPContext();
            var delObj = db.BidPackageCostItems.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId && f.LineId == lineId);
            if (delObj != null)
            {
                foreach (var bore in delObj.Package.BoreLines)
                {
                    var costItems = bore.CostItems.Where(f => f.BudgetCodeId == delObj.tBudgetCodeId).ToList();
                    costItems.ForEach(e => { bore.CostItems.Remove(e); });
                }
                db.BidPackageCostItems.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageCostItemValidate(Models.Bid.Package.CostItemViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Package Allocated Cost List Bore Setup
        [HttpGet]
        public ActionResult PackageAllocatedCostListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.AllocatedCostListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/AllocatedCost/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageAllocatedCostListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.AllocatedCostListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/DefaultSetup/AllocatedCost/_Table", model);
        }

        [HttpGet]
        public PartialViewResult PackageAllocatedCostListAdd(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var newObj = package.AddPackageCost();
            db.SaveChanges(ModelState);

            var result = new Models.Bid.Package.AllocatedCostViewModel(newObj);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            ViewBag.tableRow = "True";
            return PartialView("Forms/Package/DefaultSetup/AllocatedCost/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageAllocatedCostListUpdate(Models.Bid.Package.AllocatedCostViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), model = model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageAllocatedCostListDelete(byte bdco, int bidId, int packageId, int lineId)
        {
            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
            var delObj = package.PackageCosts.FirstOrDefault(f => f.LineId == lineId);

            if (delObj != null)
            {
                foreach (var bore in package.ActiveBoreLines)
                {
                    var costItems = bore.CostItems.Where(f => f.IsPackageCost == true && f.BudgetCodeId == delObj.BudgetCodeId).ToList();
                    costItems.ForEach(e => bore.CostItems.Remove(e));
                }
                db.BidPackageCosts.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageAllocatedCostListValidate(Models.Bid.Package.AllocatedCostViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Package Cost Summary
        [HttpGet]
        public ActionResult PackageCostSummaryListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);

            bid.Recalculate();
            db.BulkSaveChanges();

            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/CostSummary/_PanelList", model);
        }

        [HttpGet]
        public ActionResult PackageCostSummaryPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            db.Database.CommandTimeout = 120;
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.PackageCostListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/CostSummary/_Panel", model);
        }


        [HttpGet]
        public ActionResult PackageCostSummaryTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.PackageCostListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/CostSummary/Cost/_Table", model);
        }

        [HttpGet]
        public ActionResult PackageBoreSummaryTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.PackageCostListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess; 

            return PartialView("Forms/Package/CostSummary/BoreCost/_Table", model);
        }

        #endregion

        #region Package Pricing
        [HttpGet]
        public ActionResult PackagePricingListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);            
            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Price/_PanelList", model);
        }

        [HttpGet]
        public ActionResult PackagePricingPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            db.Database.CommandTimeout = 300;
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.PriceFormViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Price/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackagePriceForm(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Package.PriceViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Price/Form/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackagePriceUpdate(Models.Bid.Package.PriceViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackagePriceValidate(Models.Bid.Package.PriceViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Package Job Assignment
        #region Package List
        [HttpGet]
        public ActionResult PackageJobSetupListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.JobListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/JobSetup/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageJobSetupListTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.JobListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/JobSetup/List/_Table", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageJobSetupUpdate(Models.Bid.Package.JobViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageJobSetupValidate(Models.Bid.Package.JobViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            model.Validate(ModelState);

            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
            var boreList = new Models.Bid.Bore.BoreLineListViewModel(package);
            TryValidateModelRecursive(boreList);
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError("Description", "Review Bore Lines for Errors");
            }
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Package Job List Bore Setup
        [HttpGet]
        public ActionResult PackageJobSetupBoreListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.JobListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/JobSetup/BoreList/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageJobSetupBoreListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.JobListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/JobSetup/BoreList/_Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageJobSetupBoreUpdate(Models.Bid.Bore.JobViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageJobSetupBoreValidate(Models.Bid.Bore.JobViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            model.Validate(ModelState);
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Package Award
        #region Package Award List
        [HttpGet]
        public ActionResult PackageAwardListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.AwardListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Award/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageAwardListTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.AwardListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Award/List/_Table", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageAwardUpdate(Models.Bid.Package.AwardViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageAwardValidate(Models.Bid.Package.AwardViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            model.Validate(ModelState);

            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
            var boreList = new Models.Bid.Bore.BoreLineListViewModel(package);
            TryValidateModelRecursive(boreList);
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError("Description", "Review Bore Lines for Errors");
            }
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Package Award Bore Setup
        [HttpGet]
        public ActionResult PackageAwardBoreListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.AwardListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Award/BoreList/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageAwardBoreListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.AwardListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Award/BoreList/_Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageAwardBoreUpdate(Models.Bid.Bore.AwardViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageAwardBoreValidate(Models.Bid.Bore.AwardViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            model.Validate(ModelState);
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Package Bore Details
        #region Package List
        [HttpGet]
        public ActionResult BoreSetupPackageListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/_PanelList", model);
        }

        [HttpGet]
        public ActionResult BoreSetupListPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            package.Recalculate();

            db.BulkSaveChanges();
            var model = new Models.Bid.Bore.DetailListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/BoreSetup/_Panel", model);
        }
        #endregion
        
        #region Bore List
        [HttpGet]
        public ActionResult BoreSetupListTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.DetailListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/BoreList/_Table", model);
        }

        [HttpGet]
        public ActionResult BoreSetupPanel(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var model = new Models.Bid.Bore.SetupFormView(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/Form/_Panel", model);
        }


        [HttpGet]
        public ActionResult BoreSetupValidate(Models.Bid.Bore.DetailViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == model.BoreId);
            var setupFormModel = new Models.Bid.Bore.SetupFormView(bore);
            setupFormModel.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreSetupUpdate(Models.Bid.Bore.DetailViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Bore Setup Production Default Rates
        [HttpGet]
        public ActionResult BoreProductionRateListPanel(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var model = new Models.Bid.Bore.ProductionRateListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/Form/ProductionRate/_Panel", model);
        }

        [HttpGet]
        public ActionResult BoreProductionRateListTable(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var model = new Models.Bid.Bore.ProductionRateListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/Form/ProductionRate/_Table", model);
        }

        [HttpGet]
        public ActionResult BoreProductionRateListApplyDefault(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            bore.ApplyPackageProductionDefaults();
            bore.Recalculate();
            db.SaveChanges(ModelState);

            var model = new Models.Bid.Bore.ProductionRateListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/Form/ProductionRate/_Table", model);
        }

        [HttpGet]
        public ActionResult BoreProductionRateAdd(byte bdco, int bidId, int boreId, string phaseId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var phase = db.PhaseMasters.FirstOrDefault(f => f.PhaseGroupId == bore.Bid.Company.PhaseGroupId && f.PhaseId == phaseId);
            if (phase != null)
            {
                bore.AddProductionPasses(phase);
                db.SaveChanges(ModelState);
            }

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return RedirectToAction("BoreProductionRateListTable", new { Area = "", bdco, bidId, boreId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreProductionRateUpdate(Models.Bid.Bore.ProductionRateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreProductionRateDelete(byte bdco, int bidId, int boreId, string phaseId, int passId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var pass = bore.Passes.Where(f => f.PhaseId == phaseId && f.PassId == passId).ToList();
            bore.RecalcNeeded = true;
            foreach (var item in pass)
            {
                item.Deleted = true;
            }
            bore.RecalculateCostUnits();
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult BoreProductionRateValidate(Models.Bid.Bore.ProductionRateViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Bore Setup Cost Items

        [HttpGet]
        public ActionResult BoreCostItemListPanel(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var model = new Models.Bid.Bore.CostItemListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/BoreSetup/CostItem/_Panel", model);
        }

        [HttpGet]
        public ActionResult BoreCostItemListTable(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            var model = new Models.Bid.Bore.CostItemListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Package/BoreSetup/Form/CostItem/Tabs", model);
        }

        [HttpGet]
        public ActionResult BoreCostRecalculateAll(byte bdco, int bidId, int boreId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == boreId);
            bore.RecalcNeeded = true;
            bore.Recalculate();
            db.BulkSaveChanges();
            var model = new Models.Bid.Bore.CostItemListViewModel(bore);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/BoreSetup/Form/CostItem/Tabs", model);
        }

        [HttpGet]
        public PartialViewResult BoreCostItemAdd(byte bdco, int bidId, int boreId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreCostItemUpdate(Models.Bid.Bore.CostItemViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreCostItemDelete(byte bdco, int bidId, int boreId, int lineId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public JsonResult BoreCostItemValidate(Models.Bid.Bore.CostItemViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Package Schedule
        #region Package List

        [HttpGet]
        public ActionResult PackageSchedulePanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.ScheduleListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Schedule/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageListSchedulePanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.ScheduleListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Schedule/List/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageScheduleTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Package.ScheduleListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Schedule/List/_Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageScheduleUpdate(Models.Bid.Package.ScheduleViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult PackageScheduleValidate(Models.Bid.Package.ScheduleViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            using var db = new VPContext();
            var obj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
            var setupFormModel = new Models.Bid.Package.ScheduleViewModel(obj);
            setupFormModel.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Bore List
        [HttpGet]
        public ActionResult BoreSchedulePanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.ScheduleListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Schedule/BoreList/_Panel", model);
        }

        [HttpGet]
        public ActionResult BoreScheduleTable(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Bore.ScheduleListViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Package/Schedule/BoreList/_Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BoreScheduleUpdate(Models.Bid.Bore.ScheduleViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult BoreScheduleValidate(Models.Bid.Bore.ScheduleViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId);
            var bore = bid.BoreLines.FirstOrDefault(f => f.BoreId == model.BoreId);
            var setupFormModel = new Models.Bid.Bore.ScheduleViewModel(bore);
            setupFormModel.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Proposal
        [HttpGet]
        public ActionResult BidProposalPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.FormViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/_Panel", model);
        }

        [HttpGet]
        public ActionResult BidProposalForm(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.FormViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/_Form", model);
        }
        #endregion

        #region Proposal Info
        [HttpGet]
        public ActionResult BidProposalInfoForm(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.BidViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/Bid/Header/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BidProposalInfoUpdate(Models.Bid.Proposal.BidViewModel model)
        {
            var updModel = (Models.Bid.BidInfoViewModel)model;

            if (updModel == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            updModel.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model = updModel, errorModel = ModelState.ModelErrors() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BidProposalInfoValidate(Models.Bid.Proposal.BidViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        //portal.Models.Views.Bid.Forms.Proposal.Bid.BidViewModel
        #endregion

        #region Proposal Package List
        [HttpGet]
        public ActionResult PackageProposalListPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.PackageListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/Package/_PanelList", model);
        }

        [HttpGet]
        public ActionResult PackageProposalPanel(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Proposal.PackageProposalViewModel(package);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/Package/_Panel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageProposalUpdate(Models.Bid.Proposal.PackageProposalViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageProposalValidate(Models.Bid.Proposal.PackageProposalViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Package Scope
        [HttpGet]
        public ActionResult PackageProposalScopeListPanel(byte bdco, int bidId, int packageId, DB.ScopeTypeEnum ScopeTypeId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Proposal.PackageScopeListViewModel(package, ScopeTypeId);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Proposal/Package/ScopeList/_Panel", model);
        }

        [HttpGet]
        public ActionResult PackageProposalScopeListTable(byte bdco, int bidId, int packageId, DB.ScopeTypeEnum ScopeTypeId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var package = bid.Packages.FirstOrDefault(f => f.PackageId == packageId);
            var model = new Models.Bid.Proposal.PackageScopeListViewModel(package, ScopeTypeId);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;


            return PartialView("Forms/Proposal/Package/ScopeList/_Table", model);
        }

        [HttpGet]
        public PartialViewResult PackageProposalScopeListAdd(byte bdco, int bidId, int packageId, DB.ScopeTypeEnum scopeTypeId)
        {
            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
            var newObj = package.AddScope(scopeTypeId);

            db.SaveChanges(ModelState);
            var model = new Models.Bid.Proposal.PackageScopeViewModel(newObj);
            ViewBag.TableRow = "True";

            return PartialView("Forms/Proposal/Package/ScopeList/_TableRow", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageProposalScopeUpdate(Models.Bid.Proposal.PackageScopeViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackageProposalScopeListDelete(byte bdco, int bidId, int packageId, DB.ScopeTypeEnum scopeTypeId, int lineId)
        {
            using var db = new VPContext();
            var delObj = db.BidPackageScopes.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId && f.ScopeTypeId == (int)scopeTypeId && f.LineId == lineId);
            if (delObj != null)
            {
                db.BidPackageScopes.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PackageProposalScopeValidate(Models.Bid.Proposal.PackageScopeViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Proposal Bid Scope
        [HttpGet]
        public ActionResult BidProposalScopeListPanel(byte bdco, int bidId, DB.ScopeTypeEnum scopeTypeId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.BidScopeListViewModel(bid, scopeTypeId);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/Bid/ScopeList/_Panel", model);
        }

        [HttpGet]
        public ActionResult BidProposalScopeListTable(byte bdco, int bidId, DB.ScopeTypeEnum scopeTypeId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.Proposal.BidScopeListViewModel(bid, scopeTypeId);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;
            ViewBag.IsTitleBlank = !model.List.Any(f => !string.IsNullOrEmpty(f.Title));

            return PartialView("Forms/Proposal/Bid/ScopeList/_Table", model);
        }

        [HttpGet]
        public PartialViewResult BidProposalScopeListAdd(byte bdco, int bidId, DB.ScopeTypeEnum scopeTypeId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var newObj = bid.AddScope(scopeTypeId);
            db.SaveChanges(ModelState);
            var model = new Models.Bid.Proposal.BidScopeViewModel(newObj);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;
            ViewBag.IsTitleBlank = !bid.Scopes.Any(f => f.ScopeTypeId == (int)scopeTypeId && !string.IsNullOrEmpty(f.Title));
            ViewBag.TableRow = "True";
            return PartialView("Forms/Proposal/Bid/ScopeList/_TableRow", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BidProposalScopeUpdate(Models.Bid.Proposal.BidScopeViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BidProposalScopeListDelete(byte bdco, int bidId, DB.ScopeTypeEnum scopeTypeId, int lineId)
        {
            using var db = new VPContext();
            var delObj = db.BidProposalScopes.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.ScopeTypeId == (int)scopeTypeId && f.LineId == lineId);
            if (delObj != null)
            {
                db.BidProposalScopes.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult BidProposalScopeValidate(Models.Bid.Proposal.BidScopeViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }



		[HttpGet]
		public ActionResult BidProposalScopeDefaults(byte bdco, int bidId, DB.ScopeTypeEnum scopeTypeId)
		{
			using var db = new VPContext();
			var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            //bid.Scopes.Where(f => f.ScopeTypeId == (int)scopeTypeId).ToList().ForEach(f => db.BidProposalScopes.Remove(f));
            bid.ImportScopeTemplate();
			db.SaveChanges(ModelState);

			return RedirectToAction("BidProposalScopeListTable", new { bdco, bidId, scopeTypeId });
		}
		#endregion

		#region Propsal Customer List
		[HttpGet]
        public ActionResult ProposalCustomerTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var model = new Models.Bid.CustomerListViewModel(bid);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;

            return PartialView("Forms/Proposal/Customer/_Table", model);
        }

        [HttpGet]
        public PartialViewResult ProposalCustomerAdd(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);

            var newObj = bid.AddCustomer();
            db.SaveChanges(ModelState);

            var result = new Models.Bid.CustomerViewModel(newObj);

            var hasAccess = bid.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            ViewBag.ViewOnly = !hasAccess;
            ViewBag.tableRow = "True";

            return PartialView("Forms/Proposal/Customer/_TableRow", result);
        }
        #endregion

        #region Bid Proposal PDF
        [HttpGet]
        public ActionResult BidProposalPDF(byte bdco, int bidId, int customerId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bidCustomer = bid.Customers.FirstOrDefault(f => f.CustomerId == customerId);

            if (bidCustomer == null)
                bidCustomer = bid.Customers.FirstOrDefault();

            var result = new portal.Areas.Project.Models.Bid.Proposal.FormViewModel(bidCustomer);

            var html = Code.EmailHelper.RenderViewToString(ControllerContext, "Forms/Proposal/PDF/Index", result, false);
            ConverterProperties converterProperties = new ConverterProperties();
            MediaDeviceDescription mediaDeviceDescription = new MediaDeviceDescription(MediaType.PRINT);
            converterProperties.SetMediaDeviceDescription(mediaDeviceDescription);

            using var workStream = new MemoryStream();
            using var pdfWriter = new PdfWriter(workStream);

            HtmlConverter.ConvertToPdf(html, pdfWriter, converterProperties);

            var attachment = bid.Attachment;
            var root = attachment.GetRootFolder();

            var fileName = string.Format(AppCultureInfo.CInfo(), "{0} - {1}.pdf", bid.tDescription, bidCustomer.Customer.Name);
            var folder = root.AddFolder("Proposals");

            folder.AddFile(fileName, workStream.ToArray());
            db.BulkSaveChanges();

            return new FileStreamResult(new MemoryStream(workStream.ToArray()), contentType: "application/pdf");

        }

        [HttpGet]
        public ActionResult BidProposalPDFPreview(byte bdco, int bidId, int lineId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bidCustomer = bid.Customers.FirstOrDefault(f => f.LineId == lineId);

            if (bidCustomer == null)
            {
                bidCustomer = bid.Customers.FirstOrDefault();
            }

            var result = new portal.Areas.Project.Models.Bid.Proposal.FormViewModel(bidCustomer);

            var html = Code.EmailHelper.RenderViewToString(ControllerContext, "Forms/Proposal/PDF/Index", result, false);
            ConverterProperties converterProperties = new ConverterProperties();
            MediaDeviceDescription mediaDeviceDescription = new MediaDeviceDescription(MediaType.PRINT);
            converterProperties.SetMediaDeviceDescription(mediaDeviceDescription);

            using var workStream = new MemoryStream();
            using var pdfWriter = new PdfWriter(workStream);

            HtmlConverter.ConvertToPdf(html, pdfWriter, converterProperties);

            var fileName = string.Format(AppCultureInfo.CInfo(), "{0} - {1}.pdf", bid.tDescription, bidCustomer.Customer.Name);

            return new FileStreamResult(new MemoryStream(workStream.ToArray()), contentType: "application/pdf");
            //return View("BidProposalPDF", result);

        }

        [HttpGet]
        public ActionResult BidProposalPDFForm(byte bdco, int bidId, int lineId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bidCustomer = bid.Customers.FirstOrDefault(f => f.LineId == lineId);
            var result = new portal.Areas.Project.Models.Bid.Proposal.FormViewModel(bidCustomer);
            return PartialView("Forms/Proposal/PDF/_Form", result);
        }

        [HttpGet]
        public ActionResult BidProposalPDFPanel(byte bdco, int bidId, int lineId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var bidCustomer = bid.Customers.FirstOrDefault(f => f.LineId == lineId);
            var result = new portal.Areas.Project.Models.Bid.Proposal.FormViewModel(bidCustomer);
            return PartialView("Forms/Proposal/PDF/_Panel", result);
        }
        #endregion

        #region Bid Search
        [HttpGet]
        public PartialViewResult BidSearch()
        {
            var result = new Models.Bid.TrackerListViewModel();

            return PartialView("List/Search/_Panel", result);
        }

        [HttpGet]
        public PartialViewResult BidSearchTable()
        {
            var result = new Models.Bid.TrackerListViewModel();

            return PartialView("List/Search/_Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult BidSearchReturn(Models.Bid.TrackerViewModel model)
        {
            if (model == null)
            {
                model = new Models.Bid.TrackerViewModel();
            }
            return Json(new { success = "true", value = model.BidId, errorModel = ModelState.ModelErrors() });
        }
        [HttpGet]
        public ActionResult BidSearchData()
        {
            using var db = new VPContext();
            var results = new Models.Bid.TrackerListViewModel(db);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion
    }
}
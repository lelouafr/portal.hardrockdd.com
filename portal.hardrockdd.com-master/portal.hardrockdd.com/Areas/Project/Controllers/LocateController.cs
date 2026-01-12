using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Project.Controllers 
{
    [RouteArea("Project")]
    public class LocateController : portal.Controllers.BaseController
    {

        #region Bid Locate List
        [HttpGet]
        public ActionResult LocateBidPanel(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var results = new Models.Locate.LocateListViewModel(bid);

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateBidTable";
            ViewBag.DataAction = "LocateBidData";
            ViewBag.Title = "Locate Listing";
            return PartialView("List/Bid/_Panel", results);
        }

        [HttpGet]
        public ActionResult LocateBidTable(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var results = new Models.Locate.LocateListViewModel(bid);

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateBidTable";
            return PartialView("List/Bid/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateBidData(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var results = new Models.Locate.LocateListViewModel(bid);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Locate List
        [HttpGet]
        [Route("Locates")]
        public ActionResult LocateIndex()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateTable";
            ViewBag.DataAction = "LocateData";
            ViewBag.Title = "Locate Listing";
            return View("List/Index", results);
        }

        [HttpGet]
        public ActionResult LocateTable()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateTable";
            ViewBag.DataAction = "LocateData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateData()
        {
            using var db = new VPContext();
            var locates = db.vPMLocates
                .Where(f => f.LocateStatusId != (int)DB.PMLocateStatusEnum.Import)
                .ToList();
            var results = new Models.Locate.LocateListViewModel(locates);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        public PartialViewResult LocateAdd(int? requestId)
        {
            using var db = new VPContext();
            PMLocate locate = null;
            if (requestId == null)
                locate = db.AddLocate();
            else
            {
                var request = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
                locate = request.AddLocate();
            }

            db.BulkSaveChanges();
            db.Entry(locate).Reload();

            var result = new Models.Locate.LocateViewModel(locate);

            ViewBag.tableRow = "True";
            return PartialView("Request/Form/Locate/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocateUpdate(Models.Locate.LocateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocateDelete(int locateId)
        {
            using var db = new VPContext();
            var delObj = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            if (delObj != null)
            {
                db.PMLocates.Remove(delObj);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LocateValidate(Models.Locate.LocateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModelRecursive(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Open Locate List
        [HttpGet]
        [Route("Locates/Open")]
        public ActionResult LocateOpenIndex()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();


            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateOpenTable";
            ViewBag.DataAction = "LocateOpenData";
            ViewBag.Title = "Locate Open Listing";
            return View("List/Index", results);
        }

        [HttpGet]
        public ActionResult LocateOpenTable()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();


            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateOpenTable";
            ViewBag.DataAction = "LocateOpenData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateOpenData()
        {
            using var db = new VPContext();
            var locates = db.vPMLocates
                .Where(f => f.LocateStatusId == (int)DB.PMLocateStatusEnum.New ||
                             f.LocateStatusId == (int)DB.PMLocateStatusEnum.Active ||
                             f.LocateStatusId == (int)DB.PMLocateStatusEnum.Expired)
                .ToList();
            var results = new Models.Locate.LocateListViewModel(locates);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Open Locate List
        [HttpGet]
        [Route("Locates/Expiring")]
        public ActionResult LocateExpiredIndex()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();


            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateExpiredTable";
            ViewBag.DataAction = "LocateExpiredData";
            ViewBag.Title = "Locate Expiring Listing";
            return View("List/Index", results);
        }

        [HttpGet]
        public ActionResult LocateExpiredTable()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId != (int)DB.PMLocateStatusEnum.Import).ToList();
            var results = new Models.Locate.LocateListViewModel();


            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateExpiredTable";
            ViewBag.DataAction = "LocateExpiredData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateExpiredData()
        {
            using var db = new VPContext();
            var locates = db.vPMLocates
                .Where(f => (f.StatusId == (int)DB.PMLocateStatusEnum.New ||
                             f.StatusId == (int)DB.PMLocateStatusEnum.Active ||
                             f.StatusId == (int)DB.PMLocateStatusEnum.Expired) &&
                             f.ExpireDays <= 3)
                .ToList();
            var results = new Models.Locate.LocateListViewModel(locates);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Locate Imported List
        [HttpGet]
        [Route("Locate/Imports")]
        public ActionResult LocateImportIndex()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId == (int)DB.PMLocateStatusEnum.Import).ToList();
            //var results = new Models.Locate.LocateListViewModel(locates);
            var results = new Models.Locate.LocateListViewModel();
            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateImportTable";
            ViewBag.DataAction = "LocateImportData";
            ViewBag.Title = "Locate Import Listing";
            return View("List/Index", results);
        }

        [HttpGet]
        public ActionResult LocateImportTable()
        {
            //using var db = new VPContext();
            //var locates = db.PMLocates.Where(f => f.StatusId == (int)DB.PMLocateStatusEnum.Import).ToList();
            //var results = new Models.Locate.LocateListViewModel(locates);

            var results = new Models.Locate.LocateListViewModel();
            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateImportTable";
            ViewBag.DataAction = "LocateImportData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateImportData()
        {
            using var db = new VPContext();
            var locates = db.vPMLocates
                .Where(f => f.LocateStatusId == (int)DB.PMLocateStatusEnum.Import)
                .ToList();
            var results = new Models.Locate.LocateListViewModel(locates);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Locate Form
        [HttpGet]
        public ActionResult FormPopup(int locateId)
        {
            using var db = new VPContext();
            var entity = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var result = new Models.Locate.FormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Form/Index", result);
        }

        [HttpGet]
        public ActionResult LocatePanel(int locateId)
        {
            using var db = new VPContext();
            var entity = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var result = new Models.Locate.FormViewModel(entity);

            return PartialView("Form/_Panel", result);
        }

        [HttpGet]
        public ActionResult LocateForm(int locateId)
        {
            using var db = new VPContext();
            var entity = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var result = new Models.Locate.FormViewModel(entity);

            return PartialView("Form/_Form", result);
        }

        [HttpGet]
        public JsonResult LocateFormValidate(Models.Locate.FormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Locate Form Info
        [HttpGet]
        public ActionResult InfoForm(int locateId)
        {
            using var db = new VPContext();
            var entity = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var result = new Models.Locate.LocateViewModel(entity);

            return PartialView("Forms/Info/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoUpdate(Models.Locate.LocateViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult InfoValidate(Models.Locate.LocateViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Locate Sequences List
        [HttpGet]
        public ActionResult LocateSequenceTable(int locateId)
        {
            using var db = new VPContext();
            var locate = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var results = new Models.Locate.LocateSequenceListViewModel(locate);

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateSequenceTable";
            ViewBag.DataAction = "LocateSequenceData";
            return PartialView("Form/Sequence/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateSequenceData(int locateId)
        {
            using var db = new VPContext();
            var locate = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var results = new Models.Locate.LocateSequenceListViewModel(locate);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocateSequenceDelete(int locateId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.PMLocateSequences.FirstOrDefault(f => f.LocateId == locateId && f.SeqId == seqId);
            if (delObj != null)
            {
                db.PMLocateSequences.Remove(delObj);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LocateSequenceValidate(Models.Locate.LocateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModelRecursive(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Locate Assignments List
        [HttpGet]
        public ActionResult LocateAssignmentTable(int locateId)
        {
            using var db = new VPContext();
            var locate = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var results = new Models.Locate.LocateAssignmentListViewModel(locate);

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "LocateAssignmentTable";
            ViewBag.DataAction = "LocateAssignmentData";

            return PartialView("Form/Assignment/_Table", results);
        }

        [HttpGet]
        public ActionResult LocateAssignmentData(int locateId)
        {
            using var db = new VPContext();
            var locate = db.PMLocates.FirstOrDefault(f => f.LocateId == locateId);
            var results = new Models.Locate.LocateAssignmentListViewModel(locate);

            JsonResult result = Json(new { data = results.Tree.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocateAssignmentDelete(int locateId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.PMLocateAssignments.FirstOrDefault(f => f.LocateId == locateId && f.SeqId == seqId);
            if (delObj != null)
            {
                db.PMLocateAssignments.Remove(delObj);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LocateAssignmentValidate(Models.Locate.LocateAssignmentViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModelRecursive(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LocateAssignmentUpdate(Models.Locate.LocateAssignmentViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Request Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateUpdate(Models.Locate.CreateRequestViewModel model)
        {
            if (model != null)
            {
                ModelState.Clear();
                using var db = new VPContext();
                model = model.ProcessUpdate(db, ModelState);


                TryValidateModel(model);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult CreateValidate(Models.Locate.CreateRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create(byte? bdco, int? bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            if (bid != null)
            {
                var results = new Models.Locate.CreateRequestViewModel(bid);
                return PartialView("Create/_Index", results);
            }
            else
            {
                var results = new Models.Locate.CreateRequestViewModel(db);
                return PartialView("Create/_Index", results);
            }
        }

        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPContext();
            var results = new Models.Locate.CreateRequestViewModel(db);
            return PartialView("Create/_Form", results);
        }

        [HttpGet]
        public PartialViewResult CreateFormBid(byte bdco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId);
            var results = new Models.Locate.CreateRequestViewModel(bid);
            return PartialView("Create/_Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Locate.CreateRequestViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);

            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var request = db.AddLocateRequest(model.ToDBObject(db));
                request.Status = DB.PMRequestStatusEnum.Submitted;
                db.SaveChanges(ModelState);

                model = new Models.Locate.CreateRequestViewModel(request);
            }

            return Json(new { success = ModelState.IsValidJson(), model }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Request Open List
        [HttpGet]
        [Route("Request/Open")]
        public ActionResult RequestOpenIndex()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestOpenTable";
            ViewBag.DataAction = "RequestOpenData";
            ViewBag.Title = "Locate Request Listing";
            return View("Request/List/Index", results);
        }

        [HttpGet]
        public ActionResult RequestOpenTable()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestOpenTable";
            ViewBag.DataAction = "RequestOpenData";
            return PartialView("Request/List/_Table", results);
        }

        [HttpGet]
        public ActionResult RequestOpenData()
        {
            using var db = new VPContext();
            var requests = db.PMLocateRequests.Where(f => f.tStatusId == (int)DB.PMRequestStatusEnum.Submitted).ToList();
            var results = new Models.Locate.RequestListViewModel(requests);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Request List
        [HttpGet]
        [Route("Request")]
        public ActionResult RequestIndex()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestTable";
            ViewBag.DataAction = "RequestData";
            ViewBag.Title = "Locate Request Listing";
            return View("Request/List/Index", results);
        }

        [HttpGet]
        public ActionResult RequestTable()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestTable";
            ViewBag.DataAction = "RequestData";
            return PartialView("Request/List/_Table", results);
        }

        [HttpGet]
        public ActionResult RequestData()
        {
            using var db = new VPContext();
            var requests = db.PMLocateRequests.Where(f => f.tStatusId != (int)DB.PMRequestStatusEnum.Import).ToList();
            var results = new Models.Locate.RequestListViewModel(requests);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Request Import List
        [HttpGet]
        [Route("Request/Import")]
        public ActionResult RequestImportIndex()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestImportTable";
            ViewBag.DataAction = "RequestImportData";
            ViewBag.Title = "Locate Request Import(s)";
            return View("Request/List/Index", results);
        }

        [HttpGet]
        public ActionResult RequestImportTable()
        {
            using var db = new VPContext();

            var results = new Models.Locate.RequestListViewModel();

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestImportTable";
            ViewBag.DataAction = "RequestImportData";
            return PartialView("Request/List/_Table", results);
        }

        [HttpGet]
        public ActionResult RequestImportData()
        {
            using var db = new VPContext();
            var requests = db.PMLocateRequests.Where(f => f.tStatusId == (int)DB.PMRequestStatusEnum.Import).ToList();
            var results = new Models.Locate.RequestListViewModel(requests);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Request Form
        [HttpGet]
        [Route("Locate/Request/{requestId}")]
        public ActionResult RequestFormIndex(int requestId)
        {
            using var db = new VPContext();
            var entity = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var result = new Models.Locate.RequestFormViewModel(entity);

            return View("Request/Form/Index", result);
        }

        [HttpGet]
        public ActionResult RequestFormPopUp(int requestId)
        {
            using var db = new VPContext();
            var entity = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var result = new Models.Locate.RequestFormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Request/Form/Index", result);
        }

        [HttpGet]
        public ActionResult RequestPanel(int requestId)
        {

            using var db = new VPContext();
            var entity = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var result = new Models.Locate.RequestFormViewModel(entity);

            return PartialView("Request/Form/_Panel", result);
        }

        [HttpGet]
        public ActionResult RequestForm(int requestId)
        {
            using var db = new VPContext();
            var entity = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var result = new Models.Locate.RequestFormViewModel(entity);

            return PartialView("Request/Form/_Form", result);
        }

        [HttpGet]
        public JsonResult RequestFormValidate(Models.Locate.FormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Request Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRequestStatus(int requestId, int statusId)
        {
            using var db = new VPContext();
            var request = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);

            var model = new Models.Locate.RequestViewModel(request);
            if (request != null)
            {
                var status = (DB.PMRequestStatusEnum)statusId;

                request.Status = status;
                db.SaveChanges(ModelState);
            }
            model = new Models.Locate.RequestViewModel(request);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        #endregion

        #region Request Info Form
        [HttpGet]
        public ActionResult RequestInfoForm(int requestId)
        {
            using var db = new VPContext();
            var entity = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var result = new Models.Locate.RequestViewModel(entity);

            return PartialView("Forms/Info/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestInfoUpdate(Models.Locate.RequestViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RequestInfoValidate(Models.Locate.RequestViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Request Locate List       
        [HttpGet]
        public ActionResult RequestLocateTable(int requestId)
        {
            using var db = new VPContext();
            var request = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
            var results = new Models.Locate.LocateListViewModel(request);

            ViewBag.DataController = "Locate";
            ViewBag.TableAction = "RequestLocateTable";
            ViewBag.DataAction = "RequestLocateData";
            return PartialView("Request/Form/Locate/_Table", results);
        }

        [HttpGet]
        public PartialViewResult RequestLocateAdd(int? requestId)
        {
            using var db = new VPContext();
            PMLocate locate = null;
            if (requestId == null)
                locate = db.AddLocate();
            else
            {
                var request = db.PMLocateRequests.FirstOrDefault(f => f.RequestId == requestId);
                locate = request.AddLocate();
            }

            db.BulkSaveChanges();
            db.Entry(locate).Reload();

            var result = new Models.Locate.LocateViewModel(locate);

            ViewBag.tableRow = "True";
            return PartialView("Request/Form/Locate/_TableRow", result);
        }

        #endregion

    }
}
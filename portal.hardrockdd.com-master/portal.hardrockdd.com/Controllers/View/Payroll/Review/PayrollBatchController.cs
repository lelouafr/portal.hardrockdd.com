using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll;
using portal.Repository.VP.HQ;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.Bid
{
    public class PayrollBatchController : BaseController
    {
        [HttpGet]
        public ActionResult Panel()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            var model = new PayrollBatchListViewModel(result);
            return PartialView("../PR/Batch/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            var model = new PayrollBatchListViewModel(result);
            return PartialView("../PR/Batch/List/Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            using var repo = new BatchRepository();

            if (ModelState.IsValid)
            {
                var result = BatchRepository.Init();
                result.TableName = "PRTB";
                result.Source = "PR Entry";
                _ = repo.Create(result);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            using var db = new VPContext();
            using var repo = new BatchRepository();
            
            var model = BatchRepository.Init();
            model.TableName = "PRTB";
            model.Source = "PR Entry";

            model = repo.Create(model, ModelState);            

            var result = new PayrollBatchViewModel(model);

            ViewBag.tableRow = "True";
            return PartialView("../PR/Batch/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PayrollBatchViewModel model)
        {
            var repo = new BatchRepository();
            var result = repo.ProcessUpdate(model, ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //var jsonmodel = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            repo.Dispose();
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(PayrollBatchViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}
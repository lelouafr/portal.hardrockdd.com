//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Employee;
//using portal.Repository.VP.PR;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using System.Runtime.Caching;

//namespace portal.Controllers.VP.PR
//{
//    [Authorize]
//    public class EmployeeController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Form(byte co, int EmployeeId)
//        {
//            using var db = new VPContext();
//            var result = db.Employees.Where(f => f.PRCo == co && f.EmployeeId == EmployeeId).FirstOrDefault();
//            using var repo = new EmployeeRepository();
//            var model = new EmployeeViewModel(result);
//            return PartialView(model);
//        }

//        [HttpGet]
//        public PartialViewResult Add(byte co)
//        {
//            var result = new EmployeeViewModel
//            {
//                PRCo = co
//            };

//            return PartialView("Form", result);
//        }

//        [ValidateAntiForgeryToken]
//        [HttpPost]
//        public JsonResult Create(EmployeeViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            using var repo = new EmployeeRepository();
//            var result = repo.Create(model, ModelState);

//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), value = result.EmployeeId, errorModel = ModelState.ModelErrors() });
//        }

//        [ValidateAntiForgeryToken]
//        [HttpPost]
//        public ActionResult Update(EmployeeViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }

//            using var repo = new EmployeeRepository();
//            var result = repo.ProcessUpdate(model, ModelState);
//            //var result = repo.GetEmployee(model.Co, model.EmployeeId, "");

//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [ValidateAntiForgeryToken]
//        [HttpPost]
//        public ActionResult Delete(byte co, int EmployeeNumber)
//        {
//            using var repo = new EmployeeRepository();
//            var model = repo.GetEmployee(co, EmployeeNumber);
//            if (model != null)
//            {
//                repo.Delete(model);
//            }
//            return Json(new { success = "true", model });
//        }

//        [HttpGet]
//        public JsonResult Validate(EmployeeViewModel model)
//        {
//            ModelState.Clear();
//            TryValidateModel(model);

//            if (!ModelState.IsValid)
//            {
//                var errorModel = ModelState.ModelErrors();
//                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }

//    }
//}
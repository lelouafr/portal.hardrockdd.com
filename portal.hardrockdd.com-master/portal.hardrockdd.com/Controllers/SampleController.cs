using Newtonsoft.Json;
using portal.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    [Authorize]
    public class SampleController : Controller
    {

        [HttpGet]
        [Route("Sample")]
        public ActionResult Index()
        {
            var model = new SampleListViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var model = new SampleListViewModel();
            return PartialView(model);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            var results = new SampleViewModel(1, "New string", DateTime.Now);
            ViewBag.TableRow = true;

            return PartialView("TableRow", results);
        }

        [HttpGet]
        public ActionResult Form(int id)
        {
            var model = new SampleViewModel(id, string.Format(AppCultureInfo.CInfo(), "{0}, {1}", id, DateTime.Now.AddDays(id)), DateTime.Now.AddDays(id));
            return PartialView("Form/Form", model);
        }

        [HttpGet]
        public ActionResult Panel(int id)
        {
            var model = new SampleViewModel(id, string.Format(AppCultureInfo.CInfo(), "{0}, {1}", id, DateTime.Now.AddDays(id)), DateTime.Now.AddDays(id));
            return PartialView("Form/Panel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(SampleViewModel model)
        {
            //Update Database here with model
            var result = model;

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var model = new SampleViewModel(id, string.Format(AppCultureInfo.CInfo(), "{0}, {1}", id, DateTime.Now.AddDays(id)), DateTime.Now.AddDays(id));
            //Delete model from database using model data


            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public JsonResult Validate(SampleViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

    }
}
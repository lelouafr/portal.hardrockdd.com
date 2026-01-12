using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Forms;
using portal.Repository.VP.EM;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EquipmentOwnershipController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(EquipmentOwnershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(EquipmentOwnershipViewModel model)
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
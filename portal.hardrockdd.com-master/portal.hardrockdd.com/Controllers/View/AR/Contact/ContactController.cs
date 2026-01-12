using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Contact;
using portal.Repository.VP.AR;
using portal.Repository.VP.HQ;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AR
{
    [Authorize]
    public class ContactController : BaseController
    {
        [HttpGet]
        public ActionResult Form(byte custGroupId, int customerId, int? contactId)
        {
            using var db = new VPContext();
            if (contactId == null)
            {
                return RedirectToAction("Add", new { Area = "",custGroupId, customerId });
            }
            var customer = db.Customers.Where(f => f.CustGroupId == custGroupId && f.CustomerId == customerId).FirstOrDefault();
            var result = db.CustomerContacts.Where(f => f.CustGroupId == custGroupId && f.CustomerId == customerId && f.ContactId == contactId).FirstOrDefault().Contact;
            var model = new ContactViewModel(result, customer);
            return PartialView("../AR/Contact/Form", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte custGroupId, int customerId)
        {
            var result = new ContactViewModel
            {
                ContactGroupId = custGroupId,
                CustomerId = customerId
            };

            return PartialView("../AR/Contact/Form", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Create(ContactViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var repo = new ContactRepository();
            var result = repo.Create(model, ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), value = result.ContactId, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(ContactViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            if (model.ContactId == 0 )
            {
                return Json(new { success = ModelState.IsValidJson(), model });
            }
            using var repo = new ContactRepository();
            var result = repo.ProcessUpdate(model, ModelState);
            //var result = repo.GetCustomer(model.CustGroupId, model.CustomerId, "");

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte custGroupId, int contactId)
        {
            using var repo = new ContactRepository();
            var result = repo.GetContact(custGroupId, contactId);
            var model = new ContactViewModel(result);
            if (result != null)
            {
                repo.Delete(result);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpGet]
        public JsonResult Validate(ContactViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

    }
}
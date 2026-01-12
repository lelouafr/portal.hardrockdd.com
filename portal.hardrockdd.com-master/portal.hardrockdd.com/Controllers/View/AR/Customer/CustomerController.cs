using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Customer;
using portal.Models.Views.AR.Customer.Forms;
using portal.Repository.VP.AR;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AR.Customer.Customer
{
    [ControllerAuthorize]
    public class CustomerController : BaseController
    {

        [HttpGet]
        [Route("AR/Customer")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new CustomerListViewModel();
            results.List.Add(new CustomerViewModel());
            results.CustGroupId = (byte)company.CustGroupId;
            ViewBag.DataController = "Customer";
            ViewBag.DataAction = "Data";
            return View("../AR/Customer/Summary/List/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new CustomerListViewModel();
            results.List.Add(new CustomerViewModel());
            results.CustGroupId = (byte)company.CustGroupId;
            ViewBag.DataController = "Customer";
            ViewBag.DataAction = "Data";
            return PartialView("../AR/Customer/Summary/List/Table", results);
        }

        #region Customer Form

        [HttpGet]
        public ActionResult Panel(byte custGroupId, int customerId)
        {
            using var db = new VPContext();
            var obj = db.Customers
                .Include("ARTrans")
                .Include("ARTrans.Customer")
                .Include("ARTrans.Contract")
                .Include("ARTrans.Contract.Jobs")
                .FirstOrDefault(f => f.CustGroupId == custGroupId && f.CustomerId == customerId);

            var results = new CustomerFormViewModel(obj);
            return PartialView("../AR/Customer/Forms/Panel", results);
        }

        [HttpGet]
        public ActionResult Form(byte custGroupId, int customerId)
        {
            using var db = new VPContext();
            var obj = db.Customers
                .Include("ARTrans")
                .Include("ARTrans.Customer")
                .Include("ARTrans.Contract")
                .Include("ARTrans.Contract.Jobs")
                .FirstOrDefault(f => f.CustGroupId == custGroupId && f.CustomerId == customerId);
            var results = new CustomerFormViewModel(obj);
            return PartialView("../AR/Customer/Forms/Form", results);
        }

        [HttpGet]
        public ActionResult PopupForm(byte custGroupId, int customerId)
        {
            using var db = new VPContext();
            var obj = db.Customers.FirstOrDefault(f => f.CustGroupId == custGroupId && f.CustomerId == customerId);

            var results = new CustomerFormViewModel(obj);
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../AR/Customer/Forms/Index", results);
        }

        #endregion

        #region Form Add
        [HttpGet]
        public PartialViewResult FormAdd(byte custGroupId)
        {
            var result = new CustomerAddViewModel
            {
                CustGroupId = custGroupId,                
            };

            return PartialView("../AR/Customer/Add/Form", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Create(CustomerAddViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            var result = CustomerRepository.Init(model);
            result.CustomerId = db.Customers
                            .Where(f => f.CustGroupId == model.CustGroupId && f.CustomerId >= 90000)
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 89999 : f.CustomerId) + 1;
            db.Customers.Add(result);
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), model = result.CustomerId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateAdd(CustomerAddViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Info
        [HttpGet]
        public ActionResult InfoForm(byte custGroupId, int customerId)
        {
            using var db = new VPContext();
            var obj = db.Customers
                .Include("ARTrans")
                .Include("ARTrans.Customer")
                .Include("ARTrans.Contract")
                .Include("ARTrans.Contract.Jobs")
                .FirstOrDefault(f => f.CustGroupId == custGroupId && f.CustomerId == customerId);

            var results = new CustomerFormViewModel(obj);
            return PartialView("../AR/Customer/Forms/PartialIndex", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(CustomerInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = CustomerRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ValidateInfo(CustomerInfoViewModel model)
        {

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte custGroupId, int CustomerNumber)
        {
            using var repo = new CustomerRepository();
            var result = repo.GetCustomer(custGroupId, CustomerNumber);
            var model = new CustomerViewModel(result);
            if (result != null)
            {
                repo.Delete(result);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }


        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();

            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQGroups.FirstOrDefault(f => f.GroupId == comp.CustGroupId);
            var results = new CustomerListViewModel(company);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        public JsonResult xData()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQGroups
                .Include("ARCustomers")
                .Include("ARCustomers.ARTrans")
                .FirstOrDefault(f => f.GroupId == comp.CustGroupId);

            var customerList = company.ARCustomers.OrderBy(o => o.CustomerId).Select(s => new
            {

                id = string.Format(AppCultureInfo.CInfo(), "0.{0}", s.CustomerId),
                Name = string.Format(AppCultureInfo.CInfo(), "{0}", s.Name),
                Contact = string.Format(AppCultureInfo.CInfo(), "{0}", s.Contact),
                Active = string.Format(AppCultureInfo.CInfo(), "{0}", s.Status == "A" ? "Active" : "Disabled"),
                OpenARCount = string.Format(AppCultureInfo.CInfo(), "{0:N0}", s.ARTrans.Where(w => w.AmountDue > 0).Count()),
                OpenARBalance = string.Format(AppCultureInfo.CInfo(), "{0:C2}", s.ARTrans.Sum(sum => sum.AmountDue)),

                Co = s.CustGroupId,
                CustomerId = s.CustomerId,
                //CategoryGroup = s.CategoryGroup,
                FormType = "Customer",
            });

            var dataSet = new List<dynamic>();
            dataSet.AddRange(customerList);

            JsonResult result = Json(dataSet, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;

            return result;
        }

    }
}
//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.AR.Customer;
//using portal.Repository.VP.AR;
//using portal.Repository.VP.BD;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.AR
//{
//    [Authorize]
//    public class CustomerDNUController : BaseController
//    {
//        #region Customer Form
//        [HttpGet]
//        public ActionResult Form(byte co, int customerId)
//        {
//            using var db = new VPContext();
//            var result = db.Customers.Where(f => f.CustGroupId == co && f.CustomerId == customerId).FirstOrDefault();            
//            var model = new CustomerViewModel(result);
//            return PartialView("../AR/Customer/Form", model);
//        }
//        #endregion

//    }
//}
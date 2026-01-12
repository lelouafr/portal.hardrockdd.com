//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.JC;
//using portal.Repository.VP.PR;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.JV
//{
//    [Authorize]
//    public class EarnCodeController : BaseController
//    {
//        //[HttpGet]
//        //public JsonResult TOCombo(byte co, int employeeEarnCodeId, string selected)
//        //{
//        //    using var repo = new EarnCodeRepository();

//        //    var list = repo.GetEarnCodes(co)
//        //                   .Where(w => w.Method == (employeeEarnCodeId == 1 ? "H" : "A") &&
//        //                    w.JCCostType == 1 &&
//        //                    w.OTCalcs == "N" &&
//        //                    w.PortalIsHidden == "N"
//        //                   ).ToList();

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Type"
//        //        }
//        //    };
//        //    result.AddRange(EarnCodeRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public JsonResult Combo(byte co, int EmployeeId, string selected)
//        //{
//        //    using var repo = new EarnCodeRepository();
//        //    using var db = new VPContext();

//        //    var emp = db.Employees.FirstOrDefault(f => f.PRCo == co && f.EmployeeId == EmployeeId);
//        //    var list = repo.GetEarnCodes(co)
//        //                   .Where(w => (w.Method == (emp.EarnCodeId == 1 ? "H" : "A") &&
//        //                    w.JCCostType == 1) || w.EarnCodeId == 999
//        //                   ).ToList();

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Type"
//        //        }
//        //    };
//        //    result.AddRange(EarnCodeRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//    }
//}
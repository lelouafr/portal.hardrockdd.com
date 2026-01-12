//using portal.Repository.VP.JC;
//using portal.Repository.VP.PR;
//using System.Collections.Generic;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.JV
//{
//    [Authorize]
//    public class CrewController : BaseController
//    {
//        //[HttpGet]
//        //public JsonResult AllCrewCombo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co);

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public JsonResult Combo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co);

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public JsonResult ShopCombo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co, "SHOP");

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public JsonResult TruckCombo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co, "TRUCK");

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}


//        //[HttpGet]
//        //public JsonResult CrewCrewCombo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co, "CREW");
//        //    list.AddRange(repo.GetCrews(co, "TRUCK"));

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public JsonResult JobCrewCombo(byte co, string selected)
//        //{
//        //    using var repo = new CrewRepository();

//        //    var list = repo.GetCrews(co, "CREW");

//        //    var result = new List<SelectListItem>
//        //    {
//        //        new SelectListItem
//        //        {
//        //            Text = "Select Crew"
//        //        }
//        //    };
//        //    result.AddRange(CrewRepository.GetSelectList(list, selected));

//        //    return Json(result, JsonRequestBehavior.AllowGet);
//        //}
//    }
//}
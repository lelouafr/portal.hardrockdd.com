//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job;
//using portal.Repository.VP.JC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.JV
//{
//    [Authorize]
//    public class JCComboController : BaseController
//    {
//        [HttpGet]
//        public JsonResult IndustryCombo(byte co)
//        {
//            using var db = new VPContext();

//            var list = db.JCIndustries
//                        .Where(f => f.Co == co)
//                        .OrderBy(f => f.Description)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Industry",
//                }
//            };

//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.IndustryId.ToString(AppCultureInfo.CInfo()),
//                Text = s.Description,
//                Selected = false,
//                Group = new SelectListGroup
//                {
//                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.ActiveYN != "Y"
//                },

//            }).ToList());

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult IndustryMarketCombo(byte co, int industryId)
//        {
//            using var db = new VPContext();

//            var list = db.JCIndustryMarkets
//                        .Where(f => f.Co == co && f.IndustryId == industryId && f.MarketId != null)
//                        .OrderBy(f => f.Market.Description)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Industry",
//                }
//            };

//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.MarketId?.ToString(AppCultureInfo.CInfo()),
//                Text = s.Market.Description,
//                Selected = false,
//                Group = new SelectListGroup
//                {
//                    Name = s.Market.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.Market.ActiveYN != "Y"
//                },

//            }).ToList());

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult JCMarketCombo(byte co)
//        {
//            using var db = new VPContext();

//            var list = db.JCMarkets
//                        .Where(f => f.Co == co)
//                        .OrderBy(f => f.Description)
//                        .ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Market",
//                }
//            };

//            result.AddRange(list.Select(s => new SelectListItem
//            {
//                Value = s.MarketId.ToString(AppCultureInfo.CInfo()),
//                Text = s.Description,
//                Selected = false,
//                Group = new SelectListGroup
//                {
//                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
//                    Disabled = s.ActiveYN != "Y"
//                },

//            }).ToList());

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//    }
//}
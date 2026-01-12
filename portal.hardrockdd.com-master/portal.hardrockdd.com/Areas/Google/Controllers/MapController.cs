using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Google.Controllers 
{
    [RouteArea("Google")]
    public class MapController : portal.Controllers.BaseController
    {
        #region Google Map Form
        
        [HttpGet]
        public ActionResult Panel(int mapSetId)
        {
            using var db = new VPContext();
            var mapSet = db.WPMapSets.FirstOrDefault(f => f.MapSetId == mapSetId);
            var results = new Models.Map.MapViewModel(mapSet);   
            return PartialView("_Panel", results);
        }

        [HttpGet]
        public ActionResult Form(int mapSetId)
        {
            using var db = new VPContext();
            var mapSet = db.WPMapSets.FirstOrDefault(f => f.MapSetId == mapSetId);
            var results = new Models.Map.MapViewModel(mapSet);
            return PartialView("_Form", results);
        }

        #endregion

    }
}
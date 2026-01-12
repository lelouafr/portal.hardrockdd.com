using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Forums;
using portal.Repository.VP.HQ;
using portal.Repository.VP.WP;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.VP.WP
{
    [Authorize]
    public class ForumController : BaseController
    {
        [HttpGet]
        public ActionResult Panel(byte co, int forumId)
        {
            using var db = new VPContext();
            var result = db.Forums.FirstOrDefault(f => f.Co == co && f.ForumId == forumId);
            var model = new ForumLineListViewModel(result);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Table(byte co, int forumId)
        {
            using var db = new VPContext();
            var result = db.Forums.FirstOrDefault(f => f.Co == co && f.ForumId == forumId);
            var model = new ForumLineListViewModel(result);

            return PartialView(model);
        }


        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Add(ForumLineViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
            var db = new VPContext();
            var forum = db.Forums.FirstOrDefault(f => f.Co == model.Co && f.ForumId == model.ForumId);
            
            var forumLine = forum.AddLine();
            forumLine.HtmlComment = model.HtmlComment;
            forumLine.Comment = StaticFunctions.GetPlainTextFromHtml(model.HtmlComment);
            forum.Lines.Add(forumLine);
            db.SaveChanges(ModelState);
            db.Dispose();

            db = new VPContext();
            forumLine = db.ForumLines.FirstOrDefault(f => f.Co == forumLine.Co && f.ForumId == forumLine.ForumId && f.LineId == forumLine.LineId);
            var result = new ForumLineViewModel(forumLine);
            db.Dispose();

            return PartialView("TableRow", result);
        }


        ///PDF/BidProposalPDF?co=1&bidId=242&customerID=327

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            return Json(new { success = ModelState.IsValidJson() });
        }

        //[HttpGet]
        //public JsonResult Validate(PassViewModel model)
        //{
        //    ModelState.Clear();
        //    TryValidateModel(model);

        //    if (!ModelState.IsValid)
        //    {
        //        var errorModel = ModelState.ModelErrors();
        //        return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        //}
    }
}
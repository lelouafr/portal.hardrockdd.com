using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Attachment;
using portal.Models.Views.Web;
using portal.Repository.VP.HQ;
using portal.Repository.VP.HR;
using portal.Repository.VP.PM;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.VP.HQ
{
    [Authorize]
    public class AuditLogController : BaseController
    {
        [HttpGet]
        public ActionResult Panel(string entityName, string entityKeyString)
        {
            using var db = new VPContext();
            var model = new AuditLogListViewModel(db,entityName, entityKeyString);

            return PartialView("../Administration/AuditLog/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(string entityName, string entityKeyString)
        {
            using var db = new VPContext();
            var model = new AuditLogListViewModel(db, entityName, entityKeyString);


            return PartialView("../Administration/AuditLog/List/Table", model);
        }

    }
}
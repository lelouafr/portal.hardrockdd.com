using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard;
using portal.Repository.VP.AP.CreditCard;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardImportController : BaseController
    {
        [HttpGet]
        [Route("AP/Credit/Imports")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new CreditImportListViewModel(company);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APCreditCardImport";
            return View("../AP/CreditCard/Upload/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new CreditImportListViewModel(company);

            ViewBag.Partial = true;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APCreditCardImport";
            return PartialView("../AP/CreditCard/Upload/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var model = new CreditImportListViewModel(company);

            ViewBag.Partial = true;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APCreditCardImport";
            return PartialView("../AP/CreditCard/Upload/List/Table", model);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new CreditImportListViewModel(company);
            
            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }


        [HttpPost]
        public ActionResult Add()
        {
            using var db = new VPContext();

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase fileUpload = Request.Files[fileName];
                if (fileUpload.ContentType.Contains("image") || fileUpload.ContentType.Contains("pdf"))
                {
                    using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
                    var attach = db.CreditTransactionAttachments.FirstOrDefault(f => f.ReceiptImageRefId == fileUpload.FileName);
                    if (attach != null)
                    {
                        var trans = attach.Transaction;

                        CreditTransactionRepository.ProcessCreditCardReceipt(fileUpload, trans, db, dbAttch);

                        db.SaveChanges(ModelState);
                        dbAttch.SaveChanges(ModelState);

                    }
                }
                else
                {
                    var import = CreditImport.CreateImport(fileUpload, db);
                    db.SaveChanges(ModelState);

                    if (ModelState.IsValid)
                    {
                        var task = new Task(() =>
                        {
                            var threadDb = new VPContext();
                            var importThread = threadDb
                            .CreditImports
                            .Include("WexLines")
                            .Include("WexLines.EMEquipment")
                            .Include("WexLines.PREmployee")
                            .Include("ZionLines")
                            .Include("ZionLines.PREmployee")
                            .FirstOrDefault(f => f.Co == import.Co && f.ImportId == import.ImportId);

                            importThread.ProcessTransactions();
                            threadDb.SaveChanges(ModelState);

                            //threadDb.SaveChanges();
                            threadDb.Dispose();
                        });
                        task.Start();
                    }
                }
            }

            return Json(new { Message = "Uploaded" });
        }

        #region Import Form

        

        [HttpGet]
        public ActionResult ImportPanel(byte co, int importId)
        {
            using var db = new VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var model = new portal.Models.Views.AP.CreditCard.Import.ImportFormViewModel(import);

            return PartialView("../AP/CreditCard/Upload/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult ImportForm(byte co, int importId)
        {
            using var db = new VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var model = new portal.Models.Views.AP.CreditCard.Import.ImportFormViewModel(import);

            return PartialView("../AP/CreditCard/Upload/Form/Form", model);
        }

        #region Form Actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reprocess(byte co, int importId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            if (import != null)
            {
                switch (import.Source)
                {
                    case "Wex":
                        import.ProcessWexLines();
                        break;
                    case "Zion":
                        import.ProcessZionLines();
                        break;
                    case "TXTag":
                        import.ProcessTXTagLines();
                        break;
                    default:
                        break;
                }
                db.BulkSaveChanges();
                import.ProcessTransactions();

				db.BulkSaveChanges();
			}

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Info
        [HttpGet]
        public ActionResult ImportInfoForm(byte co, int importId)
        {
            using var db = new VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var model = new portal.Models.Views.AP.CreditCard.Import.ImportFormViewModel(import);

            return PartialView("../AP/CreditCard/Upload/Form/Info/Form", model);
        }


        #endregion

        //ZionTable

        #region Zion
        [HttpGet]
        public ActionResult ZionTable(byte co, int importId)
        {
            using var db = new VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var model = new portal.Models.Views.AP.CreditCard.Import.ZionImportLinesViewModel(import);

            return PartialView("../AP/CreditCard/Upload/Form/Zion/Table", model);
        }
        #endregion

        #region Wex
        [HttpGet]
        public ActionResult WexTable(byte co, int importId)
        {
            //using var db = new VPContext();
            //var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var model = new portal.Models.Views.AP.CreditCard.Import.WexImportLinesViewModel()
            {
                ImportId = importId,
                Co = co,
            };

            return PartialView("../AP/CreditCard/Upload/Form/Wex/Table", model);
        }

        [HttpGet]
        public ActionResult WexData(byte co, int importId)
        {
            using var db = new VPContext();
            var import = db.CreditImports.FirstOrDefault(f => f.Co == co && f.ImportId == importId);
            var results = new portal.Models.Views.AP.CreditCard.Import.WexImportLinesViewModel(import);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion
        #endregion
    }
}
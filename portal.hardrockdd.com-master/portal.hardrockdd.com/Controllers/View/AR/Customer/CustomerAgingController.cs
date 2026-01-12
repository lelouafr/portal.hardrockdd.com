using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AR.Customer;
using portal.Models.Views.AR.Customer.Aging;
using portal.Models.Views.AR.Customer.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AR.Customer.Customer
{
    [ControllerAuthorize]
    public class CustomerAgingController : BaseController
    {

        [HttpGet]
        [Route("AR/Customer/Aging")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userDiv = db.GetCurrentEmployee().Division;
            if (userDiv != null)
                db.GetCurrentEmployee().DivisionId = 1;
            var results = new CustomerAgaingListViewModel();
            results.List.Add(new CustomerAgaingViewModel());
            results.DivisionId = userDiv.DivisionId;
            ViewBag.DataController = "CustomerAging";
            ViewBag.DataAction = "Data";
            return View("../AR/Customer/Summary/Aging/Index", results);
        }

        [HttpGet]
        public ActionResult Table(int divisionId)
        {
            using var db = new VPContext();
            var results = new CustomerAgaingListViewModel();
            results.List.Add(new CustomerAgaingViewModel());
            results.DivisionId = divisionId;
            ViewBag.DataController = "CustomerAging";
            ViewBag.DataAction = "Data";
            return PartialView("../AR/Customer/Summary/Aging/Table", results);
        }

        [HttpGet]
        public ActionResult Data(int divisionId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var data = new List<dynamic>();
            var results = new CustomerAgaingListViewModel();
            if (divisionId == 1)
            {
                results = new CustomerAgaingListViewModel(db);
            }
            else
            {
                var division = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == divisionId);
                results = new CustomerAgaingListViewModel(division);
            }

            var total = results.List.GroupBy(f => 1)
                    .Select(s => new
                    {
                        name = "Total",
                        DaysOutstanding = s.Max(max => max.CompanyDaysOutstanding),
                        Bucket1 = s.Sum(sum => sum.Bucket1),
                        Bucket2 = s.Sum(sum => sum.Bucket2),
                        Bucket3 = s.Sum(sum => sum.Bucket3),
                        Bucket4 = s.Sum(sum => sum.Bucket4),
                        Bucket5 = s.Sum(sum => sum.Bucket5),
                        Total = s.Sum(sum => sum.Total),
                        Level = 0,
                        //children = new List<dynamic>()
                    }).ToList();

            var customer = results.List.GroupBy(f => new { f.CustGroupId, f.CustomerId, f.CustomerName })
                    .Select(cust => new
                    {
                        name = cust.Key.CustomerName,
                        DaysOutstanding = cust.Max(max => max.CustomerAvgDays),
                        LastPayDate = cust.Max(max => max.LastPayDate),
                        Bucket1 = cust.Sum(sum => sum.Bucket1),
                        Bucket2 = cust.Sum(sum => sum.Bucket2),
                        Bucket3 = cust.Sum(sum => sum.Bucket3),
                        Bucket4 = cust.Sum(sum => sum.Bucket4),
                        Bucket5 = cust.Sum(sum => sum.Bucket5),
                        Total = cust.Sum(sum => sum.Total),
                        Level = 0,
                        children = cust.GroupBy(f => new { f.Job, f.JobDescription })
                            .Select(job => new
                            {
                                name = job.Key.JobDescription,
                                InvoiceDate = job.Min(min => min.InvoiceDate),
                                DaysOutstanding = "",
                                Bucket1 = job.Sum(sum => sum.Bucket1),
                                Bucket2 = job.Sum(sum => sum.Bucket2),
                                Bucket3 = job.Sum(sum => sum.Bucket3),
                                Bucket4 = job.Sum(sum => sum.Bucket4),
                                Bucket5 = job.Sum(sum => sum.Bucket5),
                                Total = job.Sum(sum => sum.Total),
                                Level = 1,
                                children = job
                                    .Select(inv => new
                                    {
                                        name = inv.Invoice,
                                        InvoiceDate = inv.InvoiceDate,
                                        DaysOutstanding = "",
                                        Bucket1 = inv.Bucket1,
                                        Bucket2 = inv.Bucket2,
                                        Bucket3 = inv.Bucket3,
                                        Bucket4 = inv.Bucket4,
                                        Bucket5 = inv.Bucket5,
                                        Total = inv.Total,
                                        Level = 2,
                                    }).ToList()
                            }).ToList()
                        }).ToList();

            data.AddRange(total);
            data.AddRange(customer);
            JsonResult result = Json(new { data = data.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }


    }
}
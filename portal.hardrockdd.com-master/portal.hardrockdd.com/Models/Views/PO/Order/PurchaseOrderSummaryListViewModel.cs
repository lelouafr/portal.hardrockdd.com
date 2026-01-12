using DB.Infrastructure.ViewPointDB.Data;
using portal.Models;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Order
{
    public class PurchaseOrderSummaryListViewModel
    {       

        public PurchaseOrderSummaryListViewModel()
        {
            List = new List<PurchaseOrderSummaryViewModel>();
        }

        //public PurchaseOrderSummaryListViewModel(WebUser user, DB.POListTypeEnum listType, DB.POStatusEnum status)
        //{
        //    if (user == null)
        //    {
        //        throw new System.ArgumentNullException(nameof(user));
        //    }
        //    switch (listType)
        //    {
        //        case DB.POListTypeEnum.User:
        //            List = user.PurchaseOrders.Where(f => f.Status == (int)status).Select(s => new PurchaseOrderSummaryViewModel(s)).ToList();
        //            break;
        //        case DB.POListTypeEnum.Assigned:
        //            List = user.AssignedPORequests.GroupBy(g => new { g.Request }).Where(f => f.Key.Request.Status == (int)status).Select(s => new PurchaseOrderSummaryViewModel(s.Key.Request)).ToList();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public PurchaseOrderSummaryListViewModel(WebUser user, bool openOnly = false)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var emp = user.Employee.FirstOrDefault();
            var empId = emp.HRRef.ToString(AppCultureInfo.CInfo());

            POCo = emp.HRCo;

            List = db.vPurchaseOrders.Where(f => f.OrderedBy == empId && f.Status == (openOnly ? 0 : f.Status)).AsEnumerable().Select(s => new PurchaseOrderSummaryViewModel(s)).ToList();
        }

        public PurchaseOrderSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, bool openOnly = false)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }

            POCo = company.HQCo;
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            List = db.vPurchaseOrders.Where(f => f.POCo == company.POCo && f.Status == (openOnly ? 0 : f.Status)).AsEnumerable().Select(s => new PurchaseOrderSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new PurchaseOrderSummaryViewModel(s)).ToList());
        }

        public PurchaseOrderSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor, bool openOnly = false)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            POCo = vendor.VendorGroupId;
            VendorId = vendor.VendorId;
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            List = db.vPurchaseOrders.Where(f => f.POCo == vendor.VendorGroupId && f.VendorId == vendor.VendorId && f.Status == (openOnly ? 0 : f.Status)).AsEnumerable().Select(s => new PurchaseOrderSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new PurchaseOrderSummaryViewModel(s)).ToList());
        }

        public PurchaseOrderSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, VPContext db, bool openOnly = false)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }
            if (db == null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            POCo = job.JCCo;
            JobId = job.JobId;
            var list = (from vPO in db.vPurchaseOrders
                    join PL in db.PurchaseOrderItems.Where(f => f.JobId == job.JobId).GroupBy(g => new { g.POCo, g.PO }) on
                    new { p1 = vPO.POCo, p2 = vPO.PO }
                    equals
                    new { p1 = PL.Key.POCo, p2 = PL.Key.PO }
                    select vPO).ToList();

            List = list.Select(s => new PurchaseOrderSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new PurchaseOrderSummaryViewModel(s)).ToList());
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [HiddenInput]
        [Display(Name = "VendorId")]
        public int? VendorId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "POCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        public List<PurchaseOrderSummaryViewModel> List { get; }
    }

    public class PurchaseOrderSummaryViewModel
    {

        public PurchaseOrderSummaryViewModel()
        {

        }


        public PurchaseOrderSummaryViewModel(vPurchaseOrder po)
        {
            if (po == null)
            {
                throw new System.ArgumentNullException(nameof(po));
            }
            POCo = po.POCo;
            KeyID = po.KeyID;
            OrderedDate = po.OrderDate ?? DateTime.Now;
            Status = (DB.POStatusEnum)po.Status;
            PO = po.PO;
            Description = po.Description;
            VendorId = po.VendorId;
            VendorName = po.VendorName;
            StatusString = Status.ToString();
            //CreatedBy = vticket.CreatedBy;
            Cost = po.Cost;
            //CreatedUser = new WebUserViewModel(po.OrderedBy);
            SearchString = po.SearchStr;
            ContainsErrors = false;
            ErrorMsg = string.Empty;
            CanProcess = false;
        }
        [HiddenInput]
        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }

        public string StatusString { get; set; }
        public string SearchString { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [UIHint("TextBox")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public long KeyID { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.POStatusEnum Status { get; set; }


        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderedDate { get; set; }
        
        public string OrderedDateString { get { return OrderedDate.ToShortDateString(); } }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }
               
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "POCo")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        
        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [ReadOnly(true)]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User Id")]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Created User")]
        public Web.WebUserViewModel CreatedUser { get; set; }


        [ReadOnly(true)]
        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Cost { get; set; }


        public bool ContainsErrors { get; set; }

        [UIHint("SwitchBoxGreen")]
        public bool CanProcess { get; set; }

        public string ErrorMsg { get; set; }
    }


}

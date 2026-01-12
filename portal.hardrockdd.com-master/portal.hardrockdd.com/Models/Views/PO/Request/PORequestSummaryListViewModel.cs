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

namespace portal.Models.Views.Purchase.Request
{
    public class PORequestSummaryListViewModel
    {       

        public PORequestSummaryListViewModel()
        {
            List = new List<PORequestSummaryViewModel>();
        }

        public PORequestSummaryListViewModel(WebUser user, DB.POListTypeEnum listType, DB.PORequestStatusEnum status)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            var db = VPContext.GetDbContextFromEntity(user);

            switch (listType)
            {
                case DB.POListTypeEnum.User:
                    List = user.PORequests.Where(f => f.Status == status).Select(s => new PORequestSummaryViewModel(s)).ToList();
                    break;
                case DB.POListTypeEnum.Assigned:
                    List = db.WorkFlowUsers
                        .Include("Sequence")
                        .Include("Sequence.WorkFlow")
                        .Where(f => f.AssignedTo == user.Id && f.Sequence.Active && f.Sequence.Status == (int)status)
                        .Select(s => s.Sequence.WorkFlow)
                        .Distinct()
                        .SelectMany(s => s.PORequests)
                        .ToList()
                        .Select(s => new PORequestSummaryViewModel(s)).ToList();
                    break;
                case DB.POListTypeEnum.All:
                    var comp = user.PREmployee.PRCompanyParm.HQCompanyParm.POCompanyParm;
                    List = comp.PORequests.Where(f => f.Status == status).Select(s => new PORequestSummaryViewModel(s)).ToList();
                    break;
                default:
                    break;
            }
        }

        public PORequestSummaryListViewModel(WebUser user, DB.POListTypeEnum listType)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }
            var db = VPContext.GetDbContextFromEntity(user);
            switch (listType)
            {
                case DB.POListTypeEnum.User:
                    List = user.PORequests.Where(f => f.Status != DB.PORequestStatusEnum.Canceled).Select(s => new PORequestSummaryViewModel(s)).ToList();
                    break;
                case DB.POListTypeEnum.Assigned:
                    //var wf = new List<WorkFlow>();
                    //var activeWorkFlowSeqs = db.WorkFlowUsers.Where(f => f.AssignedTo == user.Id).Select(s => s.Sequence).Where(f => f.Active).ToList();
                    //wf = activeWorkFlowSeqs.Select(s => s.WorkFlow).Distinct().ToList();

                    List = db.WorkFlowUsers
                        .Include("Sequence")
                        .Include("Sequence.WorkFlow")
                        .Where(f => f.AssignedTo == user.Id && f.Sequence.Active)
                        .Select(s => s.Sequence.WorkFlow)
                        .Distinct()
                        .SelectMany(s => s.PORequests)
                        .ToList()
                        .Select(s => new PORequestSummaryViewModel(s)).ToList();

                    //List = wf.SelectMany(s => s.PORequests).Select(s => new PORequestSummaryViewModel(s)).ToList();
                    //List = user.AssignedPORequests.Where(f => f.Request.Status != DB.PORequestStatusEnum.Canceled).Select(s => new PORequestSummaryViewModel(s.Request)).ToList();
                    break;
                default:
                    break;
            }
        }

        public PORequestSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DB.PORequestStatusEnum status)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            var poCompany = company.POCompanyParm;
            List = poCompany.PORequests.Where(f => f.Status == status).Select(s => new PORequestSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new PORequestSummaryViewModel(s)).ToList());
        }

        public PORequestSummaryListViewModel(List<PORequest> poList)
        {
            if (poList == null)
            {
                throw new System.ArgumentNullException(nameof(poList));
            }
            List = poList.Select(s => new PORequestSummaryViewModel(s)).ToList();
            //List.AddRange(company.PurchaseOrders.Select(s => new PORequestSummaryViewModel(s)).ToList());
        }
        public List<PORequestSummaryViewModel> List { get; }
    }

    public class PORequestSummaryViewModel
    {

        public PORequestSummaryViewModel()
        {

        }

        public PORequestSummaryViewModel(PORequest request) 
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            POCo = request.POCo;
            RequestId = request.RequestId;
            OrderedDate = request.OrderedDate;
            Status = request.Status;
            StatusString = Status.ToString();
            PO = request.PO;
            Description = request.Description;
            VendorId = request.VendorId;
            VendorName = request.Vendor?.Name;
            //CreatedBy = vticket.CreatedBy;
            Cost = request.Lines.Sum(sum => sum.Cost + (sum.TaxAmount ?? 0));
            CreatedUser = new WebUserViewModel(request.CreatedUser);

            ContainsErrors = false;
            ErrorMsg = string.Empty;
            if (request.VendorId == 999999 && request.Status == DB.PORequestStatusEnum.Approved)
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "New Vendor Selected";
            }
            if (request.Lines.Any(f => f.ItemTypeId == (byte)DB.POItemTypeEnum.Job && (f.JobId == null || f.PhaseId == null || f.JCCType == null)))
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "Job data missing";
            }
            if (request.Lines.Any(f => f.ItemTypeId == (byte)DB.POItemTypeEnum.Equipment && (f.EquipmentId == null || f.EMCType == null || f.CostCodeId == null)))
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "Equipment data missing";
            }
            if (request.Lines.Any(f => f.GLAcct == null))
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "GL Account is missing";
            }
            if (request.PO == null)
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "PO is missing";
            }
            if (request.Lines.Any(f => (f.TaxTypeId ?? 0) > 0 && f.TaxCodeId == null))
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "Tax Info is missing";
            }
            ErrorMsg = ErrorMsg.Trim();

            CanProcess = !ContainsErrors && Status == DB.PORequestStatusEnum.Approved;
        }



        public PORequestSummaryViewModel(PurchaseOrder po)
        {
            if (po == null)
            {
                throw new System.ArgumentNullException(nameof(po));
            }
            POCo = po.POCo;
            RequestId = (int)po.KeyID;
            OrderedDate = po.OrderDate ?? DateTime.Now;
            Status = (DB.PORequestStatusEnum)po.Status;
            PO = po.PO;
            Description = po.Description;
            VendorId = po.VendorId;
            VendorName = po.Vendor?.Name;
            //CreatedBy = vticket.CreatedBy;
            //Cost = po.Items.Sum(sum => sum.OrigCost + sum.OrigTax );
            //CreatedUser = new WebUserViewModel(po.OrderedBy);

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

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.PORequestStatusEnum Status { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderedDate { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }
               
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId")]
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

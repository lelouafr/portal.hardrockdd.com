using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.TermRequest
{
    public class TermRequestListViewModel
    {
        public TermRequestListViewModel()
        {

        }

        public TermRequestListViewModel(List<HRTermRequest> list)
        {

            List = list.Select(s => new TermRequestViewModel(s)).ToList();

        }

        public TermRequestListViewModel(WebUser user)
        {
            if (user == null)
                return;

            List = user.HRTermRequests
                .Select(s => new TermRequestViewModel(s)).ToList();

        }

        public TermRequestListViewModel(HRResource supervisor)
        {
            if (supervisor == null)
                return;
            List = supervisor
                .DirectReports
                .SelectMany(s => s.TermRequests.Where(f => f.Status != DB.HRTermRequestStatusEnum.Completed))
                .Select(s => new TermRequestViewModel(s)).ToList();
        }

        public TermRequestListViewModel(VPContext db)
        {
            if (db == null)
                return;

            var termList = db.HRTermRequests.Where(f => f.tStatusId != (int)DB.HRTermRequestStatusEnum.Completed).ToList();
            List = termList.Select(s => new TermRequestViewModel(s)).ToList();

        }


        public List<TermRequestViewModel> List { get; }
    }

    public class TermRequestViewModel
    {
        public TermRequestViewModel()
        {

        }

        public TermRequestViewModel(HRTermRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            HRRef = request.HRRef;
            Comments = request.Comments;
            RequestedOn = request.RequestedDate;
            RequestedBy = request.RequestedBy;
            Status = request.Status;
            PRComments = request.PRComments;
            HRComments = request.HRComments;
            TermCodeId = request.TermCodeId;
            TermDate = request.TermDate;
            EffectiveDate = request.EffectiveDate;
            PayrollEndDate = request.PayrollEndDate;
            IsOpenRequest = request.IsOpenRequest;

            if (request.RequestedUser != null)
                RequestedUser = request.RequestedUser.FullName();
            if (request.HRResource != null)
                EmployeeName = request.HRResource.FullName(true);

        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/HRCombo/TermEmployeeCombo")]
        public int? HRRef { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Comments { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Requested Date")]
        public System.DateTime RequestedOn { get; set; }


        [UIHint("DropdownBox")]
        [Display(Name = "Requestor")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo")]
        public string RequestedBy { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRTermRequestStatusEnum Status { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Payroll Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string PRComments { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "HR Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string HRComments { get; set; }
                
        [UIHint("DropdownBox")]
        [Display(Name = "Term Code")]
        [Field(ComboUrl = "/HRCombo/TermCodeTypeCombo")]
        public string TermCodeId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Term Date")]
        public DateTime? TermDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Payroll Date")]
        public DateTime? PayrollEndDate { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Rehire Position")]
        public bool IsOpenRequest { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Termed Employee")]
        public string EmployeeName { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Requested By")]
        public string RequestedUser { get; set; }

        internal TermRequestViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {

            var updObj = db.HRTermRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.HRRef = this.HRRef;
                updObj.Comments = this.Comments;
                updObj.Status = this.Status;
                updObj.PRComments = this.PRComments;
                updObj.HRComments = this.HRComments;
                updObj.TermCodeId = this.TermCodeId;
                updObj.TermDate = this.TermDate;
                updObj.EffectiveDate = this.EffectiveDate;
                updObj.PayrollEndDate = this.PayrollEndDate;
                updObj.IsOpenRequest = this.IsOpenRequest;

                try
                {
                    db.BulkSaveChanges();
                    return new TermRequestViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}
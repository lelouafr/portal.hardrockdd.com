using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.PositionRequest
{
    public class RequestListViewModel
    {
        public RequestListViewModel()
        {

        }

        public RequestListViewModel(WebUser user)
        {
            if (user == null)
                return;

            List = user.HRPositionRequests
                .Select(s => new RequestViewModel(s)).ToList();

        }

        public RequestListViewModel(VPContext db)
        {
            if (db == null)
                return;
            var termList = db.HRPositionRequests.ToList();//.Where(f => f.tStatusId != (int)DB.HRPositionRequestStatusEnum.Hire)
            List = termList.Select(s => new RequestViewModel(s)).ToList();

        }

        public RequestListViewModel(List<HRPositionRequest> list)
        {
            if (list == null)
                return;
            List = list.Select(s => new RequestViewModel(s)).ToList();

        }


        public List<RequestViewModel> List { get; }
    }

    public class RequestViewModel
    {
        public RequestViewModel()
        {

        }

        public RequestViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            OpenDate = request.OpenDate;
            PRCo = request.PRCo;

            Comments = request.Comments;
            RequestedOn = request.RequestedDate;
            RequestedBy = request.RequestedBy;
            RequestedByName = request.RequestedUser.FullName();
            Status = request.Status;
            PositionCodeId = request.PositionCodeId;
            ForCrewId = request.ForCrewId;
            PriorEmployeeId = request.tPriorEmployeeId;
            if (request.RequestedUser != null)
                RequestedUser = request.RequestedUser.FullName();

            if (request.HRPosition != null)
                PositionName = request.HRPosition.Description;

            if (request.ForCrew != null)
                CrewName = request.ForCrew.Description;

            if (request.PriorPREmployee != null)
                PriorEmployeeName = request.PriorPREmployee.FullName;

            ApplicantId = request.WAApplicantId;

            NewEmployeeId = request.NewEmployeeId;
        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int RequestId { get; set; }

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


        [UIHint("TextBox")]
        [Display(Name = "Requestor")]
        public string RequestedByName { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRPositionRequestStatusEnum Status { get; set; }
                
        [UIHint("DropdownBox")]
        [Display(Name = "Position Code")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        public string PositionCodeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string PositionName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Requested By")]
        public string RequestedUser { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string CrewName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Prior Employee")]
        public string PriorEmployeeName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Create Crew?")]
        public bool CreateCrew { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Open Date")]
        public System.DateTime OpenDate { get; set; }

        public byte? PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "For Crew")]
        [Field(ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        public string ForCrewId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Prior Employee")]
        [Field(ComboUrl = "/HRCombo/EmployeeCombo")]
        public int? PriorEmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "New Employee")]
        [Field(ComboUrl = "/HRCombo/EmployeeCombo")]
        public int? NewEmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Applicant")]
        [Field(ComboUrl = "/WACombo/WAApprovedAppicantCombo", ComboForeignKeys = "PositionCodeId")]
        public int? ApplicantId { get; set; }

        internal RequestViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.PositionCodeId = this.PositionCodeId;
                updObj.Comments = this.Comments;
                //updObj.Status = this.Status;
                updObj.ForCrewId = this.ForCrewId;
                updObj.PriorEmployeeId = this.PriorEmployeeId;
                updObj.NewEmployeeId = this.NewEmployeeId;
                //updObj.WAApplicantId = this.ApplicantId;
                try
                {
                    db.BulkSaveChanges();
                    return new RequestViewModel(updObj);
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
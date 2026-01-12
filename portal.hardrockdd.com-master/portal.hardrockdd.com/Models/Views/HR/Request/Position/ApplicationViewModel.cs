using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Position
{
    public class ApplicationListViewModel
    {
        public ApplicationListViewModel()
        {

        }

        public ApplicationListViewModel(Code.Data.VP.HRPositionRequest request)//HRPositionApplicant
        {
            if (request == null)
                return;

            var list = request.db.WebApplications.Where(f => f.tIsActive &&  f.AppliedPositions.Any(f => f.PositionCodeId == request.PositionCodeId)).ToList();

            List = request.WAApplicants.Select(s => new ApplicationViewModel(s)).ToList();
            list = list.Where(f => f.Status == WAApplicationStatusEnum.Approved && !List.Any(a => a.ApplicantId == f.ApplicantId && a.ApplicationId == f.ApplicationId)).ToList();

            List.AddRange(list.Select(s => new ApplicationViewModel(request, s)));

        }
        public List<ApplicationViewModel> List { get; }
    }

    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {

        }

        public ApplicationViewModel(Code.Data.VP.HRPositionApplicant applicant)
        {
            if (applicant == null)
                return;

            HRCo = applicant.HRCo;
            RequestId = applicant.RequestId;
            SeqId = applicant.SeqId;
            ApplicantId = applicant.ApplicantId;
            ApplicationId = applicant.ApplicationId;
            Status = applicant.Status;
            ApplicationDate = applicant.Application.ApplicationDate;
            ApplicantName = string.Format("{0} {1}", applicant.Application.Applicant.FirstName, applicant.Application.Applicant.LastName);

            ApprovedBy = applicant.ApprovedUser?.FullName();
            ApprovedOn = applicant.ApprovedOn;
        }

        public ApplicationViewModel(Code.Data.VP.HRPositionRequest request, Code.Data.VP.WebApplication application)
        {
            if (application == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            SeqId = 0;
            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;
            Status = HRPositionApplicantStatusEnum.Pending;
            ApplicationDate = application.ApplicationDate;
            ApplicantName = string.Format("{0} {1}", application.Applicant.FirstName, application.Applicant.LastName);
            ApprovedBy = null;
            ApprovedOn = null;
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        public int RequestId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Applicant #")]
        public int? ApplicantId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Application #")]
        public int? ApplicationId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Applicant")]
        public string ApplicantName { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public HRPositionApplicantStatusEnum Status { get; set; }

        public string ApprovedBy { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Approved On")]
        public DateTime? ApprovedOn { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public DateTime? ApplicationDate { get; set; }

        internal ApplicationViewModel ProcessUpdate(Code.Data.VP.VPEntities db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);
            var updObj = request.WAApplicants.FirstOrDefault(f => f.SeqId == this.SeqId);

            if (updObj != null)
            {
                updObj.Status = this.Status;
                
                try
                {
                    db.BulkSaveChanges();
                    return new ApplicationViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                var application = db.WebApplications.FirstOrDefault(f => f.ApplicantId == this.ApplicantId && f.ApplicationId == this.ApplicationId);
                if (application != null)
                {
                    updObj = request.AddApplication(application);
                    db.BulkSaveChanges();
                    return ProcessUpdate(db, modelState);
                }                
                return this;
            }
        }
    }
}
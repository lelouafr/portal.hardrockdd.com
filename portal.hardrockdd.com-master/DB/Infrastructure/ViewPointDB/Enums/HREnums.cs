using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace DB
{

    public enum HRAssestStatusEnum
    {
        Available,
        Assigned,
        Disposed
    }

    public enum HRActiveStatusEnum
    {
        Inactive,
        Active
    }

    public enum HRTermRequestStatusEnum
    {
        New,
        Submitted,
        Approved,
        PRReview,
        HRReview,
        [Display(Name = "Complete Term")]
        Completed,
        Canceled,
    }

    public enum HRPositionRequestStatusEnum
    {
        New,
        Submitted,
        [Display(Name = "HR Approved")]
        HRApproved,
        [Display(Name = "Mgnt Reviewed")]
        ManagementReviewed,
        [Display(Name = "HR Reviewed")]
        HRReview,
        [Display(Name = "Hired")]
        Hire,
        [Display(Name = "PR Reviewed")]
        PRReview,
        AssetReview,
        Canceled,
        Complete,
    }

    public enum HRPositionApplicantStatusEnum
    {
        Pending,
        Approved,
        Denied
    }

}
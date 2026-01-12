using System.ComponentModel.DataAnnotations;

namespace DB
{
    public enum CMLogEnum
    {
        Added,
        Updated,
        PictureAdded,
        AutoCoded,
        EmployeeCoded,
        Coded,
        Approved,
    }

    public enum CMPictureStatusEnum
    {
        Empty,
        Attached,
        Approved,
        Rejected,
        NotNeeded
    }

    public enum CMApprovalStatusEnum
    {
        New,
        EmployeeReviewed,
        SupervisorApproved,
        //Processed,
        //APPosted,
    }
    public enum CMTranStatusEnum
    {
        Open,
        Locked,        
        APPosted,
        [Display(Name = "Request Info")]
        RequestedInfomation
    }

    public enum CMTransCodeStatusEnum
    {
        Empty,
        [Display(Name = "Employee Coded")]
        EmployeeCoded,
        [Display(Name = "Admin Coded")]
        Coded,
        [Display(Name = "Auto Coded")]
        AutoCoded,
        [Display(Name = "Need Review")]
        NeedReview,
        Complete,
    }
    public enum CMCodeLineTypeEnum
    {
        Job = 1,
        Expense = 3,
        Equipment = 4,
    }
}
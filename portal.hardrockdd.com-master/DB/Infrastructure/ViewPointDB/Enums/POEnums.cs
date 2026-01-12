using System.ComponentModel.DataAnnotations;

namespace DB
{
    public enum POItemTypeEnum
    {
        Job = 1,
        Expense = 3,
        Equipment = 4,
    }
    public enum POCalcTypeEnum
    {
        [Display(Name = "Lump Sum")]
        LumpSum,
        Units,
    }

    public enum POListTypeEnum
    {
        User,
        Assigned,
        All
    }

    public enum PORequestStatusEnum
    {
        Open,
        Submitted,
        Approved,
        Rejected,
        Processed,
        Canceled,
        Reviewed,
        SupApproved,
        Change
    }

    public enum POStatusEnum
    {
        Open,
        Complete,
        Closed,
        Pending
    }
}
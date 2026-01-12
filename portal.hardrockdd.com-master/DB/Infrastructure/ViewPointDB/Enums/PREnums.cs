namespace DB
{
    public enum LeaveRequestStatusEnum
    {
        Open,
        Submitted,
        Approved,
        Rejected,
        Processed,
        Canceled,
        Reviewed,
        SupApproved,
    }

    public enum LeaveListTypeEnum
    {
        User,
        Assigned,
        All
    }

    public enum PayrollEntryStatusEnum
    {
        Accepted,
        Review,
        OnHold,
        Pending,
        Posted,
        Reversal,
    }

    public enum PayrollReviewStatusEnum
    {
        Empty,
        Pending,
        Approved,
        Posted
    }
    public enum PRCrewRequestStatusEnum
    {
        New,
        Submitted,
        Approved,
        Completed,
        Canceled,
    }
}
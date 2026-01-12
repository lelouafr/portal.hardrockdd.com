namespace DB
{
    public enum JCJobStatusEnum
    {
        Open,
        Scheduled,
        InProgress,
        PulledPipe,
        Completed,
        OnHold,
        Canceled,
        Invoiced
    }

    public enum JCJobListEnum
    {
        Open,
        All
    }

    public enum JCJobTypeEnum
    {
        UnAssigned,
        Job,
        ContractJob,
        ShopYard,
        CrewJobCost,
        Template,
        Other,
        Project,
    }
}
namespace DB
{
    public enum PMGanttSourceTypeEnum
    {
        Bid,
        BidPhase,
        BidBore,
        Project,
        Job,
        Phase,
        Daily,
        Ticket,
    }

    public enum PMGanttTaskTypeEnum
    {
        Project,
        Job,
        Actual,
        Projected,
    }

    public enum PMGAnttConstraintTypeEnum
    {
        ASAP,       // As Soon As Possible                
        ALAP,       // As Late As Possible                
        SNET,       // Start No Earlier Than                
        SNLT,       // Start No Later Than               
        FNET,       // Finish No Earlier Than               
        FNLT,       // Finish No Later Than                
        MSO,        // Must Start On                
        MFO         // Must Finish On
    }


    public enum PMLocateStatusEnum
    {
        New,
        Active,
        Expired,
        Closed,

        Import = 99
    }

    public enum PMLocateTypeEnum
    {
        State,
        Railroad
    }

    public enum PMRequestStatusEnum
    {
        New,
        Submitted,
        Pending,
        Completed,
        Canceled = 98,
        Import = 99
    }
}
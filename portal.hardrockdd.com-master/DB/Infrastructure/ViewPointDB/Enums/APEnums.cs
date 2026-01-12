namespace DB
{
    public enum APStatusEnum
    {
        Open,
        All
    }

    public enum APDocumentStatusEnum
    {
        New,
        Filed,
        Reviewed,
        LinesAdded,
        Processed,
        Duplicate,
        Canceled,
        All,
        Error,
        RequestedInfo
    }

    public enum APLineTypeEnum
    {
        Job = 1,
        Expense = 3,
        Equipment = 4,
        PO = 6,
    }


    public enum TaxTypeEnum
    {
        None,
        Sales,
        Use,
        Vat,
    }
}